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

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                throw new ConflictException("Bu email adresi zaten kayıtlı.");
            }

            var departments = await ResolveDepartmentsAsync(dto.DepartmentIds);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = Enum.Parse<UserRole>(dto.Role, ignoreCase: true),
                MustChangePassword = true,
                CreatedAt = DateTime.UtcNow,
                Departments = departments
            };

            await _userRepository.AddAsync(user);

            var role = await _roleRepository.GetByNameAsync(dto.Role);
            if (role != null)
            {
                user.Roles.Add(role);
                await _userRepository.UpdateAsync(user);
            }

            var created = await GetUserOrThrowAsync(user.Id);
            return MapToDto(created);
        }

        public async Task<UserDto> UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await GetUserOrThrowAsync(id);

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Role = Enum.Parse<UserRole>(dto.Role, ignoreCase: true);

            var departments = await ResolveDepartmentsAsync(dto.DepartmentIds);
            user.Departments.Clear();
            foreach (var department in departments)
            {
                user.Departments.Add(department);
            }

            var role = await _roleRepository.GetByNameAsync(dto.Role);
            if (role is null)
            {
                throw new NotFoundException($"'{dto.Role}' isimli rol bulunamadı.");
            }

            user.Roles.Clear();
            user.Roles.Add(role);

            await _userRepository.UpdateAsync(user);

            var updatedRoles = string.Join(", ", user.Roles.Select(r => r.Name));
            _logger.LogInformation("User {UserId} roles updated to: {Roles}", user.Id, updatedRoles);

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
