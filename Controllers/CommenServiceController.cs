using SmartxAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Linq;
using System.Security.Claims;
using SmartxAPI.GeneralFunctions;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using SmartxAPI.Profiles;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("services")]
    [ApiController]
    public class CommenServiceController : ControllerBase
    {
        private readonly ICommenServiceRepo _repository;
        private readonly IApiFunctions _api;
        private readonly IConfiguration cofig;
        private readonly IMyFunctions myFunctions;
        private readonly AppSettings _appSettings;

        private string connectionString;
        private string masterDBConnectionString;
        private string AppURL;
        private readonly IDataAccessLayer dLayer;



        public CommenServiceController(ICommenServiceRepo repository, IOptions<AppSettings> appSettings, IMyFunctions myFun, IApiFunctions api, IDataAccessLayer dl, IConfiguration conf)
        {
            _repository = repository;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            _appSettings = appSettings.Value;
            masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");
            connectionString = conf.GetConnectionString("SmartxConnection");
            AppURL = conf.GetConnectionString("AppURL");
            cofig = conf;
        }


        // [HttpGet("auth-user")]
        // public ActionResult AuthenticateUser(string reqType,string appName)
        // {
        //     try{
        //         int userid = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        //         int companyid = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid).Value);
        //         string companyname = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.StreetAddress).Value;
        //         string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
        //         string AppID = reqType=="all"?User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.System).Value:appName;

        //         var user = _repository.Authenticate(companyid,companyname,username,userid,reqType,AppID);

        //         if (user == null){ return StatusCode(403,_api.Response(403,"Unauthorized Access" )); }

        //             return Ok(user);
        //         }
        //         catch (Exception ex)
        //         {
        //            return StatusCode(403,_api.Error(User,ex));
        //         }  
        // }

        [HttpGet("auth-user")]
        public ActionResult AuthenticateUser(string reqType, int appID, int nCompanyID, string xCompanyName, string customerKey, string xVersion)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    if (appID != 2 && appID != 8 && appID != 18 && appID != 7)
                    {
                        if (!myFunctions.CheckVersion(xVersion, dLayer, con))
                        {
                            return Ok(_api.Warning("Please clear browser cache (press Ctrl+F5) and try again!!"));
                        }
                    }
                    // SortedList Params = new SortedList();
                    // string xAppVersion="";

                    // object AppVersion = dLayer.ExecuteScalar("select TOP 1 X_AppVersion from Gen_SystemSettings order by D_EntryDate DESC", Params, con);
                    // if(AppVersion!=null)xAppVersion=AppVersion.ToString();

                    // if(xAppVersion!="")
                    // {
                    //     if(xAppVersion!=xVersion)
                    //     {
                    //         return Ok(_api.Warning("Version error! Build version is "+xVersion+" and Database version is "+xAppVersion));
                    //     }
                    // }
                }

                string ipAddress = "";
                if (Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

                int GUserID = myFunctions.GetGlobalUserID(User);

                if (GUserID > 0 && reqType == "client")
                {
                    reqType = "app";
                }

                if (reqType == "all")
                {
                    int userid = myFunctions.GetUserID(User);
                    int companyid = myFunctions.GetCompanyID(User);
                    string companyname = myFunctions.GetCompanyName(User);
                    string username = myFunctions.GetUserLoginName(User);
                    int AppID = appID;


                    var user = _repository.Authenticate(companyid, companyname, username, userid, reqType, AppID, User.FindFirst(ClaimTypes.Uri)?.Value, myFunctions.GetClientID(User), myFunctions.GetGlobalUserID(User), ipAddress, myFunctions.GetLoginID(User));

                    if (user == null) { return Ok(_api.Error(User, "Unauthorized Access")); }

                    return Ok(_api.Success(user));
                }

                if (reqType == "switchCompany")
                {
                    int userid = 0;
                    int companyid = nCompanyID;
                    string companyname = xCompanyName;
                    string username = myFunctions.GetUserLoginName(User);
                    int AppID = appID;

                    var user = _repository.Authenticate(companyid, companyname, username, userid, reqType, AppID, User.FindFirst(ClaimTypes.Uri)?.Value, myFunctions.GetClientID(User), myFunctions.GetGlobalUserID(User), ipAddress, 0);

                    if (user == null) { return Ok(_api.Error(User, "Unauthorized Access")); }

                    return Ok(_api.Success(user));
                }
                else if (reqType == "app")
                {
                    // int userid = myFunctions.GetUserID(User);
                    int clientID = myFunctions.GetClientID(User);
                    int GlobalUserID = myFunctions.GetGlobalUserID(User);
                    string username = myFunctions.GetUserLoginName(User);
                    int companyid = myFunctions.GetCompanyID(User);
                    string companyname = myFunctions.GetCompanyName(User);
                    string activeDbUri = "SmartxConnection";
                    bool b_AppNotExist = false;
                    bool b_noUserCategoryExist = false;

                    try
                    {
                        using (SqlConnection olivCnn = new SqlConnection(masterDBConnectionString))
                        {
                            olivCnn.Open();
                            SortedList paramList = new SortedList();
                            paramList.Add("@nClientID", clientID);
                            paramList.Add("@xEmailID", username);
                            paramList.Add("@nGlobalUserID", GlobalUserID);


                            if (appID == 0)
                            {

                                object userType = dLayer.ExecuteScalar("SELECT isnull(N_UserType,0) as N_UserType FROM Users where N_ClientID=@nClientID and N_UserID=@nGlobalUserID", paramList, olivCnn);
                                if (myFunctions.getIntVAL(userType.ToString()) != 0)
                                {
                                    appID = myFunctions.getIntVAL(userType.ToString());
                                }
                            }
                            paramList.Add("@nAppID", appID);
                            object checkAppExisting = dLayer.ExecuteScalar("SELECT Count(N_AppID) as Count FROM ClientApps where N_ClientID=@nClientID and N_AppID=@nAppID", paramList, olivCnn);
                      
                            if (myFunctions.getIntVAL(checkAppExisting.ToString()) == 0 )
                            {
                                object isAdmin = dLayer.ExecuteScalar("SELECT Count(N_ClientID) as Count FROM clientMaster where N_ClientID=@nClientID and X_EmailID=@xEmailID", paramList, olivCnn);
                                if (myFunctions.getIntVAL(isAdmin.ToString()) == 0)
                                {
                                    return Ok(_api.Warning("App not registerd in your company"));
                                }

                                b_AppNotExist = true;

                                if (AppURL == null)
                                    return Ok(_api.Warning("App url not configured"));
                                paramList.Add("@xAppUrl", AppURL);
                                paramList.Add("@xDBUri", activeDbUri);
                                paramList.Add("@nUserLimit", 1);

                                int expDays = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_TrialPeriod from AppMaster where N_AppID=@nAppID", paramList, olivCnn).ToString());
                                DateTime expDate = DateTime.Today.AddDays(expDays);
                                paramList.Add("@dExpDate", expDate);
                                DateTime startDate = DateTime.Today;
                                paramList.Add("@dStartDate", startDate);

                                bool isAttachment = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select isnull(B_EnableAttachment,0) from AppMaster where N_AppID=@nAppID", paramList, olivCnn).ToString());
                                
                                paramList.Add("@isAttachment", isAttachment);


                                object totalCount = dLayer.ExecuteScalar("SELECT Count(N_AppID) as Count FROM ClientApps where N_ClientID=@nClientID", paramList, olivCnn);
                                if (myFunctions.getIntVAL(totalCount.ToString()) == 0)
                                {
                                    string appUpdate = "Update ClientMaster set N_DefaultAppID=@nAppID where N_ClientID=@nClientID";
                                    dLayer.ExecuteScalar(appUpdate, paramList, olivCnn);
                                }

                                int rows = dLayer.ExecuteNonQuery("insert into ClientApps select @nClientID,@nAppID,@xAppUrl,@xDBUri,@nUserLimit,0,'Service',max(N_RefID)+1,@dExpDate,0,null,@isAttachment,null,null,null,@dStartDate from ClientApps", paramList, olivCnn);
                                  if (rows> 0)
                                {
                                  string settingsUpdate = "Update GenSettings set N_Value= (SELECT N_Value + (select isnull(N_FreeUsers,0) from AppMaster where N_AppID=@nAppID) as N_Value FROM GenSettings where N_ClientID=@nClientID and X_Description='USER LIMIT') WHERE N_ClientID=@nClientID and X_Description='USER LIMIT'";
                                  dLayer.ExecuteScalar(settingsUpdate, paramList, olivCnn);
                                  string settingsempUpdate = "Update GenSettings set N_Value= (SELECT N_Value + (select isnull(N_FreeEmployees,0) from AppMaster where N_AppID=@nAppID) as N_Value FROM GenSettings where N_ClientID=@nClientID and X_Description='EMPLOYEE LIMIT') WHERE N_ClientID=@nClientID and X_Description='EMPLOYEE LIMIT'";
                                  dLayer.ExecuteScalar(settingsempUpdate, paramList, olivCnn);
                                }
                                //     else{
                                // //         if (appID != 6 && appID != 8)
                                // // {
                                // //         dLayer.ExecuteScalar(appUpdate, paramList, olivCnn);
                                // // }
                                //     }
                            }
                            else
                            {

                            }
                            connectionString = cofig.GetConnectionString(activeDbUri);
                            // if(companyid>0)
                            // {
                            using (SqlConnection cnn = new SqlConnection(connectionString))
                            {
                                cnn.Open();
                                if(companyid>0)
                                {

                                 object sqlText = dLayer.ExecuteScalar("Select ISNULL(x_LandingPage,'') AS x_LandingPage From Sec_UserApps Where N_CompanyID="+companyid+" and N_AppID="+appID+" and N_UserID="+myFunctions.GetUserID(User)+" and N_GlobalUserID="+GlobalUserID+"",paramList,cnn);
                                    if(sqlText==null){sqlText="";}
                                    dLayer.ExecuteNonQuery("delete from Sec_UserApps where N_CompanyID="+companyid+" and N_AppID="+appID+" and N_UserID="+myFunctions.GetUserID(User)+" and N_GlobalUserID="+GlobalUserID, paramList, cnn);
                                    //insert landing page wiht object
                                    dLayer.ExecuteNonQuery("insert into Sec_UserApps select "+companyid+",max(N_APPMappingID)+1,"+appID+","+myFunctions.GetUserID(User)+","+GlobalUserID+",'"+sqlText.ToString()+"' from Sec_UserApps", paramList, cnn);
                                }
                              if(companyid>0)
                              {
                              object userCategoryCount = dLayer.ExecuteScalar("SELECT Count(N_UserCategoryID) as Count FROM Sec_UserCategory where  N_AppID="+appID+" and N_CompanyID="+companyid+"", paramList, cnn);
                              if(myFunctions.getIntVAL(userCategoryCount.ToString()) == 0){ b_noUserCategoryExist=true;}
                              }
                                string companySql = "";
                                if (companyid > 0)
                                    companySql = " and Acc_Company.N_CompanyID=" + companyid + " ";
                                string cmpSql = "select Acc_Company.N_CompanyID,Acc_Company.X_CompanyName from Acc_Company LEFT OUTER JOIN Sec_User ON Acc_Company.N_CompanyID = Sec_User.N_CompanyID " +
                            " where Acc_Company.N_ClientID=@nClientID and  Sec_User.X_UserID=@xEmailID " + companySql + " order by B_IsDefault Desc ";
                                DataTable companyDt = dLayer.ExecuteDataTable(cmpSql, paramList, cnn);
                                if (companyDt.Rows.Count == 0)
                                {
                                    object isAdmin = dLayer.ExecuteScalar("SELECT Count(N_ClientID) as Count FROM clientMaster where N_ClientID=@nClientID and X_EmailID=@xEmailID", paramList, olivCnn);
                                    if (myFunctions.getIntVAL(isAdmin.ToString()) == 0)
                                    {
                                        return Ok(_api.Warning("Error"));
                                    }
                                    return Ok(_api.Error(User, "CompanyNotFound"));
                                }
                                companyid = myFunctions.getIntVAL(companyDt.Rows[0]["N_CompanyID"].ToString());
                                companyname = companyDt.Rows[0]["X_CompanyName"].ToString();
                                if (b_AppNotExist || b_noUserCategoryExist)
                                {
                                    paramList.Add("@companyid", companyid);
                                    int nUserCat = dLayer.ExecuteNonQuery("insert into Sec_UserCategory SELECT @companyid, MAX(N_UserCategoryID)+1, (select X_UserCategory from Sec_UserCategory where N_CompanyID=-1 and N_AppID=@nAppID and X_UserCategory <> 'Admin'), MAX(N_UserCategoryID)+1, 12, @nAppID,(select N_TypeID from Sec_User where N_CompanyID=" + companyid + " and N_UserID=" + myFunctions.GetUserID(User)+"),'' FROM Sec_UserCategory ", paramList, cnn);
                                    if (nUserCat <= 0)
                                    {
                                        return Ok(_api.Warning("User category creation failed"));
                                    }

                                    int nCatID = 0;
                                    object CatID = dLayer.ExecuteScalar("select MAX(N_UserCategoryID) from Sec_UserCategory", paramList, cnn);
                                    if (CatID != null)
                                    {
                                        nCatID = myFunctions.getIntVAL(CatID.ToString());
                                    }

                                    //Company User Update
                                    String xCatList = "", xAdmCatList = "";
                                    object catList = dLayer.ExecuteScalar("select X_UserCategoryList from Sec_User where N_CompanyID=" + companyid + " and N_UserID=" + myFunctions.GetUserID(User), paramList, cnn);
                                    if (catList != null)
                                    {
                                        xCatList = catList.ToString() + "," + nCatID.ToString();
                                    }

                                    dLayer.ExecuteScalar("Update Sec_User set X_UserCategoryList='" + xCatList + "' where N_CompanyID=" + companyid + " and N_UserID=" + myFunctions.GetUserID(User), cnn);
                                    //Company User Screen Permission Update
                                    int Prevrows = dLayer.ExecuteNonQuery("Insert into Sec_UserPrevileges (N_InternalID,N_UserCategoryID,N_menuID,B_Visible,B_Edit,B_Delete,B_Save,B_View)" +
                                                                        "Select ROW_NUMBER() over(order by N_InternalID)+(select MAX(N_InternalID) from Sec_UserPrevileges)," + nCatID + ",N_menuID,B_Visible,B_Edit,B_Delete,B_Save,B_View " +
                                                                        "from Sec_UserPrevileges inner join Sec_UserCategory on Sec_UserPrevileges.N_UserCategoryID = Sec_UserCategory.N_UserCategoryID where Sec_UserPrevileges.N_UserCategoryID = (-1*(@nAppID)) and N_CompanyID = -1", paramList, cnn);
                                    if (Prevrows <= 0)
                                    {
                                        return Ok(_api.Warning("Screen permission failed"));
                                    }

                                    //Admin User Screen Permission Update
                                    int nAdmCatID = 0;
                                    object AdmCatID = dLayer.ExecuteScalar("select N_UserCategoryID from Sec_UserCategory where N_CompanyID=" + companyid + " and X_UserCategory='Admin'", paramList, cnn);
                                    if (AdmCatID != null)
                                    {
                                        nAdmCatID = myFunctions.getIntVAL(AdmCatID.ToString());
                                    }
                                    int AdmPrevrows = dLayer.ExecuteNonQuery("Insert into Sec_UserPrevileges (N_InternalID,N_UserCategoryID,N_menuID,B_Visible,B_Edit,B_Delete,B_Save,B_View)" +
                                                                        "select ROW_NUMBER() over(order by n_menuid)+(select MAX(N_InternalID) from Sec_UserPrevileges)," + nAdmCatID + ",N_MenuID,1,1,1,1,null " +
                                                                        " from Sec_Menus where N_MenuID in (select N_MenuID from vw_SetAdminSettings where N_ModuleID =0 OR N_ModuleID in " +
                                                                        " (SELECT Sec_Menus.N_ParentMenuID FROM Sec_Menus INNER JOIN Sec_UserPrevileges ON Sec_Menus.N_MenuID = Sec_UserPrevileges.N_MenuID " +
                                                                        " WHERE Sec_UserPrevileges.N_UserCategoryID=(-1*(" + appID + ")) GROUP BY Sec_Menus.N_ParentMenuID)) " +
                                                                        " and N_MenuID NOT IN (select N_MenuID from Sec_UserPrevileges where N_UserCategoryID=" + nAdmCatID + ") GROUP BY N_MenuID ", paramList, cnn);
                                    if (AdmPrevrows <= 0)
                                    {
                                        return Ok(_api.Warning("Screen permission failed"));
                                    }

                                }

                            }
                            // }else{
                            //     return Ok(_api.Error(User,"CompanyNotFound"));
                            // }
                        }
                    }
                    catch (Exception ex)
                    {
                        return Ok(_api.Error(User, "Unauthorized Access"));
                    }
                    var user = _repository.Authenticate(companyid, companyname, username, 0, reqType, appID, User.FindFirst(ClaimTypes.Uri)?.Value, clientID, GlobalUserID, ipAddress, myFunctions.GetLoginID(User));

                    if (user == null) { return Ok(_api.Error(User, "Unauthorized Access")); }

                    return Ok(_api.Success(user));
                }
                else if (reqType == "client")
                {
                    SortedList Res = new SortedList();
                    int clientID = myFunctions.GetClientID(User);
                    int GlobalUserID = myFunctions.GetGlobalUserID(User);
                    string username = myFunctions.GetUserLoginName(User);
                    using (SqlConnection olivCnn = new SqlConnection(masterDBConnectionString))
                    {
                        olivCnn.Open();
                        string sql = "SELECT Users.N_UserID, Users.X_EmailID, Users.X_UserName, Users.N_ClientID, Users.X_UserID, Users.N_ActiveAppID, ClientApps.X_AppUrl,ClientApps.X_DBUri,ClientApps., AppMaster.X_AppName, ClientMaster.X_AdminUserID AS x_AdminUser,CASE WHEN Users.N_UserType=0 THEN 1 ELSE 0 end as isAdminUser FROM Users LEFT OUTER JOIN ClientMaster ON Users.N_ClientID = ClientMaster.N_ClientID LEFT OUTER JOIN ClientApps ON Users.N_ActiveAppID = ClientApps.N_AppID AND Users.N_ClientID = ClientApps.N_ClientID LEFT OUTER JOIN AppMaster ON ClientApps.N_AppID = AppMaster.N_AppID WHERE Users.x_UserID=@emailID and Users.N_ClientID=@nClientID ";
                        SortedList Params = new SortedList();
                        Params.Add("@emailID", username);
                        Params.Add("@nClientID", clientID);
                        DataTable output = dLayer.ExecuteDataTable(sql, Params, olivCnn);
                        if (output.Rows.Count == 0)
                        {
                            return Ok(_api.Error(User, "Unauthorized Access"));
                        }

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.PrimarySid,output.Rows[0]["N_UserID"].ToString()),
                        new Claim(ClaimTypes.PrimaryGroupSid,output.Rows[0]["N_ClientID"].ToString()),
                        new Claim(ClaimTypes.Email,output.Rows[0]["X_EmailID"].ToString()),
                        new Claim(ClaimTypes.Upn,username),
                        new Claim(ClaimTypes.Role,""),
                        new Claim(ClaimTypes.GroupSid,"0"),
                        new Claim(ClaimTypes.StreetAddress,""),
                        new Claim(ClaimTypes.Sid,"-1"),
                        new Claim(ClaimTypes.Version,"V0.1"),
                        new Claim(ClaimTypes.System,"0"),
                        new Claim(ClaimTypes.Uri,output.Rows[0]["X_DBUri"].ToString())

                    }),
                            Expires = DateTime.UtcNow.AddDays(2),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        };
                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        var reToken = generateRefreshToken();
                        SortedList tokenSet = new SortedList();

                        tokenSet.Add("Token", tokenHandler.WriteToken(token));
                        tokenSet.Add("Expiry", DateTime.UtcNow.AddDays(2));
                        tokenSet.Add("RefreshToken", reToken);
                        tokenSet.Add("n_AppID", appID);
                        dLayer.ExecuteScalar("Update Users set X_Token='" + reToken + "' where N_UserID=" + output.Rows[0]["N_UserID"].ToString(), olivCnn);

                        SortedList SLUser = new SortedList();
                        SLUser.Add("Token", tokenSet);
                        SLUser.Add("UserData", output);
                        Res.Add("UserInfo", SLUser);
                        Res.Add("Type", "Client");
                        Res.Add("StatusCode", 1);
                        Res.Add("Message", "Login Success");
                    }

                    return Ok(_api.Success(Res));

                }

                else
                {
                    return Ok(_api.Error(User, "Invalid Request"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, "Unauthorized Access"));
            }
        }

        [AllowAnonymous]
        [HttpGet("customer-login")]
        public ActionResult AuthenticateCustomer(string reqType, int appID, string customerKey)
        {
            try
            {
                string ipAddress = "";
                if (Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();


                if (reqType.ToLower() == "customer")
                {
                    SortedList Res = new SortedList();
                    string seperator = "$$";
                    string[] cred = myFunctions.DecryptStringFromUrl(customerKey, System.Text.Encoding.Unicode).Split(seperator);
                    int companyID = myFunctions.getIntVAL(cred[0]);
                    int nCustomerID = myFunctions.getIntVAL(cred[1]);
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string sql = "SELECT Acc_Company.N_CompanyID, Acc_Company.X_CompanyName, Sec_User.X_UserID, Sec_User.N_UserID, Acc_Company.N_ClientID FROM Inv_Customer LEFT OUTER JOIN Acc_Company ON Inv_Customer.N_CompanyID = Acc_Company.N_CompanyID RIGHT OUTER JOIN Sec_User ON Sec_User.N_CompanyID = Inv_Customer.N_CompanyID AND Sec_User.N_CustomerID = Inv_Customer.N_CustomerID WHERE Inv_Customer.N_CustomerID= " + nCustomerID;
                        SortedList Params = new SortedList();
                        DataTable output = dLayer.ExecuteDataTable(sql, conn);
                        if (output.Rows.Count == 0)
                        {
                            return Ok(_api.Error(User, "Unauthorized Access"));
                        }

                        DataRow dRow = output.Rows[0];

                        var user = _repository.Authenticate(myFunctions.getIntVAL(dRow["N_CompanyID"].ToString()), dRow["X_CompanyName"].ToString(), dRow["X_UserID"].ToString(), myFunctions.getIntVAL(dRow["N_UserID"].ToString()), reqType, appID, "", myFunctions.getIntVAL(dRow["N_ClientID"].ToString()), 0, ipAddress, myFunctions.GetLoginID(User));

                        if (user == null) { return Ok(_api.Error(User, "Unauthorized Access")); }

                        return Ok(_api.Success(user));

                    }


                }
                if (reqType.ToLower() == "vendor")
                {
                    SortedList Res = new SortedList();
                    string seperator = "$$";
                    string[] cred = myFunctions.DecryptStringFromUrl(customerKey, System.Text.Encoding.Unicode).Split(seperator);
                    int companyID = myFunctions.getIntVAL(cred[0]);
                    int nVendorID = myFunctions.getIntVAL(cred[1]);
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string sql = "SELECT Acc_Company.N_CompanyID, Acc_Company.X_CompanyName, Sec_User.X_UserID, Sec_User.N_UserID, Acc_Company.N_ClientID FROM Inv_Vendor LEFT OUTER JOIN Acc_Company ON Inv_Vendor.N_CompanyID = Acc_Company.N_CompanyID RIGHT OUTER JOIN Sec_User ON Sec_User.N_CompanyID = Inv_Vendor.N_CompanyID AND Sec_User.N_CustomerID = Inv_Vendor.N_VendorID WHERE Sec_User.N_LoginFlag=" + appID + " and Inv_Vendor.N_VendorID=" + nVendorID;
                        SortedList Params = new SortedList();
                        DataTable output = dLayer.ExecuteDataTable(sql, conn);
                        if (output.Rows.Count == 0)
                        {
                            return Ok(_api.Error(User, "Unauthorized Access"));
                        }

                        DataRow dRow = output.Rows[0];

                        var user = _repository.Authenticate(myFunctions.getIntVAL(dRow["N_CompanyID"].ToString()), dRow["X_CompanyName"].ToString(), dRow["X_UserID"].ToString(), myFunctions.getIntVAL(dRow["N_UserID"].ToString()), reqType, appID, "", myFunctions.getIntVAL(dRow["N_ClientID"].ToString()), 0, ipAddress, myFunctions.GetLoginID(User));

                        if (user == null) { return Ok(_api.Error(User, "Unauthorized Access")); }

                        return Ok(_api.Success(user));

                    }


                }
                else
                {
                    return Ok(_api.Error(User, "Invalid Request"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, "Unauthorized Access"));
            }
        }

        [AllowAnonymous]
        [HttpGet("refreshtoken")]
        public ActionResult RefreshToken(string token)
        {
            try
            {
                string ipAddress = "";
                if (Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

                int userid = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                int companyid = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid).Value);
                string companyname = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.StreetAddress).Value;
                string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
                int AppID = 0;

                var user = _repository.Authenticate(companyid, companyname, username, userid, "RefreshToken", AppID, "", 0, 0, ipAddress, myFunctions.GetLoginID(User));

                if (user == null) { return StatusCode(403, _api.Response(403, "Unauthorized Access")); }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.Error(User, ex));
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




    }
}