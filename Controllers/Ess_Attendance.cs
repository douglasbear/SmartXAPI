// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using System;
// using SmartxAPI.GeneralFunctions;
// using System.Data;
// using System.Collections;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Data.SqlClient;
// using System.Collections.Generic;

// namespace SmartxAPI.Controllers
// {
//     [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//     [Route("attendance")]
//     [ApiController]



//     public class Ess_Attendance : ControllerBase
//     {
//         private readonly IDataAccessLayer dLayer;
//         private readonly IApiFunctions api;
//         private readonly IMyFunctions myFunctions;
//         private readonly string connectionString;
//         private readonly int FormID;


//         public Ess_Attendance(IDataAccessLayer dl, IApiFunctions apiFun, IMyFunctions myFun, IConfiguration conf)
//         {
//             dLayer = dl;
//             api = apiFun;
//             myFunctions = myFun;
//             connectionString = conf.GetConnectionString("SmartxConnection");
//             FormID = 1229;
//         }

//         [HttpGet("details")]
//         public ActionResult GetAttendanceDetails(string xPayRunID, int nEmployeeID)
//         {
//             DataTable Master = new DataTable();
//             DataTable Detail = new DataTable();
//             DataSet ds = new DataSet();
//             SortedList Params = new SortedList();
//             SortedList QueryParams = new SortedList();

//             int companyid = myFunctions.GetCompanyID(User);

//             QueryParams.Add("@nCompanyID", companyid);
//             QueryParams.Add("@xPayRunID", xPayRunID);
//             QueryParams.Add("@nEmployeeID", nEmployeeID);
//             string Condition = "";
//             string _sqlQuery = "";
//             try
//             {
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     connection.Open();
//                     Condition = "Pay_TimeSheetMaster.N_EmpID=@nEmployeeID AND Pay_TimeSheetMaster.x_PayrunText=@xPayRunID";

//                     _sqlQuery = "SELECT  case when Pay_TimeSheet.n_TotalWorkHour > 0 then 'P' else 'A' end as X_Status,Pay_TimeSheetMaster.N_TimeSheetID, Pay_TimeSheetMaster.N_EmpID, Pay_TimeSheetMaster.X_PayrunText, Pay_TimeSheetMaster.D_DateFrom, Pay_TimeSheetMaster.D_DateTo, Pay_TimeSheetMaster.N_TotalDutyHours, Pay_TimeSheetMaster.N_TotalWorkedDays, Pay_TimeSheet.D_In,Pay_TimeSheet.D_Out, Pay_TimeSheet.D_Shift2_In, Pay_TimeSheet.D_Shift2_Out, Pay_TimeSheet.N_Status, Pay_TimeSheet.N_DutyHours,Pay_TimeSheet.N_diff,CONVERT(VARCHAR ,Pay_TimeSheet.D_Date, 106) as D_Date,round(Pay_TimeSheet.N_TotalWorkHour,2) as N_TotalWorkHour,* FROM Pay_TimeSheetMaster INNER JOIN Pay_TimeSheet ON Pay_TimeSheetMaster.N_TimeSheetID = Pay_TimeSheet.N_TimeSheetID AND Pay_TimeSheetMaster.N_CompanyID = Pay_TimeSheet.N_CompanyID Where " + Condition + "";

//                     Master = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

//                     Master = api.Format(Master, "master");
//                     if (Master.Rows.Count == 0)
//                     {
//                         return Ok(api.Notice("No Results Found"));
//                     }
//                     else
//                     {
//                         ds.Tables.Add(Master);
//                         return Ok(api.Success(ds));
//                     }
//                 }

//             }
//             catch (Exception e)
//             {
//                 return Ok(api.Error(e));
//             }
//         }
//     }
// }


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
    [Route("attendance")]
    [ApiController]



    public class Ess_Attendance : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Ess_Attendance(IDataAccessLayer dl, IApiFunctions apiFun, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1229;
        }

        [HttpGet("details")]
        public ActionResult GetAttendanceDetails(int nEmployeeID, int nFnYear,DateTime payDate, string payText)
        {
            DataTable Details = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();
                    SortedList OutPut = new SortedList();

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
                    DateTime fromDate = new DateTime();
                    DateTime toDate = new DateTime();
                    DateTime dtStartDate = new DateTime(payDate.Year, payDate.Month, 1);

                    int days = 0;
                    if (PeriodType != null && PeriodType.ToString() == "M")
                    {
                        days = DateTime.DaysInMonth(payDate.Year, payDate.Month) - myFunctions.getIntVAL(Periodvalue.ToString());
                        toDate = dtStartDate.AddDays(myFunctions.getIntVAL(Periodvalue.ToString()) - 2);
                        int lastdays = myFunctions.getIntVAL(Periodvalue.ToString());
                        fromDate = dtStartDate.AddMonths(-1).AddDays(lastdays - 1);
                    }
                    else
                    {
                        days = DateTime.DaysInMonth(payDate.Year, payDate.Month) - myFunctions.getIntVAL(Periodvalue.ToString());
                        toDate = dtStartDate.AddDays(myFunctions.getIntVAL(days.ToString()) - 1);
                        int lastdays = myFunctions.getIntVAL(Periodvalue.ToString());
                        fromDate = dtStartDate.AddDays(-lastdays);
                    }

                    // QueryParams.Add("N_CompanyID", companyid);
                    // QueryParams.Add("N_FnYear", nFnYear);
                    // QueryParams.Add("D_DateFrom", fromDate);
                    // QueryParams.Add("D_DateTo", toDate);
                    // QueryParams.Add("N_EmpID", nEmployeeID);

                    // SortedList OutPut = new SortedList();
                    SortedList Master = new SortedList();
                    Master.Add("fromDate", fromDate);
                    Master.Add("toDate", toDate);
                    Master.Add("days", days);

                    QueryParams.Add("@PayText",payText);
                    QueryParams.Add("@EmployeeID",nEmployeeID);



                    string qry = "SELECT     Pay_TimeSheetMaster.N_TimeSheetID, Pay_TimeSheetMaster.N_EmpID, Pay_TimeSheetMaster.X_PayrunText, Pay_TimeSheetMaster.D_DateFrom, "+
                      " Pay_TimeSheetMaster.D_DateTo, Pay_TimeSheetMaster.N_TotalDutyHours, Pay_TimeSheetMaster.N_TotalWorkedDays, Pay_TimeSheet.D_In, "+
                      " Pay_TimeSheet.D_Out, Pay_TimeSheet.D_Shift2_In, Pay_TimeSheet.D_Shift2_Out, Pay_TimeSheet.N_Status, Pay_TimeSheet.N_DutyHours, "+
                      "Pay_TimeSheet.N_diff,Pay_TimeSheet.D_Date,round(Pay_TimeSheet.N_TotalWorkHour,2) " +
                        "FROM         Pay_TimeSheetMaster INNER JOIN "+
                     " Pay_TimeSheet ON Pay_TimeSheetMaster.N_TimeSheetID = Pay_TimeSheet.N_TimeSheetID AND "+
                      "Pay_TimeSheetMaster.N_CompanyID = Pay_TimeSheet.N_CompanyID where Pay_TimeSheetMaster.N_EmpID=@EmployeeID AND Pay_TimeSheetMaster.X_PayrunText=@PayText";

                    // Details = dLayer.ExecuteDataTablePro("SP_Pay_TimeSheet", QueryParams, connection);
                    Details = dLayer.ExecuteDataTable(qry,QueryParams, connection);

                    if (Details.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {

                            // DataTable dt = new DataTable();
                            // dt.Columns.Add("D_Date", typeof(string));

                            // DateTime fdtime = (DateTime)Details.Rows[0][3];
                            // string frDate = fdtime.ToString("dd/MM/yyyy");

                            // DateTime tdtime = (DateTime)Details.Rows[0][4];
                            // string toDate2 = tdtime.ToString("dd/MM/yyyy");


                            // char split = ' ';
                            // if (frDate.Contains('/'))
                            // {
                            //     split = '/';
                            // }

                            // if (frDate.Contains('.'))
                            // {
                            //     split = '.';
                            // }


                            // if (frDate.Contains('-'))
                            // {
                            //     split = '-';
                            // }


                            // string[] strF = frDate.Split(split);
                            // string[] strE = toDate2.Split(split);


                            // DateTime ds = new DateTime(int.Parse(strF[2]), int.Parse(strF[1]), int.Parse(strF[0]), 0, 0, 0);
                            // DateTime de = new DateTime(int.Parse(strE[2]), int.Parse(strE[1]), int.Parse(strE[0]), 0, 0, 0);
                            // TimeSpan ts = de.Subtract(ds);

                            // for (int i = 0; i <= ts.Days; i++)
                            // {
                            //     DataRow dr = dt.NewRow();
                            //     dr[0] = ds.AddDays(i).ToString(@"dd-MMM-yyyy ddd");
                            //     dt.Rows.Add(dr);
                            // }
                            Details = api.Format(Details, "details");
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
    }
}