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
    [Route("serviceCategory")]
    [ApiController]
    public class Crm_ServiceCategory : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        public Crm_ServiceCategory(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1307;
        }

        [HttpGet("details")]
        public ActionResult ServiceCategoryDetails(string xServiceCategory)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_CrmServiceCategory where N_CompanyID=@p1 and X_WActivityCode=@p3";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p3", xServiceCategory);
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
                return Ok(api.Error(e));
            }
        }
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nServiceCategoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ServiceCategoryID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string ActivityCode = "";
                    var values = MasterTable.Rows[0]["X_ServiceCategoryCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.FormID);
                        ActivityCode = dLayer.GetAutoNumber("Crm_ServiceCategory", "X_ServiceCategoryCode", Params, connection, transaction);
                        if (ActivityCode == "") { transaction.Rollback(); return Ok(api.Error("Unable to generate Code")); }
                        MasterTable.Rows[0]["X_ServiceCategoryCode"] = ActivityCode;
                    }

                    nServiceCategoryID = dLayer.SaveData("Crm_ServiceCategory", "N_ServiceCategoryID", MasterTable, connection, transaction);
                    if (nServiceCategoryID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    transaction.Commit();
                    return Ok(api.Success("Workflow Created"));
                }
            }

            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nServiceCategoryID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    Results = dLayer.DeleteData("Crm_ServiceCategory", "N_ServiceCategoryID", nServiceCategoryID, "", connection);
                    if (Results > 0)
                    {
                        return Ok(api.Success("deleted"));
                    }
                    else
                    {
                        return Ok(api.Error("Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }
        [HttpGet("serviceCategoryList")]
        public ActionResult ServiceList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select * from vw_CrmServiceCategory where N_CompanyID=@nComapnyID ";
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
                    return Ok(api.Notice("No Results Found"));
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

