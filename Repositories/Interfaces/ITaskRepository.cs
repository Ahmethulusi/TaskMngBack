using TaskMngBack.Models;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetAllAsync();
        Task<List<TaskItem>> GetByUserAsync(int userId);
        Task<TaskItem?> GetByIdAsync(int id);
        Task AddAsync(TaskItem task);
        Task AddAsync(TaskDependency dependency);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(TaskItem task);
        Task DeleteAsync(TaskDependency dependency);
        Task<TaskDependency?> GetDependencyAsync(int taskId, int dependsOnTaskId);
        Task<bool> WouldCreateCycleAsync(int taskId, int dependsOnTaskId);
        Task<bool> HasTasksForUserAsync(int userId);
        Task<bool> HasTasksForStatusAsync(Guid statusId);
        Task<int> GetCommentCountAsync(int taskId);
    }
}
