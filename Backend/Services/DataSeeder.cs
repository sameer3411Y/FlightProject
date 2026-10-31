using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!await context.Users.AnyAsync(u => u.Email == "admin@flight.com"))
            {
                var adminUser = new User
                {
                    Email = "admin@flight.com",
                    FirstName = "Admin",
                    LastName = "User",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin",
                    IsActive = true
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}