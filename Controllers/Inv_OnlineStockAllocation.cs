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
    [Route("invOnlineStockAllocation")]
    [ApiController]

    public class Inv_OnlineStockAllocation : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
         private readonly IMyAttachments myAttachments;


        public Inv_OnlineStockAllocation(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf,IMyAttachments myAtt)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1563;
             myAttachments = myAtt;

        }


        [HttpGet("list")]
        public ActionResult GetInvOnlineStockAllocation(int nComapanyId, int nFnYearId, int nBranchID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, bool bAllBranchData)
        {

            int nCompanyId = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string sqlCondition = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_AsnDocNo like '%" + xSearchkey + "%' OR N_AsnID like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_AsnID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_AsnDocNo":
                        xSortBy = "X_AsnDocNo " + xSortBy.Split(" ")[1];
                        break;
                    case "N_AsnID":
                        xSortBy = "N_AsnID " + xSortBy.Split(" ")[1];
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

                    // if (bAllBranchData)
                    sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId";
                    // else
                    //     sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and N_BranchID=@nBranchID";


                    if (Count == 0)
                        sqlCommandText = "select  top(" + nSizeperpage + ") * from Inv_OnlineStore where " + sqlCondition + " " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select  top(" + nSizeperpage + ") * from Inv_OnlineStore where " + sqlCondition + " " + Searchkey + " and N_AsnID not in (select top(" + Count + ") N_AsnID from vw_Wh_AsnMaster_Disp where " + sqlCondition + " " + xSortBy + " ) " + xSortBy;


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count from Inv_OnlineStore where " + sqlCondition + " " + Searchkey + "";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
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
                     DataRow DetailRow = DetailTable.Rows[0];
                    SortedList Params = new SortedList();

                    int nStoreID = myFunctions.getIntVAL(MasterRow["N_StoreID"].ToString());
                    int nStoreDetailID = myFunctions.getIntVAL(DetailRow["N_StoreDetailID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string xStoreCode = MasterRow["x_StoreCode"].ToString();

                    string x_StoreCode = "";
                    if (xStoreCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        x_StoreCode = dLayer.GetAutoNumber("Inv_OnlineStore", "x_StoreCode", Params, connection, transaction);
                        if (x_StoreCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate bin transfer Code");
                        }
                        MasterTable.Rows[0]["x_StoreCode"] = x_StoreCode;
                    }
                    else
                    {
                         dLayer.DeleteData("Inv_OnlineStore", "N_StoreID", nStoreID, "", connection,transaction);
                          dLayer.DeleteData("Inv_OnlineStoreDetail", "N_StoreID", nStoreID, "", connection,transaction);
                    }
                    MasterTable.Columns.Remove("n_FnYearID");

                    int n_StoreID = dLayer.SaveData("Inv_OnlineStore", "N_StoreID", "", "", MasterTable, connection, transaction);
                    if (n_StoreID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save online stock ");
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_StoreID"] = n_StoreID;
                    }
                    int n_StoreDetailID = dLayer.SaveData("Inv_OnlineStoreDetail", "N_StoreDetailID", DetailTable, connection, transaction);
                    if (n_StoreDetailID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save online stock");
                    }

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_StoreID", n_StoreID);
                    Result.Add("x_StoreCode", x_StoreCode);
                    Result.Add("n_StoreDetailID", n_StoreDetailID);

                    return Ok(_api.Success(Result, "Online Stock Allocation Created"));
                }
            }
            
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

          [HttpGet("details")]
        public ActionResult EmployeeEvaluation(string xStoreCode)
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
                    Params.Add("@xStoreCode", xStoreCode);
                    Mastersql = "select * from Inv_OnlineStore where N_CompanyID=@nCompanyID and X_StoreCode=@xStoreCode ";
                   
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int nStoreID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_StoreID"].ToString());
                    Params.Add("@nStoreID", nStoreID);

                    MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from vw_Inv_OnlineStoreDetail where N_CompanyID=@nCompanyID and N_StoreID=@nStoreID ";
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

          [HttpDelete("delete")]
        public ActionResult DeleteData(int nStoreID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nStoreID", nStoreID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Inv_OnlineStore", "N_StoreID", nStoreID, "", connection);

                    if (Results > 0)
                    {
                        dLayer.DeleteData("Inv_OnlineStoreDetail", "N_StoreID", nStoreID, "", connection);
                        return Ok(_api.Success("Online Stock deleted"));
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