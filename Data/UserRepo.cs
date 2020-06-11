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
using System.Security.Cryptography;

namespace SmartxAPI.Data
{
    public class Sec_UserRepo : ISec_UserRepo
    {
        private readonly SmartxContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        private readonly ICommenServiceRepo _services;

        public Sec_UserRepo(SmartxContext context,IOptions<AppSettings> appSettings,IMapper mapper,ICommenServiceRepo service)
        {
            _context = context;
            _appSettings=appSettings.Value;
            _mapper=mapper;
            _services=service;
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

  public LoginResponseDto Authenticate(string companyname,string username, string password,string ipAddress)
        {
            
            if (string.IsNullOrEmpty(companyname) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

                var loginRes = _context.SP_LOGIN.FromSqlRaw<SP_LOGIN>("SP_LOGIN @p0,@p1,@p2,@p3",companyname,"",username,password)    
                .ToList()
                .FirstOrDefault();
                
               return(_services.Authenticate(loginRes.N_CompanyID,loginRes.X_CompanyName,username,loginRes.N_UserID,"All"));
                
        }
    }

    public interface ISec_UserRepo
    {
        bool SaveChanges();
        IEnumerable<SecUser> GetAllUsers();
        LoginResponseDto Authenticate(string companyname,string username, string password ,string IpAddress);
        SecUser GetUserById(int id);
        void CreateUser(SecUser cust);
        void UpdateUser(SecUser cust);
        void DeleteUser(SecUser cust);
    }
}