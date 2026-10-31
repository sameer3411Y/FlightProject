using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.DTOs
{
    public class CreateFlightDto
    {
        [Required]
        public string FlightNumber { get; set; } = string.Empty;
        
        public string Airline { get; set; } = string.Empty;
        
        [Required]
        public string Origin { get; set; } = string.Empty;
        
        [Required]
        public string Destination { get; set; } = string.Empty;
        
        [Required]
        [JsonRequired]
        public DateTime DepartureTime { get; set; }
        
        [Required]
        [JsonRequired]
        public DateTime ArrivalTime { get; set; }
        
        [Required]
        [JsonRequired]
        public int TotalSeats { get; set; }
        
        [Required]
        [JsonRequired]
        public decimal EconomyPrice { get; set; }
        
        public decimal? PremiumEconomyPrice { get; set; }
        
        public decimal? BusinessPrice { get; set; }
        
        public decimal? FirstClassPrice { get; set; }
        
        public string Duration { get; set; } = string.Empty;
        
        public string Stops { get; set; } = "0";
        
        public bool IsRefundable { get; set; } = false;
    }
}