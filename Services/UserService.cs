using Microsoft.Extensions.Logging;
using TaskMngBack.DTOs.Departments;
using TaskMngBack.DTOs.Users;
using TaskMngBack.Exceptions;
using TaskMngBack.Models;
using TaskMngBack.Models.Enums;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IDepartmentRepository departmentRepository,
            ITaskRepository taskRepository,
            IRoleRepository roleRepository,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _taskRepository = taskRepository;
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto).ToList();
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await GetUserOrThrowAsync(id);
            return MapToDto(user);
        }

        public async Task<UserDto> GetOwnProfileAsync(int userId)
        {
            var user = await GetUserOrThrowAsync(userId);
            return MapToDto(user);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                throw new ConflictException("Bu email adresi zaten kayıtlı.");
            }

            if (dto.RoleIds.Count == 0)
            {
                throw new BadRequestException("En az bir rol seçilmeli.");
            }

            var departments = await ResolveDepartmentsAsync(dto.DepartmentIds);
            var roles = await ResolveRolesAsync(dto.RoleIds);

            var legacyRole = DetermineLegacyRole(roles);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = legacyRole,
                MustChangePassword = true,
                CreatedAt = DateTime.UtcNow,
                Departments = departments,
                Roles = roles
            };

            await _userRepository.AddAsync(user);

            var created = await GetUserOrThrowAsync(user.Id);
            return MapToDto(created);
        }

        public async Task<UserDto> UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await GetUserOrThrowAsync(id);

            if (dto.RoleIds.Count == 0)
            {
                throw new BadRequestException("En az bir rol seçilmeli.");
            }

            user.FullName = dto.FullName;
            user.Email = dto.Email;

            var departments = await ResolveDepartmentsAsync(dto.DepartmentIds);
            user.Departments.Clear();
            foreach (var department in departments)
            {
                user.Departments.Add(department);
            }

            var roles = await ResolveRolesAsync(dto.RoleIds);
            var legacyRole = DetermineLegacyRole(roles);
            user.Role = legacyRole;

            user.Roles.Clear();
            foreach (var role in roles)
            {
                user.Roles.Add(role);
            }

            await _userRepository.UpdateAsync(user);

            var updatedRoles = string.Join(", ", user.Roles.Select(r => r.Name));
            _logger.LogInformation("User {UserId} roles updated to: {Roles}", user.Id, updatedRoles);

            return MapToDto(user);
        }

        public async Task<UserDto> UpdateOwnProfileAsync(int userId, UpdateOwnProfileDto dto)
        {
            var user = await GetUserOrThrowAsync(userId);

            if (user.Email != dto.Email)
            {
                if (await _userRepository.EmailExistsForOtherUserAsync(dto.Email, userId))
                {
                    throw new ConflictException("Bu email adresi zaten kayıtlı.");
                }
            }

            user.FullName = dto.FullName;
            user.Email = dto.Email;

            await _userRepository.UpdateAsync(user);

            return MapToDto(user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await GetUserOrThrowAsync(id);

            if (await _taskRepository.HasTasksForUserAsync(id))
            {
                throw new ConflictException(
                    "Bu kullanıcının oluşturduğu veya kendisine atanmış görevler var. Silmeden önce bu görevleri başka bir kullanıcıya aktarın veya silin.");
            }

            await _userRepository.DeleteAsync(user);
        }

        private async Task<List<Department>> ResolveDepartmentsAsync(List<int> departmentIds)
        {
            var distinctIds = departmentIds.Distinct().ToList();

            if (distinctIds.Count == 0)
            {
                return new List<Department>();
            }

            var departments = await _departmentRepository.GetByIdsAsync(distinctIds);

            if (departments.Count != distinctIds.Count)
            {
                throw new NotFoundException("Belirtilen departmanlardan biri veya birkaçı bulunamadı.");
            }

            return departments;
        }

        private async Task<User> GetUserOrThrowAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
            {
                throw new NotFoundException($"Id'si {id} olan kullanıcı bulunamadı.");
            }

            return user;
        }

        private async Task<List<Role>> ResolveRolesAsync(List<Guid> roleIds)
        {
            var distinctIds = roleIds.Distinct().ToList();

            if (distinctIds.Count == 0)
            {
                return new List<Role>();
            }

            var roles = await _roleRepository.GetByIdsAsync(distinctIds);

            if (roles.Count != distinctIds.Count)
            {
                throw new NotFoundException("Belirtilen rollerden biri veya birkaçı bulunamadı.");
            }

            return roles;
        }

        private static UserRole DetermineLegacyRole(List<Role> roles)
        {
            if (roles.Any(r => r.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
            {
                return UserRole.Admin;
            }

            return UserRole.User;
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Roles = user.Roles.Select(r => r.Name).ToList(),
                CreatedAt = user.CreatedAt,
                Departments = user.Departments
                    .Select(d => new DepartmentDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Users = d.Users
                            .Select(u => new UserSummaryDto
                            {
                                Id = u.Id,
                                FullName = u.FullName
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }
    }
}
