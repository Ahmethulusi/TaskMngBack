using System.ComponentModel.DataAnnotations;
using TaskMngBack.Models.Enums;

namespace TaskMngBack.DTOs.Tasks
{
    public class UpdateTaskStatusDto
    {
        [Required]
        [EnumDataType(typeof(TaskItemStatus))]
        public string Status { get; set; } = string.Empty;
    }
}
