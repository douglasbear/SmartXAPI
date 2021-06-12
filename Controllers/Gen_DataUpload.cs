using AutoMapper;
using SmartxAPI.Data;
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
    [Route("dataupload")]
    [ApiController]
    public class Gen_DataUpload : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        public Gen_DataUpload(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["Crm_Leads"];

                int nCompanyID = 1;
                int nFnYearId = 1;
                int nLeadID = 0;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string LeadCode = "";
                    var values = "@Auto";
                    Params.Add("N_CompanyID", nCompanyID);
                    Params.Add("N_YearID", nFnYearId);
                    Params.Add("N_FormID", 1305);
                    // foreach (DataRow dRow in MasterTable.Rows)
                    // {
                    // if (values == "@Auto")
                    // {

                    //     LeadCode = dLayer.GetAutoNumber("CRM_Leads", "X_LeadCode", Params, connection, transaction);
                    //     if (LeadCode == "") { transaction.Rollback(); return Ok(); }
                    //     MasterTable.Rows[0]["X_LeadCode"] = LeadCode;
                    // }
                    nLeadID = dLayer.SaveData("CRM_Leads", "N_LeadID", MasterTable, connection, transaction);
                    // }
                    if (nLeadID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Lead Created"));
                    }

                    // connection.Open();
                    // SqlTransaction transaction = connection.BeginTransaction();

                    // SortedList Params = new SortedList();
                    // // Auto Gen
                    // string LocationCode = "";
                    // var values = MasterTable.Rows[0]["X_LocationCode"].ToString();
                    // if (values == "@Auto")
                    // {
                    //     Params.Add("N_CompanyID", MasterTable.Rows[0]["n_CompanyId"].ToString());
                    //     Params.Add("N_YearID", MasterTable.Rows[0]["n_FnYearId"].ToString());
                    //     Params.Add("N_FormID", 450);
                    //     Params.Add("N_BranchID", MasterTable.Rows[0]["n_BranchId"].ToString());
                    //     LocationCode = dLayer.GetAutoNumber("Inv_Location", "X_LocationCode", Params, connection, transaction);
                    //     if (LocationCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Location Code")); }
                    //     MasterTable.Rows[0]["X_LocationCode"] = LocationCode;
                    // }

                    // MasterTable.Columns.Remove("n_FnYearId");
                    // MasterTable.Columns.Remove("b_isSubLocation");
                    // int N_LocationID = dLayer.SaveData("Inv_Location", "N_LocationID", MasterTable, connection, transaction);
                    // if (N_LocationID <= 0)
                    // {
                    //     transaction.Rollback();
                    //     return Ok(_api.Warning("Unable to save"));
                    // }
                    // else
                    // {
                    //     transaction.Commit();
                    //     return Ok();
                    // }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
    }
}