namespace TaskMngBack.DTOs.Projects
{
    public class ProjectActivityItemDto
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
