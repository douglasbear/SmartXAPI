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
    [Route("actionusergroup")]
    [ApiController]
    public class Rec_ActionUserGroup : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1613;


        public Rec_ActionUserGroup(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetActionGroup()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "Select * from Gen_ActionGroup Where N_CompanyID=@nCompanyID order by X_GroupCode";
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




        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nGroupID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_GroupID"].ToString());


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string GroupCode = "";
                    var values = MasterTable.Rows[0]["X_GroupCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        GroupCode = dLayer.GetAutoNumber("Gen_ActionGroup", "X_GroupCode", Params, connection, transaction);
                        if (GroupCode == "") { return Ok(_api.Error(User, "Unable to generate Group Code")); }
                        MasterTable.Rows[0]["X_GroupCode"] = GroupCode;


                    }
                    if(nGroupID>0)
                    {
                    dLayer.DeleteData("Gen_ActionGroup", "n_GroupID", nGroupID, "", connection,transaction);
                   dLayer.DeleteData("Gen_ActionGroupDetails", "n_GroupID", nGroupID, "", connection,transaction);
                    }


                    nGroupID = dLayer.SaveData("Gen_ActionGroup", "N_GroupID", MasterTable, connection, transaction);


                    if (nGroupID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }
                    for (int i = 0; i < DetailTable.Rows.Count; i++)
                    {
                        DetailTable.Rows[i]["N_GroupID"] = nGroupID;
                    }
                    dLayer.SaveData("Gen_ActionGroupDetails", "N_GroupDetailsID", DetailTable, connection, transaction);


                    transaction.Commit();
                    return Ok(_api.Success("Group Saved"));

                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(User, ex));
            }
        }


        [HttpGet("details")]
        public ActionResult GetDetails(int nGroupID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_Actiongroup where N_CompanyID=@nCompanyID and N_GroupID=@nGroupID";
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nGroupID", nGroupID);

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

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nGroupID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Gen_ActionGroup", "n_GroupID", nGroupID, "", connection);
                    Results = dLayer.DeleteData("Gen_ActionGroupDetails", "n_GroupID", nGroupID, "", connection);
                    if (Results > 0)
                    {
                        return Ok(_api.Success("deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Unable to delete "));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }



    }
}