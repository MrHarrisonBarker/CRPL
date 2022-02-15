namespace CRPL.Web.Exceptions;

public class WorkNotRegisteredException : Exception
{
    public WorkNotRegisteredException() : base("Work not registered!")
    {
    }
}