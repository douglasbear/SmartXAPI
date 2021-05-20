using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("Assetrevaluation")]
    [ApiController]
    public class Ass_Purchase : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Ass_Purchase(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [AllowAnonymous]
        [HttpGet("list")]
        public ActionResult GetAssetList(int nCompanyId, int nFnYearId, int nBranchId, bool bAllBranchesData)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string X_Crieteria = "";
            if (bAllBranchesData == true)
            { X_Crieteria = " where B_Inactive=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3"; }
            else
            {
                X_Crieteria = " where B_Inactive=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3  and (N_BranchID=@p4 or N_BranchID=@p5)";
                Params.Add("@p4", 0);
                Params.Add("@p5", nBranchId);
            }
            string sqlCommandText = "select * from vw_InvAssetInventoryInvoiceNo_Search " + X_Crieteria + " " + " order by [Invoice No]";
            Params.Add("@p1", 0);
            Params.Add("@p2", nCompanyId); 
            Params.Add("@p3", nFnYearId);

 
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return Ok( _api.Error(e));
            }
        }




        


    }
}