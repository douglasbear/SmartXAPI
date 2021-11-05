// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using System;
// using SmartxAPI.GeneralFunctions;
// using System.Data;
// using System.Collections;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Data.SqlClient;
// using System.Collections.Generic;

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
    [Route("goodsreceivenote")]
    [ApiController]
    public class Inv_GoodsReceiveNote : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int N_FormID;
        public Inv_GoodsReceiveNote(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 555;
        }
        [HttpGet("list")]
        public ActionResult GetGoodsReceiveList(int? nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            DataTable CountTable = new DataTable();
            SortedList Params = new SortedList();
            DataSet dataSet = new DataSet();
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([MRN No] like '%" + xSearchkey + "%' or X_VendorName like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_MRNID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "[MRN No]":
                        xSortBy = "N_MRNID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }
            int Count = (nPage - 1) * nSizeperpage;
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_VendorID,N_MRNID,N_FnYearID,D_MRNDate,N_BranchID,B_YearEndProcess,B_IsDirectMRN,[MRN No] AS MRNNo,X_VendorName,MRNDate,OrderNo,X_VendorInvoice,x_Description from vw_InvMRNNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_VendorID,N_MRNID,N_FnYearID,D_MRNDate,N_BranchID,B_YearEndProcess,B_IsDirectMRN,[MRN No] AS MRNNo,X_VendorName,MRNDate,OrderNo,X_VendorInvoice,x_Description from vw_InvMRNNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and N_MRNID not in (select top(" + Count + ") N_MRNID from vw_InvMRNNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + xSortBy + " ) " + xSortBy;

            // sqlCommandText = "select * from Inv_MRNDetails where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count from vw_InvMRNNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                    }
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
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
        [HttpGet("listdetails")]
        public ActionResult GetGoodsReceiveDetails(int nCompanyId, int nFnYearId, string nMRNNo, bool showAllBranch, int nBranchId, string poNo)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable dtGoodReceive = new DataTable();
            DataTable dtGoodReceiveDetails = new DataTable();
            int N_GRNID = 0;
            int N_POrderID = 0;

            Params.Add("@CompanyID", nCompanyId);
            Params.Add("@YearID", nFnYearId);
            Params.Add("@TransType", "GRN");
            Params.Add("@BranchID", nBranchId);
            string X_MasterSql = "";
            string X_DetailsSql = "";

            if (nMRNNo != null)
            {
                Params.Add("@GRNNo", nMRNNo);
                X_MasterSql = "select N_CompanyID,N_VendorID,N_MRNID,N_FnYearID,D_MRNDate,N_BranchID,B_YearEndProcess,B_IsDirectMRN,[MRN No] AS x_MRNNo,X_VendorName,MRNDate,OrderNo,X_VendorInvoice,x_Description from vw_InvMRNNo_Search where N_CompanyID=@CompanyID and [MRN No]=@GRNNo and N_FnYearID=@YearID " + (showAllBranch ? "" : " and  N_BranchId=@BranchID");
            }
            if (poNo != null)
            {
                X_MasterSql = "Select Inv_PurchaseOrder.*,Inv_Location.X_LocationName,Inv_Vendor.* from Inv_PurchaseOrder Inner Join Inv_Vendor On Inv_PurchaseOrder.N_VendorID=Inv_Vendor.N_VendorID and Inv_PurchaseOrder.N_CompanyID=Inv_Vendor.N_CompanyID and Inv_PurchaseOrder.N_FnYearID=Inv_Vendor.N_FnYearID LEFT OUTER JOIN Inv_Location ON Inv_Location.N_LocationID=Inv_PurchaseOrder.N_LocationID Where Inv_PurchaseOrder.N_CompanyID=" + nCompanyId + " and X_POrderNo='" + poNo + "' and Inv_PurchaseOrder.B_IsSaveDraft<>1";
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dtGoodReceive = dLayer.ExecuteDataTable(X_MasterSql, Params, connection);
                    if (dtGoodReceive.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                    dtGoodReceive = _api.Format(dtGoodReceive, "Master");

                    if (poNo != null)
                    {
                        N_POrderID = myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_POrderid"].ToString());
                    }
                    else
                    {
                        N_GRNID = myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_MRNID"].ToString());

                    }
                    if (N_GRNID != 0)
                    {
                        X_DetailsSql = "Select * from vw_InvMRNDetails  Where vw_InvMRNDetails.N_CompanyID=@CompanyID and vw_InvMRNDetails.N_MRNID=" + N_GRNID + (showAllBranch ? "" : " and vw_InvMRNDetails.N_BranchId=@BranchID");
                    }
                    if (N_POrderID != 0)
                    {
                        X_DetailsSql = "Select *,dbo.SP_Cost(vw_POMrn_PendingDetail.N_ItemID,vw_POMrn_PendingDetail.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_POMrn_PendingDetail.N_ItemID,vw_POMrn_PendingDetail.N_CompanyID) As N_UnitSPrice  from vw_POMrn_PendingDetail Where N_CompanyID=" + nCompanyId + " and N_POrderID=" + N_POrderID + "";

                    }


                    dtGoodReceiveDetails = dLayer.ExecuteDataTable(X_DetailsSql, Params, connection);
                    dtGoodReceiveDetails = _api.Format(dtGoodReceiveDetails, "Details");
                    if (N_POrderID != 0)
                    {
                    }
                    else
                    {
                        DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_VendorID"].ToString()), myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_MRNID"].ToString()), this.N_FormID, myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_FnYearID"].ToString()), User, connection);
                        Attachments = _api.Format(Attachments, "attachments");
                          dt.Tables.Add(Attachments);
                       
                    }
                    dt.Tables.Add(dtGoodReceive);
                     dt.Tables.Add(dtGoodReceiveDetails);
                  

                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpGet("fillfreight")]
        public ActionResult fillMRNFreight(int nCompanyId, int nFnYearId, int nMrnId)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable dtFreightList = new DataTable();

            Params.Add("@CompanyID", nCompanyId);
            Params.Add("@YearID", nFnYearId);
            Params.Add("@MRNID", nMrnId);
            string x_SqlQurery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    x_SqlQurery = "select * from vw_InvMRNFreights where N_CompanyID = @CompanyID and N_FnYearID = @YearID and N_MRNID = @MRNID";
                    dtFreightList = dLayer.ExecuteDataTable(x_SqlQurery, Params, connection);
                    if (dtFreightList.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                    dtFreightList = _api.Format(dtFreightList, "Master");
                    dt.Tables.Add(dtFreightList);
                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        // private SortedList StatusSetup(int nSalesID, int nFnYearID, SqlConnection connection)
        // {

        //     object objInvoiceRecievable = null, objBal = null;
        //     double InvoiceRecievable = 0, BalanceAmt = 0;
        //     SortedList TxnStatus = new SortedList();
        //     TxnStatus.Add("Label", "");
        //     TxnStatus.Add("LabelColor", "");
        //     TxnStatus.Add("Alert", "");
        //     TxnStatus.Add("DeleteEnabled", true);
        //     TxnStatus.Add("SaveEnabled", true);
        //     TxnStatus.Add("ReceiptNumbers", "");
        //     int nCompanyID = myFunctions.GetCompanyID(User);

        //     objInvoiceRecievable = dLayer.ExecuteScalar("SELECT isnull((Inv_Sales.N_BillAmt-Inv_Sales.N_DiscountAmt + Inv_Sales.N_FreightAmt +isnull(Inv_Sales.N_OthTaxAmt,0)+ Inv_Sales.N_TaxAmt),0) as N_InvoiceAmount FROM Inv_Sales where Inv_Sales.N_SalesId=" + nSalesID + " and Inv_Sales.N_CompanyID=" + nCompanyID, connection);
        //     objBal = dLayer.ExecuteScalar("SELECT SUM(N_BalanceAmount) from  vw_InvReceivables where N_SalesId=" + nSalesID + " and X_Type='SALES' and N_CompanyID=" + nCompanyID, connection);


        //     object RetQty = dLayer.ExecuteScalar("select Isnull(Count(N_DebitNoteId),0) from Inv_SalesReturnMaster where N_SalesId =" + nSalesID + " and Isnull(B_IsSaveDraft,0) =0 and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, connection);
        //     object RetQtyDrft = dLayer.ExecuteScalar("select Isnull(Count(N_DebitNoteId),0) from Inv_SalesReturnMaster where N_SalesId =" + nSalesID + " and Isnull(B_IsSaveDraft,0)=1 and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, connection);


        //     if (objInvoiceRecievable != null)
        //         InvoiceRecievable = myFunctions.getVAL(objInvoiceRecievable.ToString());
        //     if (objBal != null)
        //         BalanceAmt = myFunctions.getVAL(objBal.ToString());

        //     if ((InvoiceRecievable == BalanceAmt) && (InvoiceRecievable > 0 && BalanceAmt > 0))
        //     {
        //         TxnStatus["Label"] = "NotPaid";
        //         TxnStatus["LabelColor"] = "Red";
        //         TxnStatus["Alert"] = "";
        //     }
        //     else
        //     {
        //         if (BalanceAmt == 0)
        //         {
        //             //IF PAYMENT DONE
        //             TxnStatus["Label"] = "Paid";
        //             TxnStatus["LabelColor"] = "Green";
        //             TxnStatus["Alert"] = "Customer Receipt is Processed for this Invoice.";


        //             //IF PAYMENT DONE AND HAVING RETURN
        //             if (RetQty != null && myFunctions.getIntVAL(RetQty.ToString()) > 0)
        //             {
        //                 TxnStatus["SaveEnabled"] = false;
        //                 TxnStatus["DeleteEnabled"] = false;
        //                 TxnStatus["Alert"] = "Sales Return Processed for this invoice.";
        //                 TxnStatus["Label"] = "Paid(Return)";
        //                 TxnStatus["LabelColor"] = "Green";
        //             }
        //             else if (RetQtyDrft != null && myFunctions.getIntVAL(RetQtyDrft.ToString()) > 0)
        //             {
        //                 TxnStatus["SaveEnabled"] = true;
        //                 TxnStatus["DeleteEnabled"] = false;
        //                 TxnStatus["Alert"] = "Sales Return Processed for this invoice.";
        //                 TxnStatus["Label"] = "Paid(Return)";
        //                 TxnStatus["LabelColor"] = "Green";
        //             }
        //         }
        //         else
        //         {
        //             //IF HAVING BALANCE AMOUNT
        //             TxnStatus["Alert"] = "Customer Receipt is Processed for this Invoice.";
        //             TxnStatus["Label"] = "ParPaid";
        //             TxnStatus["LabelColor"] = "Green";

        //             //IF HAVING BALANCE AMOUNT AND HAVING RETURN
        //             if (RetQty != null && myFunctions.getIntVAL(RetQty.ToString()) > 0)
        //             {
        //                 TxnStatus["SaveEnabled"] = false;
        //                 TxnStatus["DeleteEnabled"] = false;
        //                 TxnStatus["Alert"] = "Sales Return Processed for this invoice.";
        //                 TxnStatus["Label"] = "Partially Paid(Return)";
        //                 TxnStatus["LabelColor"] = "Green";
        //             }
        //             else if (RetQtyDrft != null && myFunctions.getIntVAL(RetQtyDrft.ToString()) > 0)
        //             {
        //                 TxnStatus["SaveEnabled"] = true;
        //                 TxnStatus["DeleteEnabled"] = false;
        //                 TxnStatus["Alert"] = "Sales Return Processed for this invoice.";
        //                 TxnStatus["Label"] = "Partially Paid(Return)";
        //                 TxnStatus["LabelColor"] = "Green";
        //             }
        //         }


        //         //PAYMENT NO DISPLAY IN TOP LABEL ON MOUSE HOVER
        //         DataTable Receipts = dLayer.ExecuteDataTable("SELECT  dbo.Inv_PayReceipt.X_VoucherNo FROM  dbo.Inv_PayReceipt INNER JOIN dbo.Inv_PayReceiptDetails ON dbo.Inv_PayReceipt.N_PayReceiptId = dbo.Inv_PayReceiptDetails.N_PayReceiptId Where dbo.Inv_PayReceipt.X_Type='SR' and dbo.Inv_PayReceiptDetails.N_InventoryId =" + nSalesID, connection);
        //         string InvoiceNos = "";
        //         foreach (DataRow var in Receipts.Rows)
        //         {
        //             InvoiceNos += var["X_VoucherNo"].ToString() + " , ";
        //         }
        //         char[] trim = { ',', ' ' };
        //         if (InvoiceNos != "")
        //             TxnStatus["ReceiptNumbers"] = InvoiceNos.ToString().TrimEnd(trim);

        //     }

        //     return TxnStatus;
        // }

        // //Unprocessed Purchase Order Listing 
        // [HttpGet("listPO")]
        // public ActionResult GetPurchaseOrder(int? nCompanyId,int? nVendorID)
        // {
        //     DataTable dt = new DataTable();
        //     SortedList Params = new SortedList();
        //     string sqlCommandText ="";
        //     if(nVendorID > 0){
        //         sqlCommandText = "select * from vw_purchaseOrder_Disp where N_CompanyID = @p1 and N_VendorID = @p2 and ISNULL(B_IsSaveDraft,0) = 0 order by OrderDate desc";
        //         Params.Add("@p1", nCompanyId);
        //         Params.Add("@p2",nVendorID);
        //     }
        //     else{
        //         sqlCommandText = "select * from vw_purchaseOrder_Disp where N_CompanyID = @p1 and ISNULL(B_IsSaveDraft,0) = 0 order by OrderDate desc";
        //         Params.Add("@p1", nCompanyId);
        //     }
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
        //         }
        //         if (dt.Rows.Count == 0)
        //         {
        //             return Ok(_api.Warning("No Results Found" ));
        //         }
        //         else
        //         {
        //             return Ok(_api.Success(_api.Format(dt)));
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok( _api.Error(User,e));
        //     }
        // }

        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            DataTable MasterTable;
            DataTable DetailTable;
            DataTable dtFreightChargeDist;
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            dtFreightChargeDist = ds.Tables["freightchargedist"];
            DataTable Attachment = ds.Tables["attachments"];
            SortedList Params = new SortedList();
            // Auto Gen
            string GRNNo = "";
            DataRow masterRow = MasterTable.Rows[0];
            var values = masterRow["X_MRNNo"].ToString();
            int N_GRNID = 0;
            int N_GRNFreightID = 0;
            int N_SaveDraft = 0;
            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nFnYearID = myFunctions.getIntVAL(masterRow["n_FnYearId"].ToString());
            int n_POrderID = myFunctions.getIntVAL(masterRow["n_POrderID"].ToString());
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();
                    N_GRNID = myFunctions.getIntVAL(masterRow["N_MRNID"].ToString());
                    int N_VendorID = myFunctions.getIntVAL(masterRow["n_VendorID"].ToString());

                    if (N_GRNID > 0)
                    {
                        if (CheckProcessed(N_GRNID))
                            return Ok(_api.Error(User,"Transaction Started!"));
                    }
                    if (values == "@Auto")
                    {
                        N_SaveDraft = myFunctions.getIntVAL(masterRow["b_IsSaveDraft"].ToString());
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("N_BranchID", masterRow["n_BranchId"].ToString());

                        GRNNo = dLayer.GetAutoNumber("Inv_MRN", "X_MRNNo", Params, connection, transaction);
                        if (GRNNo == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,"Unable to generate Document Number"));
                        }
                        MasterTable.Rows[0]["X_MRNNo"] = GRNNo;
                    }

                    if (N_GRNID > 0)
                    {
                        if (n_POrderID > 0)
                        {
                            dLayer.ExecuteScalar("Update Inv_PurchaseOrder Set N_Processed=1  Where N_POrderID=" + n_POrderID + " and N_CompanyID=" + nCompanyID, connection, transaction);
                        }

                        SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",masterRow["n_CompanyId"].ToString()},
                                {"X_TransType","GRN"},
                                {"N_VoucherID",N_GRNID},
                                {"N_UserID",nUserID},
                                {"X_SystemName","WebRequest"},
                                {"B_MRNVisible","0"}};

                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", DeleteParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,ex));
                        }
                    }

                    N_GRNID = dLayer.SaveData("Inv_MRN", "N_MRNID", MasterTable, connection, transaction);

                    if (N_GRNID <= 0)
                    {
                        transaction.Rollback();
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_MRNID"] = N_GRNID;
                    }
                    int N_MRNDetailsID = dLayer.SaveData("Inv_MRNDetails", "N_MRNDetailsID", DetailTable, connection, transaction);

                    if (N_MRNDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save Goods Receive Note!"));
                    }
                    // if (dtFreightChargeDist.Rows.Count > 0)
                    // {
                    //     N_GRNFreightID = dLayer.SaveData("Inv_MRNFreights", "N_MRNFreightID", dtFreightChargeDist, connection, transaction); 
                    // }
                    // if(N_GRNFreightID > 0) {
                    //     SortedList FreightToPurchaseParam = new SortedList();
                    //     FreightToPurchaseParam.Add("N_FPurchaseID",);
                    //}
                    if (N_SaveDraft == 0)
                    {
                        try
                        {
                            if (n_POrderID > 0)
                            {
                                dLayer.ExecuteScalar("Update Inv_PurchaseOrder Set N_Processed=1  Where N_POrderID=" + n_POrderID + " and N_CompanyID=" + nCompanyID, connection, transaction);

                            }
                            // SortedList StockPosting = new SortedList();
                            // StockPosting.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
                            // StockPosting.Add("N_MRNID", N_GRNID);
                            // StockPosting.Add("N_UserID", nUserID);
                            // StockPosting.Add("X_SystemName", "ERP Cloud");
                            // dLayer.ExecuteNonQueryPro("[SP_Inv_AllocateNegStock_MRN]", StockPosting, connection, transaction);

                            SortedList PostingParam = new SortedList();
                            PostingParam.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
                            PostingParam.Add("X_InventoryMode", "GRN");
                            PostingParam.Add("N_InternalID", N_GRNID);
                            PostingParam.Add("N_UserID", nUserID);
                            PostingParam.Add("X_SystemName", "ERP Cloud");
                            dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Purchase_Posting", PostingParam, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,ex));
                        }
                    }
                    SortedList VendorParams = new SortedList();
                    VendorParams.Add("@nVendorID", N_VendorID);
                    DataTable VendorInfo = dLayer.ExecuteDataTable("Select X_VendorCode,X_VendorName from Inv_Vendor where N_VendorID=@nVendorID", VendorParams, connection, transaction);
                    if (VendorInfo.Rows.Count > 0)
                    {
                        try
                        {
                            myAttachments.SaveAttachment(dLayer, Attachment, GRNNo, N_GRNID, VendorInfo.Rows[0]["X_VendorName"].ToString().Trim(), VendorInfo.Rows[0]["X_VendorCode"].ToString(), N_VendorID, "Vendor Document", User, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,ex));
                        }
                    }
                    transaction.Commit();
                }
                SortedList Result = new SortedList();
                Result.Add("N_MRNID", N_GRNID);
                Result.Add("X_MRNNo", GRNNo);
                return Ok(_api.Success(Result, "Goods Receive Note Saved"));
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }
        private bool CheckProcessed(int nGRNID)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            object Processed = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlCommand = "Select Isnull(Count(Inv_PurchaseDetails.N_RsID),0) FROM Inv_Purchase INNER JOIN Inv_PurchaseDetails ON Inv_Purchase.N_PurchaseID = Inv_PurchaseDetails.N_PurchaseID Where  Inv_Purchase.N_CompanyID=@p1 and Inv_PurchaseDetails.N_RsID=@p2 and B_IsSaveDraft = 0";
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", nGRNID);
                Processed = dLayer.ExecuteScalar(sqlCommand, Params, connection);

            }
            if (Processed != null)
            {
                if (myFunctions.getIntVAL(Processed.ToString()) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        //Delete....
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nGRNID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            int Results = 0;
            if (CheckProcessed(nGRNID))
                return Ok(_api.Error(User,"Transaction Started"));


            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nTransID", nGRNID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    string Sql = "select N_VendorID,N_FnYearID from Inv_MRN where N_MRNID=@nTransID and N_CompanyID=@nCompanyID";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User,"Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    int VendorID = myFunctions.getIntVAL(TransRow["N_VendorID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(TransRow["N_FnYearID"].ToString());


                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType","GRN"},
                                {"N_VoucherID",nGRNID},
                                {"N_UserID",nUserID},
                                {"X_SystemName","WebRequest"},
                                {"@B_MRNVisible","0"}};

                    Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", DeleteParams, connection, transaction);
                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to Delete Goods Receive Note"));
                    }

                    myAttachments.DeleteAttachment(dLayer, 1, nGRNID, VendorID, nFnYearID, this.N_FormID, User, transaction, connection);

                    transaction.Commit();
                    return Ok(_api.Success("Goods Receive Note deleted"));

                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }


        }

        [HttpGet("freighttype")]
        public ActionResult GetFreightType(int nCompanyID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("N_CompanyID", nCompanyID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTablePro("SP_FillFreightReasons", Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(_api.Format(dt)));
                }
            }
            catch (Exception e)
            {
                return Ok( _api.Error(User,e));
            }
        }

    }
}