namespace TaskMngBack.DTOs.Tasks
{
    public class SubtaskSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string StatusColorKey { get; set; } = string.Empty;
        public bool IsCompletionStatus { get; set; }
    }
}
