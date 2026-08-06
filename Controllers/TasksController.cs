using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager_Staj_Project.DTOs.Tasks;
using TaskManager_Staj_Project.Services.Interfaces;

namespace TaskManager_Staj_Project.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskDto>>> GetAll()
        {
            var tasks = await _taskService.GetAllForUser(GetCurrentUserId(), IsCurrentUserAdmin());
            return Ok(tasks);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskDto>> GetById(int id)
        {
            var task = await _taskService.GetByIdForUser(id, GetCurrentUserId(), IsCurrentUserAdmin());
            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> Create(CreateTaskDto dto)
        {
            var task = await _taskService.Create(dto, GetCurrentUserId(), IsCurrentUserAdmin());
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TaskDto>> Update(int id, UpdateTaskDto dto)
        {
            var task = await _taskService.Update(id, dto, GetCurrentUserId(), IsCurrentUserAdmin());
            return Ok(task);
        }

        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<TaskDto>> UpdateStatus(int id, UpdateTaskStatusDto dto)
        {
            var task = await _taskService.UpdateStatus(id, dto, GetCurrentUserId(), IsCurrentUserAdmin());
            return Ok(task);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _taskService.Delete(id, GetCurrentUserId(), IsCurrentUserAdmin());
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
