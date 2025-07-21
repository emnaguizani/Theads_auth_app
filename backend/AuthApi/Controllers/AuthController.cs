using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthApi.Data;
using AuthApi.Models;
using AuthApi.DTOs;
using AuthApi.Services;
using BCrypt.Net;

namespace AuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                // Validate model
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if username already exists
                var existingUserByUsername = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username.ToLower() == registerDto.Username.ToLower());
                
                if (existingUserByUsername != null)
                {
                    return BadRequest(new { message = "Username already exists" });
                }

                // Check if email already exists
                var existingUserByEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == registerDto.Email.ToLower());
                
                if (existingUserByEmail != null)
                {
                    return BadRequest(new { message = "Email already exists" });
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                // Create new user
                var newUser = new User
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    PasswordHash = passwordHash
                };

                // Save to database
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = _jwtService.GenerateToken(newUser);

                // Return success response
                var response = new AuthResponseDto
                {
                    Token = token,
                    Username = newUser.Username,
                    Email = newUser.Email,
                    ExpiresAt = _jwtService.GetTokenExpiration()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
            }
        }
    }
}