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
using System.IO;
using System.Threading.Tasks;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("paycodes")]
    [ApiController]

    public class Pay_PayCodes : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 186;

        public Pay_PayCodes(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("details")]
        public ActionResult PayCodeDetails(string xPaycode, int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList OutPut = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from  vw_Pay_PayMaster where N_CompanyID=@p1 and n_FnYearID=@p2 and X_PayCode=@p3";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearID);
            Params.Add("@p3", xPaycode);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    dt = api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                    }
                    int N_PayID = myFunctions.getIntVAL(dt.Rows[0]["N_PayID"].ToString());
                    string Pay_SummaryPercentageSql = "SELECT    * From Pay_SummaryPercentage inner join Pay_PayType on Pay_SummaryPercentage.N_PayTypeID = Pay_PayType.N_PayTypeID and Pay_SummaryPercentage.N_CompanyID = Pay_PayType.N_CompanyID  Where Pay_SummaryPercentage.N_PayID =" + N_PayID + " and Pay_SummaryPercentage.N_CompanyID=" + nCompanyId;

                    DataTable SummryTable = dLayer.ExecuteDataTable(Pay_SummaryPercentageSql, Params, connection);
                    OutPut.Add("master", dt);
                    OutPut.Add("summaryList", api.Format(SummryTable));
                }
                return Ok(api.Success(OutPut));

            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }
        }


        //Save....

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable = ds.Tables["master"];
                DataTable SummaryTable = ds.Tables["paySummaryList"];

                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nPayID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PayID"].ToString());
                int nPayTypeID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PayTypeID"].ToString());
                var d_Entrydate = MasterTable.Rows[0]["d_Entrydate"].ToString();


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string PayCode = "";
                    var values = MasterTable.Rows[0]["X_PayCode"].ToString();

                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        PayCode = dLayer.GetAutoNumber("Pay_PayMaster", "X_PayCode", Params, connection, transaction);
                        if (PayCode == "") { transaction.Rollback(); return Ok(api.Error("Unable to generate Pay Code")); }
                        MasterTable.Rows[0]["X_PayCode"] = PayCode;
                    }

                    dLayer.DeleteData("Pay_SummaryPercentage", "N_PayID", nPayID, "N_CompanyID=" + nCompanyID, connection, transaction);

                    string DupCriteria = "N_companyID=" + nCompanyID + " And X_Paycode = '" + values + "' and N_FnYearID=" + nFnYearId;

                    nPayID = dLayer.SaveData("Pay_PayMaster", "N_PayID", DupCriteria, "N_companyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId, MasterTable, connection, transaction);
                    if (nPayID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        if (SummaryTable.Rows.Count > 0)
                        {
                            foreach (DataRow Rows in SummaryTable.Rows)
                            {
                                Rows["n_PayID"] = nPayID;
                                Rows["n_PerCalcID"] = 0;

                            }

                            SummaryTable.AcceptChanges();
                            int SummaryID = dLayer.SaveData("Pay_SummaryPercentage", "N_PerCalcID", SummaryTable, connection, transaction);

                            if (SummaryID <= 0)
                            {
                                transaction.Rollback();
                                return Ok(api.Error("Unable to save"));
                            }

                        }
                        else
                        {
                            DataTable dt = new DataTable();
                            dt.Clear();
                            dt.Columns.Add("n_PayID");
                            dt.Columns.Add("n_PerCalcID");
                            dt.Columns.Add("N_CompanyID");
                            dt.Columns.Add("N_PayTypeID");
                            dt.Columns.Add("D_EntryDate");

                            DataRow row = dt.NewRow();
                            row["n_PayID"] = nPayID;
                            row["n_PerCalcID"] = 0;
                            row["N_CompanyID"] = nCompanyID;
                            row["N_PayTypeID"] = 1;
                            row["D_EntryDate"] = d_Entrydate;

                            dt.Rows.Add(row);
                            int SummaryID = dLayer.SaveData("Pay_SummaryPercentage", "N_PerCalcID", dt, connection, transaction);

                            if (SummaryID <= 0)
                            {
                                transaction.Rollback();
                                return Ok(api.Error("Unable to save"));
                            }


                        }

                        transaction.Commit();
                        return Ok(api.Success("Paycode Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }


        [HttpGet("list")]
        public ActionResult GetPayCodeList(string type, int nFnyearID)
        {
            int id = 0;
            switch (type)
            {
                case "Gosi":
                    id = 14;
                    break;
                case "All":
                    id = 0;
                    break;

                default: return Ok("Invalid Type");
            }
            string X_Criteria = "";
            if (id > 0)
                X_Criteria = "where N_PayTypeID=@p1 and N_CompanyID=@nCompanyID";

            SortedList param = new SortedList() { { "@p1", id }, { "@nCompanyID", myFunctions.GetCompanyID(User) }, { "@nFnYearID", nFnyearID } };

            DataTable dt = new DataTable();

            string sqlCommandText = "select * from Pay_PayMaster " + X_Criteria;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, param, connection);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "summeryInfo", typeof(DataTable), null);
                    foreach (DataRow dRow in dt.Rows)
                    {
                        DataTable dtNode = new DataTable();
                        int N_PayID = myFunctions.getIntVAL(dRow["N_PayID"].ToString());
                        string Pay_SummaryPercentageSql = "SELECT    * From Pay_SummaryPercentage inner join Pay_PayType on Pay_SummaryPercentage.N_PayTypeID = Pay_PayType.N_PayTypeID and Pay_SummaryPercentage.N_CompanyID = Pay_PayType.N_CompanyID  Where Pay_SummaryPercentage.N_PayID =" + N_PayID + " and Pay_SummaryPercentage.N_CompanyID=" + myFunctions.GetCompanyID(User);
                        DataTable summeryInfo = dLayer.ExecuteDataTable(Pay_SummaryPercentageSql, connection);
                        dRow["summeryInfo"] = summeryInfo;

                    }
                }
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

        [HttpGet("Dashboardlist")]
        public ActionResult PayCodeDashboardList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_PayCode like '%" + xSearchkey + "%'or X_Description like '%" + xSearchkey + "%' or  X_TypeName like '%" + xSearchkey + "%' or X_PayType like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_TypeName,X_PayType desc";
            else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "x_PayCode":
                                xSortBy = "N_PayID " + xSortBy.Split(" ")[1];
                                break;
                            
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Pay_PayMaster where N_CompanyID=" + nCompanyId + " " + Searchkey + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Pay_PayMaster where N_CompanyID=" + nCompanyId + " " + Searchkey + " and N_PayID not in (select top(" + Count + ") N_PayID from vw_Pay_PayMaster where N_CompanyID=@p1 " + xSortBy + " ) " + xSortBy;


            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_Pay_PayMaster where N_CompanyID=@p1 " + Searchkey;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        //return Ok(api.Warning("No Results Found"));
                         return Ok(api.Success(OutPut));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }
        }



        [HttpGet("payCodeType")]
        public ActionResult GetPayCodeType()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            // string sqlCommandText = "Select * from Pay_PayType where N_CompanyID=@nCompanyID and N_PerPayMethod=0 or N_PerPayMethod=3 or N_PerPayMethod=30 and n_PerPayPayment=5 order by N_PayTypeID";
            string sqlCommandText = "Select * from Pay_PayType where N_CompanyID=@nCompanyID";
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

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPayCodeId)
        {

            int Results = 0;
            try
            {
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Pay_PayMaster ", "N_PayID", nPayCodeId, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("N_PayID", nPayCodeId.ToString());
                    return Ok(api.Success(res, "PayCode deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete PayCode"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }



        }
        [HttpGet("popupList")]
        public ActionResult GetpopupListx(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            // string sqlCommandText = "Select * from Pay_PayType where N_CompanyID=@nCompanyID and N_PerPayMethod=0 or N_PerPayMethod=3 or N_PerPayMethod=30 and n_PerPayPayment=5 order by N_PayTypeID";
            string sqlCommandText = "Select * from vw_PayCode where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and B_InActive = 0 and  (B_IsPrepaid=1 or isnull(B_isInvoice,0) = 1) or isnull(B_IsCategory,0)=1 and N_TypeID<>322 ";
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



        [HttpGet("calculationMethod")]
        public ActionResult GetCalculationMethod(string xPerPayMethod)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "";
            if (xPerPayMethod == null || xPerPayMethod == "")
            {
                sqlCommandText = "Select * from Pay_PayCalulationMethod where B_Active=1 order by N_SortOrder";
            }
            else
            {

                sqlCommandText = "Select * from Pay_PayCalulationMethod where B_Active=1 and N_IndexID in (" + xPerPayMethod + ") order by N_SortOrder";
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
    }

}

