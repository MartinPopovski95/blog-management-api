using BlogManagementApi.Domain.Enums;
using BlogManagementApi.Domain.Models;

namespace BlogManagementApi.DataAccess.Repositories.Abstractions
{
    public interface IPostRepository
    {
        Task<(List<Post> Posts, int TotalCount)> GetAllAsync(int page, int pageSize, bool publishedOnly, CategoryEnum? category, string? search);
        Task<Post?> GetBySlugAsync(string slug);
        Task<Post?> GetByIdAsync(Guid id);
        Task<Post?> GetByIdIncludingDeletedAsync(Guid id);
        Task<bool> SlugExistsAsync(string slug);
        Task<Post> CreateAsync(Post post);
        Task<Post> UpdateAsync(Post post);
        Task DeleteAsync(Post post);
        Task<(List<Post> Posts, int TotalCount)> GetDeletedAsync(int page, int pageSize);
        Task RestoreAsync(Post post);
        Task<List<Post>> GetRelatedAsync(string slug, int count = 5);
    }
}
