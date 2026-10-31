using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class City
    {
        public int Id { get; set; }
        
        [Required]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string? Country { get; set; }
        
        public string? Airport { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}