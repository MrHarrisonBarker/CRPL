using CRPL.Data.Works;

namespace CRPL.Web.WorkSigners;

public class UniversalSigner : IWorkSigner
{
    public CachedWork Sign(CachedWork work)
    {
        byte[] signature = Guid.NewGuid().ToByteArray();
        
        return new CachedWork
        {
            Work = work.Work.Concat(signature).ToArray(),
            ContentType = work.ContentType
        };
    }
}