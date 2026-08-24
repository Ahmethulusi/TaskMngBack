using Microsoft.EntityFrameworkCore;
using TaskMngBack.Data;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;

namespace TaskMngBack.Repositories
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly AppDbContext _context;

        public ActivityLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<ActivityLog>> GetByTaskIdAsync(int taskId)
        {
            return _context.ActivityLogs
                .Include(a => a.User)
                .Where(a => a.TaskId == taskId)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();
        }

        public Task<List<ActivityLog>> GetByTaskIdsAsync(List<int> taskIds, int limit)
        {
            return _context.ActivityLogs
                .Include(a => a.User)
                .Include(a => a.Task)
                .Where(a => taskIds.Contains(a.TaskId))
                .OrderByDescending(a => a.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public Task<List<ActivityLog>> GetCompletionLogsAsync(List<int> taskIds, DateTime since)
        {
            return _context.ActivityLogs
                .Where(a => taskIds.Contains(a.TaskId)
                    && a.FieldName == "Status"
                    && a.CreatedAt >= since)
                .ToListAsync();
        }

        public async Task AddAsync(ActivityLog activityLog)
        {
            await _context.ActivityLogs.AddAsync(activityLog);
            await _context.SaveChangesAsync();
        }
    }
}
