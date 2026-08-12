namespace TaskMngBack.DTOs.Attachments
{
    public class AttachmentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public int UploadedByUserId { get; set; }
        public string UploadedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string DownloadUrl { get; set; } = string.Empty;
    }
}
