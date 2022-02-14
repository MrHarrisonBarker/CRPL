namespace CRPL.Data;

public class StructuredQuery
{
    public string? Keyword { get; set; }
    public Sortable? SortBy { get; set; }
    public Dictionary<WorkFilter, string>? WorkFilters { get; set; }
}

public enum WorkFilter
{
    RegisteredAfter,
    RegisteredBefore,
}

public enum Sortable
{
    Created,
    Title,
}