using SmartxAPI.Data;
using SmartxAPI.Dtos.Login;
using Microsoft.AspNetCore.Mvc;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("apps")]
    [ApiController]
    public class Apps : ControllerBase
    {
        private readonly ISec_UserRepo _repository;
        private readonly IApiFunctions _api;

        private readonly IMyFunctions myFunctions;
        private readonly IDataAccessLayer dLayer;
        private readonly string olivoClientConnectionString;

        public Apps(ISec_UserRepo repository, IApiFunctions api, IMyFunctions myFun, IDataAccessLayer dl, IConfiguration conf)
        {
            _repository = repository;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            olivoClientConnectionString = conf.GetConnectionString("OlivoClientConnection");
        }
       
        [AllowAnonymous]
        [HttpGet("list")]
        public ActionResult GetAllApps()
        {
            DataTable dt = new DataTable();
            string sqlCommandText = "select * from AppMaster where B_Inactive =0 order by N_AppID";
            try
            {
                using (SqlConnection connection = new SqlConnection(olivoClientConnectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, connection);
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
                return Ok(_api.Error(e));
            }

        }

        [HttpGet("login")]
        public ActionResult AutoLogin(int appID,int nCompanyID)
        {
            DataTable dt = new DataTable();
                    SortedList res=new SortedList();
            SortedList appParams = new SortedList();
            appParams.Add("@nClientID",myFunctions.GetClientID(User));
            appParams.Add("@nAppID",appID);
            string sqlCommandText = "select X_DBUri,X_AppUrl from ClientApps where N_ClientID=@nClientID and N_AppID=@nAppID";
            try
            {
                using (SqlConnection connection = new SqlConnection(olivoClientConnectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText,appParams, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    // App not yet registerd...
                    res.Add("AppStatus","NotRegistered");
                    res.Add("AppID",appID);
                }
                else
                {
                    res.Add("AppStatus","Registered");
                    res.Add("AppID",appID);


                }
                    return Ok(_api.Success(res));

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }

        }
    }       
}