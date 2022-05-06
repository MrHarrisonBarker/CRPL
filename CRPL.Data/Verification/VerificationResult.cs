using Microsoft.EntityFrameworkCore;

namespace CRPL.Data.Workds;

[Owned]
public class VerificationResult
{
    public bool IsAuthentic { get; set; }
    
    // Link to the existing work found causing verification failure 
    public Guid? Collision { get; set; }
}