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

        CreateMap<UserAccount, UserAccountMinimalViewModel>();

        CreateMap<RegisteredWork, RegisteredWorkViewModel>();
        CreateMap<RegisteredWork, RegisteredWorkWithAppsViewModel>().ForMember(model => model.AssociatedUsers, x => 
            x.MapFrom(src => src.UserWorks.Select(u => u.UserAccount)));

        CreateMap<Application, ApplicationViewModelWithoutAssociated>().IncludeAllDerived();
        CreateMap<CopyrightRegistrationApplication, CopyrightRegistrationViewModelWithoutAssociated>();


        CreateMap<Application, ApplicationViewModel>()
            .ForMember(model => model.AssociatedWork, x => x.MapFrom(src => src.AssociatedWork))
            .ForMember(model => model.AssociatedUsers, x => x.MapFrom(src => src.AssociatedUsers.Select(u => u.UserAccount)));

        CreateMap<CopyrightRegistrationApplication, CopyrightRegistrationViewModel>()
            .ForMember(model => model.OwnershipStakes, x =>
                x.MapFrom(src => src.OwnershipStakes.Decode()))
            .ForMember(model => model.AssociatedUsers, x => x.MapFrom(src => src.AssociatedUsers.Select(u => u.UserAccount)));
        
        CreateMap<OwnershipRestructureApplication, OwnershipRestructureViewModel>()
            .ForMember(model => model.CurrentStructure, x => x.MapFrom(src => src.CurrentStructure.Decode()))
            .ForMember(model => model.ProposedStructure, x => x.MapFrom(src => src.ProposedStructure.Decode()))
            .ForMember(model => model.AssociatedUsers, x => x.MapFrom(src => src.AssociatedUsers.Select(u => u.UserAccount)));

        CreateMap<CRPL.Data.StructuredOwnership.OwnershipStake, CRPL.Contracts.Standard.ContractDefinition.OwnershipStake>();
    }
}