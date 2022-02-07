namespace CRPL.Web.Exceptions;

public class WorkNotFoundException : Exception
{
    public WorkNotFoundException(string address) : base($"work registration transaction {address} cannot be found!")
    {
    }
}