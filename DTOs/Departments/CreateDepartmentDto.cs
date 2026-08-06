using System.ComponentModel.DataAnnotations;

namespace TaskManager_Staj_Project.DTOs.Departments
{
    public class CreateDepartmentDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
