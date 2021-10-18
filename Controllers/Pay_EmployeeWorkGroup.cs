// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using System;
// using SmartxAPI.GeneralFunctions;
// using System.Data;
// using System.Collections;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Data.SqlClient;
// using System.Net.Mail;
// using System.Text;
// using System.IO;
// using System.Threading.Tasks;
// namespace SmartxAPI.Controllers
// {
//     [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//     [Route("employeeWorkGroyp")]
//     [ApiController]
//     public class Pay_EmployeeWorkGroup : ControllerBase
//     {
//         private readonly IDataAccessLayer dLayer;
//         private readonly IApiFunctions _api;
//         private readonly IMyFunctions myFunctions;
//         private readonly string connectionString;
//         private readonly int FormID;
//         StringBuilder message = new StringBuilder();
//         public Pay_EmployeeWorkGroup(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
//         {
//             dLayer = dl;
//             _api = api;
//             dLayer = dl;
//             myFunctions = myFun;
//             connectionString = conf.GetConnectionString("SmartxConnection");
//             FormID = 716;

//         }

