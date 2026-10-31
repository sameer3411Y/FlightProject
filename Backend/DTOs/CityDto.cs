namespace Backend.DTOs
{
    public class CityDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Country { get; set; }
        public string? Airport { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCityDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Country { get; set; }
        public string? Airport { get; set; }
    }
}