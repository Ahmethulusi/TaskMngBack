using TaskMngBack.DTOs.Labels;
using TaskMngBack.DTOs.Users;

namespace TaskMngBack.DTOs.Tasks
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string StatusColorKey { get; set; } = string.Empty;
        public bool IsCompletionStatus { get; set; }
        public string Priority { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsOverdue { get; set; }
        public string? DueUrgency { get; set; }

        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }

        public Guid? ProjectId { get; set; }
        public string? ProjectName { get; set; }

        public int? ParentTaskId { get; set; }
        public string? ParentTaskTitle { get; set; }
        public List<SubtaskSummaryDto> Subtasks { get; set; } = new();
        public SubtaskProgressDto? SubtaskProgress { get; set; }

        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;

        public List<UserSummaryDto> AssignedUsers { get; set; } = new();
        public List<LabelDto> Labels { get; set; } = new();
        public List<TaskDependencyDto> BlockedBy { get; set; } = new();
        public List<TaskDependencyDto> Blocks { get; set; } = new();
        public bool IsBlocked { get; set; }
        public int CommentCount { get; set; }
        public int AttachmentCount { get; set; }
    }
}
