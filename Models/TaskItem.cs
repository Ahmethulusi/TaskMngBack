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

        public Guid? SprintId { get; set; }
        public Sprint? Sprint { get; set; }

        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;

        public int? ParentTaskId { get; set; }
        public TaskItem? ParentTask { get; set; }

        public ICollection<User> AssignedUsers { get; set; } = new List<User>();
        public ICollection<Label> Labels { get; set; } = new List<Label>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<TaskItem> Subtasks { get; set; } = new List<TaskItem>();
        public ICollection<TaskDependency> Dependencies { get; set; } = new List<TaskDependency>();
        public ICollection<TaskDependency> Blocking { get; set; } = new List<TaskDependency>();
    }
}
