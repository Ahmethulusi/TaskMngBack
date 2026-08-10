namespace TaskMngBack.DTOs.Tasks
{
    public class UpdateTaskLabelsDto
    {
        public List<Guid> LabelIds { get; set; } = new();
    }
}
