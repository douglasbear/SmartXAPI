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

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable SaveDataTable;
                DataTable PartyListTable;
                SaveDataTable = ds.Tables["master"];
                PartyListTable = ds.Tables["partylist"];
                int nCompanyID = myFunctions.GetCompanyID(User);
                int nFnYearID = myFunctions.getIntVAL(PartyListTable.Rows[0]["n_FnYearID"].ToString());
                string xTransType = PartyListTable.Rows[0]["x_TransType"].ToString();
                int nUserID = myFunctions.GetUserID(User);
                int nBranchID = myFunctions.getIntVAL(PartyListTable.Rows[0]["n_BranchID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    for (int j = 0; j < PartyListTable.Rows.Count; j++)
                    {
                        int nPartyID = myFunctions.getIntVAL(PartyListTable.Rows[j]["nPartyID"].ToString());

                        int nSalesID = dLayer.SaveData("Inv_Sales", "N_SalesID", SaveDataTable, connection, transaction);

                        if (nSalesID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to save"));
                        }

                        SortedList ProcParam = new SortedList();
                        ProcParam.Add("N_CompanyID", nCompanyID);
                        ProcParam.Add("N_FnYearID", nFnYearID);
                        ProcParam.Add("X_TransType", xTransType);
                        ProcParam.Add("N_UserID", nUserID);
                        ProcParam.Add("N_PartyID", nPartyID);
                        ProcParam.Add("N_BranchID", nBranchID);
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Acc_BeginingBalancePosting_Ins", ProcParam, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }
                    }

                    transaction.Commit();
                    return Ok(_api.Success("Opening Balance Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
    }
}
