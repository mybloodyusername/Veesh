namespace Veesh.Core.Common;

public abstract class BaseQuery
{
    public string? OrderBy { get; set; }
    public int Page { get; set; } = 0;
    public int Size { get; set; } = 20;
}