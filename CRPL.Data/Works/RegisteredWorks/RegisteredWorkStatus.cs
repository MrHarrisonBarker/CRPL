namespace CRPL.Data.Account;

public enum RegisteredWorkStatus
{
    Created,
    ProcessingVerification,
    Verified,
    SentToChain,
    Registered,
    Rejected
}