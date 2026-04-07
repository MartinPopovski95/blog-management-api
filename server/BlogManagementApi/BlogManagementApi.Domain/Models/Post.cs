using BlogManagementApi.Domain.Enums;

namespace BlogManagementApi.Domain.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string PostImage { get; set; }
        public string Author { get; set; }
        public string AuthorImage { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PublishStatusEnum PublishStatus { get; set; }
        public CategoryEnum? Category { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
