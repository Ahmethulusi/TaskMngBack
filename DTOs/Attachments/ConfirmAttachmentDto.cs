using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Attachments
{
    public class ConfirmAttachmentDto
    {
        [Required]
        public string StorageKey { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public long FileSize { get; set; }

        [Required]
        public string ContentType { get; set; } = string.Empty;

        [Required]
        public int TaskId { get; set; }
    }
}
