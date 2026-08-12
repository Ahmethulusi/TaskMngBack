using Microsoft.EntityFrameworkCore;
using TaskMngBack.Data;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;

namespace TaskMngBack.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Comment>> GetByTaskIdAsync(int taskId)
        {
            return _context.Comments
                .Include(c => c.User)
                .Include(c => c.Attachments)
                    .ThenInclude(a => a.UploadedByUser)
                .Where(c => c.TaskId == taskId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public Task<Comment?> GetByIdAsync(Guid id)
        {
            return _context.Comments
                .Include(c => c.User)
                .Include(c => c.Attachments)
                    .ThenInclude(a => a.UploadedByUser)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Comment comment)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}
