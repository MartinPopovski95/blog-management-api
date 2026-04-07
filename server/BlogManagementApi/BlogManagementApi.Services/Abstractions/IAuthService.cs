using BlogManagementApi.DTOs.Auth;

namespace BlogManagementApi.Services.Abstractions
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterEditorAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string userId);
    }
}
