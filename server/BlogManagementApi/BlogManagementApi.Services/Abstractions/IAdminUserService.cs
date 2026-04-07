using BlogManagementApi.DTOs.AdminUsers;

namespace BlogManagementApi.Services.Abstractions
{
    public interface IAdminUserService
    {
        Task<(List<AdminUserResponseDto> Users, int TotalCount)> GetAllAsync(int page, int pageSize);
        Task<AdminUserResponseDto> GetByIdAsync(string id);
        Task DeleteAsync(string id, string requestingUserId);
        Task UpdateRolesAsync(string id, List<string> roles);
    }
}
