using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.Models
{
    public class Permission
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Key { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
