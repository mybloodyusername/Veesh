using System.ComponentModel.DataAnnotations;

namespace Veesh.Domain.Entities;

public class Wish : BaseEntity
{
    [Required] [MaxLength(1024)] public string Title { get; set; } = string.Empty;

    [MaxLength(2048)] public string? Description { get; set; }

    [MaxLength(2048)] public string? ImageUrl { get; set; }

    public ICollection<WishStoreUrl> Stores { get; set; } = new List<WishStoreUrl>();

    public decimal? Price { get; set; }

    [MaxLength(10)] public string? Currency { get; set; }

    public int Priority { get; set; }

    public Guid WishListId { get; set; }
    public WishList? WishList { get; set; }
}