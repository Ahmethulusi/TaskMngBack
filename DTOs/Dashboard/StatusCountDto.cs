namespace TaskMngBack.DTOs.Dashboard
{
    public class StatusCountDto
    {
        public string StatusName { get; set; } = string.Empty;
        public string StatusColorKey { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
