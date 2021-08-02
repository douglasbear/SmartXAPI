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
        public ActionResult GetDashboardDetails(int nFnYearId)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);

            string sqlCurrentLead = "SELECT COUNT(*) as N_ThisMonth FROM crm_leads WHERE MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = "+nCompanyID+"";
            string sqlPreviousLead = "SELECT COUNT(*) as N_LastMonth FROM crm_leads WHERE DATEPART(m, D_EntryDate) = DATEPART(m, DATEADD(m, -1, getdate())) and N_CompanyID = "+nCompanyID+"";
            string sqlCurrentCustomer = "SELECT COUNT(*) as N_ThisMonth FROM CRM_Customer WHERE MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = "+nCompanyID+"";
            string sqlPreviousCustomer = "SELECT COUNT(*) as N_LastMonth FROM CRM_Customer WHERE DATEPART(m, D_EntryDate) = DATEPART(m, DATEADD(m, -1, getdate())) and N_CompanyID = "+nCompanyID+"";
            
            string sqlPerformance = " SELECT 'Leads Created' as X_Status,CONVERT(VARCHAR,COUNT(*)) as N_Count FROM crm_leads WHERE N_CompanyID = "+nCompanyID+" and D_Entrydate >= DATEADD(DAY, -90, GETDATE()) union SELECT 'Opportunities Created' as X_Status,CONVERT(VARCHAR,COUNT(*))as N_Count FROM CRM_Opportunity WHERE N_CompanyID = "+nCompanyID+" and D_Entrydate >= DATEADD(DAY, -90, GETDATE()) union SELECT 'Customer Created' as X_Status,CONVERT(VARCHAR,COUNT(*)) as N_Count FROM CRM_Customer WHERE  N_CompanyID = "+nCompanyID+" and D_Entrydate >= DATEADD(DAY, -90, GETDATE()) union SELECT  'Revenue Generated' as X_Status, CAST(CONVERT(VARCHAR, CAST(sum(N_TotAmt) AS MONEY), 1) AS VARCHAR) as N_Count FROM vw_InvSalesOrderNo_Search WHERE N_CompanyID = "+nCompanyID+" and D_OrderDate >= DATEADD(DAY, -90, GETDATE())--'Contacts Created' as X_Status,COUNT(*) as N_Count FROM CRM_Contact WHERE N_CompanyID = "+nCompanyID+" and D_Entrydate >= DATEADD(DAY, -90, GETDATE())  union SELECT 'Projects Created' as X_Status,COUNT(*) as N_Count FROM CRM_Project WHERE N_CompanyID = "+nCompanyID+" and D_Entrydate >= DATEADD(DAY, -90, GETDATE())";
           // string sqlOpportunitiesStage = "select X_Stage,CAST(COUNT(*) as varchar(50)) as N_Percentage  from vw_CRMOpportunity group by X_Stage";
            string sqlOpportunitiesStage = "select isNull(X_Stage,'Others') as X_Stage,CAST(COUNT(*) as varchar(50)) as N_Percentage  from vw_CRMOpportunity where N_CompanyID = "+nCompanyID+" group by X_Stage ";
            string sqlLeadsbySource = "select X_LeadSource,CAST(COUNT(*) as varchar(50)) as N_Percentage from vw_CRMLeads where N_CompanyID = "+nCompanyID+" group by X_LeadSource";
            string sqlPipelineoppotunity = "select count(*) as N_Count from CRM_Opportunity where N_ClosingStatusID=0 or N_ClosingStatusID is null and N_CompanyID = "+nCompanyID+"";
            string sqlWin = "select count(*) as N_ThisMonth from CRM_Opportunity where N_StatusTypeID=308 and MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = "+nCompanyID+""; 
            string sqlLose = "select count(*) as N_ThisMonth from CRM_Opportunity where N_StatusTypeID=309 and  MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = "+nCompanyID+""; 
            string sqlCurrentRevenue = "SELECT COUNT(*) as N_ThisMonth,sum(Cast(REPLACE(N_ExpRevenue,',','') as Numeric(10,2)) ) as TotalAmount FROM CRM_Opportunity WHERE MONTH(D_EntryDate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_EntryDate) = YEAR(CURRENT_TIMESTAMP)and N_CompanyID = "+nCompanyID+"";
            string sqlPreviousRevenue ="SELECT COUNT(*) as N_LastMonth,sum(Cast(REPLACE(N_ExpRevenue,',','') as Numeric(10,2)) ) as TotalAmount FROM CRM_Opportunity WHERE DATEPART(m, D_EntryDate) = DATEPART(m, DATEADD(m, -1, getdate()))and N_CompanyID = "+nCompanyID+"";
            string sqlTopSalesman ="select top(5) X_SalesmanName as X_SalesmanName, N_TotRevenue N_Percentage from vw_InvSalesmanRevenue where N_CompanyID = "+nCompanyID+" order by N_TotRevenue Desc";
            string sqlQuarterlyRevenue = "select sum(N_TotAmt) as N_Amount,quarter from vw_QuarterlyRevenue where N_CompanyID = "+nCompanyID+" and N_FnyearID ="+nFnYearId+" group by quarter ";

            SortedList Data=new SortedList();
            DataTable CurrentLead = new DataTable();
            DataTable CurrentCustomer = new DataTable();
            DataTable Performance = new DataTable();
            DataTable OpportunitiesStage = new DataTable();
            DataTable LeadsbySource = new DataTable();
            DataTable PipelineOppotunity = new DataTable();
            DataTable CurrentRevenue= new DataTable();
            DataTable Win = new DataTable();
            DataTable Lose = new DataTable();
            DataTable TopSalesman = new DataTable();
            DataTable QuarterlyRevenue = new DataTable();
            object LeadLastMonth="";
            object CustomerLastMonth="";
            object RevenueLastMonth="";
            object LeadPercentage="";
            object CustomerPercentage="";
            object RevenuePercentage="";

            

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
                    Win=dLayer.ExecuteDataTable(sqlWin, Params, connection);
                    Lose=dLayer.ExecuteDataTable(sqlLose, Params, connection);
                    LeadLastMonth = dLayer.ExecuteScalar(sqlPreviousLead, Params, connection);
                    CustomerLastMonth = dLayer.ExecuteScalar(sqlPreviousCustomer, Params, connection);
                    CurrentRevenue=dLayer.ExecuteDataTable(sqlCurrentRevenue, Params, connection);
                    RevenueLastMonth=dLayer.ExecuteDataTable(sqlPreviousRevenue, Params, connection);
                    TopSalesman = dLayer.ExecuteDataTable(sqlTopSalesman, Params, connection);
                    QuarterlyRevenue = dLayer.ExecuteDataTable(sqlQuarterlyRevenue, Params, connection);


                    if(myFunctions.getVAL(LeadLastMonth.ToString())!=0)
                        LeadPercentage=((myFunctions.getVAL(CurrentLead.Rows[0]["N_ThisMonth"].ToString())- myFunctions.getVAL(LeadLastMonth.ToString()))/myFunctions.getVAL(LeadLastMonth.ToString())*100).ToString();
                    if(myFunctions.getVAL(CustomerLastMonth.ToString())!=0)
                        CustomerPercentage=((myFunctions.getVAL(CurrentCustomer.Rows[0]["N_ThisMonth"].ToString())- myFunctions.getVAL(CustomerLastMonth.ToString()))/myFunctions.getVAL(CustomerLastMonth.ToString())*100).ToString();
                  if(myFunctions.getVAL(RevenueLastMonth.ToString())!=0)
                        RevenuePercentage=((myFunctions.getVAL(CurrentRevenue.Rows[0]["N_ThisMonth"].ToString())- myFunctions.getVAL(RevenueLastMonth.ToString()))/myFunctions.getVAL(RevenueLastMonth.ToString())*100).ToString();
                    
                }
                // double N_TotalOppotunity=0;
                

                // double N_TotalLead=0;
                // foreach (DataRow dtRow in LeadsbySource.Rows)
                // {N_TotalLead=N_TotalLead + myFunctions.getVAL(dtRow["N_Percentage"].ToString());}
                // foreach (DataRow dtRow in OpportunitiesStage.Rows)
                // {
                //     dtRow["N_Percentage"]=((myFunctions.getVAL(dtRow["N_Percentage"].ToString())/N_TotalLead)*100).ToString();
                // }
                

                CurrentLead = myFunctions.AddNewColumnToDataTable(CurrentLead, "N_LastMonth", typeof(string), LeadLastMonth);
                CurrentLead = myFunctions.AddNewColumnToDataTable(CurrentLead, "N_Percentage", typeof(string), LeadPercentage);
                CurrentLead.AcceptChanges();
                CurrentCustomer = myFunctions.AddNewColumnToDataTable(CurrentCustomer, "N_LastMonth", typeof(string), CustomerLastMonth);
                CurrentCustomer = myFunctions.AddNewColumnToDataTable(CurrentCustomer, "N_Percentage", typeof(string), CustomerPercentage);
                CurrentCustomer.AcceptChanges();
                CurrentRevenue = myFunctions.AddNewColumnToDataTable(CurrentRevenue, "N_LastMonth", typeof(string), RevenueLastMonth);
                CurrentRevenue = myFunctions.AddNewColumnToDataTable(CurrentRevenue, "N_Percentage", typeof(string),RevenuePercentage);
                CurrentRevenue.AcceptChanges();


                if(CurrentLead.Rows.Count>0)Data.Add("leadData",CurrentLead);
                if(CurrentCustomer.Rows.Count>0)Data.Add("customerData",CurrentCustomer);
                if(Performance.Rows.Count>0)Data.Add("performance",Performance);
                if(OpportunitiesStage.Rows.Count>0)Data.Add("opportunitiesStage",OpportunitiesStage);
                if(LeadsbySource.Rows.Count>0)Data.Add("leadsbySource",LeadsbySource);
                if(PipelineOppotunity.Rows.Count>0)Data.Add("oppotunityData",PipelineOppotunity);
                if(Win.Rows.Count>0)Data.Add("winData",Win);
                if(Lose.Rows.Count>0)Data.Add("loseData",Lose);
                if(CurrentRevenue.Rows.Count>0)Data.Add("revenueData",CurrentRevenue);
                if(TopSalesman.Rows.Count>0)Data.Add("topSalesmanData",TopSalesman);
                if(QuarterlyRevenue.Rows.Count>0)Data.Add("quarterlyRevenueData",QuarterlyRevenue);

                return Ok(api.Success(Data));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
[HttpGet("leadlist")]
        public ActionResult GetLeadList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
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
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMLeads where MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP)   " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMLeads where MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP)   " + Searchkey + " and N_LeadID not in (select top(" + Count + ") N_LeadID from vw_CRMLeads where N_CompanyID=@p1 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "Select * from vw_CRMLeads Where MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP)  ";
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


[HttpGet("opportunitylist")]
        public ActionResult GetOpportunityList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_Opportunity like'%" + xSearchkey + "%'or X_OpportunityCode like'%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_OpportunityID desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMOpportunity where N_ClosingStatusID=0 or N_ClosingStatusID is null and MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMOpportunity where N_ClosingStatusID=0 or N_ClosingStatusID is null and MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP)" + Searchkey + "  and N_OpportunityID not in (select top(" + Count + ") N_OpportunityID from vw_CRMOpportunity where N_CompanyID=@p1 " + xSortBy + ") " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "Select * from vw_CRMOpportunity where N_ClosingStatusID=0 or N_ClosingStatusID is null and MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP)";
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
  [HttpGet("customerslist")]
        public ActionResult CustomerList(int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int nCompanyId=myFunctions.GetCompanyID(User);
            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";

            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_Customer like '%" + xSearchkey + "%'or X_CustomerCode like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_CustomerID desc";
            else
                xSortBy = " order by " + xSortBy;
             
             if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_CRMCustomer where MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_CRMCustomer where MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP)" + Searchkey + " and N_CustomerID not in (select top("+ Count +") N_CustomerID from vw_CRMCustomer where N_CompanyID=@p1 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_CRMCustomer where MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP)";
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

 [HttpGet("opportunitylist1")]
        public ActionResult OpportunityList1(int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int nCompanyId=myFunctions.GetCompanyID(User);
            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_Opportunity like'%" + xSearchkey + "%'or X_OpportunityCode like'%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_OpportunityID desc";
            else
                xSortBy = " order by " + xSortBy;
             
             if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_CRMOpportunity where N_CompanyID=@p1 and (N_ClosingStatusID is null or N_ClosingStatusID=0) " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_CRMOpportunity where N_CompanyID=@p1 and (N_ClosingStatusID is null or N_ClosingStatusID=0) " + Searchkey + " and N_OpportunityID not in (select top("+ Count +") N_OpportunityID from vw_CRMOpportunity where N_CompanyID=@p1 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_CRMOpportunity where N_CompanyID=@p1";
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


        
        [HttpGet("salesManList")]
        public ActionResult GetSalesmanList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([X_SalesmanCode] like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_TotRevenue desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(10) * from vw_InvSalesmanRevenue where  N_CompanyID ="+nCompanyId+"  " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(10) * from vw_InvSalesmanRevenue where N_CompanyID ="+nCompanyId+"  " + Searchkey + " " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = api.Format(dt);
                    sqlCommandCount = "Select * from vw_InvSalesmanRevenue Where  N_CompanyID ="+nCompanyId+"   ";
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

      

    }
 
}