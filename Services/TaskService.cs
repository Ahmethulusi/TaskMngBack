using TaskManager_Staj_Project.DTOs.Tasks;
using TaskManager_Staj_Project.Exceptions;
using TaskManager_Staj_Project.Models;
using TaskManager_Staj_Project.Models.Enums;
using TaskManager_Staj_Project.Repositories.Interfaces;
using TaskManager_Staj_Project.Services.Interfaces;

namespace TaskManager_Staj_Project.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;

        public TaskService(ITaskRepository taskRepository, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
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

            if (!isAdmin && task.CreatedByUserId != userId && task.AssignedToUserId != userId)
            {
                throw new ForbiddenAccessException("Bu görevi görüntüleme yetkiniz yok.");
            }

            return MapToDto(task);
        }

        public async Task<TaskDto> Create(CreateTaskDto dto, int userId, bool isAdmin)
        {
            if (!isAdmin && dto.AssignedToUserId.HasValue && dto.AssignedToUserId.Value != userId)
            {
                throw new ForbiddenAccessException("Görevi sadece kendinize atayabilir ya da boş bırakabilirsiniz.");
            }

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = Enum.Parse<TaskPriority>(dto.Priority, ignoreCase: true),
                Status = TaskItemStatus.Bekliyor,
                DepartmentId = dto.DepartmentId,
                CreatedByUserId = userId,
                AssignedToUserId = dto.AssignedToUserId,
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
            task.DepartmentId = dto.DepartmentId;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            return MapToDto(task);
        }

        public async Task<TaskDto> UpdateStatus(int taskId, UpdateTaskStatusDto dto, int userId, bool isAdmin)
        {
            var task = await GetTaskOrThrowAsync(taskId);

            if (!isAdmin && task.CreatedByUserId != userId && task.AssignedToUserId != userId)
            {
                throw new ForbiddenAccessException("Bu görevin durumunu güncelleme yetkiniz yok.");
            }

            task.Status = Enum.Parse<TaskItemStatus>(dto.Status, ignoreCase: true);
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            return MapToDto(task);
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

        public async Task<TaskDto> AssignTask(int taskId, int? assignedToUserId)
        {
            var task = await GetTaskOrThrowAsync(taskId);

            if (assignedToUserId.HasValue)
            {
                var assignee = await _userRepository.GetByIdAsync(assignedToUserId.Value);
                if (assignee is null)
                {
                    throw new NotFoundException("Atanacak kullanıcı bulunamadı.");
                }
            }

            task.AssignedToUserId = assignedToUserId;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            var updated = await GetTaskOrThrowAsync(taskId);
            return MapToDto(updated);
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
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                DepartmentId = task.DepartmentId,
                DepartmentName = task.Department?.Name,
                CreatedByUserId = task.CreatedByUserId,
                CreatedByUserName = task.CreatedByUser?.FullName ?? string.Empty,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = task.AssignedToUser?.FullName
            };
        }
    }
}
