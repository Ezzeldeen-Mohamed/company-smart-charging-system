using AutoMapper;
using CompanySmartChargingSystem.Domain.Entities;

namespace CompanySmartChargingSystem.Application.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Contract, ContractDto>().ReverseMap();
            CreateMap<Meter, MeterDto>().ReverseMap();
            CreateMap<ChargeTransaction, ChargeTransactionDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
} 