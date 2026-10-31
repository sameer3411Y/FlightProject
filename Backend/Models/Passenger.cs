using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Passenger
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string? Gender { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? SeatNumber { get; set; }
        public string? PassportNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public TravelClass TravelClass { get; set; } = TravelClass.Economy;
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }
    }
}
