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
//     [Route("bankreconsil")]
//     [ApiController]
//     public class Acc_BankReconcilation : ControllerBase
//     {
//         private readonly IDataAccessLayer dLayer;
//         private readonly IApiFunctions _api;
//         private readonly IMyFunctions myFunctions;
//         private readonly string connectionString;
//         private readonly int FormID;


//         public Acc_BankReconcilation(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
//         {
//             dLayer = dl;
//             _api = api;
//             myFunctions = myFun;
//             connectionString = conf.GetConnectionString("SmartxConnection");
//             FormID = 217;
//         }

//         [HttpPost("save")]
//         public ActionResult SaveData([FromBody] DataSet ds)
//         {
//             try
//             {
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     connection.Open();
//                     SqlTransaction transaction = connection.BeginTransaction();
//                     DataTable MasterTable;
//                     MasterTable = ds.Tables["master"];
//                     SortedList Params = new SortedList();
//                     // int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
//                     int nReconcilID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ReconcilID"].ToString());
//                     // int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
//                       // string xBankCode = MasterTable.Rows[0]["x_BankCode"].ToString();

//                     MasterTable.AcceptChanges();

//                     // if (xBankCode == "@Auto")
//                     // {
//                     //     Params.Add("N_CompanyID", nCompanyID);
//                     //     Params.Add("N_YearID", nFnYearID);
//                     //     Params.Add("N_FormID", this.FormID);
//                     //     xBankCode = dLayer.GetAutoNumber("Acc_BankReconsilation", "x_BankCode", Params, connection, transaction);
//                     //     if (xBankCode == "")
//                     //     {
//                     //         transaction.Rollback();
//                     //         return Ok(_api.Error(User, "Unable to generate Bank Code"));
//                     //     }
//                     //     MasterTable.Rows[0]["x_BankCode"] = xBankCode;
//                     // }

//                     nReconcilID = dLayer.SaveData("Acc_BankReconcilation", "N_ReconcilID", MasterTable, connection, transaction);
//                     if (nReconcilID <= 0)
//                     {
//                         transaction.Rollback();
//                         return Ok(_api.Warning("Unable to save"));
//                     }
//                     else
//                     {
//                         transaction.Commit();
//                         return Ok(_api.Success("BankReconcilation Saved"));
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 return Ok(_api.Error(User, ex));
//             }
//         }


//     }

// }