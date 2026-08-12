using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Attachments
{
    public class PresignUploadRequestDto
    {
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string ContentType { get; set; } = string.Empty;

        [Required]
        public long FileSizeBytes { get; set; }

        [Required]
        public int TaskId { get; set; }
    }
}
