using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PassengerController : ControllerBase
    {
        private readonly IPassengerRepository _passengerRepository;
        public PassengerController(IPassengerRepository passengerRepository)
        {
            _passengerRepository = passengerRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PassengerDto>>> GetAll()
        {
            var passengers = await _passengerRepository.GetAllAsync();
            var dtos = passengers.Select(p => new PassengerDto
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age,
                Gender = p.Gender,
                SeatNumber = p.SeatNumber,
                PassportNumber = p.PassportNumber,
                PhoneNumber = p.PhoneNumber
            });
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PassengerDto>> GetById(int id)
        {
            var p = await _passengerRepository.GetByIdAsync(id);
            if (p == null) return NotFound();
            var dto = new PassengerDto
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age,
                Gender = p.Gender,
                SeatNumber = p.SeatNumber,
                PassportNumber = p.PassportNumber,
                PhoneNumber = p.PhoneNumber
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] PassengerDto dto)
        {
            var passenger = new Backend.Models.Passenger
            {
                Name = dto.Name,
                Age = dto.Age,
                Gender = dto.Gender,
                SeatNumber = dto.SeatNumber,
                PassportNumber = dto.PassportNumber,
                PhoneNumber = dto.PhoneNumber
            };
            await _passengerRepository.AddAsync(passenger);
            dto.Id = passenger.Id;
            return CreatedAtAction(nameof(GetById), new { id = passenger.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] PassengerDto dto)
        {
            if (id != dto.Id) return BadRequest();
            var passenger = await _passengerRepository.GetByIdAsync(id);
            if (passenger == null) return NotFound();
            passenger.Name = dto.Name;
            passenger.Age = dto.Age;
            passenger.Gender = dto.Gender;
            passenger.SeatNumber = dto.SeatNumber;
            passenger.PassportNumber = dto.PassportNumber;
            passenger.PhoneNumber = dto.PhoneNumber;
            await _passengerRepository.UpdateAsync(passenger);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _passengerRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
