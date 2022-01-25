namespace CRPL.Web.Exceptions;

public class InvalidSignature : Exception
{
    public InvalidSignature(): base("Invalid signature") {}
}