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

        public TaskService(
            ITaskRepository taskRepository,
            IUserRepository userRepository,
            ITaskStatusRepository taskStatusRepository,
            ILabelRepository labelRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _taskStatusRepository = taskStatusRepository;
            _labelRepository = labelRepository;
        }

        public async Task<List<TaskDto>> GetAllForUser(int userId, bool isAdmin)
        {
            var tasks = isAdmin
                ? await _taskRepository.GetAllAsync()
                : await _taskRepository.GetByUserAsync(userId);

            return tasks.Select(MapToDto).ToList();
        }

        public async Task<TaskDto> GetByIdForUser(int taskId, int userId, bool isAdmin)
        {
            var task = await GetTaskOrThrowAsync(taskId);

            if (!isAdmin &&
                task.CreatedByUserId != userId &&
                !task.AssignedUsers.Any(u => u.Id == userId))
            {
                throw new ForbiddenAccessException("Bu görevi görüntüleme yetkiniz yok.");
            }

            return MapToDto(task);
        }

        public async Task<TaskDto> Create(CreateTaskDto dto, int userId, bool isAdmin)
        {
            var assignedUsers = await ResolveAssignedUsersAsync(dto.AssignedUserIds, userId, isAdmin);

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
                CreatedByUserId = userId,
                AssignedUsers = assignedUsers,
                CreatedAt = DateTime.UtcNow
            };

            await _taskRepository.AddAsync(task);

            var created = await GetTaskOrThrowAsync(task.Id);
            return MapToDto(created);
        }

        public async Task<TaskDto> Update(int taskId, UpdateTaskDto dto, int userId, bool isAdmin)
        {
            var task = await GetTaskOrThrowAsync(taskId);

            if (!isAdmin && task.CreatedByUserId != userId)
            {
                throw new ForbiddenAccessException("Bu görevi güncelleme yetkiniz yok.");
            }

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Priority = Enum.Parse<TaskPriority>(dto.Priority, ignoreCase: true);
            task.DueDate = dto.DueDate;
            task.DepartmentId = dto.DepartmentId;
            task.ProjectId = dto.ProjectId;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            return MapToDto(task);
        }

        public async Task<TaskDto> UpdateStatus(int taskId, UpdateTaskStatusDto dto, int userId, bool isAdmin)
        {
            var task = await GetTaskOrThrowAsync(taskId);

            if (!isAdmin &&
                task.CreatedByUserId != userId &&
                !task.AssignedUsers.Any(u => u.Id == userId))
            {
                throw new ForbiddenAccessException("Bu görevin durumunu güncelleme yetkiniz yok.");
            }

            var status = await _taskStatusRepository.GetByIdAsync(dto.StatusId);
            if (status is null)
            {
                throw new NotFoundException($"Id'si {dto.StatusId} olan durum bulunamadı.");
            }

            task.StatusId = dto.StatusId;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            return MapToDto(task);
        }

        public async Task<TaskDto> UpdateLabels(int taskId, UpdateTaskLabelsDto dto, int userId, bool isAdmin)
        {
            var task = await GetTaskOrThrowAsync(taskId);

            if (!isAdmin &&
                task.CreatedByUserId != userId &&
                !task.AssignedUsers.Any(u => u.Id == userId))
            {
                throw new ForbiddenAccessException("Bu görevin etiketlerini güncelleme yetkiniz yok.");
            }

            var labels = await ResolveLabelsAsync(dto.LabelIds);

            task.Labels = labels;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            var updated = await GetTaskOrThrowAsync(taskId);
            return MapToDto(updated);
        }

        public async Task Delete(int taskId, int userId, bool isAdmin)
        {
            var task = await GetTaskOrThrowAsync(taskId);

            if (!isAdmin && task.CreatedByUserId != userId)
            {
                throw new ForbiddenAccessException("Bu görevi silme yetkiniz yok.");
            }

            await _taskRepository.DeleteAsync(task);
        }

        public async Task<TaskDto> AssignTask(int taskId, List<int> assignedUserIds)
        {
            var task = await GetTaskOrThrowAsync(taskId);
            var assignedUsers = await ResolveAssignedUsersAsync(assignedUserIds, userId: 0, isAdmin: true);

            task.AssignedUsers = assignedUsers;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            var updated = await GetTaskOrThrowAsync(taskId);
            return MapToDto(updated);
        }

        private async Task<List<User>> ResolveAssignedUsersAsync(List<int> assignedUserIds, int userId, bool isAdmin)
        {
            var distinctIds = assignedUserIds.Distinct().ToList();

            if (!isAdmin)
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

        private static TaskDto MapToDto(TaskItem task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                StatusId = task.StatusId,
                StatusName = task.StatusDefinition?.Name ?? string.Empty,
                StatusColorKey = task.StatusDefinition?.ColorKey ?? string.Empty,
                Priority = task.Priority.ToString(),
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                DueDate = task.DueDate,
                DepartmentId = task.DepartmentId,
                DepartmentName = task.Department?.Name,
                ProjectId = task.ProjectId,
                ProjectName = task.Project?.Name,
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
                    .ToList()
            };
        }
    }
}
