using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("salesreceipt")]
    [ApiController]
    public class Inv_SalesReceipt : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Inv_SalesReceipt(IApiFunctions apiFun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apiFun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("list")]
        public ActionResult GetSalesReceipt(int? nCompanyId, int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_InvReceipt_Search where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId";
            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nFnYearId", nFnYearId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.NotFound("Sales Receipt Not Found"));
                }
                else
                {
                    return Ok(dt);
                }
            }
            catch (Exception e)
            {
                return BadRequest(api.ErrorResponse(e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetSalesReceiptDetails(int? nCompanyId, int nFnYearId,int nBranchId,string xInvoiceNo,int bAllBranchData)
        {
            DataTable MasterTable = new DataTable();


        try{
            SortedList mParamsList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},    
                        {"X_VoucherNo",xInvoiceNo},  
                        {"N_FnYearID",nFnYearId},    
                        {"N_BranchId",nBranchId}    
                    };
            using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTablePro("SP_InvSalesReceipt_Disp",mParamsList,connection);
                    MasterTable=api.Format(MasterTable,"Master");
                    if(MasterTable.Rows.Count==0){return Ok(api.NotFound("No data found"));}
                }
            return Ok(api.Ok(MasterTable));
        }catch (Exception e){
                return BadRequest(api.ErrorResponse(e));
        }
        }

    }
}