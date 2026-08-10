using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMngBack.DTOs.Statuses;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Controllers
{
    [ApiController]
    [Route("api/task-statuses")]
    [Authorize]
    public class TaskStatusesController : ControllerBase
    {
        private readonly ITaskStatusService _taskStatusService;

        public TaskStatusesController(ITaskStatusService taskStatusService)
        {
            _taskStatusService = taskStatusService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskStatusDto>>> GetAll()
        {
            var statuses = await _taskStatusService.GetAllAsync();
            return Ok(statuses);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<TaskStatusDto>> Create(CreateTaskStatusDto dto)
        {
            var status = await _taskStatusService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = status.Id }, status);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<TaskStatusDto>> Update(Guid id, UpdateTaskStatusDefinitionDto dto)
        {
            var status = await _taskStatusService.UpdateAsync(id, dto);
            return Ok(status);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _taskStatusService.DeleteAsync(id);
            return NoContent();
        }
    }
}
