namespace CRPL.Web.Core.ChainSync.Synchronisers;

// Standard interface for interacting with a chain sync synchroniser
public interface ISynchroniser
{
    public Task SynchroniseBatch(int from, int take);
    public Task SynchroniseOne(Guid id);
}