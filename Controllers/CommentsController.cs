using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMngBack.DTOs.Comments;
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
            var comments = await _commentService.GetForTask(taskId, GetCurrentUserId(), IsCurrentUserAdmin());
            return Ok(comments);
        }

        [HttpPost("tasks/{taskId:int}/comments")]
        public async Task<ActionResult<CommentDto>> Create(int taskId, CreateCommentDto dto)
        {
            var comment = await _commentService.CreateAsync(taskId, dto, GetCurrentUserId(), IsCurrentUserAdmin());
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
            await _commentService.DeleteAsync(id, GetCurrentUserId(), IsCurrentUserAdmin());
            return NoContent();
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private bool IsCurrentUserAdmin()
        {
            return User.IsInRole("Admin");
        }
    }
}
