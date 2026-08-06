using TaskManager_Staj_Project.DTOs.Tasks;

namespace TaskManager_Staj_Project.Services.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetAllForUser(int userId, bool isAdmin);
        Task<TaskDto> GetByIdForUser(int taskId, int userId, bool isAdmin);
        Task<TaskDto> Create(CreateTaskDto dto, int userId, bool isAdmin);
        Task<TaskDto> Update(int taskId, UpdateTaskDto dto, int userId, bool isAdmin);
        Task<TaskDto> UpdateStatus(int taskId, UpdateTaskStatusDto dto, int userId, bool isAdmin);
        Task Delete(int taskId, int userId, bool isAdmin);
    }
}
