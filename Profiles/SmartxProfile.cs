using AutoMapper;
using SmartxAPI.Dtos;
using SmartxAPI.Dtos.SP;
using SmartxAPI.Dtos.Custom;
using SmartxAPI.Models;

namespace SmartxAPI.Profiles
{
    public class SmartxAPIProfile : Profile
    {
        public SmartxAPIProfile()
        {
            //Source -> Target
            CreateMap<AccCompany, Acc_CompanyReadDto>();
            
            CreateMap<CustomerCreateDto, InvCustomer>();
            CreateMap<CustomerUpdateDto, InvCustomer>();
            CreateMap<InvCustomer, CustomerUpdateDto>();

            CreateMap<SecUser, UserReadDto>();
            CreateMap<UserCreateDto, SecUser>();
            CreateMap<UserUpdateDto, SecUser>();
            CreateMap<SecUser, UserUpdateDto>();
            CreateMap<SP_LOGIN, Sec_AuthenticateDto>();
        }

    }
    
}