namespace CRPL.Data;

public class StructuredQuery
{
    public string? Keyword { get; set; }
    public Sortable? SortBy { get; set; }
    public Dictionary<WorkFilter, string>? WorkFilters { get; set; }

    public override string ToString()
    {
        var msg = $"\nKeyword -> {Keyword}\nSorted By -> {SortBy.ToString()}\n";
        if (WorkFilters?.Keys != null)
            foreach (var filter in WorkFilters?.Keys)
            {
                msg += $"{filter.ToString()} -> {WorkFilters[filter]}";
            }

        return msg;
    }
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