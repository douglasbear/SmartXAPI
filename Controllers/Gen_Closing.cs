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
    [Route("genClosing")]
    [ApiController]
    public class Gen_Closing : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Gen_Closing(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1318;
        }


        
        [HttpGet("reasonList")]
        public ActionResult ReasonList(int nFormID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int nCompanyId=myFunctions.GetCompanyID(User);
  
            string sqlCommandText = "select * from vw_Inv_QuotationclosingStatus where N_CompanyID=@nCompanyID and N_FormID=@nFormID";
            Params.Add("@nCompanyID", nCompanyId);
            Params.Add("@nFormID", nFormID);

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
                return Ok(api.Error(e));
            }
        }


        [HttpPost("saveReason")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {

try{
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nPkeyId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PkeyId"].ToString());
                int N_FormID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FormID"].ToString());
                int N_closingStatus = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ClosingStatusID"].ToString());
                string X_ClosingDescription = MasterTable.Rows[0]["x_ClosingDescription"].ToString();
                SortedList Params=new SortedList();
                Params.Add("@Desc",X_ClosingDescription);
                Params.Add("@ClosingID",N_closingStatus);
                Params.Add("@nCompanyID",nCompanyID);
                Params.Add("@nPkey",nPkeyId);
                string sql="";
                switch(N_FormID){
                case 1302: 
                        sql="Update CRM_Opportunity set X_ClosingDescription=@Desc, N_ClosingStatusID=@ClosingID where N_OpportunityID=@nPkey and n_CompanyId=@nCompanyID";
                break;
                default: return Ok(api.Warning("Invalid Form"));
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    object saved = dLayer.ExecuteScalar(sql,Params, connection,transaction);
                    if (saved != null && myFunctions.getIntVAL(saved.ToString()) <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Successfully saved"));
                    }
                }
        }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }


        [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
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
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nStatusID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_StatusID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                string xStatusCode = MasterTable.Rows[0]["x_StatusCode"].ToString();
                MasterTable.Columns.Remove("n_FnYearID");
                MasterTable.AcceptChanges();
                 if (xStatusCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        xStatusCode = dLayer.GetAutoNumber("Inv_QuotationclosingStatus", "x_StatusCode", Params, connection, transaction);
                        if (xStatusCode == "") { return Ok(api.Error("Unable to generate Status Code")); }
                        MasterTable.Rows[0]["x_StatusCode"] = xStatusCode;
                    }
                    nStatusID=dLayer.SaveData("Inv_QuotationclosingStatus","n_StatusID",MasterTable,connection,transaction);  
                    transaction.Commit();
                    return Ok(api.Success("Reason Saved")) ;
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }

       


    }
}