using BlogManagementApi.DTOs.Media;
using BlogManagementApi.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BlogManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Editor")]
    [EnableRateLimiting("general")]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;

        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<UploadMediaResponseDto>> Upload(IFormFile file)
        {
            var url = await _mediaService.UploadAsync(file.OpenReadStream(), file.FileName);
            return Ok(new UploadMediaResponseDto { Url = url });
        }
    }
}
