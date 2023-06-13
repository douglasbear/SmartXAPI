using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("projectTimesheetEntry")]
    [ApiController]

    public class Pay_ProjectTimesheetEntry : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Pay_ProjectTimesheetEntry(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 370;

        }
        [HttpGet("list")]
        public ActionResult GetAllProjectTimesheet(int nComapanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_PrjTimesheetCode like '%" + xSearchkey + "%'or cast(D_Date as VarChar) like '%" + xSearchkey + "%' or  X_ProjectName like '%" + xSearchkey + "%' or X_Name like '%" + xSearchkey + "%' or N_Hours like '%" + xSearchkey + "%' or X_Description like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by  Cast([D_Date] as DateTime ) desc";
       


            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Prj_TimeSheetMaster where N_CompanyID=@p1 and N_FnYearID=@p3 and N_UserID=@userID  " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Prj_TimeSheetMaster where N_CompanyID=@p1 and N_FnYearID=@p3 and N_UserID=@userID " + Searchkey + " and N_PrjTimeSheetID not in (select top(" + Count + ") N_PrjTimeSheetID from vw_Prj_TimeSheetMaster where N_CompanyID=@p1 and N_FnYearID=@p3 and N_UserID=@userID  " + xSearchkey + xSortBy + " ) " + xSortBy;

            Params.Add("@p1", nCompanyId);
            Params.Add("@p3", nFnYearId);
            Params.Add("@userID", myFunctions.GetUserID(User));
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count from vw_Prj_TimeSheetMaster where N_CompanyId=@p1 and N_FnYearID=@p3 and N_UserID=@userID " + Searchkey + " ";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";

                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();

                    }
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);

                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }



        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable, AdnMaster;
                    DataTable DetailTable, AdnDetails;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    AdnMaster = ds.Tables["adnMaster"];
                    AdnDetails = ds.Tables["adnDetails"];
                    SortedList Params = new SortedList();

                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int nTimeSheetID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PrjTimeSheetID"].ToString());
                    int N_UserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    string X_PrjTimesheetCode = "";
                    var values = MasterTable.Rows[0]["X_PrjTimesheetCode"].ToString();

                      transaction.Rollback();
                      return Ok(_api.Warning("Maintenance in progress. Please save later. Apologies for the inconvenience."));

                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", FormID);
                        Params.Add("N_YearID", N_FnYearID);


                        X_PrjTimesheetCode = dLayer.GetAutoNumber("Prj_TimeSheetEntryMaster", "X_PrjTimesheetCode", Params, connection, transaction);
                        if (X_PrjTimesheetCode == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Warning("Unable to generate"));
                        }
                        MasterTable.Rows[0]["X_PrjTimesheetCode"] = X_PrjTimesheetCode;

                        Params["N_FormID"] = 208;

                        // string x_Batch = dLayer.GetAutoNumber("Pay_MonthlyAddOrDed", "x_Batch", Params, connection, transaction);
                        // if (x_Batch == "")
                        // {
                        //     transaction.Rollback();
                        //     return Ok("Unable to generate Code");
                        // }
                        // AdnMaster.Rows[0]["x_Batch"] = x_Batch;
                    }
                     if (nTimeSheetID > 0)
                    {
                    dLayer.DeleteData("Prj_TimeSheetEntry", "n_PrjTimeSheetID", nTimeSheetID, "n_CompanyID=" + nCompanyID + " and n_PrjTimeSheetID=" + nTimeSheetID, connection, transaction);
                    dLayer.DeleteData("Prj_TimeSheetEntryMaster", "n_PrjTimeSheetID", nTimeSheetID, "n_CompanyID=" + nCompanyID + " and n_PrjTimeSheetID=" + nTimeSheetID, connection, transaction);
                    }

                    nTimeSheetID = dLayer.SaveData("Prj_TimeSheetEntryMaster", "N_PrjTimeSheetID", MasterTable, connection, transaction);

                    if (nTimeSheetID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }
                    SortedList deleteParams = new SortedList();
                    deleteParams.Add("@nCompanyID", nCompanyID);
                    deleteParams.Add("@nProjectID", 0);
                    deleteParams.Add("@dDate", null);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_PrjTimeSheetID"] = nTimeSheetID;
                        deleteParams["@nProjectID"] = DetailTable.Rows[j]["N_ProjectID"].ToString();
                        deleteParams["@nEmpID"] = DetailTable.Rows[j]["N_EmpID"].ToString();
                        deleteParams["@dDate"] = DetailTable.Rows[j]["D_Date"].ToString();
                        deleteParams["@nCompanyID"] = nCompanyID;
                        dLayer.ExecuteNonQuery("DELETE FROM Prj_TimeSheetEntry WHERE N_CompanyID =@nCompanyID AND N_EmpID=@nEmpID and N_ProjectID=@nProjectID and D_Date=@dDate", deleteParams, connection, transaction);

                    }
                    int N_PrjTimeSheetID = dLayer.SaveData("Prj_TimeSheetEntry", "N_TimeSheetID", DetailTable, connection, transaction);
                    if (N_PrjTimeSheetID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));

                    }

                    // int adnID = dLayer.SaveData("Pay_MonthlyAddOrDed", "n_TransID", "", "", AdnMaster, connection, transaction);
                    // if (adnID <= 0)
                    // {
                    //     transaction.Rollback();
                    //     return Ok("Unable to save ");
                    // }
                    // for (int j = 0; j < AdnDetails.Rows.Count; j++)
                    // {
                    //     AdnDetails.Rows[j]["n_TransID"] = adnID;
                    //     AdnDetails.Rows[j]["n_RefID"] = N_PrjTimeSheetID;
                    // }
                    // int adnDetailsID = dLayer.SaveData("Pay_MonthlyAddOrDedDetails", "n_TransDetailsID", "", "", AdnDetails, connection, transaction);

                    // if (adnDetailsID <= 0)
                    // {
                    //     transaction.Rollback();
                    //     return Ok("Unable to save");
                    // }

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("N_PrjTimeSheetID", N_PrjTimeSheetID);
                    Result.Add("X_PrjTimesheetCode", X_PrjTimesheetCode);
                    return Ok(_api.Success(Result, "timeSheet Saved"));
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Conversion failed when converting date and/or time from character string"))
                    return Ok(_api.Error(User, "Invalid Time Value"));
                else
                    return Ok(_api.Error(User, ex));
            }
        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nTimeSheetID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dLayer.DeleteData("prj_timesheetEntry", "N_PrjTimeSheetID", nTimeSheetID, "", connection);
                    Results = dLayer.DeleteData("prj_timesheetEntryMaster", "N_PrjTimeSheetID", nTimeSheetID, "", connection);
                    if (Results > 0)
                    {
                        return Ok(_api.Success("deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Unable to delete "));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }



        [HttpGet("details")]
        public ActionResult GetDetails(string xPrjTimesheetCode, int nFnYearID, bool bShowAllBranchData, int nProjectID, DateTime date,int nEmpID)
        {
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();
            DataTable Master = new DataTable();
            DataTable Detail = new DataTable();

            DataTable adnMaster = new DataTable();
            DataTable adnDetails = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);
            QueryParams.Add("@nCompanyID", nCompanyID);
            if (nProjectID == 0)
            {
                QueryParams.Add("@xPrjTimesheetCode", xPrjTimesheetCode);
                // QueryParams.Add("@nBranchID", nBranchID);
                QueryParams.Add("@nFnYearID", nFnYearID);
            }
            string Condition = "";
            string _sqlQuery = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    if (bShowAllBranchData == true)
                        Condition = "N_CompanyID=@nCompanyID and X_PrjTimesheetCode =@xPrjTimesheetCode and N_FnYearID=@nFnYearID";
                    else
                        Condition = "N_CompanyID=@nCompanyID and X_PrjTimesheetCode =@xPrjTimesheetCode and N_FnYearID=@nFnYearID";


                    _sqlQuery = "Select * from vw_Prj_TimeSheetMaster Where " + Condition + "";

                    if (nProjectID > 0)
                    {
                        QueryParams.Add("@nProjectID", nProjectID);
                        QueryParams.Add("@dDate", date);

                        _sqlQuery = "select Top(1) N_CompanyID,0 as N_PrjTimesheetID,'@Auto' as X_PrjTimesheetCode,N_ProjectID,X_ProjectName,D_Date,X_Description,N_FnYearID,N_BranchID from vw_Prj_TimeSheet where N_CompanyID=@nCompanyID and N_ProjectID=@nProjectID and D_Date=@dDate";
                    }
                    Master = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                    Master = _api.Format(Master, "master");
                    if (Master.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        ds.Tables.Add(Master);

                        if (nProjectID > 0)
                        {
                            _sqlQuery = "Select * from vw_Prj_TimeSheet Where N_CompanyID=@nCompanyID and N_ProjectID=@nProjectID and D_Date=@dDate and N_EmpID="+nEmpID+" ";
                        }
                        else
                        {
                            QueryParams.Add("@N_PrjTimeSheetID", Master.Rows[0]["N_PrjTimeSheetID"].ToString());
                            _sqlQuery = "Select * from vw_Prj_TimeSheet Where N_CompanyID=@nCompanyID  and N_PrjTimeSheetID=@N_PrjTimeSheetID and N_EmpID="+nEmpID+" ";
                        }
                        Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        Detail = _api.Format(Detail, "details");
                        if (Detail.Rows.Count == 0)
                        {
                            return Ok(_api.Notice("No Results Found"));
                        }
                        ds.Tables.Add(Detail);

                        _sqlQuery = "select * from Pay_MonthlyAddOrDedDetails where B_TimeSheetEntry=1 and N_refID=" + Master.Rows[0]["N_PrjTimeSheetID"].ToString() + " and N_FormID=1231 and N_PayID=38 and N_CompanyID=@nCompanyID ";
                        adnDetails = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        adnDetails = _api.Format(adnDetails, "adnDetails");
                        ds.Tables.Add(adnDetails);

                        if (adnDetails.Rows.Count > 0)
                        {
                            _sqlQuery = "select * from Pay_MonthlyAddOrDed where N_CompanyID=@nCompanyID and N_TransID=" + adnDetails.Rows[0]["N_TransID"].ToString();
                            adnMaster = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);
                            adnMaster = _api.Format(adnMaster, "adnMaster");
                        }
                        ds.Tables.Add(adnMaster);


                        return Ok(_api.Success(ds));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }



    }
}




