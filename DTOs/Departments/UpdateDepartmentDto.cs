using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Departments
{
    public class UpdateDepartmentDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int? ManagerId { get; set; }

        public List<int> UserIds { get; set; } = new();
    }
}
