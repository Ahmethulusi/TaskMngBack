using TaskManager_Staj_Project.Models.Enums;

namespace TaskManager_Staj_Project.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskItemStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;

        public int? AssignedToUserId { get; set; }
        public User? AssignedToUser { get; set; }
    }
}
