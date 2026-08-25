using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veesh.Core.Common;
using Veesh.Core.DTOs.WishStoreUrl;
using Veesh.Core.Extensions;
using Veesh.Core.Services;
using Veesh.Domain.Enums;

namespace Veesh.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishStoreUrlController(WishStoreUrlService wishStoreUrlService) : ControllerBase
    {
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpGet]
        public async Task<ActionResult<Pageable<WishStoreUrlResponse>>> GetAll([FromQuery] WishStoreUrlQuery query)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            return await wishStoreUrlService.GetAllAsync(query, userId, roles);
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpGet("{id}")]
        public async Task<ActionResult<WishStoreUrlResponse>> GetById([FromRoute] Guid id)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            return await wishStoreUrlService.GetByIdAsync(id, userId, roles);
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpPost]
        public async Task<ActionResult<WishStoreUrlResponse>> Create([FromBody] CreateWishStoreUrlRequest request)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            return await wishStoreUrlService.CreateAsync(request, userId, roles);
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpPut]
        public async Task<ActionResult<WishStoreUrlResponse>> Update([FromBody] UpdateWishStoreUrlRequest request)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            return await wishStoreUrlService.UpdateAsync(request, userId, roles);
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            var result = await wishStoreUrlService.DeleteAsync(id, userId, roles);
            return Ok(new { result });
        }
    }
}
