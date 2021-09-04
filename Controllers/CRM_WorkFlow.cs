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
    [Route("workflow")]
    [ApiController]
    public class CRM_WorkFlow : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        public CRM_WorkFlow(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1307;
        }


        [HttpGet("list")]
        public ActionResult WorkFlowList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int nCompanyId = myFunctions.GetCompanyID(User);
            string UserPattern = myFunctions.GetUserPattern(User);
            Params.Add("@p2", UserPattern);
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_WActivityCode like'%" + xSearchkey + "%'or X_WActivity like'%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_WActivityID desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRM_WorkflowMaster where N_CompanyID=@p1 and Left(X_Pattern,Len(@p2))=@p2 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRM_WorkflowMaster where N_CompanyID=@p1 and Left(X_Pattern,Len(@p2))=@p2 " + Searchkey + " and N_WActivityID not in (select top(" + Count + ") N_WActivityID from CRM_WorkflowMaster where N_CompanyID=@p1 and Left(X_Pattern,Len(@p2))=@p2 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from CRM_WorkflowMaster where N_CompanyID=@p1 and Left(X_Pattern,Len(@p2))=@p2 ";
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
        public ActionResult WorkFlowListDetails(string xActivityCode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_CRMWorkflow where N_CompanyID=@p1 and X_WActivityCode=@p3 order by N_Order";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p3", xActivityCode);
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
                DataTable DetailsTable;
                MasterTable = ds.Tables["master"];
                DetailsTable = ds.Tables["details"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nWActivityID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_WActivityID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string ActivityCode = "";
                    var values = MasterTable.Rows[0]["X_WActivityCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.FormID);
                        ActivityCode = dLayer.GetAutoNumber("CRM_WorkflowMaster", "X_WActivityCode", Params, connection, transaction);
                        if (ActivityCode == "") { transaction.Rollback(); return Ok(api.Error(User,"Unable to generate Activity Code")); }
                        MasterTable.Rows[0]["X_WActivityCode"] = ActivityCode;
                    }
                    // if (nWActivityID > 0)
                    // {
                    //     dLayer.DeleteData("CRM_WorkflowMaster", "N_WActivityID", nWActivityID, "", connection, transaction);
                    //     dLayer.DeleteData("CRM_WorkflowActivities", "N_WActivityID", nWActivityID, "", connection, transaction);
                    // }

                    nWActivityID = dLayer.SaveData("CRM_WorkflowMaster", "N_WActivityID", MasterTable, connection, transaction);
                    if (nWActivityID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        if (!DetailsTable.Columns.Contains("N_FnYearID"))
                            DetailsTable = myFunctions.AddNewColumnToDataTable(DetailsTable, "N_FnYearID", typeof(int), 0);
                        int N_Order=1;
                        foreach (DataRow var in DetailsTable.Rows)
                        {
                            var["N_WActivityID"] = nWActivityID;
                            var["N_FnYearID"] = nFnYearId;
                            var["N_Order"] = N_Order;
                            N_Order++;
                        }
                        dLayer.SaveData("CRM_WorkflowActivities", "N_WActivityDetailID", DetailsTable, connection, transaction);
                        transaction.Commit();

                        return Ok(api.Success("Workflow Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nWActivityID)
        {

            int Results = 0;
            try
            {
                SortedList Params = new SortedList();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("CRM_WorkflowMaster", "N_WActivityID", nWActivityID, "", connection, transaction);
                    if (Results > 0)
                    {
                        dLayer.DeleteData("CRM_WorkflowActivities", "N_WActivityID", nWActivityID, "", connection, transaction);
                    }
                    transaction.Commit();
                    return Ok(api.Success("Workflow deleted"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }



        }

        [HttpGet("taskworkflow")]
        public ActionResult TaskWorkFlowList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@p1", nCompanyID);
            string sqlCommandText = "select * from Prj_WorkflowMaster where N_CompanyID=@p1 order by N_WTaskID";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
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
    }
}