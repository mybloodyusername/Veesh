using Microsoft.EntityFrameworkCore;
using Veesh.Core.Common;
using Veesh.Core.Exceptions;
using Veesh.Core.Interfaces;
using Veesh.Domain.Entities;
using Veesh.Infra.Data;

namespace Veesh.Infra.Repositories;

public class WishListRepository(VeeshDbContext context) : IWishListRepository
{
    public async Task<Pageable<WishList>> GetAllAsync(WishListQuery query)
    {
        var queryable = context.WishLists.AsNoTracking();

        if (query.Id != null)
            queryable = queryable.Where(q => q.Id == query.Id);

        if (query.Name != null)
            queryable = queryable.Where(q => q.Name.Contains(query.Name));

        if (query.OwnerId != null)
            queryable = queryable.Where(q => q.OwnerId == query.OwnerId);

        if (query.Visibility != null)
            queryable = queryable.Where(q => q.Visibility.ToString() == query.Visibility);

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

        return new Pageable<WishList>(items, query.Page, query.Size, total, lastPage);
    }

    public async Task<WishList?> GetByIdAsync(Guid id)
    {
        return await context.WishLists.FindAsync(id);
    }

    public async Task<WishList> CreateAsync(WishList wishList)
    {
        wishList.CreatedAt = DateTimeOffset.UtcNow;
        wishList.UpdatedAt = DateTimeOffset.UtcNow;
        await context.WishLists.AddAsync(wishList);
        await context.SaveChangesAsync();
        return wishList;
    }

    public async Task<WishList> UpdateAsync(WishList wishList)
    {
        var existing = await context.WishLists.FindAsync(wishList.Id);
        if (existing == null) throw new NotFoundException("WishList not found");

        existing.Name = wishList.Name;
        existing.Description = wishList.Description;
        existing.Visibility = wishList.Visibility;
        existing.CoverImageUrl = wishList.CoverImageUrl;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await context.WishLists.FindAsync(id);
        if (existing == null) throw new NotFoundException("WishList not found");

        context.WishLists.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }
}
