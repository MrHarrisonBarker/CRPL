namespace CRPL.Web.Core.ChainSync.Synchronisers;

public interface ISynchroniser
{
    public Task SynchroniseBatch(int from, int take);
    public Task SynchroniseOne(Guid id);
}