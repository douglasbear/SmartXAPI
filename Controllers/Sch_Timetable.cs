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
    [Route("timetable")]
    [ApiController]
    public class Sch_Timetable : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Sch_Timetable(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult TimetableList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "select * from Sch_Timetable where N_CompanyID=@nCompanyID";
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
        [HttpGet("listdetails")]
        public ActionResult TimetableFillDetails(int nClassID, int nClassDivisionID)
        {
            DataTable dt = new DataTable();
            DataTable dtSunday = new DataTable();
            DataTable dtMonday = new DataTable();
            DataTable dtTuesday = new DataTable();
            DataTable dtWednesday = new DataTable();
            DataTable dtThursday = new DataTable();
            DataTable dtFriday = new DataTable();
            DataTable dtSaturday = new DataTable();
            DataTable dtMain = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nClassID", nClassID);
            Params.Add("@nClassDivisionID", nClassDivisionID);
            dt = myFunctions.AddNewColumnToDataTable(dt, "Day", typeof(string), null);
            dt = myFunctions.AddNewColumnToDataTable(dt, "weekdata", typeof(DataTable), null);
            dt = myFunctions.AddNewColumnToDataTable(dt, "x_weekname", typeof(string), null);
            string sqlMain = "select * from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID";
            string sqlSunday = "select CONVERT(VARCHAR(5),vw_TimetableDisplay.D_StartTime,108) +' - '+ CONVERT(VARCHAR(5),vw_TimetableDisplay.D_EndTime,108)as x_time,0 as n_TeacherID,'' as x_TeacherName,0 as n_SubjectID,'' as x_Subject,'Sunday' as x_WeekName,B_Interval  from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID and x_week='Sunday'";
            string sqlMonday = "select CONVERT(VARCHAR(5),vw_TimetableDisplay.D_StartTime,108) +' - '+ CONVERT(VARCHAR(5),vw_TimetableDisplay.D_EndTime,108)as x_time,0 as n_TeacherID,'' as x_TeacherName,0 as n_SubjectID,'' as x_Subject,'Monday' as x_WeekName,B_Interval from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID and x_week='Monday'";
            string sqlTuesday = "select CONVERT(VARCHAR(5),vw_TimetableDisplay.D_StartTime,108) +' - '+ CONVERT(VARCHAR(5),vw_TimetableDisplay.D_EndTime,108)as x_time,0 as n_TeacherID,'' as x_TeacherName,0 as n_SubjectID,'' as x_Subject,'Tuesday' as x_WeekName,B_Interval from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID and x_week='Tuesday'";
            string sqlWednesday = "select CONVERT(VARCHAR(5),vw_TimetableDisplay.D_StartTime,108) +' - '+ CONVERT(VARCHAR(5),vw_TimetableDisplay.D_EndTime,108)as x_time,0 as n_TeacherID,'' as x_TeacherName,0 as n_SubjectID,'' as x_Subject,'Wednesday' as x_WeekName,B_Interval from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID and x_week='Wednesday'";
            string sqlThursday = "select CONVERT(VARCHAR(5),vw_TimetableDisplay.D_StartTime,108) +' - '+ CONVERT(VARCHAR(5),vw_TimetableDisplay.D_EndTime,108)as x_time,0 as n_TeacherID,'' as x_TeacherName,0 as n_SubjectID,'' as x_Subject,'Thursday' as x_WeekName,B_Interval from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID and x_week='Thursday'";
            string sqlFriday = "select CONVERT(VARCHAR(5),vw_TimetableDisplay.D_StartTime,108) +' - '+ CONVERT(VARCHAR(5),vw_TimetableDisplay.D_EndTime,108)as x_time,0 as n_TeacherID,'' as x_TeacherName,0 as n_SubjectID,'' as x_Subject,'Friday' as x_WeekName,B_Interval from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID and x_week='Friday'";
            string sqlSaturday = "select CONVERT(VARCHAR(5),vw_TimetableDisplay.D_StartTime,108) +' - '+ CONVERT(VARCHAR(5),vw_TimetableDisplay.D_EndTime,108)as x_time,0 as n_TeacherID,'' as x_TeacherName,0 as n_SubjectID,'' as x_Subject,'Saturday' as x_WeekName,B_Interval from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID and x_week='Saturday'";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string Day = "";
                    string Weekname = "";

                    dtSunday = dLayer.ExecuteDataTable(sqlSunday, Params, connection);
                    dtMonday = dLayer.ExecuteDataTable(sqlMonday, Params, connection);
                    dtTuesday = dLayer.ExecuteDataTable(sqlTuesday, Params, connection);
                    dtWednesday = dLayer.ExecuteDataTable(sqlWednesday, Params, connection);
                    dtThursday = dLayer.ExecuteDataTable(sqlThursday, Params, connection);
                    dtFriday = dLayer.ExecuteDataTable(sqlFriday, Params, connection);
                    dtSaturday = dLayer.ExecuteDataTable(sqlSaturday, Params, connection);
                    dtMain = dLayer.ExecuteDataTable(sqlMain, Params, connection);
                    for (int i = 0; i <= 6; i++)
                    {
                        dt.Rows.Add();
                        if (i == 0)
                        {
                            Day = "Sunday";
                            Weekname = dtMain.Rows[0]["x_weekname"].ToString();
                            dt.Rows[i]["weekdata"] = dtSunday;
                        }
                        if (i == 1)
                        {
                            Day = "Monday";
                            dt.Rows[i]["weekdata"] = dtMonday;
                        }
                        if (i == 2)
                        {
                            Day = "Tuesday";
                            dt.Rows[i]["weekdata"] = dtTuesday;
                        }
                        if (i == 3)
                        {
                            Day = "Wednesday";
                            dt.Rows[i]["weekdata"] = dtWednesday;
                        }
                        if (i == 4)
                        {
                            Day = "Thursday";
                            dt.Rows[i]["weekdata"] = dtThursday;
                        }

                        if (i == 5)
                        {
                            Day = "Friday";
                            dt.Rows[i]["weekdata"] = dtFriday;
                        }
                        if (i == 6)
                        {
                            Day = "Saturday";
                            dt.Rows[i]["weekdata"] = dtSaturday;
                        }
                        dt.Rows[i]["Day"] = Day;
                        dt.Rows[i]["x_weekname"] = Weekname;


                    }

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
        public ActionResult GetWeekdaysDetails(int N_WeekID, int nFnYearID)
        {
            DataTable dtWeekdaysMaster = new DataTable();
            DataTable dtWeekdaysDetails = new DataTable();

            DataSet DS = new DataSet();
            SortedList Params = new SortedList();
            SortedList dParamList = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string MasterWeek = "Select * from vw_sch_class Where N_CompanyID = @p1 and N_FnYearID = @p2 and N_WeekID = @p3";
            string DetailsWeek = "Select * from vw_Sch_WeekdaysDetails Where N_CompanyID = @p1 and N_FnYearID = @p2 and N_WeekID = @p3";

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearID);
            Params.Add("@p3", N_WeekID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dtWeekdaysMaster = dLayer.ExecuteDataTable(MasterWeek, Params, connection);
                    dtWeekdaysDetails = dLayer.ExecuteDataTable(DetailsWeek, Params, connection);

                }
                dtWeekdaysMaster = api.Format(dtWeekdaysMaster, "Master");
                dtWeekdaysDetails = api.Format(dtWeekdaysDetails, "Details");

                SortedList Data = new SortedList();
                Data.Add("Master", dtWeekdaysMaster);
                Data.Add("Details", dtWeekdaysDetails);

                if (dtWeekdaysMaster.Rows.Count == 0)
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



