using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("salesreceipt")]
    [ApiController]
    public class Inv_SalesReceipt : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Inv_SalesReceipt(IApiFunctions apiFun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apiFun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("list")]
        public ActionResult GetSalesReceipt(int nFnYearId, int nListID, bool bAllBranchData, int nBranchID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int nCompanyId = myFunctions.GetCompanyID(User);
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();

                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";
                      int N_decimalPlace=2;
                    N_decimalPlace = myFunctions.getIntVAL(myFunctions.ReturnSettings("Sales", "Decimal_Place", "N_Value",nCompanyId, dLayer, connection));
                    N_decimalPlace=N_decimalPlace==0?2:N_decimalPlace;
                    bool CheckClosedYear = Convert.ToBoolean(dLayer.ExecuteScalar("Select B_YearEndProcess From Acc_FnYear Where N_CompanyID=" + nCompanyId + " and N_FnYearID = " + nFnYearId, Params, connection));
                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and (Memo like '%" + xSearchkey + "%' or [Customer Name] like '%" + xSearchkey + "%' or cast(DATE as VarChar) like '%" + xSearchkey + "%')";

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_PayReceiptId desc";
                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "customerName":
                                xSortBy = "[Customer Name] " + xSortBy.Split(" ")[1];
                                break;
                            case "receiptNo":
                                xSortBy = "N_PayReceiptId " + xSortBy.Split(" ")[1];
                                break;
                            case "date":
                                xSortBy = "Cast(DATE as DateTime )" + xSortBy.Split(" ")[1];
                                break;
                            case "amount":
                                xSortBy = "Cast(REPLACE(Amount,',','') as Numeric(10,"+N_decimalPlace+")) " + xSortBy.Split(" ")[1];
                                break;        
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }
                    if (CheckClosedYear == false)
                    {
                        if (bAllBranchData == true)
                        {
                            Searchkey = Searchkey + " and  N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and B_YearEndProcess=0 and (X_type='SR' OR X_type='SA')";
                        }
                        else
                        {
                            Searchkey = Searchkey + " and  N_CompanyID=" + nCompanyId + " and N_BranchID=" + nBranchID + " and N_FnYearID=" + nFnYearId + " and B_YearEndProcess =0 and (X_type='SR' OR X_type='SA') ";
                        }
                    }

                    else
                    {
                        if (bAllBranchData == true)
                        {
                            Searchkey = Searchkey + " and  N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and (X_type='SR' OR X_type='SA') ";

                        }
                        else
                        {
                            Searchkey = Searchkey + " and  N_CompanyID=" + nCompanyId + " and N_BranchID=" + nBranchID + " and N_FnYearID=" + nFnYearId + " and (X_type='SR' OR X_type='SA') ";
                        }
                    }

                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvReceipt_Search where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and B_YearEndProcess =0 and (X_type='SR' OR X_type='SA') " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvReceipt_Search where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and B_YearEndProcess =0 and (X_type='SR' OR X_type='SA') " + Searchkey + " and n_PayReceiptId not in (select top(" + Count + ") n_PayReceiptId from vw_InvReceipt_Search where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId  and B_YearEndProcess =0  and (X_type='SR' OR X_type='SA') " + xSortBy + " ) " + xSortBy;

                    Params.Add("@nCompanyId", nCompanyId);
                    Params.Add("@nFnYearId", nFnYearId);
                    SortedList OutPut = new SortedList();


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count,sum(Cast(REPLACE(Amount,',','') as Numeric(10,"+N_decimalPlace+")) ) as TotalAmount from vw_InvReceipt_Search where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and B_YearEndProcess =0 and (X_type='SR' OR X_type='SA') " + Searchkey + "";
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

        [HttpGet("details")]
        public ActionResult GetSalesReceiptDetails(int nCustomerId, int nFnYearId, int nBranchId, string xInvoiceNo, bool bShowAllbranch, string dTransDate, string xTransType)
        {
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            SortedList general = new SortedList();
            SortedList OutPut = new SortedList();

            OutPut.Add("totalAmtDue", 0);
            OutPut.Add("totalBalance", 0);
            OutPut.Add("txnStarted", false);
            DataSet ds = new DataSet();
            int nCompanyId = myFunctions.GetCompanyID(User);
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int n_PayReceiptId = 0;// myFunctions.getIntVAL(MasterTable.Rows[0]["n_PayReceiptId"].ToString());

                    if (xInvoiceNo != null && xInvoiceNo !="@Auto")
                    {
                        SortedList proParams1 = new SortedList(){
                                {"@nCompanyID",nCompanyId},
                                {"@xInvoiceNo",xInvoiceNo},
                                {"@nFnYearID",nFnYearId},
                                {"@nBranchID",nBranchId}};
                        string sql = "";
                        if (bShowAllbranch == true)
                            sql = "select N_PayReceiptId,X_Type,N_PartyID,D_Date from Inv_PayReceipt where N_CompanyID=@nCompanyID and X_VoucherNo=@xInvoiceNo and N_FnYearID=@nFnYearID and N_BranchID=@nBranchID";
                        else
                            sql = "select N_PayReceiptId,X_Type,N_PartyID,D_Date from Inv_PayReceipt where N_CompanyID=@nCompanyID and X_VoucherNo=@xInvoiceNo and N_FnYearID=@nFnYearID";

                        DataTable PayInfo = dLayer.ExecuteDataTable(sql, proParams1, connection);
                        if (PayInfo.Rows.Count > 0)
                        {
                            n_PayReceiptId = myFunctions.getIntVAL(PayInfo.Rows[0]["N_PayReceiptId"].ToString());
                            xTransType = PayInfo.Rows[0]["X_Type"].ToString();
                            nCustomerId = myFunctions.getIntVAL(PayInfo.Rows[0]["N_PartyID"].ToString());
                            dTransDate = myFunctions.getDateVAL(Convert.ToDateTime(PayInfo.Rows[0]["D_Date"].ToString()));
                        }


                        SortedList mParamsList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"X_VoucherNo",xInvoiceNo},
                        {"N_FnYearID",nFnYearId},
                        {"N_BranchId",nBranchId},
                        {"X_Type",xTransType}
                    };
                        MasterTable = dLayer.ExecuteDataTablePro("SP_InvSalesReceipt_Disp", mParamsList, connection);
                        MasterTable = api.Format(MasterTable, "Master");

                    }


                    string balanceSql = "";
                    if (bShowAllbranch == true)
                        balanceSql = "SELECT  ISNULL( Sum(n_Amount),0)  as N_BalanceAmount from  vw_InvCustomerStatement Where N_AccType=@AccType and N_AccID=@CustomerID and N_CompanyID=@CompanyID and  D_TransDate<=@TransDate and B_IsSaveDraft = 0";
                    else
                        balanceSql = "SELECT  ISNULL( Sum(n_Amount),0)  as N_BalanceAmount from  vw_InvCustomerStatement Where N_AccType=@AccType and N_AccID=@CustomerID and N_CompanyID=@CompanyID and  D_TransDate<=@TransDate and N_BranchId=@BranchID  and B_IsSaveDraft = 0";
                    SortedList balanceParams = new SortedList();

                    balanceParams.Add("@CustomerID", nCustomerId);
                    balanceParams.Add("@AccType", 2);
                    balanceParams.Add("@CompanyID", nCompanyId);
                    balanceParams.Add("@TransDate", dTransDate);
                    balanceParams.Add("@BranchID", nBranchId);
                    object balance = dLayer.ExecuteScalar(balanceSql, balanceParams, connection);

                    string balanceAmt = "0";

                    OutPut["totalAmtDue"] = myFunctions.getIntVAL(Math.Round(Convert.ToDouble(balance)).ToString()).ToString(myFunctions.decimalPlaceString(2));
                    if (myFunctions.getIntVAL(Math.Round(Convert.ToDouble(balance)).ToString()) < 0)
                        OutPut["totalBalance"] = Convert.ToDouble(-1 * myFunctions.getIntVAL(Math.Round(Convert.ToDouble(balance)).ToString()));
                    else if (myFunctions.getIntVAL(Math.Round(Convert.ToDouble(balance)).ToString()) > 0)
                        OutPut["totalBalance"] = myFunctions.getIntVAL(Math.Round(Convert.ToDouble(balance)).ToString());
                    else
                        OutPut["totalBalance"] = 0;


                    if (n_PayReceiptId > 0)
                    {
                        if (xTransType == "SA")
                        {
                            string DetailSql = "";
                            if (bShowAllbranch == true)
                            {
                                DetailSql = "SELECT        Inv_PayReceiptDetails.N_CompanyID, Inv_PayReceiptDetails.N_InventoryId, Inv_PayReceiptDetails.N_Amount + Inv_PayReceiptDetails.N_DiscountAmt + ISNULL(Inv_PayReceiptDetails.N_AmtPaidFromAdvance, 0) AS N_Amount,Inv_PayReceiptDetails.N_AmountF + Inv_PayReceiptDetails.N_DiscountAmtF + ISNULL(Inv_PayReceiptDetails.N_AmtPaidFromAdvanceF, 0) AS N_AmountF, Inv_PayReceiptDetails.X_Description, Inv_PayReceiptDetails.N_BranchID, Inv_CustomerProjects.X_ProjectName FROM Inv_PayReceiptDetails LEFT OUTER JOIN Inv_CustomerProjects ON Inv_PayReceiptDetails.N_ProjectID = Inv_CustomerProjects.N_ProjectID AND Inv_PayReceiptDetails.N_CompanyID = Inv_CustomerProjects.N_CompanyID " +
                                        " Where Inv_PayReceiptDetails.N_CompanyID =@CompanyID and Inv_PayReceiptDetails.N_PayReceiptId =@PayReceiptID";
                            }
                            else
                            {
                                DetailSql = "SELECT        Inv_PayReceiptDetails.N_CompanyID, Inv_PayReceiptDetails.N_InventoryId, Inv_PayReceiptDetails.N_Amount + Inv_PayReceiptDetails.N_DiscountAmt + ISNULL(Inv_PayReceiptDetails.N_AmtPaidFromAdvance, 0) AS N_Amount,Inv_PayReceiptDetails.N_AmountF + Inv_PayReceiptDetails.N_DiscountAmtF + ISNULL(Inv_PayReceiptDetails.N_AmtPaidFromAdvanceF, 0) AS N_AmountF, Inv_PayReceiptDetails.X_Description, Inv_PayReceiptDetails.N_BranchID, Inv_CustomerProjects.X_ProjectName FROM Inv_PayReceiptDetails LEFT OUTER JOIN Inv_CustomerProjects ON Inv_PayReceiptDetails.N_ProjectID = Inv_CustomerProjects.N_ProjectID AND Inv_PayReceiptDetails.N_CompanyID = Inv_CustomerProjects.N_CompanyID " +
                                        " Where Inv_PayReceiptDetails.N_CompanyID =@CompanyID and Inv_PayReceiptDetails.N_PayReceiptId =@PayReceiptID and Inv_PayReceiptDetails.N_BranchID=@BranchID";
                            }
                            SortedList detailParams = new SortedList();
                            detailParams.Add("@CompanyID", nCompanyId);
                            detailParams.Add("@PayReceiptID", n_PayReceiptId);
                            detailParams.Add("@BranchID", nBranchId);
                            DetailTable = dLayer.ExecuteDataTable(DetailSql, detailParams, connection);
                            if (DetailTable.Rows.Count > 0)
                            {
                                DetailTable.AcceptChanges();

                                ds.Tables.Add(MasterTable);
                                ds.Tables.Add(DetailTable);
                                return Ok(api.Success(new SortedList() { { "details", api.Format(DetailTable) }, { "master", MasterTable }, { "masterData", OutPut } }));
                            }
                        }
                        else
                        {

                            SortedList detailParams = new SortedList()
                                {
                                    {"N_CompanyID",nCompanyId},
                                    {"N_CustomerId",nCustomerId},
                                    {"D_SalesDate",dTransDate},
                                    {"N_PayReceiptId",n_PayReceiptId},
                                    {"N_BranchID",nBranchId}
                                };
                            DetailTable = dLayer.ExecuteDataTablePro("SP_InvReceivables_Disp", detailParams, connection);
                        }
                    }
                    else
                    {
                        int branchFlag = 0;
                        if (bShowAllbranch) { branchFlag = 1; }
                        SortedList detailParams = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"N_CustomerId",nCustomerId},
                        {"D_SalesDate",dTransDate},
                        {"N_BranchFlag",branchFlag},
                        {"N_BranchID",nBranchId}
                    };
                        DetailTable = dLayer.ExecuteDataTablePro("SP_InvReceivables", detailParams, connection);
                    }
                    DetailTable = api.Format(DetailTable, "Details");



                    if (DetailTable.Rows.Count > 0)
                    {
                        double N_InvoiceDueAmt = 0, N_TotalDueAmt = 0;
                        foreach (DataRow dr in DetailTable.Rows)
                        {
                            if (myFunctions.getIntVAL(dr["N_SalesID"].ToString()) == 0)
                            {
                                // if (n_PayReceiptId > 0)
                                // {
                                //     txtAdvanceAmount.Text = myFunctions.getVAL(dr["N_Amount"].ToString()).ToString(myFunctions.decimalPlaceString(N_decimalPlace));
                                // }
                                dr.Delete();

                                continue;
                            }

                            N_InvoiceDueAmt = myFunctions.getVAL(dr["N_Amount"].ToString()) + myFunctions.getVAL(dr["N_BalanceAmount"].ToString()) + myFunctions.getVAL(dr["N_DiscountAmt"].ToString());
                            N_TotalDueAmt += N_InvoiceDueAmt;

                            if (N_InvoiceDueAmt == 0)
                            {
                                dr.Delete();
                            }
                        }
                    }

                    DetailTable.AcceptChanges();

                    ds.Tables.Add(MasterTable);
                    ds.Tables.Add(DetailTable);
                }
                return Ok(api.Success(new SortedList() { { "details", api.Format(DetailTable) }, { "master", MasterTable }, { "masterData", OutPut } }));
                //return Ok(api.Success(ds));
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }



        [HttpGet("Receivables")]
        public ActionResult GetPendingDetails(int? nCompanyId, int nCustomerId, int nFnYearId, int nBranchId, bool bAllBranchData, string dTransDate, string xType)
        {
            //DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            DataTable dtBalance = new DataTable();
            DataSet ds = new DataSet();
            SortedList balanceParams = new SortedList();
            SortedList ParamResult = new SortedList();
            try
            {
                // SortedList mParamsList = new SortedList()
                //     {
                //         {"N_CompanyID",nCompanyId},
                //         {"X_VoucherNo",xInvoiceNo},
                //         {"N_FnYearID",nFnYearId},
                //         {"N_BranchId",nBranchId},
                //         {"X_Type",xType}
                //     };
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // MasterTable = dLayer.ExecuteDataTablePro("SP_InvSalesReceipt_Disp", mParamsList, connection);
                    // if (MasterTable.Rows.Count == 0) { return Ok(api.Notice("No data found")); }
                    // MasterTable = api.Format(MasterTable, "Master");

                    string balanceSql = "";
                    if (bAllBranchData == true)
                        balanceSql = "SELECT  Sum(n_Amount)  as N_BalanceAmount from  vw_InvCustomerStatement Where N_AccType=@AccType and N_AccID=@CustomerID and N_CompanyID=@CompanyID and  D_TransDate<=@TransDate and B_IsSaveDraft = 0";
                    else
                        balanceSql = "SELECT  Sum(n_Amount)  as N_BalanceAmount from  vw_InvCustomerStatement Where N_AccType=@AccType and N_AccID=@CustomerID and N_CompanyID=@CompanyID and  D_TransDate<=@TransDate and N_BranchId=@BranchID  and B_IsSaveDraft = 0";

                    balanceParams.Add("@CustomerID", nCustomerId);
                    balanceParams.Add("@AccType", 2);
                    balanceParams.Add("@CompanyID", nCompanyId);
                    balanceParams.Add("@TransDate", dTransDate);
                    balanceParams.Add("@BranchID", nBranchId);

                    object balance = dLayer.ExecuteScalar(balanceSql, balanceParams, connection);
                    dtBalance = dLayer.ExecuteDataTable(balanceSql, balanceParams, connection);
                    string balanceAmt = "0.00";
                    if (myFunctions.getIntVAL(Math.Round(Convert.ToDouble(balance)).ToString()) < 0)
                    {
                        balanceAmt = Convert.ToDouble(-1 * myFunctions.getIntVAL(Math.Round(Convert.ToDouble(balance)).ToString())).ToString(myFunctions.decimalPlaceString(2));
                    }
                    else if (myFunctions.getIntVAL(Math.Round(Convert.ToDouble(balance)).ToString()) > 0)
                    {
                        balanceAmt = myFunctions.getIntVAL(Math.Round(Convert.ToDouble(balance)).ToString()).ToString(myFunctions.decimalPlaceString(2));
                    }


                    int branchFlag = 0;
                    if (bAllBranchData) { branchFlag = 1; }
                    SortedList detailParams = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"N_CustomerId",nCustomerId},
                        {"D_SalesDate",dTransDate},
                        {"N_BranchFlag",branchFlag},
                        {"N_BranchID",nBranchId}
                    };
                    DetailTable = dLayer.ExecuteDataTablePro("SP_InvReceivables", detailParams, connection);
                    if (DetailTable.Rows.Count == 0) { return Ok(api.Notice("No data found")); }



                    DetailTable = api.Format(DetailTable, "Details");
                    // dtBalance = api.Format(dtBalance, "Balance");

                    //ds.Tables.Add(MasterTable);
                    //ds.Tables.Add(dtBalance);
                    ds.Tables.Add(DetailTable);
                    ParamResult.Add("details", DetailTable);
                    if (balance != null)
                    {
                        //myFunctions.AddNewColumnToDataTable(DetailTable, "N_TotalBalance", typeof(double), balance);
                        ParamResult.Add("totalBalance", myFunctions.getVAL(balanceAmt));
                    }
                }
                //return Ok(api.Ok(ds));
                //return Ok(api.Success(ds));
                // Dictionary<SortedList, DataTable> res = new Dictionary<SortedList, DataTable>();
                // res.Add(balanceParams, DetailTable);
                //ParamResult.Add("details",DetailTable);
                return Ok(api.Success(ParamResult));
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    DataRow Master = MasterTable.Rows[0];
                    double nAmount = 0, nAmountF = 0;
                    var xVoucherNo = Master["x_VoucherNo"].ToString();
                    var xType = Master["x_Type"].ToString();
                    string xDesc = "";
                    int nCompanyId = myFunctions.getIntVAL(Master["n_CompanyID"].ToString());
                    int PayReceiptId = myFunctions.getIntVAL(Master["n_PayReceiptId"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(Master["n_FnYearID"].ToString());
                    int nBranchID = myFunctions.getIntVAL(Master["n_BranchID"].ToString());
                    nAmount = myFunctions.getVAL(Master["n_Amount"].ToString());
                    nAmountF = myFunctions.getVAL(Master["n_AmountF"].ToString());


                    if (!myFunctions.CheckActiveYearTransaction(nCompanyId, nFnYearID, DateTime.ParseExact(MasterTable.Rows[0]["D_Date"].ToString(),
                     "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture), dLayer, connection, transaction))
                    {
                        object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID="+nCompanyId+" and convert(date ,'" + MasterTable.Rows[0]["D_Date"].ToString() + "') between D_Start and D_End", connection, transaction);
                        if (DiffFnYearID != null)
                        {
                            MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
                            nFnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());
                            //QueryParams["@nFnYearID"] = nFnYearID;
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


                    SortedList InvCounterParams = new SortedList()
                    {
                        {"@MenuID",66},
                        {"@CompanyID",nCompanyId},
                        {"@FnYearID",nFnYearID}
                    };

                    bool B_AutoInvoice = Convert.ToBoolean(dLayer.ExecuteScalar("Select Isnull(B_AutoInvoiceEnabled,0) from Inv_InvoiceCounter where N_MenuID=@MenuID and N_CompanyID=@CompanyID and N_FnYearID=@FnYearID", InvCounterParams, connection, transaction));
                    if (!B_AutoInvoice) { transaction.Rollback(); return Ok(api.Warning("Auto Invoice Not Enabled")); }

                    if (PayReceiptId > 0)
                    {
                        SortedList deleteParams = new SortedList()
                            {
                                {"N_CompanyID",nCompanyId},
                                {"X_TransType",xType},
                                {"N_VoucherID",PayReceiptId}
                            };
                        dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", deleteParams, connection, transaction);
                    }

                    if (xVoucherNo == "@Auto")
                    {
                        SortedList Params = new SortedList();
                        Params.Add("N_CompanyID", nCompanyId);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", 66);
                        Params.Add("N_BranchID", nBranchID);

                        xVoucherNo = dLayer.GetAutoNumber("Inv_PayReceipt", "x_VoucherNo", Params, connection, transaction);
                        if (xVoucherNo == "") { transaction.Rollback(); return Ok(api.Warning("Unable to generate Receipt Number")); }

                        MasterTable.Rows[0]["x_VoucherNo"] = xVoucherNo;

                        // MasterTable.Columns.Remove("n_PayReceiptId");
                        // MasterTable.AcceptChanges();
                        // DetailTable.Columns.Remove("n_PayReceiptDetailsId");
                        // DetailTable.AcceptChanges();
                    }

                    PayReceiptId = dLayer.SaveData("Inv_PayReceipt", "n_PayReceiptId", MasterTable, connection, transaction);
                    if (PayReceiptId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable To Save Customer Payment"));
                    }
                    if (xType == "SA")
                    {


                        DetailTable.Clear();
                        DataRow row = DetailTable.NewRow();

                        if (DetailTable.Columns.Contains("N_CompanyID"))
                            row["N_CompanyID"] = nCompanyId;
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_CompanyID", typeof(int), nCompanyId);
                        if (DetailTable.Columns.Contains("N_PayReceiptId"))
                            row["N_PayReceiptId"] = PayReceiptId;
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_PayReceiptId", typeof(int), PayReceiptId);
                        if (DetailTable.Columns.Contains("N_InventoryId"))
                            row["N_InventoryId"] = PayReceiptId;
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "N_InventoryId", typeof(int), PayReceiptId);
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
                            row["X_TransType"] = xType;
                        else
                            myFunctions.AddNewColumnToDataTable(DetailTable, "X_TransType", typeof(string), xType);
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

                        DetailTable.Rows.Add(row);

                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_PayReceiptId"] = PayReceiptId;
                    }
                    int n_PayReceiptDetailsId = dLayer.SaveData("Inv_PayReceiptDetails", "n_PayReceiptDetailsId", DetailTable, connection, transaction);

                    if (PayReceiptId > 0)
                    {
                        SortedList PostingParams = new SortedList();
                        PostingParams.Add("N_CompanyID", nCompanyId);
                        PostingParams.Add("X_InventoryMode", xType);
                        PostingParams.Add("N_InternalID", PayReceiptId);
                        PostingParams.Add("N_UserID", myFunctions.GetUserID(User));
                        PostingParams.Add("X_SystemName", "ERP Cloud");
                        object posting = dLayer.ExecuteScalarPro("SP_Acc_InventoryPosting", PostingParams, connection, transaction);

                    }

                    transaction.Commit();
                    if (n_PayReceiptDetailsId > 0 && PayReceiptId > 0)
                    {

                        SortedList Result = new SortedList();
                        Result.Add("n_SalesReceiptID", PayReceiptId);
                        Result.Add("x_SalesReceiptNo", xVoucherNo);
                        return Ok(api.Success(Result, "Customer Payment Saved"));
                    }
                    else { return Ok(api.Error(User,"Unable To Save Customer Payment")); }
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        [HttpDelete()]
        public ActionResult DeleteData(int nPayReceiptId, string xType)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList deleteParams = new SortedList()
                            {
                                {"N_CompanyID",nCompanyId},
                                {"X_TransType",xType},
                                {"N_VoucherID",nPayReceiptId}
                            };
                    int result = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", deleteParams, connection, transaction);

                    if (result > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("Sales Receipt Deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                }

                return Ok(api.Success("Unable to Delete Sales Receipt "));

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }


        }

        [HttpGet("paymentType")]
        public ActionResult GetPaymentType()
        {
            string sqlCommandText = "select 'Invoice Payment' AS X_PaymentType,'SR' AS x_Type UNION select 'Advance Payment' AS X_PaymentType,'SA' AS x_Type";
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
                return StatusCode(403, api.Error(User,e));
            }
        }

        // [HttpGet("dummy")]
        // public ActionResult GetPurchaseInvoiceDummy(int? Id)
        // {
        //     try
        //     {
        //         string sqlCommandText = "select * from Inv_PayReceipt where n_PayReceiptId=@p1";
        //         SortedList mParamList = new SortedList() { { "@p1", Id } };
        //         DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList);
        //         masterTable = api.Format(masterTable, "master");

        //         string sqlCommandText2 = "select * from Inv_PayReceiptDetails where n_PayReceiptId=@p1";
        //         SortedList dParamList = new SortedList() { { "@p1", Id } };
        //         DataTable detailTable = dLayer.ExecuteDataTable(sqlCommandText2, dParamList);
        //         detailTable = api.Format(detailTable, "details");

        //         if (detailTable.Rows.Count == 0) { return Ok(new { }); }
        //         DataSet dataSet = new DataSet();
        //         dataSet.Tables.Add(masterTable);
        //         dataSet.Tables.Add(detailTable);

        //         return Ok(dataSet);

        //     }
        //     catch (Exception e)
        //     {
        //         return StatusCode(403, api.Error(User,e));
        //     }
        // }

    }
}

