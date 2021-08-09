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
        private readonly int N_FormID;


        public Inv_VendorPayment(IDataAccessLayer dl, IApiFunctions apiFun, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 67;
        }


        [HttpGet("list")]
        public ActionResult GetVendorPayment(int? nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (Memo like '%" + xSearchkey + "%' or [Vendor Name] like '%" + xSearchkey + "%' or Cast(Date as VarChar) like '%" + xSearchkey + "%' or X_Notes like '%" + xSearchkey + "%' or amount like '%" + xSearchkey + "%')";

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
                        xSortBy = "Cast(REPLACE(Amount,',','') as Numeric(10,2)) " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }

                xSortBy = " order by " + xSortBy;
            }


            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvPayment_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and B_YearEndProcess=0 and (X_type='PP' OR X_type='PA') and amount is not null " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvPayment_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and B_YearEndProcess=0 and (X_type='PP' OR X_type='PA') and amount is not null " + Searchkey + " and n_PayReceiptID not in (select top(" + Count + ") n_PayReceiptID from vw_InvPayment_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and B_YearEndProcess=0  and amount is not null " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // connection.Open();
                    // dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    // sqlCommandCount = "select count(*) as N_Count  from vw_InvPayment_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and B_YearEndProcess=0  and amount is not null ";
                    // object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    // OutPut.Add("Details",api.Format(dt));
                    // OutPut.Add("TotalCount",TotalCount);

                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count,sum(Cast(REPLACE(Amount,',','') as Numeric(10,2)) ) as TotalAmount from vw_InvPayment_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and B_YearEndProcess=0 and (X_type='PP' OR X_type='PA') and amount is not null " + Searchkey + "";
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
                }
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
            catch (Exception e)
            {
                return StatusCode(403, api.Error(e));
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
        //             sqlCommandCount = "select count(*) as N_Count  from vw_InvPayment_Search where N_CompanyID=@p1 and N_FnYearID=@p2";
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
        //         return StatusCode(403, api.Error(e));
        //     }
        // }




        [HttpGet("payDetails")]
        public ActionResult GetVendorPayDetails(int nVendorID, int nFnYearId, string dTransDate, int nBranchID, bool bShowAllbranch, string xInvoiceNo, string xTransType)
        {
            SortedList OutPut = new SortedList();
            DataTable PayReceipt = new DataTable();
            DataTable PayInfo = new DataTable();

            string sql = "";
            int AllBranch = 1;
            int nPayReceiptID = 0;
            int nCompanyId = myFunctions.GetCompanyID(User);
            OutPut.Add("totalAmtDue", 0);
            OutPut.Add("totalBalance", 0);
            OutPut.Add("advanceAmount", 0);
            OutPut.Add("txnStarted", false);
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

                    if (xInvoiceNo != null && myFunctions.getIntVAL(xInvoiceNo) > 0)
                    {
                        SortedList proParams1 = new SortedList(){
                                {"N_CompanyID",nCompanyId},
                                {"X_VoucherNo",xInvoiceNo},
                                {"N_FnYearID",nFnYearId},
                                {"N_BranchID",nBranchID}};
                        PayInfo = dLayer.ExecuteDataTablePro("SP_Inv_InvPayReceipt_Disp", proParams1, connection);
                        if (PayInfo.Rows.Count > 0)
                        {
                            nPayReceiptID = myFunctions.getIntVAL(PayInfo.Rows[0]["N_PayReceiptId"].ToString());
                            xTransType = PayInfo.Rows[0]["X_Type"].ToString();
                            nVendorID = myFunctions.getIntVAL(PayInfo.Rows[0]["N_PartyID"].ToString());
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

                }

                PayReceipt = myFunctions.AddNewColumnToDataTable(PayReceipt, "n_DueAmount", typeof(double), 0);


                if (PayReceipt.Rows.Count > 0)
                {
                    double N_ListedAmtTotal = 0;
                    foreach (DataRow dr in PayReceipt.Rows)
                    {
                        if (PayReceipt.Columns.Contains("n_PayReceiptID"))
                        {
                            object payrcptid = dr["n_PayReceiptID"].ToString();
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
                        dr["n_DueAmount"] = N_InvoiceDueAmt.ToString(myFunctions.decimalPlaceString(2));
                    }
                }
                PayReceipt.AcceptChanges();
                return Ok(api.Success(new SortedList() { { "details", api.Format(PayReceipt) }, { "masterData", OutPut }, { "master", PayInfo } }));
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
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
                SortedList Params = new SortedList();
                int n_PayReceiptID = 0;
                string PayReceiptNo = "";
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;


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

                    transaction = connection.BeginTransaction();

                    if (x_VoucherNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyId);
                        Params.Add("N_YearID", Master["n_FnYearID"].ToString());
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("N_BranchID", Master["n_BranchID"].ToString());

                        PayReceiptNo = dLayer.GetAutoNumber("Inv_PayReceipt", "x_VoucherNo", Params, connection, transaction);
                        if (PayReceiptNo == "") { transaction.Rollback(); return Ok(api.Warning("Unable to generate Receipt Number")); }
                        MasterTable.Rows[0]["x_VoucherNo"] = PayReceiptNo;
                    }
                    else
                    {

                        if (n_PayReceiptID > 0)
                        {

                            SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyId},
                                {"X_TransType",x_Type},
                                {"N_VoucherID",n_PayReceiptID}};
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);


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


                    n_PayReceiptID = dLayer.SaveData("Inv_PayReceipt", "n_PayReceiptID", MasterTable, connection, transaction);
                    if (n_PayReceiptID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Error"));
                    }
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
                            row["N_ExchangeRate"] = myFunctions.getIntVAL(Master["N_ExchangeRate"].ToString());
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_ExchangeRate", typeof(int), myFunctions.getIntVAL(Master["N_ExchangeRate"].ToString()));
                        if (DetailTable.Columns.Contains("n_PayReceiptDetailsId"))
                        {

                        }
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "n_PayReceiptDetailsId", typeof(int), 0);










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
                    transaction.Commit();
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
        public ActionResult DeleteData(int nPayReceiptId, string xTransType)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (nPayReceiptId > 0)
                    {
                        SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",myFunctions.GetCompanyID(User)},
                                {"X_TransType",xTransType},
                                {"N_VoucherID",nPayReceiptId}};
                        int result = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection);
                        if (result > 0)
                        {
                            return Ok(api.Success("Vendor Payment Deleted"));
                        }
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
            string sqlCommandText = "select 'Vendor Payment' AS X_PaymentType,'PP' AS x_Type UNION select 'Advance Payment' AS X_PaymentType,'PA' AS x_Type";
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
                return StatusCode(403, api.Error(e));
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
        //         return StatusCode(403, api.Error(e));
        //     }
        // }
    }



}