using Microsoft.EntityFrameworkCore;
using TaskMngBack.Data;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;

namespace TaskMngBack.Repositories
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly AppDbContext _context;

        public AttachmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Attachment>> GetByTaskIdAsync(int taskId)
        {
            return _context.Attachments
                .Include(a => a.UploadedByUser)
                .Where(a => a.TaskId == taskId && a.CommentId == null)
                .ToListAsync();
        }

        public Task<List<Attachment>> GetByTaskIdIncludingCommentsAsync(int taskId)
        {
            return _context.Attachments
                .Where(a => a.TaskId == taskId)
                .ToListAsync();
        }

        public Task<List<Attachment>> GetByCommentIdAsync(Guid commentId)
        {
            return _context.Attachments
                .Include(a => a.UploadedByUser)
                .Where(a => a.CommentId == commentId)
                .ToListAsync();
        }

        public Task<Attachment?> GetByIdAsync(Guid id)
        {
            return _context.Attachments
                .Include(a => a.UploadedByUser)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Attachment attachment)
        {
            await _context.Attachments.AddAsync(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Attachment attachment)
        {
            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task ClaimForCommentAsync(List<Guid> attachmentIds, Guid commentId, int taskId, int uploaderId)
        {
            if (attachmentIds.Count == 0)
            {
                return;
            }

            var attachments = await _context.Attachments
                .Where(a => attachmentIds.Contains(a.Id))
                .ToListAsync();

            foreach (var attachment in attachments)
            {
                if (attachment.TaskId == taskId &&
                    attachment.CommentId == null &&
                    attachment.UploadedByUserId == uploaderId)
                {
                    attachment.CommentId = commentId;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
