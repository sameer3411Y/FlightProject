using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using Backend.Services;
using Backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "Admin")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly ApplicationDbContext _context;

        private readonly EmailService _emailService;

        public BookingsController(IBookingRepository bookingRepository, IFlightRepository flightRepository, ApplicationDbContext context, EmailService emailService)
        {
            _bookingRepository = bookingRepository;
            _flightRepository = flightRepository;
            _context = context;
            _emailService = emailService;
            Console.WriteLine("BookingsController initialized with EmailService: " + (_emailService != null ? "SUCCESS" : "FAILED"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            var bookingDtos = bookings.Select(MappingHelper.ToBookingHistoryDto);
            return Ok(bookingDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return NotFound();

            var bookingDto = new BookingDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                FlightId = booking.FlightId,
                BookingReference = booking.BookingReference,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                TravelClass = booking.TravelClass,
                PassengerCount = booking.PassengerCount,
                Status = booking.Status,
                SpecialRequests = booking.SpecialRequests,
                CreatedAt = booking.CreatedAt,
                Passengers = booking.Passengers.Select(p => new PassengerDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Age = p.Age,
                    Gender = p.Gender,
                    Email = p.Email,
                    SeatNumber = p.SeatNumber,
                    TravelClass = p.TravelClass
                }).ToList()
            };
            return Ok(bookingDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
        {
            // Get flight to calculate amount
            var flight = await _flightRepository.GetByIdAsync(dto.FlightId);
            if (flight == null) return BadRequest("Flight not found");
            
            // Ensure TravelClass has a valid value (default to Economy if 0)
            if (dto.TravelClass == 0 || !Enum.IsDefined(typeof(TravelClass), dto.TravelClass))
            {
                dto.TravelClass = TravelClass.Economy;
            }
            
            // Calculate total amount based on travel class
            decimal pricePerPassenger = dto.TravelClass switch
            {
                TravelClass.Economy => flight.EconomyPrice,
                TravelClass.PremiumEconomy => flight.PremiumEconomyPrice ?? flight.EconomyPrice * 1.5m,
                TravelClass.Business => flight.BusinessPrice ?? flight.EconomyPrice * 2,
                TravelClass.First => flight.FirstClassPrice ?? flight.EconomyPrice * 3,
                _ => flight.EconomyPrice
            };
            
            decimal totalAmount = pricePerPassenger * dto.Passengers.Count;
            
            Console.WriteLine($"Flight Price: {pricePerPassenger}, Passengers: {dto.Passengers.Count}, Total: {totalAmount}");
            Console.WriteLine($"TravelClass: {dto.TravelClass} (value: {(int)dto.TravelClass})");
            
            var booking = new Booking();
            booking.UserId = dto.UserId;
            booking.FlightId = dto.FlightId;
            booking.BookingReference = Guid.NewGuid().ToString("N")[..8].ToUpper();
            booking.TotalAmount = totalAmount;
            booking.TravelClass = dto.TravelClass;
            booking.PassengerCount = dto.Passengers.Count;
            booking.Status = "Confirmed";
            booking.SpecialRequests = dto.SpecialRequests;
            booking.BookingDate = DateTime.UtcNow;
            booking.CreatedAt = DateTime.UtcNow;
            
            booking.Passengers = dto.Passengers.Select(p => new Passenger
            {
                Name = p.Name,
                Age = p.Age,
                Gender = p.Gender,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                PassportNumber = p.PassportNumber,
                SeatNumber = p.SeatNumber,
                TravelClass = dto.TravelClass
            }).ToList();
            
            Console.WriteLine($"Final booking TravelClass before save: {booking.TravelClass} (int: {(int)booking.TravelClass})");

            try
            {
                // Ensure seats exist for this flight
                await SeatSeeder.SeedSeatsForFlightAsync(_context, dto.FlightId);
                
                // Debug: Log seat count for this flight
                var seatCount = await _context.Seats.CountAsync(s => s.FlightId == dto.FlightId);
                Console.WriteLine($"Flight {dto.FlightId} has {seatCount} seats in database");
                
                // Mark selected seats as occupied
                var seatNumbers = dto.Passengers.Where(p => !string.IsNullOrEmpty(p.SeatNumber)).Select(p => p.SeatNumber).ToList();
                if (seatNumbers.Any())
                {
                    var seats = await _context.Seats.Where(s => s.FlightId == dto.FlightId && seatNumbers.Contains(s.SeatNumber) && !s.IsOccupied).ToListAsync();
                    
                    // Check if any seats are already occupied or don't exist
                    if (seats.Count != seatNumbers.Count)
                    {
                        var foundSeats = seats.Select(s => s.SeatNumber).ToList();
                        var notFoundSeats = seatNumbers.Except(foundSeats).ToList();
                        
                        // Check which seats don't exist vs which are occupied
                        var allSeatsForFlight = await _context.Seats.Where(s => s.FlightId == dto.FlightId && seatNumbers.Contains(s.SeatNumber)).ToListAsync();
                        var occupiedSeats = allSeatsForFlight.Where(s => s.IsOccupied).Select(s => s.SeatNumber).ToList();
                        var invalidSeats = seatNumbers.Except(allSeatsForFlight.Select(s => s.SeatNumber)).ToList();
                        
                        if (invalidSeats.Any())
                        {
                            return BadRequest($"Invalid seat numbers: {string.Join(", ", invalidSeats)}. Valid seats are like 1A, 1B, 2C, etc.");
                        }
                        
                        if (occupiedSeats.Any())
                        {
                            return BadRequest($"Seats already occupied: {string.Join(", ", occupiedSeats)}");
                        }
                    }
                    
                    foreach (var seat in seats)
                    {
                        seat.IsOccupied = true;
                        Console.WriteLine($"Marking seat {seat.SeatNumber} as occupied for flight {dto.FlightId}");
                    }
                    
                    // Explicitly update seats
                    _context.Seats.UpdateRange(seats);
                }
                
                await _bookingRepository.AddAsync(booking);
                
                // Save everything in one transaction
                var savedRows = await _context.SaveChangesAsync();
                Console.WriteLine($"Transaction completed. Affected rows: {savedRows}");
                
                // Verify seats were actually updated in database
                var verifySeats = await _context.Seats
                    .Where(s => s.FlightId == dto.FlightId && seatNumbers.Contains(s.SeatNumber))
                    .ToListAsync();
                    
                foreach (var seat in verifySeats)
                {
                    Console.WriteLine($"VERIFICATION - Seat {seat.SeatNumber} IsOccupied: {seat.IsOccupied}");
                }
                
                // Send booking confirmation email with passenger details
                try
                {
                    var emailAddress = dto.Passengers.FirstOrDefault()?.Email;
                    Console.WriteLine($"Passenger email found: {emailAddress}");
                    Console.WriteLine($"Total passengers: {dto.Passengers.Count}");
                    
                    if (!string.IsNullOrEmpty(emailAddress))
                    {
                        Console.WriteLine($"NEW TEMPLATE: Sending booking confirmation to: {emailAddress}");
                        
                        var passengerCards = string.Join("", dto.Passengers.Select((p, i) => $@"
                            <div class='hover-scale fadeInUp' style='background: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%); border-radius: 16px; padding: 25px; margin-bottom: 20px; border-left: 5px solid #0ea5e9; box-shadow: 0 4px 15px rgba(0,0,0,0.1); position: relative; overflow: hidden;'>
                                <div style='position: absolute; top: -10px; right: -10px; width: 40px; height: 40px; background: linear-gradient(135deg, #0ea5e9, #0284c7); border-radius: 50%; opacity: 0.1;'></div>
                                <div style='margin-bottom: 20px; display: flex; align-items: center;'>
                                    <div class='pulse' style='background: linear-gradient(135deg, #0ea5e9 0%, #0284c7 100%); color: white; width: 40px; height: 40px; border-radius: 50%; display: inline-flex; align-items: center; justify-content: center; font-weight: bold; margin-right: 15px; box-shadow: 0 4px 12px rgba(14, 165, 233, 0.3);'>{i + 1}</div>
                                    <span style='color: #0f172a; font-size: 20px; font-weight: 700;'>{p.Name}</span>
                                </div>
                                <div style='display: grid; grid-template-columns: 1fr 1fr; gap: 20px;'>
                                    <div>
                                        <div style='color: #475569; font-size: 12px; font-weight: 700; text-transform: uppercase; margin-bottom: 6px; letter-spacing: 0.5px;'>📧 Email</div>
                                        <div style='color: #0f172a; font-weight: 600;'>{p.Email}</div>
                                    </div>
                                    <div>
                                        <div style='color: #475569; font-size: 12px; font-weight: 700; text-transform: uppercase; margin-bottom: 6px; letter-spacing: 0.5px;'>📱 Phone</div>
                                        <div style='color: #0f172a; font-weight: 600;'>{p.PhoneNumber}</div>
                                    </div>
                                    <div>
                                        <div style='color: #475569; font-size: 12px; font-weight: 700; text-transform: uppercase; margin-bottom: 6px; letter-spacing: 0.5px;'>⚥ Gender</div>
                                        <div style='color: #0f172a; font-weight: 600;'>{p.Gender}</div>
                                    </div>
                                    <div>
                                        <div style='color: #475569; font-size: 12px; font-weight: 700; text-transform: uppercase; margin-bottom: 6px; letter-spacing: 0.5px;'>🎫 Seat</div>
                                        <div class='glow pulse' style='background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; padding: 8px 16px; border-radius: 20px; display: inline-block; font-weight: bold; font-size: 14px; box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);'>{p.SeatNumber}</div>
                                    </div>
                                </div>
                            </div>
                        "));
                        
                        var htmlMessage = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>SkyWings Booking Confirmation</title>
    <style>
    @keyframes bounce {{ 0%, 20%, 53%, 80%, 100% {{ transform: translate3d(0,0,0); }} 40%, 43% {{ transform: translate3d(0,-30px,0); }} 70% {{ transform: translate3d(0,-15px,0); }} 90% {{ transform: translate3d(0,-4px,0); }} }}
    @keyframes pulse {{ 0% {{ transform: scale(1); }} 50% {{ transform: scale(1.1); }} 100% {{ transform: scale(1); }} }}
    @keyframes fadeInUp {{ from {{ opacity: 0; transform: translateY(30px); }} to {{ opacity: 1; transform: translateY(0); }} }}
    @keyframes slideInRight {{ from {{ opacity: 0; transform: translateX(50px); }} to {{ opacity: 1; transform: translateX(0); }} }}
    @keyframes glow {{ 0% {{ box-shadow: 0 0 5px #10b981; }} 50% {{ box-shadow: 0 0 20px #10b981, 0 0 30px #10b981; }} 100% {{ box-shadow: 0 0 5px #10b981; }} }}
    @keyframes shake {{ 0%, 100% {{ transform: translateX(0); }} 10%, 30%, 50%, 70%, 90% {{ transform: translateX(-5px); }} 20%, 40%, 60%, 80% {{ transform: translateX(5px); }} }}
    .bounce {{ animation: bounce 2s infinite; }}
    .pulse {{ animation: pulse 2s infinite; }}
    .fadeInUp {{ animation: fadeInUp 1s ease-out; }}
    .slideInRight {{ animation: slideInRight 1s ease-out; }}
    .glow {{ animation: glow 2s infinite; }}
    .shake {{ animation: shake 0.5s ease-in-out; }}
    .hover-scale {{ transition: transform 0.3s ease; }}
    .hover-scale:hover {{ transform: scale(1.02); }}
    </style>
</head>
<body style='margin: 0; padding: 20px; font-family: Arial, sans-serif; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);'>
    <div style='max-width: 650px; margin: 0 auto; background: white; border-radius: 20px; overflow: hidden; box-shadow: 0 20px 40px rgba(0,0,0,0.15);'>
        
        <!-- Animated Header -->
        <div style='background: linear-gradient(135deg, #1e3a8a 0%, #3b82f6 50%, #06b6d4 100%); padding: 40px 30px; text-align: center; position: relative; overflow: hidden;'>
            <div style='position: absolute; top: -30px; right: -30px; width: 80px; height: 80px; background: rgba(255,255,255,0.1); border-radius: 50%;'></div>
            <div style='position: absolute; bottom: -20px; left: -20px; width: 50px; height: 50px; background: rgba(255,255,255,0.1); border-radius: 50%;'></div>
            
            <div class='bounce' style='font-size: 50px; margin-bottom: 10px;'>✈️</div>
            <h1 class='slideInRight' style='color: white; margin: 0; font-size: 36px; font-weight: 800;'>SkyWings</h1>
            <div class='fadeInUp' style='background: rgba(255,255,255,0.2); color: white; padding: 8px 20px; border-radius: 20px; display: inline-block; margin-top: 15px; font-weight: 600;'>📋 Booking Confirmation</div>
        </div>
        
        <!-- Content -->
        <div style='padding: 40px 35px;'>
            
            <!-- Success Section -->
            <div class='fadeInUp' style='text-align: center; margin-bottom: 35px;'>
                <div class='pulse glow' style='background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; width: 90px; height: 90px; border-radius: 50%; display: inline-flex; align-items: center; justify-content: center; font-size: 40px; margin-bottom: 20px; box-shadow: 0 10px 30px rgba(16, 185, 129, 0.3);'>🎉</div>
                <h2 style='color: #1f2937; margin: 0 0 10px 0; font-size: 28px; font-weight: 700;'>Booking Confirmed!</h2>
                <p style='color: #6b7280; font-size: 16px; margin: 0;'>Your flight has been successfully booked</p>
            </div>
            
            <!-- Flight Details Card -->
            <div class='slideInRight hover-scale' style='background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%); border-radius: 16px; padding: 25px; margin-bottom: 30px; border: 1px solid #e5e7eb; position: relative; overflow: hidden;'>
                <div style='position: absolute; top: -15px; right: -15px; width: 60px; height: 60px; background: linear-gradient(135deg, #3b82f6, #1d4ed8); border-radius: 50%; opacity: 0.1;'></div>
                <h3 class='bounce' style='color: #1f2937; margin: 0 0 20px 0; font-size: 20px; font-weight: 600;'>📋 Flight Details</h3>
                <table style='width: 100%;'>
                    <tr><td style='padding: 8px 0; color: #4a5568; font-weight: bold;'>Booking Reference:</td><td class='pulse' style='padding: 8px 0; color: #1f2937; font-weight: 700;'>{booking.BookingReference}</td></tr>
                    <tr><td style='padding: 8px 0; color: #4a5568; font-weight: bold;'>Route:</td><td style='padding: 8px 0; color: #1f2937; font-weight: 600;'>{flight.Origin} ✈️ {flight.Destination}</td></tr>
                    <tr><td style='padding: 8px 0; color: #4a5568; font-weight: bold;'>Departure:</td><td style='padding: 8px 0; color: #1f2937; font-weight: 600;'>{flight.DepartureTime:dd MMM yyyy, HH:mm}</td></tr>
                    <tr><td style='padding: 8px 0; color: #4a5568; font-weight: bold;'>Class:</td><td style='padding: 8px 0; color: #1f2937; font-weight: 600;'>{booking.TravelClass}</td></tr>
                    <tr><td style='padding: 8px 0; color: #4a5568; font-weight: bold;'>Total Amount:</td><td class='glow shake' style='padding: 8px 0; color: #dc2626; font-weight: bold; font-size: 20px;'>₹{booking.TotalAmount}</td></tr>
                </table>
            </div>
            
            <!-- Passenger Details -->
            <div class='fadeInUp' style='margin-bottom: 30px;'>
                <h3 class='bounce' style='color: #1f2937; margin: 0 0 25px 0; font-size: 22px; font-weight: 700;'>👥 Passenger Details</h3>
                {passengerCards}
            </div>
            
            <!-- Important Notes -->
            <div class='slideInRight hover-scale' style='background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%); border: 2px solid #f59e0b; border-radius: 16px; padding: 25px; margin-bottom: 30px;'>
                <h3 class='shake' style='color: #92400e; margin: 0 0 15px 0; font-size: 18px; font-weight: 700;'>⚠️ Important Travel Notes</h3>
                <ul style='color: #78350f; margin: 0; padding-left: 20px; line-height: 1.6;'>
                    <li style='margin-bottom: 8px;'>🕐 Arrive at airport 2 hours before departure</li>
                    <li style='margin-bottom: 8px;'>🆔 Carry valid photo ID for verification</li>
                    <li style='margin-bottom: 8px;'>🧳 Check baggage allowance before travel</li>
                    <li>📱 Keep this confirmation handy during travel</li>
                </ul>
            </div>
            
            <!-- Footer -->
            <div class='fadeInUp' style='background: linear-gradient(135deg, #f1f5f9 0%, #e2e8f0 100%); padding: 25px; border-radius: 16px; text-align: center; border: 1px solid #cbd5e1;'>
                <div class='bounce' style='font-size: 24px; margin-bottom: 10px;'>✨</div>
                <h4 style='color: #1f2937; margin: 0 0 8px 0; font-size: 18px; font-weight: 700;'>Thank you for choosing SkyWings!</h4>
                <p style='color: #64748b; margin: 0; font-size: 15px;'>Have a pleasant journey! ✈️🌍</p>
            </div>
            
        </div>
    </div>
    
    <!-- Footer Note -->
    <div style='text-align: center; margin-top: 20px; color: rgba(255,255,255,0.8); font-size: 14px;'>
        Booking confirmation sent to {dto.Passengers.FirstOrDefault()?.Email} | SkyWings Flight System
    </div>
    
</body>
</html>";
                        
                        await _emailService.SendEmailAsync(emailAddress, "SkyWings - Booking Confirmation", htmlMessage);
                        Console.WriteLine($"NEW TEMPLATE: Email sent successfully to {emailAddress}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Email error: {ex.Message}");
                }
                
                return CreatedAtAction(nameof(GetById), new { id = booking.Id }, new { BookingId = booking.Id, BookingReference = booking.BookingReference });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Booking creation error: {ex.Message}");
                // If foreign key error, try with user ID 1
                if (ex.Message.Contains("FK_Bookings_Users_UserId"))
                {
                    booking.UserId = 1;
                    await _bookingRepository.AddAsync(booking);
                    return CreatedAtAction(nameof(GetById), new { id = booking.Id }, new { BookingId = booking.Id, BookingReference = booking.BookingReference });
                }
                return BadRequest($"Failed to create booking: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateBookingDto dto)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return NotFound();

            booking.TravelClass = dto.TravelClass;
            booking.SpecialRequests = dto.SpecialRequests;

            await _bookingRepository.UpdateAsync(booking);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _bookingRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                userId = 1; // Fallback for testing
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            var bookingDtos = bookings.Select(MappingHelper.ToBookingHistoryDto);
            return Ok(bookingDtos);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            var bookingDtos = bookings.Select(MappingHelper.ToBookingHistoryDto);
            return Ok(bookingDtos);
        }

        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAllForAdmin()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            var bookingDtos = bookings.Select(MappingHelper.ToBookingHistoryDto);
            return Ok(bookingDtos);
        }

        [HttpGet("seats/{flightId}")]
        public async Task<IActionResult> GetFlightSeats(int flightId)
        {
            // Ensure seats exist for this flight
            await SeatSeeder.SeedSeatsForFlightAsync(_context, flightId);
            
            var seats = await _context.Seats.Where(s => s.FlightId == flightId).ToListAsync();
            return Ok(seats);
        }

        [HttpGet("seat-availability/{flightId}")]
        public async Task<IActionResult> GetSeatAvailability(int flightId)
        {
            var flight = await _context.Flights.FindAsync(flightId);
            if (flight == null)
                return NotFound("Flight not found");

            await SeatSeeder.SeedSeatsForFlightAsync(_context, flightId);

            var seats = await _context.Seats
                .Where(s => s.FlightId == flightId)
                .Select(s => new {
                    FlightId = s.FlightId,
                    SeatNumber = s.SeatNumber,
                    SeatClass = s.SeatClass,
                    IsOccupied = s.IsOccupied,
                    IsAvailable = !s.IsOccupied
                })
                .OrderBy(s => s.SeatNumber)
                .ToListAsync();

            return Ok(new {
                FlightId = flightId,
                FlightNumber = flight.FlightNumber,
                TotalSeats = seats.Count,
                AvailableSeats = seats.Count(s => s.IsAvailable),
                OccupiedSeats = seats.Count(s => s.IsOccupied),
                Seats = seats
            });
        }

        [HttpPost("checkin/{id}")]
        public async Task<IActionResult> CheckIn(int id)
        {
            Console.WriteLine($"\n=== CHECK-IN REQUEST RECEIVED ===\nBooking ID: {id}\n");
            
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Flight)
                    .FirstOrDefaultAsync(b => b.Id == id);
                    
                if (booking == null) 
                {
                    Console.WriteLine($"Booking {id} not found");
                    return NotFound("Booking not found");
                }
                
                // Allow check-in for any user (remove ownership restriction)
                Console.WriteLine($"Check-in allowed for booking {id} by any user");

                Console.WriteLine($"Found booking: ID={booking.Id}, Status={booking.Status}");
                Console.WriteLine($"Flight departure: {booking.Flight.DepartureTime}");

                // Check if flight is today (temporarily disabled for testing)
                var today = DateTime.Today;
                var flightDate = booking.Flight.DepartureTime.Date;
                
                Console.WriteLine($"Today: {today}, Flight Date: {flightDate}");
                
                // Temporarily allow check-in regardless of date for testing
                // if (flightDate != today)
                // {
                //     Console.WriteLine("Flight is not today - check-in denied");
                //     return BadRequest("Check-in is only available on departure date");
                // }

                if (booking.Status == "Checked In")
                {
                    Console.WriteLine("Already checked in");
                    return BadRequest("Already checked in");
                }

                Console.WriteLine($"Updating booking status using Entity Framework");
                
                // Update using Entity Framework with explicit transaction
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    booking.Status = "Checked In";
                    _context.Bookings.Update(booking);
                    var rowsAffected = await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    
                    Console.WriteLine($"Transaction committed. Rows affected: {rowsAffected}");
                    
                    // Verify the update by querying fresh from database
                    var verifyBooking = await _context.Bookings.FindAsync(id);
                    Console.WriteLine($"Database verification - Status: {verifyBooking?.Status}");
                }
                catch (Exception saveEx)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Transaction rollback due to error: {saveEx.Message}");
                    throw;
                }

                return Ok(new { Message = "You have successfully checked in.", Status = "Checked In" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during check-in: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, "Internal server error during check-in");
            }
        }


    }
}