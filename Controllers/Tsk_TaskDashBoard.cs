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
        public ActionResult GetDashboardDetails(int nFnYearId, int nUserID, int N_Year, DateTime d_Date)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);

            string UserPattern = myFunctions.GetUserPattern(User);
            string Pattern = "";
            string AssigneePattern = "";
            string sqlActiveEmployees = "SELECT COUNT(*) as N_ActiveEmloyees FROM pay_employee WHERE N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and  N_Status not in (2,3)";//Active employees
            string sqlScheduledList = "select  COUNT(*) as N_ToDoList from  vw_Tsk_TaskCurrentStatus where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and isnull(B_Closed,0) =0";
            string sqlTodaysTaskList = "select Count(*) as N_TodaysTaskList from vw_Tsk_TaskCurrentStatus  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and  D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and D_DueDate='" + d_Date + "'";
            string sqlCompletedList = "select Count(*) as N_CompletedList from vw_Tsk_TaskCompletedStatus where N_CompanyID=" + nCompanyID + " and N_CreaterID=" + nUserID + " and N_Status=4";
            string sqlOverDueList = "select Count(*) as N_OverDueTaskList from vw_Tsk_TaskCurrentStatus  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and  D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and D_DueDate < '" + d_Date + "'";
            string sqlTaskStatus = "";
            SortedList Data = new SortedList();
            DataTable ActiveEmployees = new DataTable();
            DataTable ScheduledList = new DataTable();
            DataTable TodayTaskList = new DataTable();
            DataTable CompletedList = new DataTable();
            DataTable overDueList = new DataTable();
            DataTable TaskStatus = new DataTable();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))

                {
                    connection.Open();
                    ActiveEmployees = dLayer.ExecuteDataTable(sqlActiveEmployees, Params, connection);
                    ScheduledList = dLayer.ExecuteDataTable(sqlScheduledList, Params, connection);
                    TodayTaskList = dLayer.ExecuteDataTable(sqlTodaysTaskList, Params, connection);
                    CompletedList = dLayer.ExecuteDataTable(sqlCompletedList, Params, connection);
                    overDueList = dLayer.ExecuteDataTable(sqlOverDueList, Params, connection);



                    SortedList statusParams = new SortedList();
                    statusParams.Add("@N_CompanyID", nCompanyID);
                    statusParams.Add("@N_UserID", nUserID);
                    statusParams.Add("@N_Year", N_Year);


                    try
                    {
                        TaskStatus = dLayer.ExecuteDataTablePro("Sp_TaskStatus", statusParams, connection);
                    }
                    catch (Exception ex)
                    {

                        return Ok(api.Error(User, ex));
                    }


                }
                ActiveEmployees.AcceptChanges();
                ScheduledList.AcceptChanges();
                TodayTaskList.AcceptChanges();
                CompletedList.AcceptChanges();
                overDueList.AcceptChanges();
                TaskStatus.AcceptChanges();

                if (ActiveEmployees.Rows.Count > 0) Data.Add("ActiveEmployees", ActiveEmployees);
                if (ScheduledList.Rows.Count > 0) Data.Add("ScheduledList", ScheduledList);
                if (TodayTaskList.Rows.Count > 0) Data.Add("TodayTaskList", TodayTaskList);
                if (CompletedList.Rows.Count > 0) Data.Add("CompletedList", CompletedList);
                if (overDueList.Rows.Count > 0) Data.Add("overDueList", overDueList);

                if (TaskStatus.Rows.Count > 0) Data.Add("TaskStatus", TaskStatus);


                return Ok(api.Success(Data));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
        [HttpGet("taskStageList")]
        public ActionResult GetDashboardStageList()

        {


            SortedList Params = new SortedList();
            DataTable ActiveTasks = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);
            // string sqlActiveTasks = "SELECT *  FROM vw_Tsk_TaskCurrentStatus WHERE N_CompanyID=" + nCompanyID + " and N_Status=2";//Active Tasks
            string sqlActiveTasks = "SELECT *  FROM vw_Tsk_TaskCurrentStatus WHERE N_CompanyID=" + nCompanyID + " and isnull(B_Closed,0) =0 and D_TaskDate>0 and x_tasksummery<>'Project Created' and x_tasksummery<>'Project Closed'";//Active Tasks
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))

                {
                    connection.Open();
                    ActiveTasks = dLayer.ExecuteDataTable(sqlActiveTasks, Params, connection);
                    if (ActiveTasks.Rows.Count > 0)
                    {
                        ActiveTasks = api.Format(ActiveTasks, "ActiveTasks");
                        return Ok(api.Success(ActiveTasks));

                    }
                    else
                    {
                        return Ok(api.Warning("No Results Found"));
                    }



                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
        [HttpGet("dashboardDetails")]
        public ActionResult GetDetails(int nUserID, DateTime d_Date)

        {


            SortedList Params = new SortedList();
            DataSet TaskManager = new DataSet();
            DataTable TodaysTasks = new DataTable();
            DataTable UpComingTasks = new DataTable();
            DataTable OverDueTasks = new DataTable();
            DataTable FollowupTasks = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlTodaysTaskList = "select * from vw_Tsk_TaskCurrentStatus  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and  D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and D_DueDate='" + d_Date + "' order By N_PriorityID asc ";
            string sqlOverDueList = "select * from vw_Tsk_TaskCurrentStatus  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and  D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and D_DueDate < '" + d_Date + "' order By N_PriorityID asc ";
            string sqlUpcomingList = "select * from vw_Tsk_TaskCurrentStatus where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and   D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and D_DueDate >  '" + d_Date + "' order By N_PriorityID asc";
            string sqlFollowUp="select * from vw_Tsk_TaskStatus where N_Status <= 3 and  N_Status <> 1 and N_CreaterID=" + nUserID + " and ISNULL(B_Closed,0)=0 ";
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))

                {
                    connection.Open();
                    TodaysTasks = dLayer.ExecuteDataTable(sqlTodaysTaskList, Params, connection);
                    UpComingTasks = dLayer.ExecuteDataTable(sqlUpcomingList, Params, connection);
                    OverDueTasks = dLayer.ExecuteDataTable(sqlOverDueList, Params, connection);
                    FollowupTasks = dLayer.ExecuteDataTable(sqlFollowUp, Params, connection);

                    TodaysTasks = api.Format(TodaysTasks, "TodaysTasks");
                    UpComingTasks = api.Format(UpComingTasks, "UpComingTasks");
                    OverDueTasks = api.Format(OverDueTasks, "OverDueTasks");
                    FollowupTasks = api.Format(FollowupTasks, "FollowUpTasks");



                    TaskManager.Tables.Add(TodaysTasks);
                    TaskManager.Tables.Add(UpComingTasks);
                    TaskManager.Tables.Add(OverDueTasks);


                    return Ok(api.Success(TaskManager));



                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }

        }
    }
}






