namespace TaskMngBack.Models
{
    public class TaskStatusDefinition
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public string ColorKey { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}
