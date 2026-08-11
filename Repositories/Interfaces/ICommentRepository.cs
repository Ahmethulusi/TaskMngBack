using TaskMngBack.Models;

namespace TaskMngBack.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetByTaskIdAsync(int taskId);
        Task<Comment?> GetByIdAsync(Guid id);
        Task AddAsync(Comment comment);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(Comment comment);
    }
}
