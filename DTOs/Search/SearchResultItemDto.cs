namespace TaskMngBack.DTOs.Search
{
    public class SearchResultItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
