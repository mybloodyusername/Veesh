using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veesh.Core.Common;
using Veesh.Core.DTOs.Wish;
using Veesh.Core.DTOs.WishList;
using Veesh.Core.Extensions;
using Veesh.Core.Services;
using Veesh.Domain.Enums;

namespace Veesh.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishListController(WishListService wishListService) : ControllerBase
    {
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpGet]
        public async Task<ActionResult<Pageable<WishListResponse>>> GetAll([FromQuery] WishListQuery query)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            return await wishListService.GetAllAsync(query, userId, roles);
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpGet("{id}")]
        public async Task<ActionResult<WishListResponse>> GetById([FromRoute] Guid id)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            return await wishListService.GetByIdAsync(id, userId, roles);
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpPost]
        public async Task<ActionResult<WishListResponse>> Create([FromBody] CreateWishListRequest request)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            return await wishListService.CreateAsync(request, userId, roles);
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpPut]
        public async Task<ActionResult<WishListResponse>> Update([FromBody] UpdateWishListRequest request)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            return await wishListService.UpdateAsync(request, userId, roles);
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            var result = await wishListService.DeleteAsync(id, userId, roles);
            return Ok(new { result });
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpPut("{id}/WishesPriorities")]
        public async Task<ActionResult<WishListResponse>> UpdateWishesPriorities(
            [FromBody] UpdateWishPriorityRequest request, [FromRoute] Guid id)
        {
            var userId = User.GetUserId();
            var roles = User.GetRoles();
            return await wishListService.UpdateWishesPrioritiesAsync(request, id, userId, roles);
        }
    }
}
