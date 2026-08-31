using Microsoft.EntityFrameworkCore;
using Veesh.Core.Common;
using Veesh.Core.DTOs.Wish;
using Veesh.Core.Exceptions;
using Veesh.Core.Interfaces;
using Veesh.Domain.Entities;
using Veesh.Infra.Data;

namespace Veesh.Infra.Repositories;

public class WishRepository(VeeshDbContext context) : IWishRepository
{
    public async Task<Pageable<Wish>> GetAllAsync(WishQuery query)
    {
        var queryable = context.Wishes.AsNoTracking();

        if (query.Id != null)
            queryable = queryable.Where(q => q.Id == query.Id);

        if (query.Title != null)
            queryable = queryable.Where(q => q.Title.Contains(query.Title));

        if (query.WishListId != null)
            queryable = queryable.Where(q => q.WishListId == query.WishListId);

        if (query.Priority != null)
            queryable = queryable.Where(q => q.Priority == query.Priority);

        if (query.CreatedAt != null)
            queryable = queryable.Where(q => q.CreatedAt == query.CreatedAt);

        if (query.UpdatedAt != null)
            queryable = queryable.Where(q => q.UpdatedAt == query.UpdatedAt);

        if (query.OrderBy != null)
            queryable = queryable.OrderBy(q => query.OrderBy);

        var total = await queryable.CountAsync();
        var lastPage = total / query.Size;
        var items = await queryable
            .Skip(query.Page * query.Size)
            .Take(query.Size)
            .ToListAsync();

        return new Pageable<Wish>(items, query.Page, query.Size, total, lastPage);
    }

    public async Task<Wish?> GetByIdAsync(Guid id)
    {
        return await context.Wishes
            .Include(w => w.WishList)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<Wish> CreateAsync(Wish wish)
    {
        wish.CreatedAt = DateTimeOffset.UtcNow;
        wish.UpdatedAt = DateTimeOffset.UtcNow;
        await context.Wishes.AddAsync(wish);
        await context.SaveChangesAsync();
        return wish;
    }

    public async Task<Wish> UpdateAsync(Wish wish)
    {
        var existing = await context.Wishes.FindAsync(wish.Id);
        if (existing == null) throw new NotFoundException("Wish not found");

        existing.Title = wish.Title;
        existing.Description = wish.Description;
        existing.ImageUrl = wish.ImageUrl;
        existing.Priority = wish.Priority;
        existing.WishListId = wish.WishListId;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await context.Wishes.FindAsync(id);
        if (existing == null) throw new NotFoundException("Wish not found");

        context.Wishes.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Wish>> GetAllWishesByWishListIdAsync(Guid wishListId)
    {
        return await context.Wishes
            .Where(w => w.WishListId == wishListId)
            .OrderBy(w => w.Priority)
            .ToListAsync();
    }

    public async Task<ICollection<Wish>> UpdatePriorityAsync(Dictionary<Guid, int> wishPriorities, Guid wishListId)
    {
        var wishes = await GetAllWishesByWishListIdAsync(wishListId);
        foreach (var wish in wishes)
        {
            wish.Priority = wishPriorities[wish.Id];
        }

        await context.SaveChangesAsync();
        return wishes;
    }
}