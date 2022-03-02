namespace CRPL.Web.Core;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseUsageProxy(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UsageProxyMiddleware>();
    }
}