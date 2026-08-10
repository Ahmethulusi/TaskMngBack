using TaskMngBack.Models;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetAllAsync();
        Task<List<TaskItem>> GetByUserAsync(int userId);
        Task<TaskItem?> GetByIdAsync(int id);
        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(TaskItem task);
        Task<bool> HasTasksForUserAsync(int userId);
        Task<bool> HasTasksForStatusAsync(Guid statusId);
    }
}
