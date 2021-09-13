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
    [Route("projectworkflow")]
    [ApiController]
    public class Prj_WorkFlow : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        public Prj_WorkFlow(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1383;
        }


        [HttpGet("list")]
        public ActionResult ProjectWorkFlowList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            string UserPattern = myFunctions.GetUserPattern(User);
            string Pattern = "";
            if (UserPattern != "")
            {
                Pattern = " and Left(X_Pattern,Len(@p3))=@p3";
                Params.Add("@p3", UserPattern);
            }
            int nCompanyId = myFunctions.GetCompanyID(User);
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (N_WTaskID like'%" + xSearchkey + "%'or X_WTask like'%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_WTaskID desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Prj_WorkflowMaster where N_CompanyID=@p1 " + Pattern + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Prj_WorkflowMaster where N_CompanyID=@p1 " + Pattern + Searchkey + " and N_WTaskID not in (select top(" + Count + ") N_WTaskID from vw_Prj_WorkflowMaster where N_CompanyID=@p1 " + Pattern +  xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_Prj_WorkflowMaster where N_CompanyID=@p1";
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
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("details")]
        public ActionResult ProjectWorkFlowDetails(string xWTaskCode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_Prj_WorkflowDetails where N_CompanyID=@p1 and X_WTaskCode=@p3 order by N_Order";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p3", xWTaskCode);
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
                return Ok(api.Error(User, e));
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
                int nWTaskID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_WTaskID"].ToString());
                int N_CreatorID = myFunctions.GetUserID(User);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string TaskCode = "";
                    var values = MasterTable.Rows[0]["X_WTaskCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.FormID);
                        TaskCode = dLayer.GetAutoNumber("Prj_WorkflowMaster", "X_WTaskCode", Params, connection, transaction);
                        if (TaskCode == "") { transaction.Rollback(); return Ok(api.Error(User, "Unable to generate Task Code")); }
                        MasterTable.Rows[0]["X_WTaskCode"] = TaskCode;
                    }
                    nWTaskID = dLayer.SaveData("Prj_WorkflowMaster", "N_WTaskID", MasterTable, connection, transaction);
                    if (nWTaskID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                    else
                    {
                        if (!DetailsTable.Columns.Contains("N_FnYearID"))
                            DetailsTable = myFunctions.AddNewColumnToDataTable(DetailsTable, "N_FnYearID", typeof(int), 0);
                        if (!DetailsTable.Columns.Contains("N_CreaterID"))
                            DetailsTable = myFunctions.AddNewColumnToDataTable(DetailsTable, "N_CreaterID", typeof(int), N_CreatorID);
                        int N_Order = 1;
                        foreach (DataRow var in DetailsTable.Rows)
                        {
                            var["N_WTaskID"] = nWTaskID;
                            var["N_FnYearID"] = nFnYearId;
                            var["N_Order"] = N_Order;
                            N_Order++;
                        }
                        dLayer.SaveData("Prj_WorkflowTasks", "N_WTaskDetailID", DetailsTable, connection, transaction);
                        transaction.Commit();

                        return Ok(api.Success("Workflow Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nWTaskID)
        {

            int Results = 0;
            try
            {
                SortedList Params = new SortedList();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Prj_WorkflowMaster", "N_WTaskID", nWTaskID, "", connection, transaction);
                    if (Results > 0)
                    {
                        dLayer.DeleteData("Prj_WorkflowTasks", "N_WTaskID", nWTaskID, "", connection, transaction);
                    }
                    transaction.Commit();
                    return Ok(api.Success("Workflow deleted"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }



        }
        [HttpDelete("deletelinewise")]
        public ActionResult DeleteDataLineWise(int N_WTaskDetailID)
        {

            int Results = 0;
            try
            {
                SortedList Params = new SortedList();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                        dLayer.DeleteData("Prj_WorkflowTasks", "N_WTaskDetailID", N_WTaskDetailID, "", connection, transaction);

                    transaction.Commit();
                    return Ok(api.Success("Workflow deleted"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }



        }
    }
}