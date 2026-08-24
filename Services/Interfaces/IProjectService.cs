using TaskMngBack.DTOs.Projects;

namespace TaskMngBack.Services.Interfaces
{
    public interface IProjectService
    {
        Task<List<ProjectDto>> GetAllAsync();
        Task<List<ProjectDto>> GetAllForUser(int userId, List<string> permissions);
        Task<ProjectDto> GetByIdAsync(Guid id);
        Task<ProjectDto> GetByIdForUser(Guid projectId, int userId, List<string> permissions);
        Task<List<ProjectActivityItemDto>> GetActivity(Guid projectId, int userId, List<string> permissions);
        Task<ProjectDto> CreateAsync(CreateProjectDto dto);
        Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectDto dto, int userId, List<string> permissions);
        Task DeleteAsync(Guid id, int userId, List<string> permissions);
    }
}
