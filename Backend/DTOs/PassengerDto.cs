using Backend.Models;
using System.Text.Json.Serialization;

namespace Backend.DTOs
{
    public class PassengerDto
    {
        [JsonRequired]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [JsonRequired]
        public int Age { get; set; }
        public string? Gender { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? SeatNumber { get; set; }
        public string? PassportNumber { get; set; }
        public string? PhoneNumber { get; set; }
        [JsonRequired]
        public TravelClass TravelClass { get; set; }
    }
}
