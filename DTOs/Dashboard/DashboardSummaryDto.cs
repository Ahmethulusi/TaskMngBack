namespace TaskMngBack.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public int OpenCount { get; set; }
        public int InProgressCount { get; set; }
        public int OverdueCount { get; set; }
        public int CompletedCount { get; set; }
        public List<DepartmentCountDto> ByDepartment { get; set; } = new();
        public List<StatusCountDto> ByStatus { get; set; } = new();
        public List<PriorityCountDto> ByPriority { get; set; } = new();
        public List<ProjectCountDto> ByProject { get; set; } = new();
        public List<WeeklyCompletedDto> WeeklyCompleted { get; set; } = new();
    }
}
