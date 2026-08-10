using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Departments
{
    public class UpdateDepartmentDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public List<int> UserIds { get; set; } = new();
    }
}
