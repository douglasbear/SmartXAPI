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
     [Route("schExamTimeTable")]
     [ApiController]
    public class SchExamTimeTable : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
         private readonly IMyFunctions myFunctions;
         private readonly IApiFunctions _api;
          private readonly int N_FormID = 1502;
           public SchExamTimeTable(IDataAccessLayer dl,IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            _api = api;
            myFunctions=myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
           
        [HttpGet("dashboardList")]
        public ActionResult GetExamTimeTableDashboardList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_ExamTimeCode like '%" + xSearchkey + "%' or X_Name like '%" + xSearchkey + "%' or X_ClassDivision like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_ExamTimeCode desc";

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Sch_ExamTimeMaster where N_CompanyID=@nCompanyID  " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Sch_ExamTimeMaster where N_CompanyID=@nCompanyID  " + Searchkey + " and N_ExamTimeMasterID not in (select top(" + Count + ") N_ExamTimeMasterID from vw_Sch_ExamTimeMaster where N_CompanyID=@nCompanyID " + xSortBy + " ) " + " " + xSortBy;

            Params.Add("@nCompanyID", nCompanyID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(1) as N_Count  from vw_Sch_ExamTimeMaster where N_CompanyID=@nCompanyID " + Searchkey + "";
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

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();

                    int nExamTimeMasterID = myFunctions.getIntVAL(MasterRow["n_ExamTimeMasterID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string xExamTimeCode = MasterRow["X_ExamTimeCode"].ToString();

                    if (xExamTimeCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", N_FormID);
                        xExamTimeCode = dLayer.GetAutoNumber("Sch_ExamTimeMaster", "X_ExamTimeCode", Params, connection, transaction);
                        if (xExamTimeCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Employee Evaluation");
                        }
                        MasterTable.Rows[0]["X_ExamTimeCode"] = xExamTimeCode;
                    }
                    MasterTable.Columns.Remove("n_FnYearID");

                    nExamTimeMasterID = dLayer.SaveData("Sch_ExamTimeMaster", "N_ExamTimeMasterID", "", "", MasterTable, connection, transaction);
                    if (nExamTimeMasterID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Exam TimeTable");
                    }
                    dLayer.DeleteData("Sch_ExamTimeTable", "N_ExamTimeMasterID", nExamTimeMasterID, "", connection, transaction);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_ExamTimeMasterID"] = nExamTimeMasterID;
                    }
                    int nExamTimeID = dLayer.SaveData("Sch_ExamTimeTable", "N_ExamTimeID", DetailTable, connection, transaction);
                    if (nExamTimeID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Exam TimeTable");
                    }
                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_ExamTimeMasterID", nExamTimeMasterID);
                    Result.Add("x_ExamTimeCode", xExamTimeCode);
                    Result.Add("n_ExamTimeID", nExamTimeID);

                    return Ok(_api.Success(Result, "Exam TimeTable Created"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("details")]
        public ActionResult ExamTimeTableDetails(string xExamTimeCode)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable DataTable = new DataTable();

                    string Mastersql = "";
                    string DetailSql = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@xExamTimeCode", xExamTimeCode);
                    Mastersql = "select * from vw_Sch_ExamTimeMaster where N_CompanyID=@nCompanyID and X_ExamTimeCode=@xExamTimeCode  ";
                   
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int nExamTimeMasterID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ExamTimeMasterID"].ToString());
                    Params.Add("@nExamTimeMasterID", nExamTimeMasterID);

                    MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from vw_Sch_ExamTimeTable where N_CompanyID=@nCompanyID and N_ExamTimeMasterID=@nExamTimeMasterID ";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
            
        [HttpGet("list")]
        public ActionResult GetExamTimeTableList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select * from vw_Sch_ExamTimeMaster where N_CompanyID=@nComapnyID";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nExamTimeMasterID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nExamTimeMasterID", nExamTimeMasterID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Sch_ExamTimeMaster", "N_ExamTimeMasterID", nExamTimeMasterID, "", connection);


                    if (Results > 0)
                    {
                        dLayer.DeleteData("Sch_ExamTimeTable", "N_ExamTimeMasterID", nExamTimeMasterID, "", connection);
                        return Ok(_api.Success("Exam TimeTable deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete"));
                    }

                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

    }
}
    