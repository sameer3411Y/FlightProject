using Backend.Models;

namespace Backend.DTOs
{
    public class SeatDto
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public TravelClass SeatClass { get; set; }
        public string SeatType { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public bool IsBlocked { get; set; }
        public decimal BasePrice { get; set; }
        public decimal? ExtraFee { get; set; }
    }

    public class SeatMapDto
    {
        public int FlightId { get; set; }
        public List<SeatDto> Seats { get; set; } = new();
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public int OccupiedSeats { get; set; }
    }

    public class SeatSelectionDto
    {
        public int FlightId { get; set; }
        public List<string> SeatNumbers { get; set; } = new();
        public int BookingId { get; set; }
    }
}