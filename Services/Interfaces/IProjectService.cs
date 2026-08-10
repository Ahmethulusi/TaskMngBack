using TaskMngBack.DTOs.Projects;

namespace TaskMngBack.Services.Interfaces
{
    public interface IProjectService
    {
        Task<List<ProjectDto>> GetAllAsync();
        Task<ProjectDto> GetByIdAsync(Guid id);
        Task<ProjectDto> CreateAsync(CreateProjectDto dto);
        Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectDto dto);
        Task DeleteAsync(Guid id);
    }
}
