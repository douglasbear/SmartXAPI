using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("performanceRating")]
    [ApiController]

    public class Pay_PerformanceRating : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Pay_PerformanceRating(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1443;

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
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                     DetailTable = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nPerformanceID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PerformanceID"].ToString());
                    string X_Code = MasterTable.Rows[0]["X_Code"].ToString();
      

                    if (X_Code == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", this.FormID);

                        
                        
                        X_Code = dLayer.GetAutoNumber("Pay_Performance", "X_Code", Params, connection, transaction);
                        if (X_Code == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to generate  Code"));
                        }
                        MasterTable.Rows[0]["X_Code"] = X_Code;
                    }

                    if (nPerformanceID > 0)
                    {
                        dLayer.DeleteData("Pay_Performance", "n_PerformanceID", nPerformanceID, "N_CompanyID=" + nCompanyID + " and n_PerformanceID=" + nPerformanceID, connection, transaction);
                        dLayer.DeleteData("Pay_PerformanceDetails", "n_PerformanceID", nPerformanceID, "N_CompanyID=" + nCompanyID + " and n_PerformanceID=" + nPerformanceID, connection, transaction);
                    }

                     nPerformanceID = dLayer.SaveData("Pay_Performance", "n_PerformanceID", MasterTable, connection, transaction);

                    if (nPerformanceID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                     for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_PerformanceID"] = nPerformanceID;
                    }

                    int N_PerformanceDetailsID = dLayer.SaveData("Pay_PerformanceDetails", "N_PerformanceDetailsID", DetailTable, connection, transaction);
                    if (N_PerformanceDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));

                    }

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("N_PerformanceDetailsID", nPerformanceID);
                    Result.Add("x_Code", X_Code);
                    return Ok(_api.Success(Result, "Saved successfully"));

                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
    }
}