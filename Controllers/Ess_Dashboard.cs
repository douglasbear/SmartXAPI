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
        private readonly int FormID;

        public EssDashboard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        // private String CalculateGridEstDays(int VacTypeID, DateTime fromdate, float N_AccruedValue)
        // {
        //     DateTime toDate;
        //     int days = 0;
        //     double totalDays = 0;
        //     int empID = Convert.ToInt32(Session["EmpID"].ToString());
        //     int compID = Convert.ToInt32(Session["CompID"].ToString());
        //     double AvlDays = Convert.ToDouble(CalculateGridAnnualDays(VacTypeID));
        //     toDate = Convert.ToDateTime(dba.ExecuteScalar("Select isnull(Max(D_VacDateTo),getdate()) from Pay_VacationDetails Where N_CompanyID =" + compID + " and  N_EmpID  =" + empID + " and N_VacTypeID =" + VacTypeID + " and N_VacationStatus = 0 and N_VacDays>0 and ISNULL(B_IsSaveDraft,0) = 0").ToString());
        //     if (toDate < fromdate)
        //         days = Convert.ToInt32(dba.ExecuteScalar("select  DATEDIFF(day,'" + Convert.ToDateTime(toDate) + "','" + Convert.ToDateTime(fromdate) + "')").ToString());
        //     else
        //         days = 0;
        //     if (VacTypeID == 6)
        //     {
        //         totalDays = Math.Round(AvlDays + ((days / 30.458) * N_AccruedValue), 0);
        //     }
        //     else
        //         totalDays = Math.Round(AvlDays + ((days / 30.458)), 0);
        //     return totalDays.ToString();
        // }
        // private String CalculateGridAnnualDays(int VacTypeID,int compID,int empID)
        // {

        //     double vacation;
        //     const double tolerance = 8e-14;
        //     int N_VacationGroupID = 0;
        //     int.TryParse(Request.QueryString["VacationID"] == null ? "" : Request.QueryString["VacationID"].ToString(), out N_VacationGroupID);


        //     DateTime toDate = Convert.ToDateTime(dba.ExecuteScalar("Select isnull(Max(D_VacDateTo),getdate()) from Pay_VacationDetails Where N_CompanyID =" + compID + " and  N_EmpID  =" + empID + " and N_VacTypeID =" + VacTypeID + " and N_VacDays>0 ").ToString());

        //     if (N_VacationGroupID == 0)
        //         vacation = Convert.ToDouble(dba.ExecuteScalar("Select isnull(SUM(N_VacDays),0) as N_VacDays from Pay_VacationDetails Where N_CompanyID =" + compID + " and  N_EmpID  =" + empID + " and N_VacTypeID =" + VacTypeID).ToString());
        //     else
        //         vacation = Convert.ToDouble(dba.ExecuteScalar("Select isnull(SUM(N_VacDays),0) as N_VacDays from Pay_VacationDetails Where N_CompanyID =" + compID + " and  N_EmpID  =" + empID + " and N_VacTypeID =" + VacTypeID + " and isnull(N_VacationGroupID,0) < " + N_VacationGroupID).ToString());

        //     String AvlDays = RoundApproximate(vacation, 0, tolerance, MidpointRounding.AwayFromZero).ToString();


        //     return AvlDays;
        // }
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

        [HttpGet("listDetails")]
        public ActionResult GetCustomerDetails(int nEmpID,int nFnyearID)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);

            string sqlCommandEmployeeDetails = "select CONVERT(VARCHAR,vw_PayEmployee.d_DOB, 106) as d_DOB1,CONVERT(VARCHAR,vw_PayEmployee.d_HireDate, 106) as d_HireDate1,* from vw_PayEmployee where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3";
            // string sqlCommandLoan = "select SUM(N_LoanAmount) as N_LoanAmount from Pay_LoanIssue where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3 group by N_CompanyID,N_EmpID";
             string sqlCommandLoan = "select ROUND(SUM(balance),2) from vw_LoanDetails where N_CompanyID =@p1 and N_EmpID =@p3";
            string sqlCommandVacation = "Select SUM(N_VacDays) as N_VacDays from Pay_VacationDetails where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3 group by N_CompanyID,N_EmpID";
            //string sqlCommandLeave = "SELECT Pay_VacationType.X_VacType,SUM(Pay_VacationDetails.N_VacDays) AS N_VacDays FROM  Pay_VacationDetails INNER JOIN Pay_EmpAccruls ON Pay_VacationDetails.N_CompanyID = Pay_EmpAccruls.N_CompanyID AND Pay_VacationDetails.N_EmpID = Pay_EmpAccruls.N_EmpId AND Pay_VacationDetails.N_VacTypeID = Pay_EmpAccruls.N_VacTypeID INNER JOIN Pay_VacationType ON Pay_EmpAccruls.N_VacTypeID = Pay_VacationType.N_VacTypeID AND Pay_EmpAccruls.N_CompanyID = Pay_VacationType.N_CompanyID WHERE     (Pay_VacationDetails.N_VacDays <> 0) and Pay_VacationDetails.N_CompanyID=@p1 and Pay_VacationDetails.N_FnYearID=@p2 and Pay_VacationDetails.N_EmpID=@p3 Group by Pay_VacationType.X_VacType";
            string sqlCommandLeave = "Select N_VacTypeID ,N_Accrued ,N_MaxAvailDays, B_HolidayFlag ,X_VacType, 0 as X_Days ,X_Description, cast(N_Accrued as varchar)+case when N_Accrued =1 then ' Day/' else ' Days/'end + case when X_Period = 'M' then 'Month'  when X_Period ='Y' then 'Year' end as X_Accrued  from vw_pay_Vacation_List where X_Type='B' and N_EmpId = @p3 and N_CompanyID=@p1 and N_Accrued <>0 group by N_VacTypeID,X_VacType,N_Accrued,N_MaxAvailDays,B_HolidayFlag,X_Description,X_Period";
            // string sqlCommandPendingVacation = "Select SUM(N_VacDays) from Pay_VacationDetails where N_VacDays < 0 and ISNULL(B_IsSaveDraft,0)<>0 and N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3";
            string sqlCommandPendingVacation = "select COUNT(*) AS N_LeaveRequest from vw_WebApprovalDashboard where N_NextApproverID =@p4 and N_CompanyID=@p1 and N_EmpID <> @p3 and N_VacationStatus not in (2,4)";
            string sqlCommandNextLeave = "SELECT CONVERT(VARCHAR,Pay_VacationDetails.D_VacDateFrom, 106) as D_VacDateFrom,CONVERT(VARCHAR, Pay_VacationDetails.D_VacDateTo, 106) as D_VacDateTo, Pay_VacationDetails.N_VacDays,( 60 * Pay_VacationDetails.N_VacDays) as N_hours,Pay_VacationType.X_VacType FROM  Pay_VacationDetails INNER JOIN Pay_VacationType ON Pay_VacationDetails.N_VacTypeID = Pay_VacationType.N_VacTypeID AND  Pay_VacationDetails.N_CompanyID = Pay_VacationType.N_CompanyID WHERE  (Pay_VacationDetails.N_CompanyID = @p1) AND (N_FnYearID = @p2) AND (N_EmpID = @p3) and   (Pay_VacationDetails.N_VacationID = (SELECT     MAX(N_VacationID) AS Expr1 FROM         Pay_VacationDetails AS Pay_VacationDetails_1 WHERE     (N_CompanyID = @p1) AND (N_FnYearID = @p2) AND (N_EmpID = @p3) and  n_vacdays<0 ))";


            

            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnyearID);
            Params.Add("@p3", nEmpID);
            Params.Add("@p4", nUserID);

            DataTable EmployeeDetails = new DataTable();
            DataTable DashboardDetails = new DataTable();
            DataTable LeaveDetails = new DataTable();
            DataTable NextLeaveDetails = new DataTable();
            // DataTable Vacation = new DataTable();
            // DataTable ProjectCount = new DataTable();
            // DataTable ContractAmt = new DataTable();
            // DataTable BudgetAmt = new DataTable();
            // DataTable ProjectDetails = new DataTable();
            // DataTable EmployeeCount = new DataTable();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    EmployeeDetails = dLayer.ExecuteDataTable(sqlCommandEmployeeDetails, Params, connection);
                    EmployeeDetails=api.Format(EmployeeDetails,"EmployeeDetails");
                    EmployeeDetails=myFunctions.AddNewColumnToDataTable(EmployeeDetails,"EmployeeImage",typeof(string),null);
                    if (EmployeeDetails.Rows[0]["i_Employe_Image"] != null){
                        DataRow dataRow=EmployeeDetails.Rows[0];
                        byte[] Image = (byte[])dataRow["i_Employe_Image"];
                            EmployeeDetails.Rows[0]["EmployeeImage"] = "data:image/png;base64," + Convert.ToBase64String(Image, 0, Image.Length);
                        EmployeeDetails.Columns.Remove("i_Employe_Image");
                        EmployeeDetails.AcceptChanges();
                    }
                    object Loan = dLayer.ExecuteScalar(sqlCommandLoan, Params, connection);
                    object TotalVacation = dLayer.ExecuteScalar(sqlCommandVacation, Params, connection);
                    object PendingVacation = dLayer.ExecuteScalar(sqlCommandPendingVacation, Params, connection);

                    DashboardDetails=myFunctions.AddNewColumnToDataTable(DashboardDetails,"Loan",typeof(string),Loan);
                    DashboardDetails=myFunctions.AddNewColumnToDataTable(DashboardDetails,"Vacation",typeof(string),TotalVacation);
                    DashboardDetails=myFunctions.AddNewColumnToDataTable(DashboardDetails,"PendingVacation",typeof(string),PendingVacation);
                     DataRow row=DashboardDetails.NewRow();
                     DashboardDetails.Rows.Add(row);
                     DashboardDetails.AcceptChanges();

                    LeaveDetails = dLayer.ExecuteDataTable(sqlCommandLeave, Params, connection);
                    LeaveDetails=api.Format(LeaveDetails,"EmployeeLeaves");
                    NextLeaveDetails = dLayer.ExecuteDataTable(sqlCommandNextLeave, Params, connection);
                    NextLeaveDetails=api.Format(NextLeaveDetails,"EmployeeNextLeave");



                    // Loan = dLayer.ExecuteDataTable(sqlCommandLoan, Params, connection);
                    // Loan=api.Format(Loan,"Loan");
                    // Vacation = dLayer.ExecuteDataTable(sqlCommandVacation, Params, connection);
                    // Vacation=api.Format(Vacation,"Vacation");
                    // ProjectCount = dLayer.ExecuteDataTable(sqlCommandProjectCount, Params, connection);
                    // ProjectCount=api.Format(ProjectCount,"ProjectCount");
                    // ContractAmt = dLayer.ExecuteDataTable(sqlCommandContractAmt, Params, connection);
                    // ContractAmt=api.Format(ContractAmt,"ContractAmt");
                    // BudgetAmt = dLayer.ExecuteDataTable(sqlCommandBudgetAmt, Params, connection);
                    // BudgetAmt=api.Format(BudgetAmt,"BudgetAmt");
                    // ProjectDetails = dLayer.ExecuteDataTable(sqlCommandProjectDetails, Params, connection);
                    // ProjectDetails=api.Format(ProjectDetails,"ProjectDetails");
                    // EmployeeCount = dLayer.ExecuteDataTable(sqlCommandEmployeeCount, Params, connection);
                    // EmployeeCount=api.Format(EmployeeCount,"EmployeeCount");

                    
                }
                dt.Tables.Add(EmployeeDetails);
                 DashboardDetails=api.Format(DashboardDetails,"DashboardDetails");
                dt.Tables.Add(DashboardDetails);
                dt.Tables.Add(LeaveDetails);
                dt.Tables.Add(NextLeaveDetails);
                // dt.Tables.Add(Loan);
                // dt.Tables.Add(Vacation);
                //  dt.Tables.Add(SalesFunnel);
                //  dt.Tables.Add(ProjectCount);
                //  dt.Tables.Add(ContractAmt);
                //  dt.Tables.Add(BudgetAmt);
                //  dt.Tables.Add(ProjectDetails);
                //  dt.Tables.Add(EmployeeCount);


                 return Ok(api.Success(dt));

            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }
        }
        

        
    }
}