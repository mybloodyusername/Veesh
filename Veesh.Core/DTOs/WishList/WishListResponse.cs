using Veesh.Core.DTOs.Wish;
using Veesh.Domain.Enums;

namespace Veesh.Core.DTOs.WishList;

public record WishListResponse(
    Guid Id,
    string Name,
    string? Description,
    WishlistVisibility Visibility,
    ICollection<WishResponse> Wishes,
    string? CoverImageUrl,
    Guid OwnerId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
