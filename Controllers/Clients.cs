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
                        return Ok(_api.Error("Something went wrong"));
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
                    //     return Ok(_api.Error("Something went wrong"));
                    // }

                    UserTable.Rows[0]["N_ClientID"] = ClientID;
                    UserTable.Rows[0]["x_Password"] = Password;
                    UserTable.Rows[0]["b_EmailVerified"] = false;
                    UserTable.Rows[0]["b_Inactive"] = true;
                    UserTable = myFunctions.AddNewColumnToDataTable(UserTable, "N_ActiveAppID", typeof(int), 0);
                    UserTable = myFunctions.AddNewColumnToDataTable(UserTable, "X_UserID", typeof(string), email);


                    int UserID = dLayer.SaveData("Users", "n_UserID", UserTable, connection, transaction);
                    if (UserID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Something went wrong"));
                    }

                    transaction.Commit();
                }
                SortedList Res = Login(email, pwd, "Registration", 0);
                if (Res["StatusCode"].ToString() == "0")
                {
                    return Ok(_api.Error(Res["Message"].ToString()));
                }

                Res["Message"] = "Client Registration Success";
                Res["AppStatus"] = "Registered";


                return Ok(_api.Success(Res, Res["Message"].ToString()));
            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.Error(ex));
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


                    if (emailID == null || password == null) { return Ok(_api.Warning("Username or password is incorrect")); }

                    SortedList Res = Login(emailID, password, "Login", appType);
                    if (Res["StatusCode"].ToString() == "0")
                    {
                        return Ok(_api.Error(Res["Message"].ToString()));
                    }

                    return Ok(_api.Success(Res));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }


        private SortedList Login(string emailID, string password, string Type, int appType)
        {
            SortedList Res = new SortedList();

            try
            {
                using (SqlConnection cnn = new SqlConnection(masterDBConnectionString))
                {
                    cnn.Open();
                    password = myFunctions.EncryptString(password);
                    string sql = "SELECT Users.N_UserID, Users.X_EmailID, Users.X_UserName, Users.N_ClientID, Users.X_UserID, Users.N_ActiveAppID, ClientApps.X_AppUrl,ClientApps.X_DBUri, AppMaster.X_AppName, ClientMaster.X_AdminUserID AS x_AdminUser,CASE WHEN ClientMaster.X_AdminUserID=Users.X_UserID THEN 1 ELSE 0 end as isAdminUser FROM Users LEFT OUTER JOIN ClientMaster ON Users.N_ClientID = ClientMaster.N_ClientID LEFT OUTER JOIN ClientApps ON Users.N_ActiveAppID = ClientApps.N_AppID AND Users.N_ClientID = ClientApps.N_ClientID LEFT OUTER JOIN AppMaster ON ClientApps.N_AppID = AppMaster.N_AppID WHERE (Users.X_UserID =@emailID and Users.x_Password=@xPassword)";
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
                    }

                    if (Type == "Login" && (output.Rows[0]["N_ActiveAppID"].ToString() != null && output.Rows[0]["N_ActiveAppID"].ToString() != "0"))
                    {
                        int companyid = 0;
                        string companyname = "";
                        string uri = output.Rows[0]["X_DBUri"].ToString();

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
                        var user = _repository.Authenticate(companyid, companyname, emailID, 0, "all", myFunctions.getIntVAL(output.Rows[0]["N_ActiveAppID"].ToString()), uri, nClientID, nGlobalUserID);
                        Res.Add("UserInfo", user);
                        Res.Add("StatusCode", 1);
                        Res.Add("Type", "User");
                        Res.Add("Message", "Login Success");
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
                    return Res;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "InactiveUser")
                    Res.Add("Message", "User Inactive");
                else
                    Res.Add("Message", "Something went wrong..");


                Res.Add("StatusCode", 0);
                return Res;
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
                return Ok(_api.Error(e));
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
                return Ok(_api.Error(e));
            }
        }

    }
}