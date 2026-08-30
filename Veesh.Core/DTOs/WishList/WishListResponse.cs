using Veesh.Domain.Enums;

namespace Veesh.Core.DTOs.WishList;

public record WishListResponse(
    Guid Id,
    string Name,
    string? Description,
    WishlistVisibility Visibility,
    string? CoverImageUrl,
    Guid OwnerId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
