using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.Models
{
    public class Attachment
    {
        public Guid Id { get; set; }

        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        public long FileSize { get; set; }

        [MaxLength(100)]
        public string ContentType { get; set; } = string.Empty;

        [MaxLength(500)]
        public string StorageKey { get; set; } = string.Empty;

        public int TaskId { get; set; }
        public TaskItem Task { get; set; } = null!;

        public Guid? CommentId { get; set; }
        public Comment? Comment { get; set; }

        public int UploadedByUserId { get; set; }
        public User UploadedByUser { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
