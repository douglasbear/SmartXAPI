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

namespace SmartxAPI.Data
{
    public class Sec_UserRepo : ISec_UserRepo
    {
        private readonly SmartxContext _context;
        private readonly AppSettings _appSettings;

        public Sec_UserRepo(SmartxContext context,IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings=appSettings.Value;
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

         public SP_LOGIN Authenticate(string companyname,string username, string password)
        {
            
            if (string.IsNullOrEmpty(companyname) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

                var user = _context.SP_LOGIN.FromSqlRaw("SP_LOGIN @p0,@p1,@p2,@p3",companyname,"",username,password)    
                .ToList()
                .FirstOrDefault();

                
                //If User Found
                var tokenHandler=new JwtSecurityTokenHandler(); 
                var key=Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor{
                    Subject=new System.Security.Claims.ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.Name,user.X_UserName),
                        new Claim(ClaimTypes.Role,user.X_UserCategory),
                        new Claim(ClaimTypes.UserData,user.X_CompanyName),
                        new Claim(ClaimTypes.Version,"V0.1"),
                    }),
                    Expires=DateTime.UtcNow.AddDays(2),
                    SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.Token = tokenHandler.WriteToken(token);

                /* To Read Menu From DataBase */
                user.menu =_context.VwUserMenus
                .Where(VwUserMenus => VwUserMenus.NUserCategoryId==user.N_UserCategoryID && VwUserMenus.NCompanyId==user.N_CompanyID)
                .ToList();

                /**/

                return user;
                
        }

    
    }

    public interface ISec_UserRepo
    {
        bool SaveChanges();
        IEnumerable<SecUser> GetAllUsers();
        SP_LOGIN Authenticate(string companyname,string username, string password);
        SecUser GetUserById(int id);
        void CreateUser(SecUser cust);
        void UpdateUser(SecUser cust);
        void DeleteUser(SecUser cust);
    }
}