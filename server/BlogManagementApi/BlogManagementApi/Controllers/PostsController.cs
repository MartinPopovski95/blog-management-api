using BlogManagementApi.Domain.Enums;
using BlogManagementApi.DTOs.Posts;
using BlogManagementApi.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace BlogManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("general")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] CategoryEnum? category = null,
            [FromQuery] string? search = null)
        {
            var (posts, totalCount) = await _postService.GetAllAsync(page, pageSize, category, search);
            return Ok(new { Data = posts, TotalCount = totalCount, Page = page, PageSize = pageSize });
        }

        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeleted(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (posts, totalCount) = await _postService.GetDeletedAsync(page, pageSize);
            return Ok(new { Data = posts, TotalCount = totalCount, Page = page, PageSize = pageSize });
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<PostResponseDto>> GetBySlug(string slug)
        {
            var post = await _postService.GetBySlugAsync(slug);
            return Ok(post);
        }

        [HttpGet("{slug}/related")]
        public async Task<ActionResult<List<PostResponseDto>>> GetRelated(string slug)
        {
            var related = await _postService.GetRelatedAsync(slug);
            return Ok(related);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<ActionResult<PostResponseDto>> Create([FromBody] CreatePostDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var post = await _postService.CreateAsync(request, userId, userEmail);
            return CreatedAtAction(nameof(GetBySlug), new { slug = post.Slug }, post);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<ActionResult<PostResponseDto>> Update(Guid id, [FromBody] UpdatePostDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var isAdmin = User.IsInRole("Admin");
            var post = await _postService.UpdateAsync(id, request, userId, userEmail, isAdmin);
            return Ok(post);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var isAdmin = User.IsInRole("Admin");
            await _postService.DeleteAsync(id, userId, userEmail, isAdmin);
            return NoContent();
        }

        [HttpPost("{id:guid}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Restore(Guid id)
        {
            await _postService.RestoreAsync(id);
            return NoContent();
        }
    }
}
