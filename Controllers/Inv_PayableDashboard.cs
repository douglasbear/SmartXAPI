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
    [Route("payableDashboard")]
    [ApiController]
    public class Inv_PayableDashboard : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        // private readonly int N_FormID = 1429;
        public Inv_PayableDashboard(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult list(string date,int nCompanyID,int nFnYearID,int nPage,int nSizeperpage,string xSearchKey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    // int nCompanyID = myFunctions.GetCompanyID(User);
                    int Count = (nPage - 1) * nSizeperpage;
                    string Searchkey="";
                    string sqlCommandText="";
                    string sqlCommandText1="";
                    string sqlCommandCount = "";
                    SortedList OutPut = new SortedList();

                    Params.Add("N_CompanyID", nCompanyID);
                    DateTime date1 = DateTime.Now;
                    string formatted = date1.ToString("yyyy-M-dd");
                    Params.Add("@D_AsOfDate", formatted);

                    // int N_decimalPlace = 2;
                    // N_decimalPlace = myFunctions.getIntVAL(myFunctions.ReturnSettings("SALES", "Decimal_Place", "N_Value", nCompanyID, dLayer, connection, transaction));
                    // N_decimalPlace = N_decimalPlace == 0 ? 2 : N_decimalPlace;

                    if (xSearchKey != null && xSearchKey.Trim() != ""){
                        Searchkey = " and ( X_VendorName like '%" + xSearchKey + "%' or X_VendorCode like '%" + xSearchKey + "%'  ) ";
                    }
                    if (xSortBy == null || xSortBy.Trim() == ""){
                        xSortBy = " order by X_VendorName asc";
                    } else {
                        xSortBy = " order by " + xSortBy;
                    }

                    dLayer.ExecuteDataTablePro("SP_InvPayablesAsOfDate", Params, connection, transaction);

                    if (Count == 0){
                        sqlCommandText1 = "select top(" + nSizeperpage + ") * from Vw_vendorPayable where N_DueAmount<>0 and N_CompanyID=N_CompanyID" + Searchkey + xSortBy ;
                    } else {
                        sqlCommandText1 = "select top(" + nSizeperpage + ") * from Vw_vendorPayable where N_DueAmount<>0 and N_CompanyID=N_CompanyID" + Searchkey + " and X_VendorCode not in (select top(" + Count + ") X_VendorCode from Vw_vendorPayable where N_CompanyID=N_CompanyID" + Searchkey + xSortBy + ")" + xSortBy ;
                    }
                    dt = dLayer.ExecuteDataTable(sqlCommandText1, Params, connection, transaction);

                    sqlCommandCount = "select count(1) as N_Count,sum(Cast(REPLACE(N_DueAmount,',','') as Numeric(16,4)) ) as TotalAmount from Vw_vendorPayable where N_DueAmount<>0 and N_CompanyID=N_CompanyID";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection, transaction);
                    string TotalCount = "0";
                    string TotalSum = "0";
                    if (Summary.Rows.Count > 0){
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                        TotalSum = drow["TotalAmount"].ToString();
                    }
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    OutPut.Add("TotalSum", TotalSum);

                    // dt = _api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("detailList")]
        public ActionResult detailList(string xVendorCode,string date,int nCompanyID,int nFnYearID,int nPage,int nSizeperpage,string xSearchKey,string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    // int nCompanyID = myFunctions.GetCompanyID(User);
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText="";
                    string sqlCommandText1="";
                    string sqlCommandCount = "";
                    string Searchkey="";
                    SortedList OutPut = new SortedList();
                
                    Params.Add("N_CompanyID", nCompanyID);
                    DateTime date1 = DateTime.Now;
                    string formatted = date1.ToString("yyyy-M-dd");
                    Params.Add("@D_AsOfDate", formatted);

                    if (xSearchKey != null && xSearchKey.Trim() != ""){
                        Searchkey = "and ( X_VendorName like '%" + xSearchKey + "%' or X_ReferenceNo like '%" + xSearchKey + "%' or X_ProjectName like '%" + xSearchKey + "%'  ) ";
                    }
                    if (xSortBy == null || xSortBy.Trim() == ""){
                        xSortBy = " order by X_ReferenceNo desc";
                    } else {
                        xSortBy = " order by " + xSortBy;
                    }

                    dLayer.ExecuteDataTablePro("SP_InvPayablesAsOfDate", Params, connection, transaction);
                    // sqlCommandText = "select * from Vw_vendorPayableDetails where N_CompanyID=N_CompanyID and X_VendorCode=CAST('"+xVendorCode+"' as VARCHAR)" + Searchkey + xSortBy ;
                    // dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection, transaction);

                    if (Count == 0){
                        sqlCommandText1 = "select top(" + nSizeperpage + ") * from Vw_vendorPayableDetails where N_BalanceAmount<>0 and N_CompanyID=N_CompanyID and X_VendorCode=CAST('"+xVendorCode+"' as VARCHAR)" + Searchkey + xSortBy ;
                    } else {
                        sqlCommandText1 = "select top(" + nSizeperpage + ") * from Vw_vendorPayableDetails where N_BalanceAmount<>0 and N_CompanyID=N_CompanyID and X_VendorCode=CAST('"+xVendorCode+"' as VARCHAR)" + Searchkey + " and X_ReferenceNo not in (select top(" + Count + ") X_ReferenceNo from Vw_vendorPayableDetails where N_CompanyID=N_CompanyID" + Searchkey + xSortBy + ")" + xSortBy;
                    }
                    dt = dLayer.ExecuteDataTable(sqlCommandText1, Params, connection, transaction);

                    sqlCommandCount = "select count(1) as N_Count,sum(Cast(REPLACE(N_BalanceAmount,',','') as Numeric(16,4)) ) as TotalAmount from Vw_vendorPayableDetails where N_BalanceAmount<>0 and N_CompanyID=N_CompanyID and X_VendorCode=CAST('"+xVendorCode+"' as VARCHAR)";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection, transaction);
                    string TotalCount = "0";
                    string TotalSum = "0";
                    if (Summary.Rows.Count > 0){
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                        TotalSum = drow["TotalAmount"].ToString();
                    }
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    OutPut.Add("TotalSum", TotalSum);

                    // dt = _api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

    }
}