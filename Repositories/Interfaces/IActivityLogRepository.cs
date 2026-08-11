using TaskMngBack.Models;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface IActivityLogRepository
    {
        Task<List<ActivityLog>> GetByTaskIdAsync(int taskId);
        Task AddAsync(ActivityLog activityLog);
    }
}
