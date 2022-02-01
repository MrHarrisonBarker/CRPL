namespace CRPL.Web.Exceptions;

public class InvalidSignatureException : Exception
{
    public InvalidSignatureException(string signature, string address) : base($"Invalid signature! {signature} did not match the address {address}")
    {
    }
}