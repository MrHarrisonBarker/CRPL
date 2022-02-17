namespace CRPL.Web.WorkSigners;

public abstract class WorkSigner
{
    protected readonly string Signature;

    protected WorkSigner(string signature)
    {
        Signature = signature;
    }
}