using TaskMngBack.Models;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface IActivityLogRepository
    {
        Task<List<ActivityLog>> GetByTaskIdAsync(int taskId);
        Task<List<ActivityLog>> GetByTaskIdsAsync(List<int> taskIds, int limit);
        Task<List<ActivityLog>> GetCompletionLogsAsync(List<int> taskIds, DateTime since);
        Task AddAsync(ActivityLog activityLog);
    }
}
