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
    [Route("assetmaster")]
    [ApiController]
    public class Ass_ItemMaster : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 452;

        public Ass_ItemMaster(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)

        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            // FormID = 188;
        }


        [HttpGet("list")]
        public ActionResult ItemMasterList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (AssetLedgerID like '%" + xSearchkey + "%' or X_Category like '%" + xSearchkey + "%' or X_ItemName like '%" + xSearchkey + "%' or D_PurchaseDate like '%" + xSearchkey + "%' or N_LifePeriod like '%" + xSearchkey + "%' or N_BookValue like '%" + xSearchkey + "%' or X_PlateNumber like '%" + xSearchkey + "%' or X_SerialNo like '%" + xSearchkey + "%' or X_BranchName like '%" + xSearchkey + "%' or X_EmpCode like '%" + xSearchkey + "%' or X_EmpName like '%" + xSearchkey + "%' or X_Department like '%" + xSearchkey + "%' or Status like '%" + xSearchkey + "%' or X_MainCategory like '%" + xSearchkey + "%' or X_make like '%" + xSearchkey + "%' or N_Price like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by AssetLedgerID desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_AssetDashBoard where N_CompanyID=@p1 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_AssetDashBoard where N_CompanyID=@p1 " + Searchkey + " and N_ItemID not in (select top(" + Count + ") N_ItemID from vw_AssetDashBoard where N_CompanyID=@p1 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_AssetDashBoard where N_CompanyID=@p1 ";
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
                return Ok(api.Error(User, e));
            }
        }


        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                DataTable ExpiryTable;
                ExpiryTable = ds.Tables["expiry"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                string xItemCode = MasterTable.Rows[0]["X_ItemCode"].ToString();
                int nAddlInfoID = myFunctions.getIntVAL(ExpiryTable.Rows[0]["N_AddlInfoID"].ToString());
                int nItemID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ItemID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    String DupCriteria = "X_ItemCode= '" + xItemCode + "' and N_CompanyID=" + nCompanyID;
                    nItemID = dLayer.SaveData("Ass_AssetMaster", "N_ItemID", DupCriteria, "", MasterTable, connection, transaction);
                    if (nItemID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                    if (nItemID > 0)
                    {
                        dLayer.DeleteData("Ass_AssetAddlInfo", "N_ItemID", nItemID, "N_CompanyID=" + nCompanyID, connection, transaction);
                    }

                    nAddlInfoID = dLayer.SaveData("Ass_AssetAddlInfo", "n_AddlInfoID", ExpiryTable, connection, transaction);

                    if (nAddlInfoID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success(" Saved Successfully"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nItemID)
        {

            int Results = 0;
            try
            {
                SortedList Params = new SortedList();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Ass_AssetAddlInfo", "N_ItemID", nItemID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("N_ItemID", nItemID.ToString());
                    return Ok(api.Success(res, "Item deleted"));
                }
                else
                {
                    return Ok(api.Error(User, "Unable to delete Lead"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }

        [HttpGet("details")]
        public ActionResult ItemMasterListDetails(string xItemCode, int nBranchID, bool bAllBranchData, int nFnYearID)
        {
            DataSet dt = new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable ExpiryTable = new DataTable();
            DataTable HistoryTable = new DataTable();
            DataTable DeprTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nItemID = 0;
            string sqlCommandText = "";
            string ExpirysqlCommand = "";
            string HistorysqlCommand = "";
            string DeprsqlCommand = "";

            string Condn = "";
            if (bAllBranchData)
                Condn = "X_ItemCode=@p2 and N_CompanyID=@p1 and N_FnYearID=@p5";
            else
                Condn = "X_ItemCode=@p2 and N_CompanyID=@p1 and N_BranchID=@p3 and N_FnYearID=@p5";

            sqlCommandText = "select * from vw_AssetMaster where " + Condn + "";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", xItemCode);
            Params.Add("@p3", nBranchID);
            Params.Add("@p5", nFnYearID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                       MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "d_NextMaintanance", typeof(string), 0);


                 
                    if (MasterTable.Rows.Count == 0) { return Ok(api.Warning("No data found")); }
                    nItemID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_itemID"].ToString());

                    HistorysqlCommand = "Select D_StartDate, D_EndDate ,X_Description , X_RefNo ,N_Amount,X_Branch,N_BookValue,X_CostcentreName,X_ProjectName,ProjectPeriod,N_TypeOrder,X_EmpName,Dep_Amount from vw_Ass_ItemHistory where N_ItemID=@p4 and N_CompanyID=@p1 order by D_EndDate,N_TypeOrder";
                    Params.Add("@p4", nItemID);
                    HistoryTable = dLayer.ExecuteDataTable(HistorysqlCommand, Params, connection);
                    int i = -1;

                    foreach (DataRow Avar in HistoryTable.Rows)
                    {
                        i = i + 1;
                    }
                    DateTime today = Convert.ToDateTime(HistoryTable.Rows[i]["D_EndDate"]);
                    if (today.Month == 12)
                    {
                        //DateTime endOfMonth = new DateTime(today.Year + 1, 1, DateTime.DaysInMonth(today.Year + 1, 1));
                        DateTime endOfMonth = new DateTime( Convert.ToDateTime(MasterTable.Rows[0]["d_PurchaseDate"]).Year,  Convert.ToDateTime(MasterTable.Rows[0]["d_PurchaseDate"]).Month, DateTime.DaysInMonth( Convert.ToDateTime(MasterTable.Rows[0]["d_PurchaseDate"]).Year,  Convert.ToDateTime(MasterTable.Rows[0]["d_PurchaseDate"]).Month));
                       
                        MasterTable.Rows[0]["d_NextMaintanance"]= endOfMonth.ToShortDateString();

                    }
                    else
                    {
                        //DateTime endOfMonth = new DateTime(today.Year, today.Month + 1, DateTime.DaysInMonth(today.Year, today.Month + 1));
                        DateTime endOfMonth = new DateTime( Convert.ToDateTime(MasterTable.Rows[0]["d_PurchaseDate"]).Year,  Convert.ToDateTime(MasterTable.Rows[0]["d_PurchaseDate"]).Month, DateTime.DaysInMonth( Convert.ToDateTime(MasterTable.Rows[0]["d_PurchaseDate"]).Year, Convert.ToDateTime(MasterTable.Rows[0]["d_PurchaseDate"]).Month));
                        MasterTable.Rows[0]["d_NextMaintanance"]= endOfMonth.ToShortDateString();

                    }
                       MasterTable = api.Format(MasterTable, "Master");

                    HistoryTable = api.Format(HistoryTable, "History");

                    ExpirysqlCommand = "Select * From vw_AssetAddlInfo Where N_CompanyID=@p1 and N_ItemID=@p4";
                    ExpiryTable = dLayer.ExecuteDataTable(ExpirysqlCommand, Params, connection);

                    ExpiryTable = api.Format(ExpiryTable, "Expiry");

                    DeprsqlCommand = "Select Max(D_EndDate) as LastRunDate,COUNT(D_RunDate) as DepreciationCount,Sum(DATEDIFF(D,D_StartDate ,D_EndDate )) as DepreciationDays from Ass_Depreciation where N_ItemID=@p4 and N_CompanyID=@p1";
                    DeprTable = dLayer.ExecuteDataTable(DeprsqlCommand, Params, connection);

                    DeprTable = api.Format(DeprTable, "Depreciation");
                }

                if (MasterTable.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(HistoryTable);
                    dt.Tables.Add(ExpiryTable);
                    dt.Tables.Add(DeprTable);
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
        [HttpGet("assetList")]
        public ActionResult GetassetList()

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);

            string sqlCommandText = "Select *  from vw_AssetMaster Where N_CompanyID= " + nCompanyID + "";


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


        [HttpGet("defaults")]
        public ActionResult ExpiryDefaults()
        {
            DataTable dt = new DataTable();
            int nCompanyId = myFunctions.GetCompanyID(User);
            SortedList Params = new SortedList();
            string sqlCommandText = "";

            sqlCommandText = "Select * From Gen_Defaults where N_DefaultId= 51";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = api.Format(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    // dt.Tables.Add(dt); 
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("costCentreList")]
        public ActionResult ListcostCentre(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            string sqlCommandText = "Select * from Acc_CostCentreMaster Where N_CompanyID=@nCompanyID and N_FnyearID=@nFnYearID";
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


    }
}

