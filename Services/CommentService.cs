using TaskMngBack.DTOs.Comments;
using TaskMngBack.Exceptions;
using TaskMngBack.Models;
using TaskMngBack.Repositories.Interfaces;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ITaskRepository _taskRepository;

        public CommentService(
            ICommentRepository commentRepository,
            ITaskRepository taskRepository)
        {
            _commentRepository = commentRepository;
            _taskRepository = taskRepository;
        }

        public async Task<List<CommentDto>> GetForTask(int taskId, int userId, List<string> permissions)
        {
            var task = await GetTaskOrThrowAsync(taskId);

            if (!permissions.Contains("tasks.view.all") &&
                task.CreatedByUserId != userId &&
                !task.AssignedUsers.Any(u => u.Id == userId))
            {
                throw new ForbiddenAccessException("Bu görevin yorumlarını görüntüleme yetkiniz yok.");
            }

            var comments = await _commentRepository.GetByTaskIdAsync(taskId);
            return comments.Select(MapToDto).ToList();
        }

        public async Task<CommentDto> CreateAsync(int taskId, CreateCommentDto dto, int userId, List<string> permissions)
        {
            var task = await GetTaskOrThrowAsync(taskId);

            if (!permissions.Contains("tasks.view.all") &&
                task.CreatedByUserId != userId &&
                !task.AssignedUsers.Any(u => u.Id == userId))
            {
                throw new ForbiddenAccessException("Bu göreve yorum ekleme yetkiniz yok.");
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = userId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _commentRepository.AddAsync(comment);

            var created = await GetCommentOrThrowAsync(comment.Id);
            return MapToDto(created);
        }

        public async Task<CommentDto> UpdateAsync(Guid commentId, UpdateCommentDto dto, int userId)
        {
            var comment = await GetCommentOrThrowAsync(commentId);

            if (comment.UserId != userId)
            {
                throw new ForbiddenAccessException("Başkasının yorumunu düzenleyemezsiniz.");
            }

            comment.Content = dto.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            await _commentRepository.UpdateAsync(comment);

            var updated = await GetCommentOrThrowAsync(commentId);
            return MapToDto(updated);
        }

        public async Task DeleteAsync(Guid commentId, int userId, List<string> permissions)
        {
            var comment = await GetCommentOrThrowAsync(commentId);

            if (!permissions.Contains("tasks.delete.all") && comment.UserId != userId)
            {
                throw new ForbiddenAccessException("Bu yorumu silme yetkiniz yok.");
            }

            await _commentRepository.DeleteAsync(comment);
        }

        private async Task<TaskItem> GetTaskOrThrowAsync(int taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);

            if (task is null)
            {
                throw new NotFoundException($"Id'si {taskId} olan görev bulunamadı.");
            }

            return task;
        }

        private async Task<Comment> GetCommentOrThrowAsync(Guid commentId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);

            if (comment is null)
            {
                throw new NotFoundException($"Id'si {commentId} olan yorum bulunamadı.");
            }

            return comment;
        }

        private static CommentDto MapToDto(Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                TaskId = comment.TaskId,
                UserId = comment.UserId,
                UserFullName = comment.User?.FullName ?? string.Empty,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }
    }
}
