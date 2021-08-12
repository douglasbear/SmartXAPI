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
    [Route("timeSheetApproval")]
    [ApiController]
    public class Pay_TimeSheetApproval : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        StringBuilder message = new StringBuilder();
        private readonly string reportPath;
        public Pay_TimeSheetApproval(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 216;
            reportPath = conf.GetConnectionString("ReportPath");

        }
        [HttpGet("employeeList")]
        public ActionResult GetEmpList(int nFnYearID, bool b_AllBranchData, int nBranchID)
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
                        empSql = "select N_CompanyID,N_EmpID,N_BranchID,N_Status,N_FnYearID,N_CatagoryId,X_DefEmpCode,X_Position,X_Department,N_DepartmentID,N_PositionID,Name as X_EmpName,[Employee Code] as X_EmpCode from vw_PayEmployee_Disp where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and (N_Status = 0 OR N_Status = 1)   order by X_EmpCode";
                    else
                        empSql = "select N_CompanyID,N_EmpID,N_BranchID,N_Status,N_FnYearID,N_CatagoryId,X_DefEmpCode,X_Position,X_Department,N_DepartmentID,N_PositionID,Name as X_EmpName,[Employee Code] as X_EmpCode from vw_PayEmployee_Disp where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and (N_Status = 0 OR N_Status = 1)  order by X_EmpCode";

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

        // [HttpGet("employeeDetails")]
        // public ActionResult GetEmpDetails(int nFnYearID, int nEmpID, int nCategoryID, string payRunID, DateTime dtpFromdate, DateTime dtpTodate,string xPayrunText)
        // {
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             DataSet dt = new DataSet();
        //             SortedList Params = new SortedList();
        //             SortedList secParams = new SortedList();
        //             SortedList payParams = new SortedList();


        //             int nCompanyID = myFunctions.GetCompanyID(User);
        //             Params.Add("@nCompanyID", nCompanyID);
        //             DataTable ElementsTable = new DataTable();
        //             DataTable EmpGrpWorkhours = new DataTable();
        //             DataTable settingsTable = new DataTable();
        //             DataTable PayAttendence = new DataTable();
        //             DataTable PayOffDays = new DataTable();



        //             string ElementSql = "";
        //             int N_BatchID = 0;
        //             object obj = dLayer.ExecuteScalar("Select isnull(Count(X_BatchCode),0) from Pay_TimeSheetMaster where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_EmpID=" + nEmpID + " and N_BatchID=" + payRunID + "", Params, connection);
        //             if (obj != null)
        //             {
        //                 if (myFunctions.getIntVAL(obj.ToString()) > 0)
        //                 {
        //                     N_BatchID = myFunctions.getIntVAL(payRunID);
        //                     //ViewDetails();
        //                 }
        //                 else
        //                 {

        //                     {

        //                         // Get_Attendence();
        //                         if (nEmpID > 0)
        //                         {
        //                             string detailSql = "Select * From vw_EmpGrp_Workhours Where N_CompanyID = " + nCompanyID + " and N_PkeyId = " + nCategoryID + "";
        //                             EmpGrpWorkhours = dLayer.ExecuteDataTable(detailSql, Params, connection);
        //                             if (EmpGrpWorkhours.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
        //                             //(Row 0 check ["B_Compensation"] == false then extrahrs and noncompnsrs visible false )


        //                             //checksettings---------------------------------------------------------------------------------------
        //                             string Sql1 = "Select B_Addition,B_Deduction,B_Compensation from Pay_EmployeeGroup where N_CompanyID=" + nCompanyID + " and N_PkeyId=" + groupid;
        //                             settingsTable = dLayer.ExecuteDataTable(Sql1, Params, connection);
        //                             settingsTable.AcceptChanges();
        //                             settingsTable = _api.Format(settingsTable);
        //                             dt.Tables.Add(settingsTable);

        //                             // true or false 3 field check erp code ===>CategorywiseSettings for front end validation
        //                             //-----------------------------------------------------------------------------------------------------

        //                             secParams.Add("@nCompanyID", nCompanyID);
        //                             secParams.Add("@nFnYearID", nFnYearID);
        //                             secParams.Add("@dtpFromdate", dtpFromdate);
        //                             secParams.Add("@dtpTodate", dtpTodate);
        //                             secParams.Add("@N_EmpID", nEmpID);

        //                             string payAttendanceSql = "SP_Pay_TimeSheet @nCompanyID,@nFnYearID,@dtpFromdate,@dtpTodate,@N_EmpID";
        //                             PayAttendence = dLayer.ExecuteDataTable(payAttendanceSql, secParams, connection);

        //                             DateTime Date = dtpFromdate;

        //                             //----- // ----- Period Calender  //--- Isvacation -0-Normal ,1-leave,2 -Holidays or wkoff---------------------

        //                             //GetOffDays-------------------------------------------------------------------------------------------

        //                             string Sql3 = "Select * from vw_pay_OffDays Where N_CompanyID =" + nCompanyID + " and (N_FNyearID= " + nFnYearID + " or N_FNyearID=0)  ";
        //                             PayOffDays = dLayer.ExecuteDataTable(Sql3, secParams, connection);










        //                             //------------------------------------------------------------------------------------------------
        //                             {
        //                                 payParams.Add("@nCompanyID", nCompanyID);
        //                                 payParams.Add("@nFnYearID", nFnYearID);
        //                                 payParams.Add("@dtpFromdate", dtpFromdate);
        //                                 payParams.Add("@dtpTodate", dtpTodate);
        //                                 payParams.Add("@N_EmpID", nEmpID);


        //                                 string Sql = "SP_Pay_SelAddOrDed_Emp " + myCompanyID._CompanyID + "," + xPayrunText.Value.Year.ToString("00##") + dtpPayrunText.Value.Month.ToString("0#") + "," + myCompanyID._FnYearID + "," + N_EmpID;
        //                                 dba.FillDataSet(ref dsItemCategory, "Pay_Payrate", Sql, "TEXT", new DataTable());
        //                             }






















        //                         }

        //                     }
        //                 }
        //             }
