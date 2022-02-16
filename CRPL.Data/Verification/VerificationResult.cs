using Microsoft.EntityFrameworkCore;

namespace CRPL.Data.Workds;

[Owned]
public class VerificationResult
{
    public bool IsAuthentic { get; set; }
    public Guid? Collision { get; set; }
}