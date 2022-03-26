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
    [Route("taskDashBoard")]
    [ApiController]
    public class Tsk_TaskDashBoard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Tsk_TaskDashBoard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
     
 [HttpGet("details")]
        public ActionResult GetDashboardDetails(int nFnYearId, int nUserID, int N_Year)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);

            string UserPattern = myFunctions.GetUserPattern(User);
            string Pattern = "";
            string AssigneePattern = "";
            string sqlActiveEmployees = "SELECT COUNT(*) as N_ActiveEmloyees FROM pay_employee WHERE N_CompanyID=" + nCompanyID + " and N_Status not in (2,3)";//Active employees
            string sqlScheduledList = "select  COUNT(*) as N_ToDoList from  vw_Tsk_TaskCurrentStatus where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and isnull(B_Closed,0) =0";
            string sqlActiveList = "select Count(*) as N_ActiveList from vw_Tsk_TaskCurrentStatus where N_Status=7 and N_CompanyID=" + nCompanyID + " and  N_CreaterID=" + nUserID + "";
            string sqlCompletedList = "select Count(*) as N_CompletedList from vw_Tsk_TaskCurrentStatus where N_CompanyID=" + nCompanyID + " and N_CreaterID=" + nUserID + " and N_Status=4";
            string sqlClosedList = "select Count(*) as N_CompletedList from vw_Tsk_TaskCurrentStatus where N_CompanyID=" + nCompanyID + " and N_CreaterID=" + nUserID + " and N_Status=5";
            string sqlTaskStatus = "";
            SortedList Data = new SortedList();
            DataTable ActiveEmployees = new DataTable();
            DataTable ScheduledList = new DataTable();
            DataTable ActiveList = new DataTable();
            DataTable CompletedList = new DataTable();
            DataTable ClosedList = new DataTable();
            DataTable TaskStatus = new DataTable();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
              
                {
                    connection.Open();
                    ActiveEmployees = dLayer.ExecuteDataTable(sqlActiveEmployees, Params, connection);
                    ScheduledList = dLayer.ExecuteDataTable(sqlScheduledList, Params, connection);
                    ActiveList = dLayer.ExecuteDataTable(sqlActiveList, Params, connection);
                    CompletedList = dLayer.ExecuteDataTable(sqlCompletedList, Params, connection);
                    ClosedList = dLayer.ExecuteDataTable(sqlClosedList, Params, connection);

                  

                    SortedList statusParams = new SortedList();
                    statusParams.Add("@N_CompanyID", nCompanyID);
                    statusParams.Add("@N_UserID", nUserID);
                    statusParams.Add("@N_Year", N_Year);


                    try
                    {
                        TaskStatus=dLayer.ExecuteDataTablePro("Sp_TaskStatus", statusParams, connection);
                    }
                    catch (Exception ex)
                    {
                       
                        return Ok(api.Error(User, ex));
                    }


                }
                 ActiveEmployees.AcceptChanges();
                ScheduledList.AcceptChanges();
                ActiveList.AcceptChanges();
                CompletedList.AcceptChanges();
                ClosedList.AcceptChanges();
                TaskStatus.AcceptChanges();
                
                if (ActiveEmployees.Rows.Count > 0) Data.Add("activeEmployees", ActiveEmployees);
                if (ScheduledList.Rows.Count > 0) Data.Add("scheduledList", ScheduledList);
                if (ActiveList.Rows.Count > 0) Data.Add("activeList", ActiveList);
                if (CompletedList.Rows.Count > 0) Data.Add("completedList", CompletedList);
                if (ClosedList.Rows.Count > 0) Data.Add("closedList", ClosedList);
               
                if (TaskStatus.Rows.Count > 0) Data.Add("taskStatus", TaskStatus);

               
                return Ok(api.Success(Data));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }





    }
}






