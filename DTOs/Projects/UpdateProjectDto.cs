using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Projects
{
    public class UpdateProjectDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public string? IconKey { get; set; }

        public List<ProjectMemberInputDto> Members { get; set; } = new();
    }
}
