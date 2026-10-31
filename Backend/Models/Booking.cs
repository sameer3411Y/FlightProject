using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Booking
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }
        [Required]
        public int FlightId { get; set; }
        public Flight? Flight { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        [Required]
        public DateTime BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public TravelClass TravelClass { get; set; } = TravelClass.Economy;
        public int PassengerCount { get; set; }
        public string Status { get; set; } = "Confirmed";
        public string? SpecialRequests { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Passenger>? Passengers { get; set; }
    }
}
