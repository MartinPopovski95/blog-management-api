using BlogManagementApi.Domain.Models;

namespace BlogManagementApi.Services.Abstractions
{
    public interface ITokenService
    {
        Task<(string Token, DateTime ExpiresAt)> GenerateTokenAsync(User user);
        string GenerateRefreshToken();
    }
}
