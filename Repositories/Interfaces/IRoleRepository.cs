using TaskMngBack.Models;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllAsync();
        Task<List<Role>> GetByUserIdAsync(int userId);
        Task<Role?> GetByNameAsync(string name);
    }
}
