using System.ComponentModel.DataAnnotations;
using TaskMngBack.Models.Enums;

namespace TaskMngBack.DTOs.Tasks
{
    public class UpdateTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        [EnumDataType(typeof(TaskPriority))]
        public string Priority { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        public int? DepartmentId { get; set; }

        public Guid? ProjectId { get; set; }

        public Guid? SprintId { get; set; }

        public int? ParentTaskId { get; set; }
    }
}
