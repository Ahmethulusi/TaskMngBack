using Microsoft.EntityFrameworkCore;
using TaskMngBack.Data;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;

namespace TaskMngBack.Repositories
{
    public class SprintRepository : ISprintRepository
    {
        private readonly AppDbContext _context;

        public SprintRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Sprint>> GetByProjectIdAsync(Guid projectId)
        {
            return _context.Sprints
                .Include(s => s.Tasks)
                .Where(s => s.ProjectId == projectId)
                .ToListAsync();
        }

        public Task<Sprint?> GetByIdAsync(Guid id)
        {
            return _context.Sprints
                .Include(s => s.Tasks)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(Sprint sprint)
        {
            await _context.Sprints.AddAsync(sprint);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Sprint sprint)
        {
            _context.Sprints.Update(sprint);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Sprint sprint)
        {
            _context.Sprints.Remove(sprint);
            await _context.SaveChangesAsync();
        }
    }
}
