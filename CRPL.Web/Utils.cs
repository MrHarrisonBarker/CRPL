using Microsoft.IdentityModel.Tokens;

namespace CRPL.Web;

public static class Utils
{
    public static bool LifetimeValidator(DateTime? notBefore,
        DateTime? expires,
        SecurityToken securityToken,
        TokenValidationParameters validationParameters)
    {
        return expires != null && expires > DateTime.Now;
    }
}