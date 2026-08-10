using TaskMngBack.Models;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task<List<Department>> GetByIdsAsync(List<int> ids);
        Task AddAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(Department department);
    }
}
