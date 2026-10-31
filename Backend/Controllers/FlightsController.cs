using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using Backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "Admin")]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightRepository _flightRepository;

        public FlightsController(IFlightRepository flightRepository)
        {
            _flightRepository = flightRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var flights = await _flightRepository.GetAllAsync();
            var flightDtos = flights.Select(MappingHelper.ToFlightDto);
            return Ok(flightDtos);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string origin, [FromQuery] string destination, [FromQuery] string date)
        {
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                return BadRequest("Invalid date format. Use YYYY-MM-DD");

            var flights = await _flightRepository.SearchAsync(origin, destination, parsedDate);
            var flightDtos = flights.Select(MappingHelper.ToFlightDto);
            return Ok(flightDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null) return NotFound();

            var flightDto = MappingHelper.ToFlightDto(flight);
            return Ok(flightDto);
        }
        
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFlightDto dto)
        {
            var flight = new Flight
            {
                FlightNumber = dto.FlightNumber,
                Airline = dto.Airline,
                Origin = dto.Origin,
                Destination = dto.Destination,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime,
                EconomyPrice = dto.EconomyPrice,
                PremiumEconomyPrice = dto.PremiumEconomyPrice,
                BusinessPrice = dto.BusinessPrice,
                FirstClassPrice = dto.FirstClassPrice,
                TotalSeats = dto.TotalSeats,
                AvailableSeats = dto.TotalSeats,
                Duration = dto.Duration,
                Stops = dto.Stops,
                IsRefundable = dto.IsRefundable
            };

            await _flightRepository.AddAsync(flight);
            return CreatedAtAction(nameof(GetById), new { id = flight.Id }, flight.Id);
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateFlightDto dto)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null) return NotFound();

            MappingHelper.UpdateFlightFromDto(flight, dto);

            await _flightRepository.UpdateAsync(flight);
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _flightRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}