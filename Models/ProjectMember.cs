using TaskMngBack.Models.Enums;

namespace TaskMngBack.Models
{
    public class ProjectMember
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public int UserId { get; set; }
        public ProjectMemberRole Role { get; set; }

        public Project Project { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
