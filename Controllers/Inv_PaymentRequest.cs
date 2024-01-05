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
                    DataTable Approvals;
                    MasterTable = ds.Tables["master"];
                     Approvals = ds.Tables["approval"];
                     DataTable Attachment = ds.Tables["attachments"];
                    SortedList Params = new SortedList();
                    DataRow ApprovalRow = Approvals.Rows[0];

                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    int nPaymentRequestID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PaymentRequestID"].ToString());
                    string xPaymentRequestCode = MasterTable.Rows[0]["x_PaymentRequestCode"].ToString();
                    int N_NextApproverID = 0;
                    int N_SaveDraft = 0;
                    int  N_ProcStatus =0;
                    object IsSaveDraft="0";

                       if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()) && nPaymentRequestID>0)
                     {                                     
                        int N_PkeyID = nPaymentRequestID;
                        var value = MasterTable.Rows[0]["x_PaymentRequestCode"].ToString();
                        string X_Criteria = "n_PaymentRequestID=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                        myFunctions.UpdateApproverEntry(Approvals, "Inv_PaymentRequest", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "Payment Request", N_PkeyID, value, 1, "", 0, "", 0, User, dLayer, connection, transaction);
                        // N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_PurchaseOrder where n_POrderID=" + N_POrderID + " and N_CompanyID=" + nCompanyId + " and N_FnYearID=" + N_FnYearID, connection, transaction).ToString());
                          transaction.Commit();
                        return Ok(_api.Success("Payment Request Approved " + "-" + xPaymentRequestCode));
                    }

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
                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);  
                    MasterTable.Rows[0]["N_UserID"] = myFunctions.GetUserID(User);
                    
                    nPaymentRequestID = dLayer.SaveData("Inv_PaymentRequest", "n_PaymentRequestID", MasterTable, connection, transaction);
                    if (nPaymentRequestID > 0){
                     if(!myFunctions.UpdateTxnStatus(nCompanyID, nPaymentRequestID, 1844, false, dLayer, connection, transaction)){
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Update Txn Status"));
                     }
                    }

                    N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "Payment Request", nPaymentRequestID, xPaymentRequestCode, 1, "", 0, "",0, User, dLayer, connection, transaction);
                    if (nPaymentRequestID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("Unable to save"));
                    }
                    else
                    // {   try
                    //     {
                    //         myAttachments.SaveAttachment(dLayer, Attachment, MasterTable.Rows[0]["X_CustomerCode"].ToString() + "-" + MasterTable.Rows[0]["X_CustomerName"].ToString(), 0, MasterTable.Rows[0]["X_CustomerName"].ToString(), MasterTable.Rows[0]["X_CustomerCode"].ToString(), nCustomerID, "Customer Document", User, connection, transaction);
                    //     }
                    //     catch (Exception ex)
                    //     {
                    //         transaction.Rollback();
                    //         return Ok(api.Error(User, ex));
                    //     }
                        transaction.Commit();
                        return Ok(_api.Success("Payment Request Saved Successfully"));
                    }
                }
            
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPaymentRequestID,int nFnYearID,string comments)
        {
            int Results = 0;
            SortedList Params = new SortedList();
              if (comments == null)
            {
                comments = "";
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    int nCompanyID = myFunctions.GetCompanyID(User);

                     Params.Add("@n_PaymentRequestID", nPaymentRequestID);
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nFnYearID", nFnYearID);


                    string Sql = "select n_PaymentRequestID,x_PaymentRequestCode,N_UserID,N_ProcStatus,N_ApprovalLevelId from Inv_PaymentRequest where n_PaymentRequestID=@n_PaymentRequestID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID";
                   TransData = dLayer.ExecuteDataTable(Sql, Params, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];
                   DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, 1844, nPaymentRequestID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID, 0, 0, User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), comments);
                    SqlTransaction transaction = connection.BeginTransaction();

                    string X_Criteria = "n_PaymentRequestID=" + nPaymentRequestID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                    string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());
                    
                     string status = myFunctions.UpdateApprovals(Approvals, nFnYearID, "Payment Request", nPaymentRequestID, TransRow["x_PaymentRequestCode"].ToString(), ProcStatus, "Inv_PaymentRequest", X_Criteria, "", User, dLayer, connection, transaction);

                 if (status != "Error")
                    {
                    if (ButtonTag == "6" || ButtonTag == "0")
                 {
                   
                    Results = dLayer.DeleteData("Inv_PaymentRequest", "n_PaymentRequestID", nPaymentRequestID, "", connection, transaction);
                 }
                    }
                     transaction.Commit();
                    return Ok(_api.Success("Payment Request " + status + " Successfully"));
                    
                    // if (Results > 0)
                    // {
                    //     transaction.Commit();
                    //     return Ok(_api.Success("Payment Request Deleted Successfully"));
                    // }
                    // else
                    // {
                    //    transaction.Rollback();
                    //    return Ok(_api.Error(User, "Unable to delete"));
                    // }
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
                        N_Result = dLayer.ExecuteScalar("Select COUNT(*) from Acc_VoucherMaster where N_PaymentRequestID ='" + PRID + "' and N_CompanyID= " + nCompanyID + " and X_TransType ='PV'", connection, transaction);
                    }
                    if(PayTowardsID == 471) //vendor advance
                    {
                        N_Result = dLayer.ExecuteScalar("Select COUNT(*) from Inv_PayReceipt where N_PaymentRequestID ='" + PRID + "' and N_CompanyID= " + nCompanyID + " and X_Type='PA'", connection, transaction);
                    }
                    if(PayTowardsID == 472) //vendor payment
                    {
                        N_Result = dLayer.ExecuteScalar("Select COUNT(*) from Inv_Purchase where N_PaymentRequestID ='" + PRID + "' and N_CompanyID= " + nCompanyID + " and N_DivisionID="+DivisionID, connection, transaction);
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
            if(nVendorID > 0){
                sqlCommandText = "select X_POrderNo,n_POrderID,N_VendorID,X_VendorName from vw_PendingPOList where N_CompanyID="+nCompanyID+"and N_VendorID="+nVendorID+"and N_StatusID=1";  
            }
            else {
                sqlCommandText = "select X_POrderNo,n_POrderID,N_VendorID,X_VendorName from vw_PendingPOList where N_CompanyID="+nCompanyID+"and N_StatusID=1";  
            }
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