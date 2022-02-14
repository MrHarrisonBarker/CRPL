using CRPL.Data.Account.ViewModels;
using CRPL.Data.Applications.ViewModels;

namespace CRPL.Data.Account;

public class RegisteredWorkWithAppsViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Registered { get; set; }
    public RegisteredWorkStatus Status { get; set; }
    public string? RightId { get; set; }
    public byte[]? Hash { get; set; }
    public string? RegisteredTransactionId { get; set; }
    public List<Contracts.Structs.OwnershipStakeContract>? OwnershipStructure { get; set; }
    public List<Contracts.Structs.ProposalVote>? CurrentVotes { get; set; }
    public bool? HasProposal { get; set; }
    public Contracts.Structs.Meta? Meta { get; set; }
    public List<UserAccountMinimalViewModel> AssociatedUsers { get; set; }
    public List<ApplicationViewModelWithoutAssociated> AssociatedApplication { get; set; }
}