using System.ComponentModel.DataAnnotations;

namespace Veesh.Core.DTOs.Wish;

public record UpdateWishPriorityRequest(
    [Required] List<WishPriority> WishesPriorities
);