using CRPL.Data.Account;
using CRPL.Data.Proposal;
using CRPL.Web.Core.ChainSync.Synchronisers;
using CRPL.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRPL.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class CopyrightController : ControllerBase
{
    private readonly ILogger<FormsController> Logger;
    private readonly IRegistrationService RegistrationService;
    private readonly ICopyrightService CopyrightService;
    private readonly ISynchroniser OwnershipSynchroniser;

    public CopyrightController(ILogger<FormsController> logger, IRegistrationService registrationService, ICopyrightService copyrightService, IEnumerable<ISynchroniser> synchronisers)
    {
        Logger = logger;
        RegistrationService = registrationService;
        CopyrightService = copyrightService;
        OwnershipSynchroniser = synchronisers.First();
    }
    
    [HttpGet("complete/{id}")]
    public async Task<RegisteredWork> CompleteRegistration([FromRoute]Guid id)
    {
        try
        {
            return await RegistrationService.CompleteRegistration(id);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when getting application");
            throw;
        }
    }

    [HttpPost("bind")]
    public async Task<ActionResult> BindProposal(BindProposalInput proposalInput)
    {
        try
        {
            await CopyrightService.BindProposal(proposalInput);
            return Ok();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when binding a proposal vote");
            throw;
        }
    }
    
    [HttpPost("bind/work")]
    public async Task<ActionResult> BindProposal(BindProposalWorkInput proposalInput)
    {
        try
        {
            await CopyrightService.BindProposal(proposalInput);
            return Ok();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when binding a proposal vote");
            throw;
        }
    }
    
    [HttpPatch("sync/{id}")]
    public async Task<ActionResult> SyncWork(Guid id)
    {
        try
        {
            await OwnershipSynchroniser.SynchroniseOne(id);
            return Ok();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when synchronising work");
            throw;
        }
    }
}