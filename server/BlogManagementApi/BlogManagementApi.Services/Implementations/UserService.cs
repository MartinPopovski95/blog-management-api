using AutoMapper;
using BlogManagementApi.Domain.Models;
using BlogManagementApi.DTOs.Users;
using BlogManagementApi.Shared.Exceptions;
using BlogManagementApi.Services.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace BlogManagementApi.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserProfileDto> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException("User not found.");

            return await MapToDtoAsync(user);
        }

        public async Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateUserDto request)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException("User not found.");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            await _userManager.UpdateAsync(user);

            return await MapToDtoAsync(user);
        }

        private async Task<UserProfileDto> MapToDtoAsync(User user)
        {
            var dto = _mapper.Map<UserProfileDto>(user);
            dto.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            return dto;
        }
    }
}
