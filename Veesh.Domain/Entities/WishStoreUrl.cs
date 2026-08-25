using System.ComponentModel.DataAnnotations;

namespace Veesh.Domain.Entities;

public class WishStoreUrl : BaseEntity
{
    [Required, MaxLength(2048)] public required string Url { get; set; }

    [Required] [MaxLength(1024)] public string Title { get; set; } = string.Empty;

    [MaxLength(2048)] public string? Description { get; set; }

    [MaxLength(2048)] public string? ImageUrl { get; set; }

    public Guid WishId { get; set; }
    public Wish? Wish { get; set; }
}