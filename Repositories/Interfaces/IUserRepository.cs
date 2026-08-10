using TaskMngBack.Models;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task<bool> EmailExistsAsync(string email);

        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<List<User>> GetByIdsAsync(List<int> ids);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
    }
}
