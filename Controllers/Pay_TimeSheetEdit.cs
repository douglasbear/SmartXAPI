using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Net.Mail;
using System.Text;
using System.IO;
using System.Threading.Tasks;
namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("timeSheetEntry")]
    [ApiController]
    public class Pay_TimeSheetEdit : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        StringBuilder message = new StringBuilder();
        private readonly string reportPath;
        public Pay_TimeSheetEdit(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 793;
            reportPath = conf.GetConnectionString("ReportPath");

        }
        [HttpGet("list")]
        public ActionResult TimeSheetDashboardList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearId);

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ( X_BatchCode like '%" + xSearchkey + "%' or  X_PayrunText like '%" + xSearchkey + "%' or X_DateFrom like '%" + xSearchkey + "%' or X_DateTo like '%" + xSearchkey + "%' ) ";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_TimeSheetID desc";
            // xSortBy = " order by batch desc,D_TransDate desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ")  * from vw_TimeSheetEntry where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_TimeSheetEntry where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "and N_TimeSheetID not in (select top(" + Count + ") N_TimeSheetID from vw_TimeSheetEntry where N_CompanyID=@p1 ) " + Searchkey;


            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText + xSortBy, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_TimeSheetEntry where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return BadRequest(_api.Error(e));
            }
        }


        [HttpGet("deptList")]
        public ActionResult DepartmentList(int nFnYearID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);

            string sqlCommandText = "Select * from Pay_Department Where  and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and isnull(B_Inactive,0)<>1";
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

        [HttpGet("employeeDetails")]
        public ActionResult GetEmpDetails(int nFnYearID, int nEmpID, string xEmployeeCode, DateTime dtpSalaryFromdate, DateTime dtpSalaryToDate)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();

                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    DataTable ElementsTable = new DataTable();
                    string ElementSql = "";
                    ElementSql = " Select * from vw_TimesheetImport_Disp  Where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and D_Date >= '" + dtpSalaryFromdate + "' and D_Date<=' " + dtpSalaryToDate + "' and N_EmpID=" + nEmpID + " order by D_Date";
                    ElementsTable = dLayer.ExecuteDataTable(ElementSql, Params, connection);
                    if (ElementsTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    ElementsTable.AcceptChanges();
                    ElementsTable = _api.Format(ElementsTable);
                    dt.Tables.Add(ElementsTable);
                    return Ok(_api.Success(dt));

                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        [HttpGet("employeeList")]
        public ActionResult GetEmpList(int nFnYearID, DateTime dtpSalaryFromdate, DateTime dtpSalaryToDate, bool b_AllBranchData, int nBranchID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();

                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    DataTable EmpTable = new DataTable();
                    string empSql = "";


                    if (b_AllBranchData == true)
                        empSql = "select N_CompanyID,N_FnYearID,N_EmpId,X_EmpCode,X_EmpName,X_DepartMent,X_Position,N_PositionID,N_DepartmentID from vw_PayEmployee where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and D_HireDate<='" + dtpSalaryToDate + "' and ISNULL(D_TerminationDate,GETDATE())>='" + dtpSalaryFromdate + "'   order by X_EmpCode";
                    else
                        empSql = "select N_CompanyID,N_FnYearID,N_EmpId,X_EmpCode,X_EmpName,X_DepartMent,X_Position,N_PositionID,N_DepartmentID from vw_PayEmployee where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and D_HireDate<='" + dtpSalaryToDate + "' and N_BranchID=" + nBranchID + "  and ISNULL(D_TerminationDate,GETDATE())>='" + dtpSalaryFromdate + "'  order by X_EmpCode";

                    EmpTable = dLayer.ExecuteDataTable(empSql, Params, connection);
                    if (EmpTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    EmpTable.AcceptChanges();
                    EmpTable = _api.Format(EmpTable, "EmpTable");
                    dt.Tables.Add(EmpTable);
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
    }
}

