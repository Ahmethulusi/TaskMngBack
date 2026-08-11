using TaskMngBack.DTOs.Activity;

namespace TaskMngBack.Services.Interfaces
{
    public interface IActivityLogService
    {
        Task LogAsync(int taskId, int userId, string fieldName, string? oldValue, string? newValue);
        Task<List<ActivityLogDto>> GetByTaskIdAsync(int taskId, int userId, bool isAdmin);
    }
}
