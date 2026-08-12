using TaskMngBack.DTOs.Attachments;

namespace TaskMngBack.DTOs.Comments
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<AttachmentDto> Attachments { get; set; } = new();
    }
}
