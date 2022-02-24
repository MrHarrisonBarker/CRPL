using CRPL.Data.Applications.Core;

namespace CRPL.Data.Applications.DataModels;

public class DisputeApplication : Application
{
    public DisputeApplication() : base(ApplicationType.Dispute) { }
    
    public DisputeType DisputeType { get; set; }
    public string Reason { get; set; }
    public DateTime? Spotted { get; set; }
    public int? Infractions { get; set; }
    public ExpectedRecourse? ExpectedRecourse { get; set; }
    public string? ExpectedRecourseData { get; set; }
    public string? ContactAddress { get; set; }
    public string? LinkToInfraction { get; set; }
}