using TaskMngBack.DTOs.Departments;

namespace TaskMngBack.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<DepartmentDto>> GetAllAsync();
        Task<DepartmentDto> GetByIdAsync(int id);
        Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto);
        Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto);
        Task DeleteAsync(int id);
    }
}
