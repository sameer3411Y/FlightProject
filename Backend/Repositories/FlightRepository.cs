using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly ApplicationDbContext _context;
        public FlightRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Flight>> GetAllAsync()
        {
            return await _context.Flights.ToListAsync();
        }

        public async Task<Flight?> GetByIdAsync(int id)
        {
            return await _context.Flights.FindAsync(id);
        }

        public async Task AddAsync(Flight flight)
        {
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Flight flight)
        {
            _context.Flights.Update(flight);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Flight>> SearchAsync(string origin, string destination, DateTime date)
        {
            return await _context.Flights
                .Where(f => f.Origin == origin && f.Destination == destination && f.DepartureTime.Date == date.Date)
                .ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
                await _context.SaveChangesAsync();
            }
        }
    }
}
