using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Tasks
{
    public class AddDependencyDto
    {
        [Required]
        public int DependsOnTaskId { get; set; }
    }
}
