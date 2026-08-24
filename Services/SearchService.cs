using TaskMngBack.DTOs.Search;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class SearchService : ISearchService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public SearchService(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IDepartmentRepository departmentRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<SearchResultsDto> Search(string query, int userId, List<string> permissions)
        {
            var trimmedQuery = query?.Trim() ?? string.Empty;
            if (trimmedQuery.Length < 2)
            {
                return new SearchResultsDto();
            }

            var tasks = permissions.Contains("tasks.view.all")
                ? await _taskRepository.GetAllAsync()
                : await _taskRepository.GetByUserAsync(userId);

            var projects = permissions.Contains("projects.manage")
                ? await _projectRepository.GetAllAsync()
                : await _projectRepository.GetByUserAsync(userId);

            var departments = await _departmentRepository.GetAllAsync();

            return new SearchResultsDto
            {
                Tasks = tasks
                    .Where(t => t.Title.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase))
                    .Take(5)
                    .Select(t => new SearchResultItemDto
                    {
                        Id = t.Id.ToString(),
                        Title = t.Title,
                        Subtitle = t.StatusDefinition?.Name,
                        Type = "Task"
                    })
                    .ToList(),
                Projects = projects
                    .Where(p => p.Name.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase))
                    .Take(5)
                    .Select(p => new SearchResultItemDto
                    {
                        Id = p.Id.ToString(),
                        Title = p.Name,
                        Subtitle = $"{p.Members.Count} üye",
                        Type = "Project"
                    })
                    .ToList(),
                Departments = departments
                    .Where(d => d.Name.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase))
                    .Take(5)
                    .Select(d => new SearchResultItemDto
                    {
                        Id = d.Id.ToString(),
                        Title = d.Name,
                        Subtitle = null,
                        Type = "Department"
                    })
                    .ToList()
            };
        }
    }
}
