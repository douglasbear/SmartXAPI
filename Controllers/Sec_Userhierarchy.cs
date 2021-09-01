// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using System;
// using SmartxAPI.GeneralFunctions;
// using System.Data;
// using System.Collections;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Data.SqlClient;
// using System.ComponentModel;
// using System.Collections.Generic;


// namespace SmartxAPI.Controllers
// {
//     [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//     [Route("secUserHierarchy")]
//     [ApiController]
//     public class Sec_Userhierarchy : ControllerBase
//     {


//         private readonly IDataAccessLayer dLayer;
//         private readonly IApiFunctions _api;
//         private readonly IMyFunctions myFunctions;
//         private readonly string connectionString;
//         private readonly int N_FormID;
//         private readonly string reportPath;
//         public Sec_Userhierarchy(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
//         {
//             dLayer = dl;
//             _api = api;
//             myFunctions = myFun;
//             connectionString = conf.GetConnectionString("SmartxConnection");
//             N_FormID = 1349;//form id of cost center
//             reportPath = conf.GetConnectionString("ReportPath");
//         }

//         [HttpGet("chart")]
//         public ActionResult GetCategoryChart()

//         {
//             DataTable dt = new DataTable();
//             DataTable Images = new DataTable();
//             SortedList Params = new SortedList();
//             int nCompanyID = myFunctions.GetCompanyID(User);
//             Params.Add("@nCompanyID", nCompanyID);


//             string sqlCommandText = "Select *  from sec_UserHierarchy Where N_CompanyID= " + nCompanyID + " Order By X_Pattern";


//             try
//             {
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     connection.Open();
//                     dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

//                 }

//                 dt = _api.Format(dt);
//                 if (dt.Rows.Count == 0)
//                 {
//                     return Ok(_api.Notice("No Results Found"));
//                 }
//                 else
//                 {
//                     return Ok(_api.Success(dt));
//                 }
//             }
//             catch (Exception e)
//             {
//                 return Ok(_api.Error(e));
//             }

//         }
//         [HttpPost("save")]
//         public ActionResult SaveData([FromBody] DataSet ds)
//         {
//             try
//             {

//                  DataTable MasterTable;
//                 MasterTable = ds.Tables["master"];
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     connection.Open();
//                     SqlTransaction transaction = connection.BeginTransaction();
//                     SortedList Params = new SortedList();
//                     SortedList QueryParams = new SortedList();
//                     DataRow MasterRow = MasterTable.Rows[0];



