using CRPL.Data.Account.InputModels;
using CRPL.Data.Account.StatusModels;
using CRPL.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRPL.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> Logger;
    private readonly IUserService UserService;
    
    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        Logger = logger;
        UserService = userService;
    }

    [HttpPost]
    public Task<UserAccountStatusModel> UpdateAccount(Guid accountId, AccountInputModel accountInputModel)
    {
        try
        {
            return UserService.UpdateAccount(accountId, accountInputModel);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when updating a user account");
            throw;
        }
    }

    [HttpGet]
    // TODO: ownership needs to verified
    public Task<UserAccountStatusModel> GetAccount(Guid id)
    {
        try
        {
            return UserService.GetAccount(id);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when getting a user account");
            throw;
        }
    }
}