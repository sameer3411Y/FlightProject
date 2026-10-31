using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingPaymentController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly PaymentService _paymentService;

        public BookingPaymentController(IBookingRepository bookingRepository, PaymentService paymentService)
        {
            _bookingRepository = bookingRepository;
            _paymentService = paymentService;
        }

        [HttpPost("create-booking-with-payment")]
        public async Task<IActionResult> CreateBookingWithPayment([FromBody] CreateBookingWithPaymentDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            // Create payment order first
            var orderId = _paymentService.CreateOrder(dto.TotalAmount);

            // Create booking with pending status
            var booking = new Booking
            {
                UserId = userId,
                FlightId = dto.FlightId,
                BookingReference = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                TotalAmount = dto.TotalAmount,
                TravelClass = dto.TravelClass,
                PassengerCount = dto.Passengers.Count,
                Status = "Pending",
                SpecialRequests = dto.SpecialRequests,
                Passengers = dto.Passengers.Select(p => new Passenger
                {
                    Name = p.Name,
                    Age = p.Age,
                    Gender = p.Gender,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber,
                    PassportNumber = p.PassportNumber,
                    SeatNumber = p.SeatNumber,
                    TravelClass = dto.TravelClass
                }).ToList()
            };

            await _bookingRepository.AddAsync(booking);

            return Ok(new
            {
                BookingId = booking.Id,
                BookingReference = booking.BookingReference,
                OrderId = orderId,
                Amount = dto.TotalAmount,
                Status = "Pending"
            });
        }

        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentDto dto)
        {
            var isValid = _paymentService.VerifyPayment(dto.OrderId, dto.PaymentId, dto.Signature);

            if (!isValid)
                return BadRequest("Payment verification failed");

            var booking = await _bookingRepository.GetByIdAsync(dto.BookingId);
            if (booking == null)
                return NotFound("Booking not found");

            booking.Status = "Confirmed";
            await _bookingRepository.UpdateAsync(booking);

            return Ok(new
            {
                BookingId = booking.Id,
                BookingReference = booking.BookingReference,
                Status = "Confirmed",
                Message = "Payment confirmed and booking completed"
            });
        }
    }

    public class CreateBookingWithPaymentDto
    {
        [JsonRequired]
        public int FlightId { get; set; }
        [JsonRequired]
        public decimal TotalAmount { get; set; }
        public TravelClass TravelClass { get; set; } = TravelClass.Economy;
        public string? SpecialRequests { get; set; }
        public List<CreatePassengerDto> Passengers { get; set; } = new();
    }

    public class ConfirmPaymentDto
    {
        [JsonRequired]
        public int BookingId { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
    }
}