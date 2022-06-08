using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("invOpeningBalance")]
    [ApiController]
    public class inv_OpeningBalance : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public inv_OpeningBalance(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 208;
        }


          [HttpGet("CustList")]
        public ActionResult customerList(int N_Flag, int nFnYearID, int nBranchID,int nCompanyID )
        {
            DataTable mst = new DataTable();
            DataTable dt = new DataTable();
            DataTable node = new DataTable();
             nCompanyID = myFunctions.GetCompanyID(User);
           

            SortedList ProParams = new SortedList();
            ProParams.Add("N_CompanyID", nCompanyID);
            ProParams.Add("N_Flag", N_Flag);
            ProParams.Add("N_FnYearID", nFnYearID);
            ProParams.Add("N_BranchID",nBranchID);
           
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                     dt = dLayer.ExecuteDataTablePro("Sp_Inv_OpeningBalance_View", ProParams, connection,transaction);
                    if (dt.Rows.Count == 0)
                    { 
                         transaction.Rollback();
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        SortedList Output = new SortedList();
                        Output.Add("master",mst);
                        Output.Add("details",dt);
                        transaction.Commit();
                        return Ok(_api.Success(Output));
                    }
                }
            }
            catch (Exception e)
            {
                
                return Ok(_api.Error(User,e));
            }
        }
    }
}
