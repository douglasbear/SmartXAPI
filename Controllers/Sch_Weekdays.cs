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
    [Route("weekdays")]
    [ApiController]
    public class Sch_Weekdays : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Sch_Weekdays(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult WeekdaysList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "select * from vw_sch_class where N_CompanyID=@nCompanyID";
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
        public ActionResult GetWeekdaysDetails(int nFnYearID, int nClassID, int nClassDivisionID)
        {
            DataTable dtWeekdaysDetails = new DataTable();
            DataTable dtDefaults = new DataTable();

            DataSet DS = new DataSet();
            SortedList Params = new SortedList();
            SortedList dParamList = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string DetailsWeek = "Select * from vw_Sch_WeekdaysDetails Where N_CompanyID = @p1 and N_FnYearID = @p2 and N_ClassID = @p3 and N_ClassDivisionID=@p4";

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearID);
            Params.Add("@p3", nClassID);
            Params.Add("@p4", nClassDivisionID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dtWeekdaysDetails = dLayer.ExecuteDataTable(DetailsWeek, Params, connection);

                }
                dtWeekdaysDetails = api.Format(dtWeekdaysDetails, "Details");
                SortedList Data = new SortedList();
                Data.Add("Details", dtWeekdaysDetails);
                if (dtWeekdaysDetails.Rows.Count == 0)
                {
                    dtDefaults.Columns.Add("n_WeekID");
                    dtDefaults.Columns.Add("x_Week");
                    dtDefaults.Rows.Add(new Object[]{1,"Sunday"});
                    dtDefaults.Rows.Add(new Object[]{2,"Monday"});
                    dtDefaults.Rows.Add(new Object[]{3,"Tuesday"});
                    dtDefaults.Rows.Add(new Object[]{4,"Wednesday"});
                    dtDefaults.Rows.Add(new Object[]{5,"Thursday"});
                    dtDefaults.Rows.Add(new Object[]{6,"Friday"});
                    dtDefaults.Rows.Add(new Object[]{7,"Saturday"});

                    return Ok(api.Success(dtDefaults));
                }
                else
                {
                    return Ok(api.Success(dtWeekdaysDetails));
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
                    int N_WeekID = myFunctions.getIntVAL(MasterRow["n_WeekID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string X_WeekCode = MasterRow["x_WeekCode"].ToString();

                    if (N_WeekID > 0)
                    {
                        dLayer.DeleteData("Sch_Weekdays", "N_WeekID", N_WeekID, "N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection, transaction);
                        dLayer.DeleteData("Sch_WeekdaysDetails", "N_WeekID", N_WeekID, "N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection, transaction);
                    }

                    if (X_WeekCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", 1495);
                        Params.Add("N_BranchID", 0);
                        X_WeekCode = dLayer.GetAutoNumber("Sch_Weekdays", "x_WeekCode", Params, connection, transaction);
                        if (X_WeekCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Week Code");
                        }
                        Master.Rows[0]["x_WeekCode"] = X_WeekCode;
                    }
                    string DupCriteria = "";


                    N_WeekID = dLayer.SaveData("Sch_Weekdays", "N_WeekID", DupCriteria, "", Master, connection, transaction);
                    if (N_WeekID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                    for (int i = 0; i < Details.Rows.Count; i++)
                    {
                        Details.Rows[i]["N_WeekID"] = N_WeekID;

                    }

                    dLayer.SaveData("Sch_WeekdaysDetails", "N_WeekDetailsID", Details, connection, transaction);
                    transaction.Commit();
                    SortedList Result = new SortedList();

                    return Ok(api.Success(Result, "Week Days Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int N_WeekID, int nFnYearID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Sch_Weekdays", "N_WeekID", N_WeekID, "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection);
                    dLayer.DeleteData("Sch_WeekdaysDetails", "N_WeekID", N_WeekID, "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection);

                }
                if (Results > 0)
                {
                    return Ok(api.Success("Week Days deleted"));
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



