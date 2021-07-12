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

            string sqlReceivables = "select CAST(CONVERT(VARCHAR, CAST(sum(N_BalanceAmount) AS MONEY), 1) AS VARCHAR) as N_Amount from vw_InvReceivables where N_CompanyId = "+nCompanyID+" and N_FnYearId= "+nFnYearId+"";
            string sqlPayables= "select CAST(CONVERT(VARCHAR, CAST(sum(N_BalanceAmount) AS MONEY), 1) AS VARCHAR) as N_Amount from vw_InvPayables where N_CompanyId = "+nCompanyID+" and N_FnYearId= "+nFnYearId+"";
            string sqlProfitMargin= "select CAST(CONVERT(VARCHAR, CAST((((select sum(ABS(N_Amount)) from vw_AccVoucherDetailsMonthWise where X_Type='I' and N_CompanyID ="+nCompanyID+" and N_FnYearId= "+nFnYearId+") -(select sum(ABS(N_Amount))  from vw_AccVoucherDetailsMonthWise where X_Type='E' and N_CompanyID ="+nCompanyID+" and N_FnYearId= "+nFnYearId+"))  / (select sum(ABS(N_Amount)) from vw_AccVoucherDetailsMonthWise where X_Type='I' and N_CompanyID = "+nCompanyID+" and N_FnYearId= "+nFnYearId+" ) * 100) AS MONEY), 1) AS VARCHAR) as N_ProfitMargin";

            string sqlMonthWiseData = "select X_Month,N_Expense,N_Income from vw_IncomeExpenseMonthWise where N_CompanyID ="+nCompanyID+" and N_FnYearId= "+nFnYearId+"";
            string sqlIncomeExpense = "select (select CAST(CONVERT(VARCHAR, CAST(sum(ABS(N_Amount)) AS MONEY), 1) AS VARCHAR) from vw_AccVoucherDetailsMonthWise where X_Type='I' and N_CompanyID = "+nCompanyID+" and N_FnYearId= "+nFnYearId+") as N_Income, (select CAST(CONVERT(VARCHAR, CAST(sum(ABS(N_Amount)) AS MONEY), 1) AS VARCHAR)  from vw_AccVoucherDetailsMonthWise where X_Type='E' and N_CompanyID = "+nCompanyID+" and N_FnYearId= "+nFnYearId+") as N_Expense";
            string sqlAssetLiability = "SELECT Acc_VoucherDetails.N_CompanyID,Acc_VoucherDetails.N_FnYearID,ABS (sum(Acc_VoucherDetails.N_Amount)) as  N_Amount,vw_AccMastLedger.X_Type FROM   Acc_VoucherDetails INNER JOIN vw_AccMastLedger ON Acc_VoucherDetails.N_CompanyID = vw_AccMastLedger.N_CompanyID AND Acc_VoucherDetails.N_LedgerID = vw_AccMastLedger.N_LedgerID AND Acc_VoucherDetails.N_FnYearID = vw_AccMastLedger.N_FnYearID where  vw_AccMastLedger.X_Type in ('A','L') and Acc_VoucherDetails.N_CompanyID ="+nCompanyID+"   and Acc_VoucherDetails.N_FnYearID = "+nFnYearId+"  group by Acc_VoucherDetails.N_CompanyID,Acc_VoucherDetails.N_FnYearID,vw_AccMastLedger.X_Type ";
            string sqlCashBalance = "SELECT sum(Acc_VoucherDetails.N_Amount) as  N_Amount,vw_AccMastLedger.N_CashBahavID FROM   Acc_VoucherDetails INNER JOIN vw_AccMastLedger ON Acc_VoucherDetails.N_CompanyID = vw_AccMastLedger.N_CompanyID AND Acc_VoucherDetails.N_LedgerID = vw_AccMastLedger.N_LedgerID AND Acc_VoucherDetails.N_FnYearID = vw_AccMastLedger.N_FnYearID where  vw_AccMastLedger.N_CashBahavID =4 and Acc_VoucherDetails.N_CompanyID ="+nCompanyID+"  and Acc_VoucherDetails.N_FnYearID = "+nFnYearId+" group by Acc_VoucherDetails.N_CompanyID,Acc_VoucherDetails.N_FnYearID,vw_AccMastLedger.N_CashBahavID ";
            string sqlBankBalance = "SELECT sum(Acc_VoucherDetails.N_Amount) as  N_Amount,vw_AccMastLedger.N_CashBahavID FROM   Acc_VoucherDetails INNER JOIN vw_AccMastLedger ON Acc_VoucherDetails.N_CompanyID = vw_AccMastLedger.N_CompanyID AND Acc_VoucherDetails.N_LedgerID = vw_AccMastLedger.N_LedgerID AND Acc_VoucherDetails.N_FnYearID = vw_AccMastLedger.N_FnYearID where  vw_AccMastLedger.N_CashBahavID =5 and Acc_VoucherDetails.N_CompanyID ="+nCompanyID+"  and Acc_VoucherDetails.N_FnYearID = "+nFnYearId+" group by Acc_VoucherDetails.N_CompanyID,Acc_VoucherDetails.N_FnYearID,vw_AccMastLedger.N_CashBahavID ";

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
            DataTable AssetLiability = new DataTable();
            DataTable CashBalance = new DataTable();
            DataTable BankBalance = new DataTable();

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
                    AssetLiability = dLayer.ExecuteDataTable(sqlAssetLiability, Params, connection);
                    CashBalance = dLayer.ExecuteDataTable(sqlCashBalance, Params, connection);
                    BankBalance = dLayer.ExecuteDataTable(sqlBankBalance, Params, connection);
                }


                Receivables.AcceptChanges();
                Payables.AcceptChanges();
                ProfitMargin.AcceptChanges();
                MonthWiseData.AcceptChanges();
                IncomeExpense.AcceptChanges();
                AssetLiability.AcceptChanges();
                CashBalance.AcceptChanges();
                BankBalance.AcceptChanges();


                if (Receivables.Rows.Count > 0) Data.Add("receivableData", Receivables);
                if (Payables.Rows.Count > 0) Data.Add("payableData", Payables);
                if (ProfitMargin.Rows.Count > 0) Data.Add("profitMarginData", ProfitMargin);
                if (MonthWiseData.Rows.Count > 0) Data.Add("monthWiseData", MonthWiseData);
                if (IncomeExpense.Rows.Count > 0) Data.Add("incomeExpenseData", IncomeExpense);
                if (AssetLiability.Rows.Count > 0) Data.Add("assetLiabilityData", AssetLiability);
                if (CashBalance.Rows.Count > 0) Data.Add("cashBalanceData", CashBalance);
                if (BankBalance.Rows.Count > 0) Data.Add("bankBalanceData", BankBalance);


                return Ok(api.Success(Data));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
    }
}