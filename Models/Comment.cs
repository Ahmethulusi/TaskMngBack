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
    }
}
