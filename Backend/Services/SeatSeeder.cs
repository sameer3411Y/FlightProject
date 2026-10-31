using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class SeatSeeder
    {
        public static async Task SeedSeatsForFlightAsync(ApplicationDbContext context, int flightId)
        {
            // Get flight to determine total seats
            var flight = await context.Flights.FindAsync(flightId);
            if (flight == null) return;

            var totalSeats = flight.TotalSeats > 0 ? flight.TotalSeats : 180;
            
            // Check if seats already exist and if count matches
            var existingSeatsCount = await context.Seats.CountAsync(s => s.FlightId == flightId);
            if (existingSeatsCount == totalSeats)
                return; // Correct number of seats already exist
            
            // Remove existing seats if count doesn't match
            if (existingSeatsCount > 0)
            {
                var existingSeats = await context.Seats.Where(s => s.FlightId == flightId).ToListAsync();
                context.Seats.RemoveRange(existingSeats);
                Console.WriteLine($"Removed {existingSeatsCount} existing seats for flight {flightId}");
            }

            var seats = GenerateSeatsForFlight(flightId, totalSeats);
            context.Seats.AddRange(seats);
            await context.SaveChangesAsync();
            
            Console.WriteLine($"Generated {seats.Count} new seats for flight {flightId} with {totalSeats} total seats");
        }
        
        private static List<Seat> GenerateSeatsForFlight(int flightId, int totalSeats)
        {
            var seats = new List<Seat>();
            
            // Calculate seats per class based on percentages
            var firstSeats = (int)(totalSeats * 0.20); // 20%
            var businessSeats = (int)(totalSeats * 0.20); // 20%
            var premiumSeats = (int)(totalSeats * 0.20); // 20%
            var economySeats = totalSeats - firstSeats - businessSeats - premiumSeats; // 40%
            
            int seatCounter = 1;
            
            // Generate First Class seats
            seatCounter = GenerateSeatsForClass(seats, flightId, TravelClass.First, firstSeats, seatCounter);
            
            // Generate Business Class seats
            seatCounter = GenerateSeatsForClass(seats, flightId, TravelClass.Business, businessSeats, seatCounter);
            
            // Generate Premium Economy seats
            seatCounter = GenerateSeatsForClass(seats, flightId, TravelClass.PremiumEconomy, premiumSeats, seatCounter);
            
            // Generate Economy seats
            GenerateSeatsForClass(seats, flightId, TravelClass.Economy, economySeats, seatCounter);
            
            return seats;
        }
        
        private static int GenerateSeatsForClass(List<Seat> seats, int flightId, TravelClass seatClass, int seatCount, int startCounter)
        {
            int seatsPerRow = 6; // A, B, C, D, E, F
            int currentCounter = startCounter;
            
            for (int i = 0; i < seatCount; i++)
            {
                int row = (currentCounter - 1) / seatsPerRow + 1;
                char col = (char)('A' + ((currentCounter - 1) % seatsPerRow));
                var seatNumber = $"{row}{col}";
                
                seats.Add(new Seat
                {
                    FlightId = flightId,
                    SeatNumber = seatNumber,
                    SeatClass = seatClass,
                    IsOccupied = false
                });
                
                currentCounter++;
            }
            
            Console.WriteLine($"Generated {seatCount} seats for {seatClass} class (seats {startCounter} to {currentCounter-1})");
            return currentCounter;
        }
        

        

    }
}