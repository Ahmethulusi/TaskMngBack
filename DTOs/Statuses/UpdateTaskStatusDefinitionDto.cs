using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Statuses
{
    public class UpdateTaskStatusDefinitionDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string ColorKey { get; set; } = string.Empty;

        [Required]
        public int DisplayOrder { get; set; }

        [Required]
        public bool IsDefault { get; set; }
    }
}
