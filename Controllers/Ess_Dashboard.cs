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
using System.Net;
using Newtonsoft.Json;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("EssDashboard")]
    [ApiController]
    public class EssDashboard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public EssDashboard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("listDetails")]
        public ActionResult GetCustomerDetails(int nEmpID, int nFnyearID)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            object CategoryID = "";

            string sqlCommandEmployeeDetails = "select n_empID,X_EmpCode,X_EmpName,x_EmpNameLocale,x_Position,x_PositionLocale,d_HireDate,x_Department,n_ReportingToID,x_ReportTo,x_Phone1 ,x_EmailID,x_ProjectName,i_Employe_Image from vw_PayEmployee where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3";
            // string sqlCommandLoan = "select SUM(N_LoanAmount) as N_LoanAmount from Pay_LoanIssue where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3 group by N_CompanyID,N_EmpID";
            string sqlCommandLoan = "SELECT SUM(Pay_LoanIssueDetails.N_InstAmount) - SUM(ISNULL(Pay_LoanIssueDetails.N_RefundAmount, 0))AS balance FROM  Pay_LoanIssue LEFT OUTER JOIN Pay_LoanIssueDetails ON Pay_LoanIssue.N_LoanTransID = Pay_LoanIssueDetails.N_LoanTransID AND Pay_LoanIssue.N_CompanyID = Pay_LoanIssueDetails.N_CompanyID where isnull(Pay_LoanIssueDetails.B_IsLoanClose,0)=0 and Pay_LoanIssue.B_IsSaveDraft <> 1 and Pay_LoanIssue.N_CompanyID=@p1 and N_EmpID=@p3";
            string sqlCommandVacation = "Select SUM(N_VacDays) as N_VacDays from Pay_VacationDetails where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3 group by N_CompanyID,N_EmpID";
            //string sqlCommandLeave = "SELECT Pay_VacationType.X_VacType,SUM(Pay_VacationDetails.N_VacDays) AS N_VacDays FROM  Pay_VacationDetails INNER JOIN Pay_EmpAccruls ON Pay_VacationDetails.N_CompanyID = Pay_EmpAccruls.N_CompanyID AND Pay_VacationDetails.N_EmpID = Pay_EmpAccruls.N_EmpId AND Pay_VacationDetails.N_VacTypeID = Pay_EmpAccruls.N_VacTypeID INNER JOIN Pay_VacationType ON Pay_EmpAccruls.N_VacTypeID = Pay_VacationType.N_VacTypeID AND Pay_EmpAccruls.N_CompanyID = Pay_VacationType.N_CompanyID WHERE     (Pay_VacationDetails.N_VacDays <> 0) and Pay_VacationDetails.N_CompanyID=@p1 and Pay_VacationDetails.N_FnYearID=@p2 and Pay_VacationDetails.N_EmpID=@p3 Group by Pay_VacationType.X_VacType";
            string sqlCommandLeave = "Select N_VacTypeID ,dbo.Fn_CalcAvailDays(N_CompanyID,N_VacTypeID,N_EmpId,GETDATE(),0,1) as N_Accrued ,dbo.Fn_CalcAvailDays(N_CompanyID,N_VacTypeID,N_EmpId,GETDATE(),0,2) as N_MaxAvailDays, B_HolidayFlag ,X_VacType, 0.00 as X_Days ,X_Description, cast(N_Accrued as varchar)+case when N_Accrued =1 then ' Day/' else ' Days/'end + case when X_Period = 'M' then 'Month'  when X_Period ='Y' then 'Year' end as X_Accrued  from vw_pay_Vacation_List where X_Type='B' and N_EmpId = @p3 and N_CompanyID=@p1 and dbo.Fn_CalcAvailDays(N_CompanyID,N_VacTypeID,N_EmpId,GETDATE(),0,2) <>0 group by N_CompanyID,N_EmpId,N_VacTypeID,X_VacType,N_Accrued,N_MaxAvailDays,B_HolidayFlag,X_Description,X_Period";
            // string sqlCommandPendingVacation = "Select SUM(N_VacDays) from Pay_VacationDetails where N_VacDays < 0 and ISNULL(B_IsSaveDraft,0)<>0 and N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3";
            string sqlCommandPendingVacation = "select count(1) AS N_LeaveRequest from vw_WebApprovalDashboard where N_NextApproverID =@p4 and N_CompanyID=@p1 and N_EmpID <> @p3 and N_VacationStatus not in (2,4)";
            //string sqlCommandNextLeave = "SELECT CONVERT(VARCHAR,Pay_VacationDetails.D_VacDateFrom, 106) as D_VacDateFrom,CONVERT(VARCHAR, Pay_VacationDetails.D_VacDateTo, 106) as D_VacDateTo, Pay_VacationDetails.N_VacDays,( 24 * Pay_VacationDetails.N_VacDays) as N_hours,Pay_VacationType.X_VacType FROM  Pay_VacationDetails INNER JOIN Pay_VacationType ON Pay_VacationDetails.N_VacTypeID = Pay_VacationType.N_VacTypeID AND  Pay_VacationDetails.N_CompanyID = Pay_VacationType.N_CompanyID WHERE  (Pay_VacationDetails.N_CompanyID = @p1) AND (N_FnYearID = @p2) AND (N_EmpID = @p3) and   (Pay_VacationDetails.N_VacationID = (SELECT     MAX(N_VacationID) AS Expr1 FROM         Pay_VacationDetails AS Pay_VacationDetails_1 WHERE     (N_CompanyID = @p1) AND (N_FnYearID = @p2) AND (N_EmpID = @p3) and  n_vacdays<0 ))";
            string sqlCommandNextLeave = "SELECT     CONVERT(VARCHAR, Pay_VacationDetails.D_VacDateFrom, 106) AS D_VacDateFrom, CONVERT(VARCHAR, Pay_VacationDetails.D_VacDateTo, 106) AS D_VacDateTo,Pay_VacationDetails.N_VacDays, 24 * Pay_VacationDetails.N_VacDays AS N_hours, Pay_VacationType.X_VacType FROM Pay_VacationDetails INNER JOIN Pay_VacationType ON Pay_VacationDetails.N_VacTypeID = Pay_VacationType.N_VacTypeID AND Pay_VacationDetails.N_CompanyID = Pay_VacationType.N_CompanyID INNER JOIN Pay_VacationMaster ON Pay_VacationDetails.N_VacationGroupID = Pay_VacationMaster.N_VacationGroupID AND Pay_VacationDetails.N_EmpID = Pay_VacationMaster.N_EmpID AND Pay_VacationDetails.N_CompanyID = Pay_VacationMaster.N_CompanyID WHERE (Pay_VacationDetails.N_CompanyID = @p1) and Pay_VacationMaster.B_IsSaveDraft=0 and Pay_VacationDetails.B_IsAdjustEntry<>1 AND (Pay_VacationDetails.N_FnYearID = @p2) AND (Pay_VacationDetails.N_EmpID = @p3) AND (Pay_VacationDetails.N_VacationID =(SELECT     MAX(N_VacationID) AS Expr1 FROM Pay_VacationDetails AS Pay_VacationDetails_1 WHERE (N_CompanyID = @p1) AND (N_FnYearID = @p2) AND (N_EmpID = @p3) AND (N_VacDays < 0) and (D_VacDateFrom>=Convert(date, getdate()))))";
            string EnableLeave = "select N_Value from Gen_Settings where X_Group='EssOnline' and N_CompanyID=@p1";
            string WorkerHours = "select top(7) D_Date,N_EmpID,convert(varchar(5),DateDiff(s, D_In, D_Out)/3600)+':'+convert(varchar(5),DateDiff(s, D_In, D_Out)%3600/60)+':'+convert(varchar(5),(DateDiff(s, D_In, D_Out)%60)) as [hh:mm:ss] from Pay_TimeSheetImport where N_EmpID = @p3 order by D_Date desc";
            string sqlPendingLeaveApproval = "select count(1) from (select N_VacationGroupID From vw_PayVacationList where N_CompanyID=@p1 and B_IsAdjustEntry<>1 and N_VacationGroupID in ( select N_TransID from vw_ApprovalPending where N_CompanyID=@p1 and N_FnYearID=@p2 and X_Type='LEAVE REQUEST' and N_NextApproverID=@p4) group by N_VacationGroupID) as tbl";
            string sqlLastApproval = "SELECT      Top(1) vw_ApprovalSummary.*,vw_PayVacationDetails_Disp.VacTypeId ,vw_PayVacationDetails_Disp.[Vacation Type], vw_PayVacationDetails_Disp.D_VacDateFrom, vw_PayVacationDetails_Disp.D_VacDateTo, vw_PayVacationDetails_Disp.N_VacDays FROM vw_ApprovalSummary INNER JOIN vw_PayVacationDetails_Disp ON vw_ApprovalSummary.N_CompanyID = vw_PayVacationDetails_Disp.N_CompanyID AND  vw_ApprovalSummary.N_FnYearID = vw_PayVacationDetails_Disp.N_FnYearID AND vw_ApprovalSummary.N_TransID = vw_PayVacationDetails_Disp.N_VacationGroupID AND vw_ApprovalSummary.X_Type='LEAVE REQUEST' where vw_ApprovalSummary.N_CompanyID=@p1 and vw_ApprovalSummary.N_ActionUserID=@p4 and vw_ApprovalSummary.N_ProcStatusID<>6 and vw_ApprovalSummary.N_ActionUserID<>vw_ApprovalSummary.N_ReqUserID and vw_ApprovalSummary.X_Type='LEAVE REQUEST'  ORDER BY vw_ApprovalSummary.X_ActionDate DESC";

            DateTime date = DateTime.Now;
            // string url = "http://worldtimeapi.org/api/timezone/Asia/Kolkata";
            // using (var client = new WebClient())
            // {
            //     client.Headers.Add("content-type", "application/json");
            //     string response = client.DownloadString(url);
            //     response = response.Substring(62, 26);
            //     date = DateTime.Parse(response);
            // }

            //DateTime date = DateTime.Today;
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnyearID);
            Params.Add("@p3", nEmpID);
            Params.Add("@p4", nUserID);
            Params.Add("@today", date);

            DataTable EmployeeDetails = new DataTable();
            DataTable DashboardDetails = new DataTable();
            DataTable LeaveDetails = new DataTable();
            DataTable NextLeaveDetails = new DataTable();
            DataTable DailyLogin = new DataTable();
            DataTable WorkedHours = new DataTable();
            DataTable LastApproval = new DataTable();
            DataTable ShiftSchedule = new DataTable();
            DataTable scheduledDate = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlDIN = "SELECT isNull(MIN(D_In),'00:00:00') as D_In from Pay_TimeSheetImport where N_EmpID=@p3 and D_Date=@today and N_CompanyID=@p1 and D_In<> '00:00:00'";
                    string sqlDOUT = "SELECT top(1) D_Out as D_Out from Pay_TimeSheetImport  where N_EmpID=@p3 and D_Date=@today and N_CompanyID=@p1 order by N_SheetID desc";
                    object DIN = dLayer.ExecuteScalar(sqlDIN, Params, connection);
                    object DOUT = dLayer.ExecuteScalar(sqlDOUT, Params, connection);
                    // string sqlCommandDailyLogin = "SELECT '" + DIN + "' as D_In,'" + DOUT + "' as D_Out,Convert(Time, GetDate()) as D_Cur,cast(dateadd(millisecond, datediff(millisecond,case when '"+DIN+"'='00:00:00' then  Convert(Time, GetDate()) else '"+DIN+"' end,case when '"+DOUT+"' ='00:00:00' then  Convert(Time, GetDate()) else '"+DOUT+"' end), '19000101')  AS TIME) AS duration from Pay_TimeSheetImport where N_EmpID=@p3 and D_Date=@today";
                    string sqlCommandDailyLogin = "SELECT '" + DIN + "' as D_In,'" + DOUT + "' as D_Out,Convert(Time, GetDate()) as D_Cur, case when '"+DOUT+"'='00:00:00' then '' else cast(dateadd(millisecond, datediff(millisecond,case when '"+DIN+"'='00:00:00' then  Convert(Time, GetDate()) else '"+DIN+"' end,'"+DOUT+"'), '19000101')  AS TIME)end AS duration from Pay_TimeSheetImport where N_EmpID=@p3 and D_Date=@today";
                    string CatID ="select N_CatagoryId from Pay_Employee where N_EmpID=@p3 and N_CompanyID=@p1";
                    CategoryID = dLayer.ExecuteScalar(CatID, Params, connection);
                    EmployeeDetails = dLayer.ExecuteDataTable(sqlCommandEmployeeDetails, Params, connection);
                    EmployeeDetails = api.Format(EmployeeDetails, "EmployeeDetails");
                    EmployeeDetails = myFunctions.AddNewColumnToDataTable(EmployeeDetails, "EmployeeImage", typeof(string), null);
                    if (EmployeeDetails.Rows.Count > 0)
                    {
                        if (EmployeeDetails.Rows[0]["i_Employe_Image"] != null)
                        {
                            DataRow dataRow = EmployeeDetails.Rows[0];
                            string ImageData = dataRow["i_Employe_Image"].ToString();
                            if (ImageData != "")
                            {
                                byte[] Image = (byte[])dataRow["i_Employe_Image"];
                                EmployeeDetails.Rows[0]["EmployeeImage"] = "data:image/png;base64," + Convert.ToBase64String(Image, 0, Image.Length);
                                EmployeeDetails.Columns.Remove("i_Employe_Image");
                            }
                            EmployeeDetails.AcceptChanges();
                        }
                    }
                    object EnableLeaveData = dLayer.ExecuteScalar(EnableLeave, Params, connection);
                    // object EnableLeaveData = "";
                    object Loan = dLayer.ExecuteScalar(sqlCommandLoan, Params, connection);
                    object TotalVacation = null;
                    if (EnableLeaveData.ToString() == "1")
                        TotalVacation = dLayer.ExecuteScalar(sqlCommandVacation, Params, connection);
                    object PendingVacation = dLayer.ExecuteScalar(sqlCommandPendingVacation, Params, connection);
                    object PendingLeaveApproval = dLayer.ExecuteScalar(sqlPendingLeaveApproval, Params, connection);


                    DashboardDetails = myFunctions.AddNewColumnToDataTable(DashboardDetails, "Loan", typeof(string), Loan);
                    DashboardDetails = myFunctions.AddNewColumnToDataTable(DashboardDetails, "Vacation", typeof(string), TotalVacation);
                    DashboardDetails = myFunctions.AddNewColumnToDataTable(DashboardDetails, "PendingVacation", typeof(string), PendingVacation);
                    DashboardDetails = myFunctions.AddNewColumnToDataTable(DashboardDetails, "EnableLeave", typeof(string), EnableLeaveData);
                    DashboardDetails = myFunctions.AddNewColumnToDataTable(DashboardDetails, "PendingLeaveApproval", typeof(string), PendingLeaveApproval);
                    DataRow row = DashboardDetails.NewRow();
                    DashboardDetails.Rows.Add(row);
                    DashboardDetails.AcceptChanges();
                    if (EnableLeaveData.ToString() == "1")
                    {
                        LeaveDetails = dLayer.ExecuteDataTable(sqlCommandLeave, Params, connection);
                        int i = 0;
                        foreach (DataRow dtRow in LeaveDetails.Rows)
                        {
                            string Avail = GetAvailableDays(myFunctions.getIntVAL(LeaveDetails.Rows[i]["N_VacTypeID"].ToString()), DateTime.Now, nEmpID);
                            //String Avail = CalculateGridEstDays(myFunctions.getIntVAL(LeaveDetails.Rows[i]["N_VacTypeID"].ToString()), DateTime.Now, float.Parse(LeaveDetails.Rows[0]["N_Accrued"].ToString()), nEmpID);
                            dtRow["x_Days"] = Avail;
                            i++;
                        }
                        LeaveDetails.AcceptChanges();
                    }


                    //     ShiftSchedule = dLayer.ExecuteDataTable(sqlEmpShiftSchedule, Params, connection);
                    //     ShiftSchedule = api.Format(ShiftSchedule, "ShiftSchedule");

                    // DateTime Start = DateTime.Now;
                    // DateTime End = Start.AddDays(9);


                    //  double a = (End - Start).TotalDays;
                    //      bool dayFlag = false;

                    //  if(ShiftSchedule.Rows.Count !=10){

                    //       for (int j = 0; j <= a; j++) {
                    //             dayFlag = false;
                    //             DateTime NewDate = Convert.ToDateTime(Start).AddDays(j);
                    //             var Date = myFunctions.getDateVAL(NewDate).ToString();
                    //             foreach (DataRow var in ShiftSchedule.Rows)
                    //             {
                    //                         DateTime sheduleDate = Convert.ToDateTime(var["D_Date"]);
                    //                         var empDate =myFunctions.getDateVAL(sheduleDate).ToString();
                    //                         if( Date == empDate)
                    //                         {
                    //                             dayFlag=true;
                    //                         }
                    //             }
                    //            if(dayFlag==false)
                    //            {
                    //                 DayOfWeek dow = NewDate.DayOfWeek; 
                    //                 string str = dow.ToString();
                    //                 Params.Add("@p6",str);
                    //                 Params.Add("@p7",CategoryID);
                    //                 string qry="select N_CompanyID,D_In1,D_Out1,D_In2,D_Out2,'"+NewDate+"' as  D_Date   from Pay_WorkingHours where N_CompanyID=@p1 and X_Day=@p6 and N_CatagoryID=@p7";

                    //                   scheduledDate = dLayer.ExecuteDataTable(qry, Params, connection);
                    //                   DataRow row1 = scheduledDate.NewRow();
                    //             row["D_Date"] = 0;
                    //             row["N_CompanyID"] = scheduledDate.Rows[0]["N_CompanyID"];
                    //             row["D_In1"] = scheduledDate.Rows[0]["D_In1"];
                    //             row["D_Out1"] = scheduledDate.Rows[0]["D_Out1"];
                    //              row["D_In2"] = scheduledDate.Rows[0]["D_In2"];
                    //               row["D_Out2"] = scheduledDate.Rows[0]["D_Out2"];
                    //             row["D_Date"] = scheduledDate.Rows[0]["D_Date"];
                    //             scheduledDate.Rows.Add(row1);

                    //                    scheduledDate.Rows.Clear();
                    //                    ShiftSchedule.AcceptChanges();
                    //              }

                    //       }


                    //  }



                    LeaveDetails = api.Format(LeaveDetails, "EmployeeLeaves");


                    NextLeaveDetails = dLayer.ExecuteDataTable(sqlCommandNextLeave, Params, connection);
                    NextLeaveDetails = api.Format(NextLeaveDetails, "EmployeeNextLeave");

                    DailyLogin = dLayer.ExecuteDataTable(sqlCommandDailyLogin, Params, connection);
                    DailyLogin = api.Format(DailyLogin, "DailyLogin");

                    WorkedHours = dLayer.ExecuteDataTable(WorkerHours, Params, connection);
                    WorkedHours = api.Format(WorkedHours, "Worked Hour");

                    LastApproval = dLayer.ExecuteDataTable(sqlLastApproval, Params, connection);
                    LastApproval = api.Format(LastApproval, "LastApproval");

                    //  ShiftSchedule = dLayer.ExecuteDataTable(sqlEmpShiftSchedule, Params, connection);
                    //   ShiftSchedule = api.Format(ShiftSchedule, "ShiftSchedule");


                }
                dt.Tables.Add(EmployeeDetails);
                DashboardDetails = api.Format(DashboardDetails, "DashboardDetails");
                dt.Tables.Add(DashboardDetails);
                dt.Tables.Add(LeaveDetails);
                dt.Tables.Add(NextLeaveDetails);
                dt.Tables.Add(DailyLogin);
                dt.Tables.Add(WorkedHours);
                dt.Tables.Add(LastApproval);
                // dt.Tables.Add(ShiftSchedule);

                return Ok(api.Success(dt));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
        // public string GetAvailableDays(int nVacTypeID, DateTime dDateFrom, double nAccrued, int nEmpID)
        // {
        //     DateTime toDate;
        //     int days = 0;
        //     double totalDays = 0;
        //     int nVacationGroupID = 0;
        //     int nCompanyID = myFunctions.GetCompanyID(User);
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             double AvlDays = Convert.ToDouble(CalculateGridAnnualDays(nVacTypeID, nEmpID, nCompanyID, nVacationGroupID, connection));

        //             SortedList paramList = new SortedList();
        //             paramList.Add("@nCompanyID", nCompanyID);
        //             paramList.Add("@nEmpID", nEmpID);
        //             paramList.Add("@nVacTypeID", nVacTypeID);
        //             paramList.Add("@nVacationGroupID", nVacationGroupID);

        //             toDate = Convert.ToDateTime(dLayer.ExecuteScalar("Select isnull(Max(D_VacDateTo),getdate()) from Pay_VacationDetails Where N_CompanyID =@nCompanyID and  N_EmpID  =@nEmpID and N_VacTypeID =@nVacTypeID and N_VacationStatus = 0 and N_VacDays>0 and ISNULL(B_IsSaveDraft,0) = 0", paramList, connection).ToString());
        //             if (toDate < dDateFrom)
        //             {
        //                 string daySql = "select  DATEDIFF(day,'" + toDate.ToString("yyyy-MM-dd") + "','" + dDateFrom.ToString("yyyy-MM-dd") + "')";
        //                 days = Convert.ToInt32(dLayer.ExecuteScalar(daySql, connection).ToString());
        //             }
        //             else
        //             { days = 0; }
        //             if (nVacTypeID == 6)
        //             {
        //                 totalDays = Math.Round(AvlDays + ((days / 30.458) * nAccrued), 0);
        //             }
        //             else
        //                 totalDays = Math.Round(AvlDays + ((days / 30.458)), 0);
        //         }

        //         return totalDays.ToString();
        //     }
        //     catch (Exception)
        //     {
        //         return "0";
        //     }
        // }


        // private String CalculateGridAnnualDays(int VacTypeID, int empID, int compID, int nVacationGroupID, SqlConnection connection)
        // {
        //     double vacation;
        //     const double tolerance = 8e-14;
        //     SortedList paramList = new SortedList();
        //     paramList.Add("@nCompanyID", compID);
        //     paramList.Add("@nEmpID", empID);
        //     paramList.Add("@nVacTypeID", VacTypeID);
        //     paramList.Add("@nVacationGroupID", nVacationGroupID);


        //     DateTime toDate = Convert.ToDateTime(dLayer.ExecuteScalar("Select isnull(Max(D_VacDateTo),getdate()) from Pay_VacationDetails Where N_CompanyID =@nCompanyID and  N_EmpID  =@nEmpID and N_VacTypeID =@nVacTypeID and N_VacDays>0 ", paramList, connection).ToString());

        //     if (nVacationGroupID == 0)
        //         vacation = Convert.ToDouble(dLayer.ExecuteScalar("Select isnull(SUM(N_VacDays),0) as N_VacDays from Pay_VacationDetails Where N_CompanyID =@nCompanyID and  N_EmpID  =@nEmpID and N_VacTypeID =@nVacTypeID", paramList, connection).ToString());
        //     else
        //         vacation = Convert.ToDouble(dLayer.ExecuteScalar("Select isnull(SUM(N_VacDays),0) as N_VacDays from Pay_VacationDetails Where N_CompanyID =@nCompanyID and  N_EmpID  =@nEmpID and N_VacTypeID =@nVacTypeID and isnull(N_VacationGroupID,0) < @nVacationGroupID", paramList, connection).ToString());

        //     String AvlDays = RoundApproximate(vacation, 0, tolerance, MidpointRounding.AwayFromZero).ToString();


        //     return AvlDays;
        // }

        // private static double RoundApproximate(double dbl, int digits, double margin, MidpointRounding mode)
        // {
        //     double fraction = dbl * Math.Pow(10, digits);
        //     double value = Math.Truncate(fraction);
        //     fraction = fraction - value;
        //     if (fraction == 0)
        //         return dbl;

        //     double tolerance = margin * dbl;
        //     // Any remaining fractional value greater than .5 is not a midpoint value. 
        //     if (fraction > .5)
        //         return (value + 0.5) / Math.Pow(10, digits);
        //     else if (fraction < -(0.5))
        //         return (value + 0.5) / Math.Pow(10, digits);
        //     else if (fraction == .5)
        //         return Math.Round(dbl, 1);
        //     else
        //         return value / Math.Pow(10, digits);
        // }


        public string GetAvailableDays(int nVacTypeID, DateTime dDateFrom, int nEmpID)
        {
            DataTable dt = new DataTable();
            SortedList output = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            SortedList QueryParams = new SortedList();

            QueryParams.Add("@nCompanyID", nCompanyID);
            QueryParams.Add("@nEmpID", nEmpID);
            QueryParams.Add("@nVacationGroupID", 0);
            QueryParams.Add("@today", dDateFrom);
            QueryParams.Add("@nVacTypeID", nVacTypeID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable("Select dbo.Fn_CalcAvailDays(@nCompanyID,@nVacTypeID,@nEmpID,@today,@nVacationGroupID,2) As AvlDays,dbo.Fn_CalcAvailDays(@nCompanyID,@nVacTypeID,@nEmpID,@today,@nVacationGroupID,1) As Accrude", QueryParams, connection);
                }

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["AvlDays"].ToString();
                }

                return "0";

            }
            catch (Exception e)


            {
                return "0";
            }
        }




        [HttpGet("EmphiftDetails")]
        public ActionResult EmphiftDetails(int nEmpID, int nFnyearID)
        {
            DataSet dt = new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable ScheduledDate = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            object CategoryID = "";
            string Sql = "";
            string sqlCommandText = "select top(10) N_CompanyID,D_Date,D_In1,D_Out1,D_In2,D_Out2,X_GroupName,x_dutyPlace1,x_dutyPlace2 from vw_payEmpShiftDetails where N_EmpID=@p3 and N_CompanyID=@p1 and MONTH(Cast(vw_payEmpShiftDetails.D_Date as DATE)) = MONTH(CURRENT_TIMESTAMP) and YEAR(vw_payEmpShiftDetails.D_Date)= YEAR(CURRENT_TIMESTAMP) order by D_Date asc";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnyearID);
            Params.Add("@p3", nEmpID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string CatID = "select N_CatagoryId from Pay_Employee where N_EmpID=@p3 and N_CompanyID=@p1 and N_FnYearID=@p2";
                    CategoryID = dLayer.ExecuteScalar(CatID, Params, connection);

                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);


                    DateTime Start = DateTime.Now;
                    DateTime End = Start.AddDays(9);

                    double a = (End - Start).TotalDays;
                    bool dayFlag = false;
                    Params.Add("@p7", CategoryID);

                    if (MasterTable.Rows.Count == 0)
                    {

                        for (int j = 0; j <= a; j++)
                        {
                            dayFlag = false;
                            DateTime NewDate = Convert.ToDateTime(Start).AddDays(j);
                            var Date = myFunctions.getDateVAL(NewDate).ToString();
                            DayOfWeek dow = NewDate.DayOfWeek;
                            string str = dow.ToString();
                            string qry = "select N_CompanyID,D_In1,D_Out1,D_In2,D_Out2,'" + NewDate + "' as  D_Date,X_GroupName  from vw_PayWorkingHours where N_CompanyID=@p1 and X_Day='" + str + "' and N_CatagoryID=@p7";
                            Sql = Sql == "" ? qry : Sql + " UNION " + qry;

                        }
                        MasterTable = dLayer.ExecuteDataTable(Sql, Params, connection);

                    }

                    MasterTable = api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);
                }
                return Ok(api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

      

     


                [HttpGet("EmpBirthDayList")]
               public ActionResult EmpBirthDay(int nUserID,int nFnyearID, int day,bool bAllBranchData,int nBranchID)
                {
           try
            {
                 using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DataTable = new DataTable();
                    SortedList OutPut = new SortedList();
                   int nCompanyId=myFunctions.GetCompanyID(User);
                   string criteria="";
           
          
                //    DateTime Start = DateTime.Now;
                //    int day = Start.Day;
                             Params.Add("@p1", nCompanyId);
                             Params.Add("@p2", nFnyearID);
                             Params.Add("@p3", nUserID);
                            Params.Add("@p4", nBranchID);
                             Params.Add("@today", day);
                  
                 if (bAllBranchData == false)
                criteria=" and N_BranchID =@p4";

                   string sqlCommandText = "select x_EmpName,x_position from vw_PayEmployee where MONTH(vw_PayEmployee.D_DOB) = MONTH(CURRENT_TIMESTAMP) and DAY(vw_PayEmployee.D_DOB) =@today and N_CompanyID=@p1 and B_Inactive=0 and N_EmpID!=@p3 and N_FnYearID=@p2 and N_Status not in (2,3)"+criteria;
                   string sqlCommandCount ="select count(1) as N_Count from vw_PayEmployee where MONTH(vw_PayEmployee.D_DOB) = MONTH(CURRENT_TIMESTAMP) and DAY(vw_PayEmployee.D_DOB) =@today and N_CompanyID=@p1 and B_Inactive=0 and N_EmpID=@p3 and N_FnYearID=@p2 and N_Status not in (2,3)"+criteria;
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                  object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                   
                        return Ok(api.Success(OutPut));


                }
            }

             catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
          }



          [HttpGet("EmpLeaveList")]
        public ActionResult EmpLeaveList(int nEmpID, int nFnyearID)
        {
            DataSet dt = new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable ScheduledDate = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
             string Sql = "";

            Params.Add("@nCompanyID", nCompanyId);
            Params.Add("@nFnyearID", nFnyearID);
            Params.Add("@nEmpID", nEmpID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                     DateTime Start = DateTime.Now;
                    DateTime End = Start.AddDays(-6);

                    double a = (Start-End).TotalDays;
                    bool dayFlag = false;

                     Params.Add("@dtpFromdate", Start);
                     Params.Add("@dtpTodate", End);

                         Sql = "SP_Pay_TimeSheet @nCompanyID,@nFnyearID,@dtpTodate,@dtpFromdate,@nEmpID";
                        MasterTable = dLayer.ExecuteDataTable(Sql, Params, connection);

                           DateTime Date = Convert.ToDateTime(Start.ToString());
                          do
                                {
                                    DataRow[] CheckDate = MasterTable.Select("D_date = '" + myFunctions.getDateVAL(Date).ToString() + "'");
                                    if (CheckDate.Length == 0)
                                    {
                                        DataRow rowPA = MasterTable.NewRow();
                                        rowPA["D_date"] = Date;

                                        DateTime Date5 = Convert.ToDateTime(Date.ToString());
                                        string day = Date5.DayOfWeek.ToString();
                                        MasterTable.Rows.Add(rowPA);
                                    }
                                     Date = Date.AddDays(-1);
                                } while (Date >= End);
                                 MasterTable.AcceptChanges();



                    MasterTable = api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);
                }
                return Ok(api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

      

     




    }
}