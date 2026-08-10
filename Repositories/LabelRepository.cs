using Microsoft.EntityFrameworkCore;
using TaskMngBack.Data;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;

namespace TaskMngBack.Repositories
{
    public class LabelRepository : ILabelRepository
    {
        private readonly AppDbContext _context;

        public LabelRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Label>> GetAllAsync()
        {
            return _context.Labels.ToListAsync();
        }

        public Task<Label?> GetByIdAsync(Guid id)
        {
            return _context.Labels.FirstOrDefaultAsync(l => l.Id == id);
        }

        public Task<List<Label>> GetByIdsAsync(List<Guid> ids)
        {
            return _context.Labels
                .Where(l => ids.Contains(l.Id))
                .ToListAsync();
        }

        public async Task AddAsync(Label label)
        {
            await _context.Labels.AddAsync(label);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Label label)
        {
            _context.Labels.Update(label);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Label label)
        {
            _context.Labels.Remove(label);
            await _context.SaveChangesAsync();
        }
    }
}
