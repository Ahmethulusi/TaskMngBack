namespace TaskMngBack.Models
{
    public class TaskDependency
    {
        public Guid Id { get; set; }
        public int TaskId { get; set; }
        public TaskItem Task { get; set; } = null!;
        public int DependsOnTaskId { get; set; }
        public TaskItem DependsOnTask { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
