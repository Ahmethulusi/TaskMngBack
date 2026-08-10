using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMngBack.DTOs.Labels;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Controllers
{
    [ApiController]
    [Route("api/labels")]
    [Authorize]
    public class LabelsController : ControllerBase
    {
        private readonly ILabelService _labelService;

        public LabelsController(ILabelService labelService)
        {
            _labelService = labelService;
        }

        [HttpGet]
        public async Task<ActionResult<List<LabelDto>>> GetAll()
        {
            var labels = await _labelService.GetAllAsync();
            return Ok(labels);
        }

        [HttpPost]
        public async Task<ActionResult<LabelDto>> Create(CreateLabelDto dto)
        {
            var label = await _labelService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = label.Id }, label);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<LabelDto>> Update(Guid id, UpdateLabelDto dto)
        {
            var label = await _labelService.UpdateAsync(id, dto);
            return Ok(label);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _labelService.DeleteAsync(id);
            return NoContent();
        }
    }
}
