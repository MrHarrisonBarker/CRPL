namespace CRPL.Web.Exceptions;

public class WorkExpiredException : Exception
{
    public WorkExpiredException(Guid id) : base($"{id} has expired!")
    {
    }
    
    public WorkExpiredException(string id) : base($"{id} has expired!")
    {
    }
}