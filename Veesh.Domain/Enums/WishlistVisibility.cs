using System.Text.Json.Serialization;

namespace Veesh.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WishlistVisibility
{
    Public,
    Private
}