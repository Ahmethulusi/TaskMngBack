using TaskMngBack.Models;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetByTaskIdAsync(int taskId);
        Task<Comment?> GetByIdAsync(Guid id);
        Task<CommentReaction?> GetReactionAsync(Guid commentId, int userId, string emoji);
        Task AddAsync(Comment comment);
        Task AddReactionAsync(CommentReaction reaction);
        Task AddMentionAsync(CommentMention mention);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(Comment comment);
        Task RemoveReactionAsync(CommentReaction reaction);
    }
}
