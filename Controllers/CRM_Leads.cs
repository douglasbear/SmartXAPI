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
        public ActionResult LeadList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string UserPattern = myFunctions.GetUserPattern(User);
            string Pattern = "";
            if (UserPattern != "")
            {
                Pattern = " and Left(X_Pattern,Len(@p2))=@p2";
                Params.Add("@p2", UserPattern);
            }
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (x_lead like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_LeadID desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMLeads where N_CompanyID=@p1 " + Pattern + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMLeads where N_CompanyID=@p1 " + Pattern + Searchkey + " and N_LeadID not in (select top(" + Count + ") N_LeadID from vw_CRMLeads where N_CompanyID=@p1 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_CRMLeads where N_CompanyID=@p1 " + Pattern;
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
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("details")]
        public ActionResult LeadListDetails(string xLeadNo)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_CRMLeads where N_CompanyID=@p1 and X_LeadCode=@p3";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p3", xLeadNo);
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
                return Ok(api.Error(User,e));
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
                        if (LeadCode == "") { transaction.Rollback(); return Ok(api.Error(User,"Unable to generate Lead Code")); }
                        MasterTable.Rows[0]["X_LeadCode"] = LeadCode;
                    }


                    nLeadID = dLayer.SaveData("CRM_Leads", "N_LeadID", MasterTable, connection, transaction);
                    if (nLeadID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
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
                return Ok(api.Error(User,ex));
            }
        }



        [HttpPost("convertLead")]
        public ActionResult ConvertLead([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.GetCompanyID(User);
                bool CreateCustomer = myFunctions.getBoolVAL(MasterTable.Rows[0]["b_NewCustomer"].ToString());
                bool CreateContact = myFunctions.getBoolVAL(MasterTable.Rows[0]["b_NewContact"].ToString());
                bool CreateProject = myFunctions.getBoolVAL(MasterTable.Rows[0]["b_NewProject"].ToString());
                int n_LeadID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_LeadID"].ToString());
                int n_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nCustomerID = 0, nContactId = 0, nProjectID = 0;
                string CustomerSql = "", ContactSql = "", ProjectSql = "";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    DataTable CustomerTbl, ContactTbl, ProjectTbl, OprTbl;
                    Params.Add("@nLeadID", n_LeadID);
                    if (CreateCustomer == true)
                    {
                        CustomerSql = "select 0 as 'N_CustomerID',0 as 'X_CustomerCode',X_Company as X_Customer,X_Phone2 as X_Phone,X_Fax,X_Website,X_State,X_Street,X_City,N_CountryID,N_CompanyId,N_FnYearId,N_EmployeesCount as X_Employee,N_AnnRevenue From CRM_Leads where N_LeadID=@nLeadID";
                        CustomerTbl = dLayer.ExecuteDataTable(CustomerSql, Params, connection, transaction);
                        if (CustomerTbl.Rows.Count == 0) { transaction.Rollback(); return Ok(api.Error(User,"Unable to Create Customer ")); }
                        if (CustomerTbl.Rows[0]["X_Customer"].ToString() != "")
                        {

                            SortedList CustParams = new SortedList();
                            CustParams.Add("N_CompanyID", nCompanyID);
                            CustParams.Add("N_YearID", n_FnYearID);
                            CustParams.Add("N_FormID", 1306);
                            string CustCode = dLayer.GetAutoNumber("CRM_Customer", "X_CustomerCode", CustParams, connection, transaction);
                            if (CustCode == "") { transaction.Rollback(); return Ok(api.Error(User,"Unable to Create Customer ")); }

                            CustomerTbl.Rows[0]["X_CustomerCode"] = CustCode;
                            nCustomerID = dLayer.SaveData("CRM_Customer", "n_CustomerID", CustomerTbl, connection, transaction);
                            if (nCustomerID <= 0) { transaction.Rollback(); return Ok(api.Error(User,"Unable to Create Customer ")); }
                        }
                        
                        

                    }
                    else
                    {
                        nCustomerID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CustomerID"].ToString());
                    }
                    if (CreateContact == true)
                    {
                        ContactSql = "select N_CompanyID,N_FnYearId,0 as 'N_ContactID',0 as 'X_ContactCode',X_ContactName as 'X_Contact',X_Title,X_Email,X_Phone1 as 'X_Phone' From CRM_Leads where N_LeadID=@nLeadID";
                        ContactTbl = dLayer.ExecuteDataTable(ContactSql, Params, connection, transaction);
                        if (ContactTbl.Rows.Count == 0) { transaction.Rollback(); return Ok(api.Error(User,"Unable to Create Contact")); }

                        SortedList ContactParams = new SortedList();
                        ContactParams.Add("N_CompanyID", nCompanyID);
                        ContactParams.Add("N_YearID", n_FnYearID);
                        ContactParams.Add("N_FormID", 1308);
                        string ContactCode = dLayer.GetAutoNumber("CRM_Contact", "X_ContactCode", ContactParams, connection, transaction);
                        if (ContactCode == "") { transaction.Rollback(); return Ok(api.Error(User,"Unable to Create Contact")); }

                        ContactTbl.Rows[0]["X_ContactCode"] = ContactCode;
                        nContactId = dLayer.SaveData("CRM_Contact", "n_ContactID", ContactTbl, connection, transaction);
                        if (nContactId <= 0) { transaction.Rollback(); return Ok(api.Error(User,"Unable to Create Contact")); }
                    }
                    else
                    {
                        nContactId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ContactID"].ToString());
                    }
                    if (CreateProject == true)
                    {
                        ProjectSql = "select N_CompanyID,N_FnYearId,0 as 'N_ProjectID',0 as 'X_ProjectCode',X_ProjectName,X_ProjectLocation as 'X_Location',X_ProjectDescription as 'X_Description' From CRM_Leads where N_LeadID=@nLeadID";
                        ProjectTbl = dLayer.ExecuteDataTable(ProjectSql, Params, connection, transaction);
                        if (ProjectTbl.Rows.Count == 0) { transaction.Rollback(); return Ok(api.Error(User,"Unable to Create Project")); }
                        if (ProjectTbl.Rows[0]["X_ProjectName"].ToString() != "")
                        {
                            SortedList ProjectParams = new SortedList();
                            ProjectParams.Add("N_CompanyID", nCompanyID);
                            ProjectParams.Add("N_YearID", n_FnYearID);
                            ProjectParams.Add("N_FormID", 1303);
                            string ProjectCode = dLayer.GetAutoNumber("CRM_Project", "X_ProjectCode", ProjectParams, connection, transaction);
                            if (ProjectCode == "") { transaction.Rollback(); return Ok(api.Error(User,"Unable to Create Project")); }

                            ProjectTbl.Rows[0]["X_ProjectCode"] = ProjectCode;
                            nProjectID = dLayer.SaveData("CRM_Project", "n_ProjectID", ProjectTbl, connection, transaction);
                            if (nProjectID <= 0) { transaction.Rollback(); return Ok(api.Error(User,"Unable to Create Project")); }
                        }
                        
                        
                    }
                    else
                    {
                        nProjectID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ProjectID"].ToString());
                    }

                    // Auto Gen
                    string OprSql = "select N_CompanyID,N_FnYearId,0 as 'N_OpportunityID',0 as 'X_OpportunityCode',X_Lead as 'X_Opportunity',N_Probability,X_Email,X_Phone1 as 'X_Mobile',N_SalesmanID,N_AnnRevenue as 'N_ExpRevenue',N_LeadSource as 'N_LeadSourceID',X_Referredby as 'X_RefferedBy',X_ProjectDescription as 'X_Description',0 as 'N_CustomerID',0 as 'N_ContactID',0 as 'N_ProjectID' From CRM_Leads where N_LeadID=@nLeadID";
                    OprTbl = dLayer.ExecuteDataTable(OprSql, Params, connection, transaction);
                    if (OprTbl.Rows.Count == 0) { transaction.Rollback(); return Ok(api.Error(User,"Unable to Create Opportunity")); }
                    string OpportunityCode = "";
                    SortedList OprParams = new SortedList();
                    OprParams.Add("N_CompanyID", nCompanyID);
                    OprParams.Add("N_YearID", n_FnYearID);
                    OprParams.Add("N_FormID", 1302);
                    OpportunityCode = dLayer.GetAutoNumber("CRM_Opportunity", "x_OpportunityCode", OprParams, connection, transaction);
                    if (OpportunityCode == "") { transaction.Rollback(); return Ok(api.Error(User,"Unable to Create Opportunity")); }
                    OprTbl.Rows[0]["X_OpportunityCode"] = OpportunityCode;
                    OprTbl.Rows[0]["N_CustomerID"] = nCustomerID;
                    OprTbl.Rows[0]["N_ContactID"] = nContactId;
                    OprTbl.Rows[0]["N_ProjectID"] = nProjectID;
                    OprTbl.AcceptChanges();


                    int nOpportunityID = dLayer.SaveData("CRM_Opportunity", "n_OpportunityID", OprTbl, connection, transaction);
                    if (nOpportunityID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to Create Opportunity"));
                    }
                    else
                    {
                        transaction.Commit();
                        SortedList output = new SortedList() { { "x_OpportunityCode", OpportunityCode }, { "n_OpportunityID", nOpportunityID } };
                        return Ok(api.Success(output, "Opportunity Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
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
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("N_LeadID", nLeadID.ToString());
                    return Ok(api.Success(res, "Lead deleted"));
                }
                else
                {
                    return Ok(api.Error(User,"Unable to delete Lead"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }



        }
    }
}