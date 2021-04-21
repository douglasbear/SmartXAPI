using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("dayClosing")]
    [ApiController]
    public class Acc_DayClosing : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Acc_DayClosing(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 763;
        }

        [HttpGet("salesSummary")]
        public ActionResult GetCashSummaryDetails(int nCompanyID,int nFnYearID,int nBranchID,DateTime dTransDate)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nFnYearID);
            Params.Add("@p3",nBranchID);
            Params.Add("@p4",dTransDate);
            object Total = "";
            SortedList Result = new SortedList();
            //string sqlCommandText="Select N_CompanyID,SUM(N_AmountDr)as N_AmountDr,SUM(N_AmountCr) as N_AmountCr,D_TransDate,N_BranchID,N_FnYearID,X_CustomerName from  Vw_DailySalesSummary where N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 and D_TransDate=@p4 group by N_CompanyID,D_TransDate,N_BranchID,N_FnYearID,X_CustomerName union Select N_CompanyID,SUM(N_TaxAmtF)as N_AmountDr,0 as N_AmountCr,D_SalesDate,N_BranchID,N_FnYearID,'Total VAT' from  Inv_Sales where N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 and D_SalesDate=@p4  group by N_CompanyID,D_SalesDate,N_BranchID,N_FnYearID union Select N_CompanyID,COUNT(N_SalesId)as N_AmountDr,0 as N_AmountCr,D_SalesDate,N_BranchID,N_FnYearID,'Count' from  Inv_Sales where N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 and D_SalesDate=@p4  group by N_CompanyID,D_SalesDate,N_BranchID,N_FnYearID";
            string sqlCommandText="Select N_CompanyID,SUM(N_AmountDr)as N_AmountDr,SUM(N_AmountCr) as N_AmountCr,D_TransDate,N_BranchID,N_FnYearID,X_CustomerName,N_Order from  vw_DailySummaryPOS where N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 and D_TransDate=@p4 group by N_CompanyID,D_TransDate,N_BranchID,N_FnYearID,X_CustomerName,n_order order by n_order";
            string sqlCommandTotal="Select SUM(N_AmountDr)as N_Amount from  Vw_DailySalesSummary where  D_TransDate=@p4 group by N_CompanyID,D_TransDate,N_BranchID,N_FnYearID";
            // string sqlCommandTax="Select SUM(N_TaxAmt)as N_TaxAmt from  Inv_Sales where  D_SalesDate=@p4 and B_IsSaveDraft=0";
            // string sqlCommandCount="Select Count(N_SalesID)as N_Count from  Inv_Sales where  D_SalesDate=@p4 and B_IsSaveDraft=0";
            try
            {
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
                    Total = dLayer.ExecuteScalar(sqlCommandTotal, Params, connection);
                    // object TaxAmt = dLayer.ExecuteScalar(sqlCommandTax, Params, connection);
                    // object Count = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    //  dt = myFunctions.AddNewColumnToDataTable(dt, "N_TaxAmt", typeof(string), TaxAmt);
                    //  dt = myFunctions.AddNewColumnToDataTable(dt, "N_Count", typeof(string), Count);
                    //  dt.AcceptChanges();

                }
                dt = _api.Format(dt);

                
                Result.Add("details", dt);
                Result.Add("total", Total);
                
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(Result));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("cashSummary")]
        public ActionResult GetSalesSummaryDetails(int nCompanyID,int nFnYearID,int nBranchID,DateTime dTransDate)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nFnYearID);
            Params.Add("@p3",nBranchID);
            Params.Add("@p4",dTransDate);
            string sqlCommandText="Select * from vw_VoucherTransaction where N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 and D_TransDate=@p4";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
                }
                dt = _api.Format(dt);
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

        [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    int nCloseID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CloseID"].ToString());
                
                    nCloseID = dLayer.SaveData("Acc_DayClosing", "n_CloseID", MasterTable, connection, transaction);
                    
                    transaction.Commit();
                    return Ok(_api.Success("Saved")) ;
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
    }
}