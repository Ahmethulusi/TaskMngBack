using System.ComponentModel.DataAnnotations;

namespace TaskMngBack.DTOs.Users
{
    public class UpdateOwnProfileDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
