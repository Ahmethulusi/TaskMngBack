using TaskMngBack.Models;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface ISprintRepository
    {
        Task<List<Sprint>> GetByProjectIdAsync(Guid projectId);
        Task<Sprint?> GetByIdAsync(Guid id);
        Task AddAsync(Sprint sprint);
        Task UpdateAsync(Sprint sprint);
        Task DeleteAsync(Sprint sprint);
    }
}
