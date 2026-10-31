using Backend.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.DTOs
{
    public class CreateBookingDto
    {
        [Required]
        [JsonRequired]
        public int UserId { get; set; }
        
        [Required]
        [JsonRequired]
        public int FlightId { get; set; }
        
        public TravelClass TravelClass { get; set; } = TravelClass.Economy;
        
        public string? SpecialRequests { get; set; }
        
        [Required]
        public List<CreatePassengerDto> Passengers { get; set; } = new();
    }
}