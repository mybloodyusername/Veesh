using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veesh.Core.Common;
using Veesh.Core.DTOs.Wish;
using Veesh.Core.Extensions;
using Veesh.Core.Services;
using Veesh.Domain.Enums;

namespace Veesh.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishController(WishService wishService) : ControllerBase
    {
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpGet]
        public async Task<ActionResult<Pageable<WishResponse>>> GetAll([FromQuery] WishQuery query)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            return await wishService.GetAllAsync(query, userId, roles);
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpGet("{id}")]
        public async Task<ActionResult<WishResponse>> GetById([FromRoute] Guid id)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            return await wishService.GetByIdAsync(id, userId, roles);
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpPost]
        public async Task<ActionResult<WishResponse>> Create([FromBody] CreateWishRequest request)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            return await wishService.CreateAsync(request, userId, roles);
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpPut]
        public async Task<ActionResult<WishResponse>> Update([FromBody] UpdateWishRequest request)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            return await wishService.UpdateAsync(request, userId, roles);
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            var result = await wishService.DeleteAsync(id, userId, roles);
            return Ok(new { result });
        }
    }
}
