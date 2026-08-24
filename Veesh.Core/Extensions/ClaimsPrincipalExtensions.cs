using System.Security.Claims;
using Veesh.Domain.Enums;

namespace Veesh.Core.Extensions;

public static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal principal)
    {
        public Guid GetUserId()
        {
            var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var userId)
                ? userId
                : throw new UnauthorizedAccessException("Invalid user id in token.");
        }

        public List<UserRole> GetRoles()
        {
            var roleClaims = principal.FindAll(ClaimTypes.Role);

            var roles = new List<UserRole>();

            foreach (var claim in roleClaims)
            {
                if (!Enum.TryParse<UserRole>(claim.Value, ignoreCase: true, out var role))
                {
                    throw new UnauthorizedAccessException(
                        $"Invalid role '{claim.Value}' in token.");
                }

                roles.Add(role);
            }

            return roles.Count == 0 ? throw new UnauthorizedAccessException("No roles found in token.") : roles;
        }
    }
}