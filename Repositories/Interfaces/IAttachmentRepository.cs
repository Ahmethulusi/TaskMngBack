using TaskMngBack.Models;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface IAttachmentRepository
    {
        Task<List<Attachment>> GetByTaskIdAsync(int taskId);
        Task<List<Attachment>> GetByTaskIdIncludingCommentsAsync(int taskId);
        Task<List<Attachment>> GetByCommentIdAsync(Guid commentId);
        Task<Attachment?> GetByIdAsync(Guid id);
        Task AddAsync(Attachment attachment);
        Task DeleteAsync(Attachment attachment);
        Task ClaimForCommentAsync(List<Guid> attachmentIds, Guid commentId, int taskId, int uploaderId);
    }
}
