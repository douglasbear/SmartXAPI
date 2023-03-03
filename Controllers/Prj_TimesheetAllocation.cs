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
    [Route("prjTimesheetAllocation")]
    [ApiController]
    public class Prj_TimesheetAllocation : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID;


        public Prj_TimesheetAllocation(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 202;
        }

       

       [HttpGet("list")]
        public ActionResult PrjTimesheetAllocationList(int nFnYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
           
            int nCompanyId = myFunctions.GetCompanyID(User);
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (N_TimesheetID like'%" + xSearchkey + "%'or N_Hours like'%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_TimesheetID desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Prj_TimeSheetMaster_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Prj_TimeSheetMaster_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and N_TimesheetID not in (select top(" + Count + ") N_TimesheetID from vw_Prj_TimeSheetMaster_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " +  xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearID);
            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(1) as N_Count  from vw_Prj_TimeSheetMaster_Disp where N_CompanyID=@p1 and N_FnYearID=@p2";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);

                        return Ok(_api.Success(OutPut));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        //Save....
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

                    int n_TimesheetID = myFunctions.getIntVAL(MasterRow["N_TimesheetID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string x_TimesheetCode = MasterRow["X_TimesheetCode"].ToString();
                    
                    if (n_TimesheetID>0)
                    {
                         dLayer.DeleteData("Prj_TimeSheet", "N_TimesheetID", n_TimesheetID, "", connection,transaction);
                         dLayer.DeleteData("Prj_TimeSheetMaster", "N_TimesheetID", n_TimesheetID, "", connection,transaction);

                    }
                    if (x_TimesheetCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", N_FormID);
                        x_TimesheetCode = dLayer.GetAutoNumber("Prj_TimeSheetMaster", "X_TimesheetCode", Params, connection, transaction);
                        if (x_TimesheetCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Code");
                        }
                        MasterTable.Rows[0]["X_TimesheetCode"] = x_TimesheetCode;
                    }

                    n_TimesheetID = dLayer.SaveData("Prj_TimeSheetMaster", "n_TimesheetID", "", "", MasterTable, connection, transaction);
                    if (n_TimesheetID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save ");
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_TimesheetID"] = n_TimesheetID;
                    }
                    int n_TimesheetDetailID = dLayer.SaveData("Prj_TimeSheet", "n_TimesheetDetailID", DetailTable, connection, transaction);
                    if (n_TimesheetDetailID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }


                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_TimesheetID", n_TimesheetID);
                    Result.Add("x_TimesheetCode", x_TimesheetCode);
                    Result.Add("n_TimesheetDetailID", n_TimesheetDetailID);

                    return Ok(_api.Success(Result, "Project Cost Allocation Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("details")]
        public ActionResult PayEmployeApprovalCode(int xTimesheetCode, int nFnYearID,int nProjectID)
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
                    Params.Add("@xTimesheetCode", xTimesheetCode);
                    Params.Add("@nFnYearID", nFnYearID);
                    Mastersql = "select * from vw_Prj_TimeSheetMaster_Disp where N_CompanyId=@nCompanyID and X_TimesheetCode=@xTimesheetCode and N_FnYearID=@nFnYearID";
                   
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    // if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                   
                    MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from  vw_Prj_TimeSheet_Disp where N_CompanyId=@nCompanyID and X_TimesheetCode=@xTimesheetCode and N_FnYearID=@nFnYearID";
                    if(nProjectID>0){
                        DetailSql = "select N_CompanyID, N_TimesheetID, X_ProjectName, N_ProjectID, 0 as N_TimesheetDetailID, N_EmpID, 0 as N_TimesheetHours, 0 as N_Amount, Sum(N_Hours) as N_Hours,X_EmpName, N_FnYearID,X_EmpCode from vw_Prj_TimeSheet where N_ProjectID="+nProjectID+" group by N_CompanyID, N_TimesheetID, X_ProjectName, N_ProjectID, N_EmpID, X_EmpName, N_FnYearID, X_EmpCode ";
                    }
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nTimesheetID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nTimesheetID", nTimesheetID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Prj_TimeSheetMaster", "N_TimesheetID", nTimesheetID, "", connection);


                    if (Results > 0)
                    {
                        dLayer.DeleteData("Prj_TimeSheet", "N_TimesheetID", nTimesheetID, "", connection);
                        return Ok(_api.Success("Deleted Sucessfully"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete"));
                    }

                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }


        }


    }
}
