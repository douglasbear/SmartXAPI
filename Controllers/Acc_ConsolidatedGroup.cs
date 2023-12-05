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
    [Route("consolidatedGroup")]
    [ApiController]
    public class Acc_ConsolidatedGroup : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;

        public Acc_ConsolidatedGroup(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1841;
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
                    DataTable DetailTable;
                    DetailTable = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    int nConsolidatedGroupID = myFunctions.getIntVAL(DetailTable.Rows[0]["n_ConsolidatedGroupID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(DetailTable.Rows[0]["n_CompanyID"].ToString());
                    int nSubCompanyID = myFunctions.getIntVAL(DetailTable.Rows[0]["n_SubCompanyID"].ToString());
                    // int nFnYearID = myFunctions.getIntVAL(DetailTable.Rows[0]["n_FnYearID"].ToString());

                    // Params.Add("N_CompanyID", nCompanyID);
                    // Params.Add("N_YearID", nFnYearID);
                    // Params.Add("N_FormID", this.FormID);
                    
                    nConsolidatedGroupID = dLayer.SaveData("Acc_ConsolidatedGroup", "n_ConsolidatedGroupID", DetailTable, connection, transaction);

                    if (nConsolidatedGroupID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Saved Successfully"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nConsolidatedGroupID,string xGroupName)
        {
            int Results = 0;
            SortedList Params = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    Params.Add("@n_ConsolidatedGroupID", nConsolidatedGroupID);
                    Params.Add("@x_GroupName", xGroupName);

                    Results = dLayer.DeleteData("Acc_ConsolidatedGroup", "n_ConsolidatedGroupID", nConsolidatedGroupID, "x_GroupName="+xGroupName, connection, transaction);
                    
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("OtherCost Deleted"));
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
        public ActionResult GetData(int nConsolidatedGroupID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from vw_Acc_ConsolidatedGroup where N_CompanyID=@p1 and N_ConsolidatedGroupID=@p2 ";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nConsolidatedGroupID);

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

    }
}