using TaskMngBack.DTOs.Users;

namespace TaskMngBack.DTOs.Departments
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<UserSummaryDto> Users { get; set; } = new();
    }
}
