using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Seat> Seats { get; set; }
    }
}
