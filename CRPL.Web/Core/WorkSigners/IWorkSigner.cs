using CRPL.Data.Works;

namespace CRPL.Web.WorkSigners;

public interface IWorkSigner
{
    public CachedWork Sign(CachedWork work);
}