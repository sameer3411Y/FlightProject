using Backend.Models;

namespace Backend.DTOs
{
    public class SeatAvailabilityRequest
    {
        public int FlightId { get; set; }
        public List<SeatRequest> Seats { get; set; } = new List<SeatRequest>();
    }

    public class SeatRequest
    {
        public string SeatNumber { get; set; } = string.Empty;
        public TravelClass SeatClass { get; set; }
    }


}