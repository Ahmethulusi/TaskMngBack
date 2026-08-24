using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMngBack.DTOs.Sprints;
using TaskMngBack.Extensions;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class SprintsController : ControllerBase
    {
        private readonly ISprintService _sprintService;

        public SprintsController(ISprintService sprintService)
        {
            _sprintService = sprintService;
        }

        [HttpGet("projects/{projectId:guid}/sprints")]
        public async Task<ActionResult<List<SprintDto>>> GetByProject(Guid projectId)
        {
            var sprints = await _sprintService.GetByProject(
                projectId,
                GetCurrentUserId(),
                GetCurrentUserPermissions());
            return Ok(sprints);
        }

        [HttpPost("projects/{projectId:guid}/sprints")]
        public async Task<ActionResult<SprintDto>> Create(Guid projectId, CreateSprintDto dto)
        {
            var sprint = await _sprintService.Create(
                projectId,
                dto,
                GetCurrentUserId(),
                GetCurrentUserPermissions());
            return CreatedAtAction(nameof(GetByProject), new { projectId }, sprint);
        }

        [HttpPut("sprints/{id:guid}")]
        public async Task<ActionResult<SprintDto>> Update(Guid id, UpdateSprintDto dto)
        {
            var sprint = await _sprintService.Update(
                id,
                dto,
                GetCurrentUserId(),
                GetCurrentUserPermissions());
            return Ok(sprint);
        }

        [HttpDelete("sprints/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _sprintService.Delete(id, GetCurrentUserId(), GetCurrentUserPermissions());
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
