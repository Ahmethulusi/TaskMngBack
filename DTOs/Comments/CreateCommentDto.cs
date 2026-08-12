using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Comments
{
    public class CreateCommentDto
    {
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        public List<Guid> AttachmentIds { get; set; } = new();
    }
}
