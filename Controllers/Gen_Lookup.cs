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
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("genlookup")]
    [ApiController]
    public class Gen_Lookup : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Gen_Lookup(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        
        [HttpGet("listDetails")]
        public ActionResult OpportunityListDetails(int nFnYearId,int nPkeyId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string criteria = "";
            int nCompanyId=myFunctions.GetCompanyID(User);
  
            string sqlCommandText = "select * from Gen_LookupTable where N_CompanyID=@p1 and N_FnyearID=@p2 and N_PkeyId=@p3";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", nPkeyId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }
        }



        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nPkeyId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PkeyId"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string PkeyCode = "";
                    var values = MasterTable.Rows[0]["X_PkeyCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", 1305);
                        PkeyCode = dLayer.GetAutoNumber("Gen_LookupTable", "X_PkeyCode", Params, connection, transaction);
                        if (PkeyCode == "") { return Ok(api.Error("Unable to generate PkeyCode Code")); }
                        MasterTable.Rows[0]["X_PkeyCode"] = PkeyCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Gen_LookupTable", "N_PkeyId", nPkeyId, "", connection, transaction);
                    }
                    MasterTable.Columns.Remove("N_PkeyId");
                    MasterTable.AcceptChanges();

                    nPkeyId = dLayer.SaveData("Gen_LookupTable", "N_PkeyId", nPkeyId, MasterTable, connection, transaction);
                    if (nPkeyId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return OpportunityListDetails(nFnYearId, nPkeyId);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }

      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPkeyId,int nCompanyID, int nFnYearID)
        {

             int Results = 0;
            try
            {                        
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();                
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nFormID", 1305);
                QueryParams.Add("@nPkeyId", nPkeyId);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (myFunctions.getBoolVAL(myFunctions.checkProcessed("Acc_FnYear", "B_YearEndProcess", "N_FnYearID", "@nFnYearID", "N_CompanyID=@nCompanyID ", QueryParams, dLayer, connection)))
                        return Ok(api.Error("Year is closed, Cannot create new Entry..."));
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Gen_LookupTable", "N_PkeyId", nPkeyId, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_PkeyId",nPkeyId.ToString());
                    return Ok(api.Success(res,"Entry deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete Entry"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error("Unable to delete entry"));
            }



        }
    }
}