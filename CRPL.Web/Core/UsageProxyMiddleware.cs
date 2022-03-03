using CRPL.Data.Account;
using CRPL.Data.Account.Works;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Background.Usage;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.Core;

public class UsageProxyMiddleware
{
    private readonly RequestDelegate Next;
    private readonly IServiceProvider ServiceProvider;

    public UsageProxyMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        Next = next;
        ServiceProvider = serviceProvider;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/proxy/cpy"))
        {
            using var scope = ServiceProvider.CreateScope();
            var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            if (context.Request.Path.Value == null) throw new Exception("No Id!");

            // getting work id from the url
            var id = context.Request.Path.Value.Split('/')[3];
            if (string.IsNullOrEmpty(id)) throw new Exception("Id not found!");
            
            // getting and checking work exists
            var work = await applicationContext.RegisteredWorks.FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));
            if (work == null) throw new WorkNotFoundException(Guid.Parse(id));
            if (work.Cid == null) throw new Exception("Cid doesn't exist!");
            
            var target = new UriBuilder("https://ipfs.io/ipfs/" + work.Cid).Uri;
            
            // making request to proxied uri using ipfs gateway
            using var client = new HttpClient();
            var responseMessage = await client.GetAsync(target, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
            
            // Building response and request headers based on the response from the proxied resource
            context.Response.StatusCode = (int)responseMessage.StatusCode;
            CopyFromTargetResponseHeaders(context, responseMessage, work.Cid);
            ScrubRequestHeaders(context, work.Cid);

            // registering the usage by adding to the queue
            var usageQueue = scope.ServiceProvider.GetRequiredService<IUsageQueue>();
            usageQueue.QueueUsage(new WorkUsage
            {
                TimeStamp = DateTime.Now,
                UsageType = UsageType.Proxy,
                WorkId = work.Id
            });
            
            await responseMessage.Content.CopyToAsync(context.Response.Body);
            return;
        }

        await Next(context);
    }

    private void ScrubRequestHeaders(HttpContext context, string ignore)
    {
        foreach (var header in context.Request.Headers)
        {
            context.Request.Headers[header.Key] = header.Value.Where(x => !x.Contains(ignore)).ToArray();
        }
    }

    private void CopyFromTargetResponseHeaders(HttpContext context, HttpResponseMessage responseMessage, string ignore)
    {
        foreach (var header in responseMessage.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.Where(x => !x.Contains(ignore)).ToArray();
        }

        foreach (var header in responseMessage.Content.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.Where(x => !x.Contains(ignore)).ToArray();
        }

        context.Response.Headers.Remove("transfer-encoding");
    }
}