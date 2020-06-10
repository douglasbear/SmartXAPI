using AutoMapper;
using SmartxAPI.Dtos;
using SmartxAPI.Dtos.SP;
using SmartxAPI.Dtos.Login;
using SmartxAPI.Models;
using System.Data;

namespace SmartxAPI.Profiles
{
    public class SmartxAPIProfile : Profile
    {
        public SmartxAPIProfile()
        {
            //Source -> Target
            CreateMap<VwUserMenus, MenuDto>();
            CreateMap<MenuDto, ChildMenuDto>();
            CreateMap<ChildMenuDto, MenuDto>();

            CreateMap<SP_LOGIN, UserDto>();
            CreateMap<SP_LOGIN, FnYearDto>();
            CreateMap<SP_LOGIN, CompanyDto>();
            CreateMap<SP_LOGIN, TokenDto>();
            CreateMap<SP_LOGIN, LoginResponseDto>()
            .ForMember(dest => dest.UserData, opt => opt.MapFrom(src =>src))
            .ForMember(dest => dest.FnYearData, opt => opt.MapFrom(src =>src))
            .ForMember(dest => dest.CompanyData, opt => opt.MapFrom(src =>src))
            .ForMember(dest => dest.Token, opt => opt.MapFrom(src =>src)); 
        
        }

    }
    
}