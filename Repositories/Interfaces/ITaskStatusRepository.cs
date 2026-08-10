using TaskMngBack.Models;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface ITaskStatusRepository
    {
        Task<List<TaskStatusDefinition>> GetAllAsync();
        Task<TaskStatusDefinition?> GetByIdAsync(Guid id);
        Task<TaskStatusDefinition?> GetDefaultAsync();
        Task AddAsync(TaskStatusDefinition status);
        Task UpdateAsync(TaskStatusDefinition status);
        Task DeleteAsync(TaskStatusDefinition status);
    }
}
