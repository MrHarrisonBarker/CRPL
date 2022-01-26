using CRPL.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRPL.Web;

public class AccountOwnsFilter : IAsyncAuthorizationFilter
{
    private readonly IUserService UserService;

    public AccountOwnsFilter(IUserService userService)
    {
        UserService = userService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        IHeaderDictionary headers = context.HttpContext.Request.Headers;

        string address = headers["public_address"];
        string rightId = headers["right_id"];

        if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(rightId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!await UserService.isShareholder(address, rightId))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}