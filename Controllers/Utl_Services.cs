using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("utlServices")]
    [ApiController]



    public class Utl_Services : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Utl_Services(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


       [HttpGet("validateUser")]
        public ActionResult ValidateUser(int nFnYearID, int nBranchID)
        {


            int nLoginID = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string ipAddress = "";
                    if (Request.Headers.ContainsKey("X-Forwarded-For"))
                        ipAddress = Request.Headers["X-Forwarded-For"];
                    else
                        ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

                    SortedList logParams = new SortedList()
                    {
                        {"N_CompanyID",myFunctions.GetCompanyID(User)},
                        {"N_FnyearID",nFnYearID},
                        {"N_BranchId",nBranchID},
                        {"N_ActionID",1},
                        {"N_Type",1},
                        {"N_LoggedInID",myFunctions.GetLoginID(User)},
                        {"X_SystemName",ipAddress},
                        {"N_UserID",myFunctions.GetUserID(User)}
                    };
                    nLoginID = myFunctions.getIntVAL(dLayer.ExecuteScalarPro("SP_LoginDetailsInsert_Cloud", logParams, connection).ToString());

                }

                if(nLoginID==0)
                return Ok(api.Unauthorized("LogOut"));
                else
                return Ok(api.Success("Authorized"));



            }
            catch (Exception e)
            {
                return Ok(api.Unauthorized("LogOut"));
            }
        }


    }

}