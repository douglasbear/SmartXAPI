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
    [Route("puchaseDashboard")]
    [ApiController]
    public class PurchaseDashboard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public PurchaseDashboard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("details")]
        public ActionResult GetDashboardDetails(int nFnYearId,int nBranchID,bool bAllBranchData)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            string crieteria="";
          
            if (bAllBranchData == true)
            {
            crieteria="";
          
            }
            else
            {
            crieteria=" and N_BranchID="+nBranchID;
          
            }


             
            string sqlCurrentOrder = "SELECT COUNT(*) as N_ThisMonth,sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvPurchaseOrderNo_Search WHERE MONTH(Cast([Order Date] as DateTime))= MONTH(CURRENT_TIMESTAMP) AND YEAR(Cast([Order Date] as DateTime))= YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyID + " and N_FnYearID="+nFnYearId + crieteria ;
            string sqlCurrentInvoice = "SELECT COUNT(*) as N_ThisMonth,sum(Cast(REPLACE(InvoiceNetAmt,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvPurchaseInvoiceNo_Search WHERE MONTH(Cast([Invoice Date] as DateTime)) = MONTH(CURRENT_TIMESTAMP) AND YEAR(Cast([Invoice Date] as DateTime)) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyID + " and N_PurchaseType = 0 "+ " and N_FnYearID="+nFnYearId+crieteria;

            string sqlTopVendors = "select top(5) Vendor as X_TopVendors,CAST(COUNT(*) as varchar(50)) as N_Count from vw_InvPurchaseInvoiceNo_Search where N_CompanyID = " + nCompanyID + "and N_FnYearID="+nFnYearId+ crieteria + " group by Vendor order by COUNT(*) Desc";
            string sqlDraftedInvoice = "select COUNT(N_PurchaseID) as N_DraftedInvoice from Inv_Purchase where MONTH(Cast(D_InvoiceDate as DateTime)) = MONTH(CURRENT_TIMESTAMP) and YEAR(D_InvoiceDate)= YEAR(CURRENT_TIMESTAMP) and ISNULL(B_IsSaveDraft,0)=1 and X_TransType='PURCHASE' and N_CompanyID = " + nCompanyID +" and N_FnYearID="+nFnYearId + crieteria;
            string sqlUnprocessedOrder = "select COUNT(N_POrderID) as N_UnProcessed from Inv_PurchaseOrder where MONTH(Cast(D_POrderDate as DateTime)) = MONTH(CURRENT_TIMESTAMP) and YEAR(D_POrderDate)= YEAR(CURRENT_TIMESTAMP) and ISNULL(N_Processed,0)=0 and (D_ExDelvDate < getDate()) and N_CompanyID=" + nCompanyID + "and N_FnYearID="+nFnYearId + crieteria;           

            SortedList Data=new SortedList();
            DataTable CurrentOrder = new DataTable();
            DataTable CurrentInvoice = new DataTable();
            DataTable TopVendor = new DataTable();
            DataTable DraftedInvoice = new DataTable();
            DataTable UnprocessedOrder = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    CurrentOrder = dLayer.ExecuteDataTable(sqlCurrentOrder, Params, connection);
                    CurrentInvoice = dLayer.ExecuteDataTable(sqlCurrentInvoice, Params, connection);
                    TopVendor = dLayer.ExecuteDataTable(sqlTopVendors, Params, connection);
                    DraftedInvoice = dLayer.ExecuteDataTable(sqlDraftedInvoice, Params, connection);
                    UnprocessedOrder = dLayer.ExecuteDataTable(sqlUnprocessedOrder, Params, connection);
                                  
                }             
                CurrentOrder.AcceptChanges();
                CurrentInvoice.AcceptChanges();
                TopVendor.AcceptChanges();
                DraftedInvoice.AcceptChanges();
                UnprocessedOrder.AcceptChanges();

                if(CurrentOrder.Rows.Count>0)Data.Add("orderData",CurrentOrder);
                if(CurrentInvoice.Rows.Count>0)Data.Add("invoiceData",CurrentInvoice);
                if(TopVendor.Rows.Count>0)Data.Add("topVendors",TopVendor);
                if(DraftedInvoice.Rows.Count>0)Data.Add("draftedInvoice",DraftedInvoice);
                if(UnprocessedOrder.Rows.Count>0)Data.Add("unprocessedOrder",UnprocessedOrder);
               
                return Ok(api.Success(Data));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("PurchaseOrderList")]
        public ActionResult GetOrderList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
        
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            int nCompanyId = myFunctions.GetCompanyID(User);
            string crieteria="";
          
            if (bAllBranchData == true)
            {
            crieteria="";
          
            }
            else
            {
            crieteria=" and N_BranchID="+nBranchID;
          
            }
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_POrderNo like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by M_Amount desc";
            else
                xSortBy = " order by " + xSortBy;
 
            if (Count == 0)
                sqlCommandText = "select top(10) * from vw_InvPurchaseOrderNo_Search where  YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + " and N_FnYearID = " + nFnYearId + crieteria+"  " + Searchkey + " " + xSortBy ;
            else
                sqlCommandText = "select top(10) * from vw_InvPurchaseOrderNo_Search where YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + " and N_FnYearID = " + nFnYearId + crieteria+"  " + Searchkey + " and N_POrderID not in (select top(" + Count + ") N_POrderID from vw_InvPurchaseOrderNo_Search where N_CompanyID=@p1 " + xSortBy + " )" + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "Select * from vw_InvPurchaseOrderNo_Search Where MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + " and N_FnYearID = " + nFnYearId+crieteria;
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



          [HttpGet("PurchaseInvoiceList")]
        public ActionResult GetInvoiceList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
        
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            int nCompanyId = myFunctions.GetCompanyID(User);
            string crieteria="";
          
            if (bAllBranchData == true)
            {
            crieteria="";
          
            }
            else
            {
            crieteria=" and N_BranchID="+nBranchID;
          
            }
          
 
            if (Count == 0)
                sqlCommandText = "select top(10)  N_PurchaseID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],InvoiceNetAmt,X_BranchName,X_Description,N_PaymentMethod,N_FnYearID,N_BranchID,N_LocationID,N_VendorID,N_InvDueDays,B_IsSaveDraft,N_BalanceAmt,X_DueDate,X_POrderNo,d_PrintDate,n_InvoiceAmt from vw_InvPurchaseInvoiceNo_Search_Cloud where  YEAR([Invoice Date]) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + " and N_FnYearID = " + nFnYearId + crieteria+"  " + Searchkey + " " + xSortBy ;
            else
                sqlCommandText = "select top(10) N_PurchaseID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],InvoiceNetAmt,X_BranchName,X_Description,N_PaymentMethod,N_FnYearID,N_BranchID,N_LocationID,N_VendorID,N_InvDueDays,B_IsSaveDraft,N_BalanceAmt,X_DueDate,X_POrderNo,d_PrintDate,n_InvoiceAmt from vw_InvPurchaseInvoiceNo_Search_Cloud where  YEAR([Invoice Date]) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + " and N_FnYearID = " + nFnYearId + crieteria+"  " + Searchkey + " and N_PurchaseID not in (select top(" + Count + ") N_PurchaseID from vw_InvPurchaseInvoiceNo_Search_Cloud where N_CompanyID=@p1 " + xSortBy + " )" + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "Select count(*) as N_Count,sum(Cast(REPLACE(InvoiceNetAmt,',','') as Numeric(10,2)) ) as TotalAmount from vw_InvPurchaseInvoiceNo_Search_Cloud Where YEAR([Invoice Date]) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + " and N_FnYearID = " + nFnYearId+crieteria;
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

    }
}

  
