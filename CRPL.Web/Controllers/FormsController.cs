using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRPL.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class FormsController : ControllerBase
{
    private readonly ILogger<FormsController> Logger;
    private readonly IFormsService FormsService;

    public FormsController(ILogger<FormsController> logger, IFormsService formsService)
    {
        Logger = logger;
        FormsService = formsService;
    }

    [HttpGet("#id")]
    public async Task<ApplicationViewModel> Get(Guid id)
    {
        try
        {
            return await FormsService.GetApplication(id);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when getting application");
            throw;
        }
    }

    [HttpGet("users/#id")]
    public async Task<List<ApplicationViewModel>> GetMy(Guid id)
    {
        try
        {
            return await FormsService.GetMyApplications(id);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when getting application");
            throw;
        }
    }
}