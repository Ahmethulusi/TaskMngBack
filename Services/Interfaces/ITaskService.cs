using TaskMngBack.DTOs.Tasks;

namespace TaskMngBack.Services.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetAllForUser(int userId, bool isAdmin);
        Task<TaskDto> GetByIdForUser(int taskId, int userId, bool isAdmin);
        Task<TaskDto> Create(CreateTaskDto dto, int userId, bool isAdmin);
        Task<TaskDto> Update(int taskId, UpdateTaskDto dto, int userId, bool isAdmin);
        Task<TaskDto> UpdateStatus(int taskId, UpdateTaskStatusDto dto, int userId, bool isAdmin);
        Task<TaskDto> UpdateLabels(int taskId, UpdateTaskLabelsDto dto, int userId, bool isAdmin);
        Task Delete(int taskId, int userId, bool isAdmin);
        Task<TaskDto> AssignTask(int taskId, List<int> assignedUserIds, int userId);
    }
}
