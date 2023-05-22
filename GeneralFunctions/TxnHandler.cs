using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using SmartxAPI.GeneralFunctions;
using Microsoft.AspNetCore.Mvc;

namespace SmartxAPI.GeneralFunctions
{
    public class TxnHandler : ITxnHandler
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
       // private readonly int N_FormID;

        public TxnHandler(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            //N_FormID = 65;
        }
    

        public SortedList PurchaseSaveData(DataSet ds,string ipAddress,ClaimsPrincipal User,IDataAccessLayer dLayer,SqlConnection connection, SqlTransaction transaction)
        {

            DataTable MasterTable;
            DataTable DetailTable;
            DataTable DetailsToImport;
            DataTable WarrantyInfo;
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            WarrantyInfo = ds.Tables["warrantyInformation"];
            DataTable Approvals;
            Approvals = ds.Tables["approval"];
            DataRow ApprovalRow = Approvals.Rows[0];
            DataTable Attachment = ds.Tables["attachments"];
            DataTable PurchaseFreight = ds.Tables["freightCharges"];
            SortedList Params = new SortedList();
            SortedList Result = new SortedList();
            // Auto Gen
            string InvoiceNo = "";
            DataRow masterRow = MasterTable.Rows[0];
            var values = masterRow["x_InvoiceNo"].ToString();
            int N_PurchaseID = 0;
            int N_SaveDraft = 0;
            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nFnYearID = myFunctions.getIntVAL(masterRow["n_FnYearId"].ToString());
            int n_POrderID = myFunctions.getIntVAL(masterRow["N_POrderID"].ToString());
            var vendorInvoice="";
            if(MasterTable.Columns.Contains("X_VendorInvoice"))
                    vendorInvoice = masterRow["X_VendorInvoice"].ToString();
            int n_MRNID = 0;
            if (MasterTable.Columns.Contains("N_RsID"))
                n_MRNID = myFunctions.getIntVAL(masterRow["N_RsID"].ToString());
            int Dir_Purchase = 1;
            int b_FreightAmountDirect = myFunctions.getIntVAL(masterRow["b_FreightAmountDirect"].ToString());
            DetailsToImport = ds.Tables["detailsImport"];
            bool B_isImport = false;
            bool showSellingPrice =false;
            string xButtonAction="";

            if(MasterTable.Columns.Contains("showSellingPrice")) 
               showSellingPrice=myFunctions.getBoolVAL(masterRow["showSellingPrice"].ToString());
            if(MasterTable.Columns.Contains("showSellingPrice")){MasterTable.Columns.Remove("showSellingPrice");}

            if (ds.Tables.Contains("detailsImport"))
                B_isImport = true;


            // try
            // {
            //     using (SqlConnection connection = new SqlConnection(connectionString))
            //     {
            //         connection.Open();
            //         SqlTransaction transaction;
            //         transaction = connection.BeginTransaction();
                    N_PurchaseID = myFunctions.getIntVAL(masterRow["n_PurchaseID"].ToString());
                    int N_VendorID = myFunctions.getIntVAL(masterRow["n_VendorID"].ToString());
                    int N_NextApproverID = 0;

                    if (!myFunctions.CheckActiveYearTransaction(nCompanyID, nFnYearID, Convert.ToDateTime(MasterTable.Rows[0]["D_InvoiceDate"].ToString()), dLayer, connection, transaction))
                    {
                        object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID=" + nCompanyID + " and convert(date ,'" + MasterTable.Rows[0]["D_InvoiceDate"].ToString() + "') between D_Start and D_End", Params, connection, transaction);
                        if (DiffFnYearID != null)
                        {
                            MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
                            nFnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());
                             SortedList QueryParams = new SortedList();
                            QueryParams["@nFnYearID"] = nFnYearID;
                            QueryParams["@nCompanyID"] = nCompanyID;
                            QueryParams["@N_VendorID"] = N_VendorID;
                            
                              SortedList PostingParam = new SortedList();
                              PostingParam.Add("N_PartyID", N_VendorID);
                              PostingParam.Add("N_FnyearID", nFnYearID);
                              PostingParam.Add("N_CompanyID", nCompanyID);
                              PostingParam.Add("X_Type", "vendor");


                             object vendorCount = dLayer.ExecuteScalar("Select count(*) From Inv_Vendor where N_FnYearID=@nFnYearID and N_CompanyID=@nCompanyID and N_VendorID=@N_VendorID", QueryParams, connection, transaction);
                      
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
                        else
                            {
                        //     transaction.Rollback();
                        //     return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
                            Result.Add("b_IsCompleted", 0);
                            Result.Add("x_Msg", "Transaction date must be in the active Financial Year.");
                            return Result;
                        }
                    }
                    MasterTable.AcceptChanges();

                    object mrnCount = dLayer.ExecuteScalar("SELECT count(Sec_UserPrevileges.N_MenuID) as Count FROM Sec_UserPrevileges INNER JOIN Sec_UserCategory ON Sec_UserPrevileges.N_UserCategoryID = Sec_UserCategory.N_UserCategoryID and Sec_UserPrevileges.N_MenuID=555 and Sec_UserCategory.N_CompanyID=" + nCompanyID + " and Sec_UserPrevileges.B_Visible=1", connection, transaction);
                    bool B_MRNVisible = myFunctions.getIntVAL(mrnCount.ToString()) > 0 ? true : false;

                    if (B_MRNVisible && n_MRNID != 0) Dir_Purchase = 0;

                    if (N_PurchaseID > 0)
                    {
                        if (myFunctions.CheckPRProcessed(N_PurchaseID,User,dLayer,connection,transaction))
                        {
                            //  transaction.Rollback();
                            //  return Ok(_api.Error(User, "Transaction Started!"));
                            Result.Add("b_IsCompleted", 0);
                            Result.Add("x_Msg", "Transaction Started!");
                            return Result;
                        }
                            

                        object Dir_PurchaseCount = dLayer.ExecuteScalar("SELECT COUNT(Inv_Purchase.N_PurchaseID) FROM Inv_Purchase INNER JOIN Inv_MRN ON Inv_Purchase.N_CompanyID = Inv_MRN.N_CompanyID AND Inv_Purchase.N_RsID = Inv_MRN.N_MRNID AND Inv_Purchase.N_FnYearID = Inv_MRN.N_FnYearID " +
                                                                        " WHERE Inv_MRN.B_IsDirectMRN=0 and Inv_Purchase.N_CompanyID=" + nCompanyID + " and Inv_Purchase.N_PurchaseID=" + N_PurchaseID, connection, transaction);

                        if (myFunctions.getIntVAL(Dir_PurchaseCount.ToString()) > 0)
                            Dir_Purchase = 1;
                    }

                    SortedList VendParams = new SortedList();
                    VendParams.Add("@nCompanyID", nCompanyID);
                    VendParams.Add("@N_VendorID", N_VendorID);
                    VendParams.Add("@nFnYearID", nFnYearID);
                    object objVendorName = dLayer.ExecuteScalar("Select X_VendorName From Inv_Vendor where N_VendorID=@N_VendorID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", VendParams, connection, transaction);
                    object objVendorCode = dLayer.ExecuteScalar("Select X_VendorCode From Inv_Vendor where N_VendorID=@N_VendorID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", VendParams, connection, transaction);
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
                    if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()) && N_PurchaseID > 0)
                    {
                        int N_PkeyID = N_PurchaseID;
                        string X_Criteria = "N_PurchaseID=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                        myFunctions.UpdateApproverEntry(Approvals, "Inv_Purchase", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "PURCHASE", N_PkeyID, values, 1, objVendorName.ToString(), 0, "", 0, User, dLayer, connection, transaction);
                        myAttachments.SaveAttachment(dLayer, Attachment, values, N_PurchaseID, objVendorName.ToString().Trim(), objVendorCode.ToString(), N_VendorID, "Vendor Document", User, connection, transaction);

                        N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_Purchase where N_PurchaseID=" + N_PurchaseID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, connection, transaction).ToString());
                        if (N_SaveDraft == 0)
                        {
                            try
                            {
                                // SortedList PostingMRNParam = new SortedList();
                                // PostingMRNParam.Add("N_CompanyID", nCompanyID);
                                // PostingMRNParam.Add("N_PurchaseID", N_PurchaseID);
                                // PostingMRNParam.Add("N_UserID", nUserID);
                                // PostingMRNParam.Add("X_SystemName", "ERP Cloud");
                                // PostingMRNParam.Add("X_UseMRN", "");
                                // PostingMRNParam.Add("N_SaveDraft", N_SaveDraft);
                                // PostingMRNParam.Add("N_MRNID", 0);

                                // dLayer.ExecuteNonQueryPro("[SP_Inv_MRNposting]", PostingMRNParam, connection, transaction);

                                SortedList PostingMRNParam = new SortedList();
                                PostingMRNParam.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
                                PostingMRNParam.Add("N_PurchaseID", N_PurchaseID);
                                PostingMRNParam.Add("N_UserID", nUserID);
                                PostingMRNParam.Add("X_SystemName", "ERP Cloud");
                                PostingMRNParam.Add("X_UseMRN", "");
                                PostingMRNParam.Add("N_SaveDraft", N_SaveDraft);
                                PostingMRNParam.Add("B_DirectPurchase", Dir_Purchase);
                                PostingMRNParam.Add("N_MRNID", n_MRNID);

                                dLayer.ExecuteNonQueryPro("[SP_Inv_MRNprocessing]", PostingMRNParam, connection, transaction);

                                SortedList PostingParam = new SortedList();
                                PostingParam.Add("N_CompanyID", nCompanyID);
                                PostingParam.Add("X_InventoryMode", "PURCHASE");
                                PostingParam.Add("N_InternalID", N_PurchaseID);
                                PostingParam.Add("N_UserID", nUserID);
                                PostingParam.Add("X_SystemName", "ERP Cloud");
                                PostingParam.Add("MRN_Flag", Dir_Purchase==0 ? "1" : "0");

                                dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Purchase_Posting", PostingParam, connection, transaction);

                                SortedList StockOutParam = new SortedList();
                                StockOutParam.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());

                                dLayer.ExecuteNonQueryPro("SP_StockOutUpdate", StockOutParam, connection, transaction);

                                for (int j = 0; j < DetailTable.Rows.Count; j++)
                                {
                                    dLayer.ExecuteScalar("UPDATE Inv_ItemMaster SET Inv_ItemMaster.N_PurchaseCost=LastCost.N_LPrice from Inv_ItemMaster INNER JOIN "+
                                                    " (select TOP 1 N_CompanyID,N_ItemID,N_LPrice from Inv_StockMaster where X_Type='Purchase' and N_ItemID="+myFunctions.getVAL(DetailTable.Rows[j]["N_ItemID"].ToString())+" "+
                                                    " AND N_CompanyID= "+ myFunctions.getVAL(DetailTable.Rows[j]["N_CompanyID"].ToString()) +" order by D_DateIn desc ,N_StockID desc) AS LastCost ON Inv_ItemMaster.N_CompanyID=LastCost.N_CompanyID AND "+
                                                    " Inv_ItemMaster.N_ItemID=LastCost.N_ItemID WHERE Inv_ItemMaster.N_CompanyID="+myFunctions.getVAL(DetailTable.Rows[j]["N_CompanyID"].ToString())+" AND Inv_ItemMaster.N_ItemID= "+myFunctions.getVAL(DetailTable.Rows[j]["N_ItemID"].ToString()), connection, transaction);
                                }
                            }
                            catch (Exception ex)
                            {
                                // transaction.Rollback();
                                // return Ok(_api.Error(User, ex.Message));
                                transaction.Rollback();
                                throw ex; 
                                // Result.Add("b_IsCompleted", 0);
                                // Result.Add("x_Msg", ex.Message);
                                //return Result;
                            }
                        }

                        // myFunctions.SendApprovalMail(N_NextApproverID, this.N_FormID, N_PkeyID, "PURCHASE", values, dLayer, connection, transaction, User);
                        // transaction.Commit();
                        // return Ok(_api.Success("Purchase Approved " + "-" + values));
                        Result.Add("b_IsCompleted", 1);
                        Result.Add("x_Msg", "Purchase Approved " + "-" + values);
                        return Result;
                    }

                    // if (values == "@Auto")
                    // {
                    //     N_SaveDraft = myFunctions.getIntVAL(masterRow["b_IsSaveDraft"].ToString());

                    //     Params.Add("N_CompanyID", nCompanyID);
                    //     Params.Add("N_YearID", nFnYearID);
                    //     Params.Add("N_FormID", this.N_FormID);
                    //     Params.Add("N_BranchID", masterRow["n_BranchId"].ToString());

                    //     InvoiceNo = dLayer.GetAutoNumber("Inv_Purchase", "x_InvoiceNo", Params, connection, transaction);
                    //     if (InvoiceNo == "")
                    //     {
                    //         transaction.Rollback();
                    //         return Ok(_api.Error(User, "Unable to generate Invoice Number"));
                    //     }
                    //     MasterTable.Rows[0]["x_InvoiceNo"] = InvoiceNo;
                    // }
                    if (N_PurchaseID == 0 && values != "@Auto")
                    {
                        object N_DocNumber = dLayer.ExecuteScalar("Select 1 from Inv_Purchase Where X_InvoiceNo ='" + values + "' and N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection, transaction);
                        if (N_DocNumber == null)
                        {
                            N_DocNumber = 0;
                        }
                        if (myFunctions.getVAL(N_DocNumber.ToString()) >= 1)
                        {
                            // transaction.Rollback();
                            // return Ok(_api.Error(User, "Invoice number already in use"));
                            Result.Add("b_IsCompleted", 0);
                            Result.Add("x_Msg", "Invoice number already in use");
                            return Result;
                        }
                    }
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID", MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 65);
                        
                        while (true)
                        {
                            InvoiceNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            xButtonAction="Insert"; 
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_Purchase Where X_InvoiceNo ='" + values + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        if (InvoiceNo == "")
                        {
                            // transaction.Rollback();
                            // return Ok(_api.Error(User, "Unable to generate Invoice Number"));
                            Result.Add("b_IsCompleted", 0);
                            Result.Add("x_Msg", "Unable to generate Invoice Number");
                            return Result;
                        }
                        MasterTable.Rows[0]["x_InvoiceNo"] = InvoiceNo;

                        
                        // object invoiceCount ;
                        if(vendorInvoice!="")
                        {
                                object invoiceCount = dLayer.ExecuteScalar("select count(N_PurchaseID) as Count from inv_purchase where N_CompanyID= " + nCompanyID + " and X_VendorInvoice= '" +vendorInvoice +"' and N_VendorID = " + N_VendorID, connection, transaction);

                        if (myFunctions.getIntVAL(invoiceCount.ToString()) >0)
                            {
                                // transaction.Rollback();
                                // return Ok(_api.Error(User, "vendor invoice number already exist"));
                                Result.Add("b_IsCompleted", 0);
                                Result.Add("x_Msg", "vendor invoice number already exist");
                                return Result;
                            }
                        }
                    }
                    InvoiceNo = MasterTable.Rows[0]["x_InvoiceNo"].ToString();

                    if (N_PurchaseID > 0)
                    {
                        // SortedList DeleteParams = new SortedList(){
                        //         {"N_CompanyID",masterRow["n_CompanyId"].ToString()},
                        //         {"X_TransType","PURCHASE"},
                        //         {"N_VoucherID",N_PurchaseID},
                        //                                         {"N_UserID",nUserID},
                        //         {"X_SystemName","WebRequest"},
                        //         {"B_MRNVisible",n_MRNID>0?"1":"0"}};

                        // try
                        // {
                        //     dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", DeleteParams, connection, transaction);
                        // }
                        // catch (Exception ex)
                        // {
                        //     transaction.Rollback();
                        //     if (ex.Message.Contains("50"))
                        //         return Ok(_api.Error(User, "DayClosed"));
                        //     else if (ex.Message.Contains("51"))
                        //         return Ok(_api.Error(User, "YearClosed"));
                        //     else if (ex.Message.Contains("52"))
                        //         return Ok(_api.Error(User, "YearExists"));
                        //     else if (ex.Message.Contains("53"))
                        //         return Ok(_api.Error(User, "PeriodClosed"));
                        //     else if (ex.Message.Contains("54"))
                        //         return Ok(_api.Error(User, "TxnDate"));
                        //     else if (ex.Message.Contains("55"))
                        //         return Ok(_api.Error(User, "TransactionStarted"));
                        //     return Ok(_api.Error(User, ex.Message));
                        // }
                          xButtonAction="Update"; 


                        object OPaymentDone = dLayer.ExecuteScalar("SELECT DISTINCT 1	FROM dbo.Inv_PayReceipt INNER JOIN dbo.Inv_PayReceiptDetails ON dbo.Inv_PayReceipt.N_PayReceiptId = dbo.Inv_PayReceiptDetails.N_PayReceiptId AND dbo.Inv_PayReceipt.N_CompanyID = dbo.Inv_PayReceiptDetails.N_CompanyID " +
                                                                                        " WHERE dbo.Inv_PayReceipt.X_Type='PP' and dbo.Inv_PayReceiptDetails.X_TransType='PURCHASE' and dbo.Inv_PayReceipt.N_CompanyID =" + nCompanyID + " and dbo.Inv_PayReceipt.N_FnYearID=" + nFnYearID + " and  dbo.Inv_PayReceiptDetails.N_InventoryId=" + N_PurchaseID, connection, transaction);
                        if (OPaymentDone != null)
                        {
                            if (myFunctions.getIntVAL(OPaymentDone.ToString()) == 1)
                            {
                                // transaction.Rollback();
                                // return Ok(_api.Error(User, "Purchase Payment processed against this purchase."));
                                Result.Add("b_IsCompleted", 0);
                                Result.Add("x_Msg", "Purchase Payment processed against this purchase.");
                                return Result;
                            }
                        }

                        object OReturnDone = dLayer.ExecuteScalar("SELECT DISTINCT 1 FROM Inv_Purchase INNER JOIN Inv_PurchaseReturnMaster ON Inv_Purchase.N_CompanyID = Inv_PurchaseReturnMaster.N_CompanyID AND Inv_Purchase.N_FnYearID = Inv_PurchaseReturnMaster.N_FnYearID AND Inv_Purchase.N_PurchaseID = Inv_PurchaseReturnMaster.N_PurchaseId " +
                                                                                    " where dbo.Inv_PurchaseReturnMaster.N_CompanyID =" + nCompanyID + " and dbo.Inv_PurchaseReturnMaster.N_FnYearID=" + nFnYearID + " and  dbo.Inv_PurchaseReturnMaster.N_PurchaseId=" + N_PurchaseID, connection, transaction);

                        if (OReturnDone != null)
                        {
                            if (myFunctions.getIntVAL(OReturnDone.ToString()) == 1)
                            {
                                // transaction.Rollback();
                                // return Ok(_api.Error(User, "Purchase Return processed against this purchase."));
                                Result.Add("b_IsCompleted", 0);
                                Result.Add("x_Msg", "Purchase Return processed against this purchase.");
                                return Result;
                            }
                        }

                        dLayer.ExecuteNonQuery(" delete from Acc_VoucherDetails Where N_CompanyID=" + nCompanyID + " and X_VoucherNo='" + values + "' and N_FnYearID=" + nFnYearID + " and X_TransType = 'PURCHASE'", connection, transaction);
                        dLayer.ExecuteNonQuery("Delete FROM Inv_PurchaseFreights WHERE N_PurchaseID = " + N_PurchaseID + " and N_CompanyID = " + nCompanyID, connection, transaction);
                        dLayer.ExecuteNonQuery("Delete from Inv_PurchaseWarranty where N_PurchaseID=" + N_PurchaseID + " and N_CompanyID=" + nCompanyID, connection, transaction);
                        dLayer.ExecuteNonQuery("Delete from Inv_PurchaseDetails where N_PurchaseID=" + N_PurchaseID + " and N_CompanyID=" + nCompanyID, connection, transaction);
                        dLayer.ExecuteNonQuery(" Delete From Inv_Purchase Where (N_PurchaseID = " + N_PurchaseID + " OR (N_PurchaseType =4 and N_PurchaseRefID =  " + N_PurchaseID + ")) and N_CompanyID = " + nCompanyID, connection, transaction);
                        dLayer.ExecuteNonQuery("Delete From Inv_Purchase Where (N_PurchaseID = " + N_PurchaseID + " OR (N_PurchaseType =5 and N_PurchaseRefID =  " + N_PurchaseID + ")) and N_CompanyID = " + nCompanyID, connection, transaction);

                        dLayer.ExecuteNonQuery("Delete from Inv_Purchase where N_PurchaseID=" + N_PurchaseID + " and N_CompanyID=" + nCompanyID, connection, transaction);

                        SortedList StockUpdateParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
	                            {"N_TransID",n_MRNID},
	                            {"X_TransType", "PURCHASE"}};

                        dLayer.ExecuteNonQueryPro("SP_StockDeleteUpdate", StockUpdateParams, connection, transaction);
                        // xButtonAction="Update"; 
                    }
                    MasterTable.Rows[0]["n_userID"] = myFunctions.GetUserID(User);

                    MasterTable.AcceptChanges();

                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);

                    if (MasterTable.Columns.Contains("n_TaxAmtDisp"))
                        MasterTable.Columns.Remove("n_TaxAmtDisp");

                    if (MasterTable.Columns.Contains("N_ActVendorID"))
                    {
                        MasterTable.Rows[0]["N_ActVendorID"] = N_VendorID;
                    }
                    else
                    {
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_ActVendorID", typeof(int), N_VendorID);
                    }

                    N_PurchaseID = dLayer.SaveData("Inv_Purchase", "N_PurchaseID", MasterTable, connection, transaction);

                    if (N_PurchaseID <= 0)
                    {
                        // transaction.Rollback();
                        // return Ok(_api.Error(User, "Unable to save Purchase Invoice!"));
                        Result.Add("b_IsCompleted", 0);
                        Result.Add("x_Msg", "Unable to save Purchase Invoice!");
                        return Result;
                    }

                    if (B_isImport)
                    {
                        foreach (DataColumn col in DetailsToImport.Columns)
                        {
                            col.ColumnName = col.ColumnName.Replace(" ", "_");
                            col.ColumnName = col.ColumnName.Replace("*", "");
                            col.ColumnName = col.ColumnName.Replace("/", "_");
                        }

                        DetailsToImport.Columns.Add("N_MasterID");
                        DetailsToImport.Columns.Add("X_Type");
                        DetailsToImport.Columns.Add("N_CompanyID");
                        DetailsToImport.Columns.Add("PkeyID");
                        foreach (DataRow dtRow in DetailsToImport.Rows)
                        {
                            dtRow["N_MasterID"] = N_PurchaseID;
                            dtRow["N_CompanyID"] = nCompanyID;
                            dtRow["PkeyID"] = 0;
                        }

                        dLayer.ExecuteNonQuery("delete from Mig_Purchase ", connection, transaction);
                        dLayer.SaveData("Mig_Purchase", "PkeyID", "", "", DetailsToImport, connection, transaction);



                        SortedList ProParam = new SortedList();
                        ProParam.Add("N_CompanyID", nCompanyID);
                        ProParam.Add("N_PKeyID", N_PurchaseID);

                        SortedList ValidationParam = new SortedList();
                        ValidationParam.Add("N_CompanyID", nCompanyID);
                        ValidationParam.Add("N_FnYearID", nFnYearID);
                        ValidationParam.Add("X_Type", "purchase invoice");
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_SetupData_Validation", ValidationParam, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                            // return Ok(_api.Error(User, ex));
                            // Result.Add("b_IsCompleted", 0);
                            // Result.Add("x_Msg", ex);
                           // return Result;
                        }

                        ProParam.Add("X_Type", "purchase invoice");
                        DetailTable = dLayer.ExecuteDataTablePro("SP_ScreenDataImport", ProParam, connection, transaction);
                    }

                    N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "PURCHASE", N_PurchaseID, InvoiceNo, 1, objVendorName.ToString(), 0, "", 0, User, dLayer, connection, transaction);
                    N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_Purchase where N_PurchaseID=" + N_PurchaseID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, connection, transaction).ToString());
                  

                          //   Activity Log
                    //  string ipAddress = "";
                    //  if ( Request.Headers.ContainsKey("X-Forwarded-For"))
                    //     ipAddress = Request.Headers["X-Forwarded-For"];
                    //   else
                    //   ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                      myFunctions.LogScreenActivitys(nFnYearID,N_PurchaseID,InvoiceNo,65,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);


                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        int UnitID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_ItemUnitID from inv_itemunit where N_ItemID=" + myFunctions.getIntVAL(DetailTable.Rows[j]["N_ItemID"].ToString()) + " and N_CompanyID=" + myFunctions.getIntVAL(DetailTable.Rows[j]["N_CompanyID"].ToString()) + " and X_ItemUnit='" + DetailTable.Rows[j]["X_ItemUnit"].ToString() + "'", connection, transaction).ToString());
                        DetailTable.Rows[j]["N_PurchaseID"] = N_PurchaseID;
                        DetailTable.Rows[j]["N_ItemUnitID"] = UnitID;
                    }
                    if (DetailTable.Columns.Contains("X_ItemUnit"))
                        DetailTable.Columns.Remove("X_ItemUnit");
                    int N_InvoiceDetailId = 0;
                    DataTable DetailTableCopy = DetailTable.Copy();
                    DetailTableCopy.AcceptChanges();
                    if (DetailTable.Columns.Contains("n_MRNDetailsID"))
                        DetailTable.Columns.Remove("n_MRNDetailsID");
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        N_InvoiceDetailId = dLayer.SaveDataWithIndex("Inv_PurchaseDetails", "n_PurchaseDetailsID", "", "", j, DetailTable, connection, transaction);
                        if (N_InvoiceDetailId <= 0)
                        {
                            // transaction.Rollback();
                            // return Ok(_api.Error(User, "Unable to save Purchase Invoice!"));
                            Result.Add("b_IsCompleted", 0);
                            Result.Add("x_Msg", "Unable to save Purchase Invoice!");
                            return Result;
                        }

                        if (n_MRNID > 0 && B_MRNVisible)
                        {
                            dLayer.ExecuteScalar("Update Inv_MRNDetails Set N_SPrice=" + myFunctions.getVAL(DetailTableCopy.Rows[j]["N_PPrice"].ToString()) + ",N_PurchaseDetailsID=" + N_InvoiceDetailId + " Where N_ItemID=" + myFunctions.getIntVAL(DetailTableCopy.Rows[j]["N_ItemID"].ToString()) + "  and N_MRNID=" + myFunctions.getVAL(DetailTableCopy.Rows[j]["n_RsID"].ToString()) + " and N_CompanyID=" + nCompanyID + " and N_MRNDetailsID=" + myFunctions.getVAL(DetailTableCopy.Rows[j]["n_MRNDetailsID"].ToString()), connection, transaction);
                            dLayer.ExecuteScalar("Update Inv_MRN Set N_Processed = 1 Where  N_MRNID=" + myFunctions.getVAL(DetailTableCopy.Rows[j]["n_RsID"].ToString()) + " and N_CompanyID=" + nCompanyID, connection, transaction);

                            SortedList UpdateStockParam = new SortedList();
                            UpdateStockParam.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
                            UpdateStockParam.Add("N_MRNID", n_MRNID);
                            UpdateStockParam.Add("N_ItemID", myFunctions.getIntVAL(DetailTableCopy.Rows[j]["N_ItemID"].ToString()));
                            UpdateStockParam.Add("N_SPrice", myFunctions.getVAL(DetailTableCopy.Rows[j]["N_PPrice"].ToString()));

                            dLayer.ExecuteNonQueryPro("[SP_UpdateStock_MRN]", UpdateStockParam, connection, transaction);

                        }
                        if(WarrantyInfo!=null)
                        foreach (DataRow dtWarranty in WarrantyInfo.Rows)
                        {
                            if(myFunctions.getIntVAL(dtWarranty["rowID"].ToString())==j)
                            dtWarranty["N_PurchaseID"] = N_PurchaseID;
                            dtWarranty["N_PurchaseDetailID"] = N_InvoiceDetailId;
                        }

                         SortedList QueryParams = new SortedList();
                            QueryParams.Add("@Cost",myFunctions.getVAL(DetailTableCopy.Rows[j]["N_Cost"].ToString()));
                            QueryParams.Add("@nCompanyID",myFunctions.getVAL(DetailTableCopy.Rows[j]["N_CompanyID"].ToString()));
                            QueryParams.Add("@nItemID",myFunctions.getVAL(DetailTableCopy.Rows[j]["N_ItemID"].ToString()));
                            //dLayer.ExecuteNonQuery("Update Inv_ItemMaster Set N_PurchaseCost=@Cost Where N_ItemID=@nItemID and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                            // dLayer.ExecuteNonQuery("UPDATE Inv_ItemMaster SET Inv_ItemMaster.N_PurchaseCost=LastCost.N_LPrice from Inv_ItemMaster INNER JOIN "+
	                        //                         " (select TOP 1 N_CompanyID,N_ItemID,N_LPrice from Inv_StockMaster where X_Type='Purchase' and N_ItemID=@nItemID AND N_CompanyID=@nCompanyID "+
	                        //                         " order by D_DateIn desc ,N_StockID desc) AS LastCost ON Inv_ItemMaster.N_CompanyID=LastCost.N_CompanyID AND "+
                            //                         " Inv_ItemMaster.N_ItemID=LastCost.N_ItemID WHERE Inv_ItemMaster.N_CompanyID=@nCompanyID AND Inv_ItemMaster.N_ItemID=@nItemID ", QueryParams, connection, transaction);




                    }

                    if(WarrantyInfo!=null && WarrantyInfo.Rows.Count>0){
                    if (WarrantyInfo.Columns.Contains("rowID"))
                        WarrantyInfo.Columns.Remove("rowID");

                    dLayer.SaveData("Inv_PurchaseWarranty", "N_WarrantyID", WarrantyInfo, connection, transaction);
                    }

                    if (N_PurchaseID > 0)
                    {
                        dLayer.ExecuteScalar("Update Inv_PurchaseOrder Set N_Processed=1 , N_PurchaseID=" + N_PurchaseID + " Where N_POrderID=" + n_POrderID + " and N_CompanyID=" + nCompanyID, connection, transaction);
                        dLayer.ExecuteScalar("Update Inv_MRN Set N_Processed=1 Where N_MRNID=" + n_MRNID + " and N_CompanyID=" + nCompanyID, connection, transaction);
             
                        
                        
                        
                        // if (B_ServiceSheet)
                        //     dba.ExecuteNonQuery("Update Inv_VendorServiceSheet Set N_Processed=1  Where N_RefID=" + n_POrderID + " and N_FnYearID=" + nFnYearID + " and N_CompanyID=" + nCompanyID,connection,transaction);

                    }

                    if (PurchaseFreight.Rows.Count > 0)
                    {
                        if (!PurchaseFreight.Columns.Contains("N_PurchaseID"))
                        {
                            PurchaseFreight.Columns.Add("N_PurchaseID");
                        }
                        foreach (DataRow var in PurchaseFreight.Rows)
                        {
                            var["N_PurchaseID"] = N_PurchaseID;
                        }
                        dLayer.SaveData("Inv_PurchaseFreights", "N_PurchaseFreightID", PurchaseFreight, connection, transaction);
                    }

             

                    if (b_FreightAmountDirect == 0)
                    {
                        SortedList ProcParams = new SortedList(){
                            {"N_FPurchaseID", N_PurchaseID},
                            {"N_CompanyID", nCompanyID},
                            {"N_FnYearID", nFnYearID},
                            {"N_FormID", 65},
                        };
                        dLayer.ExecuteNonQueryPro("SP_FillFreightToPurchase", ProcParams, connection, transaction);
                    }



                    if (N_SaveDraft == 0)
                    {
                        try
                        {                    
                            SortedList PostingMRNParam = new SortedList();
                            PostingMRNParam.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
                            PostingMRNParam.Add("N_PurchaseID", N_PurchaseID);
                            PostingMRNParam.Add("N_UserID", nUserID);
                            PostingMRNParam.Add("X_SystemName", "ERP Cloud");
                            PostingMRNParam.Add("X_UseMRN", "");
                            PostingMRNParam.Add("N_SaveDraft", N_SaveDraft);
                            PostingMRNParam.Add("B_DirectPurchase", Dir_Purchase);
                            PostingMRNParam.Add("N_MRNID", n_MRNID);

                             dLayer.ExecuteNonQueryPro("[SP_Inv_MRNprocessing]", PostingMRNParam, connection, transaction);

                            SortedList PostingParam = new SortedList();
                            PostingParam.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
                            PostingParam.Add("X_InventoryMode", "PURCHASE");
                            PostingParam.Add("N_InternalID", N_PurchaseID);
                            PostingParam.Add("N_UserID", nUserID);
                            PostingParam.Add("X_SystemName", "ERP Cloud");
                            PostingParam.Add("MRN_Flag", Dir_Purchase==0 ? "1" : "0");

                             dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Purchase_Posting", PostingParam, connection, transaction);

                            SortedList StockOutParam = new SortedList();
                            StockOutParam.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());

                            dLayer.ExecuteNonQueryPro("SP_StockOutUpdate", StockOutParam, connection, transaction);
                            
                            for (int j = 0; j < DetailTable.Rows.Count; j++)
                            {
                                dLayer.ExecuteScalar("UPDATE Inv_ItemMaster SET Inv_ItemMaster.N_PurchaseCost=LastCost.N_LPrice from Inv_ItemMaster INNER JOIN "+
                                                " (select TOP 1 N_CompanyID,N_ItemID,N_LPrice from Inv_StockMaster where X_Type='Purchase' and N_ItemID="+myFunctions.getVAL(DetailTable.Rows[j]["N_ItemID"].ToString())+" "+
                                                " AND N_CompanyID= "+ myFunctions.getVAL(DetailTable.Rows[j]["N_CompanyID"].ToString()) +" order by D_DateIn desc ,N_StockID desc) AS LastCost ON Inv_ItemMaster.N_CompanyID=LastCost.N_CompanyID AND "+
                                                " Inv_ItemMaster.N_ItemID=LastCost.N_ItemID WHERE Inv_ItemMaster.N_CompanyID="+myFunctions.getVAL(DetailTable.Rows[j]["N_CompanyID"].ToString())+" AND Inv_ItemMaster.N_ItemID= "+myFunctions.getVAL(DetailTable.Rows[j]["N_ItemID"].ToString()), connection, transaction);
                            }
                        }
                        catch (Exception ex)
                        {
                            Result.Add("b_IsCompleted", 0);
                            if (ex.Message.Contains("50"))
                                Result.Add("x_Msg", "Day Closed");
                            else if (ex.Message.Contains("51"))
                                Result.Add("x_Msg", "Year Closed");
                            else if (ex.Message.Contains("52"))
                                Result.Add("x_Msg", "Year Exists");
                            else if (ex.Message.Contains("53"))
                                Result.Add("x_Msg", "Period Closed");
                            else if (ex.Message.Contains("54"))
                                Result.Add("x_Msg", "Wrong Txn Date");
                            else if (ex.Message.Contains("55"))
                                Result.Add("x_Msg", "Transaction Started");
                            else
                            {
                                transaction.Rollback();
                                //Result.Add("x_Msg", ex.Message);
                                throw ex;
                            }
                            return Result;
                        }

                            //StatusUpdate
                            int tempPOrderID=0;
                            for (int j = 0; j < DetailTable.Rows.Count; j++)
                            {
                                
                                if(showSellingPrice)
                                {
                                   dLayer.ExecuteScalar("Update Inv_ItemMaster Set N_Rate="+DetailTable.Rows[j]["N_Sprice"]+" Where N_ItemID=" + DetailTable.Rows[j]["N_ItemID"] + " and N_CompanyID=" + nCompanyID, connection, transaction);
                                   dLayer.ExecuteScalar("Update Inv_ItemUnit Set N_SellingPrice="+DetailTable.Rows[j]["N_Sprice"]+" Where N_ItemID=" + DetailTable.Rows[j]["N_ItemID"] + " and N_DefaultType=10 and N_CompanyID=" + nCompanyID, connection, transaction);
                                   object salesUnitQty= dLayer.ExecuteScalar("select  N_Qty from Inv_ItemUnit  Where N_ItemID=" + DetailTable.Rows[j]["N_ItemID"] + " and N_DefaultType=30 and N_CompanyID=" + nCompanyID, connection, transaction);
                                   if(salesUnitQty!=null) 
                                       {
                                        double sellingPrice=myFunctions.getVAL(salesUnitQty.ToString()) * myFunctions.getVAL(DetailTable.Rows[j]["N_Sprice"].ToString());
                                        dLayer.ExecuteScalar("Update Inv_ItemUnit Set N_SellingPrice="+sellingPrice+" Where N_ItemID=" + DetailTable.Rows[j]["N_ItemID"] + " and N_DefaultType=30 and N_CompanyID=" + nCompanyID, connection, transaction);
                                       }
                                        

                                }
                                if (myFunctions.getIntVAL(DetailTable.Rows[j]["n_POrderID"].ToString())> 0 && tempPOrderID!=myFunctions.getIntVAL(DetailTable.Rows[j]["n_POrderID"].ToString()))
                                {
                                    if(!myFunctions.UpdateTxnStatus(nCompanyID,myFunctions.getIntVAL(DetailTable.Rows[j]["n_POrderID"].ToString()),82,false,dLayer,connection,transaction))
                                    {
                                        // xturn Ok(_api.Error(User, "Unable To Update Txn Status"));

                                        Result.Add("b_IsCompleted", 0);
                                        Result.Add("x_Msg", "Unable To Update Txn Status");
                                        return Result;
                                    }
                                }
                                tempPOrderID=myFunctions.getIntVAL(DetailTable.Rows[j]["n_POrderID"].ToString());
                            };
                    }
                    SortedList VendorParams = new SortedList();
                    VendorParams.Add("@nVendorID", N_VendorID);
                    DataTable VendorInfo = dLayer.ExecuteDataTable("Select X_VendorCode,X_VendorName from Inv_Vendor where N_VendorID=@nVendorID", VendorParams, connection, transaction);
                    if (VendorInfo.Rows.Count > 0)
                    {
                        try
                        {
                            myAttachments.SaveAttachment(dLayer, Attachment, InvoiceNo, N_PurchaseID, VendorInfo.Rows[0]["X_VendorName"].ToString().Trim(), VendorInfo.Rows[0]["X_VendorCode"].ToString(), N_VendorID, "Vendor Document", User, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                             transaction.Rollback();
                            // return Ok(_api.Error(User, ex));
                            // Result.Add("b_IsCompleted", 0);
                            // Result.Add("x_Msg", ex);
                            // return Result;
                            throw ex;
                        }
                    }
                    Result.Add("b_IsCompleted", 1);
                    Result.Add("x_Msg", "Purchase Invoice Saved");
                    Result.Add("n_InvoiceID", N_PurchaseID);
                    Result.Add("x_InvoiceNo", InvoiceNo);
                    return Result;
                    //myFunctions.SendApprovalMail(N_NextApproverID, this.N_FormID, N_PurchaseID, "PURCHASE", InvoiceNo, dLayer, connection, transaction, User);

                //      transaction.Commit();
            //     }
            //     SortedList Result = new SortedList();
            //     Result.Add("n_InvoiceID", N_PurchaseID);
            //     Result.Add("x_InvoiceNo", InvoiceNo);
            //     return Ok(_api.Success(Result, "Purchase Invoice Saved"));


            // }
            
            // catch (Exception ex)
            // {
            //     return Ok(_api.Error(User, ex));
            // }
        }
    
        public SortedList SalesSaveData(DataSet ds,string ipAddress, ClaimsPrincipal User,IDataAccessLayer dLayer,SqlConnection connection, SqlTransaction transaction)
        {
            DataTable MasterTable;
            DataTable DetailTable;
            DataTable dtsaleamountdetails; ;
            DataTable AdvanceTable; ;
            DataTable Prescription; 
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            AdvanceTable = ds.Tables["advanceTable"];
            Prescription = ds.Tables["prescription"];

            DataTable Approvals;
            Approvals = ds.Tables["approval"];
            DataRow ApprovalRow = Approvals.Rows[0];

            dtsaleamountdetails = ds.Tables["saleamountdetails"];
            DataTable Attachment = ds.Tables["attachments"];

            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();
             SortedList Result = new SortedList();
            // Auto Gen 
            string InvoiceNo = "";
            string xButtonAction = "";

            DataRow MasterRow = MasterTable.Rows[0];

            int N_SalesID = myFunctions.getIntVAL(MasterRow["n_SalesID"].ToString());
            int N_SID = myFunctions.getIntVAL(MasterRow["n_SalesID"].ToString());
            int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
            int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
            int N_BranchID = myFunctions.getIntVAL(MasterRow["n_BranchID"].ToString());
            int N_LocationID = myFunctions.getIntVAL(MasterRow["n_LocationID"].ToString());
            int N_CustomerID = myFunctions.getIntVAL(MasterRow["n_CustomerID"].ToString());

            int N_PaymentMethodID = myFunctions.getIntVAL(MasterRow["n_PaymentMethodID"].ToString());

            if(N_PaymentMethodID==0){
                    // transaction.Rollback();
                    // return Ok(_api.Error(User, "No payment method selected !!"));
                    Result.Add("b_IsCompleted", 0);
                    Result.Add("x_Msg", "No payment method selected !!");
                    return Result;
            }

            int N_DeliveryNoteID = myFunctions.getIntVAL(MasterRow["n_DeliveryNoteId"].ToString());
                int N_ServiceID = MasterTable.Columns.Contains("N_ServiceID")? myFunctions.getIntVAL(MasterRow["N_ServiceID"].ToString()):0;
            int N_CreatedUser = myFunctions.getIntVAL(MasterRow["n_CreatedUser"].ToString());
            int N_UserID = myFunctions.getIntVAL(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int UserCategoryID = myFunctions.getIntVAL(User.FindFirst(ClaimTypes.GroupSid)?.Value);
            int N_AmtSplit = 0;
            int N_IsProforma = 0;
            int N_SaveDraft = myFunctions.getIntVAL(MasterRow["b_IsSaveDraft"].ToString());
            N_IsProforma = MasterTable.Columns.Contains("b_IsProforma") ? myFunctions.getIntVAL(MasterRow["b_IsProforma"].ToString()) : 0;
            bool B_AllBranchData = false, B_AllowCashPay = false, B_DirectPosting = false;
            int N_NextApproverID = 0;
            int AdvanceSettlementID = 0;


            QueryParams.Add("@nCompanyID", N_CompanyID);
            QueryParams.Add("@nFnYearID", N_FnYearID);
            QueryParams.Add("@nSalesID", N_SalesID);
            QueryParams.Add("@nBranchID", N_BranchID);
            QueryParams.Add("@nLocationID", N_LocationID);
            QueryParams.Add("@nCustomerID", N_CustomerID); 
            int N_FormID = 0;
                if (MasterTable.Columns.Contains("N_FormID"))
            {
                N_FormID = myFunctions.getIntVAL(MasterRow["N_FormID"].ToString());
            }


            if (!myFunctions.CheckActiveYearTransaction(N_CompanyID, N_FnYearID, DateTime.ParseExact(MasterTable.Rows[0]["D_SalesDate"].ToString(), "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture), dLayer, connection, transaction))
            {
                object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID=@nCompanyID and convert(date ,'" + MasterTable.Rows[0]["D_SalesDate"].ToString() + "') between D_Start and D_End", QueryParams, connection, transaction);
                if (DiffFnYearID != null)
                {
                    MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
                    N_FnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());
                    QueryParams["@nFnYearID"] = N_FnYearID;

                    SortedList PostingParam = new SortedList();
                    PostingParam.Add("N_PartyID", N_CustomerID);
                    PostingParam.Add("N_FnyearID", N_FnYearID);
                    PostingParam.Add("N_CompanyID", N_CompanyID);
                    PostingParam.Add("X_Type", "customer");


                     object custCount = dLayer.ExecuteScalar("Select count(*) From Inv_Customer where N_FnYearID=@nFnYearID and N_CompanyID=@nCompanyID and N_CustomerID=@nCustomerID", QueryParams, connection, transaction);
                      
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
                else
                {
                    // transaction.Rollback();
                    // return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
                    Result.Add("b_IsCompleted", 0);
                    Result.Add("x_Msg", "Transaction date must be in the active Financial Year.");
                    return Result;
                }
            }
            MasterTable.AcceptChanges();
            B_DirectPosting = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select B_DirPosting from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID", QueryParams, connection, transaction).ToString());
            object objAllBranchData = dLayer.ExecuteScalar("Select B_ShowAllData From Acc_BranchMaster where N_BranchID=@nBranchID and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
            if (objAllBranchData != null)
                B_AllBranchData = myFunctions.getBoolVAL(objAllBranchData.ToString());

            if (B_AllBranchData)
                B_AllowCashPay = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select cast(count(N_CustomerID) as bit) from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID  and N_AllowCashPay=1", QueryParams, connection, transaction).ToString());
            else
                B_AllowCashPay = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select cast(count(N_CustomerID) as bit) from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID  and N_AllowCashPay=1 and (N_BranchId=@nBranchID or N_BranchId=0)", QueryParams, connection, transaction).ToString());


            //saving data
            InvoiceNo = MasterRow["x_ReceiptNo"].ToString();

            SortedList CustParams = new SortedList();
            CustParams.Add("@nCompanyID", N_CompanyID);
            CustParams.Add("@N_CustomerID", N_CustomerID);
            CustParams.Add("@nFnYearID", N_FnYearID);
            object objCustName = dLayer.ExecuteScalar("Select X_CustomerName From Inv_Customer where N_CustomerID=@N_CustomerID and N_CompanyID=@nCompanyID  and N_FnYearID=@nFnYearID", CustParams, connection, transaction);
            object objCustCode = dLayer.ExecuteScalar("Select X_CustomerCode From Inv_Customer where N_CustomerID=@N_CustomerID and N_CompanyID=@nCompanyID  and N_FnYearID=@nFnYearID", CustParams, connection, transaction);


            if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()) && N_SalesID > 0)
            {
                int N_PkeyID = N_SalesID;
                string X_Criteria = "N_SalesID=" + N_PkeyID + " and N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID;
                myFunctions.UpdateApproverEntry(Approvals, "Inv_Sales", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                N_NextApproverID = myFunctions.LogApprovals(Approvals, N_FnYearID, "SALES", N_PkeyID, InvoiceNo, 1, objCustName.ToString(), 0, "",0, User, dLayer, connection, transaction);
                myAttachments.SaveAttachment(dLayer, Attachment, InvoiceNo, N_SalesID, objCustName.ToString().Trim(), objCustCode.ToString(), N_CustomerID, "Customer Document", User, connection, transaction);

                N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_Sales where N_SalesID=" + N_SalesID + " and N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID, connection, transaction).ToString());
                if (N_SaveDraft == 0)
                {
                    SortedList PostingParam = new SortedList();
                    PostingParam.Add("N_CompanyID", N_CompanyID);
                    PostingParam.Add("X_InventoryMode", "SALES");
                    PostingParam.Add("N_InternalID", N_SalesID);
                    PostingParam.Add("N_UserID", N_UserID);
                    PostingParam.Add("X_SystemName", "ERP Cloud");
                    try 
                    {
                        dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Sales_Posting", PostingParam, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        // return Ok(_api.Error(User, ex));
                        // Result.Add("b_IsCompleted", 0);
                        // Result.Add("x_Msg", ex);
                        // return Result;
                        throw ex;
                    }
                    bool B_AmtpaidEnable = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "Show SalesAmt Paid", "N_Value", "N_UserCategoryID", "0", N_CompanyID, dLayer, connection, transaction)));
                    if (B_AmtpaidEnable)
                    {
                        if (!B_DirectPosting)
                        {
                            if (myFunctions.getVAL(MasterRow["N_CashReceived"].ToString()) > 0)
                            {
                                SortedList ParamCustomerRcpt_Ins = new SortedList();
                                ParamCustomerRcpt_Ins.Add("N_CompanyID", N_CompanyID);
                                ParamCustomerRcpt_Ins.Add("N_Fn_Year", N_FnYearID);
                                ParamCustomerRcpt_Ins.Add("N_SalesId", N_SalesID);
                                ParamCustomerRcpt_Ins.Add("N_Amount", myFunctions.getVAL(MasterRow["N_CashReceived"].ToString()));
                                try
                                {
                                    dLayer.ExecuteNonQueryPro("SP_CustomerRcpt_Ins", ParamCustomerRcpt_Ins, connection, transaction);
                                }
                                catch (Exception ex)
                                {
                                    // transaction.Rollback();
                                    // return Ok(_api.Error(User, "Unable to save Sales Invoice!"));
                                    Result.Add("b_IsCompleted", 0);
                                    Result.Add("x_Msg", "Unable to save Sales Invoice!");
                                    return Result;
                                }
                            }
                        }

                    }
                }

                //myFunctions.SendApprovalMail(N_NextApproverID, this.N_FormID, N_PkeyID, "SALES", InvoiceNo, dLayer, connection, transaction, User);
                // transaction.Commit();
                // return Ok(_api.Success("Sales Approved " + "-" + InvoiceNo));
                Result.Add("b_IsCompleted", 1);
                Result.Add("x_Msg", "Sales Approved " + "-" + InvoiceNo);
                return Result;
            }


            if (N_SaveDraft == 1)
            {
                if (N_SalesID == 0 && InvoiceNo != "@Auto")
                {
                    object N_DocNumber = dLayer.ExecuteScalar("Select 1 from Inv_Sales Where X_ReceiptNo ='" + InvoiceNo + "' and N_CompanyID= " + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection, transaction);
                    if (N_DocNumber == null)
                    {
                        N_DocNumber = 0;
                    }
                    if (myFunctions.getVAL(N_DocNumber.ToString()) >= 1)
                    {
                        // transaction.Rollback();
                        // return Ok(_api.Error(User, "Invoice number already in use"));
                        Result.Add("b_IsCompleted", 0);
                        Result.Add("x_Msg", "Invoice number already in use");
                        return Result;
                    }
                }
                if (InvoiceNo == "@Auto")
                {
                    Params.Add("N_CompanyID", MasterRow["n_CompanyId"].ToString());
                    Params.Add("N_YearID", MasterRow["n_FnYearId"].ToString());
                    if (N_IsProforma == 1)
                        Params.Add("N_FormID", 1346);
                    else
                        Params.Add("N_FormID", 64);

                    while (true)
                    {
                        InvoiceNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                        object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_Sales Where X_ReceiptNo ='" + InvoiceNo + "' and N_CompanyID= " + N_CompanyID, connection, transaction);
                        if (N_Result == null)
                            break;
                    }
                    if (InvoiceNo == "")
                    {
                        // transaction.Rollback();
                        // return Ok(_api.Error(User, "Unable to generate Invoice Number"));
                        Result.Add("b_IsCompleted", 0);
                        Result.Add("x_Msg", "Unable to generate Invoice Number");
                        return Result;
                    }
                    MasterTable.Rows[0]["x_ReceiptNo"] = InvoiceNo;
                }
                xButtonAction = "INSERT";
            }
            else
            {
                object N_Resultval = dLayer.ExecuteScalar("Select 1 from Inv_Sales Where X_ReceiptNo ='" + InvoiceNo + "' and N_CompanyID= " + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection, transaction);
                if (N_Resultval == null)
                {
                    N_Resultval = 0;
                }
                if (N_SalesID == 0 && myFunctions.getVAL(N_Resultval.ToString()) >= 1)
                {
                    // transaction.Rollback();
                    // return Ok(_api.Error(User, "Invoice number already in use"));
                    Result.Add("b_IsCompleted", 0);
                    Result.Add("x_Msg", "Invoice number already in use");
                    return Result;
                }

                if (InvoiceNo == "@Auto")
                {

                    Params.Add("N_CompanyID", MasterRow["n_CompanyId"].ToString());
                    Params.Add("N_YearID", MasterRow["n_FnYearId"].ToString());
                    if (N_IsProforma == 1)
                        Params.Add("N_FormID", 1346);
                    else
                        Params.Add("N_FormID", 64);
                    Params.Add("N_BranchID", MasterRow["n_BranchId"].ToString());
                    while (true)
                    {
                        InvoiceNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                        object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_Sales Where X_ReceiptNo ='" + InvoiceNo + "' and N_CompanyID= " + N_CompanyID, connection, transaction);
                        if (N_Result == null)
                            break;
                    }
                    if (InvoiceNo == "")
                    {
                        // transaction.Rollback();
                        // return Ok(_api.Error(User, "Unable to generate Quotation Number"));
                        Result.Add("b_IsCompleted", 0);
                        Result.Add("x_Msg", "Unable to generate Quotation Number");
                        return Result;
                    }
                    MasterTable.Rows[0]["x_ReceiptNo"] = InvoiceNo;
                    xButtonAction = "INSERT";
                }

            }
            if (N_SalesID > 0)
            {
                    string payRecieptqry = "select N_PayReceiptID from  Inv_PayReceipt where N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID + " and N_RefID=" + N_SalesID + " and N_FormID=64";
                    object nRecieptID = dLayer.ExecuteScalar(payRecieptqry, Params, connection, transaction);
                    if (nRecieptID != null && myFunctions.getIntVAL(nRecieptID.ToString()) > 0)
                    {
                        dLayer.ExecuteNonQuery(" delete from Acc_VoucherDetails Where N_CompanyID=" + N_CompanyID + " and N_InventoryID=" + myFunctions.getIntVAL(nRecieptID.ToString()) + " and N_FnYearID=" + N_FnYearID + " and X_TransType = 'SA'", connection, transaction);
                        dLayer.ExecuteNonQuery(" delete from Inv_PayReceiptDetails Where N_CompanyID=" + N_CompanyID + " and N_PayReceiptID=" + myFunctions.getIntVAL(nRecieptID.ToString()) + " ", connection, transaction);
                        dLayer.ExecuteNonQuery(" delete from Inv_PayReceipt Where N_CompanyID=" + N_CompanyID + " and N_PayReceiptID=" + myFunctions.getIntVAL(nRecieptID.ToString()) + " and  N_FnYearID=" + N_FnYearID + " ", connection, transaction);
                        dLayer.DeleteData("Inv_SalesAdvanceSettlement", "N_SalesID", N_SalesID, "N_CompanyID = " + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection, transaction);
                    }

                SortedList DeleteParams = new SortedList(){
                        {"N_CompanyID",N_CompanyID},
                        {"X_TransType","SALES"},
                        {"N_VoucherID",N_SalesID}};
                try
                {
                    dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // return Ok(_api.Error(User, ex));
                    // Result.Add("b_IsCompleted", 0);
                    // Result.Add("x_Msg", ex);
                    // return Result;
                    throw ex;
                }

                dLayer.ExecuteNonQuery("delete from Inv_SaleAmountDetails where N_SalesID=" + N_SalesID + " and N_CompanyID=" + N_CompanyID + " and N_BranchID=" + N_BranchID, connection, transaction);
                dLayer.ExecuteNonQuery("delete from Inv_LoyaltyPointOut where N_TransID=" + N_SalesID + " and N_CompanyID=" + N_CompanyID + " and N_PartyId=" + N_CustomerID, connection, transaction);
                dLayer.ExecuteNonQuery("delete from Inv_ServiceContract where N_SalesID=" + N_SalesID + " and N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID + " and N_BranchID=" + N_BranchID, connection, transaction);
                xButtonAction = "Update";
            }
            MasterTable.Rows[0]["n_UserID"] = myFunctions.GetUserID(User);
            MasterTable.AcceptChanges();

            MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);

            string dupInvNo = InvoiceNo;
            if (MasterRow["x_ReceiptNo"].ToString() != "@Auto")
                dupInvNo = MasterRow["x_ReceiptNo"].ToString();

            string DupCriteria = "N_CompanyID=" + N_CompanyID + " and X_ReceiptNo='" + dupInvNo + "' and N_FnyearID=" + N_FnYearID;
            N_SalesID = dLayer.SaveData("Inv_Sales", "N_SalesId", DupCriteria, "", MasterTable, connection, transaction);
            if (N_SalesID <= 0)
            {
                // transaction.Rollback();
                // return Ok(_api.Error(User, "Unable to save Sales Invoice!"));
                Result.Add("b_IsCompleted", 0);
                Result.Add("x_Msg", "Unable to save Sales Invoice!");
                return Result;
            }
            else
            {
                // if (B_UserLevel)
                // {
                //     Inv_WorkFlowCatalog saving code here
                // }                   

                //Inv_WorkFlowCatalog insertion here
                //DataTable dtsaleamountdetails = ds.Tables["saleamountdetails"];

                N_NextApproverID = myFunctions.LogApprovals(Approvals, N_FnYearID, "SALES", N_SalesID, InvoiceNo, 1, objCustName.ToString(), 0, "",0, User, dLayer, connection, transaction);
                N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_Sales where N_SalesID=" + N_SalesID + " and N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID, connection, transaction).ToString());

                DataTable dtloyalitypoints = ds.Tables["loyalitypoints"];
                int N_IsSave = 1;
                int N_CurrentSalesID = 0;
                if (ds.Tables["saleamountdetails"].Rows.Count > 0)
                {
                    DataRow Rowsaleamountdetails = ds.Tables["saleamountdetails"].Rows[0];
                    // N_IsSave = myFunctions.getIntVAL(Rowsaleamountdetails["n_IsSave"].ToString());
                    // dtsaleamountdetails.Columns.Remove("n_IsSave");
                    // dtsaleamountdetails.AcceptChanges();
                    N_CurrentSalesID = myFunctions.getIntVAL(Rowsaleamountdetails["N_SalesID"].ToString());
                }
                //Activity Log

                // string ipAddress = "";
                // if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                //     ipAddress = Request.Headers["X-Forwarded-For"];
                // else
                //     ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                myFunctions.LogScreenActivitys(N_FnYearID,N_SalesID,InvoiceNo,64,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                   

                DataRow Rowloyalitypoints = null;
                if (ds.Tables.Contains("loyalitypoints"))
                    Rowloyalitypoints = ds.Tables["loyalitypoints"].Rows[0];

                // int N_IsSave = myFunctions.getIntVAL(Rowsaleamountdetails["n_IsSave"].ToString());
                // dtsaleamountdetails.Columns.Remove("n_IsSave");
                // dtsaleamountdetails.AcceptChanges();

                // int N_CurrentSalesID = myFunctions.getIntVAL(Rowsaleamountdetails["N_SalesID"].ToString());
                bool B_EnablePointSystem = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("64", "AllowLoyaltyPoint", "N_Value", "N_UserCategoryID", UserCategoryID.ToString(), N_CompanyID, dLayer, connection, transaction)));
                bool B_SalesOrder = myFunctions.CheckPermission(N_CompanyID, 81, "Administrator", "X_UserCategory", dLayer, connection, transaction);
                //Sales amount details/payment popup
                for (int i = 0; i < dtsaleamountdetails.Rows.Count; i++)
                    dtsaleamountdetails.Rows[i]["N_SalesId"] = N_SalesID;
                if (N_AmtSplit == 1)
                {

                    if (N_IsSave == 1)
                    {

                        int N_SalesAmountID = dLayer.SaveData("Inv_SaleAmountDetails", "n_SalesAmountID", dtsaleamountdetails, connection, transaction);
                        if (N_SalesAmountID <= 0)
                        {
                            // transaction.Rollback();
                            // return Ok(_api.Error(User, "Unable to save Sales Invoice!"));
                            Result.Add("b_IsCompleted", 0);
                            Result.Add("x_Msg", "Unable to save Sales Invoice!");
                            return Result;
                        }
                        else
                        {
                            if (B_EnablePointSystem)
                            {
                                if (ds.Tables.Contains("loyalitypoints") && dtloyalitypoints.Rows.Count > 0)
                                {
                                    int N_PointOutId = dLayer.SaveData("Inv_LoyaltyPointOut", "n_PointOutId", dtloyalitypoints, connection, transaction);
                                    if (N_SalesAmountID <= 0)
                                    {
                                        // transaction.Rollback();
                                        // return Ok(_api.Error(User, "Unable to save Sales Invoice!"));
                                        Result.Add("b_IsCompleted", 0);
                                        Result.Add("x_Msg", "Unable to save Sales Invoice!");
                                        return Result;
                                    }
                                    else
                                    {
                                        double N_DiscountAmt = myFunctions.getVAL(Rowloyalitypoints["N_AppliedAmt"].ToString()) + myFunctions.getVAL(MasterRow["N_DiscountAmt"].ToString());
                                        dLayer.ExecuteNonQuery("update  Inv_Sales  Set N_DiscountAmt=" + N_DiscountAmt + " where N_SalesID=@nSalesID and N_CompanyID=@nCompanyID and N_CustomerID=@nCustomerID", QueryParams, connection, transaction);
                                    }
                                }
                            }
                        }
                    }
                    else if (N_IsSave == 0)
                    {
                        if (N_CurrentSalesID != N_SalesID)
                            dLayer.ExecuteNonQuery("update  Inv_SaleAmountDetails set N_SalesID=" + N_SalesID + " where N_SalesID=@nSalesID and N_CompanyID=@nCompanyID and N_BranchID=@nBranchID", QueryParams, connection, transaction);
                    }
                }
                else
                {
                        if (N_SaveDraft == 0)
                        {
                            if (dtsaleamountdetails.Columns.Contains("N_CommissionAmtF"))
                                dtsaleamountdetails.Columns.Remove("N_CommissionAmtF");
                            if (dtsaleamountdetails.Columns.Contains("N_CommissionAmt"))
                                dtsaleamountdetails.Columns.Remove("N_CommissionAmt");

                             dtsaleamountdetails = myFunctions.AddNewColumnToDataTable(dtsaleamountdetails, "N_CommissionAmtF", typeof(double), 0);
                             dtsaleamountdetails = myFunctions.AddNewColumnToDataTable(dtsaleamountdetails, "N_CommissionAmt", typeof(double), 0);
                            //foreach (DataRow data in dtsaleamountdetails.Rows)
                            for (int i = 0; i < dtsaleamountdetails.Rows.Count; i++)
                            {
                                    double N_SChrgAmt = 0;
                                    double N_SChrgAmtMax = 0;
                                    object N_ServiceCharge = dLayer.ExecuteScalar("Select ISNULL(N_ServiceCharge , 0) from Inv_Customer where N_CustomerID=" + myFunctions.getVAL(dtsaleamountdetails.Rows[i]["N_CustomerID"].ToString()) + " and N_CompanyID=" + N_CompanyID + "and N_FnYearID=" +N_FnYearID, QueryParams, connection, transaction);
                                    object N_ServiceChargeMax = dLayer.ExecuteScalar("Select ISNULL(N_ServiceChargeLimit , 0) from Inv_Customer where N_CustomerID=" +  myFunctions.getVAL(dtsaleamountdetails.Rows[i]["N_CustomerID"].ToString()) + " and N_CompanyID=" + N_CompanyID + "and N_FnYearID=" + N_FnYearID,  QueryParams, connection, transaction);
                                    object N_TaxID = dLayer.ExecuteScalar("Select ISNULL(N_TaxCategoryID , 0) from Inv_Customer where N_CustomerID=" +  myFunctions.getVAL(dtsaleamountdetails.Rows[i]["N_CustomerID"].ToString()) + " and N_CompanyID=" + N_CompanyID + "and N_FnYearID=" + N_FnYearID,  QueryParams, connection, transaction);
                                    if (myFunctions.getVAL(N_ServiceCharge.ToString()) > 0)
                                    {
                                        N_SChrgAmt = (myFunctions.getVAL(dtsaleamountdetails.Rows[i]["N_AmountF"].ToString()) * myFunctions.getVAL((N_ServiceCharge.ToString())) / 100);
                                        N_SChrgAmtMax = myFunctions.getVAL(N_ServiceChargeMax.ToString());
                                        if (N_SChrgAmtMax > 0)
                                        {
                                            if (N_SChrgAmt > N_SChrgAmtMax)
                                                N_SChrgAmt = myFunctions.getVAL(N_ServiceChargeMax.ToString());
                                        }
                                    }                                    
                                    double CommissionAmtH = N_SChrgAmt * (myFunctions.getVAL(MasterRow["n_ExchangeRate"].ToString()));
                                    if(myFunctions.getVAL(N_SChrgAmt.ToString())>0)
                                    {
                                        dtsaleamountdetails.Rows[i]["N_CommissionAmtF"]=myFunctions.getVAL(N_SChrgAmt.ToString());
                                        dtsaleamountdetails.Rows[i]["N_CommissionAmt"]=myFunctions.getVAL(CommissionAmtH.ToString());
                                        dtsaleamountdetails.Rows[i]["N_CommissionPer"]=N_ServiceCharge;
                                        dtsaleamountdetails.Rows[i]["N_TaxID"]=N_TaxID;
                                        // data["N_CommissionAmtF"]=myFunctions.getVAL(N_SChrgAmt.ToString());
                                        // data["N_CommissionAmt"]=myFunctions.getVAL(CommissionAmtH.ToString());
                                        // data["N_CommissionPer"]=N_ServiceCharge;                            
                                        // data["N_TaxID"]=N_TaxID;
                                    }
                            }

                    //service charge
            

                int N_SalesAmountID = dLayer.SaveData("Inv_SaleAmountDetails", "n_SalesAmountID", dtsaleamountdetails, connection, transaction);
                if (N_SalesAmountID <= 0)
                {
                    // transaction.Rollback();
                    // return Ok(_api.Error(User, "Unable to save Sales Invoice!"));
                    Result.Add("b_IsCompleted", 0);
                    Result.Add("x_Msg", "Unable to save Sales Invoice!");
                    return Result;
                }
                    }
                }



                bool B_salesOrder = false;
                bool B_DeliveryNote = false;
                B_DeliveryNote = myFunctions.CheckPermission(N_CompanyID, 729, myFunctions.GetUserCategory(User).ToString(), "N_UserCategoryID", dLayer, connection, transaction);
                if (!B_DeliveryNote)
                    B_salesOrder = myFunctions.CheckPermission(N_CompanyID, 81, myFunctions.GetUserCategory(User).ToString(), "N_UserCategoryID", dLayer, connection, transaction);
                bool B_ServiceSheet = myFunctions.CheckPermission(N_CompanyID, 1145, myFunctions.GetUserCategory(User).ToString(), "N_UserCategoryID", dLayer, connection, transaction);
                for (int j = 0; j < DetailTable.Rows.Count; j++)
                {
                    // if (B_salesOrder)
                    // {
                        int nSalesOrderID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesOrderID"].ToString());
                        if (nSalesOrderID > 0)
                        {
                            dLayer.ExecuteNonQuery("Update Inv_SalesOrder Set N_SalesID=" + N_SalesID + ", N_Processed=1 Where N_SalesOrderID=" + nSalesOrderID + " and N_FnYearID=@nFnYearID and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                            if (B_ServiceSheet)
                                dLayer.ExecuteNonQuery("Update Inv_ServiceSheetMaster Set N_Processed=1  Where N_RefID=" + nSalesOrderID + " and N_FnYearID=@nFnYearID and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                        }
                    // }
                    // else
                    // {
                        int nQuotationID = myFunctions.getIntVAL(DetailTable.Rows[j]["N_SalesQuotationID"].ToString());
                        if (nQuotationID > 0)
                            dLayer.ExecuteNonQuery("Update Inv_SalesQuotation Set N_SalesID=" + N_SalesID + ", N_Processed=1 Where N_QuotationID=" + nQuotationID + " and N_FnYearID=@nFnYearID and N_CompanyID=@nCompanyID", QueryParams, connection,transaction);
                    // }
                        // int nDeliveryNoteID = myFunctions.getIntVAL(DetailTable.Rows[j]["N_DeliveryNoteID"].ToString());
                        // if (nDeliveryNoteID > 0)
                        //     dLayer.ExecuteNonQuery("Update Inv_DeliveryNote Set  N_Processed=1 Where N_DeliveryNoteID=" + nDeliveryNoteID + " and N_FnYearID=@nFnYearID and N_CompanyID=@nCompanyID", QueryParams, connection,transaction);
                 

                }

                // Warranty Save Code here
                //optical prescription saving here
                for (int j = 0; j < DetailTable.Rows.Count; j++)
                {
                    DetailTable.Rows[j]["N_SalesId"] = N_SalesID;
                }
                if (DetailTable.Columns.Contains("n_Stock"))
                    DetailTable.Columns.Remove("n_Stock");

                int N_InvoiceDetailId = dLayer.SaveData("Inv_SalesDetails", "n_SalesDetailsID", DetailTable, connection, transaction);
                if (N_InvoiceDetailId <= 0)
                {
                    // transaction.Rollback();
                    // return Ok(_api.Error(User, "Unable to save Sales Invoice!"));
                    Result.Add("b_IsCompleted", 0);
                    Result.Add("x_Msg", "Unable to save Sales Invoice!");
                    return Result;
                }
                else
                {
                    //Advance Settlement Save

                    if (AdvanceTable.Rows.Count > 0)
                    {
                        dLayer.DeleteData("Inv_SalesAdvanceSettlement", "N_SalesID", N_SalesID, "N_CompanyID = " + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection, transaction);
                        for (int j = 0; j < AdvanceTable.Rows.Count; j++)
                        {
                            AdvanceTable.Rows[j]["N_SalesId"] = N_SalesID;
                        }
                        AdvanceTable.AcceptChanges();
                        AdvanceSettlementID = dLayer.SaveData("Inv_SalesAdvanceSettlement", "N_PkeyID", AdvanceTable, connection, transaction);

                        SortedList advanceParams = new SortedList();
                        advanceParams.Add("@N_CompanyID", N_CompanyID);
                        advanceParams.Add("@N_SalesID", N_SalesID);
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_InvSalesAdvSettlement", advanceParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            // return Ok(_api.Error(User, ex));
                            // Result.Add("b_IsCompleted", 0);
                            // Result.Add("x_Msg", ex);
                            // return Result;
                            throw ex;
                        }





                    }
                    //EYE OPTICS
                        if (N_SID > 0)
                        {
                            int nPrescriptionID=myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_PrescriptionID from Inv_Prescription where N_SalesID=" + N_SID.ToString() + " and N_CompanyID=" + N_CompanyID+"",connection, transaction).ToString());
                            
                            dLayer.ExecuteScalar("delete from Inv_Prescription where N_SalesID=" + N_SID.ToString() + " and N_CompanyID=" + N_CompanyID, connection, transaction);
                       if(Prescription.Rows.Count>0)
                            {
                                if(nPrescriptionID>0)
                                    Prescription.Rows[0]["N_PrescriptionID"]=nPrescriptionID;
                            }
                            Prescription.AcceptChanges();
                        }
                    if(Prescription.Rows.Count>0)
                    {
                        Prescription.Rows[0]["N_SalesID"]=N_SalesID;
                        Prescription.AcceptChanges();
                        dLayer.SaveData("Inv_Prescription", "N_PrescriptionID", Prescription, connection, transaction); 
                    }








                    SortedList StockPostingParams = new SortedList();
                    StockPostingParams.Add("N_CompanyID", N_CompanyID);
                    StockPostingParams.Add("N_SalesID", N_SalesID);
                    StockPostingParams.Add("N_SaveDraft", N_SaveDraft);
                    StockPostingParams.Add("N_DeliveryNoteID", N_DeliveryNoteID);
                    if (N_DeliveryNoteID == 0 && N_ServiceID == 0 )
                    {
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_SalesDetails_InsCloud", StockPostingParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            // transaction.Rollback(); 
                            // if (ex.Message == "50") 
                            //     return Ok(_api.Error(User, "Day Closed"));
                            // else if (ex.Message == "51")
                            //     return Ok(_api.Error(User, "Year Closed"));
                            // else if (ex.Message == "52")
                            //     return Ok(_api.Error(User, "Year Exists"));
                            // else if (ex.Message == "53")
                            //     return Ok(_api.Error(User, "Period Closed"));
                            // else if (ex.Message == "54")
                            //     return Ok(_api.Error(User, "Txn Date"));
                            // else if (ex.Message == "55")
                            //     return Ok(_api.Error(User, "Quantity exceeds!"));
                            // else
                            //     return Ok(_api.Error(User, ex));

                            Result.Add("b_IsCompleted", 0);
                            if (ex.Message == "50") 
                                Result.Add("x_Msg", "Day Closed");
                            else if (ex.Message == "51")
                                Result.Add("x_Msg", "Year Closed");
                            else if (ex.Message == "52")
                                Result.Add("x_Msg", "Year Exists");
                            else if (ex.Message == "53")
                                Result.Add("x_Msg", "Period Closed");
                            else if (ex.Message == "54")
                                Result.Add("x_Msg", "Txn Date");
                            else if (ex.Message == "55")
                                Result.Add("x_Msg", "Quantity exceeds!");
                            else
                            {
                                transaction.Rollback();
                                throw ex;
                            }
                                //Result.Add("x_Msg", ex);

                            return Result;
                        }
                    }
                }

                if (N_SaveDraft == 0)
                {
                    SortedList PostingParam = new SortedList();
                    PostingParam.Add("N_CompanyID", N_CompanyID);
                    PostingParam.Add("X_InventoryMode", "SALES");
                    PostingParam.Add("N_InternalID", N_SalesID);
                    PostingParam.Add("N_UserID", N_UserID);
                    PostingParam.Add("X_SystemName", "ERP Cloud");
                    try
                    {
                         dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Sales_Posting", PostingParam, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        //transaction.Rollback();
                        // return Ok(_api.Error(User, ex));
                         Result.Add("b_IsCompleted", 0);
                        // Result.Add("x_Msg", ex);
                        // return Result;
                        if (ex.Message.Contains("50"))
                            Result.Add("x_Msg", "Day Closed");
                        else if (ex.Message.Contains("51"))
                            Result.Add("x_Msg", "Year Closed");
                        else if (ex.Message.Contains("52"))
                            Result.Add("x_Msg", "Year Exists");
                        else if (ex.Message.Contains("53"))
                            Result.Add("x_Msg", "Period Closed");
                        else if (ex.Message.Contains("54"))
                            Result.Add("x_Msg", "Wrong Txn Date");
                        else if (ex.Message.Contains("55"))
                            Result.Add("x_Msg", "Transaction Started");
                        else
                        {
                            transaction.Rollback();
                            throw ex;
                        }

                        return Result;
                    }
                    bool B_AmtpaidEnable = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "Show SalesAmt Paid", "N_Value", "N_UserCategoryID", "0", N_CompanyID, dLayer, connection, transaction)));
                    if (B_AmtpaidEnable)
                    {
                        if (!B_DirectPosting)
                        {
                            if (myFunctions.getVAL(MasterRow["N_CashReceived"].ToString()) > 0)
                            {
                                SortedList ParamCustomerRcpt_Ins = new SortedList();
                                ParamCustomerRcpt_Ins.Add("N_CompanyID", N_CompanyID);
                                ParamCustomerRcpt_Ins.Add("N_Fn_Year", N_FnYearID);
                                ParamCustomerRcpt_Ins.Add("N_SalesId", N_SalesID);
                                ParamCustomerRcpt_Ins.Add("N_Amount", myFunctions.getVAL(MasterRow["N_CashReceived"].ToString()));
                                try
                                {
                                    dLayer.ExecuteNonQueryPro("SP_CustomerRcpt_Ins", ParamCustomerRcpt_Ins, connection, transaction);
                                }
                                catch (Exception ex)
                                {
                                    // transaction.Rollback();
                                    // return Ok(_api.Error(User, "Unable to save Sales Invoice!"));
                                    Result.Add("b_IsCompleted", 0);
                                    Result.Add("x_Msg", "Unable to save Sales Invoice!");
                                    return Result;
                                }
                            }
                        }

                    }
                    //StatusUpdate
                    int tempQtn=0,tempSO=0,tempDevnote=0;
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        int nSalesOrderID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesOrderID"].ToString());
                        int nQuotationID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesQuotationID"].ToString());
                        int nDeliveryNoteID = myFunctions.getIntVAL(DetailTable.Rows[j]["N_DeliveryNoteID"].ToString());
                        if (nSalesOrderID > 0 && tempSO!=nSalesOrderID)
                        {
                            if(!myFunctions.UpdateTxnStatus(N_CompanyID,nSalesOrderID,81,false,dLayer,connection,transaction))
                            {
                                // transaction.Rollback();
                                // return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                Result.Add("b_IsCompleted", 0);
                                Result.Add("x_Msg", "Unable To Update Txn Status");
                                return Result;
                            }
                        }
                        tempSO = nSalesOrderID;

                        if (nQuotationID > 0 && tempQtn!=nQuotationID)
                        {
                            if(!myFunctions.UpdateTxnStatus(N_CompanyID,nQuotationID,80,false,dLayer,connection,transaction))
                            {
                                // transaction.Rollback();
                                // return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                Result.Add("b_IsCompleted", 0);
                                Result.Add("x_Msg", "Unable To Update Txn Status");
                                return Result;
                            }
                        }
                        tempQtn=nQuotationID;

                         if (nDeliveryNoteID > 0 && tempDevnote!=nDeliveryNoteID )
                        {
                            if(!myFunctions.UpdateTxnStatus(N_CompanyID,nDeliveryNoteID,884,false,dLayer,connection,transaction))
                            {
                                // transaction.Rollback();
                                // return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                Result.Add("b_IsCompleted", 0);
                                Result.Add("x_Msg", "Unable To Update Txn Status");
                                return Result;
                            }
                        }
                        tempDevnote=nDeliveryNoteID;
                    };
                }
                SortedList CustomerParams = new SortedList();
                CustomerParams.Add("@nCustomerID", N_CustomerID);
                DataTable CustomerInfo = dLayer.ExecuteDataTable("Select X_CustomerCode,X_CustomerName from Inv_Customer where N_CustomerID=@nCustomerID", CustomerParams, connection, transaction);
                if (CustomerInfo.Rows.Count > 0)
                {
                    try
                    {
                        myAttachments.SaveAttachment(dLayer, Attachment, InvoiceNo, N_SalesID, CustomerInfo.Rows[0]["X_CustomerName"].ToString().Trim(), CustomerInfo.Rows[0]["X_CustomerCode"].ToString(), N_CustomerID, "Customer Document", User, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        // return Ok(_api.Error(User, ex));
                        // Result.Add("b_IsCompleted", 0);
                        // Result.Add("x_Msg", ex);
                        // return Result;
                        throw ex;
                    }
                }
                //dispatch saving here

            }
            //return GetSalesInvoiceDetails(int.Parse(MasterRow["n_CompanyId"].ToString()), int.Parse(MasterRow["n_FnYearId"].ToString()), int.Parse(MasterRow["n_BranchId"].ToString()), InvoiceNo);
            // SortedList Result = new SortedList();
            // Result.Add("n_SalesID", N_SalesID);
            // Result.Add("x_SalesNo", InvoiceNo);

            object N_CustomerVendorID = dLayer.ExecuteScalar("Select N_CustomerVendorID From Inv_Customer where N_CustomerID=@N_CustomerID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", CustParams, connection, transaction);

            if (N_CustomerVendorID.ToString() != "")
            {
                SortedList PurchaseParams = new SortedList();
                PurchaseParams.Add("@N_CompanyID", N_CompanyID);
                PurchaseParams.Add("@N_FnYearID", N_FnYearID);
                PurchaseParams.Add("@N_SalesID", N_SalesID);
                dLayer.ExecuteNonQueryPro("SP_SalesToPurchase_Ins", PurchaseParams, connection, transaction);

            }
            // transaction.Commit();
            //     if (N_FormID == 64)
            //         {
            //         return Ok(_api.Success(Result, "Sales invoice saved"));
            //         }
            //         else if(N_FormID == 1601) 
            //         {
            //     return Ok(_api.Success(Result,"Rental Sales Saved Successfully"));
            //             }
            // return Ok(_api.Success(Result, "Sales invoice saved"));

            Result.Add("b_IsCompleted", 1);

                if (N_FormID == 64)
                Result.Add("x_Msg", "Sales invoice saved");
            else if(N_FormID == 1601) 
                Result.Add("x_Msg", "Rental Sales Saved Successfully");
            else
                Result.Add("x_Msg", "Sales invoice saved");

            Result.Add("n_SalesID", N_SalesID);
            Result.Add("x_SalesNo", InvoiceNo);

            return Result;

        }

        public SortedList SalesReturnSaveData(DataSet ds,string ipAddress, ClaimsPrincipal User,IDataAccessLayer dLayer,SqlConnection connection, SqlTransaction transaction)
        {
            DataTable MasterTable;
            DataTable DetailTable;
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            DataTable Attachment = ds.Tables["attachments"];
            SortedList Params = new SortedList();
            SortedList Result = new SortedList();            
            // Auto Gen
            string InvoiceNo = "";
            DataRow masterRow = MasterTable.Rows[0];
            var values = masterRow["X_DebitNoteNo"].ToString();
            int UserID = myFunctions.GetUserID(User);
            int N_CompanyID = myFunctions.GetCompanyID(User);
            int N_InvoiceId = 0;
            int nFnYearID = myFunctions.getIntVAL(masterRow["N_fnYearId"].ToString());;

            int N_DebitNoteId = myFunctions.getIntVAL(masterRow["N_DebitNoteId"].ToString());
            int N_CustomerID = myFunctions.getIntVAL(masterRow["n_CustomerID"].ToString());
            int N_SalesId = myFunctions.getIntVAL(masterRow["N_SalesId"].ToString());
            int N_DeliveryNote = 0;
            double N_TotalPaid = myFunctions.getVAL(MasterTable.Rows[0]["N_TotalPaidAmount"].ToString());
            MasterTable.Rows[0]["N_TotalPaidAmount"] = N_TotalPaid;
            double N_TotalPaidF = myFunctions.getVAL(MasterTable.Rows[0]["n_TotalPaidAmountF"].ToString());
            MasterTable.Rows[0]["n_TotalPaidAmountF"] = N_TotalPaidF;
            string xButtonAction="";

            if (!myFunctions.CheckActiveYearTransaction(N_CompanyID, nFnYearID, DateTime.ParseExact(MasterTable.Rows[0]["D_ReturnDate"].ToString(), "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture), dLayer, connection, transaction))
            {
                object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID="+N_CompanyID+" and convert(date ,'" + MasterTable.Rows[0]["D_ReturnDate"].ToString() + "') between D_Start and D_End", connection, transaction);
                if (DiffFnYearID != null)
                {
                    MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
                    nFnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());
                    //QueryParams["@nFnYearID"] = nFnYearID;
                }
                else
                {
                    // transaction.Rollback();
                    // return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
                    Result.Add("b_IsCompleted", 0);
                    Result.Add("x_Msg", "Transaction date must be in the active Financial Year.");
                    return Result;
                }
            }

            if (values == "@Auto")
            {
                Params.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
                Params.Add("N_YearID", nFnYearID);
                Params.Add("N_FormID", 55);
                Params.Add("N_BranchID", masterRow["n_BranchId"].ToString());
                InvoiceNo = dLayer.GetAutoNumber("Inv_SalesReturnMaster", "X_DebitNoteNo", Params, connection, transaction);
                  xButtonAction="Insert"; 
                if (InvoiceNo == "") 
                {
                     //transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Return Number")); 
                    Result.Add("b_IsCompleted", 0);
                    Result.Add("x_Msg", "Unable to generate Return Number");
                    return Result;
                }
                MasterTable.Rows[0]["X_DebitNoteNo"] = InvoiceNo;
            }
           
             InvoiceNo = MasterTable.Rows[0]["X_DebitNoteNo"].ToString();

            if (N_DebitNoteId > 0)
            {
                SortedList StockUpdateParams = new SortedList(){
                                {"N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString()},
	                            {"N_TransID",N_DebitNoteId},
	                            {"X_TransType", "SALES RETURN"}};

                dLayer.ExecuteNonQueryPro("SP_StockDeleteUpdate", StockUpdateParams, connection, transaction);
               xButtonAction="Update"; 
                SortedList DeleteParams = new SortedList(){
                        {"N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString()},
                        {"X_TransType","SALES RETURN"},
                        {"N_VoucherID",N_DebitNoteId}};
                try
                {
                    dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);


                    for (int j = 1; j < DetailTable.Rows.Count; j++)
                    {
                        dLayer.ExecuteNonQuery("Update Inv_StockMaster_IMEI Set N_Status = 1 Where N_IMEI='" + DetailTable.Rows[j]["N_IMEI"] + "' and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_Status=0", connection, transaction);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // return Ok(_api.Error(User, ex));
                    // Result.Add("b_IsCompleted", 0);
                    // Result.Add("x_Msg", ex);
                    // return Result;
                    throw ex;
                }
                // string sqlCommandText = "";
                // SortedList DeleteParams = new SortedList();

                // sqlCommandText = "SP_Delete_Trans_With_SaleAccounts  @N_CompanyId,'SALES RETURN',@N_DebitNoteId";
                // DeleteParams.Add("@N_CompanyId",myFunctions.getIntVAL(masterRow["n_CompanyId"].ToString()));
                // DeleteParams.Add("@N_DebitNoteId", N_DebitNoteId);

                // dLayer.ExecuteDataTable(sqlCommandText, DeleteParams, connection);
                
            }
               
            // dLayer.setTransaction();
           
            MasterTable.Columns.Remove("n_ProjectID");
            N_InvoiceId = dLayer.SaveData("Inv_SalesReturnMaster", "N_DebitNoteId", MasterTable, connection, transaction);
            if (N_InvoiceId <= 0)
            {
                //transaction.Rollback();
                Result.Add("b_IsCompleted", 0);
                Result.Add("x_Msg", "Unable to save Sales return");
                return Result;
            }
            for (int j = 0; j < DetailTable.Rows.Count; j++)
            {
                DetailTable.Rows[j]["N_DebitNoteId"] = N_InvoiceId;
            }
            int N_InvoiceDetailId = dLayer.SaveData("Inv_SalesReturnDetails", "N_DebitnoteDetailsID", DetailTable, connection, transaction);

          

            SortedList CustomerParams = new SortedList();
            CustomerParams.Add("@nCustomerID", N_CustomerID);
            DataTable CustomerInfo = dLayer.ExecuteDataTable("Select X_CustomerCode,X_CustomerName from Inv_Customer where N_CustomerID=@nCustomerID", CustomerParams, connection, transaction);
       
            if (CustomerInfo.Rows.Count > 0)
            {
                try
                {
                    myAttachments.SaveAttachment(dLayer, Attachment, InvoiceNo, N_DebitNoteId, CustomerInfo.Rows[0]["X_CustomerName"].ToString().Trim(), CustomerInfo.Rows[0]["X_CustomerCode"].ToString(), N_CustomerID, "Customer Document", User, connection, transaction);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // return Ok(_api.Error(User, ex));
                    // Result.Add("b_IsCompleted", 0);
                    // Result.Add("x_Msg", ex);
                    // return Result;
                    throw ex;
                }
            }

       
           

            SortedList InsParams = new SortedList();
            InsParams.Add("N_CompanyID", N_CompanyID);
            InsParams.Add("N_DebitNoteId", N_InvoiceId);
            InsParams.Add("N_DeliveryNote", N_DeliveryNote);

            dLayer.ExecuteNonQueryPro("SP_SalesReturn_Ins_New", InsParams, connection, transaction);

             myFunctions.LogScreenActivitys(nFnYearID,N_InvoiceId,InvoiceNo,55,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
           

            SortedList StockPostingParams = new SortedList();
            StockPostingParams.Add("N_CompanyID", N_CompanyID);
            StockPostingParams.Add("X_InventoryMode", "SALES RETURN");
            StockPostingParams.Add("N_InternalID", N_InvoiceId);
            StockPostingParams.Add("N_UserID", UserID);

            dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Sales_Posting", StockPostingParams, connection, transaction);

            SortedList StockOutParam = new SortedList();
            StockOutParam.Add("N_CompanyID", N_CompanyID);

            dLayer.ExecuteNonQueryPro("SP_StockOutUpdate", StockOutParam, connection, transaction);

            
          

            //transaction.Commit();

            Result.Add("n_SalesReturnID", N_InvoiceId);
            Result.Add("x_SalesReturnNo", InvoiceNo);
            // return Ok(_api.Success(Result, "Sales Return Saved"));
            Result.Add("b_IsCompleted", 1);
            Result.Add("x_Msg", "Sales Return Saved");
            return Result;
        }

        public SortedList PurchaseReturnSaveData(DataSet ds,string ipAddress, ClaimsPrincipal User,IDataAccessLayer dLayer,SqlConnection connection, SqlTransaction transaction)
        {

            DataTable MasterTable;
            DataTable DetailTable;
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            DataRow masterRow = MasterTable.Rows[0];
            SortedList Params = new SortedList();
            SortedList Result = new SortedList();

            string ReturnNo = "";
            int N_CreditNoteID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CreditNoteId"].ToString());
            int N_UserID = myFunctions.GetUserID(User);
            double N_TotalReceived = myFunctions.getVAL(MasterTable.Rows[0]["n_TotalReceived"].ToString());
            MasterTable.Rows[0]["n_TotalReceived"] = N_TotalReceived;
            double N_TotalReceivedF = myFunctions.getVAL(MasterTable.Rows[0]["n_TotalReceivedF"].ToString());
            MasterTable.Rows[0]["n_TotalReceivedF"] = N_TotalReceivedF;
            var values = MasterTable.Rows[0]["X_CreditNoteNo"].ToString();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int N_VendorID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_VendorID"].ToString());
             int nFnYearID = myFunctions.getIntVAL(masterRow["N_fnYearId"].ToString());;
            string xButtonAction="";

            if (!myFunctions.CheckActiveYearTransaction(myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString()), myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString()), Convert.ToDateTime(MasterTable.Rows[0]["D_RetDate"].ToString()), dLayer, connection, transaction))
            {
                object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID=" + myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString()) + " and convert(date ,'" + MasterTable.Rows[0]["D_RetDate"].ToString() + "') between D_Start and D_End", Params, connection, transaction);
                if (DiffFnYearID != null)
                {
                    MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
          
                }
                else
                {
                    // transaction.Rollback();
                    // return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
                    Result.Add("b_IsCompleted", 0);
                    Result.Add("x_Msg", "Transaction date must be in the active Financial Year.");
                    return Result;
                }
            }

            if (values == "@Auto")
            {
                Params.Add("N_CompanyID", MasterTable.Rows[0]["n_CompanyId"].ToString());
                Params.Add("N_YearID", MasterTable.Rows[0]["n_FnYearId"].ToString());
                Params.Add("N_FormID", 80);
                Params.Add("N_BranchID", MasterTable.Rows[0]["n_BranchId"].ToString());
                ReturnNo = dLayer.GetAutoNumber("Inv_PurchaseReturnMaster", "X_CreditNoteNo", Params, connection, transaction);
                xButtonAction="Insert"; 
                if (ReturnNo == "") 
                { 
                    // transaction.Rollback();
                    // return Ok(_api.Warning("Unable to generate Quotation Number")); 
                    Result.Add("b_IsCompleted", 0);
                    Result.Add("x_Msg", "Unable to generate Purchase Return Number");
                    return Result;
                }
                MasterTable.Rows[0]["X_CreditNoteNo"] = ReturnNo;
            }
  
               ReturnNo = MasterTable.Rows[0]["X_CreditNoteNo"].ToString();
       
            if (N_CreditNoteID > 0)
            {
                SortedList DeleteParams = new SortedList(){
                        {"N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString()},
                        {"X_TransType","PURCHASE RETURN"},
                        {"N_VoucherID",N_CreditNoteID}};
                try
                {
                    dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", DeleteParams, connection, transaction);
                           xButtonAction="Update"; 
                }
                catch (Exception ex)
                {
                    // transaction.Rollback();
                    // return Ok(_api.Error(User, ex));
                    transaction.Rollback();
                    throw ex;
                }
            }

            N_CreditNoteID = dLayer.SaveData("Inv_PurchaseReturnMaster", "N_CreditNoteID", MasterTable, connection, transaction);
            if (N_CreditNoteID <= 0)
            {
                //transaction.Rollback();
                Result.Add("b_IsCompleted", 0);
                Result.Add("x_Msg", "Unable to save Purchase return");
                return Result;
            }


            if(!DetailTable.Columns.Contains("N_QtyDisplay"))
            DetailTable= myFunctions.AddNewColumnToDataTable(DetailTable,"N_QtyDisplay",typeof(double),0);
            for (int j = 0; j < DetailTable.Rows.Count; j++)
            {
                DetailTable.Rows[j]["N_CreditNoteID"] = N_CreditNoteID;
                // DetailTable.Rows[j]["n_RetQty"] = (myFunctions.getVAL(DetailTable.Rows[j]["n_RetQty"].ToString())) * (myFunctions.getVAL(DetailTable.Rows[j]["N_UnitQty"].ToString()));
                // DetailTable.Rows[j]["N_QtyDisplay"] = DetailTable.Rows[j]["n_RetQty"];
            }
            if(DetailTable.Columns.Contains("N_UnitQty"))
            DetailTable.Columns.Remove("N_UnitQty");

            int N_QuotationDetailId = dLayer.SaveData("Inv_PurchaseReturnDetails", "n_CreditNoteDetailsID", DetailTable, connection, transaction);
               //Activity Log
                // string ipAddress = "";
                // if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                //     ipAddress = Request.Headers["X-Forwarded-For"];
                // else
                //     ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                      myFunctions.LogScreenActivitys(nFnYearID,N_CreditNoteID,ReturnNo,68,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);





            try
            { 
                SortedList InsParams = new SortedList(){ 
                            {"N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString()},
                            {"N_CreditNoteID",N_CreditNoteID}};
                dLayer.ExecuteNonQueryPro("[SP_PurchaseReturn_Ins]", InsParams, connection, transaction);

                SortedList PostParams = new SortedList(){
                            {"N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString()},
                            {"X_InventoryMode","PURCHASE RETURN"},
                            {"N_InternalID",N_CreditNoteID},
                            {"N_UserID",N_UserID}};
                dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Purchase_Posting", PostParams, connection, transaction);
            }
            catch (Exception ex)
            {
                Result.Add("b_IsCompleted", 0);
                if (ex.Message.Contains("50"))
                    Result.Add("x_Msg", "Day Closed");
                else if (ex.Message.Contains("51"))
                    Result.Add("x_Msg", "Year Closed");
                else if (ex.Message.Contains("52"))
                    Result.Add("x_Msg", "Year Exists");
                else if (ex.Message.Contains("53"))
                    Result.Add("x_Msg", "Period Closed");
                else if (ex.Message.Contains("54"))
                    Result.Add("x_Msg", "Wrong Txn Date");
                else if (ex.Message.Contains("55"))
                    Result.Add("x_Msg", "Transaction Started");
                else
                {
                    transaction.Rollback();
                    throw ex;
                }
                return Result;
            }

            Result.Add("n_PurchaseReturnID", N_CreditNoteID);
            Result.Add("x_PurchaseReturnNo", ReturnNo);
            // transaction.Commit();
            // return Ok(_api.Success(Result, "Purchase Return Saved"));

            Result.Add("b_IsCompleted", 1);
            Result.Add("x_Msg", "Purchase Return Saved");
            return Result;

        }
    }
    

    public interface ITxnHandler
    {
        public SortedList PurchaseSaveData( DataSet ds,string ipAddress, ClaimsPrincipal User,IDataAccessLayer dLayer,SqlConnection connection, SqlTransaction transaction);
        public SortedList SalesSaveData(DataSet ds ,string ipAddress, ClaimsPrincipal User,IDataAccessLayer dLayer,SqlConnection connection, SqlTransaction transaction);
        public SortedList SalesReturnSaveData(DataSet ds,string ipAddress, ClaimsPrincipal User,IDataAccessLayer dLayer,SqlConnection connection, SqlTransaction transaction);
        public SortedList PurchaseReturnSaveData(DataSet ds,string ipAddress, ClaimsPrincipal User,IDataAccessLayer dLayer,SqlConnection connection, SqlTransaction transaction);
    }
}