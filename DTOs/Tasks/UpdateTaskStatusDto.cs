using System.ComponentModel.DataAnnotations;
using TaskManager_Staj_Project.Models.Enums;

namespace TaskManager_Staj_Project.DTOs.Tasks
{
    public class UpdateTaskStatusDto
    {
        [Required]
        [EnumDataType(typeof(TaskItemStatus))]
        public string Status { get; set; } = string.Empty;
    }
}
