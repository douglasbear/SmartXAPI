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


        public Inv_VendorPayment(IDataAccessLayer dl, IApiFunctions apiFun, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");

        }


        [HttpGet("list")]
        public ActionResult GetVendorPayment(int? nCompanyId, int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_InvPayment_Search where N_CompanyID=@p1 and N_FnYearID=@p2 order by D_Date DESC,[Memo]";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

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
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(dt);
                }
            }
            catch (Exception e)
            {
                return StatusCode(403, api.Error(e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetVendorPaymentDetails(int? nCompanyId, int nQuotationId, int nFnYearId)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_InvSalesQuotationNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_QuotationID=@p3";

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", nQuotationId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable Quotation = new DataTable();

                    Quotation = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    Quotation = api.Format(Quotation, "Master");
                    dt.Tables.Add(Quotation);

                    //Quotation Details

                    string sqlCommandText2 = "select * from vw_InvQuotationDetails where N_CompanyID=@p1 and N_FnYearID=@p2 and N_QuotationID=@p3";

                    DataTable QuotationDetails = new DataTable();
                    QuotationDetails = dLayer.ExecuteDataTable(sqlCommandText2, Params, connection);
                    QuotationDetails = api.Format(QuotationDetails, "Details");
                    dt.Tables.Add(QuotationDetails);
                }
                return Ok(dt);
            }
            catch (Exception e)
            {
                return StatusCode(403, api.Error(e));
            }
        }
        [HttpGet("payDetails")]
        public ActionResult GetVendorPayDetails(int nVendorID, int nFnYearId, string dTransDate, int nBranchID,bool bShaowAllbranch, int nPayReceiptID, string xTransType)
        {
            DataSet OutPut = new DataSet();
                    DataTable PayReceipt = new DataTable();

            string sql = "";
            int AllBranch = 0;
            int nCompanyId = myFunctions.GetCompanyID(User);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (bShaowAllbranch == true)
                        sql = "SELECT  -1 * Sum(n_Amount)  as N_BalanceAmount from  vw_InvVendorStatement Where N_AccType=1 and isnull(N_PaymentMethod,0)<>1 and N_AccID=@nVendorID and N_CompanyID=@nCompanyID and  D_TransDate<=@dTransDate";
                    else
                        sql = "SELECT  -1 * Sum(n_Amount)  as N_BalanceAmount from  vw_InvVendorStatement Where N_AccType=1 and isnull(N_PaymentMethod,0)<>1 and N_AccID=@nVendorID and N_CompanyID=@nCompanyID and N_BranchId=@nBranchID and  D_TransDate<=@dTransDate";
                    SortedList paramList = new SortedList();
                    paramList.Add("@nVendorID", nVendorID);
                    paramList.Add("@dTransDate", dTransDate);
                    paramList.Add("@nBranchID", nBranchID);
                    paramList.Add("@nPayReceiptID", nPayReceiptID);
                    paramList.Add("@xTransType", xTransType);
                    paramList.Add("@nCompanyID",nCompanyId);
                    DataTable VendorBalance = dLayer.ExecuteDataTable(sql, paramList, connection);
                    if (bShaowAllbranch == true)
                    {
                        AllBranch = 1;
                        nBranchID = 0;
                    }
                    SortedList proParams1 = new SortedList(){
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
                            PayReceipt = dLayer.ExecuteDataTablePro("SP_Inv_InvPayReceipt_View", proParams1, connection);
                            if (PayReceipt.Rows.Count > 0)
                            {
                                object obj = dLayer.ExecuteScalar("Select isnull(Count(N_InventoryId),0) as CountExists from Inv_PayReceiptDetails where N_CompanyID=@nCompanyID and N_InventoryId<>N_PayReceiptId and N_InventoryId=@nPayReceiptID and X_TransType =@xTransType", paramList, connection);
                                if (obj != null)
                                {
                                    if (myFunctions.getIntVAL(obj.ToString()) > 0)
                                    {
                                        return Ok(api.Notice("Transaction started."));
                                    }
                                }
                                return Ok(api.Notice(""));
                            }
                        }
                        else
                        {
                            PayReceipt = dLayer.ExecuteDataTablePro("SP_Inv_InvPayReceipt_View", proParams1, connection);
                        }
                    }
                    else
                    {
                        PayReceipt = dLayer.ExecuteDataTablePro("SP_Inv_InvPayReceipt_View", proParams1, connection);
                    }

                }
                return Ok(api.Format(PayReceipt,"details"));
            }
            catch (Exception e)
            {
                return BadRequest( api.Error(e));
            }
        }
    }



    }