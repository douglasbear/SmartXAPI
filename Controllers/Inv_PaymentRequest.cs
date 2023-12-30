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
    [Route("paymentRequest")]
    [ApiController]
    public class Inv_PaymentRequest : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Inv_PaymentRequest(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1844;
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();

                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    int nPaymentRequestID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PaymentRequestID"].ToString());
                    string xPaymentRequestCode = MasterTable.Rows[0]["x_PaymentRequestCode"].ToString();

                    if (xPaymentRequestCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", 1844);
                        xPaymentRequestCode = dLayer.GetAutoNumber("Inv_PaymentRequest", "x_PaymentRequestCode", Params, connection, transaction);
                        if (xPaymentRequestCode == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to generate Request Code"));
                        }
                        MasterTable.Rows[0]["x_PaymentRequestCode"] = xPaymentRequestCode;
                    }
                    MasterTable.AcceptChanges();
                    
                    nPaymentRequestID = dLayer.SaveData("Inv_PaymentRequest", "n_PaymentRequestID", MasterTable, connection, transaction);
                    if (nPaymentRequestID > 0){
                     if(!myFunctions.UpdateTxnStatus(nCompanyID, nPaymentRequestID, 1844, false, dLayer, connection, transaction)){
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Update Txn Status"));
                     }
                    }

                    if (nPaymentRequestID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Payment Request Saved Successfully"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPaymentRequestID)
        {
            int Results = 0;
            SortedList Params = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SqlTransaction transaction = connection.BeginTransaction();

                    Params.Add("@n_PaymentRequestID", nPaymentRequestID);
                    Results = dLayer.DeleteData("Inv_PaymentRequest", "n_PaymentRequestID", nPaymentRequestID, "", connection, transaction);
                    
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Payment Request Deleted Successfully"));
                    }
                    else
                    {
                       transaction.Rollback();
                       return Ok(_api.Error(User, "Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("details")]
        public ActionResult GetData(string xPaymentRequestCode,int nFnYearID)
        {
            try
            {
                string sqlCommandText = "select * from vw_Inv_PaymentRequest where N_CompanyID=@n_CompanyID and X_PaymentRequestCode=@x_PaymentRequestCode and N_FnYearID=@n_FnYearID ";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataSet paymentRequest = new DataSet();
                    DataTable dt = new DataTable();
                    DataTable MRNCode = new DataTable();
                    SortedList Params = new SortedList();

                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@n_CompanyID", nCompanyID);
                    Params.Add("@x_PaymentRequestCode", xPaymentRequestCode);
                    Params.Add("@n_FnYearID", nFnYearID);
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection,transaction);
                    int PRID = myFunctions.getIntVAL(dt.Rows[0]["N_PaymentRequestID"].ToString());
                    int PayTowardsID = myFunctions.getIntVAL(dt.Rows[0]["N_PayTowardsID"].ToString());
                    int POrderID = myFunctions.getIntVAL(dt.Rows[0]["N_POrderID"].ToString());
                    int DivisionID = myFunctions.getIntVAL(dt.Rows[0]["N_DivisionID"].ToString());
                    object N_Result = 0; 
                    if(PayTowardsID == 469 || PayTowardsID == 470 )  //Sadad Payment or Petty Cash
                    {
                        N_Result = dLayer.ExecuteScalar("Select COUNT(*) from Acc_VoucherMaster where N_PaymentRequestID ='" + PRID + "' and N_CompanyID= " + nCompanyID + " and X_TransType ='PV' and N_FnYearID=" + nFnYearID, connection, transaction);
                    }
                    if(PayTowardsID == 471) //vendor advance
                    {
                        N_Result = dLayer.ExecuteScalar("Select COUNT(*) from Inv_PayReceipt where N_PaymentRequestID ='" + PRID + "' and N_CompanyID= " + nCompanyID + " and X_Type='PA' and N_FnYearID=" + nFnYearID, connection, transaction);
                    }
                    if(PayTowardsID == 472) //vendor payment
                    {
                        N_Result = dLayer.ExecuteScalar("Select COUNT(*) from Inv_Purchase where N_PaymentRequestID ='" + PRID + "' and N_CompanyID= " + nCompanyID + " and N_DivisionID="+DivisionID+"and N_FnYearID=" + nFnYearID, connection, transaction);
                        string MRNID = dt.Rows[0]["X_MRN"].ToString();
                        string MRNCodeList = "Select N_MRNID,X_MRNNo,D_MRNDate,N_CompanyID,N_VendorID,X_VendorInvoice from Inv_MRN where N_MRNID in ("+MRNID+")";
                        MRNCode = dLayer.ExecuteDataTable(MRNCodeList, Params, connection,transaction);
                        MRNCode = _api.Format(MRNCode, "MRNCode");
                        paymentRequest.Tables.Add(MRNCode);
                    }
                    if  (myFunctions.getIntVAL(N_Result.ToString()) > 0)
                    {
                        dt.Columns.Add("B_IsProcessed");
                        dt.Rows[0]["B_IsProcessed"] = true;
                    }
                   
                    dt = _api.Format(dt, "Master");

                    if (dt.Rows.Count == 0){
                        transaction.Rollback();
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else{
                        transaction.Commit();
                        paymentRequest.Tables.Add(dt);
                        return Ok(_api.Success(paymentRequest));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        [HttpGet("PendingPOList")]
        public ActionResult paymentRequestPOList(int nVendorID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable(); 
            SortedList Params = new SortedList();
            string sqlCommandText ;
            sqlCommandText = "select X_POrderNo,n_POrderID from Inv_PurchaseOrder where N_CompanyID="+nCompanyID+"and N_VendorID="+nVendorID+"and N_StatusID=1";  
            Params.Add("@p1", nCompanyID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok( _api.Error(User,e));
            }
        }

    }
}