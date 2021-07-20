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
using System;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("purchaseinvoice")]
    [ApiController]
    public class Inv_PurchaseInvoice : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int N_FormID;
        public Inv_PurchaseInvoice(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 65;
        }


        [HttpGet("list")]
        public ActionResult GetPurchaseInvoiceList(int? nCompanyId, int nFnYearId, bool bAllBranchData, int nBranchID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    DataTable CountTable = new DataTable();
                    SortedList Params = new SortedList();
                    DataSet dataSet = new DataSet();
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";
                    string X_TransType = "PURCHASE";
                    bool CheckClosedYear = Convert.ToBoolean(dLayer.ExecuteScalar("Select B_YearEndProcess From Acc_FnYear Where N_CompanyID=" + nCompanyId + " and N_FnYearID = " + nFnYearId, Params, connection));

                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and ([Invoice No] like '%" + xSearchkey + "%' or Vendor like '%" + xSearchkey + "%' or x_BranchName like '%" + xSearchkey + "%' or cast([Invoice Date] as VarChar) like '%" + xSearchkey + "%' or invoiceNetAmt like '%" + xSearchkey + "%' or x_Description like '%" + xSearchkey + "%')";

                    if (CheckClosedYear == false)
                    {
                        if (bAllBranchData == true)
                        {
                            Searchkey = Searchkey + " and X_TransType='" + X_TransType + "' and  N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and B_YearEndProcess=0 and N_PurchaseType = 0 ";
                        }
                        else
                        {
                            Searchkey = Searchkey + "and X_TransType='" + X_TransType + "' and  N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and B_YearEndProcess=0 and N_PurchaseType = 0  and N_BranchID=" + nBranchID + "";
                        }
                    }
                    else
                    {
                        if (bAllBranchData == true)
                        {
                            Searchkey = Searchkey + " and X_TransType='" + X_TransType + "' and  N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and B_YearEndProcess=0 and N_PurchaseType = 0 ";
                        }
                        else
                        {
                            Searchkey = Searchkey + "and X_TransType='" + X_TransType + "' and  N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and B_YearEndProcess=0 and N_PurchaseType = 0  and N_BranchID=" + nBranchID + "";
                        }
                    }


                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_PurchaseID desc";
                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "invoiceNo":
                                xSortBy = "N_PurchaseID " + xSortBy.Split(" ")[1];
                                break;
                             case "invoiceDate":
                                xSortBy = "Cast([Invoice Date] as DateTime ) " + xSortBy.Split(" ")[1];
                                break;
                            case "invoiceNetAmt":
                                xSortBy = "Cast(REPLACE(InvoiceNetAmt,',','') as Numeric(10,2)) " + xSortBy.Split(" ")[1];
                                break;
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }
                    int Count = (nPage - 1) * nSizeperpage;
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") N_PurchaseID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],InvoiceNetAmt,X_BranchName,X_Description,N_PaymentMethod,N_FnYearID,N_BranchID,N_LocationID,N_VendorID,N_InvDueDays from vw_InvPurchaseInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") N_PurchaseID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],InvoiceNetAmt,X_BranchName,X_Description,N_PaymentMethod,N_FnYearID,N_BranchID,N_LocationID,N_VendorID,N_InvDueDays from vw_InvPurchaseInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and N_PurchaseID not in (select top(" + Count + ") N_PurchaseID from vw_InvPurchaseInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + xSortBy + " ) " + xSortBy;

                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearId);
                    SortedList OutPut = new SortedList();


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "N_BalanceAmt", typeof(double), 0);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "N_DueDays", typeof(string), "");
                    foreach (DataRow var in dt.Rows)
                    {
                        double BalanceAmt = 0;
                        object objBal = dLayer.ExecuteScalar("SELECT  Sum(PurchaseBalanceAmt) from  vw_InvPayables Where  N_VendorID=" + var["N_VendorID"] + " and N_CompanyID=1 and N_PurchaseID = " + var["N_PurchaseID"], Params, connection);

                        if (objBal != null)
                        {
                            BalanceAmt = myFunctions.getVAL(objBal.ToString());
                            if (BalanceAmt > 0)
                            {
                                var["N_BalanceAmt"] = BalanceAmt;
                                if (var["N_InvDueDays"].ToString() != "")
                                {
                                    DateTime dtInvoice = new DateTime();
                                    DateTime dtDuedate = new DateTime();
                                    dtInvoice = Convert.ToDateTime(var["Invoice Date"].ToString());
                                    dtDuedate = dtInvoice.AddDays(myFunctions.getIntVAL(var["N_InvDueDays"].ToString()));
                                    if (DateTime.Now > dtDuedate)
                                    {
                                        var DueDays = (DateTime.Now - dtDuedate).TotalDays;
                                        string Due_Days = Math.Truncate(DueDays).ToString();
                                        var["N_DueDays"] = Due_Days.ToString() + " days";
                                    }
                                }
                            }
                        }

                    }


                    sqlCommandCount = "select count(*) as N_Count,sum(Cast(REPLACE(InvoiceNetAmt,',','') as Numeric(10,2)) ) as TotalAmount from vw_InvPurchaseInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    string TotalSum = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                        TotalSum = drow["TotalAmount"].ToString();
                    }
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    OutPut.Add("TotalSum", TotalSum);

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
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        [HttpGet("listOrder")]
        public ActionResult GetPurchaseOrderList(int? nCompanyId, int nFnYearId, int nVendorId, bool showAllBranch)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "";
            sqlCommandText = "select top(30) N_PurchaseID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],InvoiceNetAmt from vw_InvPurchaseInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2";

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return StatusCode(200, _api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(dt);
                }
            }
            catch (Exception e)
            {
                return StatusCode(404, _api.Error(e.Message));
            }
        }
        [HttpGet("listdetails")]
        public ActionResult GetPurchaseInvoiceDetails(int nCompanyId, int nFnYearId, string nPurchaseNO, bool showAllBranch, int nBranchId, string xPOrderNo)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable dtPurchaseInvoice = new DataTable();
            DataTable dtPurchaseInvoiceDetails = new DataTable();
            int N_PurchaseID = 0;
            int N_POrderID = 0;

            Params.Add("@CompanyID", nCompanyId);
            Params.Add("@YearID", nFnYearId);
            Params.Add("@TransType", "PURCHASE");
            Params.Add("@BranchID", nBranchId);
            string X_MasterSql = "";
            string X_DetailsSql = "";

            if (nPurchaseNO != null)

            {
                Params.Add("@PurchaseNo", nPurchaseNO);
                X_MasterSql = "select * from vw_Inv_PurchaseDisp where N_CompanyID=@CompanyID and X_InvoiceNo=@PurchaseNo and N_FnYearID=@YearID and X_TransType=@TransType" + (showAllBranch ? "" : " and  N_BranchId=@BranchID");
            }
            else if (xPOrderNo != null)
            {
                Params.Add("@POrderNo", xPOrderNo);
                X_MasterSql = "select * from vw_Inv_PurchaseOrderAsInvoiceMaster where N_CompanyID=@CompanyID and X_POrderNo=@POrderNo and N_FnYearID=@YearID " + (showAllBranch ? "" : " and  N_BranchId=@BranchID");
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dtPurchaseInvoice = dLayer.ExecuteDataTable(X_MasterSql, Params, connection);


                    if (dtPurchaseInvoice.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                    dtPurchaseInvoice = _api.Format(dtPurchaseInvoice, "Master");
                    N_PurchaseID = myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["N_PurchaseID"].ToString());
                    N_POrderID = myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["N_POrderID"].ToString());
                    if (nPurchaseNO != null)

                    {
                        SortedList Status = StatusSetup(N_PurchaseID, nFnYearId, showAllBranch, dtPurchaseInvoice, connection);
                        dtPurchaseInvoice = myFunctions.AddNewColumnToDataTable(dtPurchaseInvoice, "TxnStatus", typeof(SortedList), Status);
                    }

                    //PURCHASE INVOICE DETAILS
                    bool B_MRNVisible = myFunctions.CheckPermission(nCompanyId, 556, "Administrator", "X_UserCategory", dLayer, connection);

                    if (nPurchaseNO != null)
                    {
                        if (B_MRNVisible)
                            X_DetailsSql = "Select vw_InvPurchaseDetails.*,Inv_PurchaseOrder.X_POrderNo,Inv_MRN.X_MRNNo,dbo.SP_Cost(vw_InvPurchaseDetails.N_ItemID,vw_InvPurchaseDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseDetails.N_ItemID,vw_InvPurchaseDetails.N_CompanyID) As N_UnitSPrice   from vw_InvPurchaseDetails Left Outer Join Inv_PurchaseOrder On vw_InvPurchaseDetails.N_POrderID=Inv_PurchaseOrder.N_POrderID Left Outer Join Inv_MRN On vw_InvPurchaseDetails.N_RsID=Inv_MRN.N_MRNID  Where vw_InvPurchaseDetails.N_CompanyID=@CompanyID and vw_InvPurchaseDetails.N_PurchaseID=" + N_PurchaseID + (showAllBranch ? "" : " and vw_InvPurchaseDetails.N_BranchId=@BranchID");
                        else
                            X_DetailsSql = "Select vw_InvPurchaseDetails.*,Inv_PurchaseOrder.X_POrderNo,dbo.SP_Cost(vw_InvPurchaseDetails.N_ItemID,vw_InvPurchaseDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseDetails.N_ItemID,vw_InvPurchaseDetails.N_CompanyID) As N_UnitSPrice   from vw_InvPurchaseDetails Left Outer Join Inv_PurchaseOrder On vw_InvPurchaseDetails.N_POrderID=Inv_PurchaseOrder.N_POrderID Where vw_InvPurchaseDetails.N_CompanyID=@CompanyID and vw_InvPurchaseDetails.N_PurchaseID=" + N_PurchaseID + (showAllBranch ? "" : " and vw_InvPurchaseDetails.N_BranchId=@BranchID");
                    }
                    else if (xPOrderNo != null)
                    {
                        X_DetailsSql = "select * from vw_Inv_PurchaseOrderAsInvoiceDetails where N_CompanyID=@CompanyID and N_POrderID=" + N_POrderID + (showAllBranch ? "" : " and  N_BranchId=@BranchID");
                    }
                    dtPurchaseInvoiceDetails = dLayer.ExecuteDataTable(X_DetailsSql, Params, connection);

                    if (nPurchaseNO != null)
                    {
                        object RetQty = dLayer.ExecuteScalar("select X_CreditNoteNo from Inv_PurchaseReturnMaster where N_PurchaseId =" + N_PurchaseID + " and N_CompanyID=@CompanyID and N_FnYearID=@YearID", Params, connection);
                        if (RetQty != null)
                        {
                            dtPurchaseInvoice = myFunctions.AddNewColumnToDataTable(dtPurchaseInvoice, "IsReturnDone", typeof(bool), true);
                            dtPurchaseInvoice = myFunctions.AddNewColumnToDataTable(dtPurchaseInvoice, "X_ReturnCode", typeof(string), RetQty.ToString());
                        }
                        else
                        {
                            dtPurchaseInvoice = myFunctions.AddNewColumnToDataTable(dtPurchaseInvoice, "IsReturnDone", typeof(bool), false);
                            dtPurchaseInvoice = myFunctions.AddNewColumnToDataTable(dtPurchaseInvoice, "X_ReturnCode", typeof(string), "");
                        }

                        // int nPaymentMethod = mastr
                    }


                    dtPurchaseInvoiceDetails = _api.Format(dtPurchaseInvoiceDetails, "Details");
                    DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["N_VendorID"].ToString()), myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["N_PurchaseID"].ToString()), this.N_FormID, myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["N_FnYearID"].ToString()), User, connection);
                    Attachments = _api.Format(Attachments, "attachments");

                    dt.Tables.Add(dtPurchaseInvoice);
                    dt.Tables.Add(Attachments);
                    dt.Tables.Add(dtPurchaseInvoiceDetails);
                }

                return Ok(_api.Success(dt));

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        private SortedList StatusSetup(int nPurchaseID, int nFnYearID, bool showAllBranch, DataTable dtPurchaseInvoice, SqlConnection connection)
        {




            SortedList TxnStatus = new SortedList();
            TxnStatus.Add("Label", "");
            TxnStatus.Add("LabelColor", "");
            TxnStatus.Add("Alert", "");
            TxnStatus.Add("DeleteEnabled", true);
            TxnStatus.Add("SaveEnabled", true);
            TxnStatus.Add("ReceiptNumbers", "");
            int nCompanyID = myFunctions.GetCompanyID(User);
            int N_PaymentMethod = myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["N_PaymentMethod"].ToString());
            string x_InvoiceNo = (dtPurchaseInvoice.Rows[0]["x_InvoiceNo"].ToString());
            int n_BranchId = myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["n_BranchId"].ToString());
            string x_TransType = (dtPurchaseInvoice.Rows[0]["x_TransType"].ToString());
            int n_LocationID = myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["n_LocationID"].ToString());
            bool b_AllowCashPay = myFunctions.getBoolVAL(dtPurchaseInvoice.Rows[0]["b_AllowCashPay"].ToString());
            int n_VendorID = myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["n_VendorID"].ToString());
            object objPaid = null, objBal = null;
            double InvoicePaidAmt = 0, BalanceAmt = 0;
            string PurchaseID = "";


            string PurchaseSql = "Select N_PurchaseID from vw_Inv_PurchaseDisp Where N_CompanyID=" + nCompanyID + " and X_InvoiceNo='" + x_InvoiceNo + "' and N_FnYearID=" + nFnYearID + " and X_TransType='PURCHASE'";
            DataTable PurchaseTable = dLayer.ExecuteDataTable(PurchaseSql, connection);
            foreach (DataRow kvar in PurchaseTable.Rows)
            {
                PurchaseID += PurchaseID == "" ? kvar["N_PurchaseID"].ToString() : " , " + kvar["N_PurchaseID"].ToString();

            }




            if (N_PaymentMethod == 2)
            {

                objPaid = dLayer.ExecuteScalar("SELECT  isnull(Sum(dbo.Inv_PayReceiptDetails.N_Amount),0) as PaidAmount FROM  dbo.Inv_PayReceipt INNER JOIN dbo.Inv_PayReceiptDetails ON dbo.Inv_PayReceipt.N_PayReceiptId = dbo.Inv_PayReceiptDetails.N_PayReceiptId Where dbo.Inv_PayReceipt.X_Type='PP' and dbo.Inv_PayReceiptDetails.X_TransType='PURCHASE' and  isnull(dbo.Inv_PayReceipt.B_IsDraft,0) <> 1 and dbo.Inv_PayReceiptDetails.N_InventoryId in (" + PurchaseID + ") group by dbo.Inv_PayReceiptDetails.N_PayReceiptId", connection);
            }
            else
            {
                if (showAllBranch == true)
                    objPaid = dLayer.ExecuteScalar("Select N_CashPaid from vw_Inv_PurchaseDisp Where N_CompanyID=" + nCompanyID + " and X_InvoiceNo='" + x_InvoiceNo + "' and N_FnYearID=" + nFnYearID + " and X_TransType='" + x_TransType + "'", connection);
                else
                    objPaid = dLayer.ExecuteScalar("Select N_CashPaid from vw_Inv_PurchaseDisp Where N_CompanyID=" + nCompanyID + " and X_InvoiceNo='" + x_InvoiceNo + "' and N_FnYearID=" + nFnYearID + " and N_BranchId=" + n_BranchId + " and N_LocationID =" + n_LocationID + "  and X_TransType='" + x_TransType + "'", connection);
            }


            if (objPaid == null && N_PaymentMethod == 2)
            {
                if (showAllBranch == true)
                    objPaid = dLayer.ExecuteScalar("Select N_CashPaid from vw_Inv_PurchaseDisp Where N_CompanyID=" + nCompanyID + " and X_InvoiceNo='" + x_InvoiceNo + "' and N_FnYearID=" + nFnYearID + " and X_TransType='" + x_TransType + "'", connection);
                else
                    objPaid = dLayer.ExecuteScalar("Select N_CashPaid from vw_Inv_PurchaseDisp Where N_CompanyID=" + nCompanyID + " and X_InvoiceNo='" + x_InvoiceNo + "' and N_FnYearID=" + nFnYearID + " and N_BranchId=" + n_BranchId + " and N_LocationID =" + n_LocationID + "  and X_TransType='" + x_TransType + "'", connection);
            }

            objBal = dLayer.ExecuteScalar("SELECT  Sum(PurchaseBalanceAmt) from  vw_InvPayables Where  N_VendorID=" + n_VendorID + " and N_CompanyID=" + nCompanyID + " and N_PurchaseID = " + nPurchaseID, connection);
            object RetQty = dLayer.ExecuteScalar("select Isnull(Count(N_CreditNoteId),0) from Inv_PurchaseReturnMaster where  N_PurchaseId=" + nPurchaseID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, connection);



            if (objPaid != null)
                InvoicePaidAmt = myFunctions.getVAL(objPaid.ToString());
            if (objBal != null)
                BalanceAmt = myFunctions.getVAL(objBal.ToString());






            if (InvoicePaidAmt == 0)
            {
                if (myFunctions.getIntVAL(RetQty.ToString()) > 0)
                {
                    if (BalanceAmt == 0)
                    {
                        TxnStatus["Label"] = "Paid";
                        TxnStatus["LabelColor"] = "Green";
                        TxnStatus["Alert"] = "";
                    }
                    else
                    {


                        TxnStatus["Label"] = "NotPaid";
                        TxnStatus["LabelColor"] = "Red";
                        TxnStatus["Alert"] = "";
                    }
                }

                else
                {
                    TxnStatus["Label"] = "Not Paid ";
                    TxnStatus["LabelColor"] = "Green";
                    TxnStatus["Alert"] = "";
                }
            }
            else
            {
                if (BalanceAmt == 0)
                {
                    TxnStatus["Label"] = "Paid";
                    TxnStatus["LabelColor"] = "Green";
                    TxnStatus["Alert"] = "";
                }
                else
                {
                    TxnStatus["Label"] = "Partially Paid";
                    TxnStatus["LabelColor"] = "Green";
                    TxnStatus["Alert"] = "";
                }
            }



            return TxnStatus;
        }




        //Save....
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {

            DataTable MasterTable;
            DataTable DetailTable;
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            DataTable Approvals;
            Approvals = ds.Tables["approval"];
            DataRow ApprovalRow = Approvals.Rows[0];
            DataTable Attachment = ds.Tables["attachments"];
            SortedList Params = new SortedList();
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
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();
                    N_PurchaseID = myFunctions.getIntVAL(masterRow["n_PurchaseID"].ToString());
                    int N_VendorID = myFunctions.getIntVAL(masterRow["n_VendorID"].ToString());
                    int N_NextApproverID = 0;

                    if (!myFunctions.CheckActiveYearTransaction(nCompanyID, nFnYearID, Convert.ToDateTime(MasterTable.Rows[0]["D_InvoiceDate"].ToString()), dLayer, connection, transaction))
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Transaction date must be in the active Financial Year."));
                    }

                    if (N_PurchaseID > 0)
                    {
                        if (CheckProcessed(N_PurchaseID))
                            return Ok(_api.Error("Transaction Started!"));
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
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "PURCHASE", N_PkeyID, values, 1, objVendorName.ToString(), 0, "", User, dLayer, connection, transaction);
                        myAttachments.SaveAttachment(dLayer, Attachment, values, N_PurchaseID, objVendorName.ToString().Trim(), objVendorCode.ToString(), N_VendorID, "Vendor Document", User, connection, transaction);

                        N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_Purchase where N_PurchaseID=" + N_PurchaseID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, connection, transaction).ToString());
                        if (N_SaveDraft == 0)
                        {
                            try
                            {
                                SortedList PostingMRNParam = new SortedList();
                                PostingMRNParam.Add("N_CompanyID", nCompanyID);
                                PostingMRNParam.Add("N_PurchaseID", N_PurchaseID);
                                PostingMRNParam.Add("N_UserID", nUserID);
                                PostingMRNParam.Add("X_SystemName", "ERP Cloud");
                                PostingMRNParam.Add("X_UseMRN", "");
                                PostingMRNParam.Add("N_SaveDraft", N_SaveDraft);
                                PostingMRNParam.Add("N_MRNID", 0);

                                dLayer.ExecuteNonQueryPro("[SP_Inv_MRNposting]", PostingMRNParam, connection, transaction);


                                SortedList PostingParam = new SortedList();
                                PostingParam.Add("N_CompanyID", nCompanyID);
                                PostingParam.Add("X_InventoryMode", "PURCHASE");
                                PostingParam.Add("N_InternalID", N_PurchaseID);
                                PostingParam.Add("N_UserID", nUserID);
                                PostingParam.Add("X_SystemName", "ERP Cloud");

                                dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Purchase_Posting", PostingParam, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(ex.Message));
                            }
                        }

                        myFunctions.SendApprovalMail(N_NextApproverID, this.N_FormID, N_PkeyID, "PURCHASE", values, dLayer, connection, transaction, User);
                        transaction.Commit();
                        return Ok(_api.Success("Purchase Approved " + "-" + values));
                    }

                    if (values == "@Auto")
                    {
                        N_SaveDraft = myFunctions.getIntVAL(masterRow["b_IsSaveDraft"].ToString());

                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("N_BranchID", masterRow["n_BranchId"].ToString());

                        InvoiceNo = dLayer.GetAutoNumber("Inv_Purchase", "x_InvoiceNo", Params, connection, transaction);
                        if (InvoiceNo == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable to generate Invoice Number"));
                        }
                        MasterTable.Rows[0]["x_InvoiceNo"] = InvoiceNo;
                    }

                    if (N_PurchaseID > 0)
                    {
                        SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",masterRow["n_CompanyId"].ToString()},
                                {"X_TransType","PURCHASE"},
                                {"N_VoucherID",N_PurchaseID},
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
                            if (ex.Message.Contains("50"))
                                return Ok(_api.Error("DayClosed"));
                            else if (ex.Message.Contains("51"))
                                return Ok(_api.Error("YearClosed"));
                            else if (ex.Message.Contains("52"))
                                return Ok(_api.Error("YearExists"));
                            else if (ex.Message.Contains("53"))
                                return Ok(_api.Error("PeriodClosed"));
                            else if (ex.Message.Contains("54"))
                                return Ok(_api.Error("TxnDate"));
                            else if (ex.Message.Contains("55"))
                                return Ok(_api.Error("TransactionStarted"));
                            return Ok(_api.Error(ex.Message));
                        }
                    }
                    MasterTable.Rows[0]["n_userID"] = myFunctions.GetUserID(User);
                    if (MasterTable.Columns.Contains("N_ApprovalLevelID"))
                        MasterTable.Columns.Remove("N_ApprovalLevelID");
                    if (MasterTable.Columns.Contains("N_Procstatus"))
                        MasterTable.Columns.Remove("N_Procstatus");
                    if (MasterTable.Columns.Contains("B_IsSaveDraft"))
                        MasterTable.Columns.Remove("B_IsSaveDraft");
                    MasterTable.AcceptChanges();

                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);

                    N_PurchaseID = dLayer.SaveData("Inv_Purchase", "N_PurchaseID", MasterTable, connection, transaction);

                    if (N_PurchaseID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Purchase Invoice!"));
                    }

                    N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "PURCHASE", N_PurchaseID, InvoiceNo, 1, objVendorName.ToString(), 0, "", User, dLayer, connection, transaction);
                    N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_Purchase where N_PurchaseID=" + N_PurchaseID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, connection, transaction).ToString());

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        int UnitID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_ItemUnitID from inv_itemunit where N_ItemID=" + myFunctions.getIntVAL(DetailTable.Rows[j]["N_ItemID"].ToString()) + " and N_CompanyID=" + myFunctions.getIntVAL(DetailTable.Rows[j]["N_CompanyID"].ToString()) + " and X_ItemUnit='" + DetailTable.Rows[j]["X_ItemUnit"].ToString() + "'", connection, transaction).ToString());
                        DetailTable.Rows[j]["N_PurchaseID"] = N_PurchaseID;
                        DetailTable.Rows[j]["N_ItemUnitID"] = UnitID;
                    }
                    DetailTable.Columns.Remove("X_ItemUnit");
                    int N_InvoiceDetailId = dLayer.SaveData("Inv_PurchaseDetails", "n_PurchaseDetailsID", DetailTable, connection, transaction);
                    if (N_InvoiceDetailId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Purchase Invoice!"));
                    }

                    if (N_PurchaseID > 0)
                    {
                        dLayer.ExecuteScalar("Update Inv_PurchaseOrder Set N_Processed=1 , N_PurchaseID=" + N_PurchaseID + " Where N_POrderID=" + n_POrderID + " and N_CompanyID=" + nCompanyID, connection, transaction);
                        // if (B_ServiceSheet)
                        //     dba.ExecuteNonQuery("Update Inv_VendorServiceSheet Set N_Processed=1  Where N_RefID=" + n_POrderID + " and N_FnYearID=" + nFnYearID + " and N_CompanyID=" + nCompanyID,connection,transaction);

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
                            PostingMRNParam.Add("N_MRNID", 0);

                            dLayer.ExecuteNonQueryPro("[SP_Inv_MRNposting]", PostingMRNParam, connection, transaction);


                            SortedList PostingParam = new SortedList();
                            PostingParam.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
                            PostingParam.Add("X_InventoryMode", "PURCHASE");
                            PostingParam.Add("N_InternalID", N_PurchaseID);
                            PostingParam.Add("N_UserID", nUserID);
                            PostingParam.Add("X_SystemName", "ERP Cloud");

                            dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Purchase_Posting", PostingParam, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(ex.Message));
                        }
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
                            return Ok(_api.Error(ex));
                        }
                    }
                    myFunctions.SendApprovalMail(N_NextApproverID, this.N_FormID, N_PurchaseID, "PURCHASE", InvoiceNo, dLayer, connection, transaction, User);

                    transaction.Commit();
                }
                SortedList Result = new SortedList();
                Result.Add("n_InvoiceID", N_PurchaseID);
                Result.Add("x_InvoiceNo", InvoiceNo);
                return Ok(_api.Success(Result, "Purchase Invoice Saved"));
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
        private bool CheckProcessed(int nPurchaseID)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            object AdvancePRProcessed = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlCommand = "Select COUNT(N_TransID) From Inv_PaymentRequest Where  N_CompanyID=@p1 and N_TransID=@p2 and N_FormID=65";
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", nPurchaseID);
                AdvancePRProcessed = dLayer.ExecuteScalar(sqlCommand, Params, connection);

            }
            if (AdvancePRProcessed != null)
            {
                if (myFunctions.getIntVAL(AdvancePRProcessed.ToString()) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        //Delete....
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPurchaseID, int nFnYearID, string comments)
        {
            if(comments==null){
                comments="";
            }
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            int Results = 0;
            if (CheckProcessed(nPurchaseID))
                return Ok(_api.Error("Transaction Started"));
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nTransID", nPurchaseID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,isNull(N_VendorID,0) as N_VendorID,X_InvoiceNo from Inv_Purchase where N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID and N_PurchaseID=@nTransID";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error("Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    int VendorID = myFunctions.getIntVAL(TransRow["N_VendorID"].ToString());

                    SortedList VendParams = new SortedList();
                    VendParams.Add("@nCompanyID", nCompanyID);
                    VendParams.Add("@N_VendorID", VendorID);
                    VendParams.Add("@nFnYearID", nFnYearID);
                    object objVendorName = dLayer.ExecuteScalar("Select X_VendorName From Inv_Vendor where N_VendorID=@N_VendorID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", VendParams, connection);

                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, this.N_FormID, nPurchaseID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID, 0, 0, User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), comments);
                    SqlTransaction transaction = connection.BeginTransaction();

                    string X_Criteria = "N_PurchaseID=" + nPurchaseID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnYearID;
                    string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());
                    if (ButtonTag == "6" || ButtonTag == "0")
                    {
                        SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType","PURCHASE"},
                                {"N_VoucherID",nPurchaseID},
                                {"N_UserID",nUserID},
                                {"X_SystemName","WebRequest"},
                                {"@B_MRNVisible","0"}};

                        Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", DeleteParams, connection, transaction);
                        if (Results <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable to Delete PurchaseInvoice"));
                        }

                    }
                    else
                    {
                        string status = myFunctions.UpdateApprovals(Approvals, nFnYearID, "PURCHASE", nPurchaseID, TransRow["X_InvoiceNo"].ToString(), ProcStatus, "Inv_Purchase", X_Criteria, objVendorName.ToString(), User, dLayer, connection, transaction);
                        if (status != "Error")
                        {
                            transaction.Commit();
                            return Ok(_api.Success("Purchase Invoice " + status + " Successfully"));
                        }
                        else
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable to delete Purchase Invoice"));
                        }
                    }


                    transaction.Commit();
                    return Ok(_api.Success("Purchase invoice deleted"));

                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }


        }


    }
}