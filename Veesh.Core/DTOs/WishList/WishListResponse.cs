namespace Veesh.Core.DTOs.WishList;

public record WishListResponse(
    Guid Id,
    string Name,
    string? Description,
    string Visibility,
    string? CoverImageUrl,
    Guid OwnerId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
