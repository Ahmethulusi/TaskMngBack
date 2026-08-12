using Microsoft.EntityFrameworkCore;
using TaskMngBack.Constants;
using TaskMngBack.Data;
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
        private readonly AppDbContext _context;

        public ProjectService(
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            AppDbContext context)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<List<ProjectDto>> GetAllAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            return projects.Select(MapToDto).ToList();
        }

        public async Task<List<ProjectDto>> GetAllForUser(int userId, List<string> permissions)
        {
            var projects = permissions.Contains("projects.manage")
                ? await _projectRepository.GetAllAsync()
                : await _projectRepository.GetByUserAsync(userId);

            return projects.Select(MapToDto).ToList();
        }

        public async Task<ProjectDto> GetByIdAsync(Guid id)
        {
            var project = await GetProjectOrThrowAsync(id);
            return MapToDto(project);
        }

        public async Task<ProjectDto> GetByIdForUser(Guid projectId, int userId, List<string> permissions)
        {
            var project = await GetProjectOrThrowAsync(projectId);

            if (!permissions.Contains("projects.manage") &&
                !project.Members.Any(m => m.UserId == userId))
            {
                throw new ForbiddenAccessException("Bu projeyi görüntüleme yetkiniz yok.");
            }

            return MapToDto(project);
        }

        public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
        {
            var members = await ResolveMembersAsync(dto.Members);

            var iconKey = ValidateAndGetIconKey(dto.IconKey);

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                IconKey = iconKey,
                CreatedAt = DateTime.UtcNow,
                Members = members
            };

            await _projectRepository.AddAsync(project);

            var created = await GetProjectOrThrowAsync(project.Id);
            return MapToDto(created);
        }

        public async Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectDto dto, int userId, List<string> permissions)
        {
            var project = await GetProjectOrThrowAsync(id);

            var isOwner = project.Members.Any(m => m.UserId == userId && m.Role == ProjectMemberRole.Owner);
            if (!permissions.Contains("projects.manage") && !isOwner)
            {
                throw new ForbiddenAccessException("Bu işlem için yetkiniz yok.");
            }

            var iconKey = ValidateAndGetIconKey(dto.IconKey);

            project.Name = dto.Name;
            project.Description = dto.Description;
            project.IconKey = iconKey;

            if (dto.Members.Any())
            {
                project.Members.Clear();
                var newMembers = await ResolveMembersAsync(dto.Members);
                foreach (var member in newMembers)
                {
                    project.Members.Add(member);
                    // EF Core, elle atanmış bir Guid PK'ya sahip yeni entity'leri navigation
                    // koleksiyonuna eklerken "Added" yerine "Modified" olarak işaretleyebiliyor
                    // (zaten var olduğunu varsayıyor). Durumu açıkça zorluyoruz.
                    _context.Entry(member).State = EntityState.Added;
                }
            }

            await _projectRepository.UpdateAsync(project);

            return MapToDto(project);
        }

        public async Task DeleteAsync(Guid id, int userId, List<string> permissions)
        {
            var project = await GetProjectOrThrowAsync(id);

            var isOwner = project.Members.Any(m => m.UserId == userId && m.Role == ProjectMemberRole.Owner);
            if (!permissions.Contains("projects.manage") && !isOwner)
            {
                throw new ForbiddenAccessException("Bu işlem için yetkiniz yok.");
            }

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

        private static string ValidateAndGetIconKey(string? iconKey)
        {
            if (string.IsNullOrWhiteSpace(iconKey))
            {
                return ProjectIcons.DefaultKey;
            }

            if (!ProjectIcons.AllowedKeys.Contains(iconKey))
            {
                throw new BadRequestException("Geçersiz ikon seçimi.");
            }

            return iconKey;
        }

        private static ProjectDto MapToDto(Project project)
        {
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                IconKey = project.IconKey,
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
