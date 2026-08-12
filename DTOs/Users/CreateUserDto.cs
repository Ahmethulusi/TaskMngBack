using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Users
{
    public class CreateUserDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "En az bir rol seçilmeli.")]
        public List<Guid> RoleIds { get; set; } = new();

        public List<int> DepartmentIds { get; set; } = new();
    }
}
