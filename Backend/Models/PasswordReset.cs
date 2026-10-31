using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class PasswordReset
    {
        public int Id { get; set; }
        
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string OTP { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(10);
        
        public bool IsUsed { get; set; } = false;
    }
}