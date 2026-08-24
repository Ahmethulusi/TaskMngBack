using TaskMngBack.DTOs.Comments;

namespace TaskMngBack.Services.Interfaces
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetForTask(int taskId, int userId, List<string> permissions);
        Task<CommentDto> CreateAsync(int taskId, CreateCommentDto dto, int userId, List<string> permissions);
        Task<CommentDto> UpdateAsync(Guid commentId, UpdateCommentDto dto, int userId);
        Task DeleteAsync(Guid commentId, int userId, List<string> permissions);
        Task<CommentDto> ToggleReaction(Guid commentId, ToggleReactionDto dto, int userId, List<string> permissions);
    }
}
