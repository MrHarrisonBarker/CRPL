using CRPL.Data.Account;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Core;
using Microsoft.AspNetCore.SignalR;

namespace CRPL.Web.Hubs;

public interface IResonanceHub
{
    public Task PushApplication(ApplicationViewModel applicationViewModel);
    public Task PushWork(RegisteredWorkWithAppsViewModel workViewModel);
    public Task ListenToApplication(Guid applicationId);
    public Task ListenToWork(Guid workId);
    public Task RegisterUser(Guid userId);

}

public class ResonanceHub : Hub<IResonanceHub>
{
    private readonly ILogger<ResonanceHub> Logger;
    private readonly IResonanceService ResonanceService;

    public ResonanceHub(ILogger<ResonanceHub> logger, IResonanceService resonanceService)
    {
        Logger = logger;
        ResonanceService = resonanceService;
    }

    public Task PushApplication(ApplicationViewModel applicationViewModel)
    {
        Logger.LogInformation("pushing application {Id} to {Connection}", applicationViewModel.Id, Context.ConnectionId);
        throw new NotImplementedException();
    }

    public Task ListenToApplication(Guid applicationId)
    {
        Logger.LogInformation("ListenToApplication Called with \"{Id}\"", applicationId);
        ResonanceService.ListenToApplication(applicationId, Context.ConnectionId);
        return Task.CompletedTask;
    }
    
    public Task ListenToWork(Guid workId)
    {
        Logger.LogInformation("ListenToWork Called with \"{Id}\"", workId);
        ResonanceService.ListenToWork(workId, Context.ConnectionId);
        return Task.CompletedTask;
    }

    public Task RegisterUser(Guid userId)
    {
        Logger.LogInformation("RegisterUser Called with \"{Id}\"", userId);
        ResonanceService.RegisterUser(userId, Context.ConnectionId);
        return Task.CompletedTask;
    }

    public Task PushWork(RegisteredWorkWithAppsViewModel workViewModel)
    {
        Logger.LogInformation("pushing work {Id} to {Connection}", workViewModel.Id, Context.ConnectionId);
        throw new NotImplementedException();
    }

    public override Task OnConnectedAsync()
    {
        Logger.LogInformation("A new client has connected {connection}", Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Logger.LogInformation("A client has disconnected {connection}", Context.ConnectionId);
        ResonanceService.RemoveConnection(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}