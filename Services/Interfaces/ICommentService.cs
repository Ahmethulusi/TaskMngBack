using TaskMngBack.DTOs.Comments;

namespace TaskMngBack.Services.Interfaces
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetForTask(int taskId, int userId, bool isAdmin);
        Task<CommentDto> CreateAsync(int taskId, CreateCommentDto dto, int userId, bool isAdmin);
        Task<CommentDto> UpdateAsync(Guid commentId, UpdateCommentDto dto, int userId);
        Task DeleteAsync(Guid commentId, int userId, bool isAdmin);
    }
}
