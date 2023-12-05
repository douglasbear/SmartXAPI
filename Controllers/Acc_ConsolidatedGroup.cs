using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using ZXing;
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
                    int nConsolidatedGroupID = 0;
                    int nCompanyID = myFunctions.getIntVAL(DetailTable.Rows[0]["n_CompanyID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(DetailTable.Rows[0]["n_FnYearID"].ToString());
                    string xGroupCode = DetailTable.Rows[0]["x_ConsolidatedGroupCode"].ToString();
                    string xGroupName = DetailTable.Rows[0]["x_GroupName"].ToString();
                    string sql = "select * from Acc_ConsolidatedGroup where x_GroupName='"+xGroupName+"'and N_CompanyID="+nCompanyID;
                    DataTable count = new DataTable();
                    Params.Add("N_CompanyID", nCompanyID);
                    Params.Add("N_YearID", nFnYearID);
                    Params.Add("N_FormID", this.FormID);
                    if(xGroupCode ==  "@Auto")
                    { xGroupCode = dLayer.GetAutoNumber("Acc_ConsolidatedGroup", "x_ConsolidatedGroupCode", Params, connection, transaction); }
                    if (xGroupCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Group Code")); }
                    int code = myFunctions.getIntVAL(xGroupCode.ToString());
                    int nSubCompanyID = myFunctions.getIntVAL(DetailTable.Rows[0]["n_SubCompanyID"].ToString());
                    dLayer.DeleteData("Acc_ConsolidatedGroup", "x_ConsolidatedGroupCode", code, "", connection, transaction);
                    for (int i = 0; i < DetailTable.Rows.Count; i++)
                    {
                        DetailTable.Rows[i]["x_ConsolidatedGroupCode"] = xGroupCode;
                    }
                    DetailTable.Columns.Remove("n_FnYearID");
                    count = dLayer.ExecuteDataTable(sql, Params, connection, transaction);
                    if(count.Rows.Count > 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Group name already exists"));
                    }
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
        public ActionResult DeleteData(string xGroupCode)
        {
            SortedList Params = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    Params.Add("@p1", xGroupCode);
                    dLayer.ExecuteNonQuery("delete from Acc_ConsolidatedGroup where X_ConsolidatedGroupCode=@p1", Params, connection, transaction);
                    transaction.Commit();
                    return Ok(_api.Success("Group Successfully Deleted"));

                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("details")]
        public ActionResult GetData(string xGroupCode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from vw_Acc_ConsolidatedGroup where N_CompanyID=@p1 and X_ConsolidatedGroupCode=@p2 ";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", xGroupCode);

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