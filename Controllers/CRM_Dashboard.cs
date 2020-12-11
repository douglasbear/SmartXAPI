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
    [Route("crmDashboard")]
    [ApiController]
    public class CrmDashboard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public CrmDashboard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("listDetails")]
        public ActionResult GetDashboardDetails()
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);

            string sqlCurrentLead = "SELECT COUNT(*) as N_Count FROM crm_leads WHERE MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP)";
            string sqlCurrentCustomer = "SELECT COUNT(*) as N_Count FROM CRM_Customer WHERE MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP)";
            string sqlPerformance = "SELECT 'Leads Created',COUNT(*) as N_Count FROM crm_leads WHERE D_Entrydate >= DATEADD(DAY, -90, GETDATE())union SELECT 'Opportunities Created',COUNT(*) as N_Count FROM CRM_Opportunity WHERE D_Entrydate >= DATEADD(DAY, -90, GETDATE()) union SELECT 'Customer Created',COUNT(*) as N_Count FROM CRM_Customer WHERE D_Entrydate >= DATEADD(DAY, -90, GETDATE()) union SELECT 'Contacts Created',COUNT(*) as N_Count FROM CRM_Contact WHERE D_Entrydate >= DATEADD(DAY, -90, GETDATE()) union SELECT 'Projects Created',COUNT(*) as N_Count FROM CRM_Project WHERE D_Entrydate >= DATEADD(DAY, -90, GETDATE())";
            string sqlOpportunitiesStage = "select X_Stage,COUNT(*) as N_Count as N_Percentage  from vw_CRMOpportunity group by X_Stage";
            string sqlLeadsbySource = "select X_LeadSource,COUNT(*) as N_Count as N_Percentage from vw_CRMLeads group by X_LeadSource";
           
            DataTable CurrentLead = new DataTable();
            DataTable CurrentCustomer = new DataTable();
            DataTable Performance = new DataTable();
            DataTable OpportunitiesStage = new DataTable();
            DataTable LeadsbySource = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    CurrentLead = dLayer.ExecuteDataTable(sqlCurrentLead, Params, connection);
                    CurrentLead = api.Format(CurrentLead, "CurrentLead");
                    CurrentCustomer = dLayer.ExecuteDataTable(sqlCurrentCustomer, Params, connection);
                    CurrentCustomer = api.Format(CurrentCustomer, "CurrentCustomer");
                    Performance = dLayer.ExecuteDataTable(sqlPerformance, Params, connection);
                    Performance = api.Format(Performance, "Performance");
                    OpportunitiesStage = dLayer.ExecuteDataTable(sqlOpportunitiesStage, Params, connection);
                    OpportunitiesStage = api.Format(OpportunitiesStage, "OpportunitiesStage");
                    LeadsbySource = dLayer.ExecuteDataTable(sqlLeadsbySource, Params, connection);
                    LeadsbySource = api.Format(LeadsbySource, "LeadsbySource");
                    
                }
                dt.Tables.Add(CurrentLead);
                dt.Tables.Add(CurrentCustomer);
                dt.Tables.Add(Performance);
                dt.Tables.Add(OpportunitiesStage);
                dt.Tables.Add(LeadsbySource);

                return Ok(api.Success(dt));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
    }
}