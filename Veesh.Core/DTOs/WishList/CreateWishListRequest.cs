using System.ComponentModel.DataAnnotations;
using Veesh.Domain.Enums;

namespace Veesh.Core.DTOs.WishList;

public record CreateWishListRequest(
    [Required, MaxLength(1024)] string Name,
    [MaxLength(2048)] string? Description,
    WishlistVisibility Visibility,
    [MaxLength(2048)] string? CoverImageUrl,
    Guid? OwnerId
);
