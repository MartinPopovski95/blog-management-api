using AutoMapper;
using BlogManagementApi.DataAccess.Repositories.Abstractions;
using BlogManagementApi.Domain.Enums;
using BlogManagementApi.Domain.Models;
using BlogManagementApi.DTOs.Posts;
using BlogManagementApi.Shared.Exceptions;
using BlogManagementApi.Services.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace BlogManagementApi.Services.Implementations
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IAuditLogService _auditLogService;
        private readonly IMapper _mapper;
        private readonly ILogger<PostService> _logger;

        public PostService(IPostRepository postRepository, IAuditLogService auditLogService, IMapper mapper, ILogger<PostService> logger)
        {
            _postRepository = postRepository;
            _auditLogService = auditLogService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(List<PostResponseDto> Posts, int TotalCount)> GetAllAsync(int page, int pageSize, CategoryEnum? category = null, string? search = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            var (posts, totalCount) = await _postRepository.GetAllAsync(page, pageSize, publishedOnly: true, category, search);
            return (_mapper.Map<List<PostResponseDto>>(posts), totalCount);
        }

        public async Task<PostResponseDto> GetBySlugAsync(string slug)
        {
            var post = await _postRepository.GetBySlugAsync(slug)
                ?? throw new NotFoundException($"Post with slug '{slug}' not found.");
            return _mapper.Map<PostResponseDto>(post);
        }

        public async Task<PostResponseDto> CreateAsync(CreatePostDto request, string userId, string userEmail)
        {
            var slug = await GenerateUniqueSlugAsync(request.Title);
            var now = DateTime.UtcNow;

            var post = new Post
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Slug = slug,
                Content = request.Content,
                PostImage = request.PostImage ?? string.Empty,
                AuthorImage = request.AuthorImage ?? string.Empty,
                Author = string.Empty,
                PublishStatus = request.PublishStatus,
                Category = request.Category,
                PublishedAt = now,
                UpdatedAt = now,
                UserId = userId
            };

            var created = await _postRepository.CreateAsync(post);

            await _auditLogService.LogAsync("Post", created.Id.ToString(), AuditActionEnum.Created, userId, userEmail);

            _logger.LogInformation("Post created: {Slug} by user {UserId}", created.Slug, userId);

            return _mapper.Map<PostResponseDto>(created);
        }

        public async Task<PostResponseDto> UpdateAsync(Guid id, UpdatePostDto request, string userId, string userEmail, bool isAdmin)
        {
            var post = await _postRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Post with id '{id}' not found.");

            if (!isAdmin && post.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to edit this post.");

            if (post.Title != request.Title)
                post.Slug = await GenerateUniqueSlugAsync(request.Title);

            post.Title = request.Title;
            post.Content = request.Content;
            post.PostImage = request.PostImage ?? post.PostImage;
            post.AuthorImage = request.AuthorImage ?? post.AuthorImage;
            post.Category = request.Category;
            post.PublishStatus = request.PublishStatus;
            post.UpdatedAt = DateTime.UtcNow;

            var updated = await _postRepository.UpdateAsync(post);

            await _auditLogService.LogAsync("Post", updated.Id.ToString(), AuditActionEnum.Updated, userId, userEmail);

            return _mapper.Map<PostResponseDto>(updated);
        }

        public async Task DeleteAsync(Guid id, string userId, string userEmail, bool isAdmin)
        {
            var post = await _postRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Post with id '{id}' not found.");

            if (!isAdmin && post.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to delete this post.");

            await _postRepository.DeleteAsync(post);

            await _auditLogService.LogAsync("Post", id.ToString(), AuditActionEnum.Deleted, userId, userEmail);

            _logger.LogInformation("Post soft-deleted: {PostId} by user {UserId}", id, userId);
        }

        public async Task<(List<PostResponseDto> Posts, int TotalCount)> GetDeletedAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            var (posts, totalCount) = await _postRepository.GetDeletedAsync(page, pageSize);
            return (_mapper.Map<List<PostResponseDto>>(posts), totalCount);
        }

        public async Task RestoreAsync(Guid id)
        {
            var post = await _postRepository.GetByIdIncludingDeletedAsync(id)
                ?? throw new NotFoundException($"Post with id '{id}' not found.");

            await _postRepository.RestoreAsync(post);
        }

        public async Task<List<PostResponseDto>> GetRelatedAsync(string slug)
        {
            var _ = await _postRepository.GetBySlugAsync(slug)
                ?? throw new NotFoundException($"Post with slug '{slug}' not found.");

            var related = await _postRepository.GetRelatedAsync(slug);
            return _mapper.Map<List<PostResponseDto>>(related);
        }

        private async Task<string> GenerateUniqueSlugAsync(string title)
        {
            var baseSlug = Regex.Replace(title.ToLowerInvariant(), @"[^a-z0-9\s-]", "")
                                .Trim()
                                .Replace(" ", "-");
            baseSlug = Regex.Replace(baseSlug, @"-+", "-");

            var slug = baseSlug;
            var counter = 1;

            while (await _postRepository.SlugExistsAsync(slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }
    }
}
