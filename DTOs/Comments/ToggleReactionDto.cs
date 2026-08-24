using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Comments
{
    public class ToggleReactionDto
    {
        [Required]
        public string Emoji { get; set; } = string.Empty;
    }
}
