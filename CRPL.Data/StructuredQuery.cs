namespace CRPL.Data;

public class StructuredQuery
{
    public string? Keyword { get; set; }
    public Sortable? SortBy { get; set; }
    public Dictionary<WorkFilter, bool>? WorkFilters { get; set; }
}

public enum WorkFilter
{
}

public enum Sortable
{
    Created,
    Title,
    
}