using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMngBack.DTOs.Search;
using TaskMngBack.Extensions;
using TaskMngBack.Services.Interfaces;

namespace TaskMngBack.Controllers
{
    [ApiController]
    [Route("api/search")]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<ActionResult<SearchResultsDto>> Search([FromQuery] string? q)
        {
            var results = await _searchService.Search(
                q ?? string.Empty,
                GetCurrentUserId(),
                GetCurrentUserPermissions());
            return Ok(results);
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
