using SmartxAPI.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using SmartxAPI.Profiles;
using System.Text;
using System.Security.Cryptography;

namespace SmartxAPI.Controllers
{

    [Route("clients")]
    [ApiController]
    public class Clients : ControllerBase
    {
        private readonly ICommenServiceRepo _repository;
        private readonly IApiFunctions _api;
        private readonly AppSettings _appSettings;
        private readonly IConfiguration config;
        private readonly IMyFunctions myFunctions;
        private readonly IDataAccessLayer dLayer;
        private readonly string connectionString;
        private readonly string masterDBConnectionString;

        public Clients(ICommenServiceRepo repository, IOptions<AppSettings> appSettings, IApiFunctions api, IMyFunctions myFun, IDataAccessLayer dl, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            _appSettings = appSettings.Value;
            connectionString = conf.GetConnectionString("SmartxConnection");
            masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");
            config = conf;
            _repository = repository;
        }
        [HttpPost("register")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable, UserTable, AppTable;
                MasterTable = ds.Tables["master"];
                UserTable = ds.Tables["user"];
                // AppTable = ds.Tables["app"];

                string pwd = UserTable.Rows[0]["x_Password"].ToString();
                DataRow MasterRow = MasterTable.Rows[0];

                string email = MasterRow["x_EmailID"].ToString();


                using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;

                    transaction = connection.BeginTransaction();

                    SortedList paramList = new SortedList();
                    paramList.Add("@emailID", email);
                    int count = myFunctions.getIntVAL(dLayer.ExecuteScalar("select count(1) from Users where x_EmailID=@emailID", paramList, connection, transaction).ToString());
                    if (count > 0)
                    {
                        return Ok(_api.Warning("Email id already exists !!!"));
                    }
                    string Password = myFunctions.EncryptString(pwd);
                    MasterTable.Rows[0]["b_Inactive"] = true;
                    MasterTable.Rows[0]["n_UserLimit"] = 1;
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_AdminUserID", typeof(string), email);

                    int ClientID = dLayer.SaveData("ClientMaster", "N_ClientID", MasterTable, connection, transaction);
                    if (ClientID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Something went wrong"));
                    }


                    // AppTable.Rows[0]["N_ClientID"] = ClientID;
                    UserTable.Rows[0]["b_Inactive"] = true;

                    // AppTable = myFunctions.AddNewColumnToDataTable(AppTable, "X_DBUri", typeof(string), ConnString);
                    // AppTable = myFunctions.AddNewColumnToDataTable(AppTable, "N_UserLimit", typeof(int), 5);
                    // AppTable = myFunctions.AddNewColumnToDataTable(AppTable, "N_RefID", typeof(int), 0);
                    // AppTable.AcceptChanges();
                    // int N_RefID = dLayer.SaveData("ClientApps", "N_RefID", AppTable, connection, transaction);
                    // if (N_RefID <= 0)
                    // {
                    //     transaction.Rollback();
                    //     return Ok(_api.Error(User,"Something went wrong"));
                    // }

                    UserTable.Rows[0]["N_ClientID"] = ClientID;
                    UserTable.Rows[0]["x_Password"] = Password;
                    UserTable.Rows[0]["b_EmailVerified"] = false;
                    UserTable.Rows[0]["b_Inactive"] = true;
                    UserTable = myFunctions.AddNewColumnToDataTable(UserTable, "N_ActiveAppID", typeof(int), 0);
                    UserTable = myFunctions.AddNewColumnToDataTable(UserTable, "X_UserID", typeof(string), email);
                    UserTable = myFunctions.AddNewColumnToDataTable(UserTable, "N_UserType", typeof(int), 0);
                    UserTable = myFunctions.AddNewColumnToDataTable(UserTable, "N_LoginType", typeof(int), 2);


                    int UserID = dLayer.SaveData("Users", "n_UserID", UserTable, connection, transaction);
                    if (UserID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Something went wrong"));
                    }

                    string Sql = "SELECT (select isnull(max(N_UserID),0) from Users)+1 , REPLACE (X_EmailID, '@', '_SpAdmin@'), REPLACE (X_EmailID, '@', '_SpAdmin@'), N_ClientID, N_ActiveAppID, '.', '', 0, 1, REPLACE (X_EmailID, '@', '_SpAdmin@'), N_UserType,1 FROM Users WHERE N_UserID ="+UserID;
                    int output = dLayer.ExecuteNonQuery(Sql, connection, transaction);
                        
                    transaction.Commit();
                }
                string ipAddress = "";
                if (Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

                SortedList Res = new SortedList();
                using (SqlConnection cnn = new SqlConnection(masterDBConnectionString))
                {
                    cnn.Open();
                    Res = Login(email, pwd, "Registration", 0, ipAddress, cnn);
                    if (Res["StatusCode"].ToString() == "0")
                    {
                        return Ok(_api.Error(User, Res["Message"].ToString()));
                    }

                    Res["Message"] = "Client Registration Success";
                    Res["AppStatus"] = "Registered";
                }


                return Ok(_api.Success(Res, Res["Message"].ToString()));
            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.Error(User, ex));
            }
        }

        [HttpPost("login")]
        public ActionResult Authenticate([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    DataTable UserTable = ds.Tables["user"];
                    var password = UserTable.Rows[0]["password"].ToString();
                    var emailID = UserTable.Rows[0]["emailID"].ToString();
                    int appType = myFunctions.getIntVAL(UserTable.Rows[0]["appType"].ToString());
                    var version = UserTable.Rows[0]["version"].ToString();

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        if(appType!=2 && appType!=8)
                        {
                            if (!myFunctions.CheckVersion(version, dLayer, con))
                            {
                                return Ok(_api.Warning("Please clear browser cache (press Ctrl+F5) and try again!!"));
                            }
                        }
                    }

                    if (emailID == null || password == null) { return Ok(_api.Warning("Username or password is incorrect")); }

                    string ipAddress = "";
                    if (Request.Headers.ContainsKey("X-Forwarded-For"))
                        ipAddress = Request.Headers["X-Forwarded-For"];
                    else
                        ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

                    SortedList Res = Login(emailID, password, "Login", appType, ipAddress, connection);
                    if (Res["StatusCode"].ToString() == "0")
                    {
                        return Ok(_api.Error(User, Res["Message"].ToString()));
                    }

                    return Ok(_api.Success(Res));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }


        private SortedList Login(string emailID, string password, string Type, int appType, string ipAddress, SqlConnection cnn)
        {
            SortedList Res = new SortedList();

            try
            {
                // using (SqlConnection cnn = new SqlConnection(masterDBConnectionString))
                // {
                //     cnn.Open();
                password = myFunctions.EncryptString(password);
                string sql = "SELECT Users.N_UserID, Users.X_EmailID, Users.X_UserName, Users.N_ClientID, Users.X_UserID, Users.N_ActiveAppID, ClientApps.X_AppUrl,ClientApps.X_DBUri, AppMaster.X_AppName, ClientMaster.X_AdminUserID AS x_AdminUser,CASE WHEN Users.N_UserType=0 THEN 1 ELSE 0 end as isAdminUser,isnull(N_UserType,0) as N_UserType FROM Users LEFT OUTER JOIN ClientMaster ON Users.N_ClientID = ClientMaster.N_ClientID LEFT OUTER JOIN ClientApps ON Users.N_ActiveAppID = ClientApps.N_AppID AND Users.N_ClientID = ClientApps.N_ClientID LEFT OUTER JOIN AppMaster ON ClientApps.N_AppID = AppMaster.N_AppID WHERE (Users.X_UserID =@emailID and Users.x_Password=@xPassword)";
                SortedList Params = new SortedList();
                Params.Add("@emailID", emailID);
                Params.Add("@xPassword", password);
                DataTable output = dLayer.ExecuteDataTable(sql, Params, cnn);
                if (output.Rows.Count == 0)
                {
                    Res.Add("Message", "Invalid Username or Password!");
                    Res.Add("StatusCode", 0);
                    return Res;
                }
                if (appType > 0)
                {
                    output.Rows[0]["N_ActiveAppID"] = appType;
                }else{
                    if(myFunctions.getIntVAL(output.Rows[0]["N_UserType"].ToString())!=0 &&  (myFunctions.getIntVAL(output.Rows[0]["N_ActiveAppID"].ToString())==0 ||  myFunctions.getIntVAL(output.Rows[0]["N_ActiveAppID"].ToString()) ==null )  )
                    {
                        output.Rows[0]["N_ActiveAppID"] = myFunctions.getIntVAL(output.Rows[0]["N_UserType"].ToString());
                    }
                }

                if (Type == "Login" && (output.Rows[0]["N_ActiveAppID"].ToString() != null && output.Rows[0]["N_ActiveAppID"].ToString() != "0"))
                {
                    int companyid = 0;
                    string companyname = "";
                    // string uri = output.Rows[0]["X_DBUri"].ToString();
                    string uri = "SmartxConnection";

                    using (SqlConnection connection = new SqlConnection(config.GetConnectionString(uri)))
                    {
                        connection.Open();
                        SortedList paramList = new SortedList();
                        paramList.Add("@nClientID", myFunctions.getIntVAL(output.Rows[0]["N_ClientID"].ToString()));
                        paramList.Add("@emailID", emailID);
                        string sqlCompany = "SELECT Acc_Company.N_CompanyID, Acc_Company.X_CompanyName FROM Acc_Company LEFT OUTER JOIN Sec_User ON Acc_Company.N_CompanyID = Sec_User.N_CompanyID  where Acc_Company.N_ClientID=@nClientID and Sec_User.X_UserID=@emailID  order by B_IsDefault Desc";

                        DataTable companyDt = dLayer.ExecuteDataTable(sqlCompany, paramList, connection);
                        if (companyDt.Rows.Count == 0)
                        {
                            Res.Add("Message", "Something went wrong.");
                            Res.Add("StatusCode", 0);
                            return Res;
                        }
                        companyid = myFunctions.getIntVAL(companyDt.Rows[0]["N_CompanyID"].ToString());
                        companyname = companyDt.Rows[0]["X_CompanyName"].ToString();
                    }
                    int nClientID = myFunctions.getIntVAL(output.Rows[0]["N_ClientID"].ToString());
                    int nGlobalUserID = myFunctions.getIntVAL(output.Rows[0]["N_UserID"].ToString());
                    var user = _repository.Authenticate(companyid, companyname, emailID, 0, "all", myFunctions.getIntVAL(output.Rows[0]["N_ActiveAppID"].ToString()), uri, nClientID, nGlobalUserID, ipAddress, 0);
                    Res.Add("UserInfo", user);
                    Res.Add("StatusCode", 1);
                    Res.Add("Type", "User");
                    Res.Add("Message", "Login Success");
                    sendLicenseReminder(nClientID);
                    return Res;
                }


                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.PrimarySid,output.Rows[0]["N_UserID"].ToString()),
                        new Claim(ClaimTypes.PrimaryGroupSid,output.Rows[0]["N_ClientID"].ToString()),
                        new Claim(ClaimTypes.Email,output.Rows[0]["X_EmailID"].ToString()),
                        new Claim(ClaimTypes.UserData,output.Rows[0]["X_UserID"].ToString()),
                        new Claim(ClaimTypes.Upn,emailID),
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
                tokenSet.Add("n_AppID", "0");
                dLayer.ExecuteScalar("Update Users set X_Token='" + reToken + "' where N_UserID=" + output.Rows[0]["N_UserID"].ToString(), cnn);

                SortedList User = new SortedList();
                User.Add("Token", tokenSet);
                User.Add("UserData", output);
                Res.Add("UserInfo", User);
                Res.Add("Type", "Client");
                Res.Add("StatusCode", 1);
                Res.Add("Message", "Login Success");
                // }
                return Res;
            }
            catch (Exception ex)
            {
                if (ex.Message == "InactiveUser")
                    Res.Add("Message", "User Inactive");
                else
                if (ex.Message == "Your Subscription Expired")
                    Res.Add("Message", "Oops!!.. Your Subscription Expired");
                else
                if (ex.Message == "Unauthorized Access")
                    Res.Add("Message", "Unauthorized Access");
                else
                if (ex.Message == "Login Failed")
                    Res.Add("Message", "Login Failed");
                else
                    Res.Add("Message", "Something went wrong..");


                Res.Add("StatusCode", 0);
                return Res;
            }
        }

        private bool sendLicenseReminder(int nClientID)
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(masterDBConnectionString))
                {
                    cnn.Open();
                    int DaysToExpire = myFunctions.getIntVAL(dLayer.ExecuteScalar("select isnull(DATEDIFF(day, GETDATE(),min(D_ExpiryDate)),0) as expiry from ClientApps where N_ClientID=" + nClientID, cnn).ToString());

                    if (DaysToExpire <= 10)
                    {
                        object LastExpiryReminder = myFunctions.getIntVAL(dLayer.ExecuteScalar("select isnull(DATEDIFF(day, GETDATE(),min(D_LastExpiryReminder)),5) as expiry from ClientApps where N_ClientID=" + nClientID, cnn).ToString());
                        if (Math.Abs(myFunctions.getIntVAL(LastExpiryReminder.ToString())) >= 5)
                        {
                            string xClientName = dLayer.ExecuteScalar("select X_ClientName from ClientMaster where N_ClientID=" + nClientID, cnn).ToString();
                            string xEmail = dLayer.ExecuteScalar("select X_EmailID from ClientMaster where N_ClientID=" + nClientID, cnn).ToString();
                            string xExpiryInfo = DaysToExpire > 0 ? "expiring within " + DaysToExpire + " days " : "expired";
                            string dExpiryDate = dLayer.ExecuteScalar("select cast(FORMAT (min(D_ExpiryDate), 'dd MMMM yyyy') as varchar)  as expiry from ClientApps where N_ClientID=" + nClientID, cnn).ToString();
                            string xProductName = dLayer.ExecuteScalar("SELECT X_AppDescription FROM AppMaster INNER JOIN ClientMaster ON ClientMaster.N_DefaultAppID = AppMaster.N_AppID where N_ClientID=" + nClientID, cnn).ToString();
                            string EmailBody = "<div><div><div>Hello " + xClientName + ",</div>&nbsp;<div>Your subscription for " + xProductName + " is " + xExpiryInfo + ". It is time to renew.</div>&nbsp;<div>It is important to keep your subscription up to date in order to continue using service.</div>&nbsp;<div>If you wish to renew your subscription, contact your salesperson.</div>&nbsp;<div>Your license expires on: " + dExpiryDate + ".</div>&nbsp;<div>Your product name: " + xProductName + ".</div>&nbsp;<div>Best regards,<br /><br /><span style=\"background-color:rgb(255, 255, 255); color:rgb(44, 106, 246); font-family:sans-serif\">Olivo Cloud Solutions</span><br /><br /><span style=\"background-color:rgb(244, 245, 246); color:rgb(134, 137, 142); font-family:sans-serif; font-size:14px\">Copyright &copy; 2021 Olivo Cloud Solutions, All rights reserved.</span><span style=\"color:rgb(0, 0, 0)\"> </span></div></div><div>&nbsp;</div></div>";
                            string EmailSubject = "Renewal Reminder";
                            if (myFunctions.SendMail(xEmail, EmailBody, EmailSubject, dLayer, 1, 1, 1))
                            {
                                dLayer.ExecuteScalar("Update ClientApps set D_LastExpiryReminder=GETDATE()  where N_ClientID=" + nClientID, cnn);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
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




        [HttpGet("language")]
        public ActionResult LanguageList()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection olivoCon = new SqlConnection(masterDBConnectionString))
                {
                    olivoCon.Open();
                    dt = dLayer.ExecuteDataTable("Select * from LanguageMaster", olivoCon);

                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(dt));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("country")]
        public ActionResult CountryList()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection olivoCon = new SqlConnection(masterDBConnectionString))
                {
                    olivoCon.Open();
                    dt = dLayer.ExecuteDataTable("Select * from CountryMaster", olivoCon);

                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(dt));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("details")]
        public ActionResult clientDetails(int nClientID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "select * from vw_ClientDetails where N_ClientID=@nClientID";
            Params.Add("@nClientID", nClientID);
            try
            {
                using (SqlConnection olivoCon = new SqlConnection(masterDBConnectionString))
                {
                    olivoCon.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, olivoCon);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
    }
}