using Microsoft.EntityFrameworkCore;
using TaskMngBack.Data;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;

namespace TaskMngBack.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _context;

        public PermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Permission>> GetAllAsync()
        {
            return _context.Permissions.ToListAsync();
        }

        public Task<List<Permission>> GetByIdsAsync(List<Guid> ids)
        {
            return _context.Permissions
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();
        }
    }
}
