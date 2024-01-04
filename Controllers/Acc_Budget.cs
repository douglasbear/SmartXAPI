using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("accBudget")]
    [ApiController]
    public class Acc_Budget : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Acc_Budget(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1843;
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    int nBudgetID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BudgetID"].ToString());
                    string xBudgetCode = MasterTable.Rows[0]["x_BudgetCode"].ToString();

                    if (xBudgetCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", 1843);
                        xBudgetCode = dLayer.GetAutoNumber("Acc_BudgetMaster", "x_BudgetCode", Params, connection, transaction);
                        if (xBudgetCode == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to generate Budget Code"));
                        }
                        MasterTable.Rows[0]["x_BudgetCode"] = xBudgetCode;
                    }
                    MasterTable.AcceptChanges();
                    
                    nBudgetID = dLayer.SaveData("Acc_BudgetMaster", "n_BudgetID", MasterTable, connection, transaction);
                    if (nBudgetID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("Unable to save"));
                    }

                    int nBudgetDetailsID = dLayer.SaveData("Acc_BudgetDetails", "n_BudgetDetailsID", DetailTable, connection, transaction);
                    if (nBudgetDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Budget Saved Successfully"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nBudgetID)
        {
            int Results = 0;
            SortedList Params = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SqlTransaction transaction = connection.BeginTransaction();

                    Params.Add("@n_BudgetID", nBudgetID);
        
                    dLayer.DeleteData("Acc_BudgetDetails", "n_BudgetID", nBudgetID, "", connection, transaction);
                    Results = dLayer.DeleteData("Acc_BudgetMaster", "n_BudgetID", nBudgetID, "", connection, transaction);
                    
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Budget Deleted Successfully"));
                    }
                    else
                    {
                       transaction.Rollback();
                       return Ok(_api.Error(User, "Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("details")]
        public ActionResult GetData(string xBudgetCode)
        {
            DataSet dt = new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            SortedList Params = new SortedList();

            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from vw_Acc_BudgetMaster where N_CompanyID=@n_CompanyID and X_BudgetCode=@x_BudgetCode ";
            Params.Add("@n_CompanyID", nCompanyID);
            Params.Add("@x_BudgetCode", xBudgetCode);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    if (MasterTable.Rows.Count == 0){
                        return Ok(_api.Warning("No Results Found"));
                    }
                    MasterTable.AcceptChanges();

                    int nBudgetID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BudgetID"].ToString());
                    Params.Add("@n_BudgetID", nBudgetID);

                    string sqlCommandText2 = "select * from vw_Acc_BudgetDetailsWithBase where N_CompanyID=@n_CompanyID and N_BudgetID=@n_BudgetID ";

                    DetailTable = dLayer.ExecuteDataTable(sqlCommandText2, Params, connection);
                }
                MasterTable = _api.Format(MasterTable, "Master");
                DetailTable = _api.Format(DetailTable, "Details");
                dt.Tables.Add(MasterTable);
                dt.Tables.Add(DetailTable);
                
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

         [HttpGet("baseyearlist")]
        public ActionResult GetbaseyearList(int nCompanyID,int nBudgetTypeID)
        {
            DataTable dt = new DataTable();
            SortedList Params=new SortedList();
            string sqlCommandText = "select N_Year from Acc_BudgetMaster WHERE N_CompanyID=@p1 and N_BudgetTypeID=@p2 GROUP BY N_Year";
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nBudgetTypeID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
          [HttpGet("basemonthlist")]
        public ActionResult GetbasemonthList(int nCompanyID,int nBudgetTypeID)
        {
            DataTable dt = new DataTable();
            SortedList Params=new SortedList();
            string sqlCommandText = "select X_Month from Acc_BudgetMaster WHERE N_CompanyID=@nCompanyID AND N_Year=@nYear and N_BudgetTypeID=@nBudgetTypeID GROUP BY X_Month";
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nBudgetTypeID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpGet("baseDetails")]
        public ActionResult GetBaseBudgetDetails(int nBudgetID)
        {
            DataTable dt = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SortedList mParamsList = new SortedList();
                    mParamsList.Add("N_CompanyID", nCompanyID);
                    mParamsList.Add("N_BudgetID", nBudgetID);

                    dt = dLayer.ExecuteDataTablePro("SP_GetBaseBudget", mParamsList, connection);

                }
                dt = _api.Format(dt);

                return Ok(_api.Success(dt));

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }

        }

        [HttpGet("ledgerActualBase")]
        public ActionResult GetActualBaseAmount(int nLedgerID, int nYear, string xMonth,int nBudgetTypeID)
        {
            DataTable dt = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SortedList mParamsList = new SortedList();
                    mParamsList.Add("N_CompanyID", nCompanyID);
                    mParamsList.Add("N_LedgerID", nLedgerID);
                    mParamsList.Add("N_Year", nYear);

                    if(nBudgetTypeID==2)
                        mParamsList.Add("X_Month", xMonth);
                    else if(nBudgetTypeID==1)
                        mParamsList.Add("X_Month", "");

                    dt = dLayer.ExecuteDataTablePro("SP_GetActualBaseAmount_ByLedger", mParamsList, connection);

                }
                dt = _api.Format(dt);

                return Ok(_api.Success(dt));

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

    }
}