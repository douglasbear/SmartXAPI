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
    [Route("consolidatedAccountMapping")]
    [ApiController]
    public class Inv_ConsolidatedAccountMapping : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;

        public Inv_ConsolidatedAccountMapping(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1833;
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
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    int nLedgerID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ConsolidatedLedgerID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nConsolidatedID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ConsolidatedID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_SubCompanyFnYearID"].ToString());
                    int nConsolidatedCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ConsolidatedCompanyID"].ToString());
                    string xConsolidatedCode = MasterTable.Rows[0]["x_ConsolidatedCode"].ToString();      
                    // if (xConsolidatedCode == "@Auto")
                    // {
                    //     Params.Add("N_CompanyID", nCompanyID);
                    //     Params.Add("N_FormID", this.FormID);
                    //     Params.Add("N_YearID", nFnYearID);
                        
                    //     xConsolidatedCode = dLayer.GetAutoNumber("Inv_ConsolidatedAccountMapping", "x_ConsolidatedCode", Params, connection, transaction);
                    //     if (xConsolidatedCode == "")
                    //     {
                    //         transaction.Rollback();
                    //         return Ok(_api.Error(User, "Unable to generate ConsolidatedAccountMapping"));
                    //     }
                    //     MasterTable.Rows[0]["x_ConsolidatedCode"] = xConsolidatedCode;
                    // }
                    MasterTable.Columns.Remove("n_FnYearID");
                    MasterTable.Columns.Remove("x_CompanyName");
                    MasterTable.Columns.Remove("x_LedgerName");
                    dLayer.ExecuteDataTable("DELETE FROM Inv_ConsolidatedAccountMapping WHERE N_ConsolidatedLedgerID = "+nLedgerID+" AND N_ConsolidatedCompanyID = "+nConsolidatedCompanyID+"",Params, connection, transaction);
                    nConsolidatedID = dLayer.SaveData("Inv_ConsolidatedAccountMapping", "n_ConsolidatedID", MasterTable, connection, transaction);

                    if (nConsolidatedID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("Unable to save"));
                    }
                    else
                    {

                        transaction.Commit();
                        return Ok(_api.Success("Save Successfully"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("list")]
        
            public ActionResult Listdata()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nClientID", myFunctions.GetClientID(User));
            string sqlCommandText = "SELECT X_CompanyName , X_CompanyCode, n_CompanyID FROM Acc_Company WHERE ISNULL( B_IsConsolidated,0)<>1 AND N_ClientID = @nClientID ";
             try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
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

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nConsolidatedID)
        {
            int Results = 0;
            SortedList Params = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    Params.Add("@n_ConsolidateLedgerID", nConsolidatedID);
                    Results = dLayer.DeleteData("Inv_ConsolidatedAccountMapping", "n_ConsolidatedID", nConsolidatedID, "", connection, transaction);
                    
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("ConsolidatedAccountMapping Deleted"));
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
        public ActionResult GetData(int nLedgedID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from vw_Consolidated_AccountMapping where N_ConsolidatedCompanyID=@p1 and N_ConsolidatedLedgerID=@p2";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nLedgedID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt =_api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
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

         [HttpGet("viewdetails")]
        public ActionResult GetledgerData(int nLedgedID , int nCompanyID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "SELECT * FROM vw_Consolidated_AccountMapping WHERE N_ConsolidatedCompanyID="+nCompanyID+" and N_ConsolidatedLedgerID="+nLedgedID+"";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nLedgedID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt =_api.Format(dt);
                // if (dt.Rows.Count == 0)
                // {
                //     return Ok(_api.Warning("No Results Found"));
                // }
                // else
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