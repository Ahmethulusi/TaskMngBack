using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Roles
{
    public class UpdateRoleDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public List<Guid> PermissionIds { get; set; } = new();
    }
}
