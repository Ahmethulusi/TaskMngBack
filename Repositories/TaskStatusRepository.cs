using Microsoft.EntityFrameworkCore;
using TaskMngBack.Data;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;

namespace TaskMngBack.Repositories
{
    public class TaskStatusRepository : ITaskStatusRepository
    {
        private readonly AppDbContext _context;

        public TaskStatusRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<TaskStatusDefinition>> GetAllAsync()
        {
            return _context.TaskStatuses
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();
        }

        public Task<TaskStatusDefinition?> GetByIdAsync(Guid id)
        {
            return _context.TaskStatuses.FirstOrDefaultAsync(s => s.Id == id);
        }

        public Task<TaskStatusDefinition?> GetDefaultAsync()
        {
            return _context.TaskStatuses.FirstOrDefaultAsync(s => s.IsDefault);
        }

        public async Task AddAsync(TaskStatusDefinition status)
        {
            await _context.TaskStatuses.AddAsync(status);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskStatusDefinition status)
        {
            _context.TaskStatuses.Update(status);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskStatusDefinition status)
        {
            _context.TaskStatuses.Remove(status);
            await _context.SaveChangesAsync();
        }
    }
}
