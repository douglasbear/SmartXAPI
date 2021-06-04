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
        private readonly IDataAccessLayer dLayer;



        public CommenServiceController(ICommenServiceRepo repository, IOptions<AppSettings> appSettings, IMyFunctions myFun, IApiFunctions api, IDataAccessLayer dl, IConfiguration conf)
        {
            _repository = repository;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            _appSettings = appSettings.Value;
            masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");
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
        //            return StatusCode(403,_api.Error(ex));
        //         }  
        // }

        [HttpGet("auth-user")]
        public ActionResult AuthenticateUser(string reqType, int appID, int nCompanyID, string xCompanyName)
        {
            try
            {

                if (reqType == "all")
                {
                    int userid = myFunctions.GetUserID(User);
                    int companyid = myFunctions.GetCompanyID(User);
                    string companyname = myFunctions.GetCompanyName(User);
                    string username = myFunctions.GetEmailID(User);
                    int AppID = appID;

                    var user = _repository.Authenticate(companyid, companyname, username, userid, reqType, AppID, User.FindFirst(ClaimTypes.Uri)?.Value, myFunctions.GetClientID(User), myFunctions.GetGlobalUserID(User));

                    if (user == null) { return Ok(_api.Error("Unauthorized Access")); }

                    return Ok(_api.Success(user));
                }
                if (reqType == "switchCompany")
                {
                    int userid = 0;
                    int companyid = nCompanyID;
                    string companyname = xCompanyName;
                    string username = myFunctions.GetEmailID(User);
                    int AppID = appID;

                    var user = _repository.Authenticate(companyid, companyname, username, userid, reqType, AppID, User.FindFirst(ClaimTypes.Uri)?.Value, myFunctions.GetClientID(User), myFunctions.GetGlobalUserID(User));

                    if (user == null) { return Ok(_api.Error("Unauthorized Access")); }

                    return Ok(_api.Success(user));
                }
                else if (reqType == "app")
                {
                    // int userid = myFunctions.GetUserID(User);
                    int clientID = myFunctions.GetClientID(User);
                    int GlobalUserID = myFunctions.GetGlobalUserID(User);
                    string username = myFunctions.GetEmailID(User);
                    int companyid = 0;
                    string companyname = "";

                    string activeDbUri = "ObConnection";


                    try
                    {
                        using (SqlConnection olivCnn = new SqlConnection(masterDBConnectionString))
                        {
                            olivCnn.Open();
                            SortedList paramList = new SortedList();
                            paramList.Add("@nClientID", clientID);
                            paramList.Add("@xEmailID", username);
                            paramList.Add("@nAppID", appID);
                            object checkAppExisting = dLayer.ExecuteScalar("SELECT Count(N_AppID) as Count FROM ClientApps where N_ClientID=@nClientID and N_AppID=@nAppID", paramList, olivCnn);
                            if (myFunctions.getIntVAL(checkAppExisting.ToString()) == 0)
                            {
                                object isAdmin = dLayer.ExecuteScalar("SELECT Count(N_ClientID) as Count FROM clientMaster where N_ClientID=@nClientID and X_EmailID=@xEmailID", paramList, olivCnn);
                                if (myFunctions.getIntVAL(isAdmin.ToString()) == 0)
                                {
                                    return Ok(_api.Warning("App not registerd in your company"));
                                }
                                // paramList.Add("@xAppUrl", "http://localhost:3000");
                                paramList.Add("@xAppUrl", "http://oscpl.smartxerp.com");
                                paramList.Add("@xDBUri", activeDbUri);
                                paramList.Add("@nUserLimit", appID);


                                int rows = dLayer.ExecuteNonQuery("insert into ClientApps select @nClientID,@nAppID,@xAppUrl,@xDBUri,@nUserLimit,0,'Service',max(N_RefID)+1 from ClientApps", paramList, olivCnn);
                                if (rows <= 0)
                                {
                                    return Ok(_api.Warning("App not registerd in your company"));
                                }
                            }
                            connectionString = cofig.GetConnectionString(activeDbUri);
                            using (SqlConnection cnn = new SqlConnection(connectionString))
                            {
                                cnn.Open();

                                DataTable companyDt = dLayer.ExecuteDataTable("select N_CompanyID,X_CompanyName from Acc_Company where N_ClientID=@nClientID", paramList, cnn);
                                if (companyDt.Rows.Count == 0)
                                {
                                    object isAdmin = dLayer.ExecuteScalar("SELECT Count(N_ClientID) as Count FROM clientMaster where N_ClientID=@nClientID and X_EmailID=@xEmailID", paramList, olivCnn);
                                    if (myFunctions.getIntVAL(isAdmin.ToString()) == 0)
                                    {
                                        return Ok(_api.Warning("Error"));
                                    }
                                    return Ok(_api.Error("CompanyNotFound"));
                                }
                                companyid = myFunctions.getIntVAL(companyDt.Rows[0]["N_CompanyID"].ToString());
                                companyname = companyDt.Rows[0]["X_CompanyName"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return Ok(_api.Error("Unauthorized Access"));
                    }
                    var user = _repository.Authenticate(companyid, companyname, username, 0, reqType, appID, User.FindFirst(ClaimTypes.Uri)?.Value, clientID, GlobalUserID);

                    if (user == null) { return Ok(_api.Error("Unauthorized Access")); }

                    return Ok(_api.Success(user));
                }
                else if (reqType == "client")
                {
                    SortedList Res = new SortedList();
                    int clientID = myFunctions.GetClientID(User);
                    int GlobalUserID = myFunctions.GetGlobalUserID(User);
                    string username = myFunctions.GetEmailID(User);
                    using (SqlConnection olivCnn = new SqlConnection(masterDBConnectionString))
                    {
                        olivCnn.Open();
                        string sql = "SELECT Users.N_UserID, Users.X_EmailID, Users.X_UserName, Users.N_ClientID, Users.N_ActiveAppID, ClientApps.X_AppUrl, ClientApps.X_DBUri, AppMaster.X_AppName FROM Users LEFT OUTER JOIN ClientApps ON Users.N_ActiveAppID = ClientApps.N_AppID AND Users.N_ClientID = ClientApps.N_ClientID LEFT OUTER JOIN AppMaster ON ClientApps.N_AppID = AppMaster.N_AppID where Users.x_EmailID=@emailID and Users.N_ClientID=@nClientID ";
                        SortedList Params = new SortedList();
                        Params.Add("@emailID", username);
                        Params.Add("@nClientID", clientID);
                        DataTable output = dLayer.ExecuteDataTable(sql, Params, olivCnn);
                        if (output.Rows.Count == 0)
                        {
                            return Ok(_api.Error("Unauthorized Access"));
                        }

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.PrimarySid,output.Rows[0]["N_UserID"].ToString()),
                        new Claim(ClaimTypes.PrimaryGroupSid,output.Rows[0]["N_ClientID"].ToString()),
                        new Claim(ClaimTypes.Email,output.Rows[0]["X_EmailID"].ToString()),
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
                    return Ok(_api.Error("Invalid Request"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error("Unauthorized Access"));
            }
        }

        [AllowAnonymous]
        [HttpGet("refreshtoken")]
        public ActionResult RefreshToken(string token)
        {
            try
            {
                int userid = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                int companyid = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid).Value);
                string companyname = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.StreetAddress).Value;
                string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
                int AppID = 0;

                var user = _repository.Authenticate(companyid, companyname, username, userid, "RefreshToken", AppID, "", 0, 0);

                if (user == null) { return StatusCode(403, _api.Response(403, "Unauthorized Access")); }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.Error(ex));
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