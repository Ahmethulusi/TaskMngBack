using TaskMngBack.DTOs.Notifications;
using TaskMngBack.Exceptions;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task NotifyAsync(int userId, string type, string title, string message, int? relatedTaskId)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = type,
                Title = title,
                Message = message,
                RelatedTaskId = relatedTaskId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
        }

        public async Task<List<NotificationDto>> GetForUserAsync(int userId)
        {
            var notifications = await _notificationRepository.GetForUserAsync(userId, 50);
            return notifications.Select(MapToDto).ToList();
        }

        public Task<int> GetUnreadCountAsync(int userId)
        {
            return _notificationRepository.GetUnreadCountAsync(userId);
        }

        public async Task MarkAsReadAsync(Guid id, int userId)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification is null)
            {
                throw new NotFoundException($"Id'si {id} olan bildirim bulunamadı.");
            }

            if (notification.UserId != userId)
            {
                throw new ForbiddenAccessException("Başkasının bildirimini okundu olarak işaretleyemezsiniz.");
            }

            await _notificationRepository.MarkAsReadAsync(id);
        }

        public Task MarkAllAsReadAsync(int userId)
        {
            return _notificationRepository.MarkAllAsReadAsync(userId);
        }

        private static NotificationDto MapToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                RelatedTaskId = notification.RelatedTaskId,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }
    }
}
