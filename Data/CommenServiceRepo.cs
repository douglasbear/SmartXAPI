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
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;

namespace SmartxAPI.Data
{
    public class CommenServiceRepo : ICommenServiceRepo
    {
        private readonly SmartxContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        public CommenServiceRepo(SmartxContext context, IOptions<AppSettings> appSettings, IMapper mapper, IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        public dynamic Authenticate(int companyid, string companyname, string username, int userid, string reqtype)
        {

            if (string.IsNullOrEmpty(companyid.ToString()) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userid.ToString()))
                return null;

            var password = _context.SecUser
            .Where(y => y.NCompanyId == companyid && y.XUserId == username && y.NUserId == userid)
            .Select(x => x.XPassword)
            .FirstOrDefault();

            // var loginRes = _context.SP_LOGIN.FromSqlRaw<SP_LOGIN>("SP_LOGIN @p0,@p1,@p2,@p3", companyname, "", username, password)
            // .ToList()
            // .FirstOrDefault();



            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                    SortedList paramsList = new SortedList()
                    {
                        {"X_CompanyName",companyname},
                        {"X_FnYearDescr",""},
                        {"X_LoginName",username},
                        {"X_Pwd",password.ToString()}
                    };
                    DataTable loginDt = dLayer.ExecuteDataTablePro("SP_LOGIN",paramsList,connection);
            //var loginRes = new List<SP_LOGIN>();  
    var loginRes = (from DataRow dr in loginDt.Rows  
            select new SP_LOGIN()  
            {  
                N_UserID = Convert .ToInt32 (dr["N_UserID"]),  
                X_UserName = dr["X_UserName"].ToString(),  
                X_FnYearDescr = dr["X_FnYearDescr"].ToString(),  
                N_FnYearID = myFunctions.getIntVAL(dr["N_FnYearID"].ToString()) ,
                D_Start = Convert.ToDateTime(dr["D_Start"].ToString()),
                D_End = Convert.ToDateTime(dr["D_End"].ToString()),
                X_AcYearDescr = dr["X_AcYearDescr"].ToString(),
                N_AcYearID = myFunctions.getIntVAL(dr["N_AcYearID"].ToString()) ,
                D_AcStart = Convert.ToDateTime(dr["D_AcStart"].ToString()),
                D_AcEnd = Convert.ToDateTime(dr["D_AcEnd"].ToString()),
                X_CompanyName = dr["X_CompanyName"].ToString(),  
                X_CompanyName_Ar = dr["X_CompanyName_Ar"].ToString(),  
                X_CompanyCode = dr["X_CompanyCode"].ToString(),  
                N_CompanyID = myFunctions.getIntVAL(dr["N_CompanyID"].ToString()) ,
                X_UserCategory = dr["X_UserCategory"].ToString(),  
                N_UserCategoryID = myFunctions.getIntVAL(dr["N_UserCategoryID"].ToString()) ,
                X_Country = dr["X_Country"].ToString(),  
                N_CurrencyID = myFunctions.getIntVAL(dr["N_CurrencyID"].ToString()) ,
                D_LoginDate = Convert.ToDateTime(dr["D_LoginDate"].ToString()),
                X_Language = dr["X_Language"].ToString(),  
                N_LanguageID = myFunctions.getIntVAL(dr["N_LanguageID"].ToString()) ,
                N_BranchID = dr["N_BranchID"].ToString(),  
                X_BranchName = dr["X_BranchName"].ToString(),  
                X_LocationName = dr["X_LocationName"].ToString(),  
                N_LocationID = dr["N_LocationID"].ToString(),  
                I_Logo = (byte [])dr["I_Logo"],
                B_AllBranchesData = (bool)dr["B_AllBranchesData"],  
                N_TaxType = myFunctions.getIntVAL(dr["N_TaxType"].ToString()) ,
                X_UserFullName = dr["X_UserFullName"].ToString()
            }).ToList()
            .FirstOrDefault();  
            

                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID", loginRes.N_CompanyID);
                    Params.Add("@nUserID", loginRes.N_UserID);

                if (loginRes.N_BranchID == null || loginRes.N_BranchID == "")
                {
                    loginRes.X_BranchName = dLayer.ExecuteScalar("Select X_BranchName From Acc_BranchMaster Where N_CompanyID=@nCompanyID  and B_DefaultBranch=1",Params, connection).ToString();
                    loginRes.N_BranchID = dLayer.ExecuteScalar("Select N_BranchID From Acc_BranchMaster Where N_CompanyID=@nCompanyID  and B_DefaultBranch=1",Params, connection).ToString();
                    loginRes.B_AllBranchesData = Convert.ToBoolean(dLayer.ExecuteScalar("Select B_ShowAllData From Acc_BranchMaster Where N_CompanyID=@nCompanyID and B_DefaultBranch=1",Params, connection).ToString());
                }
                Params.Add("@nBranchID", loginRes.N_BranchID);
                loginRes.X_LocationName = dLayer.ExecuteScalar("Select X_LocationName From Inv_Location Where N_CompanyID=@nCompanyID  and N_TypeID=2 and B_IsDefault=1  and N_BranchID=@nBranchID",Params, connection).ToString();
                loginRes.N_LocationID = dLayer.ExecuteScalar("Select N_LocationID From Inv_Location Where N_CompanyID=@nCompanyID  and B_IsDefault=1 and N_BranchID=@nBranchID",Params, connection).ToString();

                loginRes.N_CurrencyID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_CurrencyID  from Acc_CurrencyMaster where N_CompanyID=@nCompanyID  and B_Default=1",Params, connection).ToString());
                
                DataTable EmplData= dLayer.ExecuteDataTable("SELECT Pay_Employee.N_EmpID, Pay_Employee.X_EmpCode, Pay_Employee.X_EmpName, Sec_User.N_UserID, Pay_Position.X_Position, Pay_Position.N_PositionID FROM Pay_Position RIGHT OUTER JOIN Pay_Employee ON Pay_Position.N_PositionID = Pay_Employee.N_PositionID AND Pay_Position.N_CompanyID = Pay_Employee.N_CompanyID RIGHT OUTER JOIN Sec_User ON Pay_Employee.N_UserID = Sec_User.N_UserID AND Pay_Employee.N_CompanyID = Sec_User.N_CompanyID AND Pay_Employee.N_EmpID = Sec_User.N_EmpID where Sec_User.N_CompanyID=@nCompanyID  and Sec_User.N_UserID=@nUserID",Params, connection);
                if(EmplData.Rows.Count>0){
                loginRes.N_EmpID = myFunctions.getIntVAL(EmplData.Rows[0]["N_EmpID"].ToString());
                loginRes.X_EmpCode = EmplData.Rows[0]["X_EmpCode"].ToString();
                loginRes.X_EmpName = EmplData.Rows[0]["X_EmpName"].ToString();
                loginRes.X_Position = EmplData.Rows[0]["X_Position"].ToString();
                loginRes.N_PositionID = myFunctions.getIntVAL(EmplData.Rows[0]["N_PositionID"].ToString());
                }
                loginRes.X_CurrencyName = dLayer.ExecuteScalar("select X_ShortName  from Acc_CurrencyMaster where N_CompanyID=@nCompanyID  and N_CurrencyID=@nCompanyID",Params, connection).ToString();


                switch (reqtype.ToLower())
                {
                    case "all":
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier,loginRes.N_UserID.ToString()),
                        new Claim(ClaimTypes.Name,loginRes.X_UserName.ToString()),
                        new Claim(ClaimTypes.Role,loginRes.X_UserCategory),
                        new Claim(ClaimTypes.GroupSid,loginRes.N_UserCategoryID.ToString()),
                        new Claim(ClaimTypes.StreetAddress,loginRes.X_CompanyName),
                        new Claim(ClaimTypes.Sid,loginRes.N_CompanyID.ToString()),
                        new Claim(ClaimTypes.Version,"V0.1"),
                    }),
                            Expires = DateTime.UtcNow.AddDays(2),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        };
                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        loginRes.Token = tokenHandler.WriteToken(token);
                        loginRes.Expiry = DateTime.UtcNow.AddDays(2);

                        loginRes.RefreshToken = generateRefreshToken();
                        var user = _context.SecUser.SingleOrDefault(u => u.NUserId == loginRes.N_UserID);
                        user.XToken = loginRes.RefreshToken;

                        object abc = dLayer.ExecuteScalar("Update Sec_User set X_Token='" + loginRes.RefreshToken + "' where N_UserID=" + loginRes.N_UserID + " and N_CompanyID=" + loginRes.N_CompanyID, connection);
                        if (loginRes.I_Logo != null)
                            loginRes.I_CompanyLogo = "data:image/png;base64," + Convert.ToBase64String(loginRes.I_Logo, 0, loginRes.I_Logo.Length);
                        var MenuList = _context.VwUserMenus
                        .Where(VwUserMenus => VwUserMenus.NUserCategoryId == loginRes.N_UserCategoryID && VwUserMenus.NCompanyId == loginRes.N_CompanyID && VwUserMenus.BShowOnline == true)
                        .ToList();
                        var Menu = _mapper.Map<List<MenuDto>>(MenuList);

                        List<MenuDto> PMList = new List<MenuDto>();
                        foreach (var ParentMenu in Menu.Where(y => y.NParentMenuId == 0))
                        {
                            List<ChildMenuDto> CMList = new List<ChildMenuDto>();
                            foreach (var ChildMenu in Menu.Where(y => y.NParentMenuId == ParentMenu.NMenuId))
                            {
                                CMList.Add(_mapper.Map<ChildMenuDto>(ChildMenu));
                            }
                            ParentMenu.ChildMenu = CMList;
                            PMList.Add(_mapper.Map<MenuDto>(ParentMenu));
                        }
                        loginRes.MenuList = PMList;
                        var LoginResponseAll = _mapper.Map<LoginResponseDto>(loginRes);
                        return (LoginResponseAll);
                    case "user":
                        var LoginResponseUser = _mapper.Map<UserDto>(loginRes);
                        return (LoginResponseUser);
                    case "fnyear":
                        var FnYear = _mapper.Map<FnYearDto>(loginRes);
                        return (FnYear);
                    case "company":
                        var Company = _mapper.Map<CompanyDto>(loginRes);
                        return (Company);

                    case "menu":
                        var RMenuList = _context.VwUserMenus
                 .Where(VwUserMenus => VwUserMenus.NUserCategoryId == loginRes.N_UserCategoryID && VwUserMenus.NCompanyId == loginRes.N_CompanyID && VwUserMenus.BShowOnline == true)
                 .ToList();
                        var RMenu = _mapper.Map<List<MenuDto>>(RMenuList);

                        List<MenuDto> RPMList = new List<MenuDto>();
                        foreach (var ParentMenu in RMenu.Where(y => y.NParentMenuId == 0))
                        {
                            List<ChildMenuDto> CMList = new List<ChildMenuDto>();
                            foreach (var ChildMenu in RMenu.Where(y => y.NParentMenuId == ParentMenu.NMenuId))
                            {
                                CMList.Add(_mapper.Map<ChildMenuDto>(ChildMenu));
                            }
                            ParentMenu.ChildMenu = CMList;
                            RPMList.Add(_mapper.Map<MenuDto>(ParentMenu));
                        }
                        return (RPMList);
                    case "refreshtoken":
                        var rtokenHandler = new JwtSecurityTokenHandler();
                        var rkey = Encoding.ASCII.GetBytes(_appSettings.Secret);
                        var rtokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier,loginRes.N_UserID.ToString()),
                        new Claim(ClaimTypes.Name,loginRes.X_UserName.ToString()),
                        new Claim(ClaimTypes.Role,loginRes.X_UserCategory),
                        new Claim(ClaimTypes.GroupSid,loginRes.N_UserCategoryID.ToString()),
                        new Claim(ClaimTypes.StreetAddress,loginRes.X_CompanyName),
                        new Claim(ClaimTypes.Sid,loginRes.N_CompanyID.ToString()),
                        new Claim(ClaimTypes.Version,"V0.1"),
                    }),
                            Expires = DateTime.UtcNow.AddDays(2),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(rkey), SecurityAlgorithms.HmacSha256Signature)
                        };
                        var rtoken = rtokenHandler.CreateToken(rtokenDescriptor);
                        var Token = rtokenHandler.WriteToken(rtoken);
                        loginRes.Expiry = DateTime.UtcNow.AddDays(2);
                        var LoginResponseToken = _mapper.Map<TokenDto>(loginRes);
                        return (LoginResponseToken);
                    default:
                        return (null);
                }
            }

            //If User Found

        }

        private string generateRefreshToken()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }


    }

    public interface ICommenServiceRepo
    {
        dynamic Authenticate(int companyid, string companyname, string username, int userid, string reqtype);
    }
}