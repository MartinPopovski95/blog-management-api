using AutoMapper;
using BlogManagementApi.Domain.Enums;
using BlogManagementApi.Domain.Models;
using BlogManagementApi.DTOs.AdminUsers;
using BlogManagementApi.Services.Abstractions;
using BlogManagementApi.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BlogManagementApi.Services.Implementations
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuditLogService _auditLogService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminUserService> _logger;

        public AdminUserService(UserManager<User> userManager, IAuditLogService auditLogService, IMapper mapper, ILogger<AdminUserService> logger)
        {
            _userManager = userManager;
            _auditLogService = auditLogService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(List<AdminUserResponseDto> Users, int TotalCount)> GetAllAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var totalCount = _userManager.Users.Count();

            var pagedUsers = _userManager.Users
                .OrderBy(u => u.Email)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtos = new List<AdminUserResponseDto>();
            foreach (var user in pagedUsers)
                dtos.Add(await MapToDtoAsync(user));

            return (dtos, totalCount);
        }

        public async Task<AdminUserResponseDto> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id)
                ?? throw new NotFoundException($"User with id '{id}' not found.");

            return await MapToDtoAsync(user);
        }

        public async Task DeleteAsync(string id, string requestingUserId)
        {
            if (id == requestingUserId)
                throw new InvalidOperationException("You cannot delete your own account.");

            var user = await _userManager.FindByIdAsync(id)
                ?? throw new NotFoundException($"User with id '{id}' not found.");

            var requestingUser = await _userManager.FindByIdAsync(requestingUserId)
                ?? throw new NotFoundException("Requesting user not found.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to delete user: {errors}");
            }

            await _auditLogService.LogAsync(
                entityType: "User",
                entityId: id,
                action: AuditActionEnum.Deleted,
                userId: requestingUserId,
                userEmail: requestingUser.Email ?? string.Empty);

            _logger.LogWarning("User deleted: {UserId} by admin {RequestingUserId}", id, requestingUserId);
        }

        public async Task UpdateRolesAsync(string id, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(id)
                ?? throw new NotFoundException($"User with id '{id}' not found.");

            var currentRoles = (await _userManager.GetRolesAsync(user)).ToList();

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to remove roles: {errors}");
            }

            var addResult = await _userManager.AddToRolesAsync(user, roles);
            if (!addResult.Succeeded)
            {
                var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to assign roles: {errors}");
            }

            await _auditLogService.LogAsync(
                entityType: "User",
                entityId: id,
                action: AuditActionEnum.Updated,
                userId: id,
                userEmail: user.Email ?? string.Empty,
                changes: $"Roles changed to: {string.Join(", ", roles)}");
        }

        private async Task<AdminUserResponseDto> MapToDtoAsync(User user)
        {
            var dto = _mapper.Map<AdminUserResponseDto>(user);
            dto.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            return dto;
        }
    }
}
