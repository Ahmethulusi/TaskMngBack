using TaskManager_Staj_Project.DTOs.Departments;
using TaskManager_Staj_Project.Exceptions;
using TaskManager_Staj_Project.Models;
using TaskManager_Staj_Project.Repositories.Interfaces;
using TaskManager_Staj_Project.Services.Interfaces;

namespace TaskManager_Staj_Project.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
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
            var department = new Department
            {
                Name = dto.Name
            };

            await _departmentRepository.AddAsync(department);

            return MapToDto(department);
        }

        public async Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto)
        {
            var department = await GetDepartmentOrThrowAsync(id);

            department.Name = dto.Name;

            await _departmentRepository.UpdateAsync(department);

            return MapToDto(department);
        }

        public async Task DeleteAsync(int id)
        {
            var department = await GetDepartmentOrThrowAsync(id);
            await _departmentRepository.DeleteAsync(department);
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
                Name = department.Name
            };
        }
    }
}
