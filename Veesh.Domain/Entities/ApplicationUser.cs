using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Veesh.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    [MaxLength(64)] public string? Name { get; set; }
    
    [MaxLength(2048)] public string? Bio { get; set; }

    [MaxLength(2048)] public string? ProfileImageUrl { get; set; }
    
    public DateTimeOffset? BirthDate { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public DateTimeOffset UpdatedAt { get; set; } =  DateTimeOffset.UtcNow;
    
    public ICollection<WishList> WishLists { get; set; } = new List<WishList>();
}