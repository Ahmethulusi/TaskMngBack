using TaskMngBack.DTOs.Sprints;
using TaskMngBack.Exceptions;
using TaskMngBack.Models;
using TaskMngBack.Models.Enums;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class SprintService : ISprintService
    {
        private readonly ISprintRepository _sprintRepository;
        private readonly IProjectRepository _projectRepository;

        public SprintService(
            ISprintRepository sprintRepository,
            IProjectRepository projectRepository)
        {
            _sprintRepository = sprintRepository;
            _projectRepository = projectRepository;
        }

        public async Task<List<SprintDto>> GetByProject(Guid projectId, int userId, List<string> permissions)
        {
            var project = await GetProjectOrThrowAsync(projectId);
            EnsureCanView(project, userId, permissions);

            var sprints = await _sprintRepository.GetByProjectIdAsync(projectId);
            return sprints.Select(MapToDto).ToList();
        }

        public async Task<SprintDto> Create(Guid projectId, CreateSprintDto dto, int userId, List<string> permissions)
        {
            var project = await GetProjectOrThrowAsync(projectId);
            EnsureCanManage(project, userId, permissions);
            EnsureValidDateRange(dto.StartDate, dto.EndDate);

            var sprint = new Sprint
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                ProjectId = projectId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CreatedAt = DateTime.UtcNow
            };

            await _sprintRepository.AddAsync(sprint);

            var created = await GetSprintOrThrowAsync(sprint.Id);
            return MapToDto(created);
        }

        public async Task<SprintDto> Update(Guid id, UpdateSprintDto dto, int userId, List<string> permissions)
        {
            var sprint = await GetSprintOrThrowAsync(id);
            var project = await GetProjectOrThrowAsync(sprint.ProjectId);
            EnsureCanManage(project, userId, permissions);
            EnsureValidDateRange(dto.StartDate, dto.EndDate);

            sprint.Name = dto.Name;
            sprint.StartDate = dto.StartDate;
            sprint.EndDate = dto.EndDate;

            await _sprintRepository.UpdateAsync(sprint);

            var updated = await GetSprintOrThrowAsync(id);
            return MapToDto(updated);
        }

        public async Task Delete(Guid id, int userId, List<string> permissions)
        {
            var sprint = await GetSprintOrThrowAsync(id);
            var project = await GetProjectOrThrowAsync(sprint.ProjectId);
            EnsureCanManage(project, userId, permissions);

            await _sprintRepository.DeleteAsync(sprint);
        }

        private async Task<Project> GetProjectOrThrowAsync(Guid projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project is null)
            {
                throw new NotFoundException($"Id'si {projectId} olan proje bulunamadı.");
            }

            return project;
        }

        private async Task<Sprint> GetSprintOrThrowAsync(Guid id)
        {
            var sprint = await _sprintRepository.GetByIdAsync(id);
            if (sprint is null)
            {
                throw new NotFoundException($"Id'si {id} olan sprint bulunamadı.");
            }

            return sprint;
        }

        private static void EnsureCanView(Project project, int userId, List<string> permissions)
        {
            if (!permissions.Contains("projects.manage") &&
                !project.Members.Any(m => m.UserId == userId))
            {
                throw new ForbiddenAccessException("Bu projeyi görüntüleme yetkiniz yok.");
            }
        }

        private static void EnsureCanManage(Project project, int userId, List<string> permissions)
        {
            var isOwner = project.Members.Any(m => m.UserId == userId && m.Role == ProjectMemberRole.Owner);
            if (!permissions.Contains("projects.manage") && !isOwner)
            {
                throw new ForbiddenAccessException("Bu işlem için yetkiniz yok.");
            }
        }

        private static void EnsureValidDateRange(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
            {
                throw new BadRequestException("Bitiş tarihi başlangıçtan önce olamaz");
            }
        }

        private static SprintDto MapToDto(Sprint sprint)
        {
            return new SprintDto
            {
                Id = sprint.Id,
                ProjectId = sprint.ProjectId,
                Name = sprint.Name,
                StartDate = sprint.StartDate,
                EndDate = sprint.EndDate,
                TaskCount = sprint.Tasks.Count
            };
        }
    }
}
