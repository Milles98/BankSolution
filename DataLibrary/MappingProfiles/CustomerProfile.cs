using AutoMapper;
using DataLibrary.Data;
using DataLibrary.ViewModels;

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
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
            .ForMember(dest => dest.Streetaddress, opt => opt.MapFrom(src => src.Streetaddress))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.Zipcode, opt => opt.MapFrom(src => src.Zipcode))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
            .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.CountryCode))
            .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.Birthday))
            .ForMember(dest => dest.Telephonecountrycode, opt => opt.MapFrom(src => src.Telephonecountrycode))
            .ForMember(dest => dest.Telephonenumber, opt => opt.MapFrom(src => src.Telephonenumber))
            .ForMember(dest => dest.Emailaddress, opt => opt.MapFrom(src => src.Emailaddress))
            .ForMember(dest => dest.TotalBalance, opt => opt.MapFrom(src => src.Dispositions.Sum(d => d.Account.Balance)))
            .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.NationalId));


        CreateMap<Customer, CustomerViewModel>()
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.Dispositions.Select(d => d.AccountId).First()))
            .ForMember(dest => dest.Accounts, opt => opt.MapFrom(src => src.Dispositions.Select(d => d.Account).ToList()));
    }
}
