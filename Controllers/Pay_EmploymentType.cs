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
    [Route("employmentType")]
    [ApiController]
    public class Pay_EmploymentType : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Pay_EmploymentType(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1272;
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
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
                int nEmploymentID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_EmploymentID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                string xEmploymentCode = MasterTable.Rows[0]["x_EmploymentCode"].ToString();
                 if (xEmploymentCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        xEmploymentCode = dLayer.GetAutoNumber("Pay_EmploymentType", "x_EmploymentCode", Params, connection, transaction);
                        if (xEmploymentCode == "") { return Ok(_api.Error("Unable to generate Employment Type Code")); }
                        MasterTable.Rows[0]["x_EmploymentCode"] = xEmploymentCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Pay_EmploymentType", "N_EmploymentID", nEmploymentID, "", connection, transaction);
                    }
                    
                    nEmploymentID=dLayer.SaveData("Pay_EmploymentType","N_EmploymentID",MasterTable,connection,transaction);  
                    transaction.Commit();
                    return Ok(_api.Success("Employment Type Saved")) ;
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nEmploymentID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Pay_EmploymentType", "N_EmploymentID", nEmploymentID, "", connection);
                    if (Results > 0)
                    {
                        return Ok( _api.Success("Employment Type deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete Employment Type"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

    }
}