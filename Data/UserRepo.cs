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
    public class Sec_UserRepo : ISec_UserRepo
    {
        private readonly SmartxContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public Sec_UserRepo(SmartxContext context,IOptions<AppSettings> appSettings,IMapper mapper)
        {
            _context = context;
            _appSettings=appSettings.Value;
            _mapper=mapper;
        }

        
           
            
        public void CreateUser(SecUser cust)
        {
            if(cust == null)
            {
                throw new ArgumentNullException(nameof(cust));
            }

            _context.SecUser.Add(cust);
        }

        public void DeleteUser(SecUser cust)
        {
            if(cust == null)
            {
                throw new ArgumentNullException(nameof(cust));
            }
            _context.SecUser.Remove(cust);
        }

        public IEnumerable<SecUser> GetAllUsers()
        {
            return _context.SecUser.ToList();
        }

        public SecUser GetUserById(int id)
        {
            return _context.SecUser.FirstOrDefault(p => p.NUserId == id);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateUser(SecUser cust)
        {
            //Nothing
        }

        //  public LoginResponseDto Authenticate(string companyname,string username, string password)
        // {
            
        //     if (string.IsNullOrEmpty(companyname) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        //         return null;

        //         var loginRes = _context.SP_LOGIN.FromSqlRaw<SP_LOGIN>("SP_LOGIN @p0,@p1,@p2,@p3",companyname,"",username,password)    
        //         .ToList()
        //         .FirstOrDefault();

                
        //         //If User Found
        //         var tokenHandler=new JwtSecurityTokenHandler(); 
        //         var key=Encoding.ASCII.GetBytes(_appSettings.Secret);
        //         var tokenDescriptor = new SecurityTokenDescriptor{
        //             Subject=new System.Security.Claims.ClaimsIdentity(new Claim[]{
        //                 new Claim(ClaimTypes.NameIdentifier,loginRes.N_UserID.ToString()),
        //                 new Claim(ClaimTypes.Name,loginRes.X_UserName),
        //                 new Claim(ClaimTypes.Role,loginRes.X_UserCategory),
        //                 new Claim(ClaimTypes.UserData,loginRes.X_CompanyName),
        //                 new Claim(ClaimTypes.Version,"V0.1"),
        //             }),
        //             Expires=DateTime.UtcNow.AddDays(2),
        //             SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
        //         };
        //         var token = tokenHandler.CreateToken(tokenDescriptor);
        //         loginRes.Token= tokenHandler.WriteToken(token);
        //         var MenuList =_context.VwUserMenus
        //         .Where(VwUserMenus => VwUserMenus.NUserCategoryId==loginRes.N_UserCategoryID && VwUserMenus.NCompanyId==loginRes.N_CompanyID)
        //         .ToList();
        //         var Menu = _mapper.Map<IEnumerable<MenuDto>>(MenuList);

                
        //         loginRes.MenuList = Menu;
        //         var m2=_mapper.Map<LoginResponseDto>(loginRes);
        //         return(m2);
        // }




  public LoginResponseDto Authenticate(string companyname,string username, string password)
        {
            
            if (string.IsNullOrEmpty(companyname) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

                var loginRes = _context.SP_LOGIN.FromSqlRaw<SP_LOGIN>("SP_LOGIN @p0,@p1,@p2,@p3",companyname,"",username,password)    
                .ToList()
                .FirstOrDefault();

                
                //If User Found
                var tokenHandler=new JwtSecurityTokenHandler(); 
                var key=Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor{
                    Subject=new System.Security.Claims.ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier,loginRes.N_UserID.ToString()),
                        new Claim(ClaimTypes.Name,loginRes.X_UserName),
                        new Claim(ClaimTypes.Role,loginRes.X_UserCategory),
                        new Claim(ClaimTypes.UserData,loginRes.X_CompanyName),
                        new Claim(ClaimTypes.Version,"V0.1"),
                    }),
                    Expires=DateTime.UtcNow.AddDays(2),
                    SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                loginRes.Token= tokenHandler.WriteToken(token);
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
                var LoginResponse=_mapper.Map<LoginResponseDto>(loginRes);
                return(LoginResponse);
        }

    
    }

    public interface ISec_UserRepo
    {
        bool SaveChanges();
        IEnumerable<SecUser> GetAllUsers();
        LoginResponseDto Authenticate(string companyname,string username, string password);
        SecUser GetUserById(int id);
        void CreateUser(SecUser cust);
        void UpdateUser(SecUser cust);
        void DeleteUser(SecUser cust);
    }
}