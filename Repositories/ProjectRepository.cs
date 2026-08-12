using Microsoft.EntityFrameworkCore;
using TaskMngBack.Data;
using TaskMngBack.Exceptions;
using TaskMngBack.Models;
using TaskMngBack.Models.Enums;
using TaskMngBack.Repositories.Interfaces;

namespace TaskMngBack.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProjectRepository> _logger;

        public ProjectRepository(AppDbContext context, ILogger<ProjectRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<List<Project>> GetAllAsync()
        {
            return _context.Projects
                .Include(p => p.Members)
                    .ThenInclude(m => m.User)
                .ToListAsync();
        }

        public Task<List<Project>> GetByUserAsync(int userId)
        {
            return _context.Projects
                .Include(p => p.Members)
                    .ThenInclude(m => m.User)
                .Where(p => p.Members.Any(m => m.UserId == userId))
                .ToListAsync();
        }

        public Task<Project?> GetByIdAsync(Guid id)
        {
            return _context.Projects
                .Include(p => p.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Project project)
        {
            var entry = _context.Entry(project);
            if (entry.State == EntityState.Detached)
            {
                _context.Projects.Update(project);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                LogConcurrencyConflict(ex);
                throw new NotFoundException(
                    "Proje bulunamadı. Başka bir kullanıcı tarafından silinmiş olabilir; lütfen sayfayı yenileyin.");
            }
        }

        public async Task DeleteAsync(Project project)
        {
            _context.Projects.Remove(project);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                LogConcurrencyConflict(ex);
                throw new NotFoundException(
                    "Proje bulunamadı. Zaten silinmiş olabilir; lütfen sayfayı yenileyin.");
            }
        }

        public async Task<ProjectMemberRole?> GetMemberRoleAsync(Guid? projectId, int userId)
        {
            if (projectId is null)
            {
                return null;
            }

            var member = await _context.ProjectMembers
                .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == userId);

            return member?.Role;
        }

        private void LogConcurrencyConflict(DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                var keyValues = entry.Metadata.FindPrimaryKey()?.Properties
                    .Select(p => $"{p.Name}={entry.Property(p.Name).CurrentValue}");
                _logger.LogError(
                    "Concurrency conflict on {EntityType} ({KeyValues}), state {State}",
                    entry.Entity.GetType().Name,
                    keyValues is null ? "unknown" : string.Join(", ", keyValues),
                    entry.State);
            }
        }
    }
}
