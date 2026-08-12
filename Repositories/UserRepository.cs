using Microsoft.EntityFrameworkCore;
using TaskMngBack.Data;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;

namespace TaskMngBack.Repositories
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
            return _context.Users
                .Include(u => u.Roles)
                    .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(u => u.Email == email);
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

        public Task<bool> EmailExistsForOtherUserAsync(string email, int excludeUserId)
        {
            return _context.Users.AnyAsync(u => u.Email == email && u.Id != excludeUserId);
        }

        public Task<List<User>> GetAllAsync()
        {
            return _context.Users
                .Include(u => u.Departments)
                .Include(u => u.Roles)
                .ToListAsync();
        }

        public Task<User?> GetByIdAsync(int id)
        {
            return _context.Users
                .Include(u => u.Departments)
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<List<User>> GetByIdsAsync(List<int> ids)
        {
            return _context.Users
                .Where(u => ids.Contains(u.Id))
                .ToListAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
