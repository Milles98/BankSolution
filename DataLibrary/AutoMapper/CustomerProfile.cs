using AutoMapper;
using DataLibrary.Data;
using DataLibrary.ViewModels;

namespace DataLibrary.AutoMapper;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, CustomerAccountViewModel>()
            .ForMember(dest => dest.Accounts, opt => opt.MapFrom(src => src.Dispositions.Select(d => new AccountViewModel
            {
                AccountId = d.Account.AccountId.ToString(),
                CustomerId = src.CustomerId,
                Frequency = d.Account.Frequency,
                Created = d.Account.Created.ToString("yyyy-MM-dd"),
                Balance = d.Account.Balance,
                Type = d.Type
            })))
            .ForMember(dest => dest.TotalBalance, opt => opt.MapFrom(src => src.Dispositions.Sum(d => d.Account.Balance)));


        CreateMap<Customer, CustomerViewModel>()
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.Dispositions.Select(d => d.AccountId).FirstOrDefault()))
            .ForMember(dest => dest.Accounts, opt => opt.MapFrom(src => src.Dispositions.Select(d => d.Account).ToList()));
    }
}