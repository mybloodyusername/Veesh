namespace Veesh.Core.Common;

public class WishListQuery : BaseQuery
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public Guid? OwnerId { get; set; }
    public string? Visibility { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
