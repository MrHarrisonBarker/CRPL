namespace CRPL.Web.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException() : base($"The user was not found!")
    {
    }
    
    public UserNotFoundException(string address) : base($"The user: {address} was not found!")
    {
    }
    
    public UserNotFoundException(Guid id) : base($"The user: {id} was not found!")
    {
    }
}