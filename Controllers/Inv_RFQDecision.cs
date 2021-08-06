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
    [Route("rfqDecision")]
    [ApiController]
    public class Inv_RFQDecision : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID = 955;

        public Inv_RFQDecision(IApiFunctions apiFun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apiFun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID=955;
        }



        [HttpGet("list")]
        public ActionResult GetList(int nCompanyId, int nFnYearId, int nBranchID, bool bAllBranchData, int FormID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string sqlCondition = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_RFQDecisionCode like '%" + xSearchkey + "%' OR X_QuotationNo like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_RFQDecisionID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_RFQDecisionCode":
                        xSortBy = "X_RFQDecisionCode " + xSortBy.Split(" ")[1];
                        break;
                    case "N_RFQDecisionID":
                        xSortBy = "N_RFQDecisionID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nFnYearId", nFnYearId);
            Params.Add("@FormID", FormID);
            Params.Add("@nBranchID", nBranchID);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (bAllBranchData)
                        sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId";
                    else
                        sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and N_BranchID=@nBranchID";

                    if (Count == 0)
                        sqlCommandText = "select  top(" + nSizeperpage + ") * from vw_RFQDecisionMaster where " + sqlCondition + " " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select  top(" + nSizeperpage + ") * from vw_RFQDecisionMaster where " + sqlCondition + " " + Searchkey + " and N_RFQDecisionID not in (select top(" + Count + ") N_RFQDecisionID from vw_RFQDecisionMaster where " + sqlCondition + " " + xSortBy + " ) " + xSortBy;


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count from vw_RFQDecisionMaster where " + sqlCondition + " " + Searchkey + "";
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
                return Ok(api.Error(e));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nRFQDecisionID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    dLayer.DeleteData("Inv_RFQDecisionDetails", "N_PRSID", nRFQDecisionID, "N_CompanyID=" + nCompanyID + " and N_RFQDecisionID=" + nRFQDecisionID, connection, transaction);
                    Results = dLayer.DeleteData("Inv_RFQDecisionMaster", "N_RFQDecisionID", nRFQDecisionID, "N_CompanyID=" + nCompanyID + " and N_RFQDecisionID=" + nRFQDecisionID, connection, transaction);

                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("RFQ Decision deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(api.Warning("Unable to delete Request"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }


        }

        //Save....
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {

            DataTable MasterTable;
            DataTable DetailTable;
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            SortedList Params = new SortedList();
            // Auto Gen
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    string X_RFQDecisionCode = "";
                    int N_RFQDecisionID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_RFQDecisionID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int N_UserID = myFunctions.GetUserID(User);

                    var values = MasterTable.Rows[0]["X_RFQDecisionCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", FormID);
                        X_RFQDecisionCode = dLayer.GetAutoNumber("Inv_RFQDecisionMaster", "X_RFQDecisionCode", Params, connection, transaction);
                        if (X_RFQDecisionCode == "") { transaction.Rollback(); return Ok(api.Warning("Unable to generate Request Decision Number")); }
                        MasterTable.Rows[0]["X_RFQDecisionCode"] = X_RFQDecisionCode;
                    }

                    if (N_RFQDecisionID > 0)
                    {
                        dLayer.DeleteData("Inv_RFQDecisionDetails", "N_RFQDecisionID", N_RFQDecisionID, "N_CompanyID=" + N_CompanyID + " and N_RFQDecisionID=" + N_RFQDecisionID, connection, transaction);
                        dLayer.DeleteData("Inv_RFQDecisionMaster", "N_RFQDecisionID", N_RFQDecisionID, "N_CompanyID=" + N_CompanyID + " and N_RFQDecisionID=" + N_RFQDecisionID, connection, transaction);
                    }

                    N_RFQDecisionID = dLayer.SaveData("Inv_RFQDecisionMaster", "N_RFQDecisionID", MasterTable, connection, transaction);
                    if (N_RFQDecisionID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_RFQDecisionID"] = N_RFQDecisionID;
                    }
                    int N_RFQDecisionDetailsID = dLayer.SaveData("Inv_RFQDecisionDetails", "N_RFQDecisionDetailsID", DetailTable, connection, transaction);

                    if (N_RFQDecisionDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }

                    transaction.Commit();

                    SortedList Result = new SortedList();
                    Result.Add("N_RFQDecisionID", N_RFQDecisionID);
                    Result.Add("X_RFQDecisionCode", X_RFQDecisionCode);
                    return Ok(api.Success(Result, "Request Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }


        [HttpGet("details")]
        public ActionResult GetDetails(string xRFQDecisionCode, int nFnYearID, int nBranchID, bool bShowAllBranchData)
        {
            DataTable Master = new DataTable();
            DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@xRFQDecisionCode", xRFQDecisionCode);
            QueryParams.Add("@nBranchID", nBranchID);
            QueryParams.Add("@nFnYearID", nFnYearID);
            string Condition = "";
            string _sqlQuery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (bShowAllBranchData == true)
                        Condition = "n_Companyid=@nCompanyID and X_RFQDecisionCode =@xRFQDecisionCode and N_FnYearID=@nFnYearID";
                    else
                        Condition = "n_Companyid=@nCompanyID and X_RFQDecisionCode =@xRFQDecisionCode and N_FnYearID=@nFnYearID and N_BranchID=@nBranchID";


                    _sqlQuery = "Select * from vw_RFQDecisionMaster Where " + Condition + "";

                    Master = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                    Master = api.Format(Master, "master");

                    if (Master.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        QueryParams.Add("@N_RFQDecisionID", Master.Rows[0]["N_RFQDecisionID"].ToString());

                        ds.Tables.Add(Master);

                        _sqlQuery = "Select * from vw_RFQDecisionDetails Where N_CompanyID=@nCompanyID and N_RFQDecisionID=@N_RFQDecisionID";
                        Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        Detail = api.Format(Detail, "details");
                        if (Detail.Rows.Count == 0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }
                        ds.Tables.Add(Detail);

                        return Ok(api.Success(ds));
                    }


                }


            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpGet("VendorList")]
        public ActionResult GetVendorList(int nQuotationDetailsID,int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nQuotationDetailsID", nQuotationDetailsID);
            Params.Add("@nFnYearID", nFnYearID);

            string sqlCommandText = "";

            sqlCommandText = "Select * from vw_RFQVendorListDetails where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_QuotationDetailsID=@nQuotationDetailsID";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
                return Ok(api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }


    }
}