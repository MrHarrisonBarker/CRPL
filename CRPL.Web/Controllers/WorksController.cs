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

    [HttpGet]
    public IActionResult SignWork(string hash, string signature)
    {
        try
        {
            // .Replace('_','/').Replace('-','=')
            var decodedHash = Convert.FromBase64String(hash.Replace('.','+'));
            var work = WorksVerificationService.Sign(decodedHash, signature);

            return File(work.Work, work.ContentType, "name.jpg");
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when signing work");
            throw;
        }
    }
}