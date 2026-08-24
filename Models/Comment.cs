namespace TaskMngBack.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public TaskItem Task { get; set; } = null!;
        public User User { get; set; } = null!;
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<CommentReaction> Reactions { get; set; } = new List<CommentReaction>();
        public ICollection<CommentMention> Mentions { get; set; } = new List<CommentMention>();
    }
}
