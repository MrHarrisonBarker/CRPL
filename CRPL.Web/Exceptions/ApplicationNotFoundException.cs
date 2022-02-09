namespace CRPL.Web.Exceptions;

public class ApplicationNotFoundException : Exception
{
    public ApplicationNotFoundException() : base("application not found!")
    {
    }

    public ApplicationNotFoundException(Guid id) : base($"application '{id}' was not found")
    {
    }
}