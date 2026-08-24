using TaskMngBack.DTOs.Dashboard;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly ITaskStatusRepository _taskStatusRepository;

        public DashboardService(
            ITaskRepository taskRepository,
            IActivityLogRepository activityLogRepository,
            ITaskStatusRepository taskStatusRepository)
        {
            _taskRepository = taskRepository;
            _activityLogRepository = activityLogRepository;
            _taskStatusRepository = taskStatusRepository;
        }

        public async Task<DashboardSummaryDto> GetSummary(int userId, List<string> permissions)
        {
            var tasks = permissions.Contains("tasks.view.all")
                ? await _taskRepository.GetAllAsync()
                : await _taskRepository.GetByUserAsync(userId);

            var weekStarts = GetLastEightWeekStarts();
            var since = weekStarts[0];
            var taskIds = tasks.Select(t => t.Id).ToList();

            var completionStatusNames = (await _taskStatusRepository.GetAllAsync())
                .Where(s => s.IsCompletionStatus)
                .Select(s => s.Name)
                .ToHashSet();

            var completionLogs = taskIds.Count == 0
                ? new List<ActivityLog>()
                : (await _activityLogRepository.GetCompletionLogsAsync(taskIds, since))
                    .Where(l => l.NewValue != null && completionStatusNames.Contains(l.NewValue))
                    .ToList();

            var completionsByWeek = completionLogs
                .GroupBy(l => GetWeekStart(l.CreatedAt.Date))
                .ToDictionary(g => g.Key, g => g.Count());

            return new DashboardSummaryDto
            {
                OpenCount = tasks.Count(t => t.StatusDefinition.IsDefault),
                InProgressCount = tasks.Count(t =>
                    !t.StatusDefinition.IsDefault && !t.StatusDefinition.IsCompletionStatus),
                OverdueCount = tasks.Count(IsOverdue),
                CompletedCount = tasks.Count(t => t.StatusDefinition.IsCompletionStatus),
                ByDepartment = tasks
                    .GroupBy(t => t.Department?.Name ?? "Departmansız")
                    .Select(g => new DepartmentCountDto
                    {
                        DepartmentName = g.Key,
                        Count = g.Count()
                    })
                    .ToList(),
                ByStatus = tasks
                    .GroupBy(t => t.StatusId)
                    .Select(g =>
                    {
                        var status = g.First().StatusDefinition;
                        return new StatusCountDto
                        {
                            StatusName = status.Name,
                            StatusColorKey = status.ColorKey,
                            Count = g.Count()
                        };
                    })
                    .ToList(),
                ByPriority = tasks
                    .GroupBy(t => t.Priority.ToString())
                    .Select(g => new PriorityCountDto
                    {
                        Priority = g.Key,
                        Count = g.Count()
                    })
                    .ToList(),
                ByProject = tasks
                    .GroupBy(t => t.Project?.Name ?? "Projesiz")
                    .Select(g => new ProjectCountDto
                    {
                        ProjectName = g.Key,
                        Count = g.Count()
                    })
                    .ToList(),
                WeeklyCompleted = weekStarts
                    .Select(weekStart => new WeeklyCompletedDto
                    {
                        WeekStartDate = weekStart,
                        Count = completionsByWeek.GetValueOrDefault(weekStart)
                    })
                    .ToList()
            };
        }

        private static bool IsOverdue(TaskItem task)
        {
            return task.DueDate.HasValue
                && task.DueDate.Value.Date < DateTime.UtcNow.Date
                && !task.StatusDefinition.IsCompletionStatus;
        }

        private static List<DateTime> GetLastEightWeekStarts()
        {
            var currentWeekStart = GetWeekStart(DateTime.UtcNow.Date);
            return Enumerable.Range(0, 8)
                .Select(i => currentWeekStart.AddDays(-7 * (7 - i)))
                .ToList();
        }

        private static DateTime GetWeekStart(DateTime date)
        {
            var daysFromMonday = ((int)date.DayOfWeek + 6) % 7;
            return date.AddDays(-daysFromMonday);
        }
    }
}
