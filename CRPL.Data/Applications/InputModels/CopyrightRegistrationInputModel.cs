using CRPL.Contracts.Structs;
using CRPL.Data.Applications.ViewModels;
using CRPL.Data.StructuredOwnership;

namespace CRPL.Data.Applications.InputModels;

public class CopyrightRegistrationInputModel : ApplicationInputModel
{
    public string? Title { get; set; }
    public byte[]? WorkHash { get; set; }
    public string? WorkUri { get; set; }
    public string? Legal { get; set; }
    public List<OwnershipStake>? OwnershipStakes { get; set; }
    public WorkType WorkType { get; set; }
    public int YearsExpire { get; set; }
    public Protections Protections { get; set; }
}