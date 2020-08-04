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
    [Route("voucher")]
    [ApiController]
    public class Acc_PaymentVoucher : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Acc_PaymentVoucher(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("list")]
        public ActionResult GetPaymentVoucherList(int? nCompanyId, int nFnYearId, string voucherType)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_AccVoucher_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 and X_TransType=@p3";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", voucherType);

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
                    return StatusCode(200, _api.Response(200, "No Results Found"));
                }
                else
                {
                    return Ok(dt);
                }
            }
            catch (Exception e)
            {
                return StatusCode(404, _api.Response(404, e.Message));
            }
        }
        [HttpGet("details")]
        public ActionResult GetVoucherDetails(int? nCompanyId, int nFnYearId, string xVoucherNo, string xTransType)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();

            string sqlCommandText = "Select * from Acc_VoucherMaster left outer join Acc_MastLedger on Acc_VoucherMaster.N_DefLedgerID = Acc_MastLedger.N_LedgerID and Acc_VoucherMaster.N_FnYearID=Acc_MastLedger.N_FnYearID and Acc_VoucherMaster.N_CompanyID=Acc_MastLedger.N_CompanyID    Where  Acc_VoucherMaster.X_VoucherNo=@VoucherNo and Acc_VoucherMaster.N_CompanyID=@CompanyID and X_TransType=@TransType  AND Acc_VoucherMaster.N_FnYearID =@FnYearID Order By D_VoucherDate";
            Params.Add("@CompanyID", nCompanyId);
            Params.Add("@FnYearID", nFnYearId);
            Params.Add("@VoucherNo", xVoucherNo);
            Params.Add("@TransType", xTransType);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable Voucher = new DataTable();
                    Voucher = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    Voucher = _api.Format(Voucher, "Master");
                    dt.Tables.Add(Voucher);
                    int nVoucherID = myFunctions.getIntVAL(Voucher.Rows[0]["N_VoucherID"].ToString());
                    Params.Add("@VoucherID", nVoucherID);

                    string sqlCommandText2 = "Select Acc_VoucherMaster_Details.*,Acc_MastLedger.*,Acc_MastGroup.X_Type,Acc_TaxCategory.X_DisplayName,Acc_TaxCategory.N_Amount,Acc_CashFlowCategory.X_Description AS X_TypeCategory from Acc_VoucherMaster_Details inner join Acc_MastLedger on Acc_VoucherMaster_Details.N_LedgerID = Acc_MastLedger.N_LedgerID and Acc_VoucherMaster_Details.N_CompanyID=Acc_MastLedger.N_CompanyID inner join Acc_MastGroup On Acc_MastLedger.N_GroupID=Acc_MastGroup.N_GroupID and Acc_MastLedger.N_CompanyID=Acc_MastGroup.N_CompanyID  and Acc_MastLedger.N_FnYearID=Acc_MastGroup.N_FnYearID  LEFT OUTER JOIN Acc_TaxCategory ON Acc_VoucherMaster_Details.N_TaxCategoryID1 = Acc_TaxCategory.N_PkeyID LEFT OUTER JOIN Acc_CashFlowCategory on Acc_VoucherMaster_Details.N_TypeID=Acc_CashFlowCategory.N_CategoryID Where N_VoucherID =@VoucherID and Acc_VoucherMaster_Details.N_CompanyID=@CompanyID and Acc_MastGroup.N_FnYearID=@FnYearID";
                    DataTable VoucherDetails = new DataTable();
                    VoucherDetails = dLayer.ExecuteDataTable(sqlCommandText2, Params, connection);
                    VoucherDetails = _api.Format(VoucherDetails, "Details");
                    dt.Tables.Add(VoucherDetails);
                }
                return Ok(dt);

            }
            catch (Exception e)
            {
                return StatusCode(404, _api.Response(404, e.Message));
            }
        }

        //Save....
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
                // Auto Gen
                string InvoiceNo = "";
                DataRow masterRow = MasterTable.Rows[0];
                var values = masterRow["x_VoucherNo"].ToString();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
                        Params.Add("N_YearID", masterRow["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 80);
                        Params.Add("N_BranchID", masterRow["n_BranchId"].ToString());
                        InvoiceNo = dLayer.GetAutoNumber("Acc_VoucherMaster", "x_VoucherNo", Params, connection, transaction);
                        if (InvoiceNo == "") { return StatusCode(409, _api.Response(409, "Unable to generate Invoice Number")); }
                        MasterTable.Rows[0]["x_VoucherNo"] = InvoiceNo;
                    }

                    int N_VoucherId = dLayer.SaveData("Acc_VoucherMaster", "N_VoucherId", 0, MasterTable, connection, transaction);
                    if (N_VoucherId <= 0)
                    {
                        transaction.Rollback();
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_VoucherId"] = N_VoucherId;
                    }
                    int N_InvoiceDetailId = dLayer.SaveData("Acc_VoucherMaster_Details", "N_VoucherDetailsID", 0, DetailTable, connection, transaction);
                    transaction.Commit();
                }
                return Ok("Data Saved");
            }
            catch (Exception ex)
            {
                return StatusCode(403, ex);
            }
        }
        //Delete....
        [HttpDelete()]
        public ActionResult DeleteData(int N_VoucherID)
        {
            int Results = 0;
            try
            {
                dLayer.setTransaction();
                Results = dLayer.DeleteData("Inv_SalesVoucher", "n_quotationID", N_VoucherID, "");
                if (Results <= 0)
                {
                    dLayer.rollBack();
                    return StatusCode(409, _api.Response(409, "Unable to delete sales quotation"));
                }
                else
                {
                    dLayer.DeleteData("Inv_SalesVoucherDetails", "n_quotationID", N_VoucherID, "");
                }

                if (Results > 0)
                {
                    dLayer.commit();
                    return StatusCode(200, _api.Response(200, "Sales quotation deleted"));
                }
                else
                {
                    dLayer.rollBack();
                    return StatusCode(409, _api.Response(409, "Unable to delete sales quotation"));
                }

            }
            catch (Exception ex)
            {
                return StatusCode(404, _api.Response(404, ex.Message));
            }


        }

    }
}