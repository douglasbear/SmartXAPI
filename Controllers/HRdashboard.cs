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
    [Route("hrDashboard")]
    [ApiController]
    public class HRdashboard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public HRdashboard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("details")]
        public ActionResult GetDashboardDetails(int nFnYearId)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);

            string sqlEmpCount = "select (select  count(*) from pay_employee where N_Status not in (2,3) and N_CompanyID = "+nCompanyID+" and N_FnYearId= "+nFnYearId+") as N_ActEmp ,(select  count(*) from pay_employee where N_Status  in (2,3) and N_CompanyID ="+nCompanyID+" and N_FnYearId= "+nFnYearId+") as N_SepEmp";
            //string sqlPayables= "select sum(N_BalanceAmount) as N_Amount from vw_InvPayables where N_CompanyId = "+nCompanyID+" and N_FnYearId= "+nFnYearId+"";
            string sqlEmpByCountry= "select X_Nationality,count(*) as N_Percentage from Pay_Employee where N_Status not in (2,3) and isnull(X_Nationality,'') <>'' and N_CompanyID ="+nCompanyID+" group by X_Nationality ";

            string sqlEmpByDpt = "select X_Department,N_Male,N_Female from vw_DptWiseEmployee where N_CompanyID ="+nCompanyID+" and N_FnYearId= "+nFnYearId+"";
           
           string sqlTerminationTrend = "SELECT  DATEADD(month, DATEDIFF(month, 0, D_EndDate), 0) AS D_MonthStart, YEAR(D_EndDate) [Year], MONTH(D_EndDate) [Month],  DATENAME(MONTH,D_EndDate) [Month Name], COUNT(1) [N_Count] FROM pay_EndOFService where N_CompanyID =  "+nCompanyID+" and N_FnYearId= "+nFnYearId+" GROUP BY DATEADD(month, DATEDIFF(month, 0, D_EndDate), 0) ,YEAR(D_EndDate), MONTH(D_EndDate),  DATENAME(MONTH, D_EndDate),N_FnYearID,N_CompanyID ORDER BY 1,2";
           // string sqlTerminationTrend = "SELECT YEAR(D_EndDate) [Year], MONTH(D_EndDate) [Month],  DATENAME(MONTH,D_EndDate) [Month Name], COUNT(1) [N_Count] FROM pay_EndOFService where N_CompanyID = "+nCompanyID+" and N_FnYearId= "+nFnYearId+" GROUP BY YEAR(D_EndDate), MONTH(D_EndDate),  DATENAME(MONTH, D_EndDate),N_FnYearID,N_CompanyID ORDER BY 1,2";
            string sqlEmpBySalary = "select X_Range, count(*) as N_Count from vw_payEmpSalary where n_companyid = "+nCompanyID+" and X_Range <> 'Other' group by X_Range";
            //"select X_LeadSource,CAST(COUNT(*) as varchar(50)) as N_Percentage from vw_CRMLeads group by X_LeadSource";
            string sqlEmpOnLeave = "select COUNT(*) as N_Count from Pay_VacationDetails where N_VacDays < 0 and B_IsAdjustEntry =0 and (D_VacDateFrom = GETDATE() or (D_VacDateTo >GETDATE()  and D_VacDateFrom <GETDATE() ) or D_VacDateFrom =GETDATE() ) and N_CompanyID ="+nCompanyID+" and N_FnYearId= "+nFnYearId+"";
            //string sqlCurrentSales ="SELECT  DATEADD(month, DATEDIFF(month, 0, D_EndDate), 0) AS D_MonthStart, YEAR(D_EndDate) [Year], MONTH(D_EndDate) [Month],  DATENAME(MONTH,D_EndDate) [Month Name], COUNT(1) [N_Count] FROM pay_EndOFService where N_CompanyID =  "+nCompanyID+" and N_FnYearId= "+nFnYearId+" GROUP BY DATEADD(month, DATEDIFF(month, 0, D_EndDate), 0) ,YEAR(D_EndDate), MONTH(D_EndDate),  DATENAME(MONTH, D_EndDate),N_FnYearID,N_CompanyID ORDER BY 1,2"

            SortedList Data = new SortedList();
            DataTable EmpCount = new DataTable();
            DataTable EmpOnLeave = new DataTable();
            DataTable EmpByCountry = new DataTable();
            DataTable EmpByDpt = new DataTable();
            DataTable TerminationTrend = new DataTable();
            DataTable EmpBySalary = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    EmpCount = dLayer.ExecuteDataTable(sqlEmpCount, Params, connection);
                    EmpOnLeave = dLayer.ExecuteDataTable(sqlEmpOnLeave, Params, connection);
                    EmpByCountry = dLayer.ExecuteDataTable(sqlEmpByCountry, Params, connection);
                    EmpByDpt = dLayer.ExecuteDataTable(sqlEmpByDpt, Params, connection);
                    TerminationTrend = dLayer.ExecuteDataTable(sqlTerminationTrend, Params, connection);
                    EmpBySalary= dLayer.ExecuteDataTable(sqlEmpBySalary, Params, connection);
                }


                EmpCount.AcceptChanges();
                EmpOnLeave.AcceptChanges();
                EmpByCountry.AcceptChanges();
                EmpByDpt.AcceptChanges();
                TerminationTrend.AcceptChanges();
                EmpBySalary.AcceptChanges();


                if (EmpCount.Rows.Count > 0) Data.Add("empCount", EmpCount);
                if (EmpOnLeave.Rows.Count > 0) Data.Add("empOnLeave", EmpOnLeave);
                if (EmpByCountry.Rows.Count > 0) Data.Add("empByCountry", EmpByCountry);
                if (EmpByDpt.Rows.Count > 0) Data.Add("empByDpt", EmpByDpt);
                if (TerminationTrend.Rows.Count > 0) Data.Add("terminationTrend", TerminationTrend);
                if (EmpBySalary.Rows.Count > 0) Data.Add("empBySalary", EmpBySalary);

                return Ok(api.Success(Data));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
    }
}