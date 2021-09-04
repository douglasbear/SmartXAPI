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
    [Route("crmProject")]
    [ApiController]
    public class CRM_Project : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1303;

        public CRM_Project(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("list")]
        public ActionResult LeadList(int nFnYearId,int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
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
            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
             string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_ProjectName like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ProjectID desc";
            else
                xSortBy = " order by " + xSortBy;
             
             if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_CRM_Project where N_CompanyID=@p1 " + Pattern + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_CRM_Project where N_CompanyID=@p1 " + Pattern + Searchkey + " and N_ProjectID not in (select top("+ Count +") N_ProjectID from vw_CRM_Project where N_CompanyID=@p1 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_CRM_Project where N_CompanyID=@p1 " + Pattern;
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
         [HttpGet("listDetails")]
        public ActionResult ProjectListInner()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
           
            string sqlCommandText = "select  * from vw_CRM_Project where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
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
                
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("details")]
        public ActionResult LeadListDetails(string xProjectNo)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_CRM_Project where N_CompanyID=@p1 and X_ProjectCode=@p3";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p3", xProjectNo);
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
                int nLeadID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ProjectID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string LeadCode = "";
                    var values = MasterTable.Rows[0]["x_ProjectCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        LeadCode = dLayer.GetAutoNumber("CRM_Project", "x_ProjectCode", Params, connection, transaction);
                        if (LeadCode == "") {transaction.Rollback(); return Ok(api.Error(User,"Unable to generate Lead Code")); }
                        MasterTable.Rows[0]["x_ProjectCode"] = LeadCode;
                    }


                    nLeadID = dLayer.SaveData("CRM_Project", "n_ProjectID", MasterTable, connection, transaction);
                    if (nLeadID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Project Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nProjectID)
        {

             int Results = 0;
            try
            {                        
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("CRM_Project", "N_ProjectID", nProjectID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_ProjectID",nProjectID.ToString());
                    return Ok(api.Success(res,"Project deleted"));
                }
                else
                {
                    return Ok(api.Error(User,"Unable to delete Project"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }



        }
    }
}