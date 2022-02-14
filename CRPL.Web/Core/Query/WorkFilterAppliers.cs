using CRPL.Data;
using CRPL.Data.Account;

namespace CRPL.Web.Core.Query;

public static class WorkFilterApplies
{
    public static IQueryable<RegisteredWork> Apply(this IQueryable<RegisteredWork> registeredWorks, WorkFilter filter, string data)
    {
        switch (filter)
        {
            case WorkFilter.RegisteredAfter:
                return RegisteredAfter(registeredWorks, DateTime.Parse(data));
            case WorkFilter.RegisteredBefore:
                return RegisteredBefore(registeredWorks, DateTime.Parse(data));
            default:
                throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
        }
    }

    private static IQueryable<RegisteredWork> RegisteredAfter(this IQueryable<RegisteredWork> registeredWorks, DateTime after)
    {
        return registeredWorks.Where(x => x.Registered > after);
    }

    private static IQueryable<RegisteredWork> RegisteredBefore(this IQueryable<RegisteredWork> registeredWorks, DateTime before)
    {
        return registeredWorks.Where(x => x.Registered < before);
    }
}