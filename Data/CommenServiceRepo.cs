using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using SmartxAPI.Models;
using SmartxAPI.Dtos.SP;
using Microsoft.EntityFrameworkCore;
using SmartxAPI.Profiles;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Newtonsoft.Json;
using SmartxAPI.Dtos.Login;
using AutoMapper;

namespace SmartxAPI.Data
{
    public class CommenServiceRepo : ICommenServiceRepo
    {
        private readonly SmartxContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public CommenServiceRepo(SmartxContext context,IOptions<AppSettings> appSettings,IMapper mapper)
        {
            _context = context;
            _appSettings=appSettings.Value;
            _mapper=mapper;
        }


  public dynamic Authenticate(int companyid,string companyname,string username, int userid,string reqtype)
        {
            
            if (string.IsNullOrEmpty(companyid.ToString()) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userid.ToString()))
                return null;

                var password =_context.SecUser
                .Where(y => y.NCompanyId==companyid && y.XUserName==username && y.NUserId==userid )
                .Select(x => x.XPassword)
                .FirstOrDefault();
                
                var loginRes = _context.SP_LOGIN.FromSqlRaw<SP_LOGIN>("SP_LOGIN @p0,@p1,@p2,@p3",companyname,"",username,password)    
                .ToList()
                .FirstOrDefault();

switch (reqtype.ToLower())
      {
          case "all":
                    var tokenHandler=new JwtSecurityTokenHandler(); 
                var key=Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor{
                    Subject=new System.Security.Claims.ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier,loginRes.N_UserID.ToString()),
                        new Claim(ClaimTypes.Name,loginRes.X_UserName),
                        new Claim(ClaimTypes.Role,loginRes.X_UserCategory),
                        new Claim(ClaimTypes.GroupSid,loginRes.N_UserCategoryID.ToString()),
                        new Claim(ClaimTypes.StreetAddress,loginRes.X_CompanyName),
                        new Claim(ClaimTypes.Sid,loginRes.N_CompanyID.ToString()),
                        new Claim(ClaimTypes.Version,"V0.1"),
                    }),
                    Expires=DateTime.UtcNow.AddDays(2),
                    SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                loginRes.Token= tokenHandler.WriteToken(token);
                loginRes.Expiry = DateTime.UtcNow.AddDays(2);
                var MenuList =_context.VwUserMenus
                .Where(VwUserMenus => VwUserMenus.NUserCategoryId==loginRes.N_UserCategoryID && VwUserMenus.NCompanyId==loginRes.N_CompanyID)
                .ToList();
                var Menu = _mapper.Map<List<MenuDto>>(MenuList);
            
            List<MenuDto> PMList = new List<MenuDto>();
            foreach(var ParentMenu in Menu.Where(y=>y.NParentMenuId==0 )){
                List<ChildMenuDto> CMList = new List<ChildMenuDto>();
                foreach(var ChildMenu in Menu.Where(y=>y.NParentMenuId==ParentMenu.NMenuId )){
                    CMList.Add(_mapper.Map<ChildMenuDto>(ChildMenu));
                }
                ParentMenu.ChildMenu=CMList;
                PMList.Add(_mapper.Map<MenuDto>(ParentMenu));
            }
                loginRes.MenuList = PMList;
                var LoginResponseAll=_mapper.Map<LoginResponseDto>(loginRes);
                return(LoginResponseAll);
          case "user":
                    var LoginResponseUser=_mapper.Map<UserDto>(loginRes);
                    return(LoginResponseUser);
            case "fnyear":
                    var FnYear=_mapper.Map<FnYearDto>(loginRes);
                    return(FnYear);
            case "company":
                    var Company=_mapper.Map<CompanyDto>(loginRes);
                    return(Company);
            
          case "menu":
                       var RMenuList =_context.VwUserMenus
                .Where(VwUserMenus => VwUserMenus.NUserCategoryId==loginRes.N_UserCategoryID && VwUserMenus.NCompanyId==loginRes.N_CompanyID)
                .ToList();
                var RMenu = _mapper.Map<List<MenuDto>>(RMenuList);
            
            List<MenuDto> RPMList = new List<MenuDto>();
            foreach(var ParentMenu in RMenu.Where(y=>y.NParentMenuId==0 )){
                List<ChildMenuDto> CMList = new List<ChildMenuDto>();
                foreach(var ChildMenu in RMenu.Where(y=>y.NParentMenuId==ParentMenu.NMenuId )){
                    CMList.Add(_mapper.Map<ChildMenuDto>(ChildMenu));
                }
                ParentMenu.ChildMenu=CMList;
                RPMList.Add(_mapper.Map<MenuDto>(ParentMenu));
            }
            return (RPMList);
          case "token":
          var rtokenHandler=new JwtSecurityTokenHandler(); 
                var rkey=Encoding.ASCII.GetBytes(_appSettings.Secret);
                var rtokenDescriptor = new SecurityTokenDescriptor{
                    Subject=new System.Security.Claims.ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier,loginRes.N_UserID.ToString()),
                        new Claim(ClaimTypes.Name,loginRes.X_UserName),
                        new Claim(ClaimTypes.Role,loginRes.X_UserCategory),
                        new Claim(ClaimTypes.GroupSid,loginRes.N_UserCategoryID.ToString()),
                        new Claim(ClaimTypes.StreetAddress,loginRes.X_CompanyName),
                        new Claim(ClaimTypes.Sid,loginRes.N_CompanyID.ToString()),
                        new Claim(ClaimTypes.Version,"V0.1"),
                    }),
                    Expires=DateTime.UtcNow.AddDays(2),
                    SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(rkey),SecurityAlgorithms.HmacSha256Signature)
                };
                var rtoken = rtokenHandler.CreateToken(rtokenDescriptor);
                var Token= rtokenHandler.WriteToken(rtoken);
                loginRes.Expiry = DateTime.UtcNow.AddDays(2);
                var LoginResponseToken=_mapper.Map<TokenDto>(loginRes);
                return(LoginResponseToken);
          default:
              return(null);
      }
                
                //If User Found
                
        }

    
    }

    public interface ICommenServiceRepo
    {
        dynamic Authenticate(int companyid,string companyname,string username, int userid,string reqtype);
    }
}