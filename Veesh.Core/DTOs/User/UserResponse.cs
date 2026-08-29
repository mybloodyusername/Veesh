using Veesh.Domain.Enums;

namespace Veesh.Core.DTOs.User;

public record UserResponse(
    Guid Id,
    string UserName,
    string PhoneNumber,
    string Email,
    string Name,
    string Bio,
    string ProfileImageUrl,
    DateTimeOffset BirthDate,
    UserRole? Role
    );