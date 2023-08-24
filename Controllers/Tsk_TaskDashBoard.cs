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
            string sqlActiveEmployees = "SELECT count(1) as N_ActiveEmloyees FROM pay_employee WHERE N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and  N_Status not in (2,3)";//Active employees
            string sqlScheduledList = "select  count(1) as N_ToDoList from  [vw_TaskCurrentStatus] where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and isnull(B_Closed,0) =0";
            string sqlTodaysTaskList = "select count(1) as N_TodaysTaskList from [vw_TaskCurrentStatus]  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_TaskDate as DATE)<='" + d_Date + "' and  Cast(D_DueDate as DATE) >= '" + d_Date + "'";
            string sqlUpcomingTaskList = "select count(1) as N_UpcomingTaskList from [vw_TaskCurrentStatus]  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE) > '" + d_Date + "' and Cast(D_TaskDate as DATE) >  '" + d_Date + "'";
            string sqlCompletedList = "select count(1) as N_CompletedList from vw_Tsk_TaskCompletedStatus where N_CompanyID=" + nCompanyID + " and N_CreaterID=" + nUserID + "and  N_Status in(4) and MONTH(Cast(vw_Tsk_TaskCompletedStatus.D_Entrydate as Datetime)) = MONTH(CURRENT_TIMESTAMP) and YEAR(vw_Tsk_TaskCompletedStatus.D_Entrydate)= YEAR(CURRENT_TIMESTAMP)";
            string sqlOverDueList = "select count(1) as N_OverDueTaskList from [vw_TaskCurrentStatus]  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + "   and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE) < '" + d_Date + "'";
            string sqlTaskStatus = "";
            SortedList Data = new SortedList();
            DataTable ActiveEmployees = new DataTable();
            DataTable ScheduledList = new DataTable();
            DataTable TodayTaskList = new DataTable();
            DataTable CompletedList = new DataTable();
            DataTable overDueList = new DataTable();
            DataTable TaskStatus = new DataTable();
            DataTable upComingTask = new DataTable();

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
                    upComingTask = dLayer.ExecuteDataTable(sqlUpcomingTaskList, Params, connection);


                    SortedList statusParams = new SortedList();
                    statusParams.Add("@N_CompanyID", nCompanyID);
                    statusParams.Add("@N_UserID", nUserID);
                    statusParams.Add("@N_Year", N_Year);
                    statusParams.Add("@D_Date", d_Date); 

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
                upComingTask.AcceptChanges();

                if (ActiveEmployees.Rows.Count > 0) Data.Add("ActiveEmployees", ActiveEmployees);
                if (ScheduledList.Rows.Count > 0) Data.Add("ScheduledList", ScheduledList);
                if (TodayTaskList.Rows.Count > 0) Data.Add("TodayTaskList", TodayTaskList);
                if (CompletedList.Rows.Count > 0) Data.Add("CompletedList", CompletedList);
                if (overDueList.Rows.Count > 0) Data.Add("overDueList", overDueList);

                if (TaskStatus.Rows.Count > 0) Data.Add("TaskStatus", TaskStatus);
                if (upComingTask.Rows.Count > 0) Data.Add("upComingTask", upComingTask);

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
            // string sqlActiveTasks = "SELECT *  FROM [vw_TaskCurrentStatus] WHERE N_CompanyID=" + nCompanyID + " and N_Status=2";//Active Tasks
            string sqlActiveTasks = "SELECT *  FROM [vw_TaskCurrentStatus] WHERE N_CompanyID=" + nCompanyID + " and isnull(B_Closed,0) =0  and x_tasksummery<>'Project Created' and x_tasksummery<>'Project Closed' order by N_TaskID desc";//Active Tasks
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
        public ActionResult GetDetails(int nUserID, DateTime d_Date, int nPage, int nSizeperpage)

        {


            SortedList Params = new SortedList();
            DataSet TaskManager = new DataSet();
            DataTable TodaysTasks = new DataTable();
            DataTable UpComingTasks = new DataTable();
            DataTable OverDueTasks = new DataTable();
            DataTable FollowupTasks = new DataTable();
            DataTable CountTable = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int Count = (nPage - 1) * nSizeperpage;
            string sqlTodaysTaskList = "";
            string sqlOverDueList = "";
            string sqlUpcomingList = "";
            string sqlFollowUp = "";
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))

                {
                    connection.Open();

                    if (Count == 0)
                    {
                        sqlTodaysTaskList = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus]  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and  D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE)='" + d_Date + "' order by N_PriorityID asc";
                        sqlOverDueList = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus]  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and  D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE) < '" + d_Date + "' order by N_PriorityID asc";
                        sqlUpcomingList = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus] where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and   D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE) >  '" + d_Date + "' order by N_PriorityID asc";
                        sqlFollowUp = "select top(" + nSizeperpage + ") * from vw_Tsk_TaskStatus where  N_CompanyID=" + nCompanyID + " and N_Status <= 3 and  N_Status <> 1 and N_CreaterID=" + nUserID + " and ISNULL(B_Closed,0)=0 ";
                    }
                    else
                    {

                        sqlTodaysTaskList = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus]  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and  D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE)='" + d_Date + "' and N_TaskID not in (select top(" + Count + ") N_TaskID from [vw_TaskCurrentStatus] where  N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + "  and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE)='" + d_Date + "') order by N_PriorityID asc";
                        sqlOverDueList = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus]  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and  D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE) < '" + d_Date + "' and N_TaskID not in (select top(" + Count + ") N_TaskID from [vw_TaskCurrentStatus] where  N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + "  and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE)='" + d_Date + "') order by N_PriorityID asc";
                        sqlUpcomingList = "select  top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus] where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and   D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE) >  '" + d_Date + "' and N_TaskID not in (select top(" + Count + ") N_TaskID from [vw_TaskCurrentStatus] where  N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + "  and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE)='" + d_Date + "') order by N_PriorityID asc";
                        sqlFollowUp = "select top(" + nSizeperpage + ") * from vw_Tsk_TaskStatus where  N_CompanyID=" + nCompanyID + " and N_Status <= 3 and  N_Status <> 1 and N_CreaterID=" + nUserID + " and ISNULL(B_Closed,0)=0  and N_TaskID not in (select top(" + Count + ") N_TaskID from [vw_TaskCurrentStatus] where  N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and  D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE)='" + d_Date + "') order By N_TaskID desc";
                    }
                    TodaysTasks = dLayer.ExecuteDataTable(sqlTodaysTaskList, Params, connection);
                    UpComingTasks = dLayer.ExecuteDataTable(sqlUpcomingList, Params, connection);
                    OverDueTasks = dLayer.ExecuteDataTable(sqlOverDueList, Params, connection);
                    FollowupTasks = dLayer.ExecuteDataTable(sqlFollowUp, Params, connection);

                    TodaysTasks = api.Format(TodaysTasks, "TodaysTasks");
                    UpComingTasks = api.Format(UpComingTasks, "UpComingTasks");
                    OverDueTasks = api.Format(OverDueTasks, "OverDueTasks");
                    FollowupTasks = api.Format(FollowupTasks, "FollowUpTasks");
                    string sqlCommandCount = "select count(1) as N_Count from [vw_TaskCurrentStatus]  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and  D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and D_DueDate='" + d_Date + "'";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();

                    }
                    string sqlCommandCount1 = "select count(1) as N_Count from [vw_TaskCurrentStatus]  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + "  and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and D_TaskDate > '" + d_Date + "'";
                    DataTable Summary1 = dLayer.ExecuteDataTable(sqlCommandCount1, Params, connection);
                    string TotalCount1 = "0";
                    if (Summary1.Rows.Count > 0)
                    {
                        DataRow drow = Summary1.Rows[0];
                        TotalCount1 = drow["N_Count"].ToString();

                    }
                    string sqlCommandCount2 = "select count(1) as N_Count from [vw_TaskCurrentStatus] where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and   D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and D_DueDate >  '" + d_Date + "'";
                    DataTable Summary2 = dLayer.ExecuteDataTable(sqlCommandCount2, Params, connection);
                    string TotalCount2 = "0";
                    if (Summary2.Rows.Count > 0)
                    {
                        DataRow drow = Summary2.Rows[0];
                        TotalCount2 = drow["N_Count"].ToString();

                    }
                    string sqlCommandCount3 = "select count(1) as N_Count from vw_Tsk_TaskStatus where N_CompanyID=" + nCompanyID + " and N_Status <= 3 and  N_Status <> 1 and N_CreaterID=" + nUserID + " and ISNULL(B_Closed,0)=0 ";
                    DataTable Summary3 = dLayer.ExecuteDataTable(sqlCommandCount3, Params, connection);
                    string TotalCount3 = "0";
                    if (Summary3.Rows.Count > 0)
                    {
                        DataRow drow = Summary3.Rows[0];
                        TotalCount3 = drow["N_Count"].ToString();

                    }


                    CountTable.Clear();
                    CountTable.Columns.Add("TodaysTasksTotalCount");
                    CountTable.Columns.Add("UpComingTasksTotalCount");
                    CountTable.Columns.Add("OverDueTasksTotalCount");
                    CountTable.Columns.Add("FollowUpTasksTotalCount");

                    DataRow row = CountTable.NewRow();
                    row["TodaysTasksTotalCount"] = myFunctions.getIntVAL(TotalCount);
                    row["UpComingTasksTotalCount"] = myFunctions.getIntVAL(TotalCount1);
                    row["OverDueTasksTotalCount"] = myFunctions.getIntVAL(TotalCount2);
                    row["FollowUpTasksTotalCount"] = myFunctions.getIntVAL(TotalCount3);

                    CountTable.Rows.Add(row);

                    CountTable = api.Format(CountTable, "CountTable");



                    TaskManager.Tables.Add(TodaysTasks);
                    TaskManager.Tables.Add(UpComingTasks);
                    TaskManager.Tables.Add(OverDueTasks);
                    TaskManager.Tables.Add(FollowupTasks);
                    TaskManager.Tables.Add(CountTable);

                    return Ok(api.Success(TaskManager));



                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }

        }


        [HttpGet("toDaysTaskDetails")]
        public ActionResult GettoDayDetails(int nUserID, DateTime d_Date, int nPage, int nSizeperpage)

        {
            SortedList Params = new SortedList();
            DataSet TaskManager = new DataSet();
            DataTable TodaysTasks = new DataTable();
            DataTable CountTable = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int Count = (nPage - 1) * nSizeperpage;
            string sqlTodaysTaskList = "";
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))

                {
                    connection.Open();

                    if (Count == 0)
                    {
                        sqlTodaysTaskList = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus]  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_TaskDate as DATE)<='" + d_Date + "' and   Cast(D_DueDate as DATE) >= '" + d_Date + "' order by N_PriorityID asc";

                    }
                    else
                    {

                        sqlTodaysTaskList = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus]  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + "  and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_TaskDate as DATE)<='" + d_Date + "' and   Cast(D_DueDate as DATE) >= '" + d_Date + "' and N_TaskID not in (select top(" + Count + ") N_TaskID from [vw_TaskCurrentStatus] where  N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_TaskDate as DATE)<='" + d_Date + "' and   Cast(D_DueDate as DATE) >= '" + d_Date + "' order by N_PriorityID asc) order by N_PriorityID asc";

                    }
                    TodaysTasks = dLayer.ExecuteDataTable(sqlTodaysTaskList, Params, connection);

                    TodaysTasks = api.Format(TodaysTasks, "TodaysTasks");
                    string sqlCommandCount = "select count(1) as N_Count from [vw_TaskCurrentStatus]  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and  x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_TaskDate as DATE)<='" + d_Date + "' and Cast(D_DueDate as DATE)>='" + d_Date + "'";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    // if (Summary.Rows.Count > 0)
                    // {
                    //     DataRow drow = Summary.Rows[0];
                    //     TotalCount = drow["N_Count"].ToString();

                    // }
                    CountTable.Clear();
                    CountTable.Columns.Add("TodaysTasksTotalCount");
                    DataRow row = CountTable.NewRow();
                    row["TodaysTasksTotalCount"] = myFunctions.getIntVAL(TotalCount);
                    CountTable.Rows.Add(row);
                    CountTable = api.Format(CountTable, "CountTable");
                    TaskManager.Tables.Add(TodaysTasks);
                    TaskManager.Tables.Add(CountTable);
                    return Ok(api.Success(TaskManager));



                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }

        }

        [HttpGet("overdueDetails")]
        public ActionResult GetoverdueDetails(int nUserID, DateTime d_Date, int nPage, int nSizeperpage)

        {
            SortedList Params = new SortedList();
            DataSet TaskManager = new DataSet();
            DataTable OverDueTasks = new DataTable();
            DataTable CountTable = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User); 
            int Count = (nPage - 1) * nSizeperpage;

            string sqlOverDueList = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (Count == 0)
                    {
                        sqlOverDueList = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus]  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and  x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE) < '" + d_Date + "' order by N_PriorityID asc";
                    }
                    else
                    {
                        sqlOverDueList = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus]  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + "  and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE) < '" + d_Date + "' and N_TaskID not in (select top(" + Count + ") N_TaskID from [vw_TaskCurrentStatus] where  N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and  D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE) < '" + d_Date + "' order by N_PriorityID asc ) order by N_PriorityID asc";
                    }

                    OverDueTasks = dLayer.ExecuteDataTable(sqlOverDueList, Params, connection);
                    OverDueTasks = api.Format(OverDueTasks, "OverDueTasks");
                    string sqlCommandCount1 = "select count(1) as N_Count from [vw_TaskCurrentStatus]  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_DueDate as DATE) < '" + d_Date + "'";
                    DataTable Summary1 = dLayer.ExecuteDataTable(sqlCommandCount1, Params, connection);
                    string TotalCount1 = "0";
                    if (Summary1.Rows.Count > 0)
                    {
                        DataRow drow = Summary1.Rows[0];
                        TotalCount1 = drow["N_Count"].ToString();
                    }
                    CountTable.Clear();
                    CountTable.Columns.Add("OverDueTasksTotalCount");
                    DataRow row = CountTable.NewRow();
                    row["OverDueTasksTotalCount"] = myFunctions.getIntVAL(TotalCount1);
                    CountTable.Rows.Add(row);
                    CountTable = api.Format(CountTable, "CountTable");
                    TaskManager.Tables.Add(OverDueTasks);
                    TaskManager.Tables.Add(CountTable);
                    return Ok(api.Success(TaskManager));

                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

         [HttpGet("upcomingDetails")]
        public ActionResult GetupcominDetails(int nUserID, DateTime d_Date, int nPage, int nSizeperpage)

        {
            SortedList Params = new SortedList();
            DataSet TaskManager = new DataSet();
            DataTable UpComingTasks = new DataTable();
            DataTable CountTable = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int Count = (nPage - 1) * nSizeperpage;
            string sqlUpcomingList = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (Count == 0)
                    {
                        sqlUpcomingList = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus] where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and   D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_TaskDate as DATE) >  '" + d_Date + "' order by N_PriorityID asc";
                    }
                    else
                    {
                        sqlUpcomingList = "select  top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus] where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and   D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_TaskDate as DATE) >  '" + d_Date + "' and N_TaskID not in (select top(" + Count + ") N_TaskID from [vw_TaskCurrentStatus] where  N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and  D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_TaskDate as DATE) ='" + d_Date + "') order by N_PriorityID asc";
                    }

                    UpComingTasks = dLayer.ExecuteDataTable(sqlUpcomingList, Params, connection);
                    UpComingTasks = api.Format(UpComingTasks, "UpComingTasks");
                    string sqlCommandCount2 = "select count(1) as N_Count from [vw_TaskCurrentStatus] where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and   D_TaskDate>0 and x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and isnull(B_Closed,0) =0 and Cast(D_TaskDate as DATE) >  '" + d_Date + "'";
                    DataTable Summary2 = dLayer.ExecuteDataTable(sqlCommandCount2, Params, connection);
                    string TotalCount2 = "0";
                    if (Summary2.Rows.Count > 0)
                    {
                        DataRow drow = Summary2.Rows[0];
                        TotalCount2 = drow["N_Count"].ToString();

                    }
                    CountTable.Clear();
                    CountTable.Columns.Add("UpComingTasksTotalCount");
                    DataRow row = CountTable.NewRow();
                    row["UpComingTasksTotalCount"] = myFunctions.getIntVAL(TotalCount2);
                    CountTable.Rows.Add(row);
                    CountTable = api.Format(CountTable, "CountTable");
                    TaskManager.Tables.Add(UpComingTasks);
                    TaskManager.Tables.Add(CountTable);

                    return Ok(api.Success(TaskManager));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }

        }



        [HttpGet("followUpDetails")]
        public ActionResult GetollowUpDetails(int nUserID, DateTime d_Date, int nPage, int nSizeperpage)

        {
            SortedList Params = new SortedList();
            DataSet TaskManager = new DataSet();
            DataTable FollowupTasks = new DataTable();
            DataTable CountTable = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int Count = (nPage - 1) * nSizeperpage;

            string sqlFollowUp = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                  //  string sqlParentFilte = " and N_TaskID in (select N_ParentID from Tsk_TaskMaster where N_CompanyID="+nCompanyID+") ";
                   string sqlParentFilte="";
                    if (Count == 0)
                    {
                        sqlFollowUp = "select top(" + nSizeperpage + ") * from vw_Tsk_TaskStatus where  N_CompanyID=" + nCompanyID + " and N_Status <> 1 and N_CreaterID=" + nUserID + " and ISNULL(B_Closed,0)=0 "+sqlParentFilte+" and  N_TaskStatusID in (select Max(N_TaskStatusID) from vw_Tsk_TaskStatus  where N_CompanyID=" + nCompanyID + " and N_Status <> 1 and N_CreaterID="+nUserID+"  and N_AssigneeID <> "+nUserID+" and ISNULL(B_Closed,0)=0  "+sqlParentFilte+"  group by N_TaskID,N_CompanyID) order by N_TaskStatusID desc " ;
                    }
                    else
                    {
                        sqlFollowUp = "select top(" + nSizeperpage + ") * from vw_Tsk_TaskStatus where  N_CompanyID=" + nCompanyID + " and N_Status <> 1 and N_CreaterID=" + nUserID + " and ISNULL(B_Closed,0)=0 "+sqlParentFilte+"  and  N_TaskStatusID in (select Max(N_TaskStatusID) from vw_Tsk_TaskStatus  where  N_CompanyID=" + nCompanyID + " and N_Status <> 1 and N_CreaterID="+nUserID+" and  N_AssigneeID <> "+nUserID+" and ISNULL(B_Closed,0)=0  "+sqlParentFilte+"  group by N_TaskID,N_CompanyID) and  N_TaskID not in (select top(" + Count + ") N_TaskID from vw_Tsk_TaskStatus where  N_CompanyID=" + nCompanyID + " and N_Status <> 1 and N_CreaterID=" + nUserID + " and ISNULL(B_Closed,0)=0  "+sqlParentFilte+"   and N_TaskStatusID in (select Max(N_TaskStatusID) from vw_Tsk_TaskStatus  where  N_CompanyID=" + nCompanyID + " and N_Status <> 1 and N_CreaterID="+nUserID+" and ISNULL(B_Closed,0)=0  "+sqlParentFilte+"    group by N_TaskID,N_CompanyID) order By N_TaskStatusID desc ) order By N_TaskStatusID desc";
                    }

                    FollowupTasks = dLayer.ExecuteDataTable(sqlFollowUp, Params, connection);
                    FollowupTasks = api.Format(FollowupTasks, "FollowUpTasks");
                    string sqlCommandCount3 = "select count(1) as N_Count from vw_Tsk_TaskStatus where  N_CompanyID=" + nCompanyID + " and N_Status <> 1 and N_CreaterID=" + nUserID + " and ISNULL(B_Closed,0)=0  "+sqlParentFilte+" and N_TaskStatusID in (select Max(N_TaskStatusID) from vw_Tsk_TaskStatus  where N_CompanyID=" + nCompanyID + " and N_Status <> 1 and N_CreaterID="+nUserID+" and N_AssigneeID <> "+nUserID+" and ISNULL(B_Closed,0)=0  "+sqlParentFilte+"  group by N_TaskID,N_CompanyID)";
                    DataTable Summary3 = dLayer.ExecuteDataTable(sqlCommandCount3, Params, connection);
                    string TotalCount3 = "0";
                    if (Summary3.Rows.Count > 0)
                    {
                        DataRow drow = Summary3.Rows[0];
                        TotalCount3 = drow["N_Count"].ToString();

                    }
                    CountTable.Clear();
                    CountTable.Columns.Add("FollowUpTasksTotalCount");
                    DataRow row = CountTable.NewRow();
                    row["FollowUpTasksTotalCount"] = myFunctions.getIntVAL(TotalCount3);
                    CountTable.Rows.Add(row);
                    CountTable = api.Format(CountTable, "CountTable");
                    TaskManager.Tables.Add(FollowupTasks);
                    TaskManager.Tables.Add(CountTable);
                    return Ok(api.Success(TaskManager));


                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

        
        [HttpGet("completedTaskDetails")]
        public ActionResult GetcompletedDetails(int nUserID, DateTime d_Date, int nPage, int nSizeperpage)

        {
             SortedList Params = new SortedList();
            DataSet TaskManager = new DataSet();
            DataTable todayCompletedTasks = new DataTable();
            DataTable CountTable = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int Count = (nPage - 1) * nSizeperpage;
            string sqlUpcomingList = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (Count == 0)
                    {
                        sqlUpcomingList = "select top(" + nSizeperpage + ") * from vw_Tsk_TaskCompletedStatus where N_CompanyID=" + nCompanyID + " and N_CreaterID=" + nUserID + "  and N_Status in(5,4,9,10) and Cast(D_EntryDate as DATE) =  '" + d_Date + "' order by N_TaskID desc";
                    }
                    else
                    {
                        sqlUpcomingList = "select  top(" + nSizeperpage + ") * from vw_Tsk_TaskCompletedStatus where N_CompanyID=" + nCompanyID + " and N_CreaterID=" + nUserID + " and   N_Status in(5,4,9,10) and Cast(D_EntryDate as DATE) =  '" + d_Date + "'  and N_TaskID not in (select top(" + Count + ") N_TaskID from vw_Tsk_TaskCompletedStatus where  N_CompanyID=" + nCompanyID + " and N_CreaterID=" + nUserID + "  and  Cast(D_TaskDate as DATE)='" + d_Date + "') order by N_PriorityID asc";
                    }

                    todayCompletedTasks = dLayer.ExecuteDataTable(sqlUpcomingList, Params, connection);
                    todayCompletedTasks = api.Format(todayCompletedTasks, "todayCompletedTasks");
                    string sqlCommandCount2 = "select count(1) as N_Count from vw_Tsk_TaskCompletedStatus where N_CompanyID=" + nCompanyID + " and N_CreaterID=" + nUserID + " and  N_Status in(5,4,9,10) and Cast(D_EntryDate as DATE) =  '" + d_Date + "' ";
                    DataTable Summary2 = dLayer.ExecuteDataTable(sqlCommandCount2, Params, connection);
                    string TotalCount2 = "0";
                    if (Summary2.Rows.Count > 0)
                    {
                        DataRow drow = Summary2.Rows[0];
                        TotalCount2 = drow["N_Count"].ToString();

                    }
                    CountTable.Clear();
                    CountTable.Columns.Add("todayCompletedTasks");
                    DataRow row = CountTable.NewRow();
                    row["todayCompletedTasks"] = myFunctions.getIntVAL(TotalCount2);
                    CountTable.Rows.Add(row);
                    CountTable = api.Format(CountTable, "CountTable");
                    TaskManager.Tables.Add(todayCompletedTasks);
                    TaskManager.Tables.Add(CountTable);

                    return Ok(api.Success(TaskManager));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }

        }


         [HttpGet("openTaskList")]
        public ActionResult GetOpenTaskDetails(int nUserID, DateTime d_Date, int nPage, int nSizeperpage)

        {
            SortedList Params = new SortedList();
            DataSet TaskManager = new DataSet();
            DataTable OverDueTasks = new DataTable();
            DataTable CountTable = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User); 
            int Count = (nPage - 1) * nSizeperpage;

            string sqlOverDueList = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (Count == 0)
                    {
                        sqlOverDueList = "select top(" + nSizeperpage + ") * from vw_Tsk_TaskCurrentStatus where N_CompanyID=" + nCompanyID + " and N_CreaterID=" + nUserID + "  and isnull(N_AssigneeID,0)=0  and  x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' order by D_TaskDate desc";
                    }
                    else
                    {
                        sqlOverDueList = "select top(" + nSizeperpage + ") * from vw_Tsk_TaskCurrentStatus  where N_CompanyID=" + nCompanyID + " and N_CreaterID=" + nUserID + "  and isnull(N_AssigneeID,0)=0  and  x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and N_TaskID not in (select top(" + Count + ") N_TaskID from [vw_Tsk_TaskCurrentStatus] where  N_CompanyID=" + nCompanyID + " and N_CompanyID=" + nCompanyID + " and N_CreaterID=" + nUserID + "  and isnull(N_AssigneeID,0)=0  and  x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' order by D_TaskDate desc ) order by D_TaskDate desc";
                    }

                    OverDueTasks = dLayer.ExecuteDataTable(sqlOverDueList, Params, connection);
                    OverDueTasks = api.Format(OverDueTasks, "openTasks");
                    string sqlCommandCount1 = "select count(1) as N_Count from vw_Tsk_TaskCurrentStatus  where N_CompanyID=" + nCompanyID + " and N_CreaterID=" + nUserID + "  and isnull(N_AssigneeID,0)=0  and  x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed'";
                    DataTable Summary1 = dLayer.ExecuteDataTable(sqlCommandCount1, Params, connection);
                    string TotalCount1 = "0";
                    if (Summary1.Rows.Count > 0)
                    {
                        DataRow drow = Summary1.Rows[0];
                        TotalCount1 = drow["N_Count"].ToString();
                    }
                    CountTable.Clear();
                    CountTable.Columns.Add("OpenTasksTotalCount");
                    DataRow row = CountTable.NewRow();
                    row["OpenTasksTotalCount"] = myFunctions.getIntVAL(TotalCount1);
                    CountTable.Rows.Add(row);
                    CountTable = api.Format(CountTable, "CountTable");
                    TaskManager.Tables.Add(OverDueTasks);
                    TaskManager.Tables.Add(CountTable);
                    return Ok(api.Success(TaskManager));

                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
       [HttpGet("submitTaskList")]
        public ActionResult GetSubmitTaskDetails(int nUserID, DateTime d_Date, int nPage, int nSizeperpage)

        {
            SortedList Params = new SortedList();
            DataSet TaskManager = new DataSet();
            DataTable SubmitTasks = new DataTable();
            DataTable CountTable = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User); 
            int Count = (nPage - 1) * nSizeperpage;

            string sqlOverDueList = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (Count == 0)
                    {
                        sqlOverDueList = "select top(" + nSizeperpage + ") * from vw_Tsk_TaskCurrentStatus where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and N_Status=4 and  x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' order by D_DueDate asc";
                    }
                    else
                    {
                        sqlOverDueList = "select top(" + nSizeperpage + ") * from vw_Tsk_TaskCurrentStatus  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and N_Status=4 and  x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' and N_TaskID not in (select top(" + Count + ") N_TaskID from [vw_Tsk_TaskCurrentStatus] where  N_CompanyID=" + nCompanyID + " and N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and N_Status=4  and  x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed' order by D_DueDate asc ) order by D_DueDate asc";
                    }

                    SubmitTasks = dLayer.ExecuteDataTable(sqlOverDueList, Params, connection);
                    SubmitTasks = api.Format(SubmitTasks, "submitTasks");
                    string sqlCommandCount1 = "select count(1) as N_Count from vw_Tsk_TaskCurrentStatus  where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + "  and N_Status=4 and  x_tasksummery<> 'Project Created' and x_tasksummery<>'Project Closed'";
                    DataTable Summary1 = dLayer.ExecuteDataTable(sqlCommandCount1, Params, connection);
                    string TotalCount1 = "0";
                    if (Summary1.Rows.Count > 0)
                    {
                        DataRow drow = Summary1.Rows[0];
                        TotalCount1 = drow["N_Count"].ToString();
                    }
                    CountTable.Clear();
                    CountTable.Columns.Add("SubmitTasksTotalCount");
                    DataRow row = CountTable.NewRow();
                    row["SubmitTasksTotalCount"] = myFunctions.getIntVAL(TotalCount1);
                    CountTable.Rows.Add(row);
                    CountTable = api.Format(CountTable, "CountTable");
                    TaskManager.Tables.Add(SubmitTasks);
                    TaskManager.Tables.Add(CountTable);
                    return Ok(api.Success(TaskManager));

                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
        [HttpGet("sprintTasks")]
        public ActionResult GetcompletedDetails(int nUserID,int nSprintID )
        {
             SortedList Params = new SortedList();
             DataTable dt = new DataTable();
             int nCompanyID=myFunctions.GetCompanyID(User);
             Params.Add("@nCompanyID",nCompanyID);
             string sqlCommandText="Select * from Vw_TaskCurrentStatus Where N_CompanyID=@nCompanyID and N_SprintID="+nSprintID+"" ;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    //return Ok(api.Notice("No Results Found"));
                    return Ok(api.Success(dt));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }


          [HttpGet("teamEmployee")]
        public ActionResult GetcompletedDetails1(int nUserID,int nUserMappingID)
        {
             SortedList Params = new SortedList();
             DataTable dt = new DataTable();
             int nCompanyID=myFunctions.GetCompanyID(User);
             string x_Critiria="";
             Params.Add("@nCompanyID",nCompanyID);
             if(nUserMappingID>0)
                x_Critiria=" and N_UserMappingID="+nUserMappingID;
                
             string sqlCommandText="Select X_UserName,N_UsersID from vw_tsk_TeamEmployee Where N_CompanyID=@nCompanyID and N_UserID="+nUserID+" "+x_Critiria + " group by X_UserName,N_UsersID" ;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    //return Ok(api.Notice("No Results Found"));
                    return Ok(api.Success(dt));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
        
    }
}

