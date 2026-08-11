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

        public async Task AddAsync(ActivityLog activityLog)
        {
            await _context.ActivityLogs.AddAsync(activityLog);
            await _context.SaveChangesAsync();
        }
    }
}
