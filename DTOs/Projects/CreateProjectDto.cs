using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Projects
{
    public class CreateProjectDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public List<ProjectMemberInputDto> Members { get; set; } = new();
    }
}
