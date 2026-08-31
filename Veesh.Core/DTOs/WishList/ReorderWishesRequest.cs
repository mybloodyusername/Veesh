namespace Veesh.Core.DTOs.WishList;

public record ReorderWishItem(Guid WishId, Guid WishListId, int Priority);

public record ReorderWishesRequest(List<ReorderWishItem> Wishes);
