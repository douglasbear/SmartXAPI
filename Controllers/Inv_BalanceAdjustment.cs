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
    [Route("BalanceAdjustment")]
    [ApiController]

 

    public class Inv_BalanceAdjustment : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        private readonly IMyAttachments myAttachments;


        public Inv_BalanceAdjustment(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf,IMyAttachments myAtt)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        //List
        [HttpGet("list")]
        public ActionResult GetBalanceDetails(int nFnyearID, int nPartyType, int nPartyID, int N_TransType, int nPage, int nSizeperpage,string xSearchkey, string xSortBy,bool bAllBranchData,int nBranchID, int nFormID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string Searchkey = "";
            if (nPartyType ==1){
                if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([Invoice No] like '%" + xSearchkey + "%' or [Customer Name] like '%"+ xSearchkey + "%' or cast([Adjustment Date] as VarChar) like '%" + xSearchkey + "%' or X_Notes like '%"+ xSearchkey + "%' or [Net Amount] like '%"+ xSearchkey + "%')";
            }
            else {
                if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([Invoice No] like '%" + xSearchkey + "%' or X_VendorName like '%"+ xSearchkey + "%' or cast([Adjustment Date] as VarChar) like '%" + xSearchkey + "%' or X_Notes like '%"+ xSearchkey + "%' or Netamt like '%"+ xSearchkey + "%')";
            }
            
             if (bAllBranchData == true)
                        {
                            Searchkey = Searchkey + " ";
                        }
                        else
                        {
                            Searchkey = Searchkey + " and N_BranchID=" + nBranchID + " ";
                        }
            if (xSortBy == null || xSortBy.Trim() == ""){
                xSortBy = " order by [Invoice No] desc";
            }
            else
            {
             switch (xSortBy.Split(" ")[0]){
                  
                   case "customerName" : xSortBy =" [Customer Name] " + xSortBy.Split(" ")[1] ;
                   break;
                   case "invoiceNo" : xSortBy =" [Invoice No] " + xSortBy.Split(" ")[1] ;
                   break;
                   case "adjustmentDate":
                   xSortBy = "Cast([Adjustment Date] as DateTime )" + xSortBy.Split(" ")[1];
                    break;
                   case "netAmount":
                    xSortBy = "Cast(REPLACE([Net Amount],',','') as Numeric(10,2))" + xSortBy.Split(" ")[1];
                    break; 
                   
                   default : break;
               }
             xSortBy = " order by " + xSortBy;
            }

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            if (Count == 0)
            {
                if (nPartyType ==1)
                    sqlCommandText = "select top(" + nSizeperpage + ") [Adjustment Date],[Invoice No],[Customer Name],[Net Amount],X_Notes,N_BillAmtF,X_CustomerName_Ar from vw_CustomerBalanceAdjustment where N_CompanyID=@p1 and N_FnYearID=@p6  and N_TransType=@p4  and N_PartyType=@p5 and N_FormID=@p7 " + Searchkey + " " + xSortBy;
                else
                    sqlCommandText = "select top(" + nSizeperpage + ") [Adjustment Date],[Invoice No],X_VendorName,Netamt as netAmount,X_Notes,X_VendorName_Ar from vw_VendorBalanceAdjustment where N_CompanyID=@p1 and N_FnYearID=@p6  and N_TransType=@p4  and N_PartyType=@p5 and N_FormID=@p7 " + Searchkey + " " + xSortBy;
            }
            else
            {
                if (nPartyType ==1)
                    sqlCommandText = "select top(" + nSizeperpage + ") [Adjustment Date],[Invoice No],[Customer Name],[Net Amount],X_Notes,N_BillAmtF,X_CustomerName_Ar from vw_CustomerBalanceAdjustment where N_CompanyID=@p1 and N_FnYearID=@p6 and N_FormID=@p7  " + Searchkey + " and N_TransType=@p4  and N_PartyType=@p5 and [Invoice No] not in (select top(" + Count + ") [Invoice No] from vw_CustomerBalanceAdjustment where N_CompanyID=@p1 and N_FnYearID=@p6 and N_TransType=@p4 and N_PartyType=@p5 and N_FormID=@p7 " + xSortBy + " ) " + xSortBy;
                else
                    sqlCommandText = "select top(" + nSizeperpage + ") [Adjustment Date],[Invoice No],X_VendorName,Netamt as netAmount,X_Notes,X_VendorName_Ar from vw_VendorBalanceAdjustment where N_CompanyID=@p1 and N_FnYearID=@p6 and N_FormID=@p7 " + Searchkey + " and N_TransType=@p4  and N_PartyType=@p5 and [Invoice No] not in (select top(" + Count + ") [Invoice No] from vw_VendorBalanceAdjustment where N_CompanyID=@p1 and N_FnYearID=@p6 and N_TransType=@p4 and N_PartyType=@p5 and N_FormID=@p7 " + xSortBy + " ) " + xSortBy;
            }
            Params.Add("@p1", nCompanyID);
            Params.Add("@p4", N_TransType);
            Params.Add("@p5", nPartyType);
            Params.Add("@p6", nFnyearID);
            Params.Add("@p7", nFormID);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    if (nPartyType ==1)
                        sqlCommandCount = "select count(1) as N_Count,sum(Cast(REPLACE([Net Amount],',','') as Numeric(10,2)) ) as TotalAmount from vw_CustomerBalanceAdjustment where N_CompanyID=@p1 and N_FnYearID=@p6  and N_TransType=@p4 and N_PartyType=@p5 " + Searchkey + "";
                    else
                        sqlCommandCount = "select count(1) as N_Count,sum(Cast(REPLACE(Netamt,',','') as Numeric(10,2)) ) as TotalAmount from vw_VendorBalanceAdjustment where N_CompanyID=@p1 and N_FnYearID=@p6 and N_TransType=@p4 and N_PartyType=@p5 " + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount="0";
                    string TotalSum="0";
                    if(Summary.Rows.Count>0){
                    DataRow drow = Summary.Rows[0];
                    TotalCount = drow["N_Count"].ToString();
                    TotalSum = drow["TotalAmount"].ToString();
                    }
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    OutPut.Add("TotalSum", TotalSum);
                }
                if (dt.Rows.Count == 0)
                {
                    //return Ok(_api.Warning("No Results Found"));
                     return Ok(_api.Success(OutPut));
                }
                else
                {
                    return Ok(_api.Success(OutPut));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
        [HttpGet("listDetails")]
        public ActionResult GetBalanceListDetails(int N_PartyType, string N_TransType, int nFnYearId, string X_ReceiptNo, bool bAllBranchData, int nBranchID, int nFormID, int nSalesOrderId)
        {
              if (X_ReceiptNo != null)
                X_ReceiptNo = X_ReceiptNo.Replace("%2F", "/");
            int nCompanyId = myFunctions.GetCompanyID(User);
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            // DataTable DataTable = new DataTable();
            DataTable Acc_CostCentreTrans = new DataTable();
            int N_AdjustmentID = 0;
            string Mastersql = "";
            
            if (bAllBranchData == true)
            {
                if(nSalesOrderId>0)
                {
                    Mastersql = "Select * from vw_SalesOrderToCustomerDebitNote where N_CompanyID=@p1 and n_SalesOrderId=@p8";
               
                }
                 else {
              
                if (N_PartyType == 1)
                    Mastersql = "Select Inv_Customer.*,Inv_BalanceAdjustmentMaster.*,Inv_CustomerProjects.X_ProjectName,Inv_CustomerProjects.X_ProjectCode from Inv_BalanceAdjustmentMaster Inner Join  Inv_Customer ON Inv_BalanceAdjustmentMaster.N_PartyID = Inv_Customer.N_CustomerID And Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_Customer.N_CompanyID and Inv_BalanceAdjustmentMaster.N_FnYearID=Inv_Customer.N_FnYearID  Left Outer Join Inv_CustomerProjects ON Inv_BalanceAdjustmentMaster.N_ProjectID = Inv_CustomerProjects.N_ProjectID And Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_CustomerProjects.N_CompanyID Where  Inv_BalanceAdjustmentMaster.N_CompanyID=@p1 and Inv_BalanceAdjustmentMaster.N_TransType =@p2 and Inv_BalanceAdjustmentMaster.N_FnYearID=@p3 and Inv_BalanceAdjustmentMaster.X_VoucherNo=@p4 and Inv_BalanceAdjustmentMaster.N_PartyType=@p5";
                else if (N_PartyType == 0)
                    Mastersql = "SELECT     Inv_BalanceAdjustmentMaster.N_CompanyID, Inv_BalanceAdjustmentMaster.N_FnYearID, Inv_BalanceAdjustmentMaster.X_VoucherNo, Inv_BalanceAdjustmentMaster.D_AdjustmentDate, Inv_BalanceAdjustmentMaster.D_EntryDate, Inv_BalanceAdjustmentMaster.N_Amount, Inv_BalanceAdjustmentMaster.N_UserID, Inv_BalanceAdjustmentMaster.N_BranchID, Inv_BalanceAdjustmentMaster.N_TransType, Inv_BalanceAdjustmentMaster.X_notes, Inv_BalanceAdjustmentMaster.N_PartyType, Inv_BalanceAdjustmentMaster.N_PartyID, Inv_BalanceAdjustmentMaster.N_AdjustmentId, Inv_BalanceAdjustmentMaster.N_AmountF, Inv_BalanceAdjustmentMaster.N_CurExchRate, Inv_BalanceAdjustmentMaster.N_WOID,Inv_BalanceAdjustmentMaster.N_ProjectID, Inv_Vendor.N_CompanyID AS Expr1, Inv_Vendor.N_VendorID, Inv_Vendor.X_VendorCode, Inv_Vendor.X_VendorName, Inv_Vendor.X_ContactName, Inv_Vendor.X_Address, Inv_Vendor.X_ZipCode, Inv_Vendor.X_PhoneNo1, Inv_Vendor.X_PhoneNo2, Inv_Vendor.X_FaxNo, Inv_Vendor.X_Email, Inv_Vendor.X_WebSite, Inv_Vendor.N_CreditLimit, Inv_Vendor.B_Inactive, Inv_Vendor.N_LedgerID, Inv_Vendor.N_InvDueDays, Inv_Vendor.N_FnYearID AS Expr2, Inv_Vendor.D_Entrydate AS Expr3, Inv_Vendor.N_CountryID, Inv_Vendor.N_TypeID, Inv_Vendor.B_DirPosting, Inv_Vendor.N_CurrencyID, Inv_Vendor.X_ReminderMsg, Inv_Vendor.X_VendorName_Ar, Inv_Vendor.N_CountryID, Inv_Vendor.X_TaxRegistrationNo, Inv_Vendor.N_TaxCategoryID, Inv_Vendor.B_AllowCashPay, Inv_Vendor.N_PartnerTypeID, Inv_Vendor.N_VendorTypeID, Inv_Vendor.N_GoodsDeliveryIn, Inv_Vendor.X_TandC, Acc_CurrencyMaster.N_CompanyID AS Expr4, Acc_CurrencyMaster.N_CurrencyID AS Expr5, Acc_CurrencyMaster.X_CurrencyCode, Acc_CurrencyMaster.X_CurrencyName, Acc_CurrencyMaster.X_ShortName, Acc_CurrencyMaster.N_ExchangeRate, Acc_CurrencyMaster.B_default, Acc_CurrencyMaster.N_Decimal, Prj_WorkOrder.N_WorkOrderId, Prj_WorkOrder.X_WorkOrderNo,Inv_CustomerProjects.X_ProjectName,Inv_CustomerProjects.X_ProjectCode FROM         Inv_BalanceAdjustmentMaster INNER JOIN Inv_Vendor ON Inv_BalanceAdjustmentMaster.N_PartyID = Inv_Vendor.N_VendorID AND Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_Vendor.N_CompanyID AND  Inv_BalanceAdjustmentMaster.N_FnYearID = Inv_Vendor.N_FnYearID INNER JOIN Acc_CurrencyMaster ON Inv_Vendor.N_CurrencyID = Acc_CurrencyMaster.N_CurrencyID LEFT OUTER JOIN Prj_WorkOrder ON Inv_BalanceAdjustmentMaster.N_CompanyID = Prj_WorkOrder.N_CompanyId AND Inv_BalanceAdjustmentMaster.N_WOID = Prj_WorkOrder.N_WorkOrderId Left Outer Join Inv_CustomerProjects ON Inv_BalanceAdjustmentMaster.N_ProjectID = Inv_CustomerProjects.N_ProjectID And Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_CustomerProjects.N_CompanyID Where  Inv_BalanceAdjustmentMaster.N_CompanyID=@p1 and Inv_BalanceAdjustmentMaster.N_FnYearID=@p3 and Inv_BalanceAdjustmentMaster.N_TransType=@p2 and Inv_BalanceAdjustmentMaster.X_VoucherNo=@p4 and Inv_BalanceAdjustmentMaster.N_PartyType=@p5";

                 }

            }
            else
            {
               
                if (N_PartyType == 1)
                    Mastersql = "Select Inv_Customer.*,Inv_BalanceAdjustmentMaster.*,Inv_CustomerProjects.X_ProjectName,Inv_CustomerProjects.X_ProjectCode from Inv_BalanceAdjustmentMaster Inner Join  Inv_Customer ON Inv_BalanceAdjustmentMaster.N_PartyID = Inv_Customer.N_CustomerID And Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_Customer.N_CompanyID and Inv_BalanceAdjustmentMaster.N_FnYearID=Inv_Customer.N_FnYearID  Left Outer Join Inv_CustomerProjects ON Inv_BalanceAdjustmentMaster.N_ProjectID = Inv_CustomerProjects.N_ProjectID And Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_CustomerProjects.N_CompanyID Where  Inv_BalanceAdjustmentMaster.N_CompanyID=@p1 and Inv_BalanceAdjustmentMaster.N_TransType =@p2 and Inv_BalanceAdjustmentMaster.N_FnYearID=@p3 and Inv_BalanceAdjustmentMaster.X_VoucherNo=@p4 and Inv_BalanceAdjustmentMaster.N_PartyType=@p5 and Inv_BalanceAdjustmentMaster.N_BranchID=@p6";
                else if (N_PartyType == 0)
                    Mastersql = "SELECT     Inv_BalanceAdjustmentMaster.N_CompanyID, Inv_BalanceAdjustmentMaster.N_FnYearID, Inv_BalanceAdjustmentMaster.X_VoucherNo, Inv_BalanceAdjustmentMaster.D_AdjustmentDate, Inv_BalanceAdjustmentMaster.D_EntryDate, Inv_BalanceAdjustmentMaster.N_Amount, Inv_BalanceAdjustmentMaster.N_UserID, Inv_BalanceAdjustmentMaster.N_BranchID, Inv_BalanceAdjustmentMaster.N_TransType, Inv_BalanceAdjustmentMaster.X_notes, Inv_BalanceAdjustmentMaster.N_PartyType, Inv_BalanceAdjustmentMaster.N_PartyID, Inv_BalanceAdjustmentMaster.N_AdjustmentId, Inv_BalanceAdjustmentMaster.N_AmountF, Inv_BalanceAdjustmentMaster.N_CurExchRate, Inv_BalanceAdjustmentMaster.N_WOID,Inv_BalanceAdjustmentMaster.N_ProjectID, Inv_Vendor.N_CompanyID AS Expr1, Inv_Vendor.N_VendorID, Inv_Vendor.X_VendorCode, Inv_Vendor.X_VendorName, Inv_Vendor.X_ContactName, Inv_Vendor.X_Address, Inv_Vendor.X_ZipCode, Inv_Vendor.X_PhoneNo1, Inv_Vendor.X_PhoneNo2, Inv_Vendor.X_FaxNo, Inv_Vendor.X_Email, Inv_Vendor.X_WebSite, Inv_Vendor.N_CreditLimit, Inv_Vendor.B_Inactive, Inv_Vendor.N_LedgerID, Inv_Vendor.N_InvDueDays, Inv_Vendor.N_FnYearID AS Expr2, Inv_Vendor.D_Entrydate AS Expr3, Inv_Vendor.N_CountryID, Inv_Vendor.N_TypeID, Inv_Vendor.B_DirPosting, Inv_Vendor.N_CurrencyID, Inv_Vendor.X_ReminderMsg, Inv_Vendor.X_VendorName_Ar, Inv_Vendor.N_CountryID, Inv_Vendor.X_TaxRegistrationNo, Inv_Vendor.N_TaxCategoryID, Inv_Vendor.B_AllowCashPay, Inv_Vendor.N_PartnerTypeID, Inv_Vendor.N_VendorTypeID, Inv_Vendor.N_GoodsDeliveryIn, Inv_Vendor.X_TandC, Acc_CurrencyMaster.N_CompanyID AS Expr4, Acc_CurrencyMaster.N_CurrencyID AS Expr5, Acc_CurrencyMaster.X_CurrencyCode, Acc_CurrencyMaster.X_CurrencyName, Acc_CurrencyMaster.X_ShortName, Acc_CurrencyMaster.N_ExchangeRate, Acc_CurrencyMaster.B_default, Acc_CurrencyMaster.N_Decimal, Prj_WorkOrder.N_WorkOrderId, Prj_WorkOrder.X_WorkOrderNo,Inv_CustomerProjects.X_ProjectName,Inv_CustomerProjects.X_ProjectCode FROM         Inv_BalanceAdjustmentMaster INNER JOIN Inv_Vendor ON Inv_BalanceAdjustmentMaster.N_PartyID = Inv_Vendor.N_VendorID AND Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_Vendor.N_CompanyID AND  Inv_BalanceAdjustmentMaster.N_FnYearID = Inv_Vendor.N_FnYearID INNER JOIN Acc_CurrencyMaster ON Inv_Vendor.N_CurrencyID = Acc_CurrencyMaster.N_CurrencyID LEFT OUTER JOIN Prj_WorkOrder ON Inv_BalanceAdjustmentMaster.N_CompanyID = Prj_WorkOrder.N_CompanyId AND Inv_BalanceAdjustmentMaster.N_WOID = Prj_WorkOrder.N_WorkOrderId Left Outer Join Inv_CustomerProjects ON Inv_BalanceAdjustmentMaster.N_ProjectID = Inv_CustomerProjects.N_ProjectID And Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_CustomerProjects.N_CompanyID Where  Inv_BalanceAdjustmentMaster.N_CompanyID=@p1 and Inv_BalanceAdjustmentMaster.N_FnYearID=@p3 and Inv_BalanceAdjustmentMaster.N_TransType=@p2 and Inv_BalanceAdjustmentMaster.X_VoucherNo=@p4 and Inv_BalanceAdjustmentMaster.N_PartyType=@p5 and Inv_BalanceAdjustmentMaster.N_BranchID=@p6";
                Params.Add("@p6", nBranchID);
            }
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", N_TransType);
            Params.Add("@p3", nFnYearId);
            Params.Add("@p4", X_ReceiptNo);
            Params.Add("@p5", N_PartyType);
            Params.Add("@p7", nFormID);
            Params.Add("@p8", nSalesOrderId);
            

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    N_AdjustmentID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AdjustmentID"].ToString());

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Data Found !!"));
                    }

                    MasterTable = _api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                    //BalaceAdjustment Details
                    int N_AdjustmentId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AdjustmentId"].ToString());

                    string DetailSql = "";

                   

                    //             DetailSql = "Select Inv_BalanceAdjustmentMasterDetails.*,Acc_MastLedger.* from Inv_BalanceAdjustmentMasterDetails " +
                    // " Left Outer JOIN Acc_MastLedger On Inv_BalanceAdjustmentMasterDetails.N_LedgerID= Acc_MastLedger.N_LedgerID and Inv_BalanceAdjustmentMasterDetails.N_CompanyID = Acc_MastLedger.N_CompanyID" +
                    // " Where Inv_BalanceAdjustmentMasterDetails.N_CompanyID=@p1 and  Acc_MastLedger.N_FnYearID=@p3 and Inv_BalanceAdjustmentMasterDetails.N_AdjustmentId=" + N_AdjustmentId;
                    DetailSql = "Select * from vw_InvBalanceAdjustmentDetaiils  Where N_CompanyID=@p1 and  N_FnYearID=@p3 and N_AdjustmentId=" + N_AdjustmentId;
                   if(nSalesOrderId>0)
                     {
                          DetailSql = "Select * from vw_SalesOrderToDebitNoteDetails  Where N_CompanyID=@p1 and N_FnYearID=@p3 and  N_ReferanceID=" + nSalesOrderId + " and N_PaidAmt=0 and n_TypeID=468";
                  
                     }
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);

                    string CostcenterSql = "SELECT X_EmpCode, X_EmpName, N_ProjectID as N_Segment_3,N_EmpID as N_Segment_4, X_ProjectCode,X_ProjectName,N_EmpID,N_ProjectID,N_CompanyID,N_FnYearID, " +
                        " N_VoucherID, N_VoucherDetailsID, N_CostCentreID,X_CostCentreName,X_CostCentreCode,N_BranchID,X_BranchName,X_BranchCode , " +
                        " N_Amount, N_LedgerID, N_CostCenterTransID, N_GridLineNo,X_Naration,0 AS N_AssetID, '' As X_AssetCode, " +
                        " GETDATE() AS D_RepaymentDate, '' AS X_AssetName,'' AS X_PayCode,0 AS N_PayID,0 AS N_Inst,CAST(0 AS BIT) AS B_IsCategory,D_Entrydate, N_FormID " +
                        " FROM   vw_InvFreeTextPurchaseCostCentreDetails where N_InventoryID = " + N_AdjustmentID + " And N_InventoryType=0 And N_FnYearID=" + nFnYearId +
                        " And N_CompanyID=" + nCompanyId + " and N_FormID=@p7 Order By N_InventoryID,N_VoucherDetailsID ";

                    Acc_CostCentreTrans = dLayer.ExecuteDataTable(CostcenterSql, Params, connection);

                    Acc_CostCentreTrans = _api.Format(Acc_CostCentreTrans, "costCenterTrans");
                    dt.Tables.Add(Acc_CostCentreTrans);

                    // DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(MasterTable.Rows[0]["N_AdjustmentId"].ToString()), myFunctions.getIntVAL(MasterTable.Rows[0]["N_AdjustmentId"].ToString()), this.FormID, myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString()), User, connection);
                    // Attachments = _api.Format(Attachments, "attachments");
                    // dt.Tables.Add(Attachments);
                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                DataTable CostCenterTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                CostCenterTable = ds.Tables["CostCenterTable"];
                SortedList Params = new SortedList();
                string xButtonAction = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    DataRow MasterRow = MasterTable.Rows[0];
                    transaction = connection.BeginTransaction();

                    // DataTable Attachment = ds.Tables["attachments"];
                    int N_AdjustmentID = myFunctions.getIntVAL(MasterRow["n_AdjustmentId"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int N_BranchID = myFunctions.getIntVAL(MasterRow["n_BranchID"].ToString());
                    int N_TransType = myFunctions.getIntVAL(MasterRow["n_TransType"].ToString());
                    int N_PartyType = myFunctions.getIntVAL(MasterRow["n_PartyType"].ToString());
                    int N_FormID = myFunctions.getIntVAL(MasterRow["N_FormID"].ToString());
                    string X_Trasnaction = "";
                    //string xButtonAction="";
             
                    int N_PartyID = myFunctions.getIntVAL(MasterRow["n_PartyID"].ToString());
                    int N_IsImport = 0;
                    if (N_FormID==1515) N_IsImport = 1;
                     SortedList PostingParam = new SortedList();
                      SortedList QueryParams = new SortedList();

                       QueryParams["@N_CompanyID"] = N_CompanyID;

                     if (!myFunctions.CheckActiveYearTransaction(N_CompanyID, N_FnYearID, DateTime.ParseExact(MasterTable.Rows[0]["D_AdjustmentDate"].ToString(), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture), dLayer, connection, transaction))
                    {

                        object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID="+N_CompanyID+" and convert(date ,'" + MasterTable.Rows[0]["D_AdjustmentDate"].ToString() + "') between D_Start and D_End", Params, connection, transaction);

                        if (DiffFnYearID != null)
                        {
                            MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
                            N_FnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());

                    QueryParams["@nFnYearID"] = N_FnYearID;
                    QueryParams["@N_PartyID"] = N_PartyID;
                   

                    if(N_PartyType==0){
                        
                    PostingParam.Add("N_PartyID", N_PartyID);
                    PostingParam.Add("N_FnyearID", N_FnYearID);
                    PostingParam.Add("N_CompanyID", N_CompanyID);
                    PostingParam.Add("X_Type", "vendor");


                    object vendorCount = dLayer.ExecuteScalar("Select count(*) From Inv_Vendor where N_FnYearID=@nFnYearID and N_CompanyID=@N_CompanyID and N_VendorID=@N_PartyID", QueryParams, connection, transaction);
                      
                               if(myFunctions.getIntVAL(vendorCount.ToString())==0){
                                try 
                                  {
                                     dLayer.ExecuteNonQueryPro("SP_CratePartyBackYear", PostingParam, connection, transaction);
                                  }
                                  catch (Exception ex)
                                  {
                                    transaction.Rollback();
                                     throw ex;
                                  }
                                  }
                      }

                      else{

                    PostingParam.Add("N_PartyID", N_PartyID);
                    PostingParam.Add("N_FnyearID", N_FnYearID);
                    PostingParam.Add("N_CompanyID", N_CompanyID);
                    PostingParam.Add("X_Type", "customer");

                 object custCount = dLayer.ExecuteScalar("Select count(*) From Inv_Customer where N_FnYearID=@nFnYearID and N_CompanyID=@N_CompanyID and N_CustomerID=@N_PartyID", QueryParams, connection, transaction);
                      
                      if(myFunctions.getIntVAL(custCount.ToString())==0){
                           try 
                    {
                        dLayer.ExecuteNonQueryPro("SP_CratePartyBackYear", PostingParam, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                      }

                      }

                           // QueryParams["@nFnYearID"] = N_FnYearID;
                        }
                        else
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
                        }
                    }

                    if (N_PartyType == 0)//Vendor
                    {
                        if (N_TransType == 1)
                            {X_Trasnaction = "VENDOR CREDIT NOTE";}
                        else if (N_TransType == 0)
                            {X_Trasnaction = "VENDOR DEBIT NOTE";}
                    }
                    if (N_PartyType == 1)//customer
                    {
                        if (N_TransType == 1)
                           { X_Trasnaction = "CUSTOMER CREDIT NOTE";}
                        else if (N_TransType == 0)
                            {X_Trasnaction = "CUSTOMER DEBIT NOTE";}
                    }

                    // Auto Gen
                    string AdjustmentNo = "";
                    var values = MasterTable.Rows[0]["X_VoucherNo"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", Master["n_CompanyId"].ToString());
                        Params.Add("N_YearID", Master["n_FnYearId"].ToString());
                        Params.Add("N_FormID", N_FormID);
                        Params.Add("N_BranchID", Master["n_BranchId"].ToString());

                        AdjustmentNo = dLayer.GetAutoNumber("Inv_BalanceAdjustmentMaster", "X_VoucherNo", Params, connection, transaction);
                        xButtonAction="Insert"; 
                        if (AdjustmentNo == "") { transaction.Rollback(); return Ok(_api.Error(User,"Unable to generate Adjustment Number")); }
                        MasterTable.Rows[0]["X_VoucherNo"] = AdjustmentNo;

                    }
                    else
                      AdjustmentNo = MasterTable.Rows[0]["X_VoucherNo"].ToString();
                    {
                        if (N_AdjustmentID > 0)
                        {
                            SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",N_CompanyID},
                                {"X_TransType",X_Trasnaction},
                                {"N_VoucherID",N_AdjustmentID}};
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                            int dltRes = dLayer.DeleteData("Inv_CostCentreTransactions", "N_InventoryID", N_AdjustmentID, " N_CompanyID = " + N_CompanyID + " and N_FnYearID=" + N_FnYearID, connection, transaction);
                            xButtonAction="Update"; 
                        }
                    }



                    N_AdjustmentID = dLayer.SaveData("Inv_BalanceAdjustmentMaster", "N_AdjustmentID", MasterTable, connection, transaction);
                    if (N_AdjustmentID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save Adjustment"));
                    }
                    int N_AdjustmentDetailsId = 0;
                    CostCenterTable = myFunctions.AddNewColumnToDataTable(CostCenterTable, "N_LedgerID", typeof(int), 0);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_AdjustmentID"] = N_AdjustmentID;

                        N_AdjustmentDetailsId = dLayer.SaveDataWithIndex("Inv_BalanceAdjustmentMasterDetails", "N_AdjustmentDetailsId", "", "", j, DetailTable, connection, transaction);
                        if (N_AdjustmentDetailsId > 0)
                        {
                            for (int k = 0; k < CostCenterTable.Rows.Count; k++)
                            {
                                if (myFunctions.getIntVAL(CostCenterTable.Rows[k]["rowID"].ToString()) == j)
                                {
                                    CostCenterTable.Rows[k]["N_VoucherID"] = N_AdjustmentID;
                                    CostCenterTable.Rows[k]["N_VoucherDetailsID"] = N_AdjustmentDetailsId;
                                    CostCenterTable.Rows[k]["N_LedgerID"] = myFunctions.getIntVAL(DetailTable.Rows[j]["n_LedgerID"].ToString());

                                }
                            }
                        }
                    }

                    CostCenterTable.AcceptChanges();

                    DataTable costcenter = new DataTable();
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_CostCenterTransID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_CompanyID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_FnYearID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_InventoryType", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_InventoryID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_InventoryDetailsID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_CostCentreID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_Amount", typeof(double), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_LedgerID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_BranchID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "X_Narration", typeof(string), "");
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "X_Naration", typeof(string), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "D_Entrydate", typeof(DateTime), null);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_GridLineNo", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_EmpID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_ProjectID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_FormID", typeof(int), 0);

                    foreach (DataRow dRow in CostCenterTable.Rows)
                    {
                        DataRow row = costcenter.NewRow();
                        row["N_CostCenterTransID"] = dRow["N_VoucherSegmentID"];
                        row["N_CompanyID"] = dRow["N_CompanyID"];
                        row["N_FnYearID"] = dRow["N_FnYearID"];
                        row["N_InventoryType"] = 0;
                        row["N_InventoryID"] = dRow["N_VoucherID"];
                        row["N_InventoryDetailsID"] = dRow["N_VoucherDetailsID"];
                        row["N_CostCentreID"] = dRow["n_Segment_2"];
                        row["N_Amount"] = dRow["N_Amount"];
                        row["N_LedgerID"] = dRow["N_LedgerID"];
                        row["N_BranchID"] = dRow["N_BranchID"];
                        row["X_Narration"] = "";
                        row["X_Naration"] = dRow["X_Naration"];
                        row["D_Entrydate"] = dRow["D_Entrydate"];
                        row["N_GridLineNo"] = dRow["rowID"];
                        row["N_EmpID"] = myFunctions.getIntVAL(dRow["N_Segment_4"].ToString());
                        row["N_ProjectID"] = myFunctions.getIntVAL(dRow["N_Segment_3"].ToString());
                        row["N_FormID"] = dRow["N_FormID"];
                        costcenter.Rows.Add(row);
                    }
                                             //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(N_FnYearID,N_AdjustmentID,AdjustmentNo,N_FormID,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                          
                          
                    // int N_AdjustmentDetailsId = dLayer.SaveData("Inv_BalanceAdjustmentMasterDetails", "N_AdjustmentDetailsId", DetailTable, connection, transaction);
                    int N_SegmentId = dLayer.SaveData("Inv_CostCentreTransactions", "N_CostCenterTransID", "", "", costcenter, connection, transaction);
                    SortedList BalanceAdjParams = new SortedList();
                           BalanceAdjParams.Add("@N_AdjustmentId", N_AdjustmentID);

                     DataTable InvBalanceAdjInfo = dLayer.ExecuteDataTable("Select X_VoucherNo,N_TransType from Inv_BalanceAdjustmentMaster where N_AdjustmentID=@N_AdjustmentID", BalanceAdjParams, connection, transaction);
                        if (InvBalanceAdjInfo.Rows.Count > 0)
                        {
                            // try
                            // {
                            //     myAttachments.SaveAttachment(dLayer, Attachment, AdjustmentNo, N_AdjustmentID, InvBalanceAdjInfo.Rows[0]["N_TransType"].ToString().Trim(), InvBalanceAdjInfo.Rows[0]["X_VoucherNo"].ToString(), N_AdjustmentID, "Balance Adjustment Document", User, connection, transaction);
                            // }
                            //  catch (Exception ex)
                            // {
                            //     transaction.Rollback();
                            //     return Ok(_api.Error(User, ex));
                            // }
                        }
                    if (N_AdjustmentDetailsId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save Adjustment"));
                    }
                    else
                    {
                        try
                        {
                            SortedList PostingParams = new SortedList(){
                                {"N_CompanyID",N_CompanyID},
                                {"X_InventoryMode",X_Trasnaction},
                                {"N_InternalID",N_AdjustmentID},
                                {"N_UserID",myFunctions.GetUserID(User)},
                                {"N_IsImport",N_IsImport}
                                };
                            dLayer.ExecuteNonQueryPro("SP_Acc_InventoryPosting", PostingParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            if (ex.Message == "50")
                                return Ok(_api.Error(User, "Day Closed"));
                            else if (ex.Message == "51")
                                return Ok(_api.Error(User, "Year Closed"));
                            else if (ex.Message == "52")
                                return Ok(_api.Error(User, "Year Exists"));
                            else if (ex.Message == "53")
                                return Ok(_api.Error(User, "Period Closed"));
                            else if (ex.Message == "54")
                                return Ok(_api.Error(User, "Wrong Txn Date"));
                            else if (ex.Message == "55")
                                return Ok(_api.Error(User, "Quantity exceeds!"));
                            else
                                return Ok(_api.Error(User, ex));
                        }
                        transaction.Commit();
                    }
                    
                    
                       SortedList Result = new SortedList();
                       Result.Add("AdjustmentNo", MasterTable.Rows[0]["X_VoucherNo"] );
                       Result.Add("N_AdjustmentID", N_AdjustmentID);
               
                    return Ok(_api.Success(Result,"Adjustment saved" + ":" + N_AdjustmentID));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID, int nAdjustmentId, string xTransType,int nFnYearID,int nFormID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                     SortedList Params = new SortedList();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    
                    ParamList.Add("@nTransID", nAdjustmentId);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                     ParamList.Add("@nFormID", nFormID);
                    

                    string Sql = "select N_AdjustmentID,X_VoucherNo from Inv_BalanceAdjustmentMaster where N_AdjustmentID=@nTransID and N_CompanyID=@nCompanyID ";
                    string xButtonAction="Delete";
                    string X_VoucherNo="";
                    
                     TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection,transaction);
                   object n_FnYearID = dLayer.ExecuteScalar("select N_FnYearID from Inv_BalanceAdjustmentMaster where N_AdjustmentID =" + nAdjustmentId + " and N_CompanyID=" + nCompanyID, Params, connection,transaction);

                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];
                    //Activity Log
                        string ipAddress = "";
                   if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                   else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(nFnYearID,nAdjustmentId,TransRow["X_VoucherNo"].ToString(),nFormID,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                  
                    
                    SortedList deleteParams = new SortedList()
                            {
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType",xTransType},
                                {"N_VoucherID",nAdjustmentId}

                            };
                    dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", deleteParams, connection, transaction);
                    transaction.Commit();
                }
                return Ok(_api.Success("Deleted"));
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }






    }
}