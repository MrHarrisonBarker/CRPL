using CRPL.Data.Account;
using CRPL.Data.Applications;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CRPL.Web;

public static class Utils
{
    public static bool LifetimeValidator(DateTime? notBefore,
        DateTime? expires,
        SecurityToken securityToken,
        TokenValidationParameters validationParameters)
    {
        return expires != null && expires > DateTime.Now;
    }

    public static IQueryable<RegisteredWork> PruneApplications(this IQueryable<RegisteredWork> registeredWorks)
    {
        return registeredWorks.AsNoTracking().Select(x => new RegisteredWork()
        {
            Created = x.Created,
            Hash = x.Hash,
            Id = x.Id,
            Cid = x.Cid,
            Registered = x.Registered,
            Status = x.Status,
            Title = x.Title,
            RightId = x.RightId,
            UserWorks = x.UserWorks,
            VerificationResult = x.VerificationResult,
            ProposalTransactionId = x.ProposalTransactionId,
            RegisteredTransactionId = x.RegisteredTransactionId,
            AssociatedApplication = x.AssociatedApplication.Where(a => a.ApplicationType != ApplicationType.Dispute || a.Status == ApplicationStatus.Complete || a.Status == ApplicationStatus.Submitted).ToList(),
            LastPing = x.LastPing,
            TimesPinged = x.TimesPinged,
            LastProxyUse = x.LastProxyUse,
            TimesProxyUsed = x.TimesProxyUsed,
            WorkType = x.WorkType
        });
    }
}