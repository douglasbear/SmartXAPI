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
        private readonly IConfiguration config;
        private readonly string connectionString;
        private readonly string masterDBConnectionString;
        public CommenServiceRepo(SmartxContext context, IOptions<AppSettings> appSettings, IMapper mapper, IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");
            config = conf;
        }


        public dynamic Authenticate(int companyid, string companyname, string username, int userid, string reqtype, int AppID, string uri, int clientID = 0, int globalUserID = 0, string requestIP = "", int loginID = 0)
        {


            try
            {
                if (string.IsNullOrEmpty(companyid.ToString()) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userid.ToString()))
                    return null;

                using (SqlConnection connection = new SqlConnection(uri != "" ? config.GetConnectionString(uri) : connectionString))
                {
                    connection.Open();

                    SortedList paramsList = new SortedList()
                    {
                        {"X_CompanyName",companyname},
                        {"X_FnYearDescr",""},
                        {"X_LoginName",username}
                    };
                    DataTable loginDt = dLayer.ExecuteDataTablePro("SP_LOGIN_CLOUD", paramsList, connection);

                    var loginRes = (from DataRow dr in loginDt.Rows
                                    select new SP_LOGIN_CLOUD()
                                    {
                                        N_UserID = Convert.ToInt32(dr["N_UserID"]),
                                        X_UserName = dr["X_UserName"].ToString(),
                                        X_FnYearDescr = dr["X_FnYearDescr"].ToString(),
                                        N_FnYearID = myFunctions.getIntVAL(dr["N_FnYearID"].ToString()),
                                        D_Start = Convert.ToDateTime(dr["D_Start"].ToString()),
                                        D_End = Convert.ToDateTime(dr["D_End"].ToString()),
                                        X_AcYearDescr = dr["X_AcYearDescr"].ToString(),
                                        N_AcYearID = myFunctions.getIntVAL(dr["N_AcYearID"].ToString()),
                                        D_AcStart = Convert.ToDateTime(dr["D_AcStart"].ToString()),
                                        D_AcEnd = Convert.ToDateTime(dr["D_AcEnd"].ToString()),
                                        X_CompanyName = dr["X_CompanyName"].ToString(),
                                        X_CompanyName_Ar = dr["X_CompanyName_Ar"].ToString(),
                                        X_CompanyCode = dr["X_CompanyCode"].ToString(),
                                        N_CompanyID = myFunctions.getIntVAL(dr["N_CompanyID"].ToString()),
                                        X_UserCategory = dr["X_UserCategory"].ToString(),
                                        N_UserCategoryID = myFunctions.getIntVAL(dr["N_UserCategoryID"].ToString()),
                                        X_Country = dr["X_Country"].ToString(),
                                        N_CountryID = Convert.ToInt32(dr["N_CountryID"]),
                                        N_CurrencyID = myFunctions.getIntVAL(dr["N_CurrencyID"].ToString()),
                                        N_CurrencyDecimal = myFunctions.getIntVAL(dr["N_CurrencyDecimal"].ToString()),
                                        N_DecimalPlace = myFunctions.getIntVAL(dr["N_DecimalPlace"].ToString()),
                                        D_LoginDate = Convert.ToDateTime(dr["D_LoginDate"].ToString()),
                                        X_Language = dr["X_Language"].ToString(),
                                        X_CurrencyName = dr["X_CurrencyName"].ToString(),
                                        N_LanguageID = myFunctions.getIntVAL(dr["N_LanguageID"].ToString()),
                                        N_BranchID = dr["N_BranchID"].ToString(),
                                        X_BranchName = dr["X_BranchName"].ToString(),
                                        X_LocationName = dr["X_LocationName"].ToString(),
                                        N_LocationID = dr["N_LocationID"].ToString(),
                                        I_Logo = (byte[])dr["I_Logo"],
                                        B_AllBranchesData = (bool)dr["B_AllBranchesData"],
                                        N_TaxType = myFunctions.getIntVAL(dr["N_TaxType"].ToString()),
                                        X_UserFullName = dr["X_UserFullName"].ToString(),
                                        X_UserCategoryIDList = dr["X_UserCategoryIDList"].ToString(),
                                        X_BranchCode = dr["X_BranchCode"].ToString(),
                                        N_Decimal = myFunctions.getIntVAL(dr["N_Decimal"].ToString()),
                                        N_EmpID = myFunctions.getIntVAL(dr["N_EmpID"].ToString()),
                                        N_PositionID = myFunctions.getIntVAL(dr["N_PositionID"].ToString()),
                                        N_DepartmentID = myFunctions.getIntVAL(dr["N_DepartmentID"].ToString()),
                                        X_EmpCode = dr["X_EmpCode"].ToString(),
                                        X_EmpName = dr["X_EmpName"].ToString(),
                                        X_EmpNameLocale = dr["X_EmpNameLocale"].ToString(),
                                        X_Position = dr["X_Position"].ToString(),
                                        X_Department = dr["X_Department"].ToString(),
                                        N_SalesmanID = myFunctions.getIntVAL(dr["N_SalesmanID"].ToString()),
                                        X_SalesmanCode = dr["X_SalesmanCode"].ToString(),
                                        X_SalesmanName = dr["X_SalesmanName"].ToString(),
                                        B_AllowEdit = (bool)dr["B_AllowEdit"],
                                        N_CustomerID = myFunctions.getIntVAL(dr["N_CustomerID"].ToString()),
                                        X_CustomerCode = dr["X_CustomerCode"].ToString(),
                                        X_CustomerName = dr["X_CustomerName"].ToString(),
                                        N_ParentID = myFunctions.getIntVAL(dr["N_ParentID"].ToString()),
                                        X_ParentCode = dr["X_ParentCode"].ToString(),
                                        X_ParentName = dr["X_ParentName"].ToString(),
                                        N_StudentID = myFunctions.getIntVAL(dr["N_StudentID"].ToString()),
                                        X_StudentCode = dr["X_StudentCode"].ToString(),
                                        X_StudentName = dr["X_StudentName"].ToString(),
                                        N_TeacherID = myFunctions.getIntVAL(dr["N_TeacherID"].ToString()),
                                        X_TeacherCode = dr["X_TeacherCode"].ToString(),
                                        X_TeacherName = dr["X_TeacherName"].ToString(),
                                        N_DefaultStudentID = myFunctions.getIntVAL(dr["N_DefaultStudentID"].ToString()),
                                        N_TypeID = Convert.ToInt32(dr["N_TypeID"]),
                                        X_UtcOffSet = dr["X_UtcOffSet"].ToString(),
                                        X_ZoneName = dr["X_ZoneName"].ToString(),
                                        N_TimeZoneID = Convert.ToInt32(dr["N_TimeZoneID"]),

                                    }).ToList()
                            .FirstOrDefault();


                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID", loginRes.N_CompanyID);
                    Params.Add("@nUserID", loginRes.N_UserID);
                    Params.Add("@nFnYearID", loginRes.N_FnYearID);
                    Params.Add("@nBranchID", loginRes.N_BranchID);
                    Params.Add("@nCurrencyID", loginRes.N_CurrencyID);
                    object userPattern = dLayer.ExecuteScalar("Select Isnull(X_Pattern,'') From Sec_UserHierarchy Where N_CompanyID=@nCompanyID and N_UserID=@nUserID", Params, connection);
                    if (userPattern == null)
                        userPattern = "";
                    loginRes.B_AllowEdit = true;
                    string xGlobalUserID = "";



                    SortedList logParams = new SortedList()
                    {
                        {"N_CompanyID",loginRes.N_CompanyID},
                        {"N_FnyearID",loginRes.N_FnYearID},
                        {"N_BranchId",loginRes.N_BranchID},
                        {"N_ActionID",1},
                        {"N_Type",1},
                        {"N_LoggedInID",loginID},
                        {"X_SystemName",requestIP},
                        {"N_UserID",loginRes.N_UserID}
                    };
                    int newLoginID = myFunctions.getIntVAL(dLayer.ExecuteScalarPro("SP_LoginDetailsInsert_Cloud", logParams, connection).ToString());
                    if (newLoginID == 0)
                    {
                        throw new Exception("Unauthorized Access");
                    }
                    else
                    {
                        loginRes.N_LoginID = newLoginID;
                    }

                    switch (reqtype.ToLower())
                    {
                        case "all":
                        case "app":
                        case "switchcompany":
                        case "customer":
                        case "vendor":


                            string Modules = "";
                            string firstModule = "";

                            using (SqlConnection cnn2 = new SqlConnection(masterDBConnectionString))
                            {
                                cnn2.Open();

                                string sqlGUserInfo = "SELECT Users.N_UserID, Users.X_EmailID, Users.X_UserName, Users.N_ClientID, Users.N_ActiveAppID, ClientApps.X_AppUrl,ClientApps.B_EnableAttachment,ClientApps.X_DBUri, AppMaster.X_AppName, ClientMaster.X_AdminUserID AS x_AdminUser,CASE WHEN Users.N_UserType=0 THEN 1 ELSE 0 end as isAdminUser,Users.X_UserID FROM Users LEFT OUTER JOIN ClientMaster ON Users.N_ClientID = ClientMaster.N_ClientID LEFT OUTER JOIN ClientApps ON Users.N_ActiveAppID = ClientApps.N_AppID AND Users.N_ClientID = ClientApps.N_ClientID LEFT OUTER JOIN AppMaster ON ClientApps.N_AppID = AppMaster.N_AppID WHERE (Users.X_UserID ='" + username + "')";
                                DataTable globalInfo = dLayer.ExecuteDataTable(sqlGUserInfo, cnn2);
                                if (globalInfo.Rows.Count > 0)
                                {
                                    loginRes.Warning = userNotifications(myFunctions.getIntVAL(globalInfo.Rows[0]["N_ClientID"].ToString()),AppID,cnn2);
                                    object AllowMultipleCompany=true;
                                    object numberOfCompanies = dLayer.ExecuteScalar("select count(*)   from Acc_Company where N_ClientID="+myFunctions.getIntVAL(globalInfo.Rows[0]["N_ClientID"].ToString())+"", Params, connection);
                                    object companyLimit = dLayer.ExecuteScalar("select isnull(N_Value,0) from GenSettings where N_ClientID="+myFunctions.getIntVAL(globalInfo.Rows[0]["N_ClientID"].ToString())+" and X_Description='COMPANY LIMIT'", Params, cnn2);
                                    if(companyLimit==null){companyLimit="0";}
                                     if(myFunctions.getIntVAL(companyLimit.ToString())<=myFunctions.getIntVAL(numberOfCompanies.ToString()))
                                     {
                                        AllowMultipleCompany=false;

                                     }

                                    globalInfo = myFunctions.AddNewColumnToDataTable(globalInfo, "B_AllowMultipleCom", typeof(bool), AllowMultipleCompany == null ? 0 : AllowMultipleCompany);
                                    loginRes.GlobalUserInfo = globalInfo;
                                    xGlobalUserID = globalInfo.Rows[0]["X_UserID"].ToString();
                                }

                                int daysToExpire = myFunctions.getIntVAL(dLayer.ExecuteScalar("select isnull(DATEDIFF(day, GETDATE(),min(D_ExpiryDate)),0) as expiry from ClientApps where N_ClientID=" + clientID+" and N_AppID="+AppID+"", cnn2).ToString());
                                if (daysToExpire <= 0)
                                {
                                    int expiredApp=AppID;
                                    string appsID=AppID.ToString();
                                    int flag=0;
                                     while (expiredApp != 0 )
                                    {
                                    string appNew = "select Top 1 N_AppID from Sec_UserApps where N_GlobalUserID="+globalInfo.Rows[0]["N_UserID"].ToString()+"  and N_AppID not in ("+appsID+") ";
                                    object newAppID =   dLayer.ExecuteScalar(appNew, connection);
                                    if(newAppID==null)
                                    {
                                        flag=1;
                                        expiredApp=0;
                                    }
                                    else
                                    {
                                         int daysToExpireNeApp = myFunctions.getIntVAL(dLayer.ExecuteScalar("select isnull(DATEDIFF(day, GETDATE(),min(D_ExpiryDate)),0) as expiry from ClientApps where N_ClientID=" + clientID+" and N_AppID="+myFunctions.getIntVAL(newAppID.ToString())+"", cnn2).ToString());
                                         if (daysToExpireNeApp <= 0)
                                         {
                                            appsID=appsID+","+newAppID.ToString();
                                            expiredApp=myFunctions.getIntVAL(newAppID.ToString());
                                            flag=1;
                                         }
                                        else  
                                        {
                                            AppID=myFunctions.getIntVAL(newAppID.ToString());
                                             expiredApp=0;
                                             flag=0;
                                        }
                                    }
                                }
                                if(flag==1)
                                  throw new Exception("Your Subscription Expired"); 

                                }  
                                // if (AppID != 6 && AppID != 8 && AppID != 15 && AppID != 16 && AppID != 13 && AppID != 14 && AppID != 18)
                                if (AppID != 6 && AppID != 15 && AppID != 16 && AppID != 8 && AppID != 13 && AppID != 14 && AppID != 18 && AppID != 20 && AppID != 21 && AppID != 22)
                                {
                                    string appUpdate = "Update Users set N_ActiveAppID=" + AppID + " WHERE (X_EmailID ='" + username + "' and N_UserID=" + globalUserID + ")";
                                    dLayer.ExecuteScalar(appUpdate, cnn2);
                                }
                                 if (globalInfo.Rows.Count > 0)
                                {
                                string sqlAttachment = "select isnull(B_EnableAttachment,0) from clientApps where N_AppId="+AppID+" and N_ClientID="+globalInfo.Rows[0]["N_ClientID"].ToString()+"";
                                object attachment = dLayer.ExecuteScalar(sqlAttachment, cnn2);
                                if(attachment!=null)
                                    globalInfo.Rows[0]["B_EnableAttachment"]=myFunctions.getBoolVAL(attachment.ToString());
                                globalInfo.AcceptChanges();


                                }

                                object modulesObj = dLayer.ExecuteScalar("SELECT X_Modules=STUFF  ((SELECT DISTINCT ',' + CAST(N_ModuleID AS VARCHAR(MAX)) FROM AppModules t2 WHERE t2.N_AppID = t1.N_AppID and  t2.N_AppID=" + AppID + " FOR XML PATH('') ),1,1,''  )  FROM AppModules t1  where t1.N_AppID=" + AppID, cnn2);
                                if (modulesObj == null)
                                {
                                    Modules = "-1";
                                }
                                else
                                {
                                    Modules = modulesObj.ToString();
                                    string[] ModuleList = Modules.Split(",");
                                    if (ModuleList.Length > 0)
                                        firstModule = ModuleList[0];
                                }

                            }


                            var tokenHandler = new JwtSecurityTokenHandler();
                            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                            var tokenDescriptor = new SecurityTokenDescriptor
                            {
                                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier,loginRes.N_UserID.ToString()),
                        new Claim(ClaimTypes.Name,loginRes.X_UserFullName.ToString()),
                        new Claim(ClaimTypes.Role,loginRes.X_UserCategory),
                        new Claim(ClaimTypes.GroupSid,loginRes.N_UserCategoryID.ToString()),
                        new Claim(ClaimTypes.StreetAddress,loginRes.X_CompanyName),
                        new Claim(ClaimTypes.Sid,loginRes.N_CompanyID.ToString()),
                        new Claim(ClaimTypes.Version,"V0.1"),
                        new Claim(ClaimTypes.System,AppID.ToString()),
                        new Claim(ClaimTypes.PrimarySid,globalUserID.ToString()),
                        new Claim(ClaimTypes.PrimaryGroupSid,clientID.ToString()),
                        new Claim(ClaimTypes.UserData,xGlobalUserID.ToString()),
                        new Claim(ClaimTypes.Email,username),
                        new Claim(ClaimTypes.Upn,username),
                        new Claim(ClaimTypes.Uri,uri),
                        new Claim(ClaimTypes.SerialNumber,userPattern.ToString()),
                        new Claim(ClaimTypes.AuthenticationInstant ,loginRes.X_UserCategoryIDList.ToString()),
                        new Claim(ClaimTypes.Thumbprint ,loginRes.N_LoginID.ToString())
                    }),
                                Expires = DateTime.UtcNow.AddDays(2),
                                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                            };
                            var token = tokenHandler.CreateToken(tokenDescriptor);
                            loginRes.Token = tokenHandler.WriteToken(token);
                            loginRes.Expiry = DateTime.UtcNow.AddDays(2);
                            loginRes.N_AppID = AppID;
                            loginRes.RefreshToken = generateRefreshToken();
                            var user = _context.SecUser.SingleOrDefault(u => u.NUserId == loginRes.N_UserID);
                            user.XToken = loginRes.RefreshToken;

                            object abc = dLayer.ExecuteScalar("Update Sec_User set X_Token='" + loginRes.RefreshToken + "' where N_UserID=" + loginRes.N_UserID + " and N_CompanyID=" + loginRes.N_CompanyID, connection);
                            if (loginRes.I_Logo != null)
                                loginRes.I_CompanyLogo = "data:image/png;base64," + Convert.ToBase64String(loginRes.I_Logo, 0, loginRes.I_Logo.Length);



                            string MenuSql = "select N_MenuId,X_MenuName,X_Caption,N_ParentMenuId,N_Order,N_HasChild,max(cast(isnull(B_Visible,0) as int)) as B_Visible,max(cast(isnull(B_Edit,0) as int)) as B_Edit,max(cast(isnull(B_Delete,0) as int)) as B_Delete, max(cast(isnull(B_Save,0) as int)) as B_Save,max(cast(isnull(B_View,0) as int)) as B_View,X_ShortcutKey,X_CaptionAr,X_FormNameWithTag,N_IsStartup,B_Show,X_RouteName,B_ShowOnline,B_WShow,max(N_UserCategoryId) as N_UserCategoryId,N_CompanyId from (select * from vw_UserMenus_Cloud where N_UserCategoryId in (  " + loginRes.X_UserCategoryIDList + "  ) and  N_CompanyId=" + loginRes.N_CompanyID + "  and B_ShowOnline=1 and ( N_ParentMenuId in( " + Modules + ") or N_MenuID in(" + Modules + " ) ) " +
                                            " union " +
                                            " select N_MenuId,X_MenuName,X_Caption,-1 as N_ParentMenuId,N_Order,N_HasChild,B_Visible,B_Edit,B_Delete,B_Save,B_View,X_ShortcutKey,X_CaptionAr,X_FormNameWithTag,N_IsStartup,B_Show,X_RouteName,B_ShowOnline,cast(0 as bit) as B_WShow,N_UserCategoryId,N_CompanyId from vw_UserMenus_Cloud where N_UserCategoryId in (  " + loginRes.X_UserCategoryIDList + "  ) and  N_CompanyId=" + loginRes.N_CompanyID + "  and B_ShowOnline=1 and ( N_ParentMenuId not in(" + Modules + ",0) )" +
                                            " union " +
                                            " select Top(1) -1 as N_MenuId,'Other Menus' as X_MenuName,'Other Menus' as X_Caption,0 as N_ParentMenuId,0 as N_Order,cast(0 as bit) as N_HasChild,cast(0 as bit) as B_Visible,cast(0 as bit) as B_Edit,cast(0 as bit) as B_Delete,cast(0 as bit) as B_Save,cast(0 as bit) as B_View,'' as X_ShortcutKey,'' as X_CaptionAr,'' as X_FormNameWithTag,cast(0 as bit) as N_IsStartup,cast(0 as bit) as B_Show,'' as X_RouteName,cast(0 as bit) as B_ShowOnline,cast(0 as bit) as B_WShow,0 as N_UserCategoryId,0 as N_CompanyId from vw_UserMenus_Cloud )  as tbl group by N_MenuId,X_MenuName,X_Caption,N_ParentMenuId,N_Order,N_HasChild,X_ShortcutKey,X_CaptionAr,X_FormNameWithTag,N_IsStartup,B_Show,X_RouteName,B_ShowOnline,B_WShow,N_CompanyId  order by B_WShow desc,N_Order";

                            DataTable MenusDTB = dLayer.ExecuteDataTable(MenuSql, connection);

                            var Menu = (from DataRow dr in MenusDTB.Rows
                                        select new MenuDto()
                                        {
                                            NMenuId = Convert.ToInt32(dr["N_MenuId"]),
                                            XMenuName = dr["X_MenuName"].ToString(),
                                            XCaption = dr["X_Caption"].ToString(),
                                            NParentMenuId = Convert.ToInt32(dr["N_ParentMenuId"]),
                                            NOrder = Convert.ToInt32(dr["N_Order"]),
                                            NHasChild = (bool)(dr["N_HasChild"] == System.DBNull.Value ? false : dr["N_HasChild"]),
                                            BVisible = (bool)(dr["B_Visible"].ToString() == "1" ? true : false),
                                            BEdit = (bool)(dr["B_Edit"].ToString() == "1" ? true : false),
                                            BDelete = (bool)(dr["B_Delete"].ToString() == "1" ? true : false),
                                            BSave = (bool)(dr["B_Save"].ToString() == "1" ? true : false),
                                            BView = (bool)(dr["B_View"].ToString() == "1" ? true : false),
                                            XShortcutKey = dr["X_ShortcutKey"].ToString(),
                                            XCaptionAr = dr["X_CaptionAr"].ToString(),
                                            XFormNameWithTag = dr["X_FormNameWithTag"].ToString(),
                                            NIsStartup = (bool)(dr["N_IsStartup"] == System.DBNull.Value ? false : dr["N_IsStartup"]),
                                            BShow = (bool)(dr["B_Show"] == System.DBNull.Value ? false : dr["B_Show"]),
                                            XRouteName = dr["X_RouteName"].ToString(),
                                            BShowOnline = (bool)(dr["B_ShowOnline"] == System.DBNull.Value ? false : dr["B_ShowOnline"]),
                                            BWShow = (bool)(dr["B_WShow"] == System.DBNull.Value ? false : dr["B_WShow"]),
                                        }).ToList();


                            List<MenuDto> PMList = new List<MenuDto>();
                            foreach (var ParentMenu in Menu.Where(y => y.NParentMenuId == 0 && y.NMenuId != -1).OrderBy(VwUserMenus => VwUserMenus.NOrder))
                            {
                                List<ChildMenuDto> CMList = new List<ChildMenuDto>();
                                foreach (var ChildMenu in Menu.Where(y => y.NParentMenuId == ParentMenu.NMenuId).OrderBy(VwUserMenus => VwUserMenus.NOrder))
                                {
                                    CMList.Add(_mapper.Map<ChildMenuDto>(ChildMenu));
                                }
                                ParentMenu.ChildMenu = CMList;
                                PMList.Add(_mapper.Map<MenuDto>(ParentMenu));
                            }
                            loginRes.MenuList = PMList;

                            List<MenuDto> AccessMList = new List<MenuDto>();
                            foreach (var ParentMenu in Menu.Where(y => y.NParentMenuId == 0).OrderBy(VwUserMenus => VwUserMenus.NOrder))
                            {
                                List<ChildMenuDto> AccessCMList = new List<ChildMenuDto>();
                                foreach (var ChildMenu in Menu.Where(y => y.NParentMenuId == ParentMenu.NMenuId).OrderBy(VwUserMenus => VwUserMenus.NOrder))
                                {
                                    AccessCMList.Add(_mapper.Map<ChildMenuDto>(ChildMenu));
                                }
                                ParentMenu.ChildMenu = AccessCMList;
                                AccessMList.Add(_mapper.Map<MenuDto>(ParentMenu));
                            }
                            loginRes.AccessList = AccessMList;
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
                            //         var RMenuList = _context.VwUserMenus
                            // .Where(VwUserMenus => loginRes.X_UserCategoryIDList.Contains(VwUserMenus.NUserCategoryId.ToString() ) && VwUserMenus.NCompanyId == loginRes.N_CompanyID && VwUserMenus.BShowOnline == true)
                            // //  .Where(VwUserMenus => VwUserMenus.NUserCategoryId == loginRes.N_UserCategoryID && VwUserMenus.NCompanyId == loginRes.N_CompanyID && VwUserMenus.BShowOnline == true)
                            //         .OrderBy(VwUserMenus => VwUserMenus.NOrder)
                            //  .ToList();
                            string NewMenuSql = "select N_MenuId,X_MenuName,X_Caption,N_ParentMenuId,N_Order,N_HasChild,B_Visible,B_Edit,B_Delete,B_Save,B_View,X_ShortcutKey,X_CaptionAr,X_FormNameWithTag," +
            "N_IsStartup,N_IsStartup,B_Show,X_RouteName,B_ShowOnline,B_WShow from VwUserMenus where N_UserCategoryId in ( " + loginRes.X_UserCategoryIDList + " ) and  N_CompanyId=" + loginRes.N_CompanyID + " and B_ShowOnline=1 Group by N_MenuId,X_MenuName,X_Caption,N_ParentMenuId,N_Order,N_HasChild,B_Visible,B_Edit,B_Delete," +
            "B_Save,B_View,X_ShortcutKey,X_CaptionAr,X_FormNameWithTag,N_IsStartup,N_IsStartup,B_Show,X_RouteName,B_ShowOnline,B_WShow order by N_Order ";
                            DataTable MenusDT = dLayer.ExecuteDataTable(NewMenuSql, connection);

                            var RMenu = (from DataRow dr in MenusDT.Rows
                                         select new MenuDto()
                                         {
                                             NMenuId = Convert.ToInt32(dr["N_MenuId"]),
                                             XMenuName = dr["X_MenuName"].ToString(),
                                             XCaption = dr["X_Caption"].ToString(),
                                             NParentMenuId = Convert.ToInt32(dr["N_ParentMenuId"]),
                                             NOrder = Convert.ToInt32(dr["N_Order"]),
                                             NHasChild = (bool)(dr["N_HasChild"] == System.DBNull.Value ? false : dr["N_HasChild"]),
                                             BVisible = (bool)(dr["B_Visible"] == System.DBNull.Value ? false : dr["B_Visible"]),
                                             BEdit = (bool)(dr["B_Edit"] == System.DBNull.Value ? false : dr["B_Edit"]),
                                             BDelete = (bool)(dr["B_Delete"] == System.DBNull.Value ? false : dr["B_Delete"]),
                                             BSave = (bool)(dr["B_Save"] == System.DBNull.Value ? false : dr["B_Save"]),
                                             BView = (bool)(dr["B_View"] == System.DBNull.Value ? false : dr["B_View"]),
                                             XShortcutKey = dr["X_ShortcutKey"].ToString(),
                                             XCaptionAr = dr["X_CaptionAr"].ToString(),
                                             XFormNameWithTag = dr["X_FormNameWithTag"].ToString(),
                                             NIsStartup = (bool)(dr["N_IsStartup"] == System.DBNull.Value ? false : dr["N_IsStartup"]),
                                             BShow = (bool)(dr["B_Show"] == System.DBNull.Value ? false : dr["B_Show"]),
                                             XRouteName = dr["X_RouteName"].ToString(),
                                             BShowOnline = (bool)(dr["B_ShowOnline"] == System.DBNull.Value ? false : dr["B_ShowOnline"]),
                                             BWShow = (bool)(dr["B_WShow"] == System.DBNull.Value ? false : dr["B_WShow"]),
                                         }).ToList();

                            List<MenuDto> RPMList = new List<MenuDto>();
                            foreach (var ParentMenu in RMenu.Where(y => y.NParentMenuId == 0).OrderBy(y => y.NOrder))
                            {
                                List<ChildMenuDto> CMList = new List<ChildMenuDto>();
                                foreach (var ChildMenu in RMenu.Where(y => y.NParentMenuId == ParentMenu.NMenuId).OrderBy(y => y.NOrder))
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
                        new Claim(ClaimTypes.Name,loginRes.X_UserFullName.ToString()),
                        new Claim(ClaimTypes.Role,loginRes.X_UserCategory),
                        new Claim(ClaimTypes.GroupSid,loginRes.N_UserCategoryID.ToString()),
                        new Claim(ClaimTypes.StreetAddress,loginRes.X_CompanyName),
                        new Claim(ClaimTypes.Sid,loginRes.N_CompanyID.ToString()),
                        new Claim(ClaimTypes.Version,"V0.1"),
                        new Claim(ClaimTypes.System,AppID.ToString()),
                        new Claim(ClaimTypes.PrimarySid,globalUserID.ToString()),
                        new Claim(ClaimTypes.PrimaryGroupSid,clientID.ToString()),
                        new Claim(ClaimTypes.UserData,xGlobalUserID.ToString()),
                        new Claim(ClaimTypes.Email,username),
                        new Claim(ClaimTypes.Upn,username),
                        new Claim(ClaimTypes.Uri,uri),
                        new Claim(ClaimTypes.SerialNumber,userPattern.ToString()),
                        new Claim(ClaimTypes.AuthenticationInstant ,loginRes.X_UserCategoryIDList.ToString()),
                        new Claim(ClaimTypes.Thumbprint ,loginRes.N_LoginID.ToString())
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

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

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

        private string userNotifications(int nClientID,int appID,SqlConnection cnn)
        {
            try
            {
                string Message = "";
                    int DaysToExpire = myFunctions.getIntVAL(dLayer.ExecuteScalar("select isnull(DATEDIFF(day, GETDATE(),min(D_ExpiryDate)),0) as expiry from ClientApps where N_ClientID=" + nClientID+" and N_AppID="+appID+"", cnn).ToString());

                    if (DaysToExpire <= 10)
                    {
                            string days = DaysToExpire == 1 ? " day" : " days";
                            string xExpiryInfo = DaysToExpire > 0 ? "expiring in " + DaysToExpire + days : "expired";
                            string xProductName = dLayer.ExecuteScalar("SELECT X_AppDescription FROM AppMaster INNER JOIN ClientMaster ON ClientMaster.N_DefaultAppID = AppMaster.N_AppID where N_ClientID=" + nClientID, cnn).ToString();
                            // Message = "Your subscription for " + xProductName +" is "+ xExpiryInfo+", please contact your salesperson immediately";
                            Message = "Your subscription is "+ xExpiryInfo;
                    }
               
                return Message;
            }
            catch (Exception ex)
            {
                return "";
            }
        }


    }

    public interface ICommenServiceRepo
    {
        dynamic Authenticate(int companyid, string companyname, string username, int userid, string reqtype, int AppID, string uri, int clientID, int globalUserID, string requestIP, int loginID);
    }
}