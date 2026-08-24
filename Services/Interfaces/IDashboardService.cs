using TaskMngBack.DTOs.Dashboard;

namespace TaskMngBack.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummary(int userId, List<string> permissions);
    }
}
