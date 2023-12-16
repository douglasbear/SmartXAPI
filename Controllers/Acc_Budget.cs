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
                        return Ok(_api.Error(User, "Unable to save"));
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
                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    MasterTable.AcceptChanges();

                    int nBudgetID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BudgetID"].ToString());
                    Params.Add("@n_BudgetID", nBudgetID);

                    string sqlCommandText2 = "select * from vw_Acc_BudgetDetails where N_CompanyID=@n_CompanyID and N_BudgetID=@n_BudgetID ";

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
        public ActionResult GetbaseyearList(int nCompanyID, int nyear,int nBudgetTypeID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();


            string sqlCommandText = "select CONVERT(NVARCHAR(MAX), N_Year) AS X_Year,N_Year from Acc_BudgetMaster WHERE N_CompanyID=@p1 and N_BudgetTypeID=@p2 and N_Year <= @p3 GROUP BY N_Year";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nBudgetTypeID);
             Params.Add("@p3", nyear);


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
                return Ok(_api.Error(User, e));
            }
        }
        [HttpGet("basemonthlist")]
        public ActionResult GetbasemonthList(int nCompanyID, int nBudgetTypeID, int nYear)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "select X_Month from Acc_BudgetMaster WHERE N_CompanyID=@p1 AND N_Year <=@p3 and N_BudgetTypeID=@p2 GROUP BY X_Month";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nBudgetTypeID);
            Params.Add("@p3", nYear);


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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("basebudgetlist")]
        public ActionResult GetbasebudgetList(int nCompanyID, int nYear,int nBudgetTypeID,string xMonth)
        {
            DataTable dt = new DataTable();
          
            SortedList Params = new SortedList();
           

            string sqlCommandText = "";
           

            Params.Add("@p1", nCompanyID);
           
            Params.Add("@p3", nYear);

            if (nBudgetTypeID == 1){
                 
                Params.Add("@nCompanyID", nCompanyID);
                Params.Add("@nYear", nYear);
                Params.Add("@nBudgetTypeID", nBudgetTypeID);

                sqlCommandText = "select N_BudgetID, X_BudgetCode, X_BudgetName from Acc_BudgetMaster WHERE N_CompanyID = @nCompanyID AND N_Year = @nYear and N_BudgetTypeID = @nBudgetTypeID";
            }
            else if (nBudgetTypeID == 2){

                 Params.Add("@nCompanyID", nCompanyID);
                Params.Add("@nYear", nYear);
                Params.Add("@nBudgetTypeID", nBudgetTypeID);
                Params.Add("@xMonth", xMonth);

                sqlCommandText="select N_BudgetID, X_BudgetCode, X_BudgetName from Acc_BudgetMaster WHERE N_CompanyID = @nCompanyID AND N_Year = @nYear and X_Month = @xMonth and N_BudgetTypeID = @nBudgetTypeID GROUP BY X_Month,N_BudgetID,X_BudgetCode, X_BudgetName";

            }

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
                return Ok(_api.Error(User, e));
            }
         }

    }
}