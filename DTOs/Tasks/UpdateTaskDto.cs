using System.ComponentModel.DataAnnotations;
using TaskManager_Staj_Project.Models.Enums;

namespace TaskManager_Staj_Project.DTOs.Tasks
{
    public class UpdateTaskDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [EnumDataType(typeof(TaskPriority))]
        public string Priority { get; set; } = string.Empty;

        public int? DepartmentId { get; set; }
    }
}
