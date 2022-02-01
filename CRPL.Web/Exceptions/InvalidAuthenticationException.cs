namespace CRPL.Web.Exceptions;

public class InvalidAuthenticationException : Exception
{
    public InvalidAuthenticationException(string token): base($"{token} is not a valid authentication token") {}
}