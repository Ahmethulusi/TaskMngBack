using System.ComponentModel.DataAnnotations;

namespace TaskManager_Staj_Project.DTOs.Departments
{
    public class UpdateDepartmentDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
