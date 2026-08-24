namespace TaskMngBack.DTOs.Search
{
    public class SearchResultsDto
    {
        public List<SearchResultItemDto> Tasks { get; set; } = new();
        public List<SearchResultItemDto> Projects { get; set; } = new();
        public List<SearchResultItemDto> Departments { get; set; } = new();
    }
}
