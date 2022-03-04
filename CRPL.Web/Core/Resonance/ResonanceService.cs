using AutoMapper;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CRPL.Web.Core;

public class ResonanceService : IResonanceService
{
    private readonly ILogger<ResonanceService> Logger;
    private readonly IMapper Mapper;
    private readonly IServiceProvider ServiceProvider;

    private Dictionary<Guid, List<string>> WorkToConnection;
    private Dictionary<Guid, List<string>> ApplicationToConnection;

    public ResonanceService(ILogger<ResonanceService> logger, IMapper mapper, IServiceProvider serviceProvider)
    {
        Logger = logger;
        Mapper = mapper;
        ServiceProvider = serviceProvider;
        WorkToConnection = new Dictionary<Guid, List<string>>();
        ApplicationToConnection = new Dictionary<Guid, List<string>>();
    }

    public Task PushWorkUpdates(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task PushWorkUpdates(RegisteredWork work)
    {
        throw new NotImplementedException();
    }

    public Task PushApplicationUpdates(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task PushApplicationUpdates(Application application)
    {
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IHubContext<ApplicationsHub, IApplicationsHub>>();

        if (!ApplicationToConnection.ContainsKey(application.Id))
        {
            Logger.LogInformation("Application was not in the listen table");
            ApplicationToConnection.Add(application.Id, new List<string>());
        } 
        await context.Clients.Clients(ApplicationToConnection[application.Id]).PushApplication(Mapper.Map<ApplicationViewModel>(application));
    }

    public void ListenToWork(Guid workId, string connectionId)
    {
        Logger.LogInformation("{Connection} is now listening to work {Id}", connectionId, workId);
        WorkToConnection[workId].Add(connectionId);
    }

    public void ListenToApplication(Guid applicationId, string connectionId)
    {
        if (!ApplicationToConnection.ContainsKey(applicationId))
        {
            Logger.LogInformation("Application was not in the listen table");
            ApplicationToConnection.Add(applicationId, new List<string> { connectionId });
        }
        else
        {
            ApplicationToConnection[applicationId].Add(connectionId);   
        }
        
        Logger.LogInformation("{Connection} is now listening to application {Id}", connectionId, applicationId);
    }
}