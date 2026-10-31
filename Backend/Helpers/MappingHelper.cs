using Backend.DTOs;
using Backend.Models;

namespace Backend.Helpers
{
    public static class MappingHelper
    {
        public static FlightDto ToFlightDto(Flight flight)
        {
            return new FlightDto
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                Airline = flight.Airline,
                Origin = flight.Origin,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                EconomyPrice = flight.EconomyPrice,
                PremiumEconomyPrice = flight.PremiumEconomyPrice,
                BusinessPrice = flight.BusinessPrice,
                FirstClassPrice = flight.FirstClassPrice,
                TotalSeats = flight.TotalSeats,
                AvailableSeats = flight.AvailableSeats,
                Duration = flight.Duration,
                Stops = flight.Stops,
                IsRefundable = flight.IsRefundable,
                Status = flight.Status
            };
        }

        public static BookingHistoryDto ToBookingHistoryDto(Booking booking)
        {
            return new BookingHistoryDto
            {
                Id = booking.Id,
                BookingReference = booking.BookingReference,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status,
                FlightNumber = booking.Flight?.FlightNumber ?? "",
                Airline = booking.Flight?.Airline ?? "",
                Origin = booking.Flight?.Origin ?? "",
                Destination = booking.Flight?.Destination ?? "",
                DepartureTime = booking.Flight?.DepartureTime ?? DateTime.MinValue,
                ArrivalTime = booking.Flight?.ArrivalTime ?? DateTime.MinValue,
                PassengerCount = booking.PassengerCount,
                TravelClass = booking.TravelClass
            };
        }

        public static void UpdateFlightFromDto(Flight flight, CreateFlightDto dto)
        {
            flight.FlightNumber = dto.FlightNumber;
            flight.Airline = dto.Airline;
            flight.Origin = dto.Origin;
            flight.Destination = dto.Destination;
            flight.DepartureTime = dto.DepartureTime;
            flight.ArrivalTime = dto.ArrivalTime;
            flight.EconomyPrice = dto.EconomyPrice;
            flight.PremiumEconomyPrice = dto.PremiumEconomyPrice;
            flight.BusinessPrice = dto.BusinessPrice;
            flight.FirstClassPrice = dto.FirstClassPrice;
            flight.TotalSeats = dto.TotalSeats;
            flight.Duration = dto.Duration;
            flight.Stops = dto.Stops;
            flight.IsRefundable = dto.IsRefundable;
        }
    }
}