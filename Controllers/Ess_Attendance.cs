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


        public Ess_Attendance(IDataAccessLayer dl, IApiFunctions apiFun, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("details")]
        public ActionResult GetAttendanceDetails(int nEmployeeID, int nFnYear, DateTime payDate, string payText, DateTime dateFrom, DateTime dateTo)
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

                    DateTime fromDate = new DateTime();
                    DateTime toDate = new DateTime();
                    int days = 0;
                    string crieteria1 = "";
                    string sqlDetails = "";
                    if (payText != null && payText != "")
                    {
                        object PeriodType = dLayer.ExecuteScalar("Select X_Value from Gen_Settings Where X_Description ='Period Settings' and N_CompanyID= " + companyid + " and X_Group='Payroll'", connection);
                        object Periodvalue = dLayer.ExecuteScalar("Select N_Value from Gen_Settings Where X_Description ='Period Settings' and N_CompanyID= " + companyid + " and X_Group='Payroll'", connection);
                        if (Periodvalue == null) return Ok(api.Notice("No Results Found"));
                        DateTime dtStartDate = new DateTime(payDate.Year, payDate.Month, 1);
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

                        crieteria1 = " and X_PayrunText=@PayText";
                        QueryParams.Add("@PayText", payText);
                        sqlDetails = "SELECT     Pay_TimeSheetMaster.N_TimeSheetID, Pay_TimeSheetMaster.N_EmpID, Pay_TimeSheetMaster.X_PayrunText, Pay_TimeSheetMaster.D_DateFrom, " +
                        " Pay_TimeSheetMaster.D_DateTo, Pay_TimeSheetMaster.N_TotalDutyHours, Pay_TimeSheetMaster.N_TotalWorkedDays, Pay_TimeSheet.D_In, " +
                        " Pay_TimeSheet.D_Out, Pay_TimeSheet.D_Shift2_In, Pay_TimeSheet.D_Shift2_Out, Pay_TimeSheet.N_Status, Pay_TimeSheet.N_DutyHours, " +
                        "Pay_TimeSheet.N_diff,Pay_TimeSheet.D_Date,round(Pay_TimeSheet.N_TotalWorkHour,2) " +
                          "FROM         Pay_TimeSheetMaster INNER JOIN " +
                       " Pay_TimeSheet ON Pay_TimeSheetMaster.N_TimeSheetID = Pay_TimeSheet.N_TimeSheetID AND " +
                        "Pay_TimeSheetMaster.N_CompanyID = Pay_TimeSheet.N_CompanyID where Pay_TimeSheetMaster.N_EmpID=@EmployeeID and Pay_TimeSheetMaster.N_companyID=@nCompanyID  AND Pay_TimeSheetMaster.X_PayrunText=@PayText";
                    }
                    else
                    {
                        fromDate = dateFrom;
                        toDate = dateTo;
                        days = (toDate - fromDate).Days;
                    QueryParams.Add("@fromDate", fromDate);
                    QueryParams.Add("@toDate", toDate);

                        sqlDetails = "select N_TimeSheetID,N_EmpID,X_PayrunText,null as D_DateFrom,null as D_DateTo,D_In,D_Out,D_Shift2_In,D_Shift2_Out,N_Status,N_DutyHours,N_diff,D_Date,round(Pay_TimeSheet.N_TotalWorkHour,2)  from Pay_TimeSheet where N_EmpID=@EmployeeID and N_CompanyID=@nCompanyID and d_Date between @fromDate and @toDate";
                    }

                    SortedList Master = new SortedList();
                    Master.Add("fromDate", fromDate);
                    Master.Add("toDate", toDate);
                    Master.Add("days", days);

                    
                    QueryParams.Add("@EmployeeID", nEmployeeID);
                    QueryParams.Add("@nCompanyID", myFunctions.GetCompanyID(User));

                    if (payText != null && payText != "")
                    {

                        string qry1 = "Select * from vw_TimeSheetMaster_Disp where N_EmpID=@EmployeeID " + crieteria1;
                        DataTable dtAttend = dLayer.ExecuteDataTable(qry1, QueryParams, connection);
                        double ExtraHour = 0, N_WorkHours = 0, N_WorkdHrs = 0, N_Deduction = 0, Uncompensated = 0, Addition = 0, NetDeduction = 0;
                        foreach (DataRow var in dtAttend.Rows)
                        {
                            N_WorkHours = HoursToMinutes(Convert.ToDouble(dtAttend.Rows[0]["N_TotalWorkingDays"].ToString()));
                            N_WorkdHrs = HoursToMinutes(Convert.ToDouble(dtAttend.Rows[0]["N_TotalWorkedDays"].ToString()));
                            N_Deduction = HoursToMinutes(Convert.ToDouble(dtAttend.Rows[0]["N_GridDedTotal"].ToString()));
                            Uncompensated = HoursToMinutes(Convert.ToDouble(dtAttend.Rows[0]["N_CompMinutes"].ToString()));

                            Addition = HoursToMinutes(Convert.ToDouble(dtAttend.Rows[0]["N_ot"].ToString()));
                            ExtraHour += HoursToMinutes(Convert.ToDouble(dtAttend.Rows[0]["N_TotalWorkedDays"].ToString())) - HoursToMinutes(Convert.ToDouble(dtAttend.Rows[0]["N_TotalWorkingDays"].ToString()));

                        }

                        Master.Add("N_WorkHours", MinutesToHours(N_WorkHours).ToString("0.00"));
                        Master.Add("N_WorkdHrs", MinutesToHours(N_WorkdHrs).ToString("0.00"));
                        Master.Add("Uncompensated", MinutesToHours(Uncompensated).ToString("0.00"));
                        Master.Add("NetDeduction", MinutesToHours(NetDeduction).ToString("0.00"));
                        Master.Add("ExtraHour", MinutesToHours(ExtraHour).ToString("0.00"));
                        Master.Add("N_Deduction", MinutesToHours(N_Deduction).ToString("0.00"));
                    }
                        OutPut.Add("master", Master);
                    Details = dLayer.ExecuteDataTable(sqlDetails, QueryParams, connection);

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

        public double MinutesToHours(double Minutes)
        {
            double Hours = 0;
            //Minutes = Round(Minutes, 2);
            Hours = (((int)Minutes / 60) + (Minutes % 60) / 100);
            return Hours;
        }
        public double HoursToMinutes(double Hours)
        {
            double Minutes = 0;
            Minutes = (((int)Hours * 60) + (Hours % 1) % .60 * 100);
            return Minutes;
        }

    }
}