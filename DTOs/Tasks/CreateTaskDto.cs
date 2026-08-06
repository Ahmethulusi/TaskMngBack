using System.ComponentModel.DataAnnotations;
using TaskManager_Staj_Project.Models.Enums;

namespace TaskManager_Staj_Project.DTOs.Tasks
{
    public class CreateTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        [EnumDataType(typeof(TaskPriority))]
        public string Priority { get; set; } = string.Empty;

        public int? DepartmentId { get; set; }

        public int? AssignedToUserId { get; set; }
    }
}
