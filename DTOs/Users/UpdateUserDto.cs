using System.ComponentModel.DataAnnotations;
using TaskManager_Staj_Project.Models.Enums;

namespace TaskManager_Staj_Project.DTOs.Users
{
    public class UpdateUserDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(UserRole))]
        public string Role { get; set; } = string.Empty;
    }
}
