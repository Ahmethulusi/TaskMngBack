using Microsoft.EntityFrameworkCore;
using TaskMngBack.Data;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;

namespace TaskMngBack.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Role>> GetAllAsync()
        {
            return _context.Roles
                .Include(r => r.Permissions)
                .ToListAsync();
        }

        public async Task<List<Role>> GetByUserIdAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                    .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.Roles.ToList() ?? new List<Role>();
        }

        public Task<Role?> GetByNameAsync(string name)
        {
            return _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Name == name);
        }
    }
}
