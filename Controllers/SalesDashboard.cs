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
    [Route("salesDashboard")]
    [ApiController]
    public class SalesDashboard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public SalesDashboard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("details")]
        public ActionResult GetDashboardDetails()
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
           
            
                     
            string sqlCurrentOrder = "SELECT COUNT(*) as N_ThisMonth,sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvSalesOrderNo_Search WHERE MONTH(Cast(D_OrderDate as DateTime)) = MONTH(CURRENT_TIMESTAMP) and YEAR(D_OrderDate)= YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyID + "";
            string sqlCurrentInvoice = "SELECT COUNT(*) as N_ThisMonth,CAST(CONVERT(varchar, CAST(sum(Cast(REPLACE(X_BillAmt,',','') as Numeric(10,2)) ) AS Money), 1) AS   varchar) AS  TotalAmount  FROM vw_InvSalesInvoiceNo_Search WHERE MONTH(Cast([Invoice Date] as DateTime)) = MONTH(CURRENT_TIMESTAMP) AND YEAR(Cast([Invoice Date] as DateTime)) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyID + "";
            string sqlCurrentQuotation = "SELECT COUNT(*) as N_ThisMonth,sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvSalesQuotationNo_Search WHERE MONTH(Cast(D_QuotationDate as DateTime)) = MONTH(CURRENT_TIMESTAMP) and YEAR(D_QuotationDate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyID + "";

             string sqlCustomerbySource = "select top(5) Customer as X_LeadSource,CAST(COUNT(*) as varchar(50)) as N_Percentage from vw_InvSalesInvoiceNo_Search where N_CompanyID = " + nCompanyID + " group by Customer order by COUNT(*) Desc";
             string sqlPipelineoppotunity = "select count(*) as N_Count from CRM_Opportunity where (N_ClosingStatusID=0 or N_ClosingStatusID is null) and N_CompanyID = " + nCompanyID + "";
            // string sqlOpenQuotation = "SELECT COUNT(*) as N_ThisMonth,sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvSalesQuotationNo_Search WHERE MONTH(D_QuotationDate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_QuotationDate) = YEAR(CURRENT_TIMESTAMP)";
            // "select X_LeadSource,CAST(COUNT(*) as varchar(50)) as N_Percentage from vw_CRMLeads group by X_LeadSource";
            // string sqlPipelineoppotunity = "select count(*) as N_Count from CRM_Opportunity where N_ClosingStatusID=0 or N_ClosingStatusID is null";
            //string sqlCurrentSales =""

            SortedList Data = new SortedList();
            DataTable CurrentOrder = new DataTable();
            DataTable CurrentInvoice = new DataTable();
            DataTable CurrentQuotation = new DataTable();
            DataTable CurrentCustomer = new DataTable();
            DataTable OpenOpportunities = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                     bool B_customer = myFunctions.CheckPermission(nCompanyID, 1302, "Administrator", "X_UserCategory", dLayer, connection);
                     CurrentOrder = dLayer.ExecuteDataTable(sqlCurrentOrder, Params, connection);
                    CurrentInvoice = dLayer.ExecuteDataTable(sqlCurrentInvoice, Params, connection);
                    CurrentQuotation = dLayer.ExecuteDataTable(sqlCurrentQuotation, Params, connection);
                    CurrentCustomer = dLayer.ExecuteDataTable(sqlCustomerbySource, Params, connection);
                    OpenOpportunities = dLayer.ExecuteDataTable(sqlPipelineoppotunity, Params, connection);
                     if(B_customer) 
                     { 
                     Data.Add("permision",true);
                    }

                }

                
                CurrentOrder.AcceptChanges();
                CurrentInvoice.AcceptChanges();
                CurrentQuotation.AcceptChanges();
                CurrentCustomer.AcceptChanges();
                OpenOpportunities.AcceptChanges();
               

                if (CurrentOrder.Rows.Count > 0) Data.Add("orderData", CurrentOrder);
                if (CurrentInvoice.Rows.Count > 0) Data.Add("invoiceData", CurrentInvoice);
                if (CurrentQuotation.Rows.Count > 0) Data.Add("quotationData", CurrentQuotation);
                if (CurrentCustomer.Rows.Count > 0) Data.Add("customerbySource", CurrentCustomer);
                if (CurrentCustomer.Rows.Count > 0) Data.Add("opportunityData", CurrentCustomer);
               

                return Ok(api.Success(Data));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
        [HttpGet("salesOrderList")]
        public ActionResult GetOrderList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
        
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
                int nCompanyId = myFunctions.GetCompanyID(User);
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_OrderNo like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by M_NetAmt desc";
            else
                xSortBy = " order by " + xSortBy;
 
            if (Count == 0)
                sqlCommandText = "select top(10) * from vw_SalesOrder_Dashboard where  YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + "  " + Searchkey + " " + xSortBy ;
            else
                sqlCommandText = "select top(10) * from vw_SalesOrder_Dashboard where YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + "  " + Searchkey + " and N_SalesOrderID not in (select top(" + Count + ") N_SalesOrderID from vw_SalesOrder_Dashboard where N_CompanyID=@p1 " + xSortBy + " )" + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "Select * from vw_SalesOrder_Dashboard Where MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + " ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("salesQuotationList")]
        public ActionResult GetQuotationList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([Quotation No] like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_BillAmtF desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(10) * from vw_InvSalesQuotationNo_Search where  YEAR(D_QuotationDate) = YEAR(CURRENT_TIMESTAMP)  and N_CompanyID = " + nCompanyId + " " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(10) * from vw_InvSalesQuotationNo_Search where YEAR(D_QuotationDate) = YEAR(CURRENT_TIMESTAMP)  and N_CompanyID = " + nCompanyId + " " + Searchkey + " and N_QuotationID not in (select top(" + Count + ") N_QuotationID from vw_InvSalesQuotationNo_Search where N_CompanyID=@p1 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = api.Format(dt);
                    sqlCommandCount = "Select * from vw_InvSalesQuotationNo_Search Where  YEAR(D_QuotationDate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + "  ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("salesInvoiceList")]
        public ActionResult GetInvoiceList(int nFnYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([Invoice No] like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_BillAmt desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(10) N_BillAmt=Cast(REPLACE(X_BillAmt,',','') as Numeric(10,2)),* from vw_InvSalesInvoiceNo_Search where  YEAR(D_SalesDate) = YEAR(CURRENT_TIMESTAMP)  and N_CompanyId = " + nCompanyID + " " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(10) N_BillAmt=Cast(REPLACE(X_BillAmt,',','') as Numeric(10,2)),* from vw_InvSalesInvoiceNo_Search where YEAR(D_SalesDate) = YEAR(CURRENT_TIMESTAMP)  and N_CompanyId = " + nCompanyID + " " + Searchkey + " and N_SalesId not in (select top(" + Count + ") N_SalesId from vw_InvSalesInvoiceNo_Search where N_CompanyId=@p1 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyID);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = api.Format(dt);
                    sqlCommandCount = "Select * from vw_InvSalesInvoiceNo_Search Where  YEAR(D_SalesDate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyId = " + nCompanyID + "  ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("monthlyList")]
        public ActionResult GetMonthlyList(string screen)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "";

            if (screen=="Order")
                sqlCommandText = "SELECT * FROM vw_InvSalesOrderNo_Search WHERE MONTH(Cast(D_OrderDate as DateTime)) = MONTH(CURRENT_TIMESTAMP) and YEAR(D_OrderDate)= YEAR(CURRENT_TIMESTAMP) and N_CompanyID =@nCompanyID ";
            else if (screen=="Quotation")
                sqlCommandText = "SELECT * FROM vw_InvSalesQuotationNo_Search WHERE MONTH(Cast(D_QuotationDate as DateTime)) = MONTH(CURRENT_TIMESTAMP) and YEAR(D_QuotationDate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID =@nCompanyID ";
            else if (screen=="Invoice")
                sqlCommandText = "SELECT * FROM vw_InvSalesInvoiceNo_Search WHERE MONTH(Cast([Invoice Date] as DateTime)) = MONTH(CURRENT_TIMESTAMP) AND YEAR(Cast([Invoice Date] as DateTime)) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID =@nCompanyID ";
            else
                sqlCommandText = "select * from CRM_Opportunity where (N_ClosingStatusID=0 or N_ClosingStatusID is null) and N_CompanyID =@nCompanyID ";
            
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
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
    }
}