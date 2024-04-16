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
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.AccountNavigation.Dispositions.FirstOrDefault().CustomerId))
                .ForMember(dest => dest.DateOfTransaction, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Operation, opt => opt.MapFrom(src => src.Operation))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance));
        }
    }
}
