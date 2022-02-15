namespace CRPL.Web.Exceptions;

public class WorkNotVerifiedException : Exception
{
    public WorkNotVerifiedException() : base("Work not verified! Cannot complete registration with unverified work")
    {
    }
}