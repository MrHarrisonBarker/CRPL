using System.Text;
using CRPL.Data.Works;

namespace CRPL.Web.WorkSigners;

public class UniversalSigner : WorkSigner, IWorkSigner
{
    public UniversalSigner(string signature) : base(signature)
    {
    }

    public CachedWork Sign(CachedWork work)
    {
        byte[] signature = Encoding.UTF8.GetBytes(Signature);

        return new CachedWork
        {
            Work = work.Work.Concat(signature).ToArray(),
            ContentType = work.ContentType
        };
    }
}