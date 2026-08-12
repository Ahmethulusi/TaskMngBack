using TaskMngBack.Models;
using TaskMngBack.Models.Enums;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetAllAsync();
        Task<List<Project>> GetByUserAsync(int userId);
        Task<Project?> GetByIdAsync(Guid id);
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(Project project);
        Task<ProjectMemberRole?> GetMemberRoleAsync(Guid? projectId, int userId);
    }
}
