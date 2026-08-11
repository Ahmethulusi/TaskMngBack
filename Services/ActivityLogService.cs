using TaskMngBack.DTOs.Activity;
using TaskMngBack.Exceptions;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly ITaskRepository _taskRepository;

        public ActivityLogService(
            IActivityLogRepository activityLogRepository,
            ITaskRepository taskRepository)
        {
            _activityLogRepository = activityLogRepository;
            _taskRepository = taskRepository;
        }

        public async Task LogAsync(int taskId, int userId, string fieldName, string? oldValue, string? newValue)
        {
            var activityLog = new ActivityLog
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = userId,
                FieldName = fieldName,
                OldValue = oldValue,
                NewValue = newValue,
                CreatedAt = DateTime.UtcNow
            };

            await _activityLogRepository.AddAsync(activityLog);
        }

        public async Task<List<ActivityLogDto>> GetByTaskIdAsync(int taskId, int userId, bool isAdmin)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);

            if (task is null)
            {
                throw new NotFoundException($"Id'si {taskId} olan görev bulunamadı.");
            }

            if (!isAdmin &&
                task.CreatedByUserId != userId &&
                !task.AssignedUsers.Any(u => u.Id == userId))
            {
                throw new ForbiddenAccessException("Bu görevin aktivite loglarını görüntüleme yetkiniz yok.");
            }

            var activityLogs = await _activityLogRepository.GetByTaskIdAsync(taskId);
            return activityLogs.Select(MapToDto).ToList();
        }

        private static ActivityLogDto MapToDto(ActivityLog activityLog)
        {
            return new ActivityLogDto
            {
                Id = activityLog.Id,
                UserId = activityLog.UserId,
                UserFullName = activityLog.User?.FullName ?? string.Empty,
                FieldName = activityLog.FieldName,
                OldValue = activityLog.OldValue,
                NewValue = activityLog.NewValue,
                CreatedAt = activityLog.CreatedAt
            };
        }
    }
}
