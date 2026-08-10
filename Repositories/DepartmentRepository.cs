using Microsoft.EntityFrameworkCore;
using TaskMngBack.Data;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;

namespace TaskMngBack.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;

        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Department>> GetAllAsync()
        {
            return _context.Departments
                .Include(d => d.Users)
                .ToListAsync();
        }

        public Task<Department?> GetByIdAsync(int id)
        {
            return _context.Departments
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public Task<List<Department>> GetByIdsAsync(List<int> ids)
        {
            return _context.Departments
                .Where(d => ids.Contains(d.Id))
                .ToListAsync();
        }

        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Department department)
        {
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }
    }
}
