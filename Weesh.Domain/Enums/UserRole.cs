using System.Text.Json.Serialization;

namespace Weesh.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    Admin,
    User
}