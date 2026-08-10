using Microsoft.EntityFrameworkCore;
using TaskMngBack.Data;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;

namespace TaskMngBack.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<TaskItem>> GetAllAsync()
        {
            return IncludeNavigations(_context.Tasks).ToListAsync();
        }

        public Task<List<TaskItem>> GetByUserAsync(int userId)
        {
            return IncludeNavigations(_context.Tasks)
                .Where(t => t.CreatedByUserId == userId || t.AssignedUsers.Any(u => u.Id == userId))
                .ToListAsync();
        }

        public Task<TaskItem?> GetByIdAsync(int id)
        {
            return IncludeNavigations(_context.Tasks).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddAsync(TaskItem task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem task)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public Task<bool> HasTasksForUserAsync(int userId)
        {
            return _context.Tasks.AnyAsync(t =>
                t.CreatedByUserId == userId ||
                t.AssignedUsers.Any(u => u.Id == userId));
        }

        public Task<bool> HasTasksForStatusAsync(Guid statusId)
        {
            return _context.Tasks.AnyAsync(t => t.StatusId == statusId);
        }

        private static IQueryable<TaskItem> IncludeNavigations(IQueryable<TaskItem> query)
        {
            return query
                .Include(t => t.StatusDefinition)
                .Include(t => t.Department)
                .Include(t => t.Project)
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedUsers)
                .Include(t => t.Labels);
        }
    }
}
