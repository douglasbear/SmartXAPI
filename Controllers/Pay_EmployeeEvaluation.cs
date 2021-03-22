using AutoMapper;
using SmartxAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
     [Route("employeeEvaluation")]
     [ApiController]
    public class PayEmployeeEvaluation : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
         private readonly IMyFunctions myFunctions;
         private readonly IApiFunctions _api;
          private readonly int N_FormID = 1068;
           public PayEmployeeEvaluation(IDataAccessLayer dl,IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
             _api = api;
            myFunctions=myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
           
        
        [HttpPost("Save")]
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
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();

                    int n_EvalID = myFunctions.getIntVAL(MasterRow["N_EvalID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string x_EvalCode = MasterRow["X_EvalCode"].ToString();

                    if (x_EvalCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", N_FormID);
                        x_EvalCode = dLayer.GetAutoNumber("Pay_EmpEvaluation", "X_EvalCode", Params, connection, transaction);
                        if (x_EvalCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Employee Evaluation");
                        }
                        MasterTable.Rows[0]["X_EvalCode"] = x_EvalCode;
                    }

                    n_EvalID = dLayer.SaveData("Pay_EmpEvaluation", "n_EvalID", "", "", MasterTable, connection, transaction);
                    if (n_EvalID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Employee Evaluation");
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_EvalID"] = n_EvalID;
                    }
                    int n_EvalDetailsID = dLayer.SaveData("Pay_EmpEvaluationDetails", "n_EvalDetailsID", DetailTable, connection, transaction);
                    if (n_EvalDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Employee Evaluation");
                    }

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_EvalID", n_EvalID);
                    Result.Add("X_EvalCode", x_EvalCode);
                    Result.Add("n_EvalDetailsID", n_EvalDetailsID);

                    return Ok(_api.Success(Result, " Employee Evaluation Created"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }



           }
}
    