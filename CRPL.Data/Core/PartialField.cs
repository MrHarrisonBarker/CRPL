using CRPL.Data.Applications;

namespace CRPL.Data;

public class PartialField
{
    public Guid Id { get; set; }
    
    public string Field { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }
    
    public Application Application { get; set; }
}