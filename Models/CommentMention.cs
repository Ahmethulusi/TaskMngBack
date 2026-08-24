namespace TaskMngBack.Models
{
    public class CommentMention
    {
        public Guid Id { get; set; }
        public Guid CommentId { get; set; }
        public Comment Comment { get; set; } = null!;
        public int MentionedUserId { get; set; }
        public User MentionedUser { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
