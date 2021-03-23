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

         [HttpGet("details")]
        public ActionResult EmployeeEvaluation(string xEvalCode)
        {


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable DataTable = new DataTable();

                    string Mastersql = "";
                    string DetailSql = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@xEvalCode", xEvalCode);
                    Mastersql = "select * from vw_PayEvaluation_Details where N_CompanyId=@nCompanyID and X_EvalCode=@xEvalCode  ";
                   
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int EvaID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_EvalID"].ToString());
                    Params.Add("@nEvalID", EvaID);

                    MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from vw_PayEvaluation_Details where N_CompanyId=@nCompanyID and N_EvalID=@nEvalID ";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
            
            [HttpGet("List")]
        public ActionResult EmployeeEvaluationList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select N_CompanyID,N_EvalID,X_EvalCode,X_Description from vw_PayEmpEvauation_List where N_CompanyID=@nComapnyID";
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
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nEvalID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nEvalID", nEvalID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Pay_EmpEvaluation", "N_EvalID", nEvalID, "", connection);


                    if (Results > 0)
                    {
                        dLayer.DeleteData("Pay_EmpEvaluationDetails", "N_EvalID", nEvalID, "", connection);
                        return Ok(_api.Success("Employee Evaluation deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete"));
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
    