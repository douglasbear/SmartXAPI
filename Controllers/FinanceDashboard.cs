using AutoMapper;
using SmartxAPI.Data;
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

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("financeDashboard")]
    [ApiController]
    public class FinanceDashboard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public FinanceDashboard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("details")]
        public ActionResult GetDashboardDetails(int nFnYearId)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);

            string sqlReceivables = "select sum(N_BalanceAmountF) as N_Amount from vw_InvReceivables where N_CompanyId = "+nCompanyID+" and N_FnYearId= "+nFnYearId+"";
            string sqlPayables= "select sum(N_BalanceAmount) as N_Amount from vw_InvPayables where N_CompanyId = "+nCompanyID+" and N_FnYearId= "+nFnYearId+"";
            string sqlProfitMargin= "select ((select sum(ABS(N_Amount)) from vw_AccVoucherDetailsMonthWise where X_Type='I' and N_CompanyID ="+nCompanyID+" and N_FnYearId= "+nFnYearId+") -(select sum(ABS(N_Amount))  from vw_AccVoucherDetailsMonthWise where X_Type='E' and N_CompanyID ="+nCompanyID+" and N_FnYearId= "+nFnYearId+"))  / (select sum(ABS(N_Amount)) from vw_AccVoucherDetailsMonthWise where X_Type='I' and N_CompanyID = "+nCompanyID+" and N_FnYearId= "+nFnYearId+" ) * 100 as N_ProfitMargin";

             string sqlMonthWiseData = "select X_Month,N_Expense,N_Income from vw_IncomeExpenseMonthWise where N_CompanyID ="+nCompanyID+" and N_FnYearId= "+nFnYearId+"";
            string sqlIncomeExpense = "select (select sum(ABS(N_Amount)) from vw_AccVoucherDetailsMonthWise where X_Type='I' and N_CompanyID = "+nCompanyID+" and N_FnYearId= "+nFnYearId+") as N_Income, (select sum(ABS(N_Amount))  from vw_AccVoucherDetailsMonthWise where X_Type='E' and N_CompanyID = "+nCompanyID+" and N_FnYearId= "+nFnYearId+") as N_Expense";
            // string sqlOpenQuotation = "SELECT COUNT(*) as N_ThisMonth,sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvSalesQuotationNo_Search WHERE MONTH(D_QuotationDate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_QuotationDate) = YEAR(CURRENT_TIMESTAMP)";
            // "select X_LeadSource,CAST(COUNT(*) as varchar(50)) as N_Percentage from vw_CRMLeads group by X_LeadSource";
            // string sqlPipelineoppotunity = "select count(*) as N_Count from CRM_Opportunity where N_ClosingStatusID=0 or N_ClosingStatusID is null";
            //string sqlCurrentSales =""

            SortedList Data = new SortedList();
            DataTable Receivables = new DataTable();
            DataTable Payables = new DataTable();
            DataTable ProfitMargin = new DataTable();
            DataTable MonthWiseData = new DataTable();
            DataTable IncomeExpense = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Receivables = dLayer.ExecuteDataTable(sqlReceivables, Params, connection);
                    Payables = dLayer.ExecuteDataTable(sqlPayables, Params, connection);
                    ProfitMargin = dLayer.ExecuteDataTable(sqlProfitMargin, Params, connection);
                    MonthWiseData = dLayer.ExecuteDataTable(sqlMonthWiseData, Params, connection);
                    IncomeExpense = dLayer.ExecuteDataTable(sqlIncomeExpense, Params, connection);
                }


                Receivables.AcceptChanges();
                Payables.AcceptChanges();
                ProfitMargin.AcceptChanges();
                MonthWiseData.AcceptChanges();
                IncomeExpense.AcceptChanges();


                if (Receivables.Rows.Count > 0) Data.Add("receivableData", Receivables);
                if (Payables.Rows.Count > 0) Data.Add("payableData", Payables);
                if (ProfitMargin.Rows.Count > 0) Data.Add("profitMarginData", ProfitMargin);
                if (MonthWiseData.Rows.Count > 0) Data.Add("monthWiseData", MonthWiseData);
                if (IncomeExpense.Rows.Count > 0) Data.Add("incomeExpenseData", IncomeExpense);


                return Ok(api.Success(Data));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
    }
}