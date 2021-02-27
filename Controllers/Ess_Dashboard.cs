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

            string sqlCommandEmployeeDetails = "select * from vw_PayEmployee where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3";
            // string sqlCommandLoan = "select SUM(N_LoanAmount) as N_LoanAmount from Pay_LoanIssue where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3 group by N_CompanyID,N_EmpID";
            string sqlCommandLoan = "select ROUND(SUM(balance),2) from vw_LoanDetails where N_CompanyID =@p1 and N_EmpID =@p3";
            string sqlCommandVacation = "Select SUM(N_VacDays) as N_VacDays from Pay_VacationDetails where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3 group by N_CompanyID,N_EmpID";
            //string sqlCommandLeave = "SELECT Pay_VacationType.X_VacType,SUM(Pay_VacationDetails.N_VacDays) AS N_VacDays FROM  Pay_VacationDetails INNER JOIN Pay_EmpAccruls ON Pay_VacationDetails.N_CompanyID = Pay_EmpAccruls.N_CompanyID AND Pay_VacationDetails.N_EmpID = Pay_EmpAccruls.N_EmpId AND Pay_VacationDetails.N_VacTypeID = Pay_EmpAccruls.N_VacTypeID INNER JOIN Pay_VacationType ON Pay_EmpAccruls.N_VacTypeID = Pay_VacationType.N_VacTypeID AND Pay_EmpAccruls.N_CompanyID = Pay_VacationType.N_CompanyID WHERE     (Pay_VacationDetails.N_VacDays <> 0) and Pay_VacationDetails.N_CompanyID=@p1 and Pay_VacationDetails.N_FnYearID=@p2 and Pay_VacationDetails.N_EmpID=@p3 Group by Pay_VacationType.X_VacType";
            string sqlCommandLeave = "Select N_VacTypeID ,N_Accrued ,N_MaxAvailDays, B_HolidayFlag ,X_VacType, 0 as X_Days ,X_Description, cast(N_Accrued as varchar)+case when N_Accrued =1 then ' Day/' else ' Days/'end + case when X_Period = 'M' then 'Month'  when X_Period ='Y' then 'Year' end as X_Accrued  from vw_pay_Vacation_List where X_Type='B' and N_EmpId = @p3 and N_CompanyID=@p1 and N_Accrued <>0 group by N_VacTypeID,X_VacType,N_Accrued,N_MaxAvailDays,B_HolidayFlag,X_Description,X_Period";
            // string sqlCommandPendingVacation = "Select SUM(N_VacDays) from Pay_VacationDetails where N_VacDays < 0 and ISNULL(B_IsSaveDraft,0)<>0 and N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3";
            string sqlCommandPendingVacation = "select COUNT(*) AS N_LeaveRequest from vw_WebApprovalDashboard where N_NextApproverID =@p4 and N_CompanyID=@p1 and N_EmpID <> @p3 and N_VacationStatus not in (2,4)";
            //string sqlCommandNextLeave = "SELECT CONVERT(VARCHAR,Pay_VacationDetails.D_VacDateFrom, 106) as D_VacDateFrom,CONVERT(VARCHAR, Pay_VacationDetails.D_VacDateTo, 106) as D_VacDateTo, Pay_VacationDetails.N_VacDays,( 24 * Pay_VacationDetails.N_VacDays) as N_hours,Pay_VacationType.X_VacType FROM  Pay_VacationDetails INNER JOIN Pay_VacationType ON Pay_VacationDetails.N_VacTypeID = Pay_VacationType.N_VacTypeID AND  Pay_VacationDetails.N_CompanyID = Pay_VacationType.N_CompanyID WHERE  (Pay_VacationDetails.N_CompanyID = @p1) AND (N_FnYearID = @p2) AND (N_EmpID = @p3) and   (Pay_VacationDetails.N_VacationID = (SELECT     MAX(N_VacationID) AS Expr1 FROM         Pay_VacationDetails AS Pay_VacationDetails_1 WHERE     (N_CompanyID = @p1) AND (N_FnYearID = @p2) AND (N_EmpID = @p3) and  n_vacdays<0 ))";
            string sqlCommandNextLeave = "SELECT     CONVERT(VARCHAR, Pay_VacationDetails.D_VacDateFrom, 106) AS D_VacDateFrom, CONVERT(VARCHAR, Pay_VacationDetails.D_VacDateTo, 106) AS D_VacDateTo,Pay_VacationDetails.N_VacDays, 24 * Pay_VacationDetails.N_VacDays AS N_hours, Pay_VacationType.X_VacType FROM Pay_VacationDetails INNER JOIN Pay_VacationType ON Pay_VacationDetails.N_VacTypeID = Pay_VacationType.N_VacTypeID AND Pay_VacationDetails.N_CompanyID = Pay_VacationType.N_CompanyID INNER JOIN Pay_VacationMaster ON Pay_VacationDetails.N_VacationGroupID = Pay_VacationMaster.N_VacationGroupID AND Pay_VacationDetails.N_EmpID = Pay_VacationMaster.N_EmpID AND Pay_VacationDetails.N_CompanyID = Pay_VacationMaster.N_CompanyID WHERE (Pay_VacationDetails.N_CompanyID = @p1) and Pay_VacationMaster.B_IsSaveDraft=0 and Pay_VacationDetails.B_IsAdjustEntry<>1 AND (Pay_VacationDetails.N_FnYearID = @p2) AND (Pay_VacationDetails.N_EmpID = @p3) AND (Pay_VacationDetails.N_VacationID =(SELECT     MAX(N_VacationID) AS Expr1 FROM Pay_VacationDetails AS Pay_VacationDetails_1 WHERE (N_CompanyID = @p1) AND (N_FnYearID = @p2) AND (N_EmpID = @p3) AND (N_VacDays < 0) and (D_VacDateFrom>=Convert(date, getdate()))))";
            string sqlCommandDailyLogin = "SELECT MAX(D_In) as D_In,MAX(D_Out) as D_Out,Convert(Time, GetDate()) as D_Cur,cast(dateadd(millisecond, datediff(millisecond,MAX(D_In),case when Max(D_Out)='00:00:00.0000000' then  Convert(Time, GetDate()) else Max(D_Out) end), '19000101')  AS TIME) AS duration from Pay_TimeSheetImport where N_EmpID=@p3 and D_Date=@today";
            string EnableLeave = "select N_Value from Gen_Settings where X_Group='EssOnline' and N_CompanyID=@p1";
            string WorkerHours="select top(7) D_Date,N_EmpID,convert(varchar(5),DateDiff(s, D_In, D_Out)/3600)+':'+convert(varchar(5),DateDiff(s, D_In, D_Out)%3600/60)+':'+convert(varchar(5),(DateDiff(s, D_In, D_Out)%60)) as [hh:mm:ss] from Pay_TimeSheetImport where N_EmpID = @p3 order by D_Date desc";

DateTime date = DateTime.Today;
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

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    EmployeeDetails = dLayer.ExecuteDataTable(sqlCommandEmployeeDetails, Params, connection);
                    EmployeeDetails = api.Format(EmployeeDetails, "EmployeeDetails");
                    EmployeeDetails = myFunctions.AddNewColumnToDataTable(EmployeeDetails, "EmployeeImage", typeof(string), null);
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
                    object EnableLeaveData = dLayer.ExecuteScalar(EnableLeave, Params, connection);
                    object Loan = dLayer.ExecuteScalar(sqlCommandLoan, Params, connection);
                    object TotalVacation=null;
                    if(EnableLeaveData.ToString()=="1")
                         TotalVacation = dLayer.ExecuteScalar(sqlCommandVacation, Params, connection);
                    object PendingVacation = dLayer.ExecuteScalar(sqlCommandPendingVacation, Params, connection);
                    

                    DashboardDetails = myFunctions.AddNewColumnToDataTable(DashboardDetails, "Loan", typeof(string), Loan);
                    DashboardDetails = myFunctions.AddNewColumnToDataTable(DashboardDetails, "Vacation", typeof(string), TotalVacation);
                    DashboardDetails = myFunctions.AddNewColumnToDataTable(DashboardDetails, "PendingVacation", typeof(string), PendingVacation);
                    DashboardDetails = myFunctions.AddNewColumnToDataTable(DashboardDetails, "EnableLeave", typeof(string), EnableLeaveData);
                    DataRow row = DashboardDetails.NewRow();
                    DashboardDetails.Rows.Add(row);
                    DashboardDetails.AcceptChanges();
                    if(EnableLeaveData.ToString()=="1")
                    {
                        LeaveDetails = dLayer.ExecuteDataTable(sqlCommandLeave, Params, connection);
                          int i = 0;
                        foreach (DataRow dtRow in LeaveDetails.Rows)
                        {
                        String Avail = GetAvailableDays(myFunctions.getIntVAL(LeaveDetails.Rows[i]["N_VacTypeID"].ToString()), DateTime.Now, double.Parse(LeaveDetails.Rows[0]["N_Accrued"].ToString()), nEmpID);
                        //String Avail = CalculateGridEstDays(myFunctions.getIntVAL(LeaveDetails.Rows[i]["N_VacTypeID"].ToString()), DateTime.Now, float.Parse(LeaveDetails.Rows[0]["N_Accrued"].ToString()), nEmpID);
                        dtRow["x_Days"] = Avail;
                        i++;
                        }
                         LeaveDetails.AcceptChanges();
                    }
                    LeaveDetails = api.Format(LeaveDetails, "EmployeeLeaves");


                    NextLeaveDetails = dLayer.ExecuteDataTable(sqlCommandNextLeave, Params, connection);
                    NextLeaveDetails = api.Format(NextLeaveDetails, "EmployeeNextLeave");

                    DailyLogin = dLayer.ExecuteDataTable(sqlCommandDailyLogin, Params, connection);
                    DailyLogin = api.Format(DailyLogin, "DailyLogin");

                    WorkedHours = dLayer.ExecuteDataTable(WorkerHours, Params, connection);
                    WorkedHours = api.Format(WorkedHours, "Worked Hour");


                }
                dt.Tables.Add(EmployeeDetails);
                DashboardDetails = api.Format(DashboardDetails, "DashboardDetails");
                dt.Tables.Add(DashboardDetails);
                dt.Tables.Add(LeaveDetails);
                dt.Tables.Add(NextLeaveDetails);
                dt.Tables.Add(DailyLogin);
                dt.Tables.Add(WorkedHours);

                return Ok(api.Success(dt));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
        public string GetAvailableDays(int nVacTypeID, DateTime dDateFrom, double nAccrued, int nEmpID)
        {
            DateTime toDate;
            int days = 0;
            double totalDays = 0;
            int nVacationGroupID = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    double AvlDays = Convert.ToDouble(CalculateGridAnnualDays(nVacTypeID, nEmpID, nCompanyID, nVacationGroupID, connection));

                    SortedList paramList = new SortedList();
                    paramList.Add("@nCompanyID", nCompanyID);
                    paramList.Add("@nEmpID", nEmpID);
                    paramList.Add("@nVacTypeID", nVacTypeID);
                    paramList.Add("@nVacationGroupID", nVacationGroupID);

                    toDate = Convert.ToDateTime(dLayer.ExecuteScalar("Select isnull(Max(D_VacDateTo),getdate()) from Pay_VacationDetails Where N_CompanyID =@nCompanyID and  N_EmpID  =@nEmpID and N_VacTypeID =@nVacTypeID and N_VacationStatus = 0 and N_VacDays>0 and ISNULL(B_IsSaveDraft,0) = 0", paramList, connection).ToString());
                    if (toDate < dDateFrom)
                    {
                        string daySql = "select  DATEDIFF(day,'" + toDate.ToString("yyyy-MM-dd") + "','" + dDateFrom.ToString("yyyy-MM-dd") + "')";
                        days = Convert.ToInt32(dLayer.ExecuteScalar(daySql, connection).ToString());
                    }
                    else
                    { days = 0; }
                    if (nVacTypeID == 6)
                    {
                        totalDays = Math.Round(AvlDays + ((days / 30.458) * nAccrued), 0);
                    }
                    else
                        totalDays = Math.Round(AvlDays + ((days / 30.458)), 0);
                }

                return totalDays.ToString();
            }
            catch (Exception)
            {
                return "0";
            }
        }
        private String CalculateGridAnnualDays(int VacTypeID, int empID, int compID, int nVacationGroupID, SqlConnection connection)
        {
            double vacation;
            const double tolerance = 8e-14;
            SortedList paramList = new SortedList();
            paramList.Add("@nCompanyID", compID);
            paramList.Add("@nEmpID", empID);
            paramList.Add("@nVacTypeID", VacTypeID);
            paramList.Add("@nVacationGroupID", nVacationGroupID);


            DateTime toDate = Convert.ToDateTime(dLayer.ExecuteScalar("Select isnull(Max(D_VacDateTo),getdate()) from Pay_VacationDetails Where N_CompanyID =@nCompanyID and  N_EmpID  =@nEmpID and N_VacTypeID =@nVacTypeID and N_VacDays>0 ", paramList, connection).ToString());

            if (nVacationGroupID == 0)
                vacation = Convert.ToDouble(dLayer.ExecuteScalar("Select isnull(SUM(N_VacDays),0) as N_VacDays from Pay_VacationDetails Where N_CompanyID =@nCompanyID and  N_EmpID  =@nEmpID and N_VacTypeID =@nVacTypeID", paramList, connection).ToString());
            else
                vacation = Convert.ToDouble(dLayer.ExecuteScalar("Select isnull(SUM(N_VacDays),0) as N_VacDays from Pay_VacationDetails Where N_CompanyID =@nCompanyID and  N_EmpID  =@nEmpID and N_VacTypeID =@nVacTypeID and isnull(N_VacationGroupID,0) < @nVacationGroupID", paramList, connection).ToString());

            String AvlDays = RoundApproximate(vacation, 0, tolerance, MidpointRounding.AwayFromZero).ToString();


            return AvlDays;
        }

        private static double RoundApproximate(double dbl, int digits, double margin, MidpointRounding mode)
        {
            double fraction = dbl * Math.Pow(10, digits);
            double value = Math.Truncate(fraction);
            fraction = fraction - value;
            if (fraction == 0)
                return dbl;

            double tolerance = margin * dbl;
            // Any remaining fractional value greater than .5 is not a midpoint value. 
            if (fraction > .5)
                return (value + 0.5) / Math.Pow(10, digits);
            else if (fraction < -(0.5))
                return (value + 0.5) / Math.Pow(10, digits);
            else if (fraction == .5)
                return Math.Round(dbl, 1);
            else
                return value / Math.Pow(10, digits);
        }



    }
}