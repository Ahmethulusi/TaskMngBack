using TaskMngBack.Models.Enums;

namespace TaskMngBack.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid StatusId { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DueDate { get; set; }

        public TaskStatusDefinition StatusDefinition { get; set; } = null!;

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public Guid? ProjectId { get; set; }
        public Project? Project { get; set; }

        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;

        public ICollection<User> AssignedUsers { get; set; } = new List<User>();
        public ICollection<Label> Labels { get; set; } = new List<Label>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    }
}
