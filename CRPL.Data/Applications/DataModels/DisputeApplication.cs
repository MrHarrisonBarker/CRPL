using CRPL.Data.Applications.Core;

namespace CRPL.Data.Applications.DataModels;

public class DisputeApplication : Application
{
    public DisputeApplication() : base(ApplicationType.Dispute) { }
    
    public DisputeType DisputeType { get; set; }
    public string Reason { get; set; }
    public DateTime? Spotted { get; set; }
    public int? Infractions { get; set; }
    
    // What is expected to happen if the dispute is accepted by the work owner
    public ExpectedRecourse? ExpectedRecourse { get; set; }
    public string? ExpectedRecourseData { get; set; }
    
    // Relationship between Application and Application of the recourse taken by the owner
    public Application? ExpectedRecourseApplication { get; set; }
    public string? ContactAddress { get; set; }
    public string? LinkToInfraction { get; set; }
    
    // Resolving needed a more complex status object
    public ResolveResult ResolveResult { get; set; }
}