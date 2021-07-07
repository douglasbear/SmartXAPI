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
    [Route("LeadsDashboard")]
    [ApiController]
    public class CRM_LeadsDashboard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public CRM_LeadsDashboard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("listDetails")]
        public ActionResult GetCustomerDetails(int xOpportunityCode)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);

            string sqlCommandActivitiesList = "select * from vw_CRM_Activity where N_CompanyID=@p1 and X_OpportunityCode=@p2";
            string sqlCommandLeadsList = "select * from vw_CRMOpportunity where N_CompanyID =@p1 and X_OpportunityCode=@p2";
            // string sqlCommandClient = "Select * from vw_CRMCustomer where N_CompanyID=@p1 and X_OpportunityCode=@p2";

            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", xOpportunityCode);

            DataTable ActivitiesList = new DataTable();
            DataTable LeadsList = new DataTable();
            DataTable Client = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    ActivitiesList = dLayer.ExecuteDataTable(sqlCommandActivitiesList, Params, connection);
                    LeadsList = dLayer.ExecuteDataTable(sqlCommandLeadsList, Params, connection);
                    // Client = dLayer.ExecuteDataTable(sqlCommandClient, Params, connection);

                    ActivitiesList = api.Format(ActivitiesList, "ActivitiesList");
                    LeadsList = api.Format(LeadsList, "LeadsList");
                    // Client = api.Format(ActivitiesList, "Client");


                    dt.Tables.Add(ActivitiesList);
                    dt.Tables.Add(LeadsList);
                    // dt.Tables.Add(Client);
                    return Ok(api.Success(dt));

                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
    }
}

