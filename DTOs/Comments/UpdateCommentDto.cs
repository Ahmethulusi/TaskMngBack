using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Comments
{
    public class UpdateCommentDto
    {
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;
    }
}
