using Microsoft.EntityFrameworkCore;
using TaskManager_Staj_Project.Data;
using TaskManager_Staj_Project.Models;
using TaskManager_Staj_Project.Repositories.Interfaces;

namespace TaskManager_Staj_Project.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            return _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
