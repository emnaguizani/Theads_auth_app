using System.ComponentModel.DataAnnotations;

namespace AuthApi.DTOs
{
    public class RegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string Password { get; set; }
    }

    public class AuthResponseDto
    {
        public required string Token { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}