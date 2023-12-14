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
                            return Ok(_api.Error(User, "Unable to generate Budget Code"));
                        }
                        MasterTable.Rows[0]["x_PaymentRequestCode"] = xPaymentRequestCode;
                    }
                    MasterTable.AcceptChanges();
                    
                    nPaymentRequestID = dLayer.SaveData("Inv_PaymentRequest", "n_PaymentRequestID", MasterTable, connection, transaction);
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
        public ActionResult GetData(int nPaymentRequestID,int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from vw_Inv_PaymentRequest where N_CompanyID=@n_CompanyID and N_PaymentRequestID=@n_PaymentRequestID and N_FnYearID=@n_FnYearID ";
            Params.Add("@n_CompanyID", nCompanyID);
            Params.Add("@n_PaymentRequestID", nPaymentRequestID);
            Params.Add("@n_FnYearID", nFnYearID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt, "Master");

                if (dt.Rows.Count == 0){
                    return Ok(_api.Warning("No Results Found"));
                }
                else{
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

    }
}