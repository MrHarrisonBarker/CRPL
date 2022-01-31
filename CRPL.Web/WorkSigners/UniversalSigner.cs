using System.Text;
using CRPL.Data.Works;

namespace CRPL.Web.WorkSigners;

public class UniversalSigner : IWorkSigner
{
    public CachedWork Sign(CachedWork work)
    {
        byte[] signature = Encoding.UTF8.GetBytes("COPYRIGHT REGISTERED BY CRPL");
        
        return new CachedWork
        {
            Work = work.Work.Concat(signature).ToArray(),
            ContentType = work.ContentType
        };
    }
}