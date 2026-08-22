using System.ComponentModel.DataAnnotations;

namespace Veesh.Core.DTOs.User;

public record UpdateUserRequest(
    Guid Id,
    [Required, MinLength(4), MaxLength(16)]
    string UserName,
    [Required] string PhoneNumber,
    [EmailAddress] string? Email,
    [MaxLength(64)] string? Name,
    [MaxLength(512)] string? Bio,
    [MaxLength(256)]string? ProfileImageUrl,
    DateTimeOffset? BirthDate
);