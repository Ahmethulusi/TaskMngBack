namespace TaskMngBack.Models
{
    /// <summary>
    /// Görev değişikliklerini kaydeder.
    /// FieldName sabit değerleri: Created, Title, Description, Priority, Status, 
    /// DueDate, Department, Project, AssignedUsers, Labels
    /// </summary>
    public class ActivityLog
    {
        public Guid Id { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime CreatedAt { get; set; }

        public TaskItem Task { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
