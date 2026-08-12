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
        public string Priority { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DueDate { get; set; }

        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }

        public Guid? ProjectId { get; set; }
        public string? ProjectName { get; set; }

        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;

        public List<UserSummaryDto> AssignedUsers { get; set; } = new();
        public List<LabelDto> Labels { get; set; } = new();
        public int CommentCount { get; set; }
        public int AttachmentCount { get; set; }
    }
}
