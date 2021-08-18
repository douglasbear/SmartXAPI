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
    [Route("TasksDashboard")]
    [ApiController]
    public class Prj_TasksDashboard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Prj_TasksDashboard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("listDetails")]
        public ActionResult GetTasksDetails(int xProjectCode)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);

            string sqlCommandTasksList = "select * from vw_Tsk_TaskMaster where N_CompanyID=@p1 and X_ProjectCode=@p2 and isnull(N_ParentID,0)=0 order by N_Order";
            string sqlCommandContactList = "Select * from Vw_InvCustomerProjects where N_CompanyID=@p1 and X_ProjectCode=@p2";
            // string sqlCommandLeadsList = "select CONVERT(varchar,d_EntryDate,101) as d_Entry,* from vw_CRMOpportunity where N_CompanyID =@p1 and X_OpportunityCode=@p2";          
            // string sqlCommandQuotationList = "Select * from inv_salesquotation where N_CompanyID=@p1 and n_opportunityID=@p3";
            // string sqlCommandOrderList = "Select * from inv_salesOrder where N_CompanyID=@p1 and n_opportunityID=@p3";
            // string sqlCommandinvoiceList = "Select * from inv_sales where N_CompanyID=@p1 and n_opportunityID=@p3";
            // string sqlCommandMailLogList = "Select CONVERT(VARCHAR(10), d_Date, 103) + ' '  + convert(VARCHAR(8), d_Date, 14) as d_Entry,* from Gen_MailLog where N_CompanyID=@p1 and N_OpportunityID=@p3 order by N_maillogid desc";
            // string sqlCommandProjectList = "Select CONVERT(varchar,d_StartDate,101) as d_Start,CONVERT(varchar,d_EndDate,101) as d_End,* from crm_Project where N_CompanyID=@p1 and N_ProjectID=@p5";

            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", xProjectCode);

            DataTable TasksList = new DataTable();
            DataTable ContactList = new DataTable();
            // DataTable LeadsList = new DataTable();

            // DataTable QuotationList = new DataTable();
            // DataTable OrderList = new DataTable();
            // DataTable InvoiceList = new DataTable();
            // DataTable MailLogList = new DataTable();
            // DataTable ProjectList = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // object N_OpportunityID = dLayer.ExecuteScalar("select N_opportunityID from crm_opportunity where X_OpportunityCode=@p2", Params, connection);
                    // object N_ProjectID = dLayer.ExecuteScalar("select N_ProjectID from crm_opportunity where X_OpportunityCode=@p2", Params, connection);
                    // Params.Add("@p3", N_OpportunityID);


                    // object N_Quotationid = dLayer.ExecuteScalar("select n_quotationid from inv_salesquotation where N_OpportunityID=@p3", Params, connection);
                    // if (N_OpportunityID != null)
                    // {
                    //     InvoiceList = dLayer.ExecuteDataTable(sqlCommandinvoiceList, Params, connection);
                    //     InvoiceList = api.Format(InvoiceList, "InvoiceList");
                    //     dt.Tables.Add(InvoiceList);
                    // }
                    // if (N_ProjectID != null)
                    // {
                    //     Params.Add("@p5", N_ProjectID);
                    //     ProjectList = dLayer.ExecuteDataTable(sqlCommandProjectList, Params, connection);
                    //     ProjectList = api.Format(ProjectList, "ProjectList");
                    //     dt.Tables.Add(ProjectList);
                    // }

                    TasksList = dLayer.ExecuteDataTable(sqlCommandTasksList, Params, connection);
                    ContactList = dLayer.ExecuteDataTable(sqlCommandContactList, Params, connection);
                    // LeadsList = dLayer.ExecuteDataTable(sqlCommandLeadsList, Params, connection);
                    // QuotationList = dLayer.ExecuteDataTable(sqlCommandQuotationList, Params, connection);
                    // OrderList = dLayer.ExecuteDataTable(sqlCommandOrderList, Params, connection);
                    // MailLogList = dLayer.ExecuteDataTable(sqlCommandMailLogList, Params, connection);


                    TasksList = api.Format(TasksList, "TasksList");
                    ContactList = api.Format(ContactList, "ContactList");
                    // LeadsList = api.Format(LeadsList, "LeadsList");
                    // QuotationList = api.Format(QuotationList, "QuotationList");
                    // MailLogList = api.Format(MailLogList, "MailLogList");
                    // OrderList = api.Format(OrderList, "OrderList");



                    dt.Tables.Add(TasksList);
                    dt.Tables.Add(ContactList);
                    // dt.Tables.Add(LeadsList);
                    // dt.Tables.Add(QuotationList);
                    // dt.Tables.Add(MailLogList);
                    // dt.Tables.Add(OrderList);

                    return Ok(api.Success(dt));

                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
        [HttpGet("update")]
        public ActionResult TasksUpdate(string xActivityCode, bool bFlag)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            if (bFlag)
                sqlCommandText = "update Tsk_TaskMaster set b_closed=1,x_status='Closed'  where N_CompanyID=@p1 and X_ActivityCode=@p2";
            else
                sqlCommandText = "update Tsk_TaskMaster set b_closed=0,x_status='Active'  where N_CompanyID=@p1 and X_ActivityCode=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", xActivityCode);


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dLayer.ExecuteNonQuery(sqlCommandText, Params, connection);
                }
                return Ok(api.Warning("Activity Updated"));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
        [HttpGet("notesupdate")]
        public ActionResult NotesUpdate(string xProjectCode, string xnotes)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            sqlCommandText = "update inv_customerprojects set X_Notes=@p3 where N_CompanyID=@p1 and X_ProjectCode=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", xProjectCode);
            Params.Add("@p3", xnotes);


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dLayer.ExecuteNonQuery(sqlCommandText, Params, connection);
                }
                return Ok(api.Warning("Notes Updated"));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpPost("orderupdate")]
        public ActionResult OrderUpdate([FromBody] DataSet ds)
        {

            DataTable MasterTable;
            MasterTable = ds.Tables["master"];
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            Params.Add("@p1", nCompanyId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int N_Order = 1;
                    foreach (DataRow var in MasterTable.Rows)
                    {
                        dLayer.ExecuteNonQuery("update crm_activity set N_Order=" + N_Order + " where N_CompanyID=@p1 and X_ActivityCode=" + var["x_ActivityCode"].ToString(), Params, connection);
                        N_Order++;
                    }
                }
                return Ok(api.Success("Order Updated"));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

          [HttpGet("emailDetails")]
        public ActionResult TaskDetails(string xTaskCode,int nTemplateID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyId", nCompanyID);
            Params.Add("@p2", xTaskCode);
            Params.Add("@p3", nTemplateID);
          
            string sqlCommandText = "select * from vw_Tsk_TaskMaster where N_CompanyID=@nCompanyId and X_TaskCode=@p2 and N_TemplateID=@p3";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection,transaction);
                    
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



    }
}

