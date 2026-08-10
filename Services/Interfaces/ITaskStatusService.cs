using TaskMngBack.DTOs.Statuses;

namespace TaskMngBack.Services.Interfaces
{
    public interface ITaskStatusService
    {
        Task<List<TaskStatusDto>> GetAllAsync();
        Task<TaskStatusDto> CreateAsync(CreateTaskStatusDto dto);
        Task<TaskStatusDto> UpdateAsync(Guid id, UpdateTaskStatusDefinitionDto dto);
        Task DeleteAsync(Guid id);
    }
}
