using System.ComponentModel.DataAnnotations;

namespace Veesh.Core.DTOs.WishStoreUrl;

public record CreateWishStoreUrlRequest(
    [Required, MaxLength(2048)] string Url,
    [Required, MaxLength(1024)] string Title,
    [MaxLength(2048)] string? Description,
    [MaxLength(2048)] string? ImageUrl,
    Guid WishId
);
