using BlogManagementApi.DTOs.AdminUsers;
using BlogManagementApi.DTOs.Common;
using BlogManagementApi.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace BlogManagementApi.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("general")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;

        public AdminUsersController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponseDto<AdminUserResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (users, totalCount) = await _adminUserService.GetAllAsync(page, pageSize);
            return Ok(new PagedResponseDto<AdminUserResponseDto>
            {
                Data = users,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdminUserResponseDto>> GetById(string id)
        {
            var user = await _adminUserService.GetByIdAsync(id);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var requestingUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _adminUserService.DeleteAsync(id, requestingUserId);
            return NoContent();
        }

        [HttpPut("{id}/roles")]
        public async Task<IActionResult> UpdateRoles(string id, [FromBody] UpdateUserRolesDto request)
        {
            await _adminUserService.UpdateRolesAsync(id, request.Roles);
            return NoContent();
        }
    }
}
