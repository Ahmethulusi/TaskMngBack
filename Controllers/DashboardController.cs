using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMngBack.DTOs.Dashboard;
using TaskMngBack.Extensions;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
        {
            var summary = await _dashboardService.GetSummary(
                GetCurrentUserId(),
                GetCurrentUserPermissions());
            return Ok(summary);
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
