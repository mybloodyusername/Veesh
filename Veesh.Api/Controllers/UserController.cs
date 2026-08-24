using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Veesh.Core.Common;
using Veesh.Core.DTOs.User;
using Veesh.Core.Extensions;
using Veesh.Core.Services;
using Veesh.Domain.Enums;

namespace Veesh.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserService userService) : ControllerBase
    {
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpGet("Me")]
        public async Task<ActionResult<UserResponse>> Me()
        {
            var userId = User.GetUserId();
            var result = await userService.GetByIdAsync(userId);
            throw new NotImplementedException();
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpGet("Pageable")]
        public async Task<ActionResult<Pageable<UserResponse>>> GetAllPageable([FromQuery] UserQuery query)
        {
            return await userService.GetAllAsync(query);
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetById([FromRoute] Guid id)
        {
            return await userService.GetByIdAsync(id);
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost]
        public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
        {
            return await userService.CreateAsync(request);
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpPut]
        public async Task<ActionResult<UserResponse>> Update([FromBody] UpdateUserRequest request)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            return await userService.UpdateAsync(request, userId, roles);
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpDelete("{userId}")]
        public async Task<ActionResult> Delete([FromRoute] Guid userId)
        {
            var result = await userService.DeleteAsync(userId);
            return Ok(new { result });
        }
    }
}