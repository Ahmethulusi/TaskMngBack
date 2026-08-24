namespace TaskMngBack.DTOs.Notifications
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int? RelatedTaskId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
