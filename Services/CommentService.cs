using TaskMngBack.Constants;
using TaskMngBack.DTOs.Attachments;
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
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IStorageService _storageService;
        private readonly INotificationService _notificationService;

        public CommentService(
            ICommentRepository commentRepository,
            ITaskRepository taskRepository,
            IAttachmentRepository attachmentRepository,
            IStorageService storageService,
            INotificationService notificationService)
        {
            _commentRepository = commentRepository;
            _taskRepository = taskRepository;
            _attachmentRepository = attachmentRepository;
            _storageService = storageService;
            _notificationService = notificationService;
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
            var dtos = new List<CommentDto>(comments.Count);

            foreach (var comment in comments)
            {
                dtos.Add(await MapToDtoAsync(comment, userId));
            }

            return dtos;
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

            if (dto.AttachmentIds.Count > 0)
            {
                await _attachmentRepository.ClaimForCommentAsync(dto.AttachmentIds, comment.Id, taskId, userId);
            }

            var taskComments = await _commentRepository.GetByTaskIdAsync(taskId);
            var relatedUserIds = taskComments.Select(c => c.UserId)
                .Append(task.CreatedByUserId)
                .Concat(task.AssignedUsers.Select(u => u.Id))
                .ToHashSet();

            var mentionedUserIds = dto.MentionedUserIds
                .Distinct()
                .Where(relatedUserIds.Contains)
                .ToList();

            foreach (var mentionedUserId in mentionedUserIds)
            {
                await _commentRepository.AddMentionAsync(new CommentMention
                {
                    Id = Guid.NewGuid(),
                    CommentId = comment.Id,
                    MentionedUserId = mentionedUserId,
                    CreatedAt = DateTime.UtcNow
                });

                if (mentionedUserId != userId)
                {
                    await _notificationService.NotifyAsync(
                        mentionedUserId,
                        "Mention",
                        "Bir yorumda bahsedildiniz",
                        $"'{task.Title}' görevindeki bir yorumda sizden bahsedildi.",
                        task.Id);
                }
            }

            var created = await GetCommentOrThrowAsync(comment.Id);
            await NotifyNewCommentAsync(task, userId);
            return await MapToDtoAsync(created, userId);
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
            return await MapToDtoAsync(updated, userId);
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

        public async Task<CommentDto> ToggleReaction(
            Guid commentId,
            ToggleReactionDto dto,
            int userId,
            List<string> permissions)
        {
            if (!AllowedReactionEmojis.Values.Contains(dto.Emoji))
            {
                throw new BadRequestException("Geçersiz emoji");
            }

            var comment = await GetCommentOrThrowAsync(commentId);
            var task = await GetTaskOrThrowAsync(comment.TaskId);

            if (!permissions.Contains("tasks.view.all") &&
                task.CreatedByUserId != userId &&
                !task.AssignedUsers.Any(u => u.Id == userId) &&
                !(task.Project?.Members.Any(m => m.UserId == userId) ?? false) &&
                task.Department?.ManagerId != userId &&
                !(task.Department?.Users.Any(u => u.Id == userId) ?? false))
            {
                throw new ForbiddenAccessException("Bu görevin yorumlarını görüntüleme yetkiniz yok.");
            }

            var existing = await _commentRepository.GetReactionAsync(commentId, userId, dto.Emoji);
            if (existing is not null)
            {
                await _commentRepository.RemoveReactionAsync(existing);
            }
            else
            {
                await _commentRepository.AddReactionAsync(new CommentReaction
                {
                    Id = Guid.NewGuid(),
                    CommentId = commentId,
                    UserId = userId,
                    Emoji = dto.Emoji,
                    CreatedAt = DateTime.UtcNow
                });
            }

            var updated = await GetCommentOrThrowAsync(commentId);
            return await MapToDtoAsync(updated, userId);
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

        private async Task NotifyNewCommentAsync(TaskItem task, int commenterUserId)
        {
            var recipientIds = task.AssignedUsers
                .Select(u => u.Id)
                .Append(task.CreatedByUserId)
                .Distinct()
                .Where(id => id != commenterUserId);

            foreach (var recipientId in recipientIds)
            {
                await _notificationService.NotifyAsync(
                    recipientId,
                    "NewComment",
                    "Yeni yorum",
                    $"'{task.Title}' görevine yeni bir yorum eklendi.",
                    task.Id);
            }
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

        private async Task<CommentDto> MapToDtoAsync(Comment comment, int currentUserId)
        {
            var attachments = comment.Attachments ?? new List<Attachment>();
            var attachmentDtos = new List<AttachmentDto>(attachments.Count);

            foreach (var attachment in attachments)
            {
                attachmentDtos.Add(new AttachmentDto
                {
                    Id = attachment.Id,
                    FileName = attachment.FileName,
                    FileSize = attachment.FileSize,
                    ContentType = attachment.ContentType,
                    UploadedByUserId = attachment.UploadedByUserId,
                    UploadedByUserName = attachment.UploadedByUser?.FullName ?? string.Empty,
                    CreatedAt = attachment.CreatedAt,
                    DownloadUrl = await _storageService.GeneratePresignedDownloadUrlAsync(attachment.StorageKey)
                });
            }

            var reactions = comment.Reactions ?? new List<CommentReaction>();

            return new CommentDto
            {
                Id = comment.Id,
                TaskId = comment.TaskId,
                UserId = comment.UserId,
                UserFullName = comment.User?.FullName ?? string.Empty,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                Attachments = attachmentDtos,
                Reactions = reactions
                    .GroupBy(r => r.Emoji)
                    .Select(g => new ReactionSummaryDto
                    {
                        Emoji = g.Key,
                        Count = g.Count(),
                        ReactedByMe = g.Any(r => r.UserId == currentUserId)
                    })
                    .ToList(),
                MentionedUsers = (comment.Mentions ?? new List<CommentMention>())
                    .Select(m => new MentionedUserDto
                    {
                        Id = m.MentionedUserId,
                        FullName = m.MentionedUser?.FullName ?? string.Empty
                    })
                    .ToList()
            };
        }
    }
}
