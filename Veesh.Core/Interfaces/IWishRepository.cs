using Veesh.Core.Common;
using Veesh.Domain.Entities;

namespace Veesh.Core.Interfaces;

public interface IWishRepository
{
    Task<Pageable<Wish>> GetAllAsync(WishQuery query);
    Task<Wish?> GetByIdAsync(Guid id);
    Task<Wish> CreateAsync(Wish wish);
    Task<Wish> UpdateAsync(Wish wish);
    Task<bool> DeleteAsync(Guid id);
}
