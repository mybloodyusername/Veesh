using System.Data.Common;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Veesh.Core.Common;
using Veesh.Core.DTOs.Wish;
using Veesh.Core.Exceptions;
using Veesh.Core.Interfaces;
using Veesh.Domain.Entities;
using Veesh.Domain.Enums;

namespace Veesh.Core.Services;

public class WishService(
    IWishRepository wishRepository,
    IWishListRepository wishListRepository,
    ILogger<WishService> logger)
{
    public async Task<Pageable<WishResponse>> GetAllAsync(WishQuery query, Guid userId, List<UserRole> roles)
    {
        if (!roles.Contains(UserRole.Admin))
        {
            var userQuery = new WishListQuery { OwnerId = userId, Size = int.MaxValue };
            var userWishLists = await wishListRepository.GetAllAsync(userQuery);
            var userWishListIds = userWishLists.Items.Select(wl => wl.Id).ToList();

            if (userWishListIds.Count == 0)
                return new Pageable<WishResponse>(new List<WishResponse>(), 0, query.Size, 0, 0);

            var allUserWishes = new List<Wish>();
            foreach (var wishListId in userWishListIds)
            {
                var wishQuery = new WishQuery { WishListId = wishListId, Size = int.MaxValue };
                var page = await wishRepository.GetAllAsync(wishQuery);
                allUserWishes.AddRange(page.Items);
            }

            var total = allUserWishes.Count;
            var lastPage = total / query.Size;
            var items = allUserWishes
                .Skip(query.Page * query.Size)
                .Take(query.Size)
                .ToList();

            return new Pageable<WishResponse>(items.Adapt<List<WishResponse>>(), query.Page, query.Size, total, lastPage);
        }

        var result = await wishRepository.GetAllAsync(query);
        return result.Adapt<Pageable<WishResponse>>();
    }

    public async Task<WishResponse> GetByIdAsync(Guid id, Guid userId, List<UserRole> roles)
    {
        var wish = await wishRepository.GetByIdAsync(id);
        if (wish == null)
            throw new NotFoundException("Wish not found.");

        if (!roles.Contains(UserRole.Admin))
        {
            if (wish.WishList == null || wish.WishList.OwnerId != userId)
                throw new UnauthorizedAccessException("You are not authorized to view this wish.");
        }

        return wish.Adapt<WishResponse>();
    }

    public async Task<WishResponse> CreateAsync(CreateWishRequest request, Guid userId, List<UserRole> roles)
    {
        try
        {
            var wishList = await wishListRepository.GetByIdAsync(request.WishListId);
            if (wishList == null)
                throw new NotFoundException("WishList not found.");

            if (!roles.Contains(UserRole.Admin) && wishList.OwnerId != userId)
                throw new UnauthorizedAccessException("You are not authorized to add wishes to this wishlist.");

            var wish = new Wish
            {
                Title = request.Title,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                Priority = request.Priority,
                WishListId = request.WishListId
            };

            var created = await wishRepository.CreateAsync(wish);
            return created.Adapt<WishResponse>();
        }
        catch (DbUpdateException e)
        {
            logger.LogError(e, "Failed to create wish for wishlist {WishListId}", request.WishListId);
            throw new ConflictException("Failed to create Wish");
        }
    }

    public async Task<WishResponse> UpdateAsync(UpdateWishRequest request, Guid userId, List<UserRole> roles)
    {
        try
        {
            var existing = await wishRepository.GetByIdAsync(request.Id);
            if (existing == null)
                throw new NotFoundException("Wish not found.");

            if (!roles.Contains(UserRole.Admin))
            {
                if (existing.WishList == null || existing.WishList.OwnerId != userId)
                    throw new UnauthorizedAccessException("You are not authorized to update this wish.");
            }

            var targetWishList = await wishListRepository.GetByIdAsync(request.WishListId);
            if (targetWishList == null)
                throw new NotFoundException("Target WishList not found.");

            if (!roles.Contains(UserRole.Admin) && targetWishList.OwnerId != userId)
                throw new UnauthorizedAccessException("You are not authorized to move this wish to the target wishlist.");

            var wish = new Wish
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                Priority = request.Priority,
                WishListId = request.WishListId
            };

            var updated = await wishRepository.UpdateAsync(wish);
            return updated.Adapt<WishResponse>();
        }
        catch (DbUpdateException e)
        {
            logger.LogError(e, "Failed to update wish with id {Id}", request.Id);
            throw new ConflictException("Failed to update Wish");
        }
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, List<UserRole> roles)
    {
        try
        {
            var existing = await wishRepository.GetByIdAsync(id);
            if (existing == null)
                throw new NotFoundException("Wish not found.");

            if (!roles.Contains(UserRole.Admin))
            {
                if (existing.WishList == null || existing.WishList.OwnerId != userId)
                    throw new UnauthorizedAccessException("You are not authorized to delete this wish.");
            }

            return await wishRepository.DeleteAsync(id);
        }
        catch (DbException e)
        {
            logger.LogError(e, "Failed to delete wish with id {Id}", id);
            throw new ConflictException("Failed to delete Wish");
        }
    }
}
