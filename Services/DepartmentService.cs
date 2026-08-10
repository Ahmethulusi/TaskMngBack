using TaskMngBack.DTOs.Departments;
using TaskMngBack.DTOs.Users;
using TaskMngBack.Exceptions;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserRepository _userRepository;

        public DepartmentService(
            IDepartmentRepository departmentRepository,
            IUserRepository userRepository)
        {
            _departmentRepository = departmentRepository;
            _userRepository = userRepository;
        }

        public async Task<List<DepartmentDto>> GetAllAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();
            return departments.Select(MapToDto).ToList();
        }

        public async Task<DepartmentDto> GetByIdAsync(int id)
        {
            var department = await GetDepartmentOrThrowAsync(id);
            return MapToDto(department);
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
        {
            var users = await ResolveUsersAsync(dto.UserIds);

            if (dto.ManagerId.HasValue)
            {
                var manager = await _userRepository.GetByIdAsync(dto.ManagerId.Value);
                if (manager is null)
                {
                    throw new NotFoundException("Belirtilen yönetici bulunamadı.");
                }
            }

            var department = new Department
            {
                Name = dto.Name,
                ManagerId = dto.ManagerId,
                Users = users
            };

            await _departmentRepository.AddAsync(department);

            var created = await GetDepartmentOrThrowAsync(department.Id);
            return MapToDto(created);
        }

        public async Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto)
        {
            var department = await GetDepartmentOrThrowAsync(id);

            if (dto.ManagerId.HasValue)
            {
                var manager = await _userRepository.GetByIdAsync(dto.ManagerId.Value);
                if (manager is null)
                {
                    throw new NotFoundException("Belirtilen yönetici bulunamadı.");
                }
            }

            department.Name = dto.Name;
            department.ManagerId = dto.ManagerId;
            department.Users = await ResolveUsersAsync(dto.UserIds);

            await _departmentRepository.UpdateAsync(department);

            var updated = await GetDepartmentOrThrowAsync(id);
            return MapToDto(updated);
        }

        public async Task DeleteAsync(int id)
        {
            var department = await GetDepartmentOrThrowAsync(id);
            await _departmentRepository.DeleteAsync(department);
        }

        private async Task<List<User>> ResolveUsersAsync(List<int> userIds)
        {
            var distinctIds = userIds.Distinct().ToList();

            if (distinctIds.Count == 0)
            {
                return new List<User>();
            }

            var users = await _userRepository.GetByIdsAsync(distinctIds);

            if (users.Count != distinctIds.Count)
            {
                throw new NotFoundException("Belirtilen kullanıcılardan biri veya birkaçı bulunamadı.");
            }

            return users;
        }

        private async Task<Department> GetDepartmentOrThrowAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);

            if (department is null)
            {
                throw new NotFoundException($"Id'si {id} olan departman bulunamadı.");
            }

            return department;
        }

        private static DepartmentDto MapToDto(Department department)
        {
            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                ManagerId = department.ManagerId,
                ManagerName = department.Manager?.FullName,
                Users = department.Users
                    .Select(u => new UserSummaryDto
                    {
                        Id = u.Id,
                        FullName = u.FullName
                    })
                    .ToList()
            };
        }
    }
}
