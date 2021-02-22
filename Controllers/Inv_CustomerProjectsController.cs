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
    [Route("projects")]
    [ApiController]
    public class InvCustomerProjectsController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
         private readonly IMyFunctions myFunctions;

        public InvCustomerProjectsController(IDataAccessLayer dl,IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions=myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        //GET api/Projects/list
        [HttpGet("list")]
        public ActionResult GetAllProjects(int? nCompanyID, int? nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from Vw_InvCustomerProjects where N_CompanyID=@p1 and N_FnYearID=@p2 order by X_ProjectCode";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
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

        [HttpGet("dashboardlist")]
        public ActionResult ProjectList(int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyId=myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and x_projectcode like '%" + xSearchkey + "%'or x_projectname like'%" + xSearchkey + "%' or x_CustomerName like '%" + xSearchkey + "%' or x_District like '%" + xSearchkey + "%' or d_StartDate like '%" + xSearchkey + "%' or n_ContractAmt like '%" + xSearchkey + "%'";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ProjectID desc";
            else
                xSortBy = " order by " + xSortBy;
             
             if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +") X_ProjectCode,X_ProjectName,X_CustomerName,X_District,X_Name,D_StartDate,N_ContractAmt,N_EstimateCost,AwardedBudget,ActualBudget,CommittedBudget,RemainingBudget,X_PO,N_Progress,N_CompanyID,N_Branchid,N_CustomerID,B_IsSaveDraft,B_Inactive,N_ProjectID from vw_InvProjectDashBoard where N_CompanyID=@p1 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top("+ nSizeperpage +") X_ProjectCode,X_ProjectName,X_CustomerName,X_District,X_Name,D_StartDate,N_ContractAmt,N_EstimateCost,AwardedBudget,ActualBudget,CommittedBudget,RemainingBudget,X_PO,N_Progress,N_CompanyID,N_Branchid,N_CustomerID,B_IsSaveDraft,B_Inactive,N_ProjectID from vw_InvProjectDashBoard where N_CompanyID=@p1 " + Searchkey + " and N_ProjectID not in (select top("+ Count +") N_ProjectID from vw_InvProjectDashBoard where N_CompanyID=@p1 "+Searchkey + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    string sqlCommandCount = "select count(*) as N_Count  from vw_InvProjectDashBoard where N_CompanyID=@p1 ";
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