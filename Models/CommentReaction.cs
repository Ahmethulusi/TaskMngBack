namespace TaskMngBack.Models
{
    public class CommentReaction
    {
        public Guid Id { get; set; }
        public Guid CommentId { get; set; }
        public Comment Comment { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string Emoji { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
