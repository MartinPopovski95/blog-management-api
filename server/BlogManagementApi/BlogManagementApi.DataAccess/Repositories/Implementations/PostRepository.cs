using BlogManagementApi.DataAccess.DbContext;
using BlogManagementApi.DataAccess.Repositories.Abstractions;
using BlogManagementApi.Domain.Enums;
using BlogManagementApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogManagementApi.DataAccess.Repositories.Implementations
{
    public class PostRepository : IPostRepository
    {
        private readonly BlogDbContext _context;

        public PostRepository(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Post> Posts, int TotalCount)> GetAllAsync(int page, int pageSize, bool publishedOnly, CategoryEnum? category, string? search)
        {
            var query = _context.Posts
                .Include(p => p.User)
                .Where(p => p.DeletedAt == null)
                .AsQueryable();

            if (publishedOnly)
                query = query.Where(p => p.PublishStatus == PublishStatusEnum.Published);

            if (category.HasValue)
                query = query.Where(p => p.Category == category);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Title.Contains(search));

            var totalCount = await query.CountAsync();
            var posts = await query
                .OrderByDescending(p => p.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (posts, totalCount);
        }

        public async Task<Post?> GetBySlugAsync(string slug)
        {
            return await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.DeletedAt == null);
        }

        public async Task<Post?> GetByIdAsync(Guid id)
        {
            return await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);
        }

        public async Task<Post?> GetByIdIncludingDeletedAsync(Guid id)
        {
            return await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> SlugExistsAsync(string slug)
        {
            return await _context.Posts.AnyAsync(p => p.Slug == slug && p.DeletedAt == null);
        }

        public async Task<Post> CreateAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post> UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task DeleteAsync(Post post)
        {
            post.DeletedAt = DateTime.UtcNow;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task<(List<Post> Posts, int TotalCount)> GetDeletedAsync(int page, int pageSize)
        {
            var query = _context.Posts
                .Include(p => p.User)
                .Where(p => p.DeletedAt != null)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var posts = await query
                .OrderByDescending(p => p.DeletedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (posts, totalCount);
        }

        public async Task RestoreAsync(Post post)
        {
            post.DeletedAt = null;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Post>> GetRelatedAsync(string slug, int count = 5)
        {
            var sourcePost = await _context.Posts
                .FirstOrDefaultAsync(p => p.Slug == slug && p.DeletedAt == null);

            if (sourcePost is null)
                return new List<Post>();

            var related = await _context.Posts
                .Include(p => p.User)
                .Where(p => p.DeletedAt == null
                         && p.PublishStatus == PublishStatusEnum.Published
                         && p.Id != sourcePost.Id
                         && p.Category == sourcePost.Category
                         && p.Category != null)
                .OrderByDescending(p => p.PublishedAt)
                .Take(count)
                .ToListAsync();

            if (related.Count < count)
            {
                var existingIds = related.Select(p => p.Id).ToList();
                existingIds.Add(sourcePost.Id);

                var padCount = count - related.Count;
                var padding = await _context.Posts
                    .Include(p => p.User)
                    .Where(p => p.DeletedAt == null
                             && p.PublishStatus == PublishStatusEnum.Published
                             && !existingIds.Contains(p.Id))
                    .OrderByDescending(p => p.PublishedAt)
                    .Take(padCount)
                    .ToListAsync();

                related.AddRange(padding);
            }

            return related;
        }
    }
}
