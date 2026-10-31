using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Flight
    {
        public int Id { get; set; }
        [Required]
        public string FlightNumber { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        [Required]
        public string Origin { get; set; } = string.Empty;
        [Required]
        public string Destination { get; set; } = string.Empty;
        [Required]
        public DateTime DepartureTime { get; set; }
        [Required]
        public DateTime ArrivalTime { get; set; }
        [Required]
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        [Required]
        public decimal EconomyPrice { get; set; }
        public decimal? PremiumEconomyPrice { get; set; }
        public decimal? BusinessPrice { get; set; }
        public decimal? FirstClassPrice { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string Stops { get; set; } = "0";
        public bool IsRefundable { get; set; } = false;
        public string Status { get; set; } = "Active";
        public ICollection<Booking>? Bookings { get; set; }
    }
}
