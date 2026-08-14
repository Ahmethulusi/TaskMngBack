using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Statuses
{
    public class CreateTaskStatusDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string ColorKey { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;
        public bool IsCompletionStatus { get; set; } = false;
    }
}
