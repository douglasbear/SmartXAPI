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
    [Route("employeeEndOfServiceSettings")]
    [ApiController]
    public class EmployeeEndOfServiceSettings : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID;
      


        public EmployeeEndOfServiceSettings(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 454;
        }

       
      [HttpGet("list")]
        public ActionResult EmployeeEndOfServiceList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select * from Pay_ServiceEnd where N_CompanyID=@nComapnyID";
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


         [HttpGet("TerminationType")]
        public ActionResult TerminationTypeList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select * from Pay_EmployeeStatus where N_Status in(2,3)";
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

                    int n_ServiceEndID = myFunctions.getIntVAL(MasterRow["N_ServiceEndID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string x_ServiceEndCode = MasterRow["X_ServiceEndCode"].ToString();
                    
                    if (n_ServiceEndID>0)
                    {
                         dLayer.DeleteData("Pay_ServiceEnd", "N_ServiceEndID", n_ServiceEndID, "", connection,transaction);
                         dLayer.DeleteData("Pay_ServiceEndSettings", "N_ServiceEndID", n_ServiceEndID, "", connection,transaction);

                    }
                    if (x_ServiceEndCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", N_FormID);
                        x_ServiceEndCode = dLayer.GetAutoNumber("Pay_ServiceEnd", "x_ServiceEndCode", Params, connection, transaction);
                        if (x_ServiceEndCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Code");
                        }
                        MasterTable.Rows[0]["X_ServiceEndCode"] = x_ServiceEndCode;
                    }

                    n_ServiceEndID = dLayer.SaveData("Pay_ServiceEnd", "n_ServiceEndID", "", "", MasterTable, connection, transaction);
                    if (n_ServiceEndID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save ");
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_ServiceEndID"] = n_ServiceEndID;
                        

                    }
                  
                     int n_EndSettiingsID = dLayer.SaveData("Pay_ServiceEndSettings", "n_EndSettiingsID", DetailTable, connection, transaction);
                     if (n_EndSettiingsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }lan

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_ServiceEndID", n_ServiceEndID);
                    Result.Add("x_ServiceEndCode", x_ServiceEndCode);
                    Result.Add("n_EndSettiingsID", n_EndSettiingsID);

                    return Ok(_api.Success(Result, "Employee End Of Service Settings Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }


         [HttpGet("details")]
        public ActionResult PayEndOfServiceSettings(string xServiceEndCode)
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
                     Params.Add("@xServiceEndCode", xServiceEndCode);
                    Mastersql = "select * from Pay_ServiceEnd where N_CompanyId=@nCompanyID and X_ServiceEndCode=@xServiceEndCode ";
                   
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int ServiceEndID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ServiceEndID"].ToString());
                    Params.Add("@nServiceEndID", ServiceEndID);

                    MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from vw_ServiceEndSettings where N_CompanyId=@nCompanyID and N_ServiceEndID=@nServiceEndID ";
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

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nServiceEndID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nServiceEndID", nServiceEndID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Pay_ServiceEnd", "n_ServiceEndID", nServiceEndID, "", connection);


                    if (Results > 0)
                    {
                        dLayer.DeleteData("Pay_ServiceEndSettings ", "n_ServiceEndID", nServiceEndID, "", connection);
                        return Ok(_api.Success("Employee End of Service settings deleted"));
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