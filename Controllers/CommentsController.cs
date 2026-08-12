using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMngBack.DTOs.Comments;
using TaskMngBack.Extensions;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("tasks/{taskId:int}/comments")]
        public async Task<ActionResult<List<CommentDto>>> GetForTask(int taskId)
        {
            var comments = await _commentService.GetForTask(taskId, GetCurrentUserId(), GetCurrentUserPermissions());
            return Ok(comments);
        }

        [HttpPost("tasks/{taskId:int}/comments")]
        public async Task<ActionResult<CommentDto>> Create(int taskId, CreateCommentDto dto)
        {
            var comment = await _commentService.CreateAsync(taskId, dto, GetCurrentUserId(), GetCurrentUserPermissions());
            return CreatedAtAction(nameof(GetForTask), new { taskId }, comment);
        }

        [HttpPut("comments/{id:guid}")]
        public async Task<ActionResult<CommentDto>> Update(Guid id, UpdateCommentDto dto)
        {
            var comment = await _commentService.UpdateAsync(id, dto, GetCurrentUserId());
            return Ok(comment);
        }

        [HttpDelete("comments/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _commentService.DeleteAsync(id, GetCurrentUserId(), GetCurrentUserPermissions());
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
