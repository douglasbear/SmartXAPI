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
        [HttpGet("details")]
        public ActionResult GetDashboardDetails()
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);

            string sqlCurrentLead = "SELECT COUNT(*) as N_Count FROM crm_leads WHERE MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP)";
            string sqlCurrentCustomer = "SELECT COUNT(*) as N_Count FROM CRM_Customer WHERE MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP)";
            string sqlPerformance = "SELECT 'Leads Created' as X_Status,COUNT(*) as N_Count FROM crm_leads WHERE D_Entrydate >= DATEADD(DAY, -90, GETDATE())union SELECT 'Opportunities Created' as X_Status,COUNT(*) as N_Count FROM CRM_Opportunity WHERE D_Entrydate >= DATEADD(DAY, -90, GETDATE()) union SELECT 'Customer Created' as X_Status,COUNT(*) as N_Count FROM CRM_Customer WHERE D_Entrydate >= DATEADD(DAY, -90, GETDATE()) union SELECT 'Contacts Created' as X_Status,COUNT(*) as N_Count FROM CRM_Contact WHERE D_Entrydate >= DATEADD(DAY, -90, GETDATE()) union SELECT 'Projects Created' as X_Status,COUNT(*) as N_Count FROM CRM_Project WHERE D_Entrydate >= DATEADD(DAY, -90, GETDATE())";
            string sqlOpportunitiesStage = "select X_Stage,CAST(COUNT(*) as varchar(50)) as N_Percentage  from vw_CRMOpportunity group by X_Stage";
            string sqlLeadsbySource = "select X_LeadSource,CAST(COUNT(*) as varchar(50)) as N_Percentage from vw_CRMLeads group by X_LeadSource";
            string sqlPipelineoppotunity = "select count(*) as N_Count from CRM_Opportunity where N_ClosingStatusID=0 or N_ClosingStatusID is null";
            SortedList Data=new SortedList();
            DataTable CurrentLead = new DataTable();
            DataTable CurrentCustomer = new DataTable();
            DataTable Performance = new DataTable();
            DataTable OpportunitiesStage = new DataTable();
            DataTable LeadsbySource = new DataTable();
            DataTable PipelineOppotunity = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    CurrentLead = dLayer.ExecuteDataTable(sqlCurrentLead, Params, connection);
                    CurrentCustomer = dLayer.ExecuteDataTable(sqlCurrentCustomer, Params, connection);
                    Performance = dLayer.ExecuteDataTable(sqlPerformance, Params, connection);
                    OpportunitiesStage = dLayer.ExecuteDataTable(sqlOpportunitiesStage, Params, connection);
                    LeadsbySource = dLayer.ExecuteDataTable(sqlLeadsbySource, Params, connection);
                    PipelineOppotunity = dLayer.ExecuteDataTable(sqlPipelineoppotunity, Params, connection);
                    
                }
                // double N_TotalOppotunity=0;
                // foreach (DataRow dtRow in OpportunitiesStage.Rows)
                // {N_TotalOppotunity=N_TotalOppotunity + myFunctions.getVAL(dtRow["N_Percentage"].ToString());}
                // foreach (DataRow dtRow in OpportunitiesStage.Rows)
                // {
                //     dtRow["N_Percentage"]=((myFunctions.getVAL(dtRow["N_Percentage"].ToString())/N_TotalOppotunity)*100).ToString();
                // }

                // double N_TotalLead=0;
                // foreach (DataRow dtRow in LeadsbySource.Rows)
                // {N_TotalLead=N_TotalLead + myFunctions.getVAL(dtRow["N_Percentage"].ToString());}
                // foreach (DataRow dtRow in OpportunitiesStage.Rows)
                // {
                //     dtRow["N_Percentage"]=((myFunctions.getVAL(dtRow["N_Percentage"].ToString())/N_TotalLead)*100).ToString();
                // }




                if(CurrentLead.Rows.Count>0)Data.Add("CurrentLead",CurrentLead);
                if(CurrentCustomer.Rows.Count>0)Data.Add("CurrentCustomer",CurrentCustomer);
                if(Performance.Rows.Count>0)Data.Add("Performance",Performance);
                if(OpportunitiesStage.Rows.Count>0)Data.Add("OpportunitiesStage",OpportunitiesStage);
                if(LeadsbySource.Rows.Count>0)Data.Add("LeadsbySource",LeadsbySource);
                if(PipelineOppotunity.Rows.Count>0)Data.Add("PipelineOppotunity",PipelineOppotunity);

                return Ok(api.Success(Data));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
    }
}