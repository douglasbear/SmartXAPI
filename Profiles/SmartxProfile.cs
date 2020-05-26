using AutoMapper;
using SmartxAPI.Dtos;
using SmartxAPI.Dtos.SP;
using SmartxAPI.Dtos.Login;
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

            CreateMap<VwUserMenus, MenuDto>();


            CreateMap<SP_LOGIN, UserDto>();
            CreateMap<SP_LOGIN, FnYearDto>();
            CreateMap<SP_LOGIN, CompanyDto>();
            CreateMap<SP_LOGIN, LoginResponseDto>()
            .ForMember(dest => dest.UserData, opt => opt.MapFrom(src =>src))
            .ForMember(dest => dest.FnYearData, opt => opt.MapFrom(src =>src))
            .ForMember(dest => dest.CompanyData, opt => opt.MapFrom(src =>src));



            
        }

    }
    
}