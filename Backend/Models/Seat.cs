using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Seat
    {
        public int Id { get; set; }
        
        [Required]
        public int FlightId { get; set; }
        
        [Required]
        public string SeatNumber { get; set; } = string.Empty;
        
        [Required]
        public TravelClass SeatClass { get; set; } = TravelClass.Economy;
        
        public bool IsOccupied { get; set; } = false;
        
        // Navigation property
        public virtual Flight Flight { get; set; } = null!;
    }
}