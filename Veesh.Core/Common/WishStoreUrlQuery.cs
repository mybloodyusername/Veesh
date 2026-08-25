namespace Veesh.Core.Common;

public class WishStoreUrlQuery : BaseQuery
{
    public Guid? Id { get; set; }
    public string? Title { get; set; }
    public Guid? WishId { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
