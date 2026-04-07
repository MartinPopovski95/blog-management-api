using BlogManagementApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BlogManagementApi.DTOs.Posts
{
    public class UpdatePostDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? PostImage { get; set; }

        public string? AuthorImage { get; set; }

        public CategoryEnum? Category { get; set; }

        public PublishStatusEnum PublishStatus { get; set; }
    }
}
