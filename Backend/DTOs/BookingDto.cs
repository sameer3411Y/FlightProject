using System;
using System.Collections.Generic;
using Backend.Models;

namespace Backend.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FlightId { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public TravelClass TravelClass { get; set; }
        public int PassengerCount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? SpecialRequests { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PassengerDto> Passengers { get; set; } = new();
    }

    public class BookingHistoryDto
    {
        public int Id { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int PassengerCount { get; set; }
        public TravelClass TravelClass { get; set; }
    }
}
