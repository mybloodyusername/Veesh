using System.ComponentModel.DataAnnotations;

namespace Veesh.Core.DTOs.Wish;

public record CreateWishRequest(
    [Required, MaxLength(1024)] string Title,
    [MaxLength(2048)] string? Description,
    [MaxLength(2048)] string? ImageUrl,
    int Priority,
    Guid WishListId
);
