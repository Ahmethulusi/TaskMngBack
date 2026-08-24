namespace TaskMngBack.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int? RelatedTaskId { get; set; }
        public TaskItem? RelatedTask { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }
    }
}
