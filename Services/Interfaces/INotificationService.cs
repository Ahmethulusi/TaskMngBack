using TaskMngBack.DTOs.Notifications;

namespace TaskMngBack.Services.Interfaces
{
    public interface INotificationService
    {
        Task NotifyAsync(int userId, string type, string title, string message, int? relatedTaskId);
        Task<List<NotificationDto>> GetForUserAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task MarkAsReadAsync(Guid id, int userId);
        Task MarkAllAsReadAsync(int userId);
    }
}
