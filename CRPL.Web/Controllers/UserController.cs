using CRPL.Data.Account.InputModels;
using CRPL.Data.Account.StatusModels;
using CRPL.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CRPL.Web.Controllers;

[ApiController]
[Route("[controller]")]
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
    public async Task<UserAccountStatusModel> UpdateAccount(Guid accountId, AccountInputModel accountInputModel)
    {
        try
        {
            return await UserService.UpdateAccount(accountId, accountInputModel);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when updating a user account");
            throw;
        }
    }

    [HttpGet]
    // TODO: ownership needs to verified
    public async Task<UserAccountStatusModel> GetAccount(Guid id)
    {
        try
        {
            return await UserService.GetAccount(id);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when getting a user account");
            throw;
        }
    }

    [HttpPost("nonce")]
    public async Task<ActionResult<string>> FetchNonce(string walletAddress)
    {
        try
        {
            return Ok(JsonConvert.SerializeObject(await UserService.FetchNonce(walletAddress)));
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when fetching a nonce");
            throw;
        }
    }

    [HttpPost("sig")]
    public async Task<AuthenticateResult> AuthenticateSignature(AuthenticateSignatureInputModel authenticateInputModel)
    {
        try
        {
            return await UserService.AuthenticateSignature(authenticateInputModel);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when authenticating signature");
            throw;
        }
    }

    [HttpGet("auth")]
    public async Task<ActionResult> Authenticate(string token)
    {
        try
        {
            await UserService.Authenticate(token);
            return Ok();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when authenticating token");
            throw;
        }
    }

    [HttpDelete("auth")]
    public async Task<ActionResult> RevokeAuthenticate(string token)
    {
        try
        {
            await UserService.RevokeAuthentication(token);
            return Ok();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when revoking authentication");
            throw;
        }
    }
}