namespace TaskMngBack.DTOs.Sprints
{
    public class SprintDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TaskCount { get; set; }
    }
}
