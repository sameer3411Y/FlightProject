using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public int? Age { get; set; }
        [MaxLength(20)]
        public string? MobileNumber { get; set; }
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty; // "User" or "Admin"
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Booking>? Bookings { get; set; }
    }
}
