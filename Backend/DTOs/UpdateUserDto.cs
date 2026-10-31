using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        public string? LastName { get; set; }
        
        public string? Gender { get; set; }
        
        public int? Age { get; set; }
        
        public string? MobileNumber { get; set; }
        
        public string Role { get; set; } = string.Empty;
        
        public bool? IsActive { get; set; }
    }
}