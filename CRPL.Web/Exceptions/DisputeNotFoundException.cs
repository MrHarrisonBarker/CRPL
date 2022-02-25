namespace CRPL.Web.Exceptions;

public class DisputeNotFoundException : Exception
{
    public DisputeNotFoundException(Guid id) : base($"Dispute {id} not found!")
    {
        
    }
}