using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Gender = u.Gender,
                MobileNumber = u.MobileNumber,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            });
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                MobileNumber = user.MobileNumber,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Gender = dto.Gender,
                MobileNumber = dto.MobileNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            await _userRepository.AddAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user.Id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.FirstName)) user.FirstName = dto.FirstName;
            if (!string.IsNullOrEmpty(dto.LastName)) user.LastName = dto.LastName;
            if (!string.IsNullOrEmpty(dto.Gender)) user.Gender = dto.Gender;
            if (!string.IsNullOrEmpty(dto.MobileNumber)) user.MobileNumber = dto.MobileNumber;
            if (!string.IsNullOrEmpty(dto.Role)) user.Role = dto.Role;
            if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;

            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            user.IsActive = false;
            await _userRepository.UpdateAsync(user);
            return NoContent();
        }
    }
}