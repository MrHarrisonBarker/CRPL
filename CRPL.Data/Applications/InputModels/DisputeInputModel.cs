using CRPL.Data.Applications.Core;

namespace CRPL.Data.Applications.InputModels;

public class DisputeInputModel : ApplicationInputModel
{
    public DisputeType DisputeType { get; set; }
    public string Reason { get; set; }
    public DateTime? Spotted { get; set; }
    public int? Infractions { get; set; }
    public ExpectedRecourse? ExpectedRecourse { get; set; }
    public string? ExpectedRecourseData { get; set; }
    public string? ContactAddress { get; set; }
    public string? LinkToInfraction { get; set; }
     
    public Guid? DisputedWorkId { get; set; }
    public Guid? AccuserId { get; set; }
}