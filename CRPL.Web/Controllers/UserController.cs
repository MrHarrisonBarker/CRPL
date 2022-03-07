using CRPL.Data.Account.InputModels;
using CRPL.Data.Account.StatusModels;
using CRPL.Data.Account.ViewModels;
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
    public async Task<UserAccountViewModel> Authenticate(string token)
    {
        try
        {
            return await UserService.Authenticate(token);
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

    [HttpGet("unique/phone/{phone}")]
    public async Task<bool> IsUniquePhoneNumber(Guid user, [FromRoute]string phone)
    {
        try
        {
            return await UserService.IsUniquePhoneNumber(user, phone);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when checking uniqueness of phone number");
            throw;
        }
    }

    [HttpGet("unique/email/{email}")]
    public async Task<bool> IsUniqueEmail(Guid user, [FromRoute]string email)
    {
        try
        {
            return await UserService.IsUniqueEmail(user, email);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when checking uniqueness of phone number");
            throw;
        }
    }

    [HttpGet("real/{address}")]
    public bool IsRealUser(string address)
    {
        try
        {
            return UserService.AreUsersReal(new List<string> { address });
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when searching for users by address");
            throw;
        }
    }
}