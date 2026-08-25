using Veesh.Core.Common;
using Veesh.Domain.Entities;

namespace Veesh.Core.Interfaces;

public interface IWishStoreUrlRepository
{
    Task<Pageable<WishStoreUrl>> GetAllAsync(WishStoreUrlQuery query);
    Task<WishStoreUrl?> GetByIdAsync(Guid id);
    Task<WishStoreUrl> CreateAsync(WishStoreUrl wishStoreUrl);
    Task<WishStoreUrl> UpdateAsync(WishStoreUrl wishStoreUrl);
    Task<bool> DeleteAsync(Guid id);
}
