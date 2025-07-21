using AuthApi.Models;

namespace AuthApi.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<User> GetUserByEmailAsync(string email);
        bool VerifyPassword(string password, string hash);
        string GenerateJwtToken(User user);
    }
}