namespace Veesh.Core.Common;

public record Pageable<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int Size,
    int Total,
    int LastPage
);