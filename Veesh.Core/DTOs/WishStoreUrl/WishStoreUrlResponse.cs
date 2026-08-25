namespace Veesh.Core.DTOs.WishStoreUrl;

public record WishStoreUrlResponse(
    Guid Id,
    string Url,
    string Title,
    string? Description,
    string? ImageUrl,
    Guid WishId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
