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

// namespace SmartxAPI.Controllers
// {
//     [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//     [Route("schFeeSetup")]
//     [ApiController]
//     public class Sch_FeeCodes : ControllerBase
//     {
//         private readonly IApiFunctions _api;
//         private readonly IDataAccessLayer dLayer;
//         private readonly int FormID;
//         private readonly IMyFunctions myFunctions;
//         private readonly IMyAttachments myAttachments;

//         public Sch_FeeCodes(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
//         {
//             _api = api;
//             dLayer = dl;
//             myFunctions = myFun;
//             myAttachments = myAtt;
//             connectionString = conf.GetConnectionString("SmartxConnection");
//             FormID = 1056;
//         }
//         private readonly string connectionString;
