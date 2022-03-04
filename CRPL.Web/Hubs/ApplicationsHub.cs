using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Core;
using Microsoft.AspNetCore.SignalR;

namespace CRPL.Web.Hubs;

public interface IApplicationsHub
{
    public Task PushApplication(ApplicationViewModel applicationViewModel);
    public Task ListenToApplication(Guid applicationId);
}

public class ApplicationsHub : Hub<IApplicationsHub>
{
    private readonly ILogger<ApplicationsHub> Logger;
    private readonly IResonanceService ResonanceService;

    public ApplicationsHub(ILogger<ApplicationsHub> logger, IResonanceService resonanceService)
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