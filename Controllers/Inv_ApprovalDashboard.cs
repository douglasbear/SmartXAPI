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
    [Route("ApprovalDashboard")]
    [ApiController]
    public class Inv_ApprovalDashboard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Inv_ApprovalDashboard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("listDetails")]
        public ActionResult GetApprovalDetails(int nCompanyID, int nnextApproverID,bool bShowAll,bool bShowAllBranch,int N_Branchid,int nApprovalType)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText="";

            if(nApprovalType==1)
            {
                if(bShowAllBranch)
                {
                    if(bShowAll)
                         sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1";
                    else
                         sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1 and N_NextApproverID=@p2";
                }
                else
                {
                    if(bShowAll)
                         sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1 and N_Branchid = @p3";
                    else
                         sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1 and N_NextApproverID=@p2 and N_Branchid = @p3";

                    Params.Add("@p3", N_Branchid);     
                }
            
            }
            else
            {
                if(bShowAllBranch)
                    sqlCommandText = "select * from vw_ApprovalSummary where N_CompanyID=@p1 and N_ActionUserID=@p2 and N_ProcStatusID<>6";
                else
                {
                    sqlCommandText = "select * from vw_ApprovalSummary where N_CompanyID=@p1 and N_ActionUserID=@p2 and N_ProcStatusID<>6 and N_Branchid = @p3"; 
                    Params.Add("@p3", N_Branchid);
                }

            }
            
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nnextApproverID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
        

        
    }
}