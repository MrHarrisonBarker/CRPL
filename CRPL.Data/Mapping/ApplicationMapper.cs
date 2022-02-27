using AutoMapper;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.StructuredOwnership;

namespace CRPL.Data.Applications.ViewModels;

public static class ApplicationMapper
{
    public static ApplicationViewModel Map(this Application application, IMapper mapper)
    {
        switch (application.ApplicationType)
        {
            case ApplicationType.CopyrightRegistration:
                return mapper.Map<CopyrightRegistrationApplication, CopyrightRegistrationViewModel>((CopyrightRegistrationApplication)application);
            case ApplicationType.OwnershipRestructure:
                return mapper.Map<OwnershipRestructureApplication, OwnershipRestructureViewModel>((OwnershipRestructureApplication)application);
            case ApplicationType.Dispute:
                return mapper.Map<DisputeApplication, DisputeViewModel>((DisputeApplication)application);
            case ApplicationType.DeleteAccount:
                return mapper.Map<DeleteAccountApplication, DeleteAccountViewModel>((DeleteAccountApplication)application);
            case ApplicationType.WalletTransfer:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static List<OwnershipStake> Decode(this string src)
    {
        var stakes = src.Split(';').ToList();

        List<OwnershipStake> ownershipStakes = new List<OwnershipStake>();

        foreach (var stake in stakes)
        {
            if (stake.Length > 0)
            {
                ownershipStakes.Add(new OwnershipStake()
                {
                    Owner = stake.Split('!')[0],
                    Share = Convert.ToInt32(stake.Split('!')[1])
                });
            }
        }

        return ownershipStakes;
    }

    public static string Encode(this List<OwnershipStake> src)
    {
        var stakes = "";

        foreach (var stake in src)
        {
            stakes += $"{stake.Owner}!{stake.Share};";
        }

        return stakes;
    }
}