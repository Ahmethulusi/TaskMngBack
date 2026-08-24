using TaskMngBack.DTOs.Sprints;

namespace TaskMngBack.Services.Interfaces
{
    public interface ISprintService
    {
        Task<List<SprintDto>> GetByProject(Guid projectId, int userId, List<string> permissions);
        Task<SprintDto> Create(Guid projectId, CreateSprintDto dto, int userId, List<string> permissions);
        Task<SprintDto> Update(Guid id, UpdateSprintDto dto, int userId, List<string> permissions);
        Task Delete(Guid id, int userId, List<string> permissions);
    }
}
