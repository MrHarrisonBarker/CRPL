using AutoMapper;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.Services;

public class DisputeService : IDisputeService
{
    private readonly ApplicationContext Context;
    private readonly IMapper Mapper;
    private readonly ILogger<DisputeService> Logger;

    public DisputeService(ILogger<DisputeService> logger, ApplicationContext context, IMapper mapper)
    {
        Context = context;
        Mapper = mapper;
        Logger = logger;
    }

    public Task<DisputeViewModel> AcceptRecourse(Guid disputeId, string message)
    {
        
        throw new NotImplementedException();
    }

    public async Task<DisputeViewModel> RejectRecourse(Guid disputeId, string message)
    {
        var dispute = await Context.DisputeApplications.FirstOrDefaultAsync(x => x.Id == disputeId);
        if (dispute == null) throw new DisputeNotFoundException(disputeId);

        Context.Update(dispute);

        dispute.ResolveResult.Rejected = true;
        dispute.ResolveResult.Message = message;
        dispute.Status = ApplicationStatus.Complete;

        await Context.SaveChangesAsync();

        return Mapper.Map<DisputeViewModel>(dispute);
    }
}