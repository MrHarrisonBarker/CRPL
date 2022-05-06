using AutoMapper;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Hubs;
using CRPL.Web.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CRPL.Web.Core;

// A service for handling all websocket updates and subscriptions
public class ResonanceService : IResonanceService
{
    private readonly ILogger<ResonanceService> Logger;
    private readonly IMapper Mapper;
    private readonly IServiceProvider ServiceProvider;

    // Subscription hash tables
    public readonly Dictionary<Guid, List<string>> WorkToConnection;
    public readonly Dictionary<Guid, List<string>> ApplicationToConnection;
    public readonly Dictionary<Guid, List<string>> UserToConnection;

    public ResonanceService(ILogger<ResonanceService> logger, IMapper mapper, IServiceProvider serviceProvider)
    {
        Logger = logger;
        Mapper = mapper;
        ServiceProvider = serviceProvider;
        WorkToConnection = new Dictionary<Guid, List<string>>();
        ApplicationToConnection = new Dictionary<Guid, List<string>>();
        UserToConnection = new Dictionary<Guid, List<string>>();
    }

    public Task PushWorkUpdates(Guid id)
    {
        Logger.LogInformation("Pushing work updates");
        return Task.CompletedTask;
    }

    // Pushes the registered work to all connections subscribed to the specific work
    public async Task PushWorkUpdates(RegisteredWork work)
    {
        Logger.LogInformation("Pushing work updates");

        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IHubContext<ResonanceHub, IResonanceHub>>();

        // If the work is not already in the dictionary add it
        if (!WorkToConnection.ContainsKey(work.Id))
        {
            Logger.LogInformation("Work was not in the listen table");
            WorkToConnection.Add(work.Id, new List<string>());
        }

        makeUserListenToWork(work.UserWorks);

        var queryService = scope.ServiceProvider.GetRequiredService<IQueryService>();
        var freshWork = await queryService.GetWork(work.Id);
        
        // Send work to all connections subscribed
        await context.Clients.Clients(WorkToConnection[work.Id]).PushWork(freshWork);
    }

    // Subscribes users to works based on database relationships
    private void makeUserListenToWork(List<UserWork>? userWorks)
    {
        if (userWorks != null)
            foreach (var userWork in userWorks.Where(userWork => UserToConnection.ContainsKey(userWork.UserId)))
            {
                // go though all the users connections and listening to work
                UserToConnection[userWork.UserId].ForEach(connection =>
                {
                    // if the connection is not already listening to this work
                    if (!WorkToConnection[userWork.WorkId].Contains(connection)) ListenToWork(userWork.WorkId, connection);
                });
            }
    }

    public Task PushApplicationUpdates(Guid id)
    {
        return Task.CompletedTask;
    }

    // Pushes the application to all connections subscribed to the specific application
    public async Task PushApplicationUpdates(Application application)
    {
        Logger.LogInformation("Pushing application updates");

        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IHubContext<ResonanceHub, IResonanceHub>>();

        // If the application doesn't exist in the dictionary add it
        if (!ApplicationToConnection.ContainsKey(application.Id))
        {
            Logger.LogInformation("Application was not in the listen table");
            ApplicationToConnection.Add(application.Id, new List<string>());
        }

        makeUserListenToApplication(application.AssociatedUsers);

        var formsService = scope.ServiceProvider.GetRequiredService<IFormsService>();
        var freshApplication = await formsService.GetApplication(application.Id);

        // Send application to all connections subscribed
        await context.Clients.Clients(ApplicationToConnection[application.Id]).PushApplication(Mapper.Map<ApplicationViewModel>(freshApplication));
    }

    // Subscribes users to applications based on database relationships
    private void makeUserListenToApplication(List<UserApplication>? userApplications)
    {
        if (userApplications != null)
            foreach (var userApplication in userApplications.Where(userApplication => UserToConnection.ContainsKey(userApplication.UserId)))
            {
                // go though all the users connections and listening to application
                UserToConnection[userApplication.UserId].ForEach(connection =>
                {
                    // if the connection is not already listening to this application
                    if (!ApplicationToConnection[userApplication.ApplicationId].Contains(connection)) ListenToApplication(userApplication.ApplicationId, connection);
                });
            }
    }

    // Subscribe a connection to a work
    public void ListenToWork(Guid workId, string connectionId)
    {
        // If the work is not in the dictionary add it
        if (!WorkToConnection.ContainsKey(workId))
        {
            Logger.LogInformation("Work {Id} was not in the listen table", workId);
            WorkToConnection.Add(workId, new List<string> { connectionId });
        }
        else
        {
            // Add connection to list of connection ids
            WorkToConnection[workId] = WorkToConnection[workId].Concat(new List<string> { connectionId }).Distinct().ToList();
        }

        Logger.LogInformation("{Connection} is now listening to work {Id}", connectionId, workId);
    }

    // Subscribe a connection to a application
    public void ListenToApplication(Guid applicationId, string connectionId)
    {
        // If the application is not in the dictionary add it
        if (!ApplicationToConnection.ContainsKey(applicationId))
        {
            Logger.LogInformation("Application {Id} was not in the listen table", applicationId);
            ApplicationToConnection.Add(applicationId, new List<string> { connectionId });
        }
        else
        {
            // Add connection to list of connection ids
            ApplicationToConnection[applicationId] = ApplicationToConnection[applicationId].Concat(new List<string> { connectionId }).Distinct().ToList();
        }

        Logger.LogInformation("{Connection} is now listening to application {Id}", connectionId, applicationId);
    }

    // Used to self identify connections as a user
    public void RegisterUser(Guid userId, string connectionId)
    {
        if (!UserToConnection.ContainsKey(userId))
        {
            Logger.LogInformation("User {Id} was not in the registered table", userId);
            UserToConnection.Add(userId, new List<string> { connectionId });
        }
        else
        {
            UserToConnection[userId].Add(connectionId);
        }

        Logger.LogInformation("{Connection} is now register to {Id}", connectionId, userId);
    }

    // Removes connection id from all dictionaries
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

        if (UserToConnection.Values.Any(x => x.Contains(connectionId)))
        {
            var instances = UserToConnection.Where(x => x.Value.Contains(connectionId)).ToList();
            instances.ForEach(x => UserToConnection[x.Key].Remove(connectionId));
        }
    }
}