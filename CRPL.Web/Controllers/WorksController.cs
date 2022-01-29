using CRPL.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRPL.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class WorksController : ControllerBase
{
    private readonly ILogger<UserController> Logger;
    private readonly IWorksVerificationService WorksVerificationService;

    public WorksController(ILogger<UserController> logger, IWorksVerificationService worksVerificationService)
    {
        Logger = logger;
        WorksVerificationService = worksVerificationService;
    }

    [HttpPost, DisableRequestSizeLimit]
    public async Task<IActionResult> Upload()
    {
        try
        {
            var formCollection = await Request.ReadFormAsync();
            var file = formCollection.Files.First();
            Ok(await WorksVerificationService.Upload(file));
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when uploading work");
            throw;
        }

        return Ok();
    }
}