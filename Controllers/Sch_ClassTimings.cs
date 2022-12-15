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
    [Route("classTimings")]
    [ApiController]
    public class Sch_ClassTimings : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Sch_ClassTimings(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult ClassTimingsList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "select * from Sch_ClassTimings where N_CompanyID=@nCompanyID";
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
                return Ok(api.Error(User, e));
            }
        }
        [HttpGet("details")]
        public ActionResult GetClassTimingsDetails(int N_TimingID, int nFnYearID)
        {
            DataTable dtClassTimingsMaster = new DataTable();
            DataTable dtClassTimingsDetails = new DataTable();

            DataSet DS = new DataSet();
            SortedList Params = new SortedList();
            SortedList dParamList = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string MasterClassTimings = "Select * from Sch_ClassTimings Where N_CompanyID = @p1 and N_FnYearID = @p2 and N_TimingID = @p3";
            string DetailsClassTimings = "Select * from Sch_ClassTimingsDetails Where N_CompanyID = @p1 and N_FnYearID = @p2 and N_TimingID = @p3";

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearID);
            Params.Add("@p3", N_TimingID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dtClassTimingsMaster = dLayer.ExecuteDataTable(MasterClassTimings, Params, connection);
                    dtClassTimingsDetails = dLayer.ExecuteDataTable(DetailsClassTimings, Params, connection);

                }
                dtClassTimingsMaster = api.Format(dtClassTimingsMaster, "Master");
                dtClassTimingsDetails = api.Format(dtClassTimingsDetails, "Details");

                SortedList Data = new SortedList();
                Data.Add("Master", dtClassTimingsMaster);
                Data.Add("Details", dtClassTimingsDetails);

                if (dtClassTimingsMaster.Rows.Count == 0)
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



                    DataTable Master = ds.Tables["master"];
                    DataTable Details = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    DataRow MasterRow = Master.Rows[0];
                    int N_TimingID = myFunctions.getIntVAL(MasterRow["N_TimingID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string X_TimingCode = MasterRow["X_TimingCode"].ToString();

                    if(N_TimingID>0)
                     {
                      dLayer.DeleteData("Sch_ClassTimings", "N_TimingID", N_TimingID, "N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection,transaction);
                      dLayer.DeleteData("Sch_ClassTimings", "N_TimingID", N_TimingID, "N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection,transaction);
                     }
               
                    if (X_TimingCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", 1494);
                        Params.Add("N_BranchID", 0);
                        X_TimingCode = dLayer.GetAutoNumber("Sch_ClassTimings", "X_TimingCode", Params, connection, transaction);
                        if (X_TimingCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Timing Code");
                        }
                        Master.Rows[0]["X_TimingCode"] = X_TimingCode;
                    }
                    string DupCriteria = "";


                    N_TimingID = dLayer.SaveData("Sch_ClassTimings", "N_TimingID", DupCriteria, "", Master, connection, transaction);
                    if (N_TimingID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                    dLayer.DeleteData("Sch_ClassTimingsDetails", "N_TimingID", N_TimingID, "", connection, transaction);
                    for (int i = 0; i < Details.Rows.Count; i++)
                    {
                        Details.Rows[i]["N_TimingID"] = N_TimingID;

                    }

                    dLayer.SaveData("Sch_ClassTimingsDetails", "N_TimingDetailsID", Details, connection, transaction);
                    transaction.Commit();
                    SortedList Result = new SortedList();

                    return Ok(api.Success(Result, "Class Timings Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int N_TimingID, int nFnYearID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Sch_ClassTimings", "N_TimingID", N_TimingID, "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection);
                    dLayer.DeleteData("Sch_ClassTimingsDetails", "N_TimingID", N_TimingID, "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection);

                }
                if (Results > 0)
                {
                    return Ok(api.Success("Class Timings deleted"));
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



