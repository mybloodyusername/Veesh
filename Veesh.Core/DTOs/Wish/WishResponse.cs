namespace Veesh.Core.DTOs.Wish;

public record WishResponse(
    Guid Id,
    string Title,
    string? Description,
    string? ImageUrl,
    int Priority,
    Guid WishListId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
