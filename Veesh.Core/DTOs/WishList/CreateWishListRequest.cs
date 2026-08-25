using System.ComponentModel.DataAnnotations;

namespace Veesh.Core.DTOs.WishList;

public record CreateWishListRequest(
    [Required, MaxLength(1024)] string Name,
    [MaxLength(2048)] string? Description,
    string? Visibility,
    [MaxLength(2048)] string? CoverImageUrl
);
