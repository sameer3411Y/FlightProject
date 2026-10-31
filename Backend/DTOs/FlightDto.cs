using System;

namespace Backend.DTOs
{
    public class FlightDto
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal EconomyPrice { get; set; }
        public decimal? PremiumEconomyPrice { get; set; }
        public decimal? BusinessPrice { get; set; }
        public decimal? FirstClassPrice { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string Stops { get; set; } = "0";
        public bool IsRefundable { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
