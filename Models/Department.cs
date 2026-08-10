namespace TaskMngBack.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int? ManagerId { get; set; }
        public User? Manager { get; set; }

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
