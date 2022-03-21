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
//         public ActionResult GetDashboardDetails(int nFnYearId)
//         {
//             SortedList Params = new SortedList();
//             int nCompanyID = myFunctions.GetCompanyID(User);
//             int nUserID = myFunctions.GetUserID(User);
//             string UserPattern = myFunctions.GetUserPattern(User);
//             string Pattern = "";
//             string AssigneePattern = "";
//              string sqlActiveEmployees = "SELECT COUNT(*) as N_ActiveEmloyees FROM vw_CRMLeads WHERE N_CompanyID="+nCompanyID+" and N_Status not in (2,3)";//Active employees
//              string sqlToDoList="select     COUNT(*) as N_ToDoList "

