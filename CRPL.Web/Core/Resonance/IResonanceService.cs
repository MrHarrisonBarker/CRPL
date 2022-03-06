using CRPL.Data.Account;
using CRPL.Data.Applications;

namespace CRPL.Web.Core;

public interface IResonanceService
{
    public Task PushWorkUpdates(Guid id);
    public Task PushWorkUpdates(RegisteredWork work);
    public Task PushApplicationUpdates(Guid id);
    public Task PushApplicationUpdates(Application application);

    public void ListenToWork(Guid workId, string connectionId);
    public void ListenToApplication(Guid applicationId, string connectionId);
    public void RegisterUser(Guid userId, string connectionId);
    public void RemoveConnection(string connectionId);
}