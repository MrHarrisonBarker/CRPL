using AutoMapper;
using CRPL.Data.Account;
using CRPL.Data.Account.ViewModels;

namespace CRPL.Data;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        CreateMap<UserAccount, UserAccountViewModel>()
            .ForMember(model => model.WalletPublicAddress, x => 
                x.MapFrom(a => a.Wallet.PublicAddress));
    }
}