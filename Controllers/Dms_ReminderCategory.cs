using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("remCategory")]
    [ApiController]
    public class Dms_ReminderCategory : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID = 955;
        public Dms_ReminderCategory(IApiFunctions apiFun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apiFun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 955;
        }



        [HttpGet("list")]
        public ActionResult GetRemCategoryList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "",sqlCommandCount="";
            int nCompanyId = myFunctions.GetCompanyID(User);

            Params.Add("@nCompanyId", nCompanyId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    sqlCommandText = "select * from Dms_ReminderCategory where N_CompanyID=@nCompanyId";
                             
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count from Dms_ReminderCategory where N_CompanyID=@nCompanyId";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                    }
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                }
                // dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(OutPut));
                }
            }
            catch (Exception e)
            {
                return StatusCode(403, api.Error(e));
            }
        }

           
        [HttpGet("recepient")]
        public ActionResult GetRecepient(int nMediaTypeID, int nFnYearID, bool bAllBranchData, int nBranchID)

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nBranchID", nBranchID);

            string sqlCommandText = "";
            if (nMediaTypeID != 155)//--EMail or SMS--
            {
                if (bAllBranchData == true)
                    sqlCommandText = "Select Name,X_Phone1,X_EmailID from vw_EmpReminderRecepient where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID order by n_Sort";
                else
                    sqlCommandText = "Select Name,X_Phone1,X_EmailID from vw_EmpReminderRecepient where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and (N_BranchID=0 OR N_BranchID=@nBranchID) order by n_Sort";
            }
            else//--Notification--
            {
                sqlCommandText = "Select X_UserID,X_UserName,X_UserCategory  from vw_PayEmployeeUser N_CompanyID=@nCompanyID and X_UserCategory<>'Olivo'";             
            }

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
        //Save....
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {

            DataTable MasterTable;
            DataTable DetailTable;
            DataTable RecepientTable;
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            RecepientTable = ds.Tables["recepient"];

            SortedList Params = new SortedList();
            // Auto Gen
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    string X_CategoryCode = "";
                    int N_CategoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CategoryID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int N_UserID = myFunctions.GetUserID(User);

                    var values = MasterTable.Rows[0]["X_CategoryCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID",N_CompanyID);
                        Params.Add("N_YearID",N_FnYearID);
                        Params.Add("N_FormID", FormID);
                        X_CategoryCode = dLayer.GetAutoNumber("Dms_ReminderCategory", "X_CategoryCode", Params, connection, transaction);
                        if (X_CategoryCode == "") { transaction.Rollback(); return Ok(api.Warning("Unable to generate Request Number")); }
                        MasterTable.Rows[0]["X_CategoryCode"] = X_CategoryCode;
                    }

                    if (MasterTable.Columns.Contains("N_FnYearID"))
                            MasterTable.Columns.Remove("N_FnYearID");
                        MasterTable.AcceptChanges();

                    if (N_CategoryID > 0)
                    {
                       dLayer.DeleteData("Dms_RemRecepientList", "N_CategoryID", N_CategoryID, "N_CompanyID=" + N_CompanyID + " and N_CategoryID=" + N_CategoryID, connection, transaction);
                       dLayer.DeleteData("Dms_ReminderCategoryDetails", "N_CategoryID", N_CategoryID, "N_CompanyID=" + N_CompanyID + " and N_CategoryID=" + N_CategoryID, connection, transaction);
                       dLayer.DeleteData("Dms_ReminderCategory", "N_CategoryID", N_CategoryID, "N_CompanyID=" + N_CompanyID + " and N_CategoryID=" + N_CategoryID, connection, transaction);
                    }

                    N_CategoryID = dLayer.SaveData("Dms_ReminderCategory", "N_CategoryID", MasterTable, connection, transaction);
                    if (N_CategoryID <= 0)
                    {
                        transaction.Rollback();
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        int N_CategoryDetailsID=0;
                        DetailTable.Rows[j]["N_CategoryID"] = N_CategoryID;
                        N_CategoryDetailsID = dLayer.SaveDataWithIndex("Dms_ReminderCategoryDetails", "N_CategoryDetailsID","","",j, DetailTable, connection, transaction);

                        if (RecepientTable.Rows.Count > 0)
                        {
                            for (int k = 0; k < RecepientTable.Rows.Count; k++)
                            {
                                if(myFunctions.getIntVAL(RecepientTable.Rows[k]["n_RowID"].ToString())==j)
                                {
                                    RecepientTable.Rows[k]["N_CategoryID"] = N_CategoryID;
                                    RecepientTable.Rows[k]["N_CategoryDetailsID"] = N_CategoryDetailsID;
                                }
                            }
                        }
                    }
                    if (RecepientTable.Rows.Count > 0)
                    {
                        if (RecepientTable.Columns.Contains("n_RowID"))
                            RecepientTable.Columns.Remove("n_RowID");
                        RecepientTable.AcceptChanges();

                        int N_RecepientListID =  dLayer.SaveData("Dms_RemRecepientList", "N_RecepientListID", RecepientTable, connection, transaction);
                    }
                    transaction.Commit();

                    SortedList Result = new SortedList();
                    Result.Add("N_CategoryID", N_CategoryID);
                    Result.Add("X_CategoryCode", X_CategoryCode);
                    return Ok(api.Success(Result, "Category Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }
        
        [HttpGet("details")]
        public ActionResult GetCategoryDetails(string  X_CategoryCode)
        {
            DataTable Master = new DataTable();
            DataTable Detail = new DataTable();
            DataTable RecepientTable = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@X_CategoryCode", X_CategoryCode);
            string _sqlQuery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    _sqlQuery = "Select * from vw_DmsReminderCategory Where N_CompanyID=@nCompanyID and X_CategoryCode=@X_CategoryCode";

                    Master = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                    Master = api.Format(Master, "master");

                    if (Master.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        QueryParams.Add("@N_CategoryID", Master.Rows[0]["N_CategoryID"].ToString());

                        ds.Tables.Add(Master);

                        _sqlQuery = "Select * from vw_DmsReminderCategoryDetails Where N_CompanyID=@nCompanyID and N_CategoryID=@N_CategoryID";
                        Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        Detail = api.Format(Detail, "details");
                        if (Detail.Rows.Count == 0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }
                        ds.Tables.Add(Detail);

                        _sqlQuery = "Select * from Dms_RemRecepientList Where N_CompanyID=@nCompanyID and N_CategoryID=@N_CategoryID";
                        RecepientTable = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        RecepientTable = api.Format(RecepientTable, "recepient");
                        ds.Tables.Add(RecepientTable);

                        return Ok(api.Success(ds));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int N_CategoryID)
        {
            int Results = 0;
            int companyid = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                        connection.Open();

                    if (N_CategoryID > 0)
                    {
                        dLayer.DeleteData("Dms_RemRecepientList", "N_CategoryID", N_CategoryID, "N_CompanyID=" + companyid + " and N_CategoryID=" + N_CategoryID, connection);
                        dLayer.DeleteData("Dms_ReminderCategoryDetails", "N_CategoryID", N_CategoryID, "N_CompanyID=" + companyid + " and N_CategoryID=" + N_CategoryID, connection);
                    Results= dLayer.DeleteData("Dms_ReminderCategory", "N_CategoryID", N_CategoryID, "N_CompanyID=" + companyid + " and N_CategoryID=" + N_CategoryID, connection);
                    
                        if (Results > 0)
                        {
                            return Ok(api.Success( "Reminder Category deleted"));
                        }
                        else
                        {
                            return Ok(api.Warning("Unable to delete Reminder Category"));
                        }
                    }
                    else
                    {
                        return Ok(api.Warning("Unable to delete Reminder Category"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }


        }
        

    }
}