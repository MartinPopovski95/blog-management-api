using BlogManagementApi.Domain.Enums;

namespace BlogManagementApi.DTOs.Posts
{
    public class PostResponseDto
    {
        public Guid Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? PostImage { get; set; }
        public string Author { get; set; } = string.Empty;
        public string? AuthorImage { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PublishStatusEnum PublishStatus { get; set; }
        public string UserId { get; set; } = string.Empty;
        public CategoryEnum? Category { get; set; }
    }
}
