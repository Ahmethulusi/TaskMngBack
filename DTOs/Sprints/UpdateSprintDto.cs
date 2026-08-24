using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Sprints
{
    public class UpdateSprintDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
