using Site.Domain.Entities;
using YjSite.DTOs;

namespace YjSite.Services.Auth
{
    public interface IAuthService
    {
        Task<(bool Success, string Token, User User)> LoginAsync(string username, string password);
        Task<(bool Success, string Message)> RegisterAsync(User user, string password);
        Task<bool> ValidateUserAsync(string username, string password);
        string GenerateJwtToken(User user);
    }
} 