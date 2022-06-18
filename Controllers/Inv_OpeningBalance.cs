using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("invOpeningBalance")]
    [ApiController]
    public class inv_OpeningBalance : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public inv_OpeningBalance(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 208;
        }


        [HttpGet("partyList")]
        public ActionResult getPartyList(int nFlag, int nFnYearID, int nBranchID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable partylist = new DataTable();
                    DataTable settings = new DataTable();
                    DataTable details = new DataTable();
                    int nCompanyID = myFunctions.GetCompanyID(User);

                    SortedList ProParams = new SortedList();
                    ProParams.Add("N_CompanyID", nCompanyID);
                    ProParams.Add("N_Flag", nFlag);
                    ProParams.Add("N_FnYearID", nFnYearID);
                    ProParams.Add("N_BranchID",nBranchID);

                    SqlTransaction transaction = connection.BeginTransaction();
                    partylist = dLayer.ExecuteDataTablePro("Sp_Inv_OpeningBalance_View", ProParams, connection,transaction);
                    details = dLayer.ExecuteDataTablePro("Sp_Inv_OpeningBalance_DispAll", ProParams, connection,transaction);

                    settings.Clear();
                    settings.Columns.Add("N_CompanyID");
                    settings.Columns.Add("B_FinancialEntryOpen");
                    settings.Columns.Add("B_CustomerPO");
                    settings.Columns.Add("B_VendorPO");

                    DataRow row = settings.NewRow();
                    row["N_CompanyID"] = myFunctions.GetCompanyID(User);
                    row["B_FinancialEntryOpen"] = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Financial", "FinancialEntryOpen", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection,transaction)));
                    row["B_CustomerPO"] = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("64", "EnableCustomerPO", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection,transaction)));
                    row["B_VendorPO"] = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("65", "EnableVendorPO", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection,transaction)));

                    if (partylist.Rows.Count == 0)
                    { 
                        transaction.Rollback();
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        SortedList Output = new SortedList();
                        Output.Add("partylist",partylist);
                        Output.Add("settings",settings);
                        transaction.Commit();
                        return Ok(_api.Success(Output));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        // [HttpPost("save")]
        // public ActionResult SaveData([FromBody] DataSet ds)
        // {
        //     try
        //     {
        //         DataTable MasterTable;
        //         MasterTable = ds.Tables["master"];
        //         int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
        //         int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
        //         int N_SalesID = myFunctions.getIntVAL(MasterRow["n_SalesID"].ToString());

        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();
        //             SortedList Params = new SortedList();

        //             // Auto Gen
        //             string InvoiceNo = "";
        //             var values = MasterTable.Rows[0]["x_ReceiptNo"].ToString();
        //             if (values == "@Auto")
        //             {
        //                 Params.Add("N_CompanyID", nCompanyID);
        //                 Params.Add("N_YearID", nFnYearID);
        //                 Params.Add("N_FormID", this.FormID);
        //                 InvoiceNo = dLayer.GetAutoNumber("Inv_Sales", "x_ReceiptNo", Params, connection, transaction);
        //                 if (xEvaluationCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Code")); }
        //                 MasterTable.Rows[0]["x_ReceiptNo"] = InvoiceNo;
        //             }
        //             nSalesID = dLayer.SaveData("Inv_Sales", "N_SalesID", MasterTable, connection, transaction);
        //             if (nSalesID <= 0)
        //             {
        //                 transaction.Rollback();
        //                 return Ok(_api.Error(User, "Unable to save"));
        //             }

        //             for (int j = 0; j < DetailTable.Rows.Count; j++)
        //             {
        //                 DetailTable.Rows[j]["N_EvaluationID"] = nEvaluationID;
        //             }
        //             nEvaluationDetailsID = dLayer.SaveData("Pay_EmpEvaluationSettingsDetails", "N_EvaluationDetailsID", DetailTable, connection, transaction);
        //             if (nEvaluationDetailsID <= 0)
        //             {
        //                 transaction.Rollback();
        //                 return Ok(_api.Error(User, "Unable to save"));
        //             }

        //             dLayer.DeleteData("Pay_EmpEvaluators", "N_EvaluationID", nEvaluationID, "", connection, transaction);
        //             for (int j = 0; j < EmpEvalTable.Rows.Count; j++)
        //             {
        //                 EmpEvalTable.Rows[j]["N_EvaluationID"] = nEvaluationID;
        //             }
        //             nEvaluatorsDetailsID = dLayer.SaveData("Pay_EmpEvaluators", "N_EvaluatorsDetailsID", EmpEvalTable, connection, transaction);
        //             // if (nEvaluatorsDetailsID <= 0)
        //             // {
        //             //     transaction.Rollback();
        //             //     return Ok(_api.Error(User, "Unable to save"));
        //             // }
        //             transaction.Commit();
        //             return Ok(_api.Success("Employee Evaluation Settings Saved"));
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(_api.Error(User, ex));
        //     }
        // }
    }
}
