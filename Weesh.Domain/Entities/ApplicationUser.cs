using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Weesh.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    [MaxLength(64)] public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)] public string? Bio { get; set; }

    [MaxLength(255)] public string? ProfileImageUrl { get; set; }
    
    public DateTimeOffset? BirthDate { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
}