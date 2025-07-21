
using Microsoft.AspNetCore.Mvc;
using AuthApi.Models;
using AuthApi.Services;

namespace AuthApi.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IAuthService authService, ILogger<LoginController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.LoginAsync(request);
                
                _logger.LogInformation("User {Username} logged in successfully", response.Username);
                
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Login failed for {Email}: {Message}", request.Email, ex.Message);
                return Unauthorized(new { message = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }
    }
}