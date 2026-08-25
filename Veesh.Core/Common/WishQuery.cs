namespace Veesh.Core.Common;

public class WishQuery : BaseQuery
{
    public Guid? Id { get; set; }
    public string? Title { get; set; }
    public Guid? WishListId { get; set; }
    public int? Priority { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
