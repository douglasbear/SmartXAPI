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
    [Route("employeeClearanceSettings")]
    [ApiController]
    public class EmployeeClearanceSettings : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID;
      


        public EmployeeClearanceSettings(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 1062;
        }

        //    [HttpGet("list")]
        // public ActionResult PayAccruedList(int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        // {
        //     int nCompanyId=myFunctions.GetCompanyID(User);
        //     DataTable dt = new DataTable();
        //     SortedList Params = new SortedList();
        //     int Count= (nPage - 1) * nSizeperpage;
        //     string sqlCommandText ="";
        //     string Searchkey = "";
        //     if (xSearchkey != null && xSearchkey.Trim() != "")
        //         Searchkey = "and (x_VacCode like '%" + xSearchkey + "%'or x_VacType like'%" + xSearchkey + "%' or x_Type like '%" + xSearchkey + "%' or x_Period like '%" + xSearchkey + "%' or x_Description like '%" + xSearchkey + "%' )";

        //     if (xSortBy == null || xSortBy.Trim() == "")
        //         xSortBy = " order by N_VacTypeID desc";
        //     else
        //         xSortBy = " order by " + xSortBy;
             
        //      if(Count==0)
        //         sqlCommandText = "select top("+ nSizeperpage +") X_VacCode,X_VacType,X_Type,X_Period,X_Description from vw_PayVacationType where N_CompanyID=@p1 " + Searchkey + " " + xSortBy;
        //     else
        //         sqlCommandText = "select top("+ nSizeperpage +") X_VacCode,X_VacType,X_Type,X_Period,X_Description,N_VacTypeID from vw_PayVacationType where N_CompanyID=@p1 " + Searchkey + " and N_VacTypeID not in (select top("+ Count +") N_VacTypeID from vw_PayVacationType where N_CompanyID=@p1 "+Searchkey + xSortBy + " ) " + xSortBy;
        //     Params.Add("@p1", nCompanyId);

        //     SortedList OutPut = new SortedList();


        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

        //             string sqlCommandCount = "select count(*) as N_Count  from vw_PayVacationType where N_CompanyID=@p1 ";
        //             object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
        //             OutPut.Add("Details", api.Format(dt));
        //             OutPut.Add("TotalCount", TotalCount);
        //             if (dt.Rows.Count == 0)
        //             {
        //                 return Ok(api.Warning("No Results Found"));
        //             }
        //             else
        //             {
        //                 return Ok(api.Success(OutPut));
        //             }

        //         }
                
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(api.Error(e));
        //     }
        // }
 
      [HttpGet("list")]
        public ActionResult EmployeeClearanceList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select * from from vw_Pay_EmployeeClearanceSettingsDetails where N_CompanyID=@nComapnyID";
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

                    int n_ClearanceSettingsID = myFunctions.getIntVAL(MasterRow["N_ClearanceSettingsID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string x_ClearanceCode = MasterRow["X_ClearanceCode"].ToString();

                    if (x_ClearanceCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", N_FormID);
                        x_ClearanceCode = dLayer.GetAutoNumber("Pay_EmployeeClearanceSettings", "x_ClearanceCode", Params, connection, transaction);
                        if (x_ClearanceCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Clearance Code");
                        }
                        MasterTable.Rows[0]["x_ClearanceCode"] = x_ClearanceCode;
                    }
                    MasterTable.Columns.Remove("N_FnYearID");

                    n_ClearanceSettingsID = dLayer.SaveData("Pay_EmployeeClearanceSettings", "n_ClearanceSettingsID", "", "", MasterTable, connection, transaction);
                    if (n_ClearanceSettingsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Clearance Code");
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_ClearanceSettingsID"] = n_ClearanceSettingsID;
                    }
                    int n_ClearanceSettingsDetailsID = dLayer.SaveData("Pay_EmployeeClearanceSettingsDetails", "n_ClearanceSettingsDetailsID", DetailTable, connection, transaction);
                    if (n_ClearanceSettingsDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Clearance Code");
                    }


                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_ClearanceSettingsID", n_ClearanceSettingsID);
                    Result.Add("x_ClearanceCode", x_ClearanceCode);
                    Result.Add("n_ClearanceSettingsDetailsID", n_ClearanceSettingsDetailsID);

                    return Ok(_api.Success(Result, "Clearance Code Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

         [HttpGet("details")]
        public ActionResult PayEmployeApprovalCode(string xClearanceCode)
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
                     Params.Add("@xClearanceCode", xClearanceCode);
                    Mastersql = "select * from Pay_EmployeeClearanceSettings where N_CompanyId=@nCompanyID and X_ClearanceCode=@xClearanceCode ";
                   
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int ClearanceSettingsID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ClearanceSettingsID"].ToString());
                    Params.Add("@nClearanceSettingsID", ClearanceSettingsID);

                    MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from vw_Pay_EmployeeClearanceSettingsDetails where N_CompanyId=@nCompanyID and N_ClearanceSettingsID=@nClearanceSettingsID ";
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
        public ActionResult DeleteData(int nClearanceSettingsID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nClearanceSettingsID", nClearanceSettingsID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Pay_EmployeeClearanceSettings", "n_ClearanceSettingsID", nClearanceSettingsID, "", connection);


                    if (Results > 0)
                    {
                        dLayer.DeleteData("Pay_EmployeeClearanceSettingsDetails", "n_ClearanceSettingsID", nClearanceSettingsID, "", connection);
                        return Ok(_api.Success("Employee Clearance settings deleted"));
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