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
//     public class CommenServiceRepo : ICommenServiceRepo
//     {
//         private readonly SmartxContext _context;
//         private readonly AppSettings _appSettings;
//         private readonly IMapper _mapper;
//         private readonly IApiFunctions _api;
//         private readonly IDataAccessLayer dLayer;
//         private readonly IMyFunctions myFunctions;
//         private readonly IConfiguration config;
//         private readonly string connectionString;
//         private readonly string masterDBConnectionString;
//         public CommenServiceRepo(SmartxContext context, IOptions<AppSettings> appSettings, IMapper mapper, IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf)
//         {
//             _context = context;
//             _appSettings = appSettings.Value;
//             _mapper = mapper;
//             _api = api;
//             dLayer = dl;
//             myFunctions = fun;
//             connectionString = conf.GetConnectionString("SmartxConnection");
//             masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");
//             config = conf;
//         }


//         public dynamic Authenticate(int companyid, string companyname, string username, int userid, string reqtype, int AppID, string uri, int clientID = 0, int globalUserID = 0)
//         {



//             if (string.IsNullOrEmpty(companyid.ToString()) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userid.ToString()))
//                 return null;

//             using (SqlConnection connection = new SqlConnection(uri != "" ? config.GetConnectionString(uri) : connectionString))
//             {
//                 connection.Open();

//                 SortedList paramsList = new SortedList()
//                     {
//                         {"X_CompanyName",companyname},
//                         {"X_FnYearDescr",""},
//                         {"X_LoginName",username}
//                     };
//                 DataTable loginDt = dLayer.ExecuteDataTablePro("SP_LOGIN_CLOUD", paramsList, connection);

//                 var loginRes = (from DataRow dr in loginDt.Rows
//                                 select new SP_LOGIN_CLOUD()
//                                 {
//                                     N_UserID = Convert.ToInt32(dr["N_UserID"]),
//                                     X_UserName = dr["X_UserName"].ToString(),
//                                     X_FnYearDescr = dr["X_FnYearDescr"].ToString(),
//                                     N_FnYearID = myFunctions.getIntVAL(dr["N_FnYearID"].ToString()),
//                                     D_Start = Convert.ToDateTime(dr["D_Start"].ToString()),
//                                     D_End = Convert.ToDateTime(dr["D_End"].ToString()),
//                                     X_AcYearDescr = dr["X_AcYearDescr"].ToString(),
//                                     N_AcYearID = myFunctions.getIntVAL(dr["N_AcYearID"].ToString()),
//                                     D_AcStart = Convert.ToDateTime(dr["D_AcStart"].ToString()),
//                                     D_AcEnd = Convert.ToDateTime(dr["D_AcEnd"].ToString()),
//                                     X_CompanyName = dr["X_CompanyName"].ToString(),
//                                     X_CompanyName_Ar = dr["X_CompanyName_Ar"].ToString(),
//                                     X_CompanyCode = dr["X_CompanyCode"].ToString(),
//                                     N_CompanyID = myFunctions.getIntVAL(dr["N_CompanyID"].ToString()),
//                                     X_UserCategory = dr["X_UserCategory"].ToString(),
//                                     N_UserCategoryID = myFunctions.getIntVAL(dr["N_UserCategoryID"].ToString()),
//                                     X_Country = dr["X_Country"].ToString(),
//                                     N_CountryID = Convert.ToInt32(dr["N_CountryID"]),
//                                     N_CurrencyID = myFunctions.getIntVAL(dr["N_CurrencyID"].ToString()),
//                                     D_LoginDate = Convert.ToDateTime(dr["D_LoginDate"].ToString()),
//                                     X_Language = dr["X_Language"].ToString(),
//                                     N_LanguageID = myFunctions.getIntVAL(dr["N_LanguageID"].ToString()),
//                                     N_BranchID = dr["N_BranchID"].ToString(),
//                                     X_BranchName = dr["X_BranchName"].ToString(),
//                                     X_LocationName = dr["X_LocationName"].ToString(),
//                                     N_LocationID = dr["N_LocationID"].ToString(),
//                                     I_Logo = (byte[])dr["I_Logo"],
//                                     B_AllBranchesData = (bool)dr["B_AllBranchesData"],
//                                     N_TaxType = myFunctions.getIntVAL(dr["N_TaxType"].ToString()),
//                                     X_UserFullName = dr["X_UserFullName"].ToString(),
//                                     X_UserCategoryIDList = dr["X_UserCategoryIDList"].ToString(),
//                                 }).ToList()
//                         .FirstOrDefault();


//                         var tokenHandler = new JwtSecurityTokenHandler();
//                         var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
//                         var tokenDescriptor = new SecurityTokenDescriptor
//                         {
//                             Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]{
//                         new Claim(ClaimTypes.NameIdentifier,"loginRes.N_UserID.ToString()"),
//                         new Claim(ClaimTypes.Name,"loginRes.X_UserFullName.ToString()"),
//                         new Claim(ClaimTypes.Role,"loginRes.X_UserCategory"),
//                         new Claim(ClaimTypes.GroupSid,"loginRes.N_UserCategoryID.ToString()"),
//                         new Claim(ClaimTypes.StreetAddress,"loginRes.X_CompanyName"),
//                         new Claim(ClaimTypes.Sid,"loginRes.N_CompanyID.ToString()"),
//                         new Claim(ClaimTypes.Version,"V0.1"),
//                         new Claim(ClaimTypes.System,"AppID.ToString()"),
//                         new Claim(ClaimTypes.PrimarySid,"globalUserID.ToString()"),
//                         new Claim(ClaimTypes.PrimaryGroupSid,"clientID.ToString()"),
//                         new Claim(ClaimTypes.UserData,"xGlobalUserID.ToString()"),
//                         new Claim(ClaimTypes.Email,"username"),
//                         new Claim(ClaimTypes.Upn,"username"),
//                         new Claim(ClaimTypes.Uri,"uri"),
//                         new Claim(ClaimTypes.SerialNumber,"userPattern.ToString()")
//                     }),
//                             Expires = DateTime.UtcNow.AddDays(2),
//                             SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//                         };
//                         var token = tokenHandler.CreateToken(tokenDescriptor);
//                         loginRes.Token = tokenHandler.WriteToken(token);
//                         loginRes.Expiry = DateTime.UtcNow.AddDays(2);
//                         loginRes.N_AppID = AppID;

//                         loginRes.RefreshToken = generateRefreshToken();
//                         var user = _context.SecUser.SingleOrDefault(u => u.NUserId == loginRes.N_UserID);
//                         user.XToken = loginRes.RefreshToken;

//                         object abc = dLayer.ExecuteScalar("Update Sec_User set X_Token='" + loginRes.RefreshToken + "' where N_UserID=" + loginRes.N_UserID + " and N_CompanyID=" + loginRes.N_CompanyID, connection);
//                         if (loginRes.I_Logo != null)
//                             loginRes.I_CompanyLogo = "data:image/png;base64," + Convert.ToBase64String(loginRes.I_Logo, 0, loginRes.I_Logo.Length);

//                         //     var MenuList = _context.VwUserMenus
//                         //     // .Where(VwUserMenus => VwUserMenus.NUserCategoryId == loginRes.N_UserCategoryID && VwUserMenus.NCompanyId == loginRes.N_CompanyID && VwUserMenus.BShowOnline == true)
//                         //   .Where(VwUserMenus => loginRes.X_UserCategoryIDList.Contains(VwUserMenus.NUserCategoryId.ToString() ) && VwUserMenus.NCompanyId == loginRes.N_CompanyID && VwUserMenus.BShowOnline == true)

//                         //     .OrderBy(VwUserMenus => VwUserMenus.NOrder)
//                         //     .ToList();
//                         string Modules = "";
//                         string firstModule = "";

//                         using (SqlConnection cnn2 = new SqlConnection(masterDBConnectionString))
//                         {
//                             cnn2.Open();
//                             if (AppID != 6 && AppID != 8)
//                             {
//                                 string appUpdate = "Update Users set N_ActiveAppID=" + AppID + " WHERE (X_EmailID ='" + username + "' and N_UserID=" + globalUserID + ")";
//                                 dLayer.ExecuteScalar(appUpdate, cnn2);
//                             }

//                             object modulesObj = dLayer.ExecuteScalar("SELECT X_Modules=STUFF  ((SELECT DISTINCT ',' + CAST(N_ModuleID AS VARCHAR(MAX)) FROM AppModules t2 WHERE t2.N_AppID = t1.N_AppID and  t2.N_AppID=" + AppID + " FOR XML PATH('') ),1,1,''  )  FROM AppModules t1  where t1.N_AppID=" + AppID, cnn2);
//                             if (modulesObj == null)
//                             {
//                                 Modules = "-1";
//                             }
//                             else
//                             {
//                                 Modules = modulesObj.ToString();
//                                 string[] ModuleList = Modules.Split(",");
//                                 if (ModuleList.Length > 0)
//                                     firstModule = ModuleList[0];
//                             }

//                         }


//                         //                     string MenuSql = "select N_MenuId,X_MenuName,X_Caption,N_ParentMenuId,N_Order,N_HasChild,B_Visible,B_Edit,B_Delete,B_Save,B_View,X_ShortcutKey,X_CaptionAr,X_FormNameWithTag," +
//                         // "N_IsStartup,N_IsStartup,B_Show,X_RouteName,B_ShowOnline,B_WShow from VwUserMenus where N_UserCategoryId in ( " + loginRes.X_UserCategoryIDList + " ) and  N_CompanyId=" + loginRes.N_CompanyID + " and B_ShowOnline=1 " +
//                         // " and ( N_ParentMenuId in( " + Modules + ") or N_MenuID in( " + Modules + ") ) Group by N_MenuId,X_MenuName,X_Caption,N_ParentMenuId,N_Order,N_HasChild,B_Visible,B_Edit,B_Delete," +
//                         // "B_Save,B_View,X_ShortcutKey,X_CaptionAr,X_FormNameWithTag,N_IsStartup,N_IsStartup,B_Show,X_RouteName,B_ShowOnline,B_WShow order by N_Order ";

//                         string MenuSql = "select * from (select * from vw_UserMenus_Cloud where N_UserCategoryId in (  " + loginRes.X_UserCategoryIDList + "  ) and  N_CompanyId=" + loginRes.N_CompanyID + "  and B_ShowOnline=1 and ( N_ParentMenuId in( " + Modules + ") or N_MenuID in(" + Modules + " ) ) " +
//                                         " union " +
//                                         " select N_MenuId,X_MenuName,X_Caption,-1 as N_ParentMenuId,N_Order,N_HasChild,B_Visible,B_Edit,B_Delete,B_Save,B_View,X_ShortcutKey,X_CaptionAr,X_FormNameWithTag,N_IsStartup,B_Show,X_RouteName,B_ShowOnline,cast(0 as bit) as B_WShow,N_UserCategoryId,N_CompanyId from vw_UserMenus_Cloud where N_UserCategoryId in (  " + loginRes.X_UserCategoryIDList + "  ) and  N_CompanyId=" + loginRes.N_CompanyID + "  and B_ShowOnline=1 and ( N_ParentMenuId not in(" + Modules + ",0) )" +
//                                         " union " +
//                                         " select Top(1) -1 as N_MenuId,'Other Menus' as X_MenuName,'Other Menus' as X_Caption,0 as N_ParentMenuId,0 as N_Order,cast(0 as bit) as N_HasChild,cast(0 as bit) as B_Visible,cast(0 as bit) as B_Edit,cast(0 as bit) as B_Delete,cast(0 as bit) as B_Save,cast(0 as bit) as B_View,'' as X_ShortcutKey,'' as X_CaptionAr,'' as X_FormNameWithTag,cast(0 as bit) as N_IsStartup,cast(0 as bit) as B_Show,'' as X_RouteName,cast(0 as bit) as B_ShowOnline,cast(0 as bit) as B_WShow,0 as N_UserCategoryId,0 as N_CompanyId from vw_UserMenus_Cloud )  as tbl order by B_WShow desc,N_Order";

//                         DataTable MenusDTB = dLayer.ExecuteDataTable(MenuSql, connection);

//                         var Menu = (from DataRow dr in MenusDTB.Rows
//                                     select new MenuDto()
//                                     {
//                                         NMenuId = Convert.ToInt32(dr["N_MenuId"]),
//                                         XMenuName = dr["X_MenuName"].ToString(),
//                                         XCaption = dr["X_Caption"].ToString(),
//                                         NParentMenuId = Convert.ToInt32(dr["N_ParentMenuId"]),
//                                         NOrder = Convert.ToInt32(dr["N_Order"]),
//                                         NHasChild = (bool)(dr["N_HasChild"] == System.DBNull.Value ? false : dr["N_HasChild"]),
//                                         BVisible = (bool)(dr["B_Visible"] == System.DBNull.Value ? false : dr["B_Visible"]),
//                                         BEdit = (bool)(dr["B_Edit"] == System.DBNull.Value ? false : dr["B_Edit"]),
//                                         BDelete = (bool)(dr["B_Delete"] == System.DBNull.Value ? false : dr["B_Delete"]),
//                                         BSave = (bool)(dr["B_Save"] == System.DBNull.Value ? false : dr["B_Save"]),
//                                         BView = (bool)(dr["B_View"] == System.DBNull.Value ? false : dr["B_View"]),
//                                         XShortcutKey = dr["X_ShortcutKey"].ToString(),
//                                         XCaptionAr = dr["X_CaptionAr"].ToString(),
//                                         XFormNameWithTag = dr["X_FormNameWithTag"].ToString(),
//                                         NIsStartup = (bool)(dr["N_IsStartup"] == System.DBNull.Value ? false : dr["N_IsStartup"]),
//                                         BShow = (bool)(dr["B_Show"] == System.DBNull.Value ? false : dr["B_Show"]),
//                                         XRouteName = dr["X_RouteName"].ToString(),
//                                         BShowOnline = (bool)(dr["B_ShowOnline"] == System.DBNull.Value ? false : dr["B_ShowOnline"]),
//                                         BWShow = (bool)(dr["B_WShow"] == System.DBNull.Value ? false : dr["B_WShow"]),
//                                     }).ToList();


//                         List<MenuDto> PMList = new List<MenuDto>();
//                         foreach (var ParentMenu in Menu.Where(y => y.NParentMenuId == 0 && y.NMenuId != -1).OrderBy(VwUserMenus => VwUserMenus.NOrder))
//                         {
//                             List<ChildMenuDto> CMList = new List<ChildMenuDto>();
//                             foreach (var ChildMenu in Menu.Where(y => y.NParentMenuId == ParentMenu.NMenuId).OrderBy(VwUserMenus => VwUserMenus.NOrder))
//                             {
//                                 CMList.Add(_mapper.Map<ChildMenuDto>(ChildMenu));
//                             }
//                             ParentMenu.ChildMenu = CMList;
//                             PMList.Add(_mapper.Map<MenuDto>(ParentMenu));
//                         }
//                         loginRes.MenuList = PMList;

//                         List<MenuDto> AccessMList = new List<MenuDto>();
//                         foreach (var ParentMenu in Menu.Where(y => y.NParentMenuId == 0).OrderBy(VwUserMenus => VwUserMenus.NOrder))
//                         {
//                             List<ChildMenuDto> AccessCMList = new List<ChildMenuDto>();
//                             foreach (var ChildMenu in Menu.Where(y => y.NParentMenuId == ParentMenu.NMenuId).OrderBy(VwUserMenus => VwUserMenus.NOrder))
//                             {
//                                 AccessCMList.Add(_mapper.Map<ChildMenuDto>(ChildMenu));
//                             }
//                             ParentMenu.ChildMenu = AccessCMList;
//                             AccessMList.Add(_mapper.Map<MenuDto>(ParentMenu));
//                         }
//                         loginRes.AccessList = AccessMList;
//                         var LoginResponseAll = _mapper.Map<LoginResponseDto>(loginRes);
//                         return (LoginResponseAll);
   
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

//     public interface ICommenServiceRepo
//     {
//         dynamic Authenticate(int companyid, string companyname, string username, int userid, string reqtype, int AppID, string uri, int clientID, int globalUserID);
//     }
// }