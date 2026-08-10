using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Labels
{
    public class CreateLabelDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
