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

    private readonly Dictionary<Guid, List<string>> WorkToConnection;
    private readonly Dictionary<Guid, List<string>> ApplicationToConnection;

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
        Logger.LogInformation("Pushing work updates");
        throw new NotImplementedException();
    }

    public async Task PushWorkUpdates(RegisteredWork work)
    {
        Logger.LogInformation("Pushing work updates");
        
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IHubContext<WorksHub, IWorksHub>>();

        if (!WorkToConnection.ContainsKey(work.Id))
        {
            Logger.LogInformation("Work was not in the listen table");
            WorkToConnection.Add(work.Id, new List<string>());
        } 
        await context.Clients.Clients(WorkToConnection[work.Id]).PushWork(Mapper.Map<RegisteredWorkViewModel>(work));
    }

    public Task PushApplicationUpdates(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task PushApplicationUpdates(Application application)
    {
        Logger.LogInformation("Pushing application updates");
        
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
        if (!WorkToConnection.ContainsKey(workId))
        {
            Logger.LogInformation("Work {Id} was not in the listen table", workId);
            WorkToConnection.Add(workId, new List<string> { connectionId });
        }
        else
        {
            WorkToConnection[workId].Add(connectionId);   
        }

        Logger.LogInformation("{Connection} is now listening to work {Id}", connectionId, workId);
    }

    public void ListenToApplication(Guid applicationId, string connectionId)
    {
        if (!ApplicationToConnection.ContainsKey(applicationId))
        {
            Logger.LogInformation("Application {Id} was not in the listen table", applicationId);
            ApplicationToConnection.Add(applicationId, new List<string> { connectionId });
        }
        else
        {
            ApplicationToConnection[applicationId].Add(connectionId);   
        }
        
        Logger.LogInformation("{Connection} is now listening to application {Id}", connectionId, applicationId);
    }

    public void RemoveConnection(string connectionId)
    {
        if (WorkToConnection.Values.Any(x => x.Contains(connectionId)))
        {
            var instances = WorkToConnection.Where(x => x.Value.Contains(connectionId)).ToList();
            instances.ForEach(x => WorkToConnection[x.Key].Remove(connectionId));
        }

        if (ApplicationToConnection.Values.Any(x => x.Contains(connectionId)))
        {
            var instances = ApplicationToConnection.Where(x => x.Value.Contains(connectionId)).ToList();
            instances.ForEach(x => ApplicationToConnection[x.Key].Remove(connectionId));
        }
    }
}