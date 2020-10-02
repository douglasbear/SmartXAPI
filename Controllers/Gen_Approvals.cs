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
using System.Security.Claims;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("approval")]
    [ApiController]
    public class Gen_Approvals : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        public Gen_Approvals(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("dashboard")]
        public ActionResult GetApprovalDetails( bool bShowAll, bool bShowAllBranch, int N_Branchid, int nApprovalType)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "";
            string DateCol="D_ApprovedDate";
            if (nApprovalType == 1)
            {
                DateCol = "D_ApprovedDate";

                if (bShowAllBranch)
                {
                    if (bShowAll)
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1";
                    else
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1 and N_NextApproverID=@p2";
                }
                else
                {
                    if (bShowAll)
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1 and N_Branchid = @p3";
                    else
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1 and N_NextApproverID=@p2 and N_Branchid = @p3";

                    //Params.Add("@p3", N_Branchid);
                    Params.Add("@p3", N_Branchid);
                }
    
            }
            else
            {
                DateCol = "X_RequestDate";
                if (bShowAllBranch)
                    sqlCommandText = "select * from vw_ApprovalSummary where N_CompanyID=@p1 and N_ActionUserID=@p2 and N_ProcStatusID<>6";
                else
                {
                    sqlCommandText = "select * from vw_ApprovalSummary where N_CompanyID=@p1 and N_ActionUserID=@p2 and N_ProcStatusID<>6 and N_Branchid = @p3";
                    Params.Add("@p3", N_Branchid);
                }

            }
            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);

            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nUserID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText + " and n_FormID in (212,210,1226,1229,1232,1234,1235,1236,1239,2001,2002,2003,2004,2005) order by "+ DateCol +" desc", Params, connection);
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

        [HttpGet("GetApprovalSettings")]
        public ActionResult GetApprovalSettings(int nIsApprovalSystem, int nFormID, int nTransID, int nTransUserID, int nTransStatus, int nTransApprovalLevel, int nNextApprovalLevel, int nApprovalID, int nGroupID, int nFnYearID, int nEmpID, int nActionID)
        {
            



            try
            {
            using (SqlConnection connection = new SqlConnection(connectionString))
                {
                connection.Open();
                SortedList Response = myFunctions.GetApprovals(nIsApprovalSystem, nFormID, nTransID, nTransUserID, nTransStatus, nTransApprovalLevel, nNextApprovalLevel, nApprovalID, nGroupID, nFnYearID, nEmpID, nActionID,User,dLayer, connection);
                return Ok(api.Success(Response));
                }
            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }

        }

        // public SortedList SetStatus(SortedList Response, DataRow rw, string struserName, string strEntryTime)
        // {
        //     if (rw != null)
        //     {
        //         Response["lblVisible"] = true;
        //         Response["lblText"] = rw["X_MsgStatus"].ToString();
        //         Response["lblText"] = Response["lblText"].ToString().Replace("#NAME", struserName);
        //         if (strEntryTime.Trim() != "")
        //             strEntryTime = Convert.ToDateTime(strEntryTime).ToString("dd/MM/yyyy HH:mm:ss");
        //         Response["lblText"] = Response["lblText"].ToString().Replace("#DATE", strEntryTime);
        //     }
        //     return Response;
        // }

        



    }
}