using Veesh.Core.Common;
using Veesh.Domain.Entities;

namespace Veesh.Core.Interfaces;

public interface IWishListRepository
{
    Task<Pageable<WishList>> GetAllAsync(WishListQuery query);
    Task<WishList?> GetByIdAsync(Guid id);
    Task<WishList> CreateAsync(WishList wishList);
    Task<WishList> UpdateAsync(WishList wishList);
    Task<bool> DeleteAsync(Guid id);
}
