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
    [Route("apiServiceSettings")]
    [ApiController]
    public class Gen_APIServiceSettings : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Gen_APIServiceSettings(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("details")]
        public ActionResult GetServicesettingsDetails(int nFnYearID)
        {

            DataTable dtServiceDetails = new DataTable();

            DataSet DS = new DataSet();
            SortedList Params = new SortedList();
            SortedList dParamList = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string DetailsService = "Select * from Vw_ServiceSettingsDetails Where N_CompanyID = @p1 and N_FnYearID = @p2";

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dtServiceDetails = dLayer.ExecuteDataTable(DetailsService, Params, connection);

                }
                dtServiceDetails = api.Format(dtServiceDetails, "Details");

                SortedList Data = new SortedList();
                Data.Add("Details", dtServiceDetails);

                if (dtServiceDetails.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(Data));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable Details = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    int nCompanyId = myFunctions.GetCompanyID(User);

                    dLayer.DeleteData("API_ServiceSettings", "N_CompanyID", nCompanyId, "", connection, transaction);

                    int N_ServiceSettingsID = dLayer.SaveData("API_ServiceSettings", "N_ServiceSettingsID", Details, connection, transaction);
                    if (N_ServiceSettingsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                    transaction.Commit();
                    SortedList Result = new SortedList();

                    return Ok(api.Success(Result, "Service Settings Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nFnYearID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("API_ServiceSettings", "N_CompanyID", nCompanyID, "", connection);

                }
                if (Results > 0)
                {
                    return Ok(api.Success("Service Settings deleted"));
                }
                else
                {
                    return Ok(api.Error(User, "Unable to delete"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }

        }

    }
}



