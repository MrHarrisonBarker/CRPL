using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRPL.Web.Controllers;

[ApiController]
[Route("q")]
public class QueryController : ControllerBase
{
    private readonly ILogger<QueryController> Logger;
    private readonly IQueryService QueryService;

    public QueryController(ILogger<QueryController> logger, IQueryService queryService)
    {
        Logger = logger;
        QueryService = queryService;
    }

    [HttpGet("{id}")]
    public async Task<RegisteredWorkWithAppsViewModel> Get([FromRoute] Guid id)
    {
        try
        {
            return await QueryService.GetWork(id);
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
            return await QueryService.GetUsersWorks(id);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when getting users works");
            throw;
        }
    }

    [HttpGet("all")]
    public async Task<List<RegisteredWorkViewModel>> GetAll(int from, int take = 100)
    {
        try
        {
            return await QueryService.GetAll(from, take);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when getting all works");
            throw;
        }
    }

    [HttpPost("search")]
    public async Task<List<RegisteredWorkViewModel>> Search([FromBody]StructuredQuery structuredQuery, int from, int take = 100)
    {
        try
        {
            return await QueryService.Search(structuredQuery, from, take);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when searching for works");
            throw;
        }
    }
}