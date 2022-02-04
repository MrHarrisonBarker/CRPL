using AutoMapper;
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
                break;
            case ApplicationType.CopyrightTypeChange:
                break;
            case ApplicationType.Dispute:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        throw new Exception();
    }
    
    public static List<OwnershipStake> Decode(this string src)
    {
        var stakes = src.Split(';').ToList();

        List<OwnershipStake> ownershipStakes = new List<OwnershipStake>();

        foreach (var stake in stakes)
        {
            ownershipStakes.Add(new OwnershipStake()
            {
                Owner = stake.Split('!')[0],
                Share = Convert.ToInt32(stake.Split('!')[1])
            });
        }

        return ownershipStakes;
    }
}