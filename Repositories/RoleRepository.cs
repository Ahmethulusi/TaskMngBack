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

        public Task<Role?> GetByIdAsync(Guid id)
        {
            return _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public Task<List<Role>> GetByIdsAsync(List<Guid> ids)
        {
            return _context.Roles
                .Where(r => ids.Contains(r.Id))
                .Include(r => r.Permissions)
                .ToListAsync();
        }

        public async Task AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Role role)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}
