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
    [Route("genNotifications")]
    [ApiController]
    public class Gen_Notifications : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Gen_Notifications(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 0;
        }



        [HttpGet("count")]
        public ActionResult NotificationCount(bool bShowAllBranch, int nBranchID)
        {
            SortedList Params = new SortedList();

            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlCommandText = "";

            if (bShowAllBranch)
            {
                sqlCommandText = "select count(*) from vw_ApprovalPending where N_CompanyID=@nCompanyID and N_NextApproverID=@nUserID ";
            }
            else
            {
                sqlCommandText = "select count(*) from vw_ApprovalPending where N_CompanyID=@nCompanyID and N_NextApproverID=@nUserID and N_Branchid = @nBranchID ";
                Params.Add("@nBranchID", nBranchID);
            }

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nUserID", nUserID);

            try
            {
                int count = 0;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    count = myFunctions.getIntVAL(dLayer.ExecuteScalar(sqlCommandText, Params, connection).ToString());
                }
                SortedList res = new SortedList();
                res.Add("Count", count);
                return Ok(api.Success(res));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
    }
}