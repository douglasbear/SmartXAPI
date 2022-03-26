// using AutoMapper;
// using SmartxAPI.Data;
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
//     [Route("taskDashBoard")]
//     [ApiController]
//         public class Tsk_TaskDashBoard : ControllerBase
//     {
//         private readonly IApiFunctions api;
//         private readonly IDataAccessLayer dLayer;
//         private readonly IMyFunctions myFunctions;
//         private readonly string connectionString;

//         public Tsk_TaskDashBoard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
//         {
//             api = apifun;
//             dLayer = dl;
//             myFunctions = myFun;
//             connectionString = conf.GetConnectionString("SmartxConnection");
//         }
//           [HttpGet("details")]
//         public ActionResult GetDashboardDetails(int nFnYearId,int nUserID)
//         {
//             SortedList Params = new SortedList();
//             int nCompanyID = myFunctions.GetCompanyID(User);
           
//             string UserPattern = myFunctions.GetUserPattern(User);
//             string Pattern = "";
//             string AssigneePattern = "";
//              string sqlActiveEmployees = "SELECT COUNT(*) as N_ActiveEmloyees FROM vw_CRMLeads WHERE N_CompanyID="+nCompanyID+" and N_Status not in (2,3)";//Active employees
//             string sqlScheduledList="select  COUNT(*) as N_ToDoList from  vw_Tsk_TaskCurrentStatus where N_CompanyID="+nCompanyID+" and N_AssigneeID="+nUserID+"";
//              string sqlActiveList="select Count(*) as N_ActiveList from vw_Tsk_TaskCurrentStatus where N_Status=7 and N_CompanyID="+nCompanyID+" and  N_CreaterID="+nUserID+"";
//              string  sqlCompletedList="select Count(*) from N_CompletedList from vw_Tsk_TaskCurrentStatus where N_CompanyID="+nCompanyID+" and N_CreaterID="+nUserID+" and N_Status=4";
//              string sqlClosedList ="select Count(*) from N_CompletedList from vw_Tsk_TaskCurrentStatus where N_CompanyID="+nCompanyID+" and N_CreaterID="+nUserID+" and N_Status=4";
//              SortedList Data=new SortedList();
//             DataTable ActiveEmployees = new DataTable();
//             DataTable ScheduledList = new DataTable();
//             DataTable ActiveList = new DataTable();
//             DataTable CompletedList = new DataTable();
//             DataTable ClosedList = new DataTable();

   
//             object ActiveEmp="";
//             object ScheduledTask="";
//             object ActiveTask="";
//             object CompletedTask="";
//             object ClosedTask="";
//                try
//             {
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     connection.Open();
//                      ActiveEmp = dLayer.ExecuteDataTable(sqlActiveEmployees, Params, connection);
//                     ScheduledTask = dLayer.ExecuteDataTable(sqlScheduledList, Params, connection);
//                     ActiveTask = dLayer.ExecuteDataTable(sqlActiveList, Params, connection);
//                     CompletedTask = dLayer.ExecuteDataTable(sqlCompletedList, Params, connection);
//                     ClosedTask = dLayer.ExecuteDataTable(sqlClosedList, Params, connection);
                      
//                 ActiveEmployees = myFunctions.AddNewColumnToDataTable(ActiveEmployees, "N_ActiveEmloyees", typeof(string),ActiveEmp);
//                 ScheduledTask = myFunctions.AddNewColumnToDataTable(CurrentLead, "N_Percentage", typeof(string),LeadPercentage);
//                 CurrentLead.AcceptChanges();
//                 CurrentCustomer = myFunctions.AddNewColumnToDataTable(CurrentCustomer, "N_LastMonth", typeof(string), CustomerLastMonth);
//                 CurrentCustomer = myFunctions.AddNewColumnToDataTable(CurrentCustomer, "N_Percentage", typeof(string), CustomerPercentage);
//                 CurrentCustomer.AcceptChanges();
//                 CurrentRevenue = myFunctions.AddNewColumnToDataTable(CurrentRevenue, "N_LastMonth", typeof(string), RevenueLastMonth);
//                 CurrentRevenue = myFunctions.AddNewColumnToDataTable(CurrentRevenue, "N_Percentage", typeof(string),RevenuePercentage);
//                 CurrentRevenue.AcceptChanges();



                    



