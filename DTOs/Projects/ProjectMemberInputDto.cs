using System.ComponentModel.DataAnnotations;
using TaskMngBack.Models.Enums;

namespace TaskMngBack.DTOs.Projects
{
    public class ProjectMemberInputDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [EnumDataType(typeof(ProjectMemberRole))]
        public string Role { get; set; } = string.Empty;
    }
}
