namespace TaskMngBack.DTOs.Statuses
{
    public class TaskStatusDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public string ColorKey { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}
