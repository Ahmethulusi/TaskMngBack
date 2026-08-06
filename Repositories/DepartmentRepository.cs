using Microsoft.EntityFrameworkCore;
using TaskManager_Staj_Project.Data;
using TaskManager_Staj_Project.Models;
using TaskManager_Staj_Project.Repositories.Interfaces;

namespace TaskManager_Staj_Project.Repositories
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
            return _context.Departments.ToListAsync();
        }

        public Task<Department?> GetByIdAsync(int id)
        {
            return _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
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
