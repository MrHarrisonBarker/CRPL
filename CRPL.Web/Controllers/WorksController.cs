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
    public async Task<byte[]> Upload()
    {
        try
        {
            var formCollection = await Request.ReadFormAsync();
            var file = formCollection.Files.First();

            return await WorksVerificationService.Upload(file);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when uploading work");
            throw;
        }
    }

    // [HttpGet("{id}")]
    // public async Task<IActionResult> GetSignedWork(Guid id)
    // {
    //     try
    //     {
    //         var work = await WorksVerificationService.GetSigned(id);
    //
    //         return File(work.Work, work.ContentType, work.FileName);
    //     }
    //     catch (Exception e)
    //     {
    //         Logger.LogError(e, "Exception thrown when getting signed work");
    //         throw;
    //     }
    // }
}