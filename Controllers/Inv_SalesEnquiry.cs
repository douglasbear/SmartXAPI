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
    [Route("salesenquiry")]
    [ApiController]
    public class Inv_SalesEnquiry : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly string connectionString;

        public Inv_SalesEnquiry(IApiFunctions apiFun, IDataAccessLayer dl, IConfiguration conf)
        {
            api = apiFun;
            dLayer = dl;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetSalesEnqList(int? nCompanyId, int nFnYearId, bool bAllBranchesData, int nBranchId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1", 0);
            Params.Add("@p2", nCompanyId);
            Params.Add("@p3", nFnYearId);
            string X_Crieteria;
            if (bAllBranchesData == true)
            { X_Crieteria = " where B_Processed=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3"; }
            else
            {
                X_Crieteria = " where B_Processed=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3 and N_BranchID=@p4";
                Params.Add("@p4", nBranchId);
            }

            string sqlCommandText = "select X_CRMCode,[Enquiry Date],X_ClientName,N_CompanyID,N_CRMID,N_SalesmanID,N_FnYearID,D_Date,N_BranchID,N_StatusID,B_Processed from vw_crmmaster " + X_Crieteria + " order by D_Date DESC,X_CRMCode";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(dt);
                }

            }
            catch (Exception e)
            {
                return StatusCode(403, api.Error(e));
            }
        }


    }
}