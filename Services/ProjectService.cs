using TaskMngBack.DTOs.Projects;
using TaskMngBack.Exceptions;
using TaskMngBack.Models;
using TaskMngBack.Models.Enums;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;

        public ProjectService(
            IProjectRepository projectRepository,
            IUserRepository userRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        public async Task<List<ProjectDto>> GetAllAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            return projects.Select(MapToDto).ToList();
        }

        public async Task<ProjectDto> GetByIdAsync(Guid id)
        {
            var project = await GetProjectOrThrowAsync(id);
            return MapToDto(project);
        }

        public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
        {
            var members = await ResolveMembersAsync(dto.Members);

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                Members = members
            };

            await _projectRepository.AddAsync(project);

            var created = await GetProjectOrThrowAsync(project.Id);
            return MapToDto(created);
        }

        public async Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectDto dto)
        {
            var project = await GetProjectOrThrowAsync(id);

            project.Name = dto.Name;
            project.Description = dto.Description;
            project.Members = await ResolveMembersAsync(dto.Members);

            await _projectRepository.UpdateAsync(project);

            var updated = await GetProjectOrThrowAsync(id);
            return MapToDto(updated);
        }

        public async Task DeleteAsync(Guid id)
        {
            var project = await GetProjectOrThrowAsync(id);
            await _projectRepository.DeleteAsync(project);
        }

        private async Task<List<ProjectMember>> ResolveMembersAsync(List<ProjectMemberInputDto> memberInputs)
        {
            if (memberInputs.Count == 0)
            {
                return new List<ProjectMember>();
            }

            var userIds = memberInputs.Select(m => m.UserId).Distinct().ToList();
            var users = await _userRepository.GetByIdsAsync(userIds);

            if (users.Count != userIds.Count)
            {
                throw new NotFoundException("Belirtilen kullanıcılardan biri veya birkaçı bulunamadı.");
            }

            return memberInputs.Select(m => new ProjectMember
            {
                Id = Guid.NewGuid(),
                UserId = m.UserId,
                Role = Enum.Parse<ProjectMemberRole>(m.Role, ignoreCase: true)
            }).ToList();
        }

        private async Task<Project> GetProjectOrThrowAsync(Guid id)
        {
            var project = await _projectRepository.GetByIdAsync(id);

            if (project is null)
            {
                throw new NotFoundException($"Id'si {id} olan proje bulunamadı.");
            }

            return project;
        }

        private static ProjectDto MapToDto(Project project)
        {
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                Members = project.Members
                    .Select(m => new ProjectMemberDto
                    {
                        UserId = m.UserId,
                        FullName = m.User?.FullName ?? string.Empty,
                        Role = m.Role.ToString()
                    })
                    .ToList()
            };
        }
    }
}
