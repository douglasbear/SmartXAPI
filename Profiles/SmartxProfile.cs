using AutoMapper;
using SmartxAPI.Dtos;
using SmartxAPI.Models;

namespace SmartxAPI.Profiles
{
    public class SmartxAPIProfile : Profile
    {
        public SmartxAPIProfile()
        {
            //Source -> Target
            CreateMap<InvCustomer, CustomerReadDto>();
            CreateMap<CustomerCreateDto, InvCustomer>();
            CreateMap<CustomerUpdateDto, InvCustomer>();
            CreateMap<InvCustomer, CustomerUpdateDto>();
        }

    }
    
}