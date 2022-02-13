using CRPL.Data.Account;
using CRPL.Data.Proposal;
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

    public CopyrightController(ILogger<FormsController> logger, IRegistrationService registrationService, ICopyrightService copyrightService)
    {
        Logger = logger;
        RegistrationService = registrationService;
        CopyrightService = copyrightService;
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
    
    [HttpGet("{id}")]
    public async Task<RegisteredWorkWithAppsViewModel> Get([FromRoute]Guid id)
    {
        try
        {
            return await CopyrightService.GetWork(id);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when getting registered work");
            throw;
        }
    }

    [HttpGet("my/{id}")]
    public async Task<List<RegisteredWorkWithAppsViewModel>> GetMy(Guid id)
    {
        try
        {
            return await CopyrightService.GetUsersWorks(id);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when getting users application");
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
            Logger.LogError(e, "Exception thrown when getting users application");
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
            Logger.LogError(e, "Exception thrown when getting users application");
            throw;
        }
    }
}