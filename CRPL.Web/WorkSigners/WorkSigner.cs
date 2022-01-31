namespace CRPL.Web.WorkSigners;

public abstract class WorkSigner
{
    protected readonly string Signature;

    protected WorkSigner(string signature)
    {
        var now = DateTime.Now;
        Signature = $"CRPL COPYRIGHT SIGNATURE ({now.ToLongDateString()} at {now.ToLongTimeString()}) // " + signature;
    }
}