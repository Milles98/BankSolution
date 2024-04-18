using AutoMapper;
using DataLibrary.Data;
using DataLibrary.ViewModels;

namespace DataLibrary.AutoMapper
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, TransactionViewModel>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.AccountNavigation.Dispositions.First().CustomerId))
                .ForMember(dest => dest.DateOfTransaction, opt => opt.MapFrom(src => src.Date));
        }
    }
}
