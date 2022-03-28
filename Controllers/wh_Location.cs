using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("location")]
    [ApiController]
    public class Inv_WhLocation : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;



        public Inv_WhLocation(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


           [HttpGet("update")]
        public ActionResult Locationpdate(int nMainLocationID,int nLocationID,int nCompanyId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
             nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
             sqlCommandText = "update Inv_Location set N_MainLocationID=@p2 where N_CompanyID=@p1 and N_LocationID=@p3";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nMainLocationID);
            Params.Add("@p3", nLocationID);
           


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dLayer.ExecuteNonQuery(sqlCommandText, Params, connection);
                    if(nMainLocationID>0)
                        dLayer.ExecuteNonQuery("update Inv_Location set N_MainLocationID=@p2 where N_CompanyID=@p1 and N_LocationID=@p3", Params, connection);
                }
                 return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
    }
}