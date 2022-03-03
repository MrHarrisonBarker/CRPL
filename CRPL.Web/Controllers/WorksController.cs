using CRPL.Data.Account.Works;
using CRPL.Web.Services.Background.Usage;
using CRPL.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRPL.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class WorksController : ControllerBase
{
    private readonly ILogger<UserController> Logger;
    private readonly IWorksVerificationService WorksVerificationService;
    private readonly IUsageQueue UsageQueue;

    public WorksController(ILogger<UserController> logger, IWorksVerificationService worksVerificationService, IUsageQueue usageQueue)
    {
        Logger = logger;
        WorksVerificationService = worksVerificationService;
        UsageQueue = usageQueue;
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

    [HttpPatch("ping/{id}")]
    public async Task<IActionResult> Ping(Guid id)
    {
        try
        {
            UsageQueue.QueueUsage(new WorkUsage
            {
                WorkId = id,
                TimeStamp = DateTime.Now,
                UsageType = UsageType.Ping
            });
            return Ok();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when pinging");
            throw;
        }
    }
}