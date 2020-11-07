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
using SmartxAPI.GeneralFunctions;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;

namespace SmartxAPI.Data
{
    public class Sec_UserRepo : ISec_UserRepo
    {
        private readonly SmartxContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        private readonly ICommenServiceRepo _services;

        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Sec_UserRepo(SmartxContext context,IOptions<AppSettings> appSettings,IMapper mapper,ICommenServiceRepo service,IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            _context = context;
            _appSettings=appSettings.Value;
            _mapper=mapper;
            _services=service;

            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        
           
            
  public LoginResponseDto Authenticate(string companyname,string username, string password,string ipAddress,string AppType)
        {
            
            if (string.IsNullOrEmpty(companyname) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

                // var loginRes = _context.SP_LOGIN.FromSqlRaw<SP_LOGIN>("SP_LOGIN @p0,@p1,@p2,@p3",companyname,"",username,password)    
                // .ToList()
                // .FirstOrDefault();
            DataTable dt=new DataTable();
                            SortedList paramsList = new SortedList()
                    {
                        {"X_CompanyName",companyname},
                        {"X_FnYearDescr",""},
                        {"X_LoginName",username},
                        {"X_Pwd",password},
                        {"X_AppType",AppType}
                    };

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTablePro("SP_LOGIN",paramsList,connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return null;
                }
                else
                {
                    DataRow dr =dt.Rows[0];
                   return(_services.Authenticate(myFunctions.getIntVAL(dr["N_CompanyID"].ToString()),dr["X_CompanyName"].ToString(),dr["X_UserName"].ToString(),myFunctions.getIntVAL(dr["N_UserID"].ToString()),"All",AppType));
                }                   
        }
    }

    public interface ISec_UserRepo
    {

        LoginResponseDto Authenticate(string companyname,string username, string password ,string IpAddress,string AppType);
 
    }
}