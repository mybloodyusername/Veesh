using System.ComponentModel.DataAnnotations;

namespace Veesh.Core.DTOs.Auth;

public record RegisterRequest(
    [Required] string PhoneNumber,
    [Required, MinLength(4), MaxLength(16)]
    string UserName,
    [EmailAddress] string Email,
    [MaxLength(64)] string Name,
    [Required, MaxLength(8)] string Password);