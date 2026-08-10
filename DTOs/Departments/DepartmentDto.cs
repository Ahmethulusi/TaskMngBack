using TaskMngBack.DTOs.Users;

namespace TaskMngBack.DTOs.Departments
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public List<UserSummaryDto> Users { get; set; } = new();
    }
}
