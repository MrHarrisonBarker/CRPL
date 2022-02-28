using CRPL.Data.Applications.ViewModels;

namespace CRPL.Web.Services.Interfaces;

public interface IDisputeService
{
    public Task<DisputeViewModel> AcceptRecourse(Guid disputeId, string message);
    public Task<DisputeViewModel> RejectRecourse(Guid disputeId, string message);
    public Task RecordPaymentAndResolve(Guid disputeId, string transaction);
    public Task<OwnershipRestructureViewModel> RestructureAndResolve(Guid disputeId);
}