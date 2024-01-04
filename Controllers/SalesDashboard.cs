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
        public ActionResult GetDashboardDetails(int nFnYearID,int nBranchID,bool bAllBranchData)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            string crieteria = "";
            string crieteria1 = "";
            string revCriteria= "";
            string revsaCriteria= "";
            if (bAllBranchData == true)
            {
            crieteria="";
            crieteria1="";
            }
            else
            {
            revsaCriteria=" and Inv_Sales.N_BranchID="+nBranchID;
            crieteria=" and N_BranchID="+nBranchID;
            crieteria1="and Inv_PayReceipt.N_BranchID="+nBranchID;
            revCriteria="and Inv_PayReceipt.N_BranchID="+nBranchID;
            }
         
            string MonthWiseDate="";
            string YearWiseDate="";

           string sqlCurrentOrder="";
           string sqlCurrentInvoice="";
           string sqlCurrentQuotation="";
           string sqlBranchWiseData="";
           string sqlCustomerbySource="";
           string sqlPipelineoppotunity  ="";
           string sqlReceivedRevenue="";
           string sqlDailySales="";
           string sqlMonthlySales="";
           string DraftedDelivery="";
           string DraftedInvoice="";
           string RevenueToday="";
           string recvdRvnTdy="";
           string BranchWiseToday="";
        //    string draftCount="";
            // string sqlReceivedRevenue = "SELECT SUM(Inv_PayReceiptDetails.N_AmountF-Inv_PayReceiptDetails.N_DiscountAmtF)as N_ReceivedAmount FROM Inv_PayReceiptDetails INNER JOIN Inv_PayReceipt ON Inv_PayReceiptDetails.N_PayReceiptId = Inv_PayReceipt.N_PayReceiptId AND Inv_PayReceiptDetails.N_CompanyID = Inv_PayReceipt.N_CompanyID where Inv_PayReceipt.X_Type in ('SR','SA') and MONTH(Cast(Inv_PayReceiptDetails.D_Entrydate as DateTime)) ="+MonthWiseDate+"and YEAR(Inv_PayReceiptDetails.D_Entrydate)= YEAR(CURRENT_TIMESTAMP) and Inv_PayReceiptDetails.N_CompanyID = " + nCompanyID  + " and Inv_PayReceipt.N_FnyearID="+nFnYearID + crieteria1;
            // string sqlOpenQuotation = "SELECT count(1) as N_ThisMonth,sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvSalesQuotationNo_Search WHERE MONTH(D_QuotationDate) ="+MonthWiseDate+"AND YEAR(D_QuotationDate) = YEAR(CURRENT_TIMESTAMP)";
            // "select X_LeadSource,CAST(count(1) as varchar(50)) as N_Percentage from vw_CRMLeads group by X_LeadSource";
            // string sqlPipelineoppotunity = "select count(1) as N_Count from CRM_Opportunity where N_ClosingStatusID=0 or N_ClosingStatusID is null";
            //string sqlCurrentSales =""

            SortedList Data = new SortedList();
            DataTable CurrentOrder = new DataTable();
            DataTable CurrentInvoice = new DataTable();
            DataTable CurrentQuotation = new DataTable();
            DataTable CurrentCustomer = new DataTable();
            DataTable OpenOpportunities = new DataTable();
            DataTable ReceivedRevenue = new DataTable();
            DataTable BranchWiseData = new DataTable();
            DataTable DailySales = new DataTable();
            DataTable MonthlySales = new DataTable();
            DataTable MnothlyDelivery= new DataTable();
            DataTable MnothlydraftedInvoice = new DataTable();
            DataTable todayRevenue = new DataTable();
            DataTable ReceivedRvnTdy = new DataTable();
            DataTable branchwiseTdy = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    object StartYear=dLayer.ExecuteScalar("select Year(D_Start) from Acc_FnYear where N_CompanyID ="+nCompanyID+" and N_FnYearID="+nFnYearID+"", Params, connection);
                    object EndDate=dLayer.ExecuteScalar("select CONVERT(varchar,D_END ,23) AS D_End from Acc_FnYear where N_CompanyID = "+nCompanyID+" and N_FnYearID="+nFnYearID+"", Params, connection);
                    object CurrentYear=dLayer.ExecuteScalar("select Year(CURRENT_TIMESTAMP)", Params, connection);
                    if(myFunctions.getIntVAL(StartYear.ToString())!=myFunctions.getIntVAL(CurrentYear.ToString()))
                    {
                        MonthWiseDate="MONTH('"+EndDate+"')";
                        YearWiseDate= " YEAR('"+EndDate+"')";
                    }
                    else
                    {
                        MonthWiseDate="MONTH(CURRENT_TIMESTAMP)";
                        YearWiseDate="YEAR(CURRENT_TIMESTAMP)";

                    }

                 
                     
                      sqlCurrentOrder = "SELECT count(1) as N_ThisMonth,sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvSalesOrderNo_Search WHERE MONTH(Cast(D_OrderDate as DateTime)) = "+MonthWiseDate+" and YEAR(D_OrderDate)= "+YearWiseDate+" and N_CompanyID = " + nCompanyID + " and N_FnyearID="+nFnYearID  + "and N_FormID=81" + crieteria ;
                      sqlCurrentInvoice = "SELECT count(1) as N_ThisMonth,sum(Cast(REPLACE(TotalAmount,',','') as Numeric(10,2)) ) AS  TotalAmount  FROM Vw_SalesRevenew_Cloud WHERE MONTH(Cast(D_SalesDate as DateTime)) = "+MonthWiseDate+" AND YEAR(Cast(D_SalesDate as DateTime)) = "+YearWiseDate+" and N_CompanyID = " + nCompanyID  + " and N_FnyearID="+nFnYearID + crieteria;
                     //string sqlCurrentInvoice = "SELECT count(1) as N_ThisMonth,CAST(CONVERT(varchar, CAST(sum(Cast(REPLACE(X_BillAmt,',','') as Numeric(10,2)) ) AS Money), 1) AS   varchar) AS  TotalAmount  FROM vw_InvSalesInvoiceNo_Search WHERE MONTH(Cast([Invoice Date] as DateTime)) ="+MonthWiseDate+"AND YEAR(Cast([Invoice Date] as DateTime)) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyID  + " and N_FnyearID="+nFnYearID;
                      sqlCurrentQuotation = "SELECT count(1) as N_ThisMonth,sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvSalesQuotationNo_Search WHERE MONTH(Cast(D_QuotationDate as DateTime)) = "+MonthWiseDate+" and YEAR(D_QuotationDate) = "+YearWiseDate+" and N_CompanyID = " + nCompanyID  + " and N_FnyearID="+nFnYearID + crieteria;
                      sqlBranchWiseData = "select sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as TotalAmount,X_BranchName from vw_BranchWiseSales where MONTH(D_SalesDate) = "+MonthWiseDate+" AND YEAR(D_SalesDate) = "+YearWiseDate+" and N_CompanyID ="+nCompanyID + crieteria + " Group BY X_BranchName,N_CompanyID";
                      sqlCustomerbySource = "select top(5) Customer as X_LeadSource,sum(Cast(REPLACE(X_BillAmt,',','') as Numeric(10,2)) ) as N_Percentage from vw_InvSalesInvoiceNo_Search where N_CompanyID = " + nCompanyID  + " and N_TypeID <>1 and  N_FnyearID="+nFnYearID + crieteria + " group by Customer order by N_Percentage Desc";
                      sqlPipelineoppotunity = "select count(1) as N_Count from CRM_Opportunity where (N_ClosingStatusID=0 or N_ClosingStatusID is null) and N_CompanyID = " + nCompanyID  + " and N_FnyearID="+nFnYearID + crieteria;
                      //sqlReceivedRevenue = "select sum(N_ReceivedAmount) as N_ReceivedAmount,N_CompanyId from (select SUM(N_BillAmt)+ SUM(N_TaxAmt) AS N_ReceivedAmount,Inv_Sales.N_CompanyId from Inv_Sales WHERE N_PaymentMethodId=1 AND MONTH(Cast(Inv_Sales.D_Entrydate as DateTime)) ="+MonthWiseDate+" and YEAR(Inv_Sales.D_Entrydate)= "+YearWiseDate+" AND N_CompanyId="+nCompanyID+" AND N_FnYearId="+nFnYearID+ crieteria +" group by Inv_Sales.N_CompanyID union SELECT SUM(Inv_PayReceiptDetails.N_AmountF-Inv_PayReceiptDetails.N_DiscountAmtF)as N_ReceivedAmount,Inv_PayReceiptDetails.N_CompanyID  FROM Inv_PayReceiptDetails INNER JOIN Inv_PayReceipt ON Inv_PayReceiptDetails.N_PayReceiptId = Inv_PayReceipt.N_PayReceiptId AND Inv_PayReceiptDetails.N_CompanyID = Inv_PayReceipt.N_CompanyID where Inv_PayReceipt.X_Type in ('SR','SA') and MONTH(Cast(Inv_PayReceiptDetails.D_Entrydate as DateTime)) ="+MonthWiseDate+"and YEAR(Inv_PayReceiptDetails.D_Entrydate)= "+YearWiseDate+" and Inv_PayReceiptDetails.N_CompanyID = " + nCompanyID  + " and Inv_PayReceipt.N_FnyearID="+nFnYearID + revCriteria+" group by Inv_PayReceiptDetails.N_CompanyID) as temp where N_CompanyID="+nCompanyID+" group by N_CompanyID";
                    //   sqlReceivedRevenue = "select sum(N_ReceivedAmount) as N_ReceivedAmount,N_CompanyId from (select SUM(N_BillAmt)+ SUM(N_TaxAmt) AS N_ReceivedAmount,Inv_Sales.N_CompanyId from Inv_Sales WHERE N_PaymentMethodId=1 AND MONTH(Cast(Inv_Sales.D_Entrydate as DateTime)) ="+MonthWiseDate+" and YEAR(Inv_Sales.D_Entrydate)= "+YearWiseDate+" AND N_CompanyId="+nCompanyID+" AND N_FnYearId="+nFnYearID+ crieteria +" group by Inv_Sales.N_CompanyID union SELECT SUM(Inv_PayReceiptDetails.N_AmountF-Inv_PayReceiptDetails.N_DiscountAmtF) as N_ReceivedAmount,Inv_PayReceiptDetails.N_CompanyID  FROM Inv_PayReceiptDetails INNER JOIN Inv_PayReceipt ON Inv_PayReceiptDetails.N_PayReceiptId = Inv_PayReceipt.N_PayReceiptId AND Inv_PayReceiptDetails.N_CompanyID = Inv_PayReceipt.N_CompanyID where Inv_PayReceipt.X_Type in ('SR','SA') and MONTH(Cast(Inv_PayReceipt.D_Date as DateTime)) ="+MonthWiseDate+"and YEAR(Inv_PayReceipt.D_Date)= "+YearWiseDate+" and Inv_PayReceiptDetails.N_CompanyID = " + nCompanyID  + " and Inv_PayReceipt.N_FnyearID="+nFnYearID + revCriteria+" group by Inv_PayReceiptDetails.N_CompanyID) as temp where N_CompanyID="+nCompanyID+" group by N_CompanyID";
                      sqlReceivedRevenue = "select sum(N_ReceivedAmount) as N_ReceivedAmount,N_CompanyId from "+
                                            " (select SUM(Inv_SaleAmountDetails.N_Amount) AS N_ReceivedAmount,Inv_Sales.N_CompanyId from Inv_Sales INNER JOIN Inv_SaleAmountDetails ON Inv_Sales.N_CompanyID=Inv_SaleAmountDetails.N_CompanyID AND Inv_Sales.N_SalesID=Inv_SaleAmountDetails.N_SalesID INNER JOIN Inv_Customer ON Inv_Customer.N_CompanyID=Inv_SaleAmountDetails.N_CompanyID AND Inv_Customer.N_CustomerID=Inv_SaleAmountDetails.N_CustomerID AND Inv_Customer.N_FnYearId=Inv_Sales.N_FnYearId "+
	                                            " WHERE Inv_Customer.N_TypeID<>2 AND MONTH(Cast(Inv_Sales.D_Entrydate as DateTime)) ="+MonthWiseDate+" and YEAR(Inv_Sales.D_Entrydate)= "+YearWiseDate+" AND Inv_Sales.N_CompanyId="+nCompanyID+" AND Inv_Sales.N_FnYearId="+nFnYearID+ revsaCriteria +" group by Inv_Sales.N_CompanyID "+
                                            " union SELECT SUM(Inv_PayReceiptDetails.N_AmountF-Inv_PayReceiptDetails.N_DiscountAmtF) as N_ReceivedAmount,Inv_PayReceiptDetails.N_CompanyID  FROM Inv_PayReceiptDetails INNER JOIN Inv_PayReceipt ON Inv_PayReceiptDetails.N_PayReceiptId = Inv_PayReceipt.N_PayReceiptId AND Inv_PayReceiptDetails.N_CompanyID = Inv_PayReceipt.N_CompanyID "+
	                                            " where Inv_PayReceipt.X_Type in ('SR','SA') and MONTH(Cast(Inv_PayReceipt.D_Date as DateTime)) ="+MonthWiseDate+"and YEAR(Inv_PayReceipt.D_Date)= "+YearWiseDate+" and Inv_PayReceiptDetails.N_CompanyID = " + nCompanyID  + " and Inv_PayReceipt.N_FnyearID="+nFnYearID + revCriteria+" group by Inv_PayReceiptDetails.N_CompanyID) as temp where N_CompanyID="+nCompanyID+" group by N_CompanyID";
                      sqlDailySales = " select sum(N_TotalSales) AS  TotalSales,Cast(D_SalesDate as date) as d_salesdate ,sum(N_CashSales) AS  TotalCashSales,Cast(D_SalesDate as date) as d_cashdate from vw_DateWiseTotalSales  where MONTH(Cast(D_SalesDate as DateTime)) ="+MonthWiseDate+"and YEAR(D_SalesDate)= "+YearWiseDate+" and N_CompanyID = " + nCompanyID  + " and N_FnyearID="+nFnYearID + crieteria + "  group by  Cast(D_SalesDate as date)";
                      sqlMonthlySales = " select D_Start,X_Month, N_Year, N_Month, sum (N_SaleOrderAmt)AS N_SalesOrderAmt,sum (N_SalesAmt)AS N_SalesAmt,sum (N_PaidAmt)AS N_PaidAmt from vw_MonthBranchWiseSalesAmt  where  N_CompanyID = " + nCompanyID  + " and N_FnyearID="+nFnYearID + crieteria + " group by D_Start,X_Month, N_Year, N_Month order by  N_Year, N_Month";
                      DraftedDelivery="SELECT sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvDeliveryNoteNo_Search_Disp WHERE B_IsSaveDraft=1  and N_CompanyID = " + nCompanyID  + " and N_FnyearID="+nFnYearID;
                      object draftCount=dLayer.ExecuteScalar( "select count(1) as N_Count from inv_deliverynote where b_IsSaveDraft=1 and N_CompanyID = " + nCompanyID  + " and N_FnyearID="+nFnYearID+"",Params,connection);
                      DraftedInvoice="SELECT sum(Cast(REPLACE(X_BillAmt,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvSalesInvoiceNo_Search_New_Cloud  WHERE B_IsSaveDraft=1 and isnull(b_Isproforma,0)=0  and N_CompanyID = " + nCompanyID  + " and N_FnyearID="+nFnYearID +crieteria;
                      object draftinvoicecount=dLayer.ExecuteScalar("Select count(1) as N_Count from Inv_Sales where b_IsSaveDraft=1 and isnull(b_Isproforma,0)=0 and N_CompanyID = " + nCompanyID  + " and N_FnyearID="+nFnYearID+crieteria+"",Params,connection);
                      //recvdRvnTdy="select sum(N_ReceivedAmount) as N_ReceivedAmount,N_CompanyId from (select SUM(N_BillAmt)+ SUM(N_TaxAmt) AS N_ReceivedAmount,Inv_Sales.N_CompanyId from Inv_Sales WHERE N_PaymentMethodId=1 AND Cast(Inv_Sales.D_Salesdate AS DATE)>=cast(GETDATE() as DATE) AND N_CompanyId="+nCompanyID+" AND N_FnYearId="+nFnYearID+ crieteria +" group by Inv_Sales.N_CompanyID union SELECT SUM(Inv_PayReceiptDetails.N_AmountF-Inv_PayReceiptDetails.N_DiscountAmtF)as N_ReceivedAmount,Inv_PayReceiptDetails.N_CompanyID  FROM Inv_PayReceiptDetails INNER JOIN Inv_PayReceipt ON Inv_PayReceiptDetails.N_PayReceiptId = Inv_PayReceipt.N_PayReceiptId AND Inv_PayReceiptDetails.N_CompanyID = Inv_PayReceipt.N_CompanyID where Inv_PayReceipt.X_Type in ('SR','SA') and Cast(Inv_PayReceipt.D_Date as DATE)=cast(GETDATE() as DATE) and Inv_PayReceiptDetails.N_CompanyID = " + nCompanyID  + " and Inv_PayReceipt.N_FnyearID="+nFnYearID + revCriteria+" group by Inv_PayReceiptDetails.N_CompanyID) as temp where N_CompanyID="+nCompanyID+" group by N_CompanyID";
                      recvdRvnTdy="select sum(N_ReceivedAmount) as N_ReceivedAmount,N_CompanyId from "+
                                   " (select SUM(Inv_SaleAmountDetails.N_Amount) AS N_ReceivedAmount,Inv_Sales.N_CompanyId from Inv_Sales INNER JOIN Inv_SaleAmountDetails ON Inv_Sales.N_CompanyID=Inv_SaleAmountDetails.N_CompanyID AND Inv_Sales.N_SalesID=Inv_SaleAmountDetails.N_SalesID INNER JOIN Inv_Customer ON Inv_Customer.N_CompanyID=Inv_SaleAmountDetails.N_CompanyID AND Inv_Customer.N_CustomerID=Inv_SaleAmountDetails.N_CustomerID AND Inv_Customer.N_FnYearId=Inv_Sales.N_FnYearId "+
                                        " WHERE Inv_Customer.N_TypeID<>2 AND Cast(Inv_Sales.D_Salesdate AS DATE)=cast(GETDATE() as DATE) AND Inv_Sales.N_CompanyId="+nCompanyID+" AND Inv_Sales.N_FnYearId="+nFnYearID+ revsaCriteria +" group by Inv_Sales.N_CompanyID "+
                                    " union SELECT SUM(Inv_PayReceiptDetails.N_AmountF-Inv_PayReceiptDetails.N_DiscountAmtF)as N_ReceivedAmount,Inv_PayReceiptDetails.N_CompanyID  FROM Inv_PayReceiptDetails INNER JOIN Inv_PayReceipt ON Inv_PayReceiptDetails.N_PayReceiptId = Inv_PayReceipt.N_PayReceiptId AND Inv_PayReceiptDetails.N_CompanyID = Inv_PayReceipt.N_CompanyID "+
                                        " where Inv_PayReceipt.X_Type in ('SR','SA') and Cast(Inv_PayReceipt.D_Date as DATE)=cast(GETDATE() as DATE) and Inv_PayReceiptDetails.N_CompanyID = " + nCompanyID  + " and Inv_PayReceipt.N_FnyearID="+nFnYearID + revCriteria+" group by Inv_PayReceiptDetails.N_CompanyID) as temp where N_CompanyID="+nCompanyID+" group by N_CompanyID";

                      RevenueToday="select SUM(N_BillAmt) as N_ReceivedAmount,count(1) as N_Count from inv_sales where n_CompanyId= " + nCompanyID  + " and Cast(Inv_Sales.D_SalesDate as DATE)>=cast(GETDATE() as DATE) and isnull(Inv_Sales.B_IsSaveDraft,0)=0";
                      BranchWiseToday="select sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as TotalAmount,X_BranchName from vw_BranchWiseSales where Cast(D_SalesDate as DATE) >=cast(GETDATE() as DATE) and N_CompanyID = " + nCompanyID  + " Group BY X_BranchName,N_CompanyID";

                     bool B_customer = myFunctions.CheckPermission(nCompanyID, 1302, "Administrator", "X_UserCategory", dLayer, connection);
                     CurrentOrder = dLayer.ExecuteDataTable(sqlCurrentOrder, Params, connection);
                    CurrentInvoice = dLayer.ExecuteDataTable(sqlCurrentInvoice, Params, connection);
                    CurrentQuotation = dLayer.ExecuteDataTable(sqlCurrentQuotation, Params, connection);
                    CurrentCustomer = dLayer.ExecuteDataTable(sqlCustomerbySource, Params, connection);
                    OpenOpportunities = dLayer.ExecuteDataTable(sqlPipelineoppotunity, Params, connection);
                    ReceivedRevenue = dLayer.ExecuteDataTable(sqlReceivedRevenue, Params, connection);
                    BranchWiseData = dLayer.ExecuteDataTable(sqlBranchWiseData, Params, connection);
                    DailySales = dLayer.ExecuteDataTable(sqlDailySales, Params, connection);
                    MonthlySales = dLayer.ExecuteDataTable(sqlMonthlySales, Params, connection);
                    MnothlyDelivery = dLayer.ExecuteDataTable(DraftedDelivery, Params, connection);
                    MnothlydraftedInvoice = dLayer.ExecuteDataTable(DraftedInvoice, Params, connection);
                    todayRevenue=dLayer.ExecuteDataTable(RevenueToday, Params, connection);
                    ReceivedRvnTdy=dLayer.ExecuteDataTable(recvdRvnTdy, Params, connection);
                    branchwiseTdy=dLayer.ExecuteDataTable(BranchWiseToday, Params, connection);
                     if(B_customer) 
                     { 
                     Data.Add("permision",true);
                    }
                    myFunctions.AddNewColumnToDataTable(MnothlyDelivery, "N_Count", typeof(int), draftCount);
                    myFunctions.AddNewColumnToDataTable(MnothlydraftedInvoice, "N_Count", typeof(int), draftinvoicecount);

                }

                
                CurrentOrder.AcceptChanges();
                CurrentInvoice.AcceptChanges();
                CurrentQuotation.AcceptChanges();
                CurrentCustomer.AcceptChanges();
                OpenOpportunities.AcceptChanges();
                ReceivedRevenue.AcceptChanges();
                BranchWiseData.AcceptChanges();
                DailySales.AcceptChanges();
                MonthlySales.AcceptChanges();
                MnothlyDelivery.AcceptChanges();
                MnothlydraftedInvoice.AcceptChanges();
                todayRevenue.AcceptChanges();
                ReceivedRevenue.AcceptChanges();
                if (CurrentOrder.Rows.Count > 0) Data.Add("orderData", CurrentOrder);
                if (CurrentInvoice.Rows.Count > 0) Data.Add("invoiceData", CurrentInvoice);
                if (CurrentQuotation.Rows.Count > 0) Data.Add("quotationData", CurrentQuotation);
                if (CurrentCustomer.Rows.Count > 0) Data.Add("customerbySource", CurrentCustomer);
                if (OpenOpportunities.Rows.Count > 0) Data.Add("opportunityData", OpenOpportunities);
                if (ReceivedRevenue.Rows.Count > 0) Data.Add("receivedRevenue", ReceivedRevenue);
                if (BranchWiseData.Rows.Count > 0) Data.Add("branchWiseData", BranchWiseData);
                if (DailySales.Rows.Count > 0) Data.Add("dailySales", DailySales);
                if (MonthlySales.Rows.Count > 0) Data.Add("monthlySales", MonthlySales);
                if (MnothlyDelivery.Rows.Count > 0) Data.Add("MnothlyDelivery", MnothlyDelivery);
                if (MnothlydraftedInvoice.Rows.Count > 0) Data.Add("MnothlydraftedInvoice", MnothlydraftedInvoice);
                if (todayRevenue.Rows.Count > 0) Data.Add("todayRevenue", todayRevenue);
                if (ReceivedRvnTdy.Rows.Count > 0) Data.Add("ReceivedRvnTdy", ReceivedRvnTdy);
                if (branchwiseTdy.Rows.Count > 0) Data.Add("branchwiseTdy", branchwiseTdy);
                return Ok(api.Success(Data));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
        [HttpGet("salesOrderList")]
        public ActionResult GetOrderList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
        
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            string crieteria = "";
            if (bAllBranchData == true)
            {
            crieteria="";
           
            }
            else
            {
            crieteria=" and N_BranchID="+nBranchID;
          
            }
             string MonthWiseDate="";
            string YearWiseDate="";

            int nCompanyId = myFunctions.GetCompanyID(User);
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and (X_OrderNo like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by M_NetAmt desc";
            else
                xSortBy = " order by " + xSortBy;
 
        
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                     object StartYear=dLayer.ExecuteScalar("select Year(D_Start) from Acc_FnYear where N_CompanyID ="+nCompanyId+" and N_FnYearID="+nFnYearId+"", Params, connection);
                    object EndDate=dLayer.ExecuteScalar("select CONVERT(varchar,D_END ,23) AS D_End from Acc_FnYear where N_CompanyID = "+nCompanyId+" and N_FnYearID="+nFnYearId+"", Params, connection);
                    object CurrentYear=dLayer.ExecuteScalar("select Year(CURRENT_TIMESTAMP)", Params, connection);
                    if(myFunctions.getIntVAL(StartYear.ToString())!=myFunctions.getIntVAL(CurrentYear.ToString()))
                    {
                        MonthWiseDate="MONTH('"+EndDate+"')";
                        YearWiseDate= " YEAR('"+EndDate+"')";
                    }
                    else
                    {
                        MonthWiseDate="MONTH(CURRENT_TIMESTAMP)";
                        YearWiseDate="YEAR(CURRENT_TIMESTAMP)";

                    }
                   if (Count == 0)
                      sqlCommandText = "select top(10) * from vw_SalesOrder_Dashboard where  YEAR(D_Entrydate) = "+YearWiseDate+" and N_CompanyID = " + nCompanyId + " and N_FnyearID="+nFnYearId + "and N_FormID=81" + crieteria + Searchkey + " " + xSortBy ;
                   else
                      sqlCommandText = "select top(10) * from vw_SalesOrder_Dashboard where YEAR(D_Entrydate) = "+YearWiseDate+" and N_CompanyID = " + nCompanyId + " and N_FnyearID="+nFnYearId + "and N_FormID=81" + crieteria + "  " + Searchkey + " and N_SalesOrderID not in (select top(" + Count + ") N_SalesOrderID from vw_SalesOrder_Dashboard where N_CompanyID=@p1 " + crieteria + xSortBy + " )" + xSortBy;

                   dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);






                    sqlCommandCount = "Select  count(1) from vw_SalesOrder_Dashboard Where YEAR(D_Entrydate) = "+YearWiseDate+" and N_CompanyID = " + nCompanyId + " and N_FnyearID="+nFnYearId + "and N_FormID=81" + crieteria ;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Success(OutPut));
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
        public ActionResult GetQuotationList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
             string crieteria = "";
                 string MonthWiseDate="";
            string YearWiseDate="";
            if (bAllBranchData == true)
            {
            crieteria="";
           
            }
            else
            {
            crieteria=" and N_BranchID="+nBranchID;
          
            }
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and ([Quotation No] like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_BillAmtF desc";
            else
                xSortBy = " order by " + xSortBy;

          
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                   object StartYear=dLayer.ExecuteScalar("select Year(D_Start) from Acc_FnYear where N_CompanyID ="+nCompanyId+" and N_FnYearID="+nFnYearId+"", Params, connection);
                    object EndDate=dLayer.ExecuteScalar("select CONVERT(varchar,D_END ,23) AS D_End from Acc_FnYear where N_CompanyID = "+nCompanyId+" and N_FnYearID="+nFnYearId+"", Params, connection);
                    object CurrentYear=dLayer.ExecuteScalar("select Year(CURRENT_TIMESTAMP)", Params, connection);
                    if(myFunctions.getIntVAL(StartYear.ToString())!=myFunctions.getIntVAL(CurrentYear.ToString()))
                    {
                        MonthWiseDate="MONTH('"+EndDate+"')";
                        YearWiseDate= " YEAR('"+EndDate+"')";
                    }
                    else
                    {
                        MonthWiseDate="MONTH(CURRENT_TIMESTAMP)";
                        YearWiseDate="YEAR(CURRENT_TIMESTAMP)";

                    }


                if (Count == 0)
                    sqlCommandText = "select top(10) * from vw_InvSalesQuotationNo_Search where  YEAR(D_QuotationDate) ="+YearWiseDate+"  and N_CompanyID = " + nCompanyId + " and N_FnyearID="+nFnYearId + crieteria + " " + Searchkey + " " + xSortBy;
                else
                    sqlCommandText = "select top(10) * from vw_InvSalesQuotationNo_Search where YEAR(D_QuotationDate) = "+YearWiseDate+"   and N_CompanyID = " + nCompanyId + " and N_FnyearID="+nFnYearId + crieteria + " " + Searchkey + " and N_QuotationID not in (select top(" + Count + ") N_QuotationID from vw_InvSalesQuotationNo_Search where N_CompanyID=@p1  and N_FnyearID="+nFnYearId + crieteria + xSortBy + " ) " + xSortBy;




                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = api.Format(dt);
                    sqlCommandCount = "Select  count(1) from vw_InvSalesQuotationNo_Search Where  YEAR(D_QuotationDate) = "+YearWiseDate+"  and N_CompanyID = " + nCompanyId + " and N_FnyearID="+nFnYearId + crieteria + "  ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                       return Ok(api.Success(OutPut));
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
        public ActionResult GetInvoiceList(int nFnYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            string crieteria = "";
                string MonthWiseDate="";
            string YearWiseDate="";
            if (bAllBranchData == true)
            {
            crieteria="";
           
            }
            else
            {
            crieteria=" and N_BranchID="+nBranchID;
          
            }
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and ([Invoice No] like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_BillAmt desc";
            else
                xSortBy = " order by " + xSortBy;

          
            Params.Add("@p1", nCompanyID);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    object StartYear=dLayer.ExecuteScalar("select Year(D_Start) from Acc_FnYear where N_CompanyID ="+nCompanyID+" and N_FnYearID="+nFnYearID+"", Params, connection);
                    object EndDate=dLayer.ExecuteScalar("select CONVERT(varchar,D_END ,23) AS D_End from Acc_FnYear where N_CompanyID = "+nCompanyID+" and N_FnYearID="+nFnYearID+"", Params, connection);
                    object CurrentYear=dLayer.ExecuteScalar("select Year(CURRENT_TIMESTAMP)", Params, connection);
                    if(myFunctions.getIntVAL(StartYear.ToString())!=myFunctions.getIntVAL(CurrentYear.ToString()))
                    {
                        MonthWiseDate="MONTH('"+EndDate+"')";
                        YearWiseDate= " YEAR('"+EndDate+"')";
                    }
                    else
                    {
                        MonthWiseDate="MONTH(CURRENT_TIMESTAMP)";
                        YearWiseDate="YEAR(CURRENT_TIMESTAMP)";

                    }


                   if (Count == 0)
                        sqlCommandText = "select top(10) N_BillAmt=Cast(REPLACE(X_BillAmt,',','') as Numeric(10,2)),* from vw_InvSalesInvoiceNo_Search_Cloud where  YEAR(D_SalesDate) = "+YearWiseDate+"  and N_FnyearID="+nFnYearID + crieteria + " " + Searchkey + " " + xSortBy;
                   else
                        sqlCommandText = "select top(10) N_BillAmt=Cast(REPLACE(X_BillAmt,',','') as Numeric(10,2)),* from vw_InvSalesInvoiceNo_Search_Cloud where YEAR(D_SalesDate) = "+YearWiseDate+"  and N_FnyearID="+nFnYearID + crieteria + " " + Searchkey + " and N_SalesId not in (select top(" + Count + ") N_SalesId from vw_InvSalesInvoiceNo_Search_Cloud where N_FnyearID="+nFnYearID + crieteria + xSortBy + " ) " + xSortBy;


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = api.Format(dt);
                    sqlCommandCount = "Select count(1) from vw_InvSalesInvoiceNo_Search_Cloud Where  YEAR(D_SalesDate) = "+YearWiseDate+"  and  N_FnyearID="+nFnYearID + crieteria;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                       return Ok(api.Success(OutPut));
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


          [HttpGet("salesDivisionInvoiceList")]
        public ActionResult GetDivisionInvoiceList(int nFnYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            string crieteria = "";
            string MonthWiseDate="";
            string YearWiseDate="";
            if (bAllBranchData == true)
            {
            crieteria="";
           
            }
            else
            {
            crieteria=" and N_BranchID="+nBranchID;
          
            }
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and (x_DivisionCode like '%" + xSearchkey + "%' or x_DivisionName like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by x_BillAmt desc";
            else
                xSortBy = " order by " + xSortBy;

          
            Params.Add("@p1", nCompanyID);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                   if (Count == 0)
                        sqlCommandText = "select * from vw_InvDivisionWise_Sales where  x_DivisionName!=''  and N_FnyearID="+nFnYearID + crieteria + " " + Searchkey + " " + xSortBy;
                   else
                        sqlCommandText = "select * from vw_InvDivisionWise_Sales where  x_DivisionName!='' and N_FnyearID="+nFnYearID + crieteria + " " + Searchkey + " " + xSortBy;


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = api.Format(dt);
                    sqlCommandCount = "Select count(1) from vw_InvDivisionWise_Sales Where  x_DivisionName!='' and  N_FnyearID="+nFnYearID + crieteria;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                       return Ok(api.Success(OutPut));
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


        [HttpGet("salesDivisionBranchInvoiceList")]
        public ActionResult GetDivisionBranchInvoiceList(int nFnYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData,int nDivision)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            string crieteria = "";
            string MonthWiseDate="";
            string YearWiseDate="";
            if (bAllBranchData == true)
            {
            crieteria="";
           
            }
            else
            {
            crieteria=" and N_BranchID="+nBranchID;
          
            }
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and (X_DivisionName like '%" + xSearchkey + "%' OR x_BranchName like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by x_BillAmt desc";
            else
                xSortBy = " order by " + xSortBy;

          
            Params.Add("@p1", nCompanyID);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                   if (Count == 0)
                        sqlCommandText = "select * from vw_InvBranchWise_Sales where  x_DivisionName!=''  and N_FnyearID="+nFnYearID+" and n_DivisionID ="+nDivision + crieteria + " " + Searchkey + " " + xSortBy;
                   else
                        sqlCommandText = "select * from vw_InvBranchWise_Sales where  x_DivisionName!='' and N_FnyearID="+nFnYearID+" and n_DivisionID ="+nDivision + crieteria + " " + Searchkey + " " + xSortBy;


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = api.Format(dt);
                    sqlCommandCount = "Select count(1) from vw_InvBranchWise_Sales Where  x_DivisionName!='' and  N_FnyearID="+nFnYearID + " and n_DivisionID ="+nDivision+crieteria;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                       return Ok(api.Success(OutPut));
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

        [HttpGet("salesBranchInvoiceList")]
        public ActionResult GetBranchInvoiceList(int nFnYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData,int nDivision,int nDivBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            string crieteria = "";
            string MonthWiseDate="";
            string YearWiseDate="";
            if (bAllBranchData == true)
            {
            crieteria="";
           
            }
            else
            {
            crieteria=" and N_BranchID="+nBranchID;
          
            }
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and (invoiceNo like '%" + xSearchkey + "%' OR customner like '%" + xSearchkey + "%' OR invoiceDate like '%" + xSearchkey + "%' OR d_CustPODate like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by x_BillAmt desc";
            else
                xSortBy = " order by " + xSortBy;

          
            Params.Add("@p1", nCompanyID);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                   if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvSales_BranchByDivision where  N_FnyearID="+nFnYearID+" and N_BranchID="+nDivBranchID+" and n_DivisionID ="+nDivision + crieteria + " " + Searchkey + " " + xSortBy;
                   else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvSales_BranchByDivision where  N_FnyearID="+nFnYearID+" and N_BranchID="+nDivBranchID+" and n_DivisionID ="+nDivision + crieteria + " " + Searchkey + " and N_SalesId not in (select top(" + Count + ") N_SalesId from vw_InvSales_BranchByDivision where N_FnyearID="+nFnYearID + " and N_BranchID="+nDivBranchID+" and n_DivisionID ="+nDivision + crieteria + xSortBy + " ) " + xSortBy;


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = api.Format(dt);
                    sqlCommandCount = "Select count(1) from vw_InvSales_BranchByDivision Where  N_FnyearID="+nFnYearID + " and N_BranchID="+nDivBranchID+" and n_DivisionID ="+nDivision+crieteria;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                       return Ok(api.Success(OutPut));
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

        

    }
}