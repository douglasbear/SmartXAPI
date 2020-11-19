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
    [Route("leads")]
    [ApiController]
    public class CRM_Leads : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1305;

        public CRM_Leads(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("list")]
        public ActionResult LeadList(int nFnYearId,int nPage,int nSizeperpage)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
             
             if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_CRMLeads where N_CompanyID=@p1 order by N_LeadID desc";
            else
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_CRMLeads where N_CompanyID=@p1 and N_LeadID not in (select top("+ Count +") N_LeadID from vw_CRMLeads where N_CompanyID=@p1 order by N_LeadID desc) order by N_LeadID desc";
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_CRMLeads where N_CompanyID=@p1 ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }

                }
                
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpGet("details")]
        public ActionResult LeadListDetails(string xLeadNo)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_CRMLeads where N_CompanyID=@p1 and X_LeadCode=@p3";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p3", xLeadNo);
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
                int nLeadID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LeadID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string LeadCode = "";
                    var values = MasterTable.Rows[0]["X_LeadCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        LeadCode = dLayer.GetAutoNumber("CRM_Leads", "X_LeadCode", Params, connection, transaction);
                        if (LeadCode == "") { return Ok(api.Error("Unable to generate Lead Code")); }
                        MasterTable.Rows[0]["X_LeadCode"] = LeadCode;
                    }


                    nLeadID = dLayer.SaveData("CRM_Leads", "N_LeadID", MasterTable, connection, transaction);
                    if (nLeadID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Lead Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }

      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nLeadID)
        {

             int Results = 0;
            try
            {                        
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("CRM_Leads", "N_LeadID", nLeadID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_LeadID",nLeadID.ToString());
                    return Ok(api.Success(res,"Lead deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete Lead"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }



        }
    }
}