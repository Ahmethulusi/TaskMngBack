using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMngBack.DTOs.Projects;
using TaskMngBack.Extensions;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Controllers
{
    [ApiController]
    [Route("api/projects")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectDto>>> GetAll()
        {
            var projects = await _projectService.GetAllForUser(GetCurrentUserId(), GetCurrentUserPermissions());
            return Ok(projects);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProjectDto>> GetById(Guid id)
        {
            var project = await _projectService.GetByIdForUser(id, GetCurrentUserId(), GetCurrentUserPermissions());
            return Ok(project);
        }

        [HttpGet("{id:guid}/activity")]
        public async Task<ActionResult<List<ProjectActivityItemDto>>> GetActivity(Guid id)
        {
            var activity = await _projectService.GetActivity(
                id,
                GetCurrentUserId(),
                GetCurrentUserPermissions());
            return Ok(activity);
        }

        [Authorize(Policy = "Permission:projects.manage")]
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> Create(CreateProjectDto dto)
        {
            var project = await _projectService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProjectDto>> Update(Guid id, UpdateProjectDto dto)
        {
            var project = await _projectService.UpdateAsync(id, dto, GetCurrentUserId(), GetCurrentUserPermissions());
            return Ok(project);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _projectService.DeleteAsync(id, GetCurrentUserId(), GetCurrentUserPermissions());
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
