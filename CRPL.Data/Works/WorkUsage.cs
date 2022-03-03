namespace CRPL.Data.Account.Works;

public struct WorkUsage
{
    public DateTime TimeStamp { get; set; }
    public UsageType UsageType { get; set; }
    public Guid WorkId { get; set; }
}

public enum UsageType
{
    Proxy,
    Ping
}