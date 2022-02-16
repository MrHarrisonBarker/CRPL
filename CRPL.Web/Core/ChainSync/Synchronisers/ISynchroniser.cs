namespace CRPL.Web.Core.ChainSync.Synchronisers;

public interface ISynchroniser
{
    public Task SynchroniseBatch();
    public Task SynchroniseOne(Guid id);
}