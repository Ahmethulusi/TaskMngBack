using TaskMngBack.DTOs.Tasks;

namespace TaskMngBack.Services.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetAllForUser(int userId, List<string> permissions);
        Task<TaskDto> GetByIdForUser(int taskId, int userId, List<string> permissions);
        Task<TaskDto> Create(CreateTaskDto dto, int userId, List<string> permissions);
        Task<TaskDto> Update(int taskId, UpdateTaskDto dto, int userId, List<string> permissions);
        Task<TaskDto> UpdateStatus(int taskId, UpdateTaskStatusDto dto, int userId, List<string> permissions);
        Task<TaskDto> UpdateLabels(int taskId, UpdateTaskLabelsDto dto, int userId, List<string> permissions);
        Task Delete(int taskId, int userId, List<string> permissions);
        Task<TaskDto> AssignTask(int taskId, List<int> assignedUserIds, int userId);
    }
}
