using System.Data.Common;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Veesh.Core.Common;
using Veesh.Core.DTOs.WishStoreUrl;
using Veesh.Core.Exceptions;
using Veesh.Core.Interfaces;
using Veesh.Domain.Entities;
using Veesh.Domain.Enums;

namespace Veesh.Core.Services;

public class WishStoreUrlService(
    IWishStoreUrlRepository wishStoreUrlRepository,
    IWishRepository wishRepository,
    ILogger<WishStoreUrlService> logger)
{
    public async Task<Pageable<WishStoreUrlResponse>> GetAllAsync(WishStoreUrlQuery query, Guid userId, List<UserRole> roles)
    {
        if (!roles.Contains(UserRole.Admin))
        {
            var wishQuery = new WishQuery { Size = int.MaxValue };
            var allWishes = await wishRepository.GetAllAsync(wishQuery);
            var userWishIds = allWishes.Items
                .Where(w => w.WishList != null && w.WishList.OwnerId == userId)
                .Select(w => w.Id)
                .ToList();

            if (userWishIds.Count == 0)
                return new Pageable<WishStoreUrlResponse>(new List<WishStoreUrlResponse>(), 0, query.Size, 0, 0);

            var allUserUrls = new List<WishStoreUrl>();
            foreach (var wishId in userWishIds)
            {
                var urlQuery = new WishStoreUrlQuery { WishId = wishId, Size = int.MaxValue };
                var page = await wishStoreUrlRepository.GetAllAsync(urlQuery);
                allUserUrls.AddRange(page.Items);
            }

            var total = allUserUrls.Count;
            var lastPage = total / query.Size;
            var items = allUserUrls
                .Skip(query.Page * query.Size)
                .Take(query.Size)
                .ToList();

            return new Pageable<WishStoreUrlResponse>(items.Adapt<List<WishStoreUrlResponse>>(), query.Page, query.Size, total, lastPage);
        }

        var result = await wishStoreUrlRepository.GetAllAsync(query);
        return result.Adapt<Pageable<WishStoreUrlResponse>>();
    }

    public async Task<WishStoreUrlResponse> GetByIdAsync(Guid id, Guid userId, List<UserRole> roles)
    {
        var storeUrl = await wishStoreUrlRepository.GetByIdAsync(id);
        if (storeUrl == null)
            throw new NotFoundException("WishStoreUrl not found.");

        if (!roles.Contains(UserRole.Admin))
        {
            if (storeUrl.Wish == null || storeUrl.Wish.WishList == null || storeUrl.Wish.WishList.OwnerId != userId)
                throw new UnauthorizedAccessException("You are not authorized to view this store URL.");
        }

        return storeUrl.Adapt<WishStoreUrlResponse>();
    }

    public async Task<WishStoreUrlResponse> CreateAsync(CreateWishStoreUrlRequest request, Guid userId, List<UserRole> roles)
    {
        try
        {
            var wish = await wishRepository.GetByIdAsync(request.WishId);
            if (wish == null)
                throw new NotFoundException("Wish not found.");

            if (!roles.Contains(UserRole.Admin))
            {
                if (wish.WishList == null || wish.WishList.OwnerId != userId)
                    throw new UnauthorizedAccessException("You are not authorized to add store URLs to this wish.");
            }

            var storeUrl = new WishStoreUrl
            {
                Url = request.Url,
                Title = request.Title,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                WishId = request.WishId
            };

            var created = await wishStoreUrlRepository.CreateAsync(storeUrl);
            return created.Adapt<WishStoreUrlResponse>();
        }
        catch (DbUpdateException e)
        {
            logger.LogError(e, "Failed to create store URL for wish {WishId}", request.WishId);
            throw new ConflictException("Failed to create WishStoreUrl");
        }
    }

    public async Task<WishStoreUrlResponse> UpdateAsync(UpdateWishStoreUrlRequest request, Guid userId, List<UserRole> roles)
    {
        try
        {
            var existing = await wishStoreUrlRepository.GetByIdAsync(request.Id);
            if (existing == null)
                throw new NotFoundException("WishStoreUrl not found.");

            if (!roles.Contains(UserRole.Admin))
            {
                if (existing.Wish == null || existing.Wish.WishList == null || existing.Wish.WishList.OwnerId != userId)
                    throw new UnauthorizedAccessException("You are not authorized to update this store URL.");
            }

            var targetWish = await wishRepository.GetByIdAsync(request.WishId);
            if (targetWish == null)
                throw new NotFoundException("Target Wish not found.");

            if (!roles.Contains(UserRole.Admin))
            {
                if (targetWish.WishList == null || targetWish.WishList.OwnerId != userId)
                    throw new UnauthorizedAccessException("You are not authorized to move this store URL to the target wish.");
            }

            var storeUrl = new WishStoreUrl
            {
                Id = request.Id,
                Url = request.Url,
                Title = request.Title,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                WishId = request.WishId
            };

            var updated = await wishStoreUrlRepository.UpdateAsync(storeUrl);
            return updated.Adapt<WishStoreUrlResponse>();
        }
        catch (DbUpdateException e)
        {
            logger.LogError(e, "Failed to update store URL with id {Id}", request.Id);
            throw new ConflictException("Failed to update WishStoreUrl");
        }
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, List<UserRole> roles)
    {
        try
        {
            var existing = await wishStoreUrlRepository.GetByIdAsync(id);
            if (existing == null)
                throw new NotFoundException("WishStoreUrl not found.");

            if (!roles.Contains(UserRole.Admin))
            {
                if (existing.Wish == null || existing.Wish.WishList == null || existing.Wish.WishList.OwnerId != userId)
                    throw new UnauthorizedAccessException("You are not authorized to delete this store URL.");
            }

            return await wishStoreUrlRepository.DeleteAsync(id);
        }
        catch (DbException e)
        {
            logger.LogError(e, "Failed to delete store URL with id {Id}", id);
            throw new ConflictException("Failed to delete WishStoreUrl");
        }
    }
}
