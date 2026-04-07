using BlogManagementApi.DTOs.Users;

namespace BlogManagementApi.Services.Abstractions
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(string userId);
        Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateUserDto request);
    }
}
