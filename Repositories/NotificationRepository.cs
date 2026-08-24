using Microsoft.EntityFrameworkCore;
using TaskMngBack.Data;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;

namespace TaskMngBack.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Notification>> GetForUserAsync(int userId, int limit)
        {
            return _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public Task<int> GetUnreadCountAsync(int userId)
        {
            return _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public Task<Notification?> GetByIdAsync(Guid id)
        {
            return _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var notification = await GetByIdAsync(id);
            if (notification is null)
            {
                return;
            }

            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }

        public Task MarkAllAsReadAsync(int userId)
        {
            return _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }

        public Task<bool> ExistsAsync(int userId, int taskId, string type)
        {
            return _context.Notifications.AnyAsync(n =>
                n.UserId == userId &&
                n.RelatedTaskId == taskId &&
                n.Type == type);
        }
    }
}
