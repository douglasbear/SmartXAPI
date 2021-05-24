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

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("services")]
    [ApiController]
    public class CommenServiceController : ControllerBase
    {
        private readonly ICommenServiceRepo _repository;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private string connectionString;
        private readonly IDataAccessLayer dLayer;



        public CommenServiceController(ICommenServiceRepo repository, IMyFunctions myFun, IApiFunctions api, IDataAccessLayer dl, IConfiguration conf)
        {
            _repository = repository;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
        }


        // [HttpGet("auth-user")]
        // public ActionResult AuthenticateUser(string reqType,string appName)
        // {
        //     try{
        //         int userid = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        //         int companyid = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid).Value);
        //         string companyname = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.StreetAddress).Value;
        //         string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
        //         string AppType = reqType=="all"?User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.System).Value:appName;

        //         var user = _repository.Authenticate(companyid,companyname,username,userid,reqType,AppType);

        //         if (user == null){ return StatusCode(403,_api.Response(403,"Unauthorized Access" )); }

        //             return Ok(user);
        //         }
        //         catch (Exception ex)
        //         {
        //            return StatusCode(403,_api.Error(ex));
        //         }  
        // }

        [HttpGet("auth-user")]
        public ActionResult AuthenticateUser(string reqType, string appName)
        {
            connectionString = myFunctions.GetConnectionString(User);
            try
            {

                if (reqType == "all")
                {
                    int userid = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                    int companyid = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid).Value);
                    string companyname = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.StreetAddress).Value;
                    string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
                    string AppType = reqType == "all" ? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.System).Value : appName;

                    var user = _repository.Authenticate(companyid, companyname, username, userid, reqType, AppType,User.FindFirst(ClaimTypes.Uri)?.Value,myFunctions.GetClientID(User),myFunctions.GetGlobalUserID(User));

                    if (user == null) { return StatusCode(403, _api.Response(403, "Unauthorized Access")); }

                    return Ok(user);
                }
                else
                {
                    int userid = myFunctions.GetUserID(User);
                    int clientID = myFunctions.GetClientID(User);
                    int GlobalUserID = myFunctions.GetGlobalUserID(User);
                    string username = myFunctions.GetEmailID(User);
                    int companyid = 0;
                    string companyname = "";
                    try
                    {
                        using (SqlConnection cnn = new SqlConnection(connectionString))
                        {
                            cnn.Open();
                            SortedList paramList = new SortedList();
                            paramList.Add("@nClientID", clientID);
                            DataTable companyDt = dLayer.ExecuteDataTable("select N_CompanyID,X_CompanyName from Acc_Company where N_ClientID=@nClientID", paramList, cnn);
                            if (companyDt.Rows.Count == 0)
                            {
                                return Ok("Company Not Found");
                            }
                            companyid = myFunctions.getIntVAL(companyDt.Rows[0]["N_CompanyID"].ToString());
                            companyname = companyDt.Rows[0]["X_CompanyName"].ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        return Ok();
                    }
                    var user = _repository.Authenticate(companyid, companyname, username, userid, reqType, appName,User.FindFirst(ClaimTypes.Uri)?.Value,clientID,GlobalUserID);

                    if (user == null) { return StatusCode(403, _api.Response(403, "Unauthorized Access")); }

                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.Error(ex));
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
                string AppType = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.System).Value;

                var user = _repository.Authenticate(companyid, companyname, username, userid, "RefreshToken", AppType,"",0,0);

                if (user == null) { return StatusCode(403, _api.Response(403, "Unauthorized Access")); }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.Error(ex));
            }
        }


    }
}