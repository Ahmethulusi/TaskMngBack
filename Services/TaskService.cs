using TaskMngBack.DTOs.Labels;
using TaskMngBack.DTOs.Tasks;
using TaskMngBack.DTOs.Users;
using TaskMngBack.Exceptions;
using TaskMngBack.Models;
using TaskMngBack.Models.Enums;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITaskStatusRepository _taskStatusRepository;
        private readonly ILabelRepository _labelRepository;
        private readonly IActivityLogService _activityLogService;
        private readonly IProjectRepository _projectRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IStorageService _storageService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<TaskService> _logger;

        public TaskService(
            ITaskRepository taskRepository,
            IUserRepository userRepository,
            ITaskStatusRepository taskStatusRepository,
            ILabelRepository labelRepository,
            IActivityLogService activityLogService,
            IProjectRepository projectRepository,
            IDepartmentRepository departmentRepository,
            IAttachmentRepository attachmentRepository,
            IStorageService storageService,
            INotificationService notificationService,
            ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _taskStatusRepository = taskStatusRepository;
            _labelRepository = labelRepository;
            _activityLogService = activityLogService;
            _projectRepository = projectRepository;
            _departmentRepository = departmentRepository;
            _attachmentRepository = attachmentRepository;
            _storageService = storageService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<List<TaskDto>> GetAllForUser(int userId, List<string> permissions)
        {
            var tasks = permissions.Contains("tasks.view.all")
                ? await _taskRepository.GetAllAsync()
                : await _taskRepository.GetByUserAsync(userId);

            var taskDtos = tasks.Select(MapToDto).ToList();

            foreach (var taskDto in taskDtos)
            {
                taskDto.CommentCount = await _taskRepository.GetCommentCountAsync(taskDto.Id);
            }

            return taskDtos;
        }

        public async Task<TaskDto> GetByIdForUser(int taskId, int userId, List<string> permissions)
        {
            var task = await GetTaskOrThrowAsync(taskId);

            if (!permissions.Contains("tasks.view.all") &&
                task.CreatedByUserId != userId &&
                !task.AssignedUsers.Any(u => u.Id == userId) &&
                !await IsProjectMemberAsync(task, userId) &&
                !await IsDepartmentMemberAsync(task, userId))
            {
                throw new ForbiddenAccessException("Bu görevi görüntüleme yetkiniz yok.");
            }

            var taskDto = MapToDto(task);
            taskDto.CommentCount = await _taskRepository.GetCommentCountAsync(taskId);

            return taskDto;
        }

        public async Task<TaskDto> Create(CreateTaskDto dto, int userId, List<string> permissions)
        {
            var assignedUsers = await ResolveAssignedUsersAsync(dto.AssignedUserIds, userId, permissions);
            await ValidateParentTaskAsync(dto.ParentTaskId);

            var defaultStatus = await _taskStatusRepository.GetDefaultAsync();
            if (defaultStatus is null)
            {
                throw new InvalidOperationException("Varsayılan görev durumu bulunamadı. Lütfen sistem yöneticisine başvurun.");
            }

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = Enum.Parse<TaskPriority>(dto.Priority, ignoreCase: true),
                StatusId = defaultStatus.Id,
                DueDate = dto.DueDate,
                DepartmentId = dto.DepartmentId,
                ProjectId = dto.ProjectId,
                ParentTaskId = dto.ParentTaskId,
                CreatedByUserId = userId,
                AssignedUsers = assignedUsers,
                CreatedAt = DateTime.UtcNow
            };

            await _taskRepository.AddAsync(task);

            await _activityLogService.LogAsync(task.Id, userId, "Created", null, "Görev oluşturuldu");

            var created = await GetTaskOrThrowAsync(task.Id);
            var taskDto = MapToDto(created);
            taskDto.CommentCount = await _taskRepository.GetCommentCountAsync(created.Id);

            await NotifyNewlyAssignedUsersAsync(
                created.Title,
                created.Id,
                created.AssignedUsers.Select(u => u.Id),
                userId);

            return taskDto;
        }

        public async Task<TaskDto> Update(int taskId, UpdateTaskDto dto, int userId, List<string> permissions)
        {
            var task = await GetTaskOrThrowAsync(taskId);

            if (!permissions.Contains("tasks.update.all") && 
                task.CreatedByUserId != userId && 
                !await IsProjectOwnerAsync(task, userId) &&
                !await IsDepartmentManagerAsync(task, userId))
            {
                throw new ForbiddenAccessException("Bu görevi güncelleme yetkiniz yok.");
            }

            await ValidateParentTaskAsync(dto.ParentTaskId, task);

            var oldTitle = task.Title;
            var oldDescription = task.Description;
            var oldPriority = task.Priority;
            var oldDueDate = task.DueDate;
            var oldDepartmentId = task.DepartmentId;
            var oldDepartmentName = task.Department?.Name;
            var oldProjectId = task.ProjectId;
            var oldProjectName = task.Project?.Name;

            var newPriority = Enum.Parse<TaskPriority>(dto.Priority, ignoreCase: true);

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Priority = newPriority;
            task.DueDate = dto.DueDate;
            task.DepartmentId = dto.DepartmentId;
            task.ProjectId = dto.ProjectId;
            task.ParentTaskId = dto.ParentTaskId;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            if (oldTitle != dto.Title)
            {
                await _activityLogService.LogAsync(taskId, userId, "Title", oldTitle, dto.Title);
            }

            if (oldDescription != dto.Description)
            {
                await _activityLogService.LogAsync(taskId, userId, "Description", 
                    "Açıklama güncellendi", "Açıklama güncellendi");
            }

            if (oldPriority != newPriority)
            {
                await _activityLogService.LogAsync(taskId, userId, "Priority", 
                    oldPriority.ToString(), newPriority.ToString());
            }

            if (oldDueDate != dto.DueDate)
            {
                var oldValue = oldDueDate?.ToString("dd.MM.yyyy");
                var newValue = dto.DueDate?.ToString("dd.MM.yyyy");
                await _activityLogService.LogAsync(taskId, userId, "DueDate", oldValue, newValue);
            }

            if (oldDepartmentId != dto.DepartmentId)
            {
                var newDepartmentName = dto.DepartmentId.HasValue 
                    ? (await _taskRepository.GetByIdAsync(taskId))?.Department?.Name 
                    : null;
                await _activityLogService.LogAsync(taskId, userId, "Department", 
                    oldDepartmentName, newDepartmentName);
            }

            if (oldProjectId != dto.ProjectId)
            {
                var newProjectName = dto.ProjectId.HasValue 
                    ? (await _taskRepository.GetByIdAsync(taskId))?.Project?.Name 
                    : null;
                await _activityLogService.LogAsync(taskId, userId, "Project", 
                    oldProjectName, newProjectName);
            }

            var taskDto = MapToDto(task);
            taskDto.CommentCount = await _taskRepository.GetCommentCountAsync(taskId);

            return taskDto;
        }

        public async Task<TaskDto> UpdateStatus(int taskId, UpdateTaskStatusDto dto, int userId, List<string> permissions)
        {
            var task = await GetTaskOrThrowAsync(taskId);

            if (!permissions.Contains("tasks.update.all") &&
                task.CreatedByUserId != userId &&
                !task.AssignedUsers.Any(u => u.Id == userId) &&
                !await IsProjectOwnerAsync(task, userId) &&
                !await IsDepartmentManagerAsync(task, userId))
            {
                throw new ForbiddenAccessException("Bu görevin durumunu güncelleme yetkiniz yok.");
            }

            var status = await _taskStatusRepository.GetByIdAsync(dto.StatusId);
            if (status is null)
            {
                throw new NotFoundException($"Id'si {dto.StatusId} olan durum bulunamadı.");
            }

            var oldStatusName = task.StatusDefinition?.Name;

            task.StatusId = dto.StatusId;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            await _activityLogService.LogAsync(taskId, userId, "Status", oldStatusName, status.Name);

            var taskDto = MapToDto(task);
            taskDto.CommentCount = await _taskRepository.GetCommentCountAsync(taskId);

            return taskDto;
        }

        public async Task<TaskDto> UpdateLabels(int taskId, UpdateTaskLabelsDto dto, int userId, List<string> permissions)
        {
            var task = await GetTaskOrThrowAsync(taskId);

            if (!permissions.Contains("tasks.update.all") &&
                task.CreatedByUserId != userId &&
                !task.AssignedUsers.Any(u => u.Id == userId) &&
                !await IsProjectOwnerAsync(task, userId))
            {
                throw new ForbiddenAccessException("Bu görevin etiketlerini güncelleme yetkiniz yok.");
            }

            var oldLabelNames = task.Labels.Any() 
                ? string.Join(", ", task.Labels.Select(l => l.Name))
                : "Kimse yok";

            var labels = await ResolveLabelsAsync(dto.LabelIds);

            task.Labels = labels;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            var newLabelNames = labels.Any() 
                ? string.Join(", ", labels.Select(l => l.Name))
                : "Kimse yok";

            await _activityLogService.LogAsync(taskId, userId, "Labels", 
                oldLabelNames, newLabelNames);

            var updated = await GetTaskOrThrowAsync(taskId);
            var taskDto = MapToDto(updated);
            taskDto.CommentCount = await _taskRepository.GetCommentCountAsync(taskId);

            return taskDto;
        }

        public async Task Delete(int taskId, int userId, List<string> permissions)
        {
            var task = await GetTaskOrThrowAsync(taskId);

            if (!permissions.Contains("tasks.delete.all") && 
                task.CreatedByUserId != userId && 
                !await IsProjectOwnerAsync(task, userId) &&
                !await IsDepartmentManagerAsync(task, userId))
            {
                throw new ForbiddenAccessException("Bu görevi silme yetkiniz yok.");
            }

            var attachments = await _attachmentRepository.GetByTaskIdIncludingCommentsAsync(taskId);
            foreach (var attachment in attachments)
            {
                try
                {
                    await _storageService.DeleteObjectAsync(attachment.StorageKey);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "R2'den silinemedi: {StorageKey}", attachment.StorageKey);
                }
            }

            await _taskRepository.DeleteAsync(task);
        }

        public async Task<TaskDto> AssignTask(int taskId, List<int> assignedUserIds, int userId, List<string> permissions)
        {
            var task = await GetTaskOrThrowAsync(taskId);

            var canAssign = permissions.Contains("tasks.assign") || 
                           await IsProjectOwnerAsync(task, userId) ||
                           await IsDepartmentManagerAsync(task, userId);
            if (!canAssign)
            {
                throw new ForbiddenAccessException("Bu göreve kullanıcı atama yetkiniz yok.");
            }
            
            var previousAssignedIds = task.AssignedUsers.Select(u => u.Id).ToHashSet();

            var oldAssignedNames = task.AssignedUsers.Any() 
                ? string.Join(", ", task.AssignedUsers.Select(u => u.FullName))
                : "Kimse yok";

            var assignedUsers = await ResolveAssignedUsersAsync(assignedUserIds, userId: 0, new List<string> { "tasks.assign" });

            task.AssignedUsers = assignedUsers;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            var newAssignedNames = assignedUsers.Any() 
                ? string.Join(", ", assignedUsers.Select(u => u.FullName))
                : "Kimse yok";

            await _activityLogService.LogAsync(taskId, userId, "AssignedUsers", 
                oldAssignedNames, newAssignedNames);

            var updated = await GetTaskOrThrowAsync(taskId);
            var taskDto = MapToDto(updated);
            taskDto.CommentCount = await _taskRepository.GetCommentCountAsync(taskId);

            await NotifyNewlyAssignedUsersAsync(
                updated.Title,
                updated.Id,
                assignedUsers.Select(u => u.Id).Where(id => !previousAssignedIds.Contains(id)),
                userId);

            return taskDto;
        }

        public async Task AddDependency(
            int taskId,
            AddDependencyDto dto,
            int userId,
            List<string> permissions)
        {
            var task = await GetTaskOrThrowAsync(taskId);
            await EnsureCanUpdateTaskAsync(task, userId, permissions);

            if (taskId == dto.DependsOnTaskId)
            {
                throw new BadRequestException("Bir görev kendisine bağımlı olamaz.");
            }

            await GetTaskOrThrowAsync(dto.DependsOnTaskId);

            if (await _taskRepository.GetDependencyAsync(taskId, dto.DependsOnTaskId) is not null)
            {
                throw new ConflictException("Bu bağımlılık zaten mevcut.");
            }

            if (await _taskRepository.WouldCreateCycleAsync(taskId, dto.DependsOnTaskId))
            {
                throw new BadRequestException("Bu bağımlılık döngüsel bir ilişki oluşturur.");
            }

            await _taskRepository.AddAsync(new TaskDependency
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                DependsOnTaskId = dto.DependsOnTaskId,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task RemoveDependency(
            int taskId,
            int dependsOnTaskId,
            int userId,
            List<string> permissions)
        {
            var task = await GetTaskOrThrowAsync(taskId);
            await EnsureCanUpdateTaskAsync(task, userId, permissions);

            var dependency = await _taskRepository.GetDependencyAsync(taskId, dependsOnTaskId);
            if (dependency is null)
            {
                throw new NotFoundException("Belirtilen görev bağımlılığı bulunamadı.");
            }

            await _taskRepository.DeleteAsync(dependency);
        }

        private async Task<List<User>> ResolveAssignedUsersAsync(List<int> assignedUserIds, int userId, List<string> permissions)
        {
            var distinctIds = assignedUserIds.Distinct().ToList();

            if (!permissions.Contains("tasks.assign"))
            {
                if (distinctIds.Count > 1 ||
                    (distinctIds.Count == 1 && distinctIds[0] != userId))
                {
                    throw new ForbiddenAccessException(
                        "Görevi sadece kendinize atayabilir ya da boş bırakabilirsiniz.");
                }
            }

            if (distinctIds.Count == 0)
            {
                return new List<User>();
            }

            var users = await _userRepository.GetByIdsAsync(distinctIds);

            if (users.Count != distinctIds.Count)
            {
                throw new NotFoundException("Atanacak kullanıcılardan biri veya birkaçı bulunamadı.");
            }

            return users;
        }

        private async Task<List<Label>> ResolveLabelsAsync(List<Guid> labelIds)
        {
            var distinctIds = labelIds.Distinct().ToList();

            if (distinctIds.Count == 0)
            {
                return new List<Label>();
            }

            var labels = await _labelRepository.GetByIdsAsync(distinctIds);

            if (labels.Count != distinctIds.Count)
            {
                throw new NotFoundException("Belirtilen etiketlerden biri veya birkaçı bulunamadı.");
            }

            return labels;
        }

        private async Task<TaskItem> GetTaskOrThrowAsync(int taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);

            if (task is null)
            {
                throw new NotFoundException($"Id'si {taskId} olan görev bulunamadı.");
            }

            return task;
        }

        private async Task ValidateParentTaskAsync(int? parentTaskId, TaskItem? currentTask = null)
        {
            if (!parentTaskId.HasValue)
            {
                return;
            }

            if (currentTask?.Id == parentTaskId.Value)
            {
                throw new BadRequestException("Bir görev kendisinin alt görevi olamaz.");
            }

            var parentTask = await _taskRepository.GetByIdAsync(parentTaskId.Value);
            if (parentTask is null)
            {
                throw new NotFoundException($"Id'si {parentTaskId.Value} olan üst görev bulunamadı.");
            }

            if (parentTask.ParentTaskId.HasValue || currentTask?.Subtasks.Count > 0)
            {
                throw new BadRequestException("Bir alt görevin alt görevi olamaz.");
            }
        }

        private async Task NotifyNewlyAssignedUsersAsync(
            string taskTitle,
            int taskId,
            IEnumerable<int> newlyAssignedUserIds,
            int actorUserId)
        {
            foreach (var assignedUserId in newlyAssignedUserIds.Where(id => id != actorUserId))
            {
                await _notificationService.NotifyAsync(
                    assignedUserId,
                    "TaskAssigned",
                    "Yeni görev ataması",
                    $"'{taskTitle}' görevine atandınız.",
                    taskId);
            }
        }

        private async Task EnsureCanUpdateTaskAsync(TaskItem task, int userId, List<string> permissions)
        {
            if (!permissions.Contains("tasks.update.all") &&
                task.CreatedByUserId != userId &&
                !await IsProjectOwnerAsync(task, userId) &&
                !await IsDepartmentManagerAsync(task, userId))
            {
                throw new ForbiddenAccessException("Bu görevi güncelleme yetkiniz yok.");
            }
        }

        private async Task<bool> IsProjectOwnerAsync(TaskItem task, int userId)
        {
            var role = await _projectRepository.GetMemberRoleAsync(task.ProjectId, userId);
            return role == ProjectMemberRole.Owner;
        }

        private async Task<bool> IsProjectMemberAsync(TaskItem task, int userId)
        {
            var role = await _projectRepository.GetMemberRoleAsync(task.ProjectId, userId);
            return role != null;
        }

        private async Task<bool> IsDepartmentManagerAsync(TaskItem task, int userId)
        {
            if (task.DepartmentId == null) return false;
            var department = await _departmentRepository.GetByIdAsync(task.DepartmentId.Value);
            return department?.ManagerId == userId;
        }

        private async Task<bool> IsDepartmentMemberAsync(TaskItem task, int userId)
        {
            if (task.DepartmentId == null) return false;
            var department = await _departmentRepository.GetByIdAsync(task.DepartmentId.Value);
            return department?.Users.Any(u => u.Id == userId) ?? false;
        }

        private static TaskDto MapToDto(TaskItem task)
        {
            var blockedBy = task.Dependencies
                .Select(d => new TaskDependencyDto
                {
                    TaskId = d.DependsOnTaskId,
                    TaskTitle = d.DependsOnTask?.Title ?? string.Empty,
                    StatusName = d.DependsOnTask?.StatusDefinition?.Name ?? string.Empty,
                    StatusColorKey = d.DependsOnTask?.StatusDefinition?.ColorKey ?? string.Empty,
                    IsCompletionStatus = d.DependsOnTask?.StatusDefinition?.IsCompletionStatus ?? false
                })
                .ToList();

            var blocks = task.Blocking
                .Select(d => new TaskDependencyDto
                {
                    TaskId = d.TaskId,
                    TaskTitle = d.Task?.Title ?? string.Empty,
                    StatusName = d.Task?.StatusDefinition?.Name ?? string.Empty,
                    StatusColorKey = d.Task?.StatusDefinition?.ColorKey ?? string.Empty,
                    IsCompletionStatus = d.Task?.StatusDefinition?.IsCompletionStatus ?? false
                })
                .ToList();

            var isOverdue = task.DueDate.HasValue
                && task.DueDate.Value.Date < DateTime.UtcNow.Date
                && !task.StatusDefinition.IsCompletionStatus;

            string? dueUrgency = null;
            if (task.DueDate.HasValue && !task.StatusDefinition.IsCompletionStatus && !isOverdue)
            {
                var daysUntilDue = (task.DueDate.Value.Date - DateTime.UtcNow.Date).Days;
                if (daysUntilDue == 1)
                    dueUrgency = "Tomorrow";
                else if (daysUntilDue >= 0 && daysUntilDue <= 7)
                    dueUrgency = "Soon";
            }

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                StatusId = task.StatusId,
                StatusName = task.StatusDefinition?.Name ?? string.Empty,
                StatusColorKey = task.StatusDefinition?.ColorKey ?? string.Empty,
                IsCompletionStatus = task.StatusDefinition?.IsCompletionStatus ?? false,
                Priority = task.Priority.ToString(),
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                DueDate = task.DueDate,
                IsOverdue = isOverdue,
                DueUrgency = dueUrgency,
                DepartmentId = task.DepartmentId,
                DepartmentName = task.Department?.Name,
                ProjectId = task.ProjectId,
                ProjectName = task.Project?.Name,
                ParentTaskId = task.ParentTaskId,
                ParentTaskTitle = task.ParentTask?.Title,
                Subtasks = task.Subtasks
                    .Select(s => new SubtaskSummaryDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        StatusName = s.StatusDefinition?.Name ?? string.Empty,
                        StatusColorKey = s.StatusDefinition?.ColorKey ?? string.Empty,
                        IsCompletionStatus = s.StatusDefinition?.IsCompletionStatus ?? false
                    })
                    .ToList(),
                SubtaskProgress = task.ParentTaskId == null && task.Subtasks.Count > 0
                    ? new SubtaskProgressDto
                    {
                        TotalCount = task.Subtasks.Count,
                        CompletedCount = task.Subtasks.Count(s =>
                            s.StatusDefinition?.IsCompletionStatus == true)
                    }
                    : null,
                CreatedByUserId = task.CreatedByUserId,
                CreatedByUserName = task.CreatedByUser?.FullName ?? string.Empty,
                AssignedUsers = task.AssignedUsers
                    .Select(u => new UserSummaryDto
                    {
                        Id = u.Id,
                        FullName = u.FullName
                    })
                    .ToList(),
                Labels = task.Labels
                    .Select(l => new LabelDto
                    {
                        Id = l.Id,
                        Name = l.Name
                    })
                    .ToList(),
                BlockedBy = blockedBy,
                Blocks = blocks,
                IsBlocked = blockedBy.Any(d => !d.IsCompletionStatus),
                AttachmentCount = task.Attachments.Count(a => a.CommentId == null)
            };
        }
    }
}
