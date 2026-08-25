using Microsoft.EntityFrameworkCore;
using Veesh.Core.Common;
using Veesh.Core.Exceptions;
using Veesh.Core.Interfaces;
using Veesh.Domain.Entities;
using Veesh.Infra.Data;

namespace Veesh.Infra.Repositories;

public class WishStoreUrlRepository(VeeshDbContext context) : IWishStoreUrlRepository
{
    public async Task<Pageable<WishStoreUrl>> GetAllAsync(WishStoreUrlQuery query)
    {
        var queryable = context.WishStoreUrls.AsNoTracking();

        if (query.Id != null)
            queryable = queryable.Where(q => q.Id == query.Id);

        if (query.Title != null)
            queryable = queryable.Where(q => q.Title.Contains(query.Title));

        if (query.WishId != null)
            queryable = queryable.Where(q => q.WishId == query.WishId);

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

        return new Pageable<WishStoreUrl>(items, query.Page, query.Size, total, lastPage);
    }

    public async Task<WishStoreUrl?> GetByIdAsync(Guid id)
    {
        return await context.WishStoreUrls
            .Include(wsu => wsu.Wish)
            .ThenInclude(w => w!.WishList)
            .FirstOrDefaultAsync(wsu => wsu.Id == id);
    }

    public async Task<WishStoreUrl> CreateAsync(WishStoreUrl wishStoreUrl)
    {
        wishStoreUrl.CreatedAt = DateTimeOffset.UtcNow;
        wishStoreUrl.UpdatedAt = DateTimeOffset.UtcNow;
        await context.WishStoreUrls.AddAsync(wishStoreUrl);
        await context.SaveChangesAsync();
        return wishStoreUrl;
    }

    public async Task<WishStoreUrl> UpdateAsync(WishStoreUrl wishStoreUrl)
    {
        var existing = await context.WishStoreUrls.FindAsync(wishStoreUrl.Id);
        if (existing == null) throw new NotFoundException("WishStoreUrl not found");

        existing.Url = wishStoreUrl.Url;
        existing.Title = wishStoreUrl.Title;
        existing.Description = wishStoreUrl.Description;
        existing.ImageUrl = wishStoreUrl.ImageUrl;
        existing.WishId = wishStoreUrl.WishId;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await context.WishStoreUrls.FindAsync(id);
        if (existing == null) throw new NotFoundException("WishStoreUrl not found");

        context.WishStoreUrls.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }
}
