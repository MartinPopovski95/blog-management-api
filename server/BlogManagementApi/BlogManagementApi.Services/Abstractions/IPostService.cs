using BlogManagementApi.Domain.Enums;
using BlogManagementApi.DTOs.Posts;

namespace BlogManagementApi.Services.Abstractions
{
    public interface IPostService
    {
        Task<(List<PostResponseDto> Posts, int TotalCount)> GetAllAsync(int page, int pageSize, CategoryEnum? category = null, string? search = null);
        Task<PostResponseDto> GetBySlugAsync(string slug);
        Task<PostResponseDto> CreateAsync(CreatePostDto request, string userId, string userEmail);
        Task<PostResponseDto> UpdateAsync(Guid id, UpdatePostDto request, string userId, string userEmail, bool isAdmin);
        Task DeleteAsync(Guid id, string userId, string userEmail, bool isAdmin);
        Task<(List<PostResponseDto> Posts, int TotalCount)> GetDeletedAsync(int page, int pageSize);
        Task RestoreAsync(Guid id);
        Task<List<PostResponseDto>> GetRelatedAsync(string slug);
    }
}
