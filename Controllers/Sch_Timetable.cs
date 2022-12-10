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
            string sqlCommandText = "select * from vw_Timetable where N_CompanyID=@nCompanyID order by n_timetableid desc";
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
        public ActionResult TimetableFillDetails(int nClassID, int nClassDivisionID, int type, string xTimetableCode, int nStudentID, bool isDashboard)
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
            string sqlMain = "";
            string sqlSunday = "";
            string sqlMonday = "";
            string sqlTuesday = "";
            string sqlWednesday = "";
            string sqlThursday = "";
            string sqlFriday = "";
            string sqlSaturday = "";
            int nCompanyID = myFunctions.GetCompanyID(User);

            Params.Add("@nCompanyID", nCompanyID);
            dt = myFunctions.AddNewColumnToDataTable(dt, "Day", typeof(string), null);
            dt = myFunctions.AddNewColumnToDataTable(dt, "weekdata", typeof(DataTable), null);
            dt = myFunctions.AddNewColumnToDataTable(dt, "x_weekname", typeof(string), null);
            dt = myFunctions.AddNewColumnToDataTable(dt, "n_ClassID", typeof(string), null);
            dt = myFunctions.AddNewColumnToDataTable(dt, "x_Class", typeof(string), null);
            dt = myFunctions.AddNewColumnToDataTable(dt, "n_ClassDivisionID", typeof(string), null);
            dt = myFunctions.AddNewColumnToDataTable(dt, "x_ClassDivision", typeof(string), null);
            dt = myFunctions.AddNewColumnToDataTable(dt, "n_TimetableID", typeof(string), null);
            dt = myFunctions.AddNewColumnToDataTable(dt, "d_EffectiveDate", typeof(string), null);
            dt = myFunctions.AddNewColumnToDataTable(dt, "count", typeof(int), null);
            if (type == 1)
            {

                sqlMain = "select * from vw_Timetable where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode";
                sqlSunday = "select  x_time,n_TeacherID,x_TeacherName,n_SubjectID,x_Subject,'Sunday' as x_WeekName,B_Interval,N_TimetableID,N_TimetableDetailsID,N_CompanyID,N_FnyearID,X_Class,X_ClassDivision,n_ClassID,n_ClassDivisionID  from vw_TimetableDetails where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode and x_weekname='Sunday'";
                sqlMonday = "select  x_time,n_TeacherID,x_TeacherName,n_SubjectID,x_Subject,'Monday' as x_WeekName,B_Interval,N_TimetableID,N_TimetableDetailsID,N_CompanyID,N_FnyearID,X_Class,X_ClassDivision,n_ClassID,n_ClassDivisionID  from vw_TimetableDetails where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode and x_weekname='Monday'";
                sqlTuesday = "select  x_time,n_TeacherID,x_TeacherName,n_SubjectID,x_Subject,'Tuesday' as x_WeekName,B_Interval,N_TimetableID,N_TimetableDetailsID,N_CompanyID,N_FnyearID,X_Class,X_ClassDivision,n_ClassID,n_ClassDivisionID  from vw_TimetableDetails where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode and x_weekname='Tuesday'";
                sqlWednesday = "select  x_time,n_TeacherID,x_TeacherName,n_SubjectID,x_Subject,'Wednesday' as x_WeekName,B_Interval,N_TimetableID,N_TimetableDetailsID,N_CompanyID,N_FnyearID,X_Class,X_ClassDivision,n_ClassID,n_ClassDivisionID  from vw_TimetableDetails where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode and x_weekname='Wednesday'";
                sqlThursday = "select  x_time,n_TeacherID,x_TeacherName,n_SubjectID,x_Subject,'Thursday' as x_WeekName,B_Interval,N_TimetableID,N_TimetableDetailsID,N_CompanyID,N_FnyearID,X_Class,X_ClassDivision,n_ClassID,n_ClassDivisionID  from vw_TimetableDetails where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode and x_weekname='Thursday'";
                sqlFriday = "select  x_time,n_TeacherID,x_TeacherName,n_SubjectID,x_Subject,'Friday' as x_WeekName,B_Interval,N_TimetableID,N_TimetableDetailsID,N_CompanyID,N_FnyearID,X_Class,X_ClassDivision,n_ClassID,n_ClassDivisionID  from vw_TimetableDetails where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode and x_weekname='Friday'";
                sqlSaturday = "select  x_time,n_TeacherID,x_TeacherName,n_SubjectID,x_Subject,'Saturday' as x_WeekName,B_Interval,N_TimetableID,N_TimetableDetailsID,N_CompanyID,N_FnyearID,X_Class,X_ClassDivision,n_ClassID,n_ClassDivisionID  from vw_TimetableDetails where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode and x_weekname='Saturday'";


            }
            else
            {

                sqlMain = "select * from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID";
                sqlSunday = "select CONVERT(VARCHAR(5),vw_TimetableDisplay.D_StartTime,108) +' - '+ CONVERT(VARCHAR(5),vw_TimetableDisplay.D_EndTime,108)as x_time,0 as n_TeacherID,'' as x_TeacherName,0 as n_SubjectID,'' as x_Subject,'Sunday' as x_WeekName,B_Interval,0 as N_TimetableID,0 as N_TimetableDetailsID,N_CompanyID,N_FnyearID,'' as X_Class,'' as X_ClassDivision,0 as n_ClassID,0 as n_ClassDivisionID  from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID and x_week='Sunday'";
                sqlMonday = "select CONVERT(VARCHAR(5),vw_TimetableDisplay.D_StartTime,108) +' - '+ CONVERT(VARCHAR(5),vw_TimetableDisplay.D_EndTime,108)as x_time,0 as n_TeacherID,'' as x_TeacherName,0 as n_SubjectID,'' as x_Subject,'Monday' as x_WeekName,B_Interval,0 as N_TimetableID,0 as N_TimetableDetailsID,N_CompanyID,N_FnyearID,'' as X_Class,'' as X_ClassDivision,0 as n_ClassID,0 as n_ClassDivisionID from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID and x_week='Monday'";
                sqlTuesday = "select CONVERT(VARCHAR(5),vw_TimetableDisplay.D_StartTime,108) +' - '+ CONVERT(VARCHAR(5),vw_TimetableDisplay.D_EndTime,108)as x_time,0 as n_TeacherID,'' as x_TeacherName,0 as n_SubjectID,'' as x_Subject,'Tuesday' as x_WeekName,B_Interval,0 as N_TimetableID,0 as N_TimetableDetailsID,N_CompanyID,N_FnyearID,'' as X_Class,'' as X_ClassDivision,0 as n_ClassID,0 as n_ClassDivisionID from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID and x_week='Tuesday'";
                sqlWednesday = "select CONVERT(VARCHAR(5),vw_TimetableDisplay.D_StartTime,108) +' - '+ CONVERT(VARCHAR(5),vw_TimetableDisplay.D_EndTime,108)as x_time,0 as n_TeacherID,'' as x_TeacherName,0 as n_SubjectID,'' as x_Subject,'Wednesday' as x_WeekName,B_Interval,0 as N_TimetableID,0 as N_TimetableDetailsID,N_CompanyID,N_FnyearID,'' as X_Class,'' as X_ClassDivision,0 as n_ClassID,0 as n_ClassDivisionID from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID and x_week='Wednesday'";
                sqlThursday = "select CONVERT(VARCHAR(5),vw_TimetableDisplay.D_StartTime,108) +' - '+ CONVERT(VARCHAR(5),vw_TimetableDisplay.D_EndTime,108)as x_time,0 as n_TeacherID,'' as x_TeacherName,0 as n_SubjectID,'' as x_Subject,'Thursday' as x_WeekName,B_Interval,0 as N_TimetableID,0 as N_TimetableDetailsID,N_CompanyID,N_FnyearID,'' as X_Class,'' as X_ClassDivision,0 as n_ClassID,0 as n_ClassDivisionID from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID and x_week='Thursday'";
                sqlFriday = "select CONVERT(VARCHAR(5),vw_TimetableDisplay.D_StartTime,108) +' - '+ CONVERT(VARCHAR(5),vw_TimetableDisplay.D_EndTime,108)as x_time,0 as n_TeacherID,'' as x_TeacherName,0 as n_SubjectID,'' as x_Subject,'Friday' as x_WeekName,B_Interval,0 as N_TimetableID,0 as N_TimetableDetailsID,N_CompanyID,N_FnyearID,'' as X_Class,'' as X_ClassDivision,0 as n_ClassID,0 as n_ClassDivisionID from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID and x_week='Friday'";
                sqlSaturday = "select CONVERT(VARCHAR(5),vw_TimetableDisplay.D_StartTime,108) +' - '+ CONVERT(VARCHAR(5),vw_TimetableDisplay.D_EndTime,108)as x_time,0 as n_TeacherID,'' as x_TeacherName,0 as n_SubjectID,'' as x_Subject,'Saturday' as x_WeekName,B_Interval,0 as N_TimetableID,0 as N_TimetableDetailsID,N_CompanyID,N_FnyearID,'' as X_Class,'' as X_ClassDivision,0 as n_ClassID,0 as n_ClassDivisionID from vw_TimetableDisplay where N_CompanyID=@nCompanyID and N_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID and x_week='Saturday'";
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string Day = "";
                    string Weekname = "";
                    object n_ClassID = 0;
                    object x_Class = "";
                    object n_ClassDivisionID = 0;
                    object x_ClassDivision = "";
                    object n_TimetableID = "";
                    object d_EffectiveDate = "";

                    SqlTransaction transaction = connection.BeginTransaction();
                    if (isDashboard && nStudentID > 0)
                    {
                        object N_ClassID = dLayer.ExecuteScalar("select N_ClassID from sch_Admission where N_AdmissionID=" + nStudentID + " and N_CompanyID =@nCompanyID", Params, connection, transaction);
                        if (N_ClassID != null)
                            Params.Add("@nClassID", myFunctions.getIntVAL(N_ClassID.ToString()));
                        object N_ClassDivisionID = dLayer.ExecuteScalar("select N_DivisionID from sch_Admission where N_AdmissionID=" + nStudentID + " and N_CompanyID =@nCompanyID", Params, connection, transaction);
                        if (N_ClassDivisionID != null)
                            Params.Add("@nClassDivisionID", myFunctions.getIntVAL(N_ClassDivisionID.ToString()));
                        object TimetableCode = dLayer.ExecuteScalar("select X_TimetableCode from vw_Timetable where n_ClassID=@nClassID and N_ClassDivisionID=@nClassDivisionID and N_CompanyID =@nCompanyID", Params, connection, transaction);
                        if (TimetableCode != null)
                            Params.Add("@xTimetableCode", myFunctions.getVAL(TimetableCode.ToString()));


                    }
                    else
                    {
                        Params.Add("@nClassID", nClassID);
                        Params.Add("@nClassDivisionID", nClassDivisionID);
                        if (xTimetableCode != null)
                            Params.Add("@xTimetableCode", xTimetableCode);
                    }


                    if (type == 1)
                    {
                        n_ClassID = dLayer.ExecuteScalar("Select n_ClassID from vw_Timetable where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode", Params, connection, transaction);
                        x_Class = dLayer.ExecuteScalar("Select x_Class from vw_Timetable where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode", Params, connection, transaction);
                        n_ClassDivisionID = dLayer.ExecuteScalar("Select n_ClassDivisionID from vw_Timetable where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode", Params, connection, transaction);
                        x_ClassDivision = dLayer.ExecuteScalar("Select x_ClassDivision from vw_Timetable where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode", Params, connection, transaction);
                        n_TimetableID = dLayer.ExecuteScalar("Select n_TimetableID from vw_Timetable where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode", Params, connection, transaction);
                        d_EffectiveDate = dLayer.ExecuteScalar("Select d_EffectiveDate from vw_Timetable where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode", Params, connection, transaction);
                    }
                    dtSunday = dLayer.ExecuteDataTable(sqlSunday, Params, connection, transaction);
                    dtMonday = dLayer.ExecuteDataTable(sqlMonday, Params, connection, transaction);
                    dtTuesday = dLayer.ExecuteDataTable(sqlTuesday, Params, connection, transaction);
                    dtWednesday = dLayer.ExecuteDataTable(sqlWednesday, Params, connection, transaction);
                    dtThursday = dLayer.ExecuteDataTable(sqlThursday, Params, connection, transaction);
                    dtFriday = dLayer.ExecuteDataTable(sqlFriday, Params, connection, transaction);
                    dtSaturday = dLayer.ExecuteDataTable(sqlSaturday, Params, connection, transaction);
                    dtMain = dLayer.ExecuteDataTable(sqlMain, Params, connection, transaction);
                    int row=0;
                    for (int i = 0; i <= 6; i++)
                    {
                        

                        if (i == 0)
                        {
                            Day = "Sunday";
                            if (dtSunday.Rows.Count == 0)
                                continue;
                            dt.Rows.Add();
                            dt.Rows[row]["weekdata"] = dtSunday;
                        }
                        if (i == 1)
                        {
                            Day = "Monday";
                            if (dtMonday.Rows.Count == 0)
                                continue;
                            dt.Rows.Add();
                            dt.Rows[row]["weekdata"] = dtMonday;
                        }
                        if (i == 2)
                        {
                            if (dtTuesday.Rows.Count == 0)
                                continue;
                            dt.Rows.Add();
                            Day = "Tuesday";
                            dt.Rows[row]["weekdata"] = dtTuesday;
                        }
                        if (i == 3)
                        {
                            if (dtWednesday.Rows.Count == 0)
                                continue;
                            dt.Rows.Add();
                            Day = "Wednesday";
                            dt.Rows[row]["weekdata"] = dtWednesday;
                        }
                        if (i == 4)
                        {
                            if (dtThursday.Rows.Count == 0)
                                continue;
                            dt.Rows.Add();
                            Day = "Thursday";
                            dt.Rows[row]["weekdata"] = dtThursday;
                        }

                        if (i == 5)
                        {
                            if (dtFriday.Rows.Count == 0)
                                continue;
                            dt.Rows.Add();
                            Day = "Friday";
                            dt.Rows[row]["weekdata"] = dtFriday;
                        }
                        if (i == 6)
                        {
                            if (dtSaturday.Rows.Count == 0)
                                continue;
                            dt.Rows.Add();
                            Day = "Saturday";
                            dt.Rows[row]["weekdata"] = dtSaturday;
                        }
                        dt.Rows[row]["Day"] = Day;
                        dt.Rows[row]["x_weekname"] = Weekname;
                        dt.Rows[row]["n_ClassID"] = n_ClassID;
                        dt.Rows[row]["x_Class"] = x_Class;
                        dt.Rows[row]["n_ClassDivisionID"] = n_ClassDivisionID;
                        dt.Rows[row]["x_ClassDivision"] = x_ClassDivision;
                        dt.Rows[row]["n_timetableID"] = n_TimetableID;
                        dt.Rows[row]["d_EffectiveDate"] = d_EffectiveDate;
                        row=row+1;


                    }

                }
                foreach (DataRow row in dt.Rows)
                {
                    // if (row["weekdata"].Rows.count() == "")
                    // {
                    //     var a="test";
                    // }

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
        public ActionResult TimetableDetails(string xTimetableCode, int nFnYearID)
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
            Params.Add("@xTimetableCode", xTimetableCode);
            dt = myFunctions.AddNewColumnToDataTable(dt, "Day", typeof(string), null);
            dt = myFunctions.AddNewColumnToDataTable(dt, "weekdata", typeof(DataTable), null);
            dt = myFunctions.AddNewColumnToDataTable(dt, "x_weekname", typeof(string), null);
            string sqlMain = "select * from vw_Timetable where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode";
            string sqlSunday = "select  x_time,n_TeacherID,x_TeacherName,n_SubjectID,x_Subject,'Sunday' as x_WeekName,B_Interval,N_TimetableID,N_TimetableDetailsID,N_CompanyID,N_FnyearID  from vw_TimetableDetails where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode and x_weekname='Sunday'";
            string sqlMonday = "select  x_time,n_TeacherID,x_TeacherName,n_SubjectID,x_Subject,'Sunday' as x_WeekName,B_Interval,N_TimetableID,N_TimetableDetailsID,N_CompanyID,N_FnyearID  from vw_TimetableDetails where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode and x_weekname='Monday'";
            string sqlTuesday = "select  x_time,n_TeacherID,x_TeacherName,n_SubjectID,x_Subject,'Sunday' as x_WeekName,B_Interval,N_TimetableID,N_TimetableDetailsID,N_CompanyID,N_FnyearID  from vw_TimetableDetails where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode and x_weekname='Tuesday'";
            string sqlWednesday = "select  x_time,n_TeacherID,x_TeacherName,n_SubjectID,x_Subject,'Sunday' as x_WeekName,B_Interval,N_TimetableID,N_TimetableDetailsID,N_CompanyID,N_FnyearID  from vw_TimetableDetails where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode and x_weekname='Wednesday'";
            string sqlThursday = "select  x_time,n_TeacherID,x_TeacherName,n_SubjectID,x_Subject,'Sunday' as x_WeekName,B_Interval,N_TimetableID,N_TimetableDetailsID,N_CompanyID,N_FnyearID  from vw_TimetableDetails where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode and x_weekname='Thursday'";
            string sqlFriday = "select  x_time,n_TeacherID,x_TeacherName,n_SubjectID,x_Subject,'Sunday' as x_WeekName,B_Interval,N_TimetableID,N_TimetableDetailsID,N_CompanyID,N_FnyearID  from vw_TimetableDetails where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode and x_weekname='Friday'";
            string sqlSaturday = "select  x_time,n_TeacherID,x_TeacherName,n_SubjectID,x_Subject,'Sunday' as x_WeekName,B_Interval,N_TimetableID,N_TimetableDetailsID,N_CompanyID,N_FnyearID  from vw_TimetableDetails where N_CompanyID=@nCompanyID and X_TimetableCode=@xTimetableCode and x_weekname='Saturday'";
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
                    return Ok(api.Notice(""));
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
        // [HttpGet("details")]
        // public ActionResult GetWeekdaysDetails(int N_WeekID, int nFnYearID)
        // {
        //     DataTable dtWeekdaysMaster = new DataTable();
        //     DataTable dtWeekdaysDetails = new DataTable();

        //     DataSet DS = new DataSet();
        //     SortedList Params = new SortedList();
        //     SortedList dParamList = new SortedList();
        //     int nCompanyId = myFunctions.GetCompanyID(User);

        //     string MasterWeek = "Select * from vw_sch_class Where N_CompanyID = @p1 and N_FnYearID = @p2 and N_WeekID = @p3";
        //     string DetailsWeek = "Select * from vw_Sch_WeekdaysDetails Where N_CompanyID = @p1 and N_FnYearID = @p2 and N_WeekID = @p3";

        //     Params.Add("@p1", nCompanyId);
        //     Params.Add("@p2", nFnYearID);
        //     Params.Add("@p3", N_WeekID);
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             dtWeekdaysMaster = dLayer.ExecuteDataTable(MasterWeek, Params, connection);
        //             dtWeekdaysDetails = dLayer.ExecuteDataTable(DetailsWeek, Params, connection);

        //         }
        //         dtWeekdaysMaster = api.Format(dtWeekdaysMaster, "Master");
        //         dtWeekdaysDetails = api.Format(dtWeekdaysDetails, "Details");

        //         SortedList Data = new SortedList();
        //         Data.Add("Master", dtWeekdaysMaster);
        //         Data.Add("Details", dtWeekdaysDetails);

        //         if (dtWeekdaysMaster.Rows.Count == 0)
        //         {
        //             return Ok(api.Warning("No Results Found"));
        //         }
        //         else
        //         {
        //             return Ok(api.Success(Data));
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(api.Error(User, e));
        //     }
        // }


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
                    DataTable Details = ds.Tables["timetableListDetails"];
                    SortedList Params = new SortedList();
                    DataRow MasterRow = Master.Rows[0];
                    int N_TimetableID = myFunctions.getIntVAL(MasterRow["n_TimetableID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string x_TimetableCode = MasterRow["x_TimetableCode"].ToString();

                    if (N_TimetableID > 0)
                    {
                        dLayer.DeleteData("Sch_Timetable", "N_TimetableID", N_TimetableID, "N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection, transaction);
                        dLayer.DeleteData("Sch_Timetabledetails", "N_TimetableID", N_TimetableID, "N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection, transaction);
                    }

                    if (x_TimetableCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", 1495);
                        Params.Add("N_BranchID", 0);
                        x_TimetableCode = dLayer.GetAutoNumber("Sch_Timetable", "x_TimetableCode", Params, connection, transaction);
                        if (x_TimetableCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Timetable Code");
                        }
                        Master.Rows[0]["x_TimetableCode"] = x_TimetableCode;
                    }
                    string DupCriteria = "";


                    N_TimetableID = dLayer.SaveData("Sch_Timetable", "N_TimetableID", DupCriteria, "", Master, connection, transaction);
                    if (N_TimetableID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                    for (int i = 0; i < Details.Rows.Count; i++)
                    {
                        Details.Rows[i]["N_TimetableID"] = N_TimetableID;
                        Details.Rows[i]["n_FnYearID"] = N_FnYearID;
                        Details.Rows[i]["n_CompanyID"] = N_CompanyID;
                    }
                    Details.Columns.Remove("x_TeacherName");
                    Details.Columns.Remove("x_Subject");
                    Details.Columns.Remove("n_ClassID");
                    Details.Columns.Remove("x_Class");
                    Details.Columns.Remove("n_ClassDivisionID");
                    Details.Columns.Remove("x_ClassDivision");
                    // foreach (DataRow row in Details.Rows)
                    // {
                    //     DataTable DetailsNew = row.Tables["master"];
                    // }

                    dLayer.SaveData("Sch_Timetabledetails", "N_TimetableDetailsID", Details, connection, transaction);
                    transaction.Commit();
                    SortedList Result = new SortedList();

                    return Ok(api.Success(Result, "Timetable Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int ntimetableid)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Sch_Timetable", "N_TimetableID", ntimetableid, "N_CompanyID=" + nCompanyID + "", connection);
                    dLayer.DeleteData("Sch_TimetableDetails", "N_TimetableID", ntimetableid, "N_CompanyID=" + nCompanyID + "", connection);

                }
                if (Results > 0)
                {
                    return Ok(api.Success("Timetable deleted"));
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



