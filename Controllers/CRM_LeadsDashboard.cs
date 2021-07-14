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

            string sqlCommandActivitiesList = "select * from vw_CRM_Activity where N_CompanyID=@p1 and X_OpportunityCode=@p2 order by N_Order";
            string sqlCommandLeadsList = "select * from vw_CRMOpportunity where N_CompanyID =@p1 and X_OpportunityCode=@p2";
            string sqlCommandContactList = "Select * from vw_CRMContact where N_CompanyID=@p1 and X_OpportunityCode=@p2";
            string sqlCommandQuotationList = "Select * from inv_salesquotation where N_CompanyID=@p1 and n_opportunityID=@p3";
            string sqlCommandinvoiceList = "Select * from inv_sales where N_CompanyID=@p1 and n_quotationid=@p4";
            string sqlCommandMailLogList = "Select * from Gen_MailLog where N_CompanyID=@p1 and N_OpportunityID=@p3";

            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", xOpportunityCode);

            DataTable ActivitiesList = new DataTable();
            DataTable LeadsList = new DataTable();
            DataTable ContactList = new DataTable();
            DataTable QuotationList = new DataTable();
            DataTable InvoiceList = new DataTable();
            DataTable MailLogList = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object N_OpportunityID = dLayer.ExecuteScalar("select N_opportunityID from crm_opportunity where X_OpportunityCode=@p2", Params, connection);
                    Params.Add("@p3", N_OpportunityID);

                    object N_Quotationid = dLayer.ExecuteScalar("select n_quotationid from inv_salesquotation where N_OpportunityID=@p3", Params, connection);
                    if (N_Quotationid != null)
                    {
                        Params.Add("@p4", N_Quotationid);
                        InvoiceList = dLayer.ExecuteDataTable(sqlCommandinvoiceList, Params, connection);
                        InvoiceList = api.Format(InvoiceList, "InvoiceList");
                        dt.Tables.Add(InvoiceList);
                    }

                    ActivitiesList = dLayer.ExecuteDataTable(sqlCommandActivitiesList, Params, connection);
                    LeadsList = dLayer.ExecuteDataTable(sqlCommandLeadsList, Params, connection);
                    ContactList = dLayer.ExecuteDataTable(sqlCommandContactList, Params, connection);
                    QuotationList = dLayer.ExecuteDataTable(sqlCommandQuotationList, Params, connection);
                    MailLogList = dLayer.ExecuteDataTable(sqlCommandMailLogList, Params, connection);


                    ActivitiesList = api.Format(ActivitiesList, "ActivitiesList");
                    LeadsList = api.Format(LeadsList, "LeadsList");
                    ContactList = api.Format(ContactList, "ContactList");
                    QuotationList = api.Format(QuotationList, "QuotationList");
                    MailLogList = api.Format(MailLogList, "MailLogList");



                    dt.Tables.Add(ActivitiesList);
                    dt.Tables.Add(LeadsList);
                    dt.Tables.Add(ContactList);
                    dt.Tables.Add(QuotationList);
                    dt.Tables.Add(MailLogList);

                    return Ok(api.Success(dt));

                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
        [HttpGet("update")]
        public ActionResult ActivityUpdate(string xActivityCode, bool bFlag)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            if (bFlag)
                sqlCommandText = "update crm_activity set b_closed=1,x_status='Closed'  where N_CompanyID=@p1 and X_ActivityCode=@p2";
            else
                sqlCommandText = "update crm_activity set b_closed=0,x_status='Active'  where N_CompanyID=@p1 and X_ActivityCode=@p2";
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
        public ActionResult NotesUpdate(string xOpportunityCode, string xnotes)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            sqlCommandText = "update crm_opportunity set X_Notes=@p3 where N_CompanyID=@p1 and X_OpportunityCode=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", xOpportunityCode);
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

        [HttpGet("orderupdate")]
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


    }
}

