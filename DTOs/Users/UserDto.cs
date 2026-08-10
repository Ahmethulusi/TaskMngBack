using TaskMngBack.DTOs.Departments;

namespace TaskMngBack.DTOs.Users
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<DepartmentDto> Departments { get; set; } = new();
    }
}
