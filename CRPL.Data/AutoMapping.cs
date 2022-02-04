using AutoMapper;
using CRPL.Data.Account;
using CRPL.Data.Account.ViewModels;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;

namespace CRPL.Data;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        CreateMap<UserAccount, UserAccountViewModel>()
            .ForMember(model => model.WalletPublicAddress, x =>
                x.MapFrom(a => a.Wallet.PublicAddress));

        CreateMap<Application, ApplicationViewModel>();
        CreateMap<CopyrightRegistrationApplication, CopyrightRegistrationViewModel>()
            .ForMember(model => model.OwnershipStakes, x =>
                x.MapFrom(src => src.OwnershipStakes.Decode()));
        CreateMap<OwnershipRestructureApplication, OwnershipRestructureViewModel>()
            .ForMember(model => model.CurrentStructure, x => x.MapFrom(src => src.CurrentStructure.Decode()))
            .ForMember(model => model.ProposedStructure, x => x.MapFrom(src => src.ProposedStructure.Decode()));
    }
}