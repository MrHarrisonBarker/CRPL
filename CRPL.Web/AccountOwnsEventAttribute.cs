using Microsoft.AspNetCore.Mvc.Filters;

namespace CRPL.Web;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AccountOwnsEventAttribute : Attribute, IFilterFactory
{
    /// <inheritdoc />
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetService<AccountOwnsFilter>();
    }

    /// <inheritdoc />
    public bool IsReusable => false;
}