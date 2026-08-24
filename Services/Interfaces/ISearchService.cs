using TaskMngBack.DTOs.Search;

namespace TaskMngBack.Services.Interfaces
{
    public interface ISearchService
    {
        Task<SearchResultsDto> Search(string query, int userId, List<string> permissions);
    }
}
