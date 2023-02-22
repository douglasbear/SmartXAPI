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
    [Route("vendorpayment")]
    [ApiController]
    public class Inv_VendorPayment : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly int N_FormID;


        public Inv_VendorPayment(IDataAccessLayer dl, IApiFunctions apiFun, IMyFunctions myFun, IConfiguration conf,IMyAttachments myAtt)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 67;
        }


        [HttpGet("list")]
        public ActionResult GetVendorPayment(int? nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,bool bAllBranchData,int nBranchID)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";
                    int nCompanyID = myFunctions.GetCompanyID(User);

                    string UserPattern = myFunctions.GetUserPattern(User);
                    int nUserID = myFunctions.GetUserID(User);
                    string Pattern = "";

                    if (UserPattern != "")
                     {
                    Pattern = " and Left(X_Pattern,Len(@UserPattern))=@UserPattern ";
                    Params.Add("@UserPattern", UserPattern);
                     }  
                    //  else
                    //        {
                    // object HierarchyCount = dLayer.ExecuteScalar("select count(N_HierarchyID) from Sec_UserHierarchy where N_CompanyID="+nCompanyID, Params, connection);


                    // if( myFunctions.getIntVAL(HierarchyCount.ToString())>0)
                    // Pattern = " and N_CreatedUser=" + nUserID;
                    // }

                    // if( myFunctions.getIntVAL(HierarchyCount.ToString())>0)
                    // Pattern = " and N_UserID=" + nUserID;
                    // }

                    // if( myFunctions.getIntVAL(HierarchyCount.ToString())>0)
                    // Pattern = " and N_CreatedUser=" + nUserID;
                    // }




                    int N_decimalPlace = 2;
                    N_decimalPlace = myFunctions.getIntVAL(myFunctions.ReturnSettings("Purchase", "Decimal_Place", "N_Value", nCompanyID, dLayer, connection));
                    N_decimalPlace = N_decimalPlace == 0 ? 2 : N_decimalPlace;

                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = " and (Memo like '%" + xSearchkey + "%' or [Vendor Name] like '%" + xSearchkey + "%' or Cast(Date as VarChar) like '%" + xSearchkey + "%' or X_Notes like '%" + xSearchkey + "%' or Amount like '%" + xSearchkey + "%' or x_ProjectCode like '%" + xSearchkey + "%'  or X_ProjectName like '%" + xSearchkey + "%')";

                   if (bAllBranchData == true)
                        {
                            Searchkey = Searchkey + " ";
                        }
                        else
                        {
                            Searchkey = Searchkey + " and N_BranchID=" + nBranchID + " ";
                        }
                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_PayReceiptId desc";
                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "vendorName":
                                xSortBy = "[Vendor Name] " + xSortBy.Split(" ")[1];
                                break;
                            case "receiptNo":
                                xSortBy = "N_PayReceiptId " + xSortBy.Split(" ")[1];
                                break;
                            case "date":
                                xSortBy = "Cast([Date] as DateTime ) " + xSortBy.Split(" ")[1];
                                break;
                            case "amount":
                                xSortBy = "Cast(REPLACE(Amount,',','') as Numeric(20," + N_decimalPlace + ")) " + xSortBy.Split(" ")[1];
                                break;
                            case "x_ProjectName":
                               xSortBy = "[x_ProjectName] " + xSortBy.Split(" ")[1];
                                break;
                            case "x_ProjectCode":
                               xSortBy = "[x_ProjectCode] " + xSortBy.Split(" ")[1];
                                break;
                            default: break;
                        }

                        xSortBy = " order by " + xSortBy;
                    }


                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvPayment_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID <> 91 and  (X_type='PP' OR X_type='PA') and amount is not null " + Pattern + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvPayment_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID <> 91 and (X_type='PP' OR X_type='PA') and amount is not null " + Pattern + Searchkey + " and n_PayReceiptID not in (select top(" + Count + ") n_PayReceiptID from vw_InvPayment_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and B_YearEndProcess=0  and amount is not null " + xSortBy + " ) " + xSortBy;
                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearId);
                    SortedList OutPut = new SortedList();


                    // connection.Open();
                    // dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    // sqlCommandCount = "select count(1) as N_Count  from vw_InvPayment_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and B_YearEndProcess=0  and amount is not null ";
                    // object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    // OutPut.Add("Details",api.Format(dt));
                    // OutPut.Add("TotalCount",TotalCount);


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count,sum(Cast(REPLACE(Amount,',','') as Numeric(20," + N_decimalPlace + ")) ) as TotalAmount from vw_InvPayment_Search where N_CompanyID=@p1  and N_FormID <> 91 and  N_FnYearID=@p2 and (X_type='PP' OR X_type='PA') and amount is not null " + Pattern + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    string TotalSum = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                        TotalSum = drow["TotalAmount"].ToString();
                    }
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    OutPut.Add("TotalSum", TotalSum);

                    // dt = api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Success(OutPut));
                        // return Ok(api.Warning("No Results Found"));

                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(403, api.Error(User, e));
            }
        }

        // [HttpGet("defaults")]
        // public ActionResult GetScreenDefaults(int nFnYearId)
        // {
        //     DataTable dt = new DataTable();
        //     SortedList Params = new SortedList();

        //     int nCompanyId =myFunctions.GetCompanyID(User);
        //     string sqlCommandText ="";
        //     string sqlCommandCount="";

        //     Params.Add("@p1", nCompanyId);
        //     Params.Add("@p2", nFnYearId);
        //     SortedList OutPut = new SortedList();

        //     txtPaymentType.Text = dLayer.ExecuteScalar("Select X_PayMethod,N_PaymentMethodID,N_TypeID,B_IsCheque from Acc_PaymentMethodMaster where B_IsDefault=1 and N_CompanyID=@nCompanyID");
        //     txtDefaultAccount.Text = GetDefaultAccount(txtPaymentType.Text.Trim(), N_BehID);
        //     txtDefaultAccName.Text = GetDefaultAccountName(txtPaymentType.Text.Trim(), N_BehID);
        //     N_DefAccountLedgerID = myFunctions.getIntVAL(dba.ExecuteSclar("Select N_LedgerID from Acc_MastLedger where X_LedgerCode='" + txtDefaultAccount.Text  + "' and N_CompanyID=" + myCompanyID._CompanyID + "and N_FnYearID="+ myCompanyID._FnYearID , "TEXT", new DataTable()).ToString());


        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
        //             sqlCommandCount = "select count(1) as N_Count  from vw_InvPayment_Search where N_CompanyID=@p1 and N_FnYearID=@p2";
        //             object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
        //             OutPut.Add("Details",api.Format(dt));
        //             OutPut.Add("TotalCount",TotalCount);
        //         }
        //         // dt = api.Format(dt);
        //         if (dt.Rows.Count == 0)
        //         {
        //             return Ok(api.Warning("No Results Found"));
        //         }
        //         else
        //         {
        //             return Ok(api.Success(OutPut));
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         return StatusCode(403, api.Error(User,e));
        //     }
        // }




        [HttpGet("payDetails")]
        public ActionResult GetVendorPayDetails(int nVendorID, int nFnYearId, string dTransDate, int nBranchID, bool bShowAllbranch, string xInvoiceNo, string xTransType)
        {
                if (xInvoiceNo != null)
                xInvoiceNo = xInvoiceNo.Replace("%2F", "/");
                
            SortedList OutPut = new SortedList();
            DataTable PayReceipt = new DataTable();
            DataTable PayInfo = new DataTable();
            DataTable Attachments = new DataTable();
            string sql = "";
            int AllBranch = 1;
            int nPayReceiptID = 0;
            int nCompanyId = myFunctions.GetCompanyID(User);
            OutPut.Add("totalAmtDue", 0);
            OutPut.Add("totalBalance", 0);
            OutPut.Add("advanceAmount", 0);
            OutPut.Add("txnStarted", false);
            object decimalobj=null;
            if (bShowAllbranch == true)
            {
                AllBranch = 0;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //SqlTransaction transaction = connection.BeginTransaction();
                    if (bShowAllbranch == true)
                        sql = "SELECT  -1 * ISNULL( Sum(n_Amount),0)  as N_BalanceAmount from  vw_InvVendorStatement Where N_AccType=1 and isnull(N_PaymentMethod,0)<>1 and N_AccID=@nVendorID and N_CompanyID=@nCompanyID and  D_TransDate<=@dTransDate";
                    else
                        sql = "SELECT  -1 * ISNULL( Sum(n_Amount),0)  as N_BalanceAmount from  vw_InvVendorStatement Where N_AccType=1 and isnull(N_PaymentMethod,0)<>1 and N_AccID=@nVendorID and N_CompanyID=@nCompanyID and N_BranchId=@nBranchID and  D_TransDate<=@dTransDate";

                    if (xInvoiceNo != null && xInvoiceNo.ToString() != "")
                    {
                        SortedList proParams1 = new SortedList(){
                                {"N_CompanyID",nCompanyId},
                                {"X_VoucherNo",xInvoiceNo},
                                {"N_FnYearID",nFnYearId},
                                {"N_BranchID",bShowAllbranch?0:nBranchID}};
                        PayInfo = dLayer.ExecuteDataTablePro("SP_Inv_InvPayReceipt_Disp", proParams1, connection);
                        if (PayInfo.Rows.Count > 0)
                        {
                            nPayReceiptID = myFunctions.getIntVAL(PayInfo.Rows[0]["N_PayReceiptId"].ToString());
                            xTransType = PayInfo.Rows[0]["X_Type"].ToString();
                            nVendorID = myFunctions.getIntVAL(PayInfo.Rows[0]["N_PartyID"].ToString());
                            int nCurrencyID = myFunctions.getIntVAL(PayInfo.Rows[0]["N_CurrencyID"].ToString());
                            string X_CurrencyName = dLayer.ExecuteScalar("select X_CurrencyName from Acc_CurrencyMaster where N_CompanyID="+nCompanyId+" and N_CurrencyID="+nCurrencyID, connection).ToString();
                            int N_CurrencyDecimal = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_Decimal from Acc_CurrencyMaster where N_CompanyID="+nCompanyId+" and N_CurrencyID="+nCurrencyID, connection).ToString());
                            myFunctions.AddNewColumnToDataTable(PayInfo, "X_CurrencyName", typeof(string), X_CurrencyName);
                            myFunctions.AddNewColumnToDataTable(PayInfo, "N_Decimal", typeof(int), N_CurrencyDecimal);
                            dTransDate = myFunctions.getDateVAL(Convert.ToDateTime(PayInfo.Rows[0]["D_Date"].ToString()));
                        }
                    }

                    SortedList paramList = new SortedList();
                    paramList.Add("@nVendorID", nVendorID);
                    paramList.Add("@dTransDate", dTransDate);
                    paramList.Add("@nBranchID", nBranchID);
                    paramList.Add("@nPayReceiptID", nPayReceiptID);
                    paramList.Add("@xTransType", xTransType);
                    paramList.Add("@nCompanyID", nCompanyId);
                    DataTable VendorBalance = dLayer.ExecuteDataTable(sql, paramList, connection);

                    if (VendorBalance.Rows.Count > 0)
                    {
                        OutPut["totalAmtDue"] = myFunctions.getVAL(VendorBalance.Rows[0]["N_BalanceAmount"].ToString());
                        if (myFunctions.getVAL(VendorBalance.Rows[0]["N_BalanceAmount"].ToString()) < 0)
                            OutPut["totalBalance"] = Convert.ToDouble(-1 * myFunctions.getVAL(VendorBalance.Rows[0]["N_BalanceAmount"].ToString()));
                        else if (myFunctions.getVAL(VendorBalance.Rows[0]["N_BalanceAmount"].ToString()) > 0)
                            OutPut["totalBalance"] = myFunctions.getVAL(VendorBalance.Rows[0]["N_BalanceAmount"].ToString());
                        else
                            OutPut["totalBalance"] = 0;
                    }

                    SortedList proParams2 = new SortedList(){
                                {"N_CompanyID",nCompanyId},
                                {"N_FnYearID",nFnYearId},
                                {"N_CustomerID",nVendorID},
                                {"N_PayReceiptId",nPayReceiptID},
                                {"D_InvoiceDate",dTransDate},
                                {"BranchFlag",AllBranch},
                                {"BranchId",nBranchID}};


                    if (nPayReceiptID > 0)
                    {

                        if (xTransType == "PA")
                        {
                            PayReceipt = dLayer.ExecuteDataTablePro("SP_Inv_InvPayReceipt_View", proParams2, connection);
                            if (PayReceipt.Rows.Count > 0)
                            {
                                object obj = dLayer.ExecuteScalar("Select isnull(Count(N_InventoryId),0) as CountExists from Inv_PayReceiptDetails where N_CompanyID=@nCompanyID and N_InventoryId<>N_PayReceiptId and N_InventoryId=@nPayReceiptID and X_TransType =@xTransType", paramList, connection);
                                if (obj != null)
                                {
                                    if (myFunctions.getIntVAL(obj.ToString()) > 0)
                                    {
                                        OutPut["txnStarted"] = true;
                                    }
                                }
                                // return Ok(api.Success(api.Format(PayReceipt,"details")));

                            }
                        }
                        else
                        {
                            PayReceipt = dLayer.ExecuteDataTablePro("SP_Inv_InvPayReceipt_View", proParams2, connection);
                        }
                    }
                    else
                    {
                        PayReceipt = dLayer.ExecuteDataTablePro("SP_Inv_InvPayReceipt_View", proParams2, connection);
                    }
                    if(PayInfo.Rows.Count>0)
                      Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(PayInfo.Rows[0]["N_PayReceiptId"].ToString()), myFunctions.getIntVAL(PayInfo.Rows[0]["N_PayReceiptId"].ToString()), 67, 0, User, connection);
                    //Attachments = api.Format(Attachments, "attachments");
                    decimalobj = dLayer.ExecuteScalar("Select isnull(N_Decimal, 0)  from Acc_Company where N_CompanyID=@nCompanyID ", paramList, connection);
                }

                PayReceipt = myFunctions.AddNewColumnToDataTable(PayReceipt, "n_DueAmount", typeof(double), 0);

                       

                if (PayReceipt.Rows.Count > 0)
                {
                    double N_ListedAmtTotal = 0;
                    foreach (DataRow dr in PayReceipt.Rows)
                    {
                        if (PayReceipt.Columns.Contains("n_PayReceiptID"))
                        {
                            object payrcptid = dr["N_PayReceiptId"].ToString();
                            if (payrcptid != null)
                            {
                                if (payrcptid.ToString() == nPayReceiptID.ToString() && dr["x_Type"].ToString() == "PA")
                                {
                                    OutPut["advanceAmount"] = myFunctions.getVAL(dr["n_Amount"].ToString());
                                }
                            }
                        }
                        double N_InvoiceDueAmt = myFunctions.getVAL(dr["N_Amount"].ToString()) + myFunctions.getVAL(dr["N_BalanceAmount"].ToString()) + myFunctions.getVAL(dr["N_DiscountAmt"].ToString());// +myFunctions.getVAL(dr["N_DiscountAmt"].ToString());
                        N_ListedAmtTotal += N_InvoiceDueAmt;
                        if (N_InvoiceDueAmt == 0) { dr.Delete(); continue; }
                        if (nPayReceiptID > 0 && (myFunctions.getVAL(dr["N_DiscountAmt"].ToString()) == 0 && myFunctions.getVAL(dr["N_Amount"].ToString()) == 0)) { dr.Delete(); continue; }

                        if (myFunctions.getIntVAL(decimalobj.ToString()) > 0)
                            dr["n_DueAmount"] = N_InvoiceDueAmt.ToString(myFunctions.decimalPlaceString(myFunctions.getIntVAL(decimalobj.ToString())));
                        else 
                            dr["n_DueAmount"] = N_InvoiceDueAmt.ToString(myFunctions.decimalPlaceString(2));
                    }
                }
                PayReceipt.AcceptChanges();
                
                return Ok(api.Success(new SortedList() { 
              { "details", api.Format(PayReceipt) }, 
                { "masterData", OutPut },
                 { "attachments", api.Format(Attachments) },
                 { "master", PayInfo } }));
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                DataTable Attachment = ds.Tables["attachments"];

                DataTable Approvals;
                Approvals = ds.Tables["approval"];
                DataRow ApprovalRow = Approvals.Rows[0];

                SortedList Params = new SortedList();
                int n_PayReceiptID = 0;
                string PayReceiptNo = "";
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                String xButtonAction="";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();

                    // Auto Gen

                    if (MasterTable.Rows.Count > 0)
                    {

                    }
                    var x_VoucherNo = MasterTable.Rows[0]["x_VoucherNo"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    double nAmount = 0, nAmountF = 0; string xDesc = "";
                    int nCompanyId = myFunctions.getIntVAL(Master["n_CompanyId"].ToString());
                    n_PayReceiptID = myFunctions.getIntVAL(Master["n_PayReceiptID"].ToString());
                    string x_Type = MasterTable.Rows[0]["x_Type"].ToString();
                    nAmount = myFunctions.getVAL(Master["n_Amount"].ToString());
                    nAmountF = myFunctions.getVAL(Master["n_AmountF"].ToString());
                    int n_PartyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PartyID"].ToString());
                    int N_SaveDraft = myFunctions.getIntVAL(Master["b_IsSaveDraft"].ToString());
                    int nUserID = myFunctions.GetUserID(User);
                    int N_NextApproverID=0;
                   
                     if (!myFunctions.CheckActiveYearTransaction(nCompanyId, nFnYearID, Convert.ToDateTime(MasterTable.Rows[0]["D_Date"].ToString()), dLayer, connection, transaction))
                    {
                        object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID=" + nCompanyId + " and convert(date ,'" + MasterTable.Rows[0]["D_Date"].ToString() + "') between D_Start and D_End", Params, connection, transaction);
                        if (DiffFnYearID != null)
                        {
                            MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
                            nFnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());
                        }
                        else
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User, "Transaction date must be in the active Financial Year."));
                        }
                    }

                    if (MasterTable.Columns.Contains("x_Desc"))
                    {
                        xDesc = Master["x_Desc"].ToString();
                    }
                    if (MasterTable.Columns.Contains("n_Amount"))
                        MasterTable.Columns.Remove("n_Amount");
                    if (MasterTable.Columns.Contains("n_AmountF"))
                        MasterTable.Columns.Remove("n_AmountF");
                    if (MasterTable.Columns.Contains("x_Desc"))
                        MasterTable.Columns.Remove("x_Desc");

                    SortedList VendParams = new SortedList();
                    VendParams.Add("@nCompanyID", nCompanyId);
                    VendParams.Add("@n_PartyID", n_PartyID);
                    VendParams.Add("@nFnYearID", nFnYearID);
                    object objVendName = dLayer.ExecuteScalar("Select X_VendorName From Inv_Vendor where N_VendorID=@n_PartyID and N_CompanyID=@nCompanyID  and N_FnYearID=@nFnYearID", VendParams, connection, transaction);                   

                    if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()) && n_PayReceiptID > 0)
                    {
                        int N_PkeyID = n_PayReceiptID;
                        string X_Criteria = "N_PayReceiptId=" + n_PayReceiptID + " and N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearID;
                        myFunctions.UpdateApproverEntry(Approvals, "Inv_PayReceipt", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        N_NextApproverID = myFunctions.LogApprovals(Approvals,myFunctions.getIntVAL(nFnYearID.ToString()), "PURCHASE PAYMENT", N_PkeyID, x_VoucherNo, 1, "", 0, "",0, User, dLayer, connection, transaction);

                        myAttachments.SaveAttachment(dLayer, Attachment,PayReceiptNo, n_PayReceiptID,objVendName.ToString().Trim(),PayReceiptNo, n_PayReceiptID, "VendorPayment Document", User, connection, transaction);

                        N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_PayReceipt where N_PayReceiptId=" + n_PayReceiptID + " and N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearID, connection, transaction).ToString());
                        if (N_SaveDraft == 0)
                        {
                            SortedList PostingParams = new SortedList();
                            PostingParams.Add("N_CompanyID", nCompanyId);
                            PostingParams.Add("X_InventoryMode", x_Type);
                            PostingParams.Add("N_InternalID", n_PayReceiptID);
                            PostingParams.Add("N_UserID", myFunctions.GetUserID(User));
                            PostingParams.Add("X_SystemName", "ERP Cloud");
                            object posting = dLayer.ExecuteScalarPro("SP_Acc_InventoryPosting", PostingParams, connection, transaction);

                        }

                        //myFunctions.SendApprovalMail(N_NextApproverID, this.N_FormID, N_PkeyID, "PURCHASE PAYMENT", x_VoucherNo, dLayer, connection, transaction, User);
                        transaction.Commit();
                        return Ok(api.Success("Vendor Payment Approved " + "-" + x_VoucherNo));
                    }


                    if (x_VoucherNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyId);
                        Params.Add("N_YearID", Master["n_FnYearID"].ToString());
                        Params.Add("N_FormID", this.N_FormID);
                        // Params.Add("N_BranchID", Master["n_BranchID"].ToString());

                        PayReceiptNo = dLayer.GetAutoNumber("Inv_PayReceipt", "x_VoucherNo", Params, connection, transaction);
                       xButtonAction="Insert"; 
                        if (PayReceiptNo == "") { transaction.Rollback(); return Ok(api.Warning("Unable to generate Receipt Number")); }
                        MasterTable.Rows[0]["x_VoucherNo"] = PayReceiptNo;
                    }
                    else

                    
                    {
                        PayReceiptNo = MasterTable.Rows[0]["PayReceiptNo"].ToString();

                        if (n_PayReceiptID > 0)
                        {

                            SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyId},
                                {"X_TransType",x_Type},
                                {"N_VoucherID",n_PayReceiptID}};
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                             xButtonAction="Update";

                            // if (myFunctions.CheckPermission(nCompanyId, 576, "Administrator", dLayer, connection, transaction))
                            // {
                            //     dLayer.DeleteData("Inv_PurchasePaymentStatus", "N_PaymentID",)
                            //     dba.DeleteDataNoTry("Inv_PurchasePaymentStatus", "N_PaymentID", PayReceiptId_Loc.ToString(), "N_CompanyID = " + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID);
                            //     dba.DeleteDataNoTry("Inv_PaymentDetails", "N_PaymentID", PayReceiptId_Loc.ToString(), "N_CompanyID = " + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID);

                            // }

                            //          if (B_PaymentDetails)
                            // {

                            //     dLayer.DeleteData("Inv_PurchasePaymentStatus", "N_PaymentID", n_PayReceiptID, "N_CompanyID = " + nCompanyId + " and N_FnYearID=" + nFnYearID,connection,transaction);
                            //     dLayer.DeleteData("Inv_PaymentDetails", "N_PaymentID", n_PayReceiptID, "N_CompanyID = " + nCompanyId + " and N_FnYearID=" + nFnYearID,connection,transaction);

                            // }



                        }
                    }

            

                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);

                    n_PayReceiptID = dLayer.SaveData("Inv_PayReceipt", "n_PayReceiptID", MasterTable, connection, transaction);
                    if (n_PayReceiptID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Error"));
                    }

                    N_NextApproverID = myFunctions.LogApprovals(Approvals,myFunctions.getIntVAL(nFnYearID.ToString()), "PURCHASE PAYMENT", n_PayReceiptID, PayReceiptNo, 1, "", 0, "",0, User, dLayer, connection, transaction);
                    N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_PayReceipt where N_PayReceiptId=" + n_PayReceiptID + " and N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearID, connection, transaction).ToString());

                    if (x_Type == "PA")
                    {


                        DetailTable.Clear();

                        DataRow row = DetailTable.NewRow();
                        if (DetailTable.Columns.Contains("N_CompanyID"))
                            row["N_CompanyID"] = myFunctions.getIntVAL(Master["n_CompanyID"].ToString());
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_CompanyID", typeof(int), myFunctions.getIntVAL(Master["n_CompanyID"].ToString()));
                        if (DetailTable.Columns.Contains("N_PayReceiptId"))
                            row["N_PayReceiptId"] = n_PayReceiptID;
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_PayReceiptId", typeof(int), n_PayReceiptID);
                        if (DetailTable.Columns.Contains("N_InventoryId"))
                            row["N_InventoryId"] = n_PayReceiptID;
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_InventoryId", typeof(int), n_PayReceiptID);
                        if (DetailTable.Columns.Contains("N_DiscountAmt"))
                            row["N_DiscountAmt"] = 0;
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_DiscountAmt", typeof(int), 0);
                        if (DetailTable.Columns.Contains("N_DiscountAmtF"))
                            row["N_DiscountAmtF"] = 0;
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_DiscountAmtF", typeof(int), 0);
                        if (DetailTable.Columns.Contains("N_Amount"))
                            row["N_Amount"] = nAmount;
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_Amount", typeof(double), nAmount);
                        if (DetailTable.Columns.Contains("X_Description"))
                            row["X_Description"] = xDesc;
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "X_Description", typeof(string), xDesc);
                        if (DetailTable.Columns.Contains("N_BranchID"))
                            row["N_BranchID"] = myFunctions.getIntVAL(Master["N_BranchID"].ToString());
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_BranchID", typeof(int), myFunctions.getIntVAL(Master["N_BranchID"].ToString()));
                        if (DetailTable.Columns.Contains("X_TransType"))
                            row["X_TransType"] = x_Type;
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "X_TransType", typeof(string), x_Type);
                        if (DetailTable.Columns.Contains("N_AmountF"))
                            row["N_AmountF"] = nAmountF;
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_AmountF", typeof(double), nAmountF);
                        if (DetailTable.Columns.Contains("N_AmtPaidFromAdvanceF"))
                            row["N_AmtPaidFromAdvanceF"] = 0;
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_AmtPaidFromAdvanceF", typeof(double), 0);
                        if (DetailTable.Columns.Contains("N_CurrencyID"))
                            row["N_CurrencyID"] = myFunctions.getIntVAL(Master["N_CurrencyID"].ToString());
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_CurrencyID", typeof(int), myFunctions.getIntVAL(Master["N_CurrencyID"].ToString()));

                        if (DetailTable.Columns.Contains("N_ExchangeRate"))
                            row["N_ExchangeRate"] = myFunctions.getVAL(Master["N_ExchangeRate"].ToString());
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_ExchangeRate", typeof(double), myFunctions.getVAL(Master["N_ExchangeRate"].ToString()));
                        if (DetailTable.Columns.Contains("n_PayReceiptDetailsId"))
                        {

                        }
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "n_PayReceiptDetailsId", typeof(int), 0);

                        if (DetailTable.Columns.Contains("N_ProjectID"))
                            row["N_ProjectID"] = myFunctions.getIntVAL(Master["N_ProjectID"].ToString());
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_ProjectID", typeof(int), myFunctions.getIntVAL(Master["N_ProjectID"].ToString()));

                        // row["N_CompanyID"] = myFunctions.getIntVAL(Master["n_CompanyID"].ToString());
                        // row["N_PayReceiptId"] = n_PayReceiptID;
                        // row["N_InventoryId"] = n_PayReceiptID;
                        // row["N_DiscountAmt"] = 0;
                        // row["N_DiscountAmtF"] = 0;
                        // row["N_Amount"] = nAmount;
                        // row["X_Description"] = xDesc;
                        // row["N_BranchID"] = myFunctions.getIntVAL(Master["N_BranchID"].ToString());
                        // row["X_TransType"] = x_Type;
                        // row["N_AmountF"] = nAmountF;
                        // row["N_AmtPaidFromAdvanceF"] = 0;
                        // row["N_CurrencyID"] = myFunctions.getIntVAL(Master["N_CurrencyID"].ToString());
                        // row["N_ExchangeRate"] = myFunctions.getVAL(Master["N_ExchangeRate"].ToString());

                        DetailTable.Rows.Add(row);

                    }

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_PayReceiptID"] = n_PayReceiptID;
                    }
                    int n_PayReceiptDetailId = dLayer.SaveData("Inv_PayReceiptDetails", "n_PayReceiptDetailsID", DetailTable, connection, transaction);

                    if (n_PayReceiptID > 0)
                    {
                        SortedList PostingParams = new SortedList();
                        PostingParams.Add("N_CompanyID", nCompanyId);
                        PostingParams.Add("X_InventoryMode", x_Type);
                        PostingParams.Add("N_InternalID", n_PayReceiptID);
                        PostingParams.Add("N_UserID", myFunctions.GetUserID(User));
                        PostingParams.Add("X_SystemName", "ERP Cloud");
                        object posting = dLayer.ExecuteScalarPro("SP_Acc_InventoryPosting", PostingParams, connection, transaction);

                    }

                  //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(nFnYearID,n_PayReceiptID,PayReceiptNo,67,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                          
                        

                    transaction.Commit();
           
                    if (Attachment.Rows.Count > 0)
                    {
                        try
                        {
                            myAttachments.SaveAttachment(dLayer, Attachment,PayReceiptNo, n_PayReceiptID,objVendName.ToString().Trim(),PayReceiptNo, n_PayReceiptID, "VendorPayment Document", User, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User, ex));
                        }
                    }
                }

                 
                SortedList Result = new SortedList();
                Result.Add("n_VendorReceiptID", n_PayReceiptID);
                Result.Add("x_VendorReceiptNo", PayReceiptNo);
                return Ok(api.Success(Result, "Vendor Payment Saved"));
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }
        [HttpDelete]
        public ActionResult DeleteData(int nPayReceiptId, string xTransType, int nFnyearID,int nCompanyID,string comments,int nFnYearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList(); 
                    ParamList.Add("@nTransID", nPayReceiptId);
                    ParamList.Add("@nFnYearID", nFnyearID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    string xButtonAction = "Delete";
                    string PayReceiptNo = "";


                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,X_VoucherNo,N_PayReceiptId from Inv_PayReceipt where N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID and N_PayReceiptId=@nTransID";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, this.N_FormID, nPayReceiptId, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnyearID, 0, 0, User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), comments);
                    SqlTransaction transaction = connection.BeginTransaction();

                    string X_Criteria = "N_PayReceiptId=" + nPayReceiptId + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnyearID;
                    string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());

                    // if (ButtonTag == "6" || ButtonTag == "0")
                    // {
                    //     if (nPayReceiptId > 0)
                    //     {
                    //         SortedList DeleteParams = new SortedList(){
                    //                 {"N_CompanyID",myFunctions.GetCompanyID(User)},
                    //                 {"X_TransType",xTransType},
                    //                 {"N_VoucherID",nPayReceiptId}};

                    //         int result = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection,transaction);
                    //         if (result > 0)
                    //         {
                    //             myAttachments.DeleteAttachment(dLayer, 1, nPayReceiptId, nPayReceiptId, nFnyearID,67, User, transaction, connection);
                    //             transaction.Commit();
                    //             return Ok(api.Success("Vendor Payment Deleted"));
                    //         }         
                    //     }
                    // }
                    // else
                    // {
                    //     string status = myFunctions.UpdateApprovals(Approvals, nFnyearID, "PURCHASE PAYMENT", nPayReceiptId, TransRow["X_VoucherNo"].ToString(), ProcStatus, "Inv_PayReceipt", X_Criteria, "", User, dLayer, connection, transaction);
                    //     if (status != "Error")
                    //     {
                    //         transaction.Commit();
                    //         return Ok(api.Success("Vendor Payment " + status + " Successfully"));
                    //     }
                    //     else
                    //     {
                    //         transaction.Rollback();
                    //         return Ok(api.Error(User, "Unable to delete Vendor Payment"));
                    //     }
                    // }

                     //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(myFunctions.getIntVAL( nFnYearID.ToString()),nPayReceiptId,TransRow["PayReceiptNo"].ToString(),67,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);



                    string status = myFunctions.UpdateApprovals(Approvals, nFnyearID, "PURCHASE PAYMENT", nPayReceiptId, TransRow["X_VoucherNo"].ToString(), ProcStatus, "Inv_PayReceipt", X_Criteria, "", User, dLayer, connection, transaction);
                    if (status != "Error")
                    {
                        if (ButtonTag == "6" || ButtonTag == "0")
                        {
                            if (nPayReceiptId > 0)
                            {
                                SortedList DeleteParams = new SortedList(){
                                        {"N_CompanyID",myFunctions.GetCompanyID(User)},
                                        {"X_TransType",xTransType},
                                        {"N_VoucherID",nPayReceiptId}};

                                int result = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection,transaction);
                                if (result > 0)
                                {
                                    myAttachments.DeleteAttachment(dLayer, 1, nPayReceiptId, nPayReceiptId, nFnyearID,67, User, transaction, connection);
                                    transaction.Commit();
                                    return Ok(api.Success("Vendor Payment Deleted"));
                                }         
                            }
                        }
                        transaction.Commit();
                        return Ok(api.Success("Vendor Payment " + status + " Successfully"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to delete Vendor Payment"));
                    }
                }
                return Ok(api.Warning("Unable to delete Vendor Payment"));

            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

        [HttpGet("paymentType")]
        public ActionResult GetPaymentType()
        {
            string sqlCommandText = "select 'Invoice Payment' AS X_PaymentType,'ادفع الفاتورة' AS X_PaymentType_Ar,'PP' AS x_Type UNION select 'Advance Payment' AS X_PaymentType,'دفعه مقدمه' AS X_PaymentType_Ar,'PA' AS x_Type";
            SortedList mParamList = new SortedList() { };
            DataTable typeTable = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    typeTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList, connection);
                }
                typeTable = api.Format(typeTable, "PaymentType");
                if (typeTable.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(typeTable));
                }
            }
            catch (Exception e)
            {
                return StatusCode(403, api.Error(User, e));
            }
        }

        //  [HttpGet("dummy")]
        // public ActionResult GetPurchaseInvoiceDummy(int? Id)
        // {
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //         string sqlCommandText = "select * from Inv_PayReceipt where N_PayReceiptId=@p1";
        //         SortedList mParamList = new SortedList() { { "@p1", Id } };
        //         DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList,connection);
        //         masterTable = api.Format(masterTable, "master");

        //         string sqlCommandText2 = "select * from Inv_PayReceiptDetails where N_PayReceiptId=@p1";
        //         SortedList dParamList = new SortedList() { { "@p1", Id } };
        //         DataTable detailTable = dLayer.ExecuteDataTable(sqlCommandText2, dParamList,connection);
        //         detailTable = api.Format(detailTable, "details");

        //         if (detailTable.Rows.Count == 0) { return Ok(new { }); }
        //         DataSet dataSet = new DataSet();
        //         dataSet.Tables.Add(masterTable);
        //         dataSet.Tables.Add(detailTable);

        //         return Ok(dataSet);
        //         }

        //     }
        //     catch (Exception e)
        //     {
        //         return StatusCode(403, api.Error(User,e));
        //     }
        // }
    }



}