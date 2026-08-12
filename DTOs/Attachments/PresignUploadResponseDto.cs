namespace TaskMngBack.DTOs.Attachments
{
    public class PresignUploadResponseDto
    {
        public string UploadUrl { get; set; } = string.Empty;
        public string StorageKey { get; set; } = string.Empty;
    }
}
