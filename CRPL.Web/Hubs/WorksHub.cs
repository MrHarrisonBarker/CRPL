using CRPL.Data.Account;
using CRPL.Web.Core;
using Microsoft.AspNetCore.SignalR;

namespace CRPL.Web.Hubs;

public interface IWorksHub
{
    public Task ListenToWork(Guid workId);
    public Task PushWork(RegisteredWorkViewModel workViewModel);
}

public class WorksHub : Hub<IWorksHub>
{
    private readonly ILogger<WorksHub> Logger;
    private readonly IResonanceService ResonanceService;

    public WorksHub(ILogger<WorksHub> logger, IResonanceService resonanceService)
    {
        Logger = logger;
        ResonanceService = resonanceService;
    }
    
    public Task ListenToWork(Guid workId)
    {
        Logger.LogInformation("ListenToWork Called with \"{Id}\"", workId);
        ResonanceService.ListenToWork(workId, Context.ConnectionId);
        return Task.CompletedTask;
    }

    public Task PushWork(RegisteredWorkViewModel workViewModel)
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