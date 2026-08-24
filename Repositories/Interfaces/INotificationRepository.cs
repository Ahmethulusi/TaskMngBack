using TaskMngBack.Models;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetForUserAsync(int userId, int limit);
        Task<int> GetUnreadCountAsync(int userId);
        Task<Notification?> GetByIdAsync(Guid id);
        Task AddAsync(Notification notification);
        Task MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync(int userId);
        Task<bool> ExistsAsync(int userId, int taskId, string type);
    }
}
