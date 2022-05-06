namespace CRPL.Web.Core;

// Used register the proxy middleware in the pipeline
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseUsageProxy(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UsageProxyMiddleware>();
    }
}