using AutoMapper;
using DataLibrary.Data;
using DataLibrary.ViewModels;

namespace DataLibrary.AutoMapper
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountViewModel>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId.ToString()))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.Customers, opt => opt.MapFrom(src => src.Dispositions.Select(d => new CustomerDispositionViewModel
                {
                    Customer = d.Customer,
                    DispositionType = d.Type
                }).ToList()));
        }
    }
}
