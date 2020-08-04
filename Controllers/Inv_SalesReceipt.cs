using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

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
        public ActionResult GetSalesReceipt(int? nCompanyId, int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_InvReceipt_Search where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId";
            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nFnYearId", nFnYearId);

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
                    return Ok(api.NotFound("Sales Receipt Not Found"));
                }
                else
                {
                    return Ok(dt);
                }
            }
            catch (Exception e)
            {
                return BadRequest(api.ErrorResponse(e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetSalesReceiptDetails(int? nCompanyId, int nFnYearId, int nBranchId, string xInvoiceNo, bool bAllBranchData, string dTransDate)
        {
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                SortedList mParamsList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"X_VoucherNo",xInvoiceNo},
                        {"N_FnYearID",nFnYearId},
                        {"N_BranchId",nBranchId}
                    };
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTablePro("SP_InvSalesReceipt_Disp", mParamsList, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(api.NotFound("No data found")); }
                    MasterTable = api.Format(MasterTable, "Master");

                    string balanceSql = "";
                    if (bAllBranchData == true)
                        balanceSql = "SELECT  Sum(n_Amount)  as N_BalanceAmount from  vw_InvCustomerStatement Where N_AccType=@AccType and N_AccID=@CustomerID and N_CompanyID=@CompanyID and  D_TransDate<=@TransDate and B_IsSaveDraft = 0";
                    else
                        balanceSql = "SELECT  Sum(n_Amount)  as N_BalanceAmount from  vw_InvCustomerStatement Where N_AccType=@AccType and N_AccID=@CustomerID and N_CompanyID=@CompanyID and  D_TransDate<=@TransDate and N_BranchId=@BranchID  and B_IsSaveDraft = 0";
                    SortedList balanceParams = new SortedList();
                    string CustomerID = MasterTable.Rows[0]["n_PartyID"].ToString();
                    string x_Type = MasterTable.Rows[0]["x_Type"].ToString();
                    int n_PayReceiptId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PayReceiptId"].ToString());
                    balanceParams.Add("@CustomerID", CustomerID);
                    balanceParams.Add("@AccType", 2);
                    balanceParams.Add("@CompanyID", nCompanyId);
                    balanceParams.Add("@TransDate", dTransDate);
                    balanceParams.Add("@BranchID", nBranchId);
                    object balance = dLayer.ExecuteScalar(balanceSql, balanceParams, connection);
                    string balanceAmt = "0.00";
                    if (myFunctions.getIntVAL(Math.Round(Convert.ToDouble(balance)).ToString()) < 0)
                    {
                        balanceAmt = Convert.ToDouble(-1 * myFunctions.getIntVAL(Math.Round(Convert.ToDouble(balance)).ToString())).ToString(myFunctions.decimalPlaceString(2));
                    }
                    else if (myFunctions.getIntVAL(Math.Round(Convert.ToDouble(balance)).ToString()) > 0)
                    {
                        balanceAmt = myFunctions.getIntVAL(Math.Round(Convert.ToDouble(balance)).ToString()).ToString(myFunctions.decimalPlaceString(2));
                    }

                    if (n_PayReceiptId > 0)
                    {
                        if (x_Type == "SA")
                        {
                            string DetailSql = "";
                            if (bAllBranchData == true)
                            {
                                DetailSql = "Select Inv_PayReceiptDetails.N_CompanyID,N_InventoryId,N_Amount+N_DiscountAmt+Isnull(N_AmtPaidFromAdvance,0) AS N_Amount,X_Description,N_BranchID  from Inv_PayReceiptDetails " +
                                        " Where N_CompanyID =@CompanyID and N_PayReceiptId =@PayReceiptID";
                            }
                            else
                            {
                                DetailSql = "Select Inv_PayReceiptDetails.N_CompanyID,N_InventoryId,N_Amount+N_DiscountAmt+Isnull(N_AmtPaidFromAdvance,0) AS N_Amount,X_Description,N_BranchID  from Inv_PayReceiptDetails " +
                                        " Where N_CompanyID =@CompanyID and N_PayReceiptId =@PayReceiptID and N_BranchID=@BranchID";
                            }
                            SortedList detailParams = new SortedList();
                            detailParams.Add("@CompanyID", nCompanyId);
                            detailParams.Add("@PayReceiptID", n_PayReceiptId);
                            detailParams.Add("@BranchID", nBranchId);
                            DetailTable = dLayer.ExecuteDataTable(DetailSql, detailParams, connection);
                            if (DetailTable.Rows.Count == 0) { return Ok(api.NotFound("No data found")); }
                        }
                        else
                        {

                            SortedList detailParams = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"N_CustomerId",CustomerID},
                        {"D_SalesDate",dTransDate},
                        {"N_PayReceiptId",n_PayReceiptId},
                        {"N_BranchID",nBranchId}
                    };
                            DetailTable = dLayer.ExecuteDataTablePro("SP_InvReceivables_Disp", detailParams, connection);
                            if (DetailTable.Rows.Count == 0) { return Ok(api.NotFound("No data found")); }
                        }
                    }
                    else
                    {
                        int branchFlag = 0;
                        if (bAllBranchData) { branchFlag = 1; }
                        SortedList detailParams = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"N_CustomerId",CustomerID},
                        {"D_SalesDate",dTransDate},
                        {"N_BranchFlag",branchFlag},
                        {"N_BranchID",nBranchId}
                    };
                        DetailTable = dLayer.ExecuteDataTablePro("SP_InvReceivables", detailParams, connection);
                        if (DetailTable.Rows.Count == 0) { return Ok(api.NotFound("No data found")); }
                    }
                    DetailTable = api.Format(DetailTable, "Details");

                    ds.Tables.Add(MasterTable);
                    ds.Tables.Add(DetailTable);
                }
                //return Ok(api.Ok(ds));
                return Ok(ds);
            }
            catch (Exception e)
            {
                return BadRequest(api.ErrorResponse(e));
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

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;


                    // Auto Gen
                    string PorderNo = "";
                    var values = MasterTable.Rows[0]["x_POrderNo"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    int nCompanyId = myFunctions.getIntVAL(Master["n_CompanyId"].ToString());

                    int N_POrderID = myFunctions.getIntVAL(Master["n_POrderID"].ToString());

                    if (Master["n_POTypeID"].ToString() == null || myFunctions.getIntVAL(Master["n_POTypeID"].ToString()) == 0)
                        MasterTable.Rows[0]["n_POTypeID"] = 174;

                    if (Master["n_POType"].ToString() == null || myFunctions.getIntVAL(Master["n_POType"].ToString()) == 0)
                        MasterTable.Rows[0]["n_POType"] = 121;

                    transaction = connection.BeginTransaction();

                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyId);
                        Params.Add("N_YearID", Master["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 80);
                        Params.Add("N_BranchID", Master["n_BranchId"].ToString());

                        PorderNo = dLayer.GetAutoNumber("Inv_PurchaseOrder", "x_POrderNo", Params, connection, transaction);
                        if (PorderNo == "") { return StatusCode(409, api.Response(409, "Unable to generate Quotation Number")); }
                        MasterTable.Rows[0]["x_POrderNo"] = PorderNo;

                        MasterTable.Columns.Remove("n_POrderID");
                        MasterTable.AcceptChanges();
                        DetailTable.Columns.Remove("n_POrderDetailsID");
                        DetailTable.AcceptChanges();
                    }
                    else
                    {
                        SortedList AdvParams = new SortedList();
                        AdvParams.Add("@companyId", Master["n_CompanyId"].ToString());
                        AdvParams.Add("@PorderId", Master["n_POrderID"].ToString());
                        object AdvancePRProcessed = dLayer.ExecuteScalar("Select COUNT(N_TransID) From Inv_PaymentRequest Where  N_CompanyID=@companyId and N_TransID=@PorderId and N_FormID=82", AdvParams, connection, transaction);
                        if (AdvancePRProcessed != null)
                        {
                            if (myFunctions.getIntVAL(AdvancePRProcessed.ToString()) > 0)
                            {
                                transaction.Rollback();
                                return StatusCode(400, "Payment Request Processed");
                            }
                        }


                        if (N_POrderID > 0)
                        {
                            MasterTable.Columns.Remove("n_POrderID");
                            MasterTable.AcceptChanges();
                            DetailTable.Columns.Remove("n_POrderDetailsID");
                            DetailTable.AcceptChanges();

                            bool B_PRSVisible = false;
                            bool MaterailRequestVisible = myFunctions.CheckPermission(nCompanyId, 556, "Administrator", dLayer, connection, transaction);
                            bool PurchaseRequestVisible = myFunctions.CheckPermission(nCompanyId, 1049, "Administrator", dLayer, connection, transaction);

                            if (MaterailRequestVisible || PurchaseRequestVisible)
                                B_PRSVisible = true;

                           
                            SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyId},
                                {"X_TransType","Purchase Order"},
                                {"N_VoucherID",N_POrderID}};
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                        }
                    }


                    int N_PurchaseOrderId = dLayer.SaveData("Inv_PurchaseOrder", "n_POrderID", N_POrderID, MasterTable, connection, transaction);
                    if (N_PurchaseOrderId <= 0)
                    {
                        transaction.Rollback();
                        return StatusCode(403, "Error");
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_POrderID"] = N_PurchaseOrderId;
                    }
                    int N_PurchaseOrderDetailId = dLayer.SaveData("Inv_PurchaseOrderDetails", "n_POrderDetailsID", 0, DetailTable, connection, transaction);
                    transaction.Commit();
                }
                return Ok("Purchase Order Saved");
            }
            catch (Exception ex)
            {
                return StatusCode(403, ex);
            }
        }

    }
}