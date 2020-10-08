// using Dapper;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using SmartxAPI.Models;
// using SmartxAPI.Dtos.SP;
// using Microsoft.EntityFrameworkCore;
// using SmartxAPI.Profiles;
// using Microsoft.Extensions.Options;
// using System.IdentityModel.Tokens.Jwt;
// using System.Text;
// using Microsoft.IdentityModel.Tokens;
// using System.Security.Claims;
// using Newtonsoft.Json;
// using SmartxAPI.Dtos.Login;
// using AutoMapper;
// using System.Security.Cryptography;
// using SmartxAPI.GeneralFunctions;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Data.SqlClient;
// using System.Collections;
// using System.Data;

// namespace SmartxAPI.Data
// {
//     public class CommenServiceRepo_PRG : ICommenServiceRepo_PRG
//     {
//         private readonly SmartxContext _context;
//         private readonly AppSettings _appSettings;
//         private readonly IMapper _mapper;
//         private readonly IApiFunctions _api;
//         private readonly IDataAccessLayer dLayer;
//         private readonly IMyFunctions myFunctions;
//         private readonly string connectionString;
//         public CommenServiceRepo_PRG(SmartxContext context, IOptions<AppSettings> appSettings, IMapper mapper, IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf)
//         {
//             _context = context;
//             _appSettings = appSettings.Value;
//             _mapper = mapper;
//             _api = api;
//             dLayer = dl;
//             myFunctions = fun;
//             connectionString = conf.GetConnectionString("SmartxConnection");
//         }


//         public dynamic Authenticate(int companyid, string companyname, string username, int userid, string reqtype)
//         {

//             if (string.IsNullOrEmpty(companyid.ToString()) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userid.ToString()))
//                 return null;


//             using (SqlConnection connection = new SqlConnection(connectionString))
//             {
//                 connection.Open();

//             // var password = _context.SecUser
//             // .Where(y => y.NCompanyId == companyid && y.XUserId == username && y.NUserId == userid)
//             // .Select(x => x.XPassword)
//             // .FirstOrDefault();
//             SortedList pwdParams = new SortedList();
//             pwdParams.Add("@nCompanyID", companyid);
//             pwdParams.Add("@xUserName", username);
//             pwdParams.Add("@nUserID", userid);
//             object password = dLayer.ExecuteScalar("select X_Password from Sec_User where N_CompanyID=@nCompanyID and X_UserId=@xUserName and N_UserId=@nUserID",pwdParams,connection);

//                         DataTable loginDt=new DataTable();
//                             SortedList paramsList = new SortedList()
//                     {
//                         {"X_CompanyName",companyname},
//                         {"X_FnYearDescr",""},
//                         {"X_LoginName",username},
//                         {"X_Pwd",password.ToString()}
//                     };
//                      loginDt = dLayer.ExecuteDataTablePro("SP_LOGIN",paramsList,connection);
//             loginDt = myFunctions.AddNewColumnToDataTable(loginDt,"X_CurrencyName",typeof(string),"");
//             loginDt = myFunctions.AddNewColumnToDataTable(loginDt,"Expiry",typeof(DateTime),null);
//             loginDt = myFunctions.AddNewColumnToDataTable(loginDt,"RefreshToken",typeof(string),"");
//             loginDt = myFunctions.AddNewColumnToDataTable(loginDt,"Token",typeof(string),"");

// //		public List<MenuDto> MenuList{get;set;}
//             DataRow loginRes=loginDt.Rows[0];
//             // var loginRes = _context.SP_LOGIN.FromSqlRaw<SP_LOGIN>("SP_LOGIN @p0,@p1,@p2,@p3", companyname, "", username, password.ToString())
//             // .ToList()
//             // .FirstOrDefault();

//                     SortedList Params = new SortedList();
//                     Params.Add("@nCompanyID", loginRes["N_CompanyID"]);

//                 if (loginRes["N_BranchID"] == null || loginRes["N_BranchID"].ToString() == "")
//                 {
//                     loginRes["X_BranchName"] = dLayer.ExecuteScalar("Select X_BranchName From Acc_BranchMaster Where N_CompanyID=@nCompanyID  and B_DefaultBranch=1",Params, connection).ToString();
//                     loginRes["N_BranchID"] = dLayer.ExecuteScalar("Select N_BranchID From Acc_BranchMaster Where N_CompanyID=@nCompanyID  and B_DefaultBranch=1",Params, connection).ToString();
//                     loginRes["B_AllBranchesData"] = Convert.ToBoolean(dLayer.ExecuteScalar("Select B_ShowAllData From Acc_BranchMaster Where N_CompanyID=@nCompanyID and B_DefaultBranch=1",Params, connection).ToString());
//                 }
//                 Params.Add("@nBranchID", loginRes["N_BranchID"]);
//                 loginRes["X_LocationName"] = dLayer.ExecuteScalar("Select X_LocationName From Inv_Location Where N_CompanyID=@nCompanyID  and N_TypeID=2 and B_IsDefault=1  and N_BranchID=@nBranchID",Params, connection).ToString();
//                 loginRes["N_LocationID"] = dLayer.ExecuteScalar("Select N_LocationID From Inv_Location Where N_CompanyID=@nCompanyID  and B_IsDefault=1 and N_BranchID=@nBranchID",Params, connection).ToString();

//                 loginRes["N_CurrencyID"] = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_CurrencyID  from Acc_CurrencyMaster where N_CompanyID=@nCompanyID  and B_Default=1",Params, connection).ToString());
//                 loginRes["X_CurrencyName"] = dLayer.ExecuteScalar("select X_ShortName  from Acc_CurrencyMaster where N_CompanyID=@nCompanyID  and N_CurrencyID=@nCompanyID",Params, connection).ToString();


//                 switch (reqtype.ToLower())
//                 {
//                     case "all":
//                         var tokenHandler = new JwtSecurityTokenHandler();
//                         var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
//                         var tokenDescriptor = new SecurityTokenDescriptor
//                         {
//                             Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]{
//                         new Claim(ClaimTypes.NameIdentifier,loginRes["N_UserID"].ToString()),
//                         new Claim(ClaimTypes.Name,loginRes["X_UserName"].ToString()),
//                         new Claim(ClaimTypes.Role,loginRes["X_UserCategory"].ToString()),
//                         new Claim(ClaimTypes.GroupSid,loginRes["N_UserCategoryID"].ToString()),
//                         new Claim(ClaimTypes.StreetAddress,loginRes["X_CompanyName"].ToString()),
//                         new Claim(ClaimTypes.Sid,loginRes["N_CompanyID"].ToString()),
//                         new Claim(ClaimTypes.Version,"V0.1"),
//                     }),
//                             Expires = DateTime.UtcNow.AddDays(2),
//                             SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//                         };
//                         var token = tokenHandler.CreateToken(tokenDescriptor);
//                         loginRes["Token"] = tokenHandler.WriteToken(token);
//                         loginRes["Expiry"] = DateTime.UtcNow.AddDays(2);

//                         loginRes["RefreshToken"] = generateRefreshToken();
//                         var user = _context.SecUser.SingleOrDefault(u => u.NUserId == myFunctions.getIntVAL(loginRes["N_UserID"].ToString()));
//                         user.XToken = loginRes["RefreshToken"].ToString();

//                         object abc = dLayer.ExecuteScalar("Update Sec_User set X_Token='" + loginRes["RefreshToken"] + "' where N_UserID=" + loginRes["N_UserID"] + " and N_CompanyID=" + loginRes["N_CompanyID"], connection);
//                         // if (loginRes["I_Logo"] != null)
//                         //     loginRes.[I_CompanyLogo] = "data:image/png;base64," + Convert.ToBase64String(loginRes.I_Logo, 0, loginRes.I_Logo.Length);
//                         var MenuList = _context.VwUserMenus
//                         .Where(VwUserMenus => VwUserMenus.NUserCategoryId == myFunctions.getIntVAL(loginRes["N_UserCategoryID"].ToString()) && VwUserMenus.NCompanyId == myFunctions.getIntVAL(loginRes["N_CompanyID"].ToString()) && VwUserMenus.BShowOnline == true)
//                         .ToList();
//                         var Menu = _mapper.Map<List<MenuDto>>(MenuList);

//                         List<MenuDto> PMList = new List<MenuDto>();
//                         foreach (var ParentMenu in Menu.Where(y => y.NParentMenuId == 0))
//                         {
//                             List<ChildMenuDto> CMList = new List<ChildMenuDto>();
//                             foreach (var ChildMenu in Menu.Where(y => y.NParentMenuId == ParentMenu.NMenuId))
//                             {
//                                 CMList.Add(_mapper.Map<ChildMenuDto>(ChildMenu));
//                             }
//                             ParentMenu.ChildMenu = CMList;
//                             PMList.Add(_mapper.Map<MenuDto>(ParentMenu));
//                         }
//                         loginRes.MenuList = PMList;
//                         var LoginResponseAll = _mapper.Map<LoginResponseDto>(loginRes);
//                         return (LoginResponseAll);
//                     case "user":
//                         var LoginResponseUser = _mapper.Map<UserDto>(loginRes);
//                         return (LoginResponseUser);
//                     case "fnyear":
//                         var FnYear = _mapper.Map<FnYearDto>(loginRes);
//                         return (FnYear);
//                     case "company":
//                         var Company = _mapper.Map<CompanyDto>(loginRes);
//                         return (Company);

//                     case "menu":
//                         var RMenuList = _context.VwUserMenus
//                  .Where(VwUserMenus => VwUserMenus.NUserCategoryId == loginRes.N_UserCategoryID && VwUserMenus.NCompanyId == loginRes.N_CompanyID && VwUserMenus.BShowOnline == true)
//                  .ToList();
//                         var RMenu = _mapper.Map<List<MenuDto>>(RMenuList);

//                         List<MenuDto> RPMList = new List<MenuDto>();
//                         foreach (var ParentMenu in RMenu.Where(y => y.NParentMenuId == 0))
//                         {
//                             List<ChildMenuDto> CMList = new List<ChildMenuDto>();
//                             foreach (var ChildMenu in RMenu.Where(y => y.NParentMenuId == ParentMenu.NMenuId))
//                             {
//                                 CMList.Add(_mapper.Map<ChildMenuDto>(ChildMenu));
//                             }
//                             ParentMenu.ChildMenu = CMList;
//                             RPMList.Add(_mapper.Map<MenuDto>(ParentMenu));
//                         }
//                         return (RPMList);
//                     case "refreshtoken":
//                         var rtokenHandler = new JwtSecurityTokenHandler();
//                         var rkey = Encoding.ASCII.GetBytes(_appSettings.Secret);
//                         var rtokenDescriptor = new SecurityTokenDescriptor
//                         {
//                             Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]{
//                         new Claim(ClaimTypes.NameIdentifier,loginRes.N_UserID.ToString()),
//                         new Claim(ClaimTypes.Name,loginRes.X_UserName),
//                         new Claim(ClaimTypes.Role,loginRes.X_UserCategory),
//                         new Claim(ClaimTypes.GroupSid,loginRes.N_UserCategoryID.ToString()),
//                         new Claim(ClaimTypes.StreetAddress,loginRes.X_CompanyName),
//                         new Claim(ClaimTypes.Sid,loginRes.N_CompanyID.ToString()),
//                         new Claim(ClaimTypes.Version,"V0.1"),
//                     }),
//                             Expires = DateTime.UtcNow.AddDays(2),
//                             SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(rkey), SecurityAlgorithms.HmacSha256Signature)
//                         };
//                         var rtoken = rtokenHandler.CreateToken(rtokenDescriptor);
//                         var Token = rtokenHandler.WriteToken(rtoken);
//                         loginRes.Expiry = DateTime.UtcNow.AddDays(2);
//                         var LoginResponseToken = _mapper.Map<TokenDto>(loginRes);
//                         return (LoginResponseToken);
//                     default:
//                         return (null);
//                 }
//             }

//             //If User Found

//         }

//         private string generateRefreshToken()
//         {
//             using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
//             {
//                 var randomBytes = new byte[64];
//                 rngCryptoServiceProvider.GetBytes(randomBytes);
//                 return Convert.ToBase64String(randomBytes);
//             }
//         }


//     }

//     public interface ICommenServiceRepo_PRG
//     {
//         dynamic Authenticate(int companyid, string companyname, string username, int userid, string reqtype);
//     }
// }