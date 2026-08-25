using System.ComponentModel.DataAnnotations;
using Veesh.Domain.Enums;

namespace Veesh.Domain.Entities;

public class WishList : BaseEntity
{
    [Required, MaxLength(1024)] public required string Name { get; set; }

    [MaxLength(2048)] public string? Description { get; set; }

    public WishlistVisibility Visibility { get; set; } = WishlistVisibility.Private;

    [MaxLength(2048)] public string? CoverImageUrl { get; set; }

    public ICollection<Wish> Wishes { get; set; } = new List<Wish>();

    public Guid OwnerId { get; set; }
    public required ApplicationUser Owner { get; set; }
}