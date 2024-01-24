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
using System.Net;
using System.IO;
namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("zatcaCSIDgenerator")]
    [ApiController]
    public class CSID_generator : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID = 1863;
        private readonly IApiFunctions api;

        public CSID_generator(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("GetAutoNo")]
        public ActionResult RefreshSerialNumber()
        {
            DataTable dt = new DataTable();
            string txt_serialNumber = "1-TST|2-TST|3-" + Guid.NewGuid().ToString();
            string input = txt_serialNumber.Trim();
            int index = input.LastIndexOf("|3-");

            if (index >= 0)
            {
                input = input.Substring(0, index + 3);
                string refreshedSerialNumber = input + Guid.NewGuid().ToString();
                dt.Columns.Add("SerialNo");
                DataRow row = dt.NewRow();
                row["SerialNo"] = (refreshedSerialNumber);
                dt.Rows.Add(row);
                dt = _api.Format(dt, "Master");
                return Ok(_api.Success(dt));
            }
            return BadRequest("Invalid input for serial number");
        }
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody]DataSet ds)

        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    SortedList Params = new SortedList();
                    MasterTable = ds.Tables["master"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int N_fnYearId = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                    Params.Add("@p1", N_CompanyID);
                    Params.Add("@p2", N_fnYearId);
                    string x_ZatcaName = MasterRow["X_ZatcaName"].ToString();
                    string X_ZatcaMode = MasterRow["x_ZatcaMode"].ToString();      
                    string X_ZatcaSerial = MasterRow["X_ZatcaSerial"].ToString();
                    string X_ZatcaInvoicetype = MasterRow["X_ZatcaInvoicetype"].ToString();
                    string N_ZatcaOTP = MasterRow["N_ZatcaOTP"].ToString();
                    Params.Add("@p3",X_ZatcaMode);
                    Params.Add("@p4",x_ZatcaName);
                    Params.Add("@p5",X_ZatcaSerial);
                    Params.Add("@p6",X_ZatcaInvoicetype);
                    Params.Add("@p7",N_ZatcaOTP);

                    object result = dLayer.ExecuteNonQuery("update Acc_Company set X_ZatcaMode=@p3,X_ZatcaName=@p4,X_ZatcaInvoicetype=@p6, X_ZatcaSerial=@p5 ,N_ZatcaOTP=@p7 where n_CompanyID=@p1  ", Params, connection, transaction);
                    if (myFunctions.getIntVAL(result.ToString()) > 0){
                        transaction.Commit();
                    }
                }
                return Ok(_api.Success(" Saved"));
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
    }
}