namespace TaskMngBack.Models
{
    public class Label
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
