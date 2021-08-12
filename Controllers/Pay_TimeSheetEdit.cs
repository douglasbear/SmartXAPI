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
        public ActionResult GetEmpDetails(DataTable dtTimesheet, int nFnYearID, int nEmpID, string xEmployeeCode, DateTime dtpSalaryFromdate, DateTime dtpSalaryToDate)
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
                    //if(dtTimesheet.Rows.Count)
                    ElementSql = " Select * from vw_TimesheetImport_Disp  Where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and D_Date >= '" + dtpSalaryFromdate + "' and D_Date<=' " + dtpSalaryToDate + "' and N_EmpID=" + nEmpID + " order by D_Date";
                    ElementsTable = dLayer.ExecuteDataTable(ElementSql, Params, connection);
                    if (ElementsTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    ElementsTable.AcceptChanges();

                    DateTime Date = dtpSalaryFromdate;
                    do
                    {
                        object objDatePresent= dLayer.ExecuteScalar("Select N_EmpID from vw_TimesheetImport_Disp  Where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and D_Date = '" + Date + "' and N_EmpID=" + nEmpID, Params, connection);
                        if(objDatePresent==null)
                        {
                            DataRow rowET = ElementsTable.NewRow();
                            rowET["D_Date"] = Date;

                            ElementsTable.Rows.Add(rowET);
                        }

                    }while (Date <= dtpSalaryToDate);

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

        // [HttpPost("save")]
        // public ActionResult SaveData([FromBody] DataSet ds)
        // {
        //     try
        //     {
        //         DataTable MasterTable;
        //         DataTable DetailTable;
        //         MasterTable = ds.Tables["master"];
        //         DetailTable = ds.Tables["details"];
        //         int N_SaveDraft = 0;
        //         int N_Status = 0;
        //         int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
        //         int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
        //         int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_BranchID"].ToString());
        //         int nTimesheetID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TimesheetID"].ToString());
        //         int nEmpID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_EmpID"].ToString());
        //         var dEndDate = MasterTable.Rows[0]["D_EndDate"].ToString();
        //         string xMethod = MasterTable.Rows[0]["X_Method"].ToString();
        //         int nEOSDetailID = 0;


        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();
        //             SortedList Params = new SortedList();
        //             SortedList QueryParams = new SortedList();

        //             // Auto Gen
        //             string X_BatchCode = "";
        //             var values = MasterTable.Rows[0]["X_BatchCode"].ToString();
        //             if (values == "@Auto")
        //             {
        //                 Params.Add("N_CompanyID", nCompanyID);
        //                 Params.Add("N_YearID", nFnYearId);
        //                 Params.Add("N_FormID", this.FormID);
        //                 Params.Add("N_BranchID", nBranchID);
        //                 X_BatchCode = dLayer.GetAutoNumber("Pay_TimeSheetEntry", "X_BatchCode", Params, connection, transaction);
        //                 if (X_BatchCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate timesheet entry Code")); }
        //                 MasterTable.Rows[0]["X_BatchCode"] = X_BatchCode;
        //             }
        //             string DupCriteria = "N_CompanyID=" + nCompanyID + " and X_BatchCode='" + X_BatchCode + "' and N_FnyearID=" + nFnYearId;
        //             nTimesheetID = dLayer.SaveData("Pay_TimeSheetEntry", "N_TimesheetID", DupCriteria, "", MasterTable, connection, transaction);
        //             if (nTimesheetID <= 0)
        //             {
        //                 transaction.Rollback();
        //                 return Ok(_api.Error("Unable to save"));
        //             }
        //             QueryParams.Add("@nCompanyID", nCompanyID);
        //             QueryParams.Add("@nFnYearID", nFnYearId);
        //             QueryParams.Add("@N_ServiceEndID", nServiceEndID);
        //             QueryParams.Add("@N_EmpID", nEmpID);
        //             QueryParams.Add("@X_Method", xMethod);
        //             object Savedraft = dLayer.ExecuteScalar("select CAST(B_IsSaveDraft as INT) from pay_EndOFService where N_CompanyID=@nCompanyID and N_ServiceEndID=@N_ServiceEndID", QueryParams, connection, transaction);
        //             if (Savedraft != null)
        //                 N_SaveDraft = myFunctions.getIntVAL(Savedraft.ToString());
        //             object Status = "3";// dLayer.ExecuteScalar("select N_Status  from Pay_EmployeeStatus where X_Description=@X_Method", QueryParams, connection, transaction);
        //             if (Status != null)
        //                 N_Status = myFunctions.getIntVAL(Status.ToString());

        //             object PositionID = dLayer.ExecuteScalar("select N_PositionID from vw_PayEmployee where N_CompanyID=@nCompanyID and N_EMPID=@N_EmpID", QueryParams, connection, transaction);

        //             if (N_SaveDraft == 0)
        //             {
        //                 dLayer.ExecuteNonQuery("Update Pay_Employee Set N_Status = " + N_Status + ",D_StatusDate='" + dEndDate.ToString() + "' Where N_CompanyID =" + nCompanyID + " And N_EmpID =" + nEmpID.ToString(), QueryParams, connection, transaction);
        //                 dLayer.ExecuteNonQuery("Update Pay_SuperVisor Set N_EmpID = 0  Where N_CompanyID =" + nCompanyID + " And N_PositionID =" + PositionID.ToString(), QueryParams, connection, transaction);
        //             }
        //             dLayer.DeleteData("pay_EndOfServiceSDetails", "N_ServiceEndID", nServiceEndID, "", connection, transaction);
        //             for (int j = 0; j < DetailTable.Rows.Count; j++)
        //             {
        //                 nEOSDetailID = dLayer.SaveData("pay_EndOfServiceSDetails", "N_EOSDetailID", DetailTable, connection, transaction);
        //             }
        //             transaction.Commit();
        //             return Ok(api.Success("Terminated"));
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(api.Error(ex));
        //     }
        // }

    }
}

