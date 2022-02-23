using AutoMapper;
using CRPL.Data.Account;
using CRPL.Data.Account.ViewModels;
using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
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

        CreateMap<RegisteredWork, RegisteredWorkViewModel>()
            .ForMember(model => model.RegisteredTransactionUri, x => x.MapFrom(src => "https://etherscan.io/tx/" + src.RegisteredTransactionId));
        CreateMap<RegisteredWork, RegisteredWorkWithAppsViewModel>()
            .ForMember(model => model.AssociatedUsers, x =>
                x.MapFrom(src => src.UserWorks.Select(u => u.UserAccount)))
            .ForMember(model => model.OwnershipStructure, x =>
                x.Ignore())
            .ForMember(src => src.CurrentVotes, x =>
                x.Ignore())
            .ForMember(src => src.HasProposal, x =>
                x.Ignore())
            .ForMember(src => src.Meta, x =>
                x.Ignore())
            .ForMember(model => model.RegisteredTransactionUri, x => x.MapFrom(src => "https://etherscan.io/tx/" + src.RegisteredTransactionId));
        

        CreateMap<Application, ApplicationViewModelWithoutAssociated>().IncludeAllDerived()
            .ForMember(model => model.TransactionUri, x => x.MapFrom(src => "https://etherscan.io/tx/" + src.TransactionId));

        CreateMap<CopyrightRegistrationApplication, CopyrightRegistrationViewModelWithoutAssociated>();
        CreateMap<OwnershipRestructureApplication, OwnershipRestructureViewModelWithoutAssociated>();
        CreateMap<DisputeApplication, DisputeViewModelWithoutAssociated>();

        CreateMap<Application, ApplicationViewModel>()
            .ForMember(model => model.AssociatedWork, x => x.MapFrom(src => src.AssociatedWork))
            .ForMember(model => model.AssociatedUsers, x => x.MapFrom(src => src.AssociatedUsers.Select(u => u.UserAccount)))
            .ForMember(model => model.TransactionUri, x => x.MapFrom(src => "https://etherscan.io/tx/" + src.TransactionId));

        CreateMap<CopyrightRegistrationApplication, CopyrightRegistrationViewModel>()
            .ForMember(model => model.OwnershipStakes, x =>
                x.MapFrom(src => src.OwnershipStakes.Decode()))
            .ForMember(model => model.AssociatedUsers, x => x.MapFrom(src => src.AssociatedUsers.Select(u => u.UserAccount)))
            .ForMember(model => model.TransactionUri, x => x.MapFrom(src => "https://etherscan.io/tx/" + src.TransactionId));

        CreateMap<OwnershipRestructureApplication, OwnershipRestructureViewModel>()
            .ForMember(model => model.CurrentStructure, x => x.MapFrom(src => src.CurrentStructure.Decode()))
            .ForMember(model => model.ProposedStructure, x => x.MapFrom(src => src.ProposedStructure.Decode()))
            .ForMember(model => model.AssociatedUsers, x => x.MapFrom(src => src.AssociatedUsers.Select(u => u.UserAccount)))
            .ForMember(model => model.TransactionUri, x => x.MapFrom(src => "https://etherscan.io/tx/" + src.TransactionId));

        CreateMap<DisputeApplication, DisputeViewModel>()
            .ForMember(model => model.AssociatedUsers, x => x.MapFrom(src => src.AssociatedUsers.Select(u => u.UserAccount)))
            .ForMember(model => model.TransactionUri, x => x.MapFrom(src => "https://etherscan.io/tx/" + src.TransactionId));;
        
        CreateMap<CRPL.Data.StructuredOwnership.OwnershipStake, CRPL.Contracts.Structs.OwnershipStakeContract>();
    }
}