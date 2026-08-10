using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Tasks
{
    public class UpdateTaskStatusDto
    {
        [Required]
        public Guid StatusId { get; set; }
    }
}
