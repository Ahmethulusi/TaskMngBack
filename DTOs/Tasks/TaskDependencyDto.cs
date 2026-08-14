namespace TaskMngBack.DTOs.Tasks
{
    public class TaskDependencyDto
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string StatusColorKey { get; set; } = string.Empty;
        public bool IsCompletionStatus { get; set; }
    }
}
