using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        public string? LastName { get; set; }
        
        public string? Gender { get; set; }
        
        public string? MobileNumber { get; set; }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
    }

    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string OTP { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }
}