using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMngBack.DTOs.Attachments;
using TaskMngBack.Extensions;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class AttachmentsController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;

        public AttachmentsController(IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpPost("attachments/presign")]
        public async Task<ActionResult<PresignUploadResponseDto>> Presign(PresignUploadRequestDto dto)
        {
            var result = await _attachmentService.PresignUpload(dto, GetCurrentUserId(), GetCurrentUserPermissions());
            return Ok(result);
        }

        [HttpPost("attachments/confirm")]
        public async Task<ActionResult<AttachmentDto>> Confirm(ConfirmAttachmentDto dto)
        {
            var attachment = await _attachmentService.Confirm(dto, GetCurrentUserId(), GetCurrentUserPermissions());
            return Ok(attachment);
        }

        [HttpGet("tasks/{taskId:int}/attachments")]
        public async Task<ActionResult<List<AttachmentDto>>> GetForTask(int taskId)
        {
            var attachments = await _attachmentService.GetForTask(taskId, GetCurrentUserId(), GetCurrentUserPermissions());
            return Ok(attachments);
        }

        [HttpDelete("attachments/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _attachmentService.Delete(id, GetCurrentUserId(), GetCurrentUserPermissions());
            return NoContent();
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private List<string> GetCurrentUserPermissions()
        {
            return User.GetPermissions();
        }
    }
}
