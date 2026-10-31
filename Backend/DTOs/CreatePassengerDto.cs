namespace Backend.DTOs
{
    public class CreatePassengerDto
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? PassportNumber { get; set; }
        public string? SeatNumber { get; set; }
        public string? TravelClass { get; set; }
    }
}
