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
    [Route("addDeductBatchUpdate")]
    [ApiController]
    public class Pay_AddorDedBatchUpdate : ControllerBase
    {
         private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
         private readonly IMyAttachments myAttachments;
        private readonly int FormID = ;

        public Pay_AddorDedBatchUpdate(IApiFunctions apiFun, IDataAccessLayer dl, IMyFunctions myFun,IMyAttachments myAtt, IConfiguration conf)
        {
            api = apiFun;
            dLayer = dl;
            myAttachments = myAtt;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID= ;
        }
        //Save....
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {

            DataTable MasterTable;
            DataTable DetailTable;
             DataTable CustomerInfo;
            DataTable AddOrDedTable;
            DataTable AddOrDedDetailTable;
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            AddOrDedTable = ds.Tables["AddOrDed"];
            AddOrDedDetailTable = ds.Tables["AddOrDedDetails"];
            SortedList Params = new SortedList();
            String xButtonAction="";
            // Auto Gen
            try
            {

               
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    string X_AdditionBatchCode = "";
                    DataTable Attachment = ds.Tables["attachments"];
                    int N_AdditionBatchID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AdditionBatchID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                     string X_AdditionBatchCode =MasterTable.Rows[0]["X_AdditionBatchCode"].ToString();
                    int N_AdditionBatchID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AdditionBatchID"].ToString());
                    int N_UserID = myFunctions.GetUserID(User);
                    //int N_CustomerId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CustomerId"].ToString());
                    //CustomerInfo = dLayer.ExecuteDataTable("Select X_CustomerCode,X_CustomerName from Inv_Customer where N_CustomerID="+ N_CustomerId, Params, connection, transaction);

                    //  if (MasterTable.Columns.Contains("n_CustomerId"))
                    //   MasterTable.Columns.Remove("n_CustomerId");

                    var values = MasterTable.Rows[0]["X_AdditionBatchCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", FormID);
                        X_AdditionBatchCode = dLayer.GetAutoNumber("Pay_AddorDedBatchUpdate", "X_AdditionBatchCode", Params, connection, transaction);
                         xButtonAction="Insert"; 
                        if (X_AdditionBatchCode == "") { transaction.Rollback(); return Ok(api.Warning("Unable to generate Request Decision Number")); }
                        MasterTable.Rows[0]["X_AdditionBatchCode"] = X_AdditionBatchCode;
                    }

                    if (N_AdditionBatchID > 0)
                    {
                        dLayer.DeleteData("Pay_AddorDedBatchUpdateDetails", "N_AdditionBatchID", N_AdditionBatchID, "N_CompanyID=" + N_CompanyID + " and N_AdditionBatchID=" + N_AdditionBatchID, connection, transaction);
                        dLayer.DeleteData("Pay_AddorDedBatchUpdate", "N_AdditionBatchID", N_AdditionBatchID, "N_CompanyID=" + N_CompanyID + " and N_AdditionBatchID=" + N_AdditionBatchID, connection, transaction);
                        dLayer.DeleteData("Pay_MonthlyAddOrDedDetails", "N_TransDetailsID", N_TransDetailsID, "N_CompanyID=" + N_CompanyID + " and N_TransDetailsID=" + N_TransDetailsID, connection, transaction);
                        dLayer.DeleteData("Pay_MonthlyAddOrDed", "N_TransID", N_TransID,"N_CompanyID=" + N_CompanyID + " and N_TransID=" + N_TransID, connection, transaction);

                    
                    }
                    N_AdditionBatchID = dLayer.SaveData("Pay_AddorDedBatchUpdate", "N_AdditionBatchID", MasterTable, connection, transaction);
                   


                    //   if (CustomerInfo.Rows.Count > 0)
                    // {
                    //     try
                    //     {
                    //         myAttachments.SaveAttachment(dLayer, Attachment,X_AdditionBatchCode, N_AdditionBatchID, CustomerInfo.Rows[0]["X_CustomerName"].ToString().Trim(), CustomerInfo.Rows[0]["X_CustomerCode"].ToString(), N_CustomerId, "Customer Document", User, connection, transaction);
                    //     }
                    //     catch (Exception ex)
                    //     {
                    //         transaction.Rollback();
                    //         return Ok(api.Error(User, ex));
                    //     }
                    // }

                   xButtonAction="Update"; 
                    if (N_AdditionBatchID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_AdditionBatchID"] = N_AdditionBatchID;
                    }
            

            // Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(N_FnYearID,N_AdditionBatchID,X_AdditionBatchCode,   ,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                          
                   
                    int N_AdditionBatchID = dLayer.SaveData("Pay_AddorDedBatchUpdateDetails", "N_AdditionBatchID", DetailTable, connection, transaction);
                   
                    if (N_AdditionBatchID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }


                    //AdditionDedutionSave
                   // object obj = dLayer.ExecuteScalar(" select N_TransID from Pay_MonthlyAddOrDed where N_CompanyID=" + nCompanyID + " and N_PayrunID=" + myFunctions.getIntVAL(MasterTable.Rows[0]["N_BatchID"].ToString())+ "", connection, transaction);
                     N_AddOrDedID = 0;
                    // N_AddOrDedID = myFunctions.getIntVAL(obj.ToString());
                    int nTimesheetID = 0, nAddOrDedDetailID = 0;
                    nAddOrDedDetailID = dLayer.SaveData("Pay_MonthlyAddOrDedDetails", "N_TransDetailsID", AddOrDedDetailTable, connection, transaction);
                    N_AddOrDedID = dLayer.SaveData("Pay_MonthlyAddOrDed", "N_TransID", AddOrDedTable, connection, transaction);



                    transaction.Commit();

                    SortedList Result = new SortedList();
                    Result.Add("N_AdditionBatchID", N_AdditionBatchID);
                    Result.Add("X_AdditionBatchCode", X_AdditionBatchCode);
                    return Ok(api.Success(Result, "Request Saved"));
                }
                
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }



        [HttpDelete("delete")]
        public ActionResult DeleteData(int nAdditionBranchID,int nFnYearID,int nCustomerId)
        {
            int Results = 0;
             SortedList ParamList=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            //  DataTable TransData = new DataTable();
            //  DataRow TransRow = TransData.Rows[0];
            //  int N_AdditionBatchID = myFunctions.getIntVAL(TransRow["N_AdditionBatchID"].ToString());
             SortedList Params = new SortedList();
             ParamList.Add("@nFnYearID",nFnYearID);
              string xButtonAction="Delete";
              String X_AdditionBatchCode="";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    
              object n_FnYearID = dLayer.ExecuteScalar("select N_FnyearID from Pay_AddorDedBatchUpdate where N_AdditionBatchID =" + nAdditionBranchID + " and N_CompanyID=" + nCompanyID, Params, connection,transaction);
                    //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    myFunctions.LogScreenActivitys(myFunctions.getIntVAL( n_FnYearID.ToString()),nAdditionBranchID,X_AdditionBatchCode,   ,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                   
                    dLayer.DeleteData("Pay_AddorDedBatchUpdateDetails", "N_AdditionBatchID", nAdditionBranchID, "N_CompanyID=" + nCompanyID + " and N_AdditionBatchID=" + nAdditionBranchID, connection, transaction);
                    Results = dLayer.DeleteData("Pay_AddorDedBatchUpdate", "N_AdditionBatchID", nAdditionBranchID, "N_CompanyID=" + nCompanyID + " and N_AdditionBatchID=" + nAdditionBranchID, connection, transaction);

                    if (Results > 0)
                    {

                         myAttachments.DeleteAttachment(dLayer, 1, nAdditionBranchID,  nFnYearID, this.FormID, User, transaction, connection);
                        transaction.Commit();
                        return Ok(api.Success("Batch deleted"));
                    } 
                    else
                    {
                        transaction.Rollback();
                        return Ok(api.Warning("Unable to delete Request"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }


        }

    }

}    