using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMngBack.DTOs.Activity;
using TaskMngBack.DTOs.Tasks;
using TaskMngBack.Extensions;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IActivityLogService _activityLogService;

        public TasksController(ITaskService taskService, IActivityLogService activityLogService)
        {
            _taskService = taskService;
            _activityLogService = activityLogService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskDto>>> GetAll()
        {
            var tasks = await _taskService.GetAllForUser(GetCurrentUserId(), GetCurrentUserPermissions());
            return Ok(tasks);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskDto>> GetById(int id)
        {
            var task = await _taskService.GetByIdForUser(id, GetCurrentUserId(), GetCurrentUserPermissions());
            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> Create(CreateTaskDto dto)
        {
            var task = await _taskService.Create(dto, GetCurrentUserId(), GetCurrentUserPermissions());
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TaskDto>> Update(int id, UpdateTaskDto dto)
        {
            var task = await _taskService.Update(id, dto, GetCurrentUserId(), GetCurrentUserPermissions());
            return Ok(task);
        }

        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<TaskDto>> UpdateStatus(int id, UpdateTaskStatusDto dto)
        {
            var task = await _taskService.UpdateStatus(id, dto, GetCurrentUserId(), GetCurrentUserPermissions());
            return Ok(task);
        }

        [HttpPatch("{id:int}/labels")]
        public async Task<ActionResult<TaskDto>> UpdateLabels(int id, UpdateTaskLabelsDto dto)
        {
            var task = await _taskService.UpdateLabels(id, dto, GetCurrentUserId(), GetCurrentUserPermissions());
            return Ok(task);
        }

        [HttpPatch("{id:int}/assign")]
        public async Task<ActionResult<TaskDto>> Assign(int id, AssignTaskDto dto)
        {
            var task = await _taskService.AssignTask(id, dto.AssignedUserIds, GetCurrentUserId(), GetCurrentUserPermissions());
            return Ok(task);
        }

        [HttpPost("{taskId:int}/dependencies")]
        public async Task<IActionResult> AddDependency(int taskId, AddDependencyDto dto)
        {
            await _taskService.AddDependency(
                taskId,
                dto,
                GetCurrentUserId(),
                GetCurrentUserPermissions());
            return NoContent();
        }

        [HttpDelete("{taskId:int}/dependencies/{dependsOnTaskId:int}")]
        public async Task<IActionResult> RemoveDependency(int taskId, int dependsOnTaskId)
        {
            await _taskService.RemoveDependency(
                taskId,
                dependsOnTaskId,
                GetCurrentUserId(),
                GetCurrentUserPermissions());
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _taskService.Delete(id, GetCurrentUserId(), GetCurrentUserPermissions());
            return NoContent();
        }

        [HttpGet("{id:int}/activity")]
        public async Task<ActionResult<List<ActivityLogDto>>> GetActivity(int id)
        {
            var activityLogs = await _activityLogService.GetByTaskIdAsync(id, GetCurrentUserId(), GetCurrentUserPermissions());
            return Ok(activityLogs);
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
