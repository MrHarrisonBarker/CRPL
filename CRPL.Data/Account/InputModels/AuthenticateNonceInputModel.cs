namespace CRPL.Data.Account.InputModels;

// what is sent back from the client, signature = signed nonce
public class AuthenticateSignatureInputModel
{
    public string Signature { get; set; }
}