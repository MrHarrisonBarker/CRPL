using CRPL.Data.Account;
using CRPL.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRPL.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class CopyrightController : ControllerBase
{
    private readonly ILogger<FormsController> Logger;
    private readonly IRegistrationService RegistrationService;
    
    public CopyrightController(ILogger<FormsController> logger, IRegistrationService registrationService)
    {
        Logger = logger;
        RegistrationService = registrationService;
    }
    
    [HttpGet("{id}")]
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
}