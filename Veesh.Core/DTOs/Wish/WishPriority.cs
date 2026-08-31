using System.ComponentModel.DataAnnotations;

namespace Veesh.Core.DTOs.Wish;

public record WishPriority(
    [Required] Guid Id,
    [Required] int Priority
);