namespace CRPL.Web.Exceptions;

public class WorkNotFoundException : Exception
{
    public WorkNotFoundException() : base("Work not found!")
    {
    }

    public WorkNotFoundException(Guid id) : base($"work {id} cannot be found!")
    {
    }

    public WorkNotFoundException(string address) : base($"work registration transaction {address} cannot be found!")
    {
    }
}