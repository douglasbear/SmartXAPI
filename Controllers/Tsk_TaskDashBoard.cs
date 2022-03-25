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
            string sqlActiveEmployees = "SELECT COUNT(*) as N_ActiveEmloyees FROM vw_CRMLeads WHERE N_CompanyID=" + nCompanyID + " and N_Status not in (2,3)";//Active employees
            string sqlScheduledList = "select  COUNT(*) as N_ToDoList from  vw_Tsk_TaskCurrentStatus where N_CompanyID=" + nCompanyID + " and N_AssigneeID=" + nUserID + " and and isnull(B_Closed,0) =0";
            string sqlActiveList = "select Count(*) as N_ActiveList from vw_Tsk_TaskCurrentStatus where N_Status=7 and N_CompanyID=" + nCompanyID + " and  N_CreaterID=" + nUserID + "";
            string sqlCompletedList = "select Count(*) from N_CompletedList from vw_Tsk_TaskCurrentStatus where N_CompanyID=" + nCompanyID + " and N_CreaterID=" + nUserID + " and N_Status=4";
            string sqlClosedList = "select Count(*) from N_CompletedList from vw_Tsk_TaskCurrentStatus where N_CompanyID=" + nCompanyID + " and N_CreaterID=" + nUserID + " and N_Status=5";
            string sqlTaskStatus = "";
            SortedList Data = new SortedList();
            DataTable ActiveEmployees = new DataTable();
            DataTable ScheduledList = new DataTable();
            DataTable ActiveList = new DataTable();
            DataTable CompletedList = new DataTable();
            DataTable ClosedList = new DataTable();
            DataTable TaskStatus = new DataTable();


            object ActiveEmp = "";
            object ScheduledTask = "";
            object ActiveTask = "";
            object CompletedTask = "";
            object ClosedTask = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
              
                {
                    connection.Open();
                    ActiveEmp = dLayer.ExecuteDataTable(sqlActiveEmployees, Params, connection);
                    ScheduledTask = dLayer.ExecuteDataTable(sqlScheduledList, Params, connection);
                    ActiveTask = dLayer.ExecuteDataTable(sqlActiveList, Params, connection);
                    CompletedTask = dLayer.ExecuteDataTable(sqlCompletedList, Params, connection);
                    ClosedTask = dLayer.ExecuteDataTable(sqlClosedList, Params, connection);

                    ActiveEmployees = myFunctions.AddNewColumnToDataTable(ActiveEmployees, "N_ActiveEmloyees", typeof(string), ActiveEmp);
                    ActiveEmployees.AcceptChanges();
                    ScheduledList = myFunctions.AddNewColumnToDataTable(ScheduledList, "N_ScheduledTask", typeof(string), ScheduledTask);
                    ScheduledList.AcceptChanges();
                    ActiveList = myFunctions.AddNewColumnToDataTable(ActiveList, "N_ActiveTask", typeof(string), ActiveTask);
                    ActiveList.AcceptChanges();
                    CompletedList = myFunctions.AddNewColumnToDataTable(CompletedList, "N_CompletedTask", typeof(string), CompletedTask);
                    CompletedList.AcceptChanges();
                    ClosedList = myFunctions.AddNewColumnToDataTable(ClosedList, "N_ClosedTask", typeof(string), ClosedTask);
                    ClosedList.AcceptChanges();
                    

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

                if (ActiveEmployees.Rows.Count > 0) Data.Add("ActiveEmployees", ActiveEmployees);
                if (ScheduledList.Rows.Count > 0) Data.Add("ScheduledList", ScheduledList);
                if (ActiveList.Rows.Count > 0) Data.Add("ActiveList", ActiveList);
                if (CompletedList.Rows.Count > 0) Data.Add("CompletedList", CompletedList);
                if (ClosedList.Rows.Count > 0) Data.Add("ClosedList", ClosedList);
                if (ClosedList.Rows.Count > 0) Data.Add("ClosedList", ClosedList);
                if (TaskStatus.Rows.Count > 0) Data.Add("TaskStatus", TaskStatus);

               
                return Ok(api.Success(Data));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }







    }
}






