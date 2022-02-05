using CRPL.Data.StructuredOwnership;

namespace CRPL.Data.Applications.InputModels;

public class CopyrightRegistrationInputModel : ApplicationInputModel
{
    public string? Title { get; set; }
    public byte[]? WorkHash { get; set; }
    public string? WorkUri { get; set; }
    public string? Legal { get; set; }
    public List<OwnershipStake>? OwnershipStakes { get; set; }
}