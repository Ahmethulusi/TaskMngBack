using TaskMngBack.DTOs.Attachments;

namespace TaskMngBack.Services.Interfaces
{
    public interface IAttachmentService
    {
        Task<PresignUploadResponseDto> PresignUpload(PresignUploadRequestDto dto, int userId, List<string> permissions);
        Task<AttachmentDto> Confirm(ConfirmAttachmentDto dto, int userId, List<string> permissions);
        Task<List<AttachmentDto>> GetForTask(int taskId, int userId, List<string> permissions);
        Task Delete(Guid attachmentId, int userId, List<string> permissions);
    }
}
