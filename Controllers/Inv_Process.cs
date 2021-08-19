// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using SmartxAPI.GeneralFunctions;
// using System;
// using System.Data;
// using System.Collections;
// using Microsoft.Data.SqlClient;
// using Microsoft.Extensions.Configuration;
// using System;

// namespace SmartxAPI.Controllers

// {
//     [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//     [Route("productionOrder")]
//     [ApiController]
//     public class Inv_Process : ControllerBase
//     {

//         private readonly IApiFunctions _api;
//         private readonly IDataAccessLayer dLayer;
//         private readonly IMyFunctions myFunctions;
//         private readonly IMyAttachments myAttachments;
//         private readonly string connectionString;
//         private readonly int N_FormID;

//         public Inv_Process(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf, IMyAttachments myAtt)
//         {
//             _api = api;
//             dLayer = dl;
//             myFunctions = fun;
//             myAttachments = myAtt;
//             connectionString = conf.GetConnectionString("SmartxConnection");
//             N_FormID = 54;
//         }
//         [HttpGet("list")]
//         public ActionResult ProductionOrderList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
//         {
//                     DataTable dt = new DataTable();
//             SortedList Params = new SortedList();
//             int nCompanyID = myFunctions.GetCompanyID(User);
//             string sqlCommandCount = "";
//             int Count = (nPage - 1) * nSizeperpage;
//             string sqlCommandText = "";
//             string Searchkey = "";
//             Params.Add("@p1", nCompanyID);
//             Params.Add("@p2", nFnYearId);
//              if (xSearchkey != null && xSearchkey.Trim() != "")
//                 Searchkey = "and ( X_BatchCode like '%" + xSearchkey + "%' or  X_PayrunText like '%" + xSearchkey + "%' or X_DateFrom like '%" + xSearchkey + "%' or X_DateTo like '%" + xSearchkey + "%' ) ";

//             if (xSortBy == null || xSortBy.Trim() == "")
//                 xSortBy = " order by N_TimeSheetID desc";
//             // xSortBy = " order by batch desc,D_TransDate desc";
//             else
//                 xSortBy = " order by " + xSortBy;




