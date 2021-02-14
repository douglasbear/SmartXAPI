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
    [Route("timesheet")]
    [ApiController]



    public class Ess_TimeSheet : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1305;


        public Ess_TimeSheet(IDataAccessLayer dl, IApiFunctions apiFun, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        // [HttpGet("details")]
        // public ActionResult GetAttendanceDetails(string xPayRunID, int nEmployeeID)
        // {
        //     DataTable Master = new DataTable();
        //     DataTable Detail = new DataTable();
        //     DataSet ds = new DataSet();
        //     SortedList Params = new SortedList();
        //     SortedList QueryParams = new SortedList();

        //     int companyid = myFunctions.GetCompanyID(User);

        //     QueryParams.Add("@nCompanyID", companyid);
        //     QueryParams.Add("@xPayRunID", xPayRunID);
        //     QueryParams.Add("@nEmployeeID", nEmployeeID);
        //     string Condition = "";
        //     string _sqlQuery = "";
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             Condition = "Pay_TimeSheetMaster.N_EmpID=@nEmployeeID AND Pay_TimeSheetMaster.x_PayrunText=@xPayRunID";

        //             _sqlQuery = "SELECT  case when Pay_TimeSheet.n_TotalWorkHour > 0 then 'P' else 'A' end as X_Status,Pay_TimeSheetMaster.N_TimeSheetID, Pay_TimeSheetMaster.N_EmpID, Pay_TimeSheetMaster.X_PayrunText, Pay_TimeSheetMaster.D_DateFrom, Pay_TimeSheetMaster.D_DateTo, Pay_TimeSheetMaster.N_TotalDutyHours, Pay_TimeSheetMaster.N_TotalWorkedDays, Pay_TimeSheet.D_In,Pay_TimeSheet.D_Out, Pay_TimeSheet.D_Shift2_In, Pay_TimeSheet.D_Shift2_Out, Pay_TimeSheet.N_Status, Pay_TimeSheet.N_DutyHours,Pay_TimeSheet.N_diff,CONVERT(VARCHAR ,Pay_TimeSheet.D_Date, 106) as D_Date,round(Pay_TimeSheet.N_TotalWorkHour,2) as N_TotalWorkHour,* FROM Pay_TimeSheetMaster INNER JOIN Pay_TimeSheet ON Pay_TimeSheetMaster.N_TimeSheetID = Pay_TimeSheet.N_TimeSheetID AND Pay_TimeSheetMaster.N_CompanyID = Pay_TimeSheet.N_CompanyID Where " + Condition + "";

        //             Master = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

        //             Master = api.Format(Master, "master");
        //             if (Master.Rows.Count == 0)
        //             {
        //                 return Ok(api.Notice("No Results Found"));
        //             }
        //             else
        //             {
        //                 ds.Tables.Add(Master);
        //                 return Ok(api.Success(ds));
        //             }
        //         }

        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(api.Error(e));
        //     }
        // }

        [HttpGet("details")]
        public ActionResult GetAttendanceDetails(int nEmployeeID, int nFnYear, DateTime payDate, DateTime dDateFrom, DateTime dDateTo)
        {
            DataTable Details = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);





            // string Condition = "";
            // string _sqlQuery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Condition = "Pay_TimeSheetMaster.N_EmpID=@nEmployeeID AND Pay_TimeSheetMaster.x_PayrunText=@xPayRunID";

                    // _sqlQuery = "SELECT  case when Pay_TimeSheet.n_TotalWorkHour > 0 then 'P' else 'A' end as X_Status,Pay_TimeSheetMaster.N_TimeSheetID, Pay_TimeSheetMaster.N_EmpID, Pay_TimeSheetMaster.X_PayrunText, Pay_TimeSheetMaster.D_DateFrom, Pay_TimeSheetMaster.D_DateTo, Pay_TimeSheetMaster.N_TotalDutyHours, Pay_TimeSheetMaster.N_TotalWorkedDays, Pay_TimeSheet.D_In,Pay_TimeSheet.D_Out, Pay_TimeSheet.D_Shift2_In, Pay_TimeSheet.D_Shift2_Out, Pay_TimeSheet.N_Status, Pay_TimeSheet.N_DutyHours,Pay_TimeSheet.N_diff,CONVERT(VARCHAR ,Pay_TimeSheet.D_Date, 106) as D_Date,round(Pay_TimeSheet.N_TotalWorkHour,2) as N_TotalWorkHour,* FROM Pay_TimeSheetMaster INNER JOIN Pay_TimeSheet ON Pay_TimeSheetMaster.N_TimeSheetID = Pay_TimeSheet.N_TimeSheetID AND Pay_TimeSheetMaster.N_CompanyID = Pay_TimeSheet.N_CompanyID Where " + Condition + "";


                    object PeriodType = dLayer.ExecuteScalar("Select X_Value from Gen_Settings Where X_Description ='Period Settings' and N_CompanyID= " + companyid + " and X_Group='Payroll'", connection);
                    object Periodvalue = dLayer.ExecuteScalar("Select N_Value from Gen_Settings Where X_Description ='Period Settings' and N_CompanyID= " + companyid + " and X_Group='Payroll'", connection);
                    if (Periodvalue == null) return Ok(api.Notice("No Results Found"));
                    // DateTime fromDate=new DateTime();
                    // DateTime toDate=new DateTime();

                    DateTime fromDate = dDateFrom;
                    DateTime toDate = dDateTo;

                    DateTime dtStartDate = new DateTime(payDate.Year, payDate.Month, 1);

                    int days = 0;
                    // if (PeriodType != null && PeriodType.ToString() == "M")
                    // {
                    //     days = DateTime.DaysInMonth(payDate.Year, payDate.Month) - myFunctions.getIntVAL(Periodvalue.ToString());
                    //     toDate = dtStartDate.AddDays(myFunctions.getIntVAL(Periodvalue.ToString()) - 2);
                    //     int lastdays = myFunctions.getIntVAL(Periodvalue.ToString());
                    //     fromDate = dtStartDate.AddMonths(-1).AddDays(lastdays - 1);
                    // }
                    // else
                    // {
                    //     days = DateTime.DaysInMonth(payDate.Year, payDate.Month) - myFunctions.getIntVAL(Periodvalue.ToString());
                    //     toDate = dtStartDate.AddDays(myFunctions.getIntVAL(days.ToString()) - 1);
                    //     int lastdays = myFunctions.getIntVAL(Periodvalue.ToString());
                    //     fromDate = dtStartDate.AddDays(-lastdays); 
                    // }

                    QueryParams.Add("N_CompanyID", companyid);
                    QueryParams.Add("N_FnYear", nFnYear);
                    QueryParams.Add("D_DateFrom", fromDate);
                    QueryParams.Add("D_DateTo", toDate);
                    QueryParams.Add("N_EmpID", nEmployeeID);

                    SortedList OutPut = new SortedList();
                    SortedList Master = new SortedList();
                    Master.Add("fromDate", fromDate);
                    Master.Add("toDate", toDate);
                    Master.Add("days", days);
                    Details = dLayer.ExecuteDataTablePro("SP_Pay_TimeSheet", QueryParams, connection);
                    if (Details.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        Details = api.Format(Details, "master");
                        OutPut.Add("master", Master);
                        OutPut.Add("details", Details);
                        return Ok(api.Success(OutPut));
                    }
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nEmpID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_EmpID"].ToString());
                DataRow masterRow = MasterTable.Rows[0];

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    int nTimesheetID = 0;
                    string defultTime = "00:00:00";
                    string currentTime = DateTime.Now.ToString("HH:mm:ss");
            DateTime date = DateTime.Today;


                    string d_in = Convert.ToDateTime(masterRow["d_In"].ToString()).ToString("HH:mm:ss");
                    string d_out = Convert.ToDateTime(masterRow["d_Out"].ToString()).ToString("HH:mm:ss");
                    string d_Shift2_In = Convert.ToDateTime(masterRow["d_Shift2_In"].ToString()).ToString("HH:mm:ss");
                    string d_Shift2_Out = Convert.ToDateTime(masterRow["d_Shift2_Out"].ToString()).ToString("HH:mm:ss");
                    
                    if (d_in == defultTime)
                    {
                        masterRow["d_In"] = currentTime;
                    }
                    else if (d_out == defultTime)
                    {
                        masterRow["d_out"] = currentTime;
                    }
                    else if (d_Shift2_In == defultTime)
                    {
                        masterRow["d_Shift2_In"] = currentTime;
                    }
                    else if (d_Shift2_Out == defultTime)
                    {
                        masterRow["d_Shift2_Out"] = currentTime;
                    }
                    masterRow["d_Date"] = date.ToString();
                    MasterTable.AcceptChanges();

                    nTimesheetID = dLayer.SaveData("Pay_TimeSheetImport", "N_SheetID", MasterTable, connection, transaction);
                    if (nTimesheetID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        SortedList QueryParams = new SortedList();
            
                    QueryParams.Add("@nCompanyID", nCompanyID);
                    QueryParams.Add("@nFnYear", nFnYearId);
                    QueryParams.Add("@nDate", date);
                    QueryParams.Add("@nEmpID", nEmpID);

                    DataTable Details = dLayer.ExecuteDataTable("select * from Pay_TimeSheetImport where D_Date=@nDate and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYear and N_EmpID=@nEmpID", QueryParams, connection,transaction);
                    if (Details.Rows.Count == 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        Details = api.Format(Details, "master");
                        return Ok(api.Success(Details,"Your Attendance Marked"));
                    }

                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }


        [HttpGet("dayAttendanceDetails")]
        public ActionResult GetDayAttendance(int nEmpID, int nFnYear)
        {
            DataTable Details = new DataTable();
            DateTime date = DateTime.Today;

            int companyid = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SortedList QueryParams = new SortedList();
            
                    QueryParams.Add("@nCompanyID", companyid);
                    QueryParams.Add("@nFnYear", nFnYear);
                    QueryParams.Add("@nDate", date);
                    QueryParams.Add("@nEmpID", nEmpID);

                    Details = dLayer.ExecuteDataTable("select * from Pay_TimeSheetImport where D_Date=@nDate and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYear and N_EmpID=@nEmpID", QueryParams, connection);
                    if (Details.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        Details = api.Format(Details, "master");
                        return Ok(api.Success(Details));
                    }
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
    }

}