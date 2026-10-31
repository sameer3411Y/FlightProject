using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public class PassengerRepository : IPassengerRepository
    {
        private readonly ApplicationDbContext _context;
        public PassengerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Passenger>> GetAllAsync()
        {
            return await _context.Passengers.ToListAsync();
        }

        public async Task<Passenger?> GetByIdAsync(int id)
        {
            return await _context.Passengers.FindAsync(id);
        }

        public async Task AddAsync(Passenger passenger)
        {
            _context.Passengers.Add(passenger);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Passenger passenger)
        {
            _context.Passengers.Update(passenger);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var passenger = await _context.Passengers.FindAsync(id);
            if (passenger != null)
            {
                _context.Passengers.Remove(passenger);
                await _context.SaveChangesAsync();
            }
        }
    }
}
