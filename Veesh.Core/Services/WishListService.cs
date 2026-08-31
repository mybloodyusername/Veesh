using System.Data.Common;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Veesh.Core.Common;
using Veesh.Core.DTOs.Wish;
using Veesh.Core.DTOs.WishList;
using Veesh.Core.Exceptions;
using Veesh.Core.Interfaces;
using Veesh.Domain.Entities;
using Veesh.Domain.Enums;

namespace Veesh.Core.Services;

public class WishListService(
    IWishListRepository wishListRepository,
    IWishRepository wishRepository,
    ILogger<WishListService> logger)
{
    public async Task<Pageable<WishListResponse>> GetAllAsync(WishListQuery query, Guid userId, List<UserRole> roles)
    {
        if (!roles.Contains(UserRole.Admin))
            query.OwnerId = userId;

        var result = await wishListRepository.GetAllAsync(query);
        return result.Adapt<Pageable<WishListResponse>>();
    }

    public async Task<WishListResponse> GetByIdAsync(Guid id, Guid userId, List<UserRole> roles)
    {
        var wishList = await wishListRepository.GetByIdAsync(id);
        if (wishList == null)
            throw new NotFoundException("WishList not found.");

        if (!roles.Contains(UserRole.Admin) && wishList.OwnerId != userId)
            throw new UnauthorizedAccessException("You are not authorized to view this wishlist.");

        return wishList.Adapt<WishListResponse>();
    }

    public async Task<WishListResponse> CreateAsync(CreateWishListRequest request, Guid userId, List<UserRole> roles)
    {
        try
        {
            Guid ownerId;

            if (roles.Contains(UserRole.Admin))
            {
                if (request.OwnerId == null)
                    throw new UnauthorizedAccessException(
                        "Admins are not allowed to create wishlists for themselves. Provide an OwnerId.");
                ownerId = request.OwnerId.Value;
            }
            else
            {
                ownerId = userId;
            }

            var wishList = new WishList
            {
                Name = request.Name,
                Description = request.Description,
                Visibility = request.Visibility,
                CoverImageUrl = request.CoverImageUrl,
                OwnerId = ownerId
            };

            var created = await wishListRepository.CreateAsync(wishList);
            return created.Adapt<WishListResponse>();
        }
        catch (DbUpdateException e)
        {
            logger.LogError(e, "Failed to create wishlist for user {UserId}", userId);
            throw new ConflictException("Failed to create WishList");
        }
    }

    public async Task<WishListResponse> UpdateAsync(UpdateWishListRequest request, Guid userId, List<UserRole> roles)
    {
        try
        {
            var existing = await wishListRepository.GetByIdAsync(request.Id);
            if (existing == null)
                throw new NotFoundException("WishList not found.");

            if (!roles.Contains(UserRole.Admin) && existing.OwnerId != userId)
                throw new UnauthorizedAccessException("You are not authorized to update this wishlist.");

            var wishList = new WishList
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                Visibility = request.Visibility,
                CoverImageUrl = request.CoverImageUrl
            };

            var updated = await wishListRepository.UpdateAsync(wishList);
            return updated.Adapt<WishListResponse>();
        }
        catch (DbUpdateException e)
        {
            logger.LogError(e, "Failed to update wishlist with id {Id}", request.Id);
            throw new ConflictException("Failed to update WishList");
        }
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, List<UserRole> roles)
    {
        try
        {
            var existing = await wishListRepository.GetByIdAsync(id);
            if (existing == null)
                throw new NotFoundException("WishList not found.");

            if (!roles.Contains(UserRole.Admin) && existing.OwnerId != userId)
                throw new UnauthorizedAccessException("You are not authorized to delete this wishlist.");

            return await wishListRepository.DeleteAsync(id);
        }
        catch (DbException e)
        {
            logger.LogError(e, "Failed to delete wishlist with id {Id}", id);
            throw new ConflictException("Failed to delete WishList");
        }
    }

    public async Task<WishListResponse> UpdateWishesPrioritiesAsync(UpdateWishPriorityRequest request, Guid wishListId,
        Guid userId,
        List<UserRole> roles)
    {
        try
        {
            var wishList = await wishListRepository.GetByIdAsync(wishListId);
            if (wishList == null) throw new NotFoundException("WishList not found.");
            if (!roles.Contains(UserRole.Admin) && wishList.OwnerId != userId)
                throw new UnauthorizedAccessException("You are not authorized to update wishes in this wishlist.");

            var wishPriorityDictionary = request.WishesPriorities.ToDictionary(w => w.Id, w => w.Priority);
            if (!wishList.Wishes.All(w => wishPriorityDictionary.ContainsKey(w.Id)))
                throw new ConflictException("Not all wishes are available.");

            var updatedWishes = await wishRepository.UpdatePriorityAsync(wishPriorityDictionary, wishList.Id);
            wishList.Wishes = updatedWishes;

            return wishList.Adapt<WishListResponse>();
        }
        catch (DbUpdateException e)
        {
            logger.LogError(e, "Failed to update wishes priority in wishlist with id {Id}", wishListId);
            throw new ConflictException("Failed to update Wishes priority in WishList");
        }
    }
}
