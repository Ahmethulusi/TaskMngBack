using TaskManager_Staj_Project.Models;

namespace TaskManager_Staj_Project.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task<bool> EmailExistsAsync(string email);
    }
}
