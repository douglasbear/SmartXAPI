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
        public ActionResult GetInvOnlineStockAllocation(int? nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    DataTable CountTable = new DataTable();
                    SortedList Params = new SortedList();
                    DataSet dataSet = new DataSet();
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";

                    int nUserID = myFunctions.GetUserID(User);
                      if (xSearchkey != null && xSearchkey.Trim() != "")
                      Searchkey = "and (X_StoreCode like'%" + xSearchkey + "%'or X_StoreName like'%" + xSearchkey + "%')";

                       if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_StoreID desc";
                       else
                        xSortBy = " order by " + xSortBy;

                    
                    int Count = (nPage - 1) * nSizeperpage;
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") [X_StoreCode] AS X_StoreCode,* from Inv_OnlineStore where N_CompanyID=@p1" + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") [X_StoreCode] AS X_StoreCode,* from Inv_OnlineStore where N_CompanyID=@p1" + Searchkey + " and N_StoreID not in (select top(" + Count + ") N_StoreID from Inv_OnlineStore where N_CompanyID=@p1" + xSortBy + " ) " + xSortBy;

                    // sqlCommandText = "select * from Inv_MRNDetails where N_CompanyID=@p1";
                    Params.Add("@p1", nCompanyId);
                  //  Params.Add("@p2", nFnYearId);
                    SortedList OutPut = new SortedList();


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count from Inv_OnlineStore where N_CompanyID=@p1 " + Searchkey + "";
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
        public ActionResult GetDetails(string xStoreCode,int nCompanyID, int nBranchID, bool bShowAllBranchData)
        {
            DataTable Master = new DataTable();
            DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();
            DataTable Attachments = new DataTable();
            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", nCompanyID);

            QueryParams.Add("@nBranchID", nBranchID);
           // QueryParams.Add("@nFnYearID", nFnYearID);
            string Condition = "";
            string _sqlQuery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    if (xStoreCode != "" && xStoreCode != null)
                    {
                        QueryParams.Add("@xStoreCode", xStoreCode);
                        _sqlQuery = "Select * from Inv_OnlineStore Where n_Companyid=@nCompanyID and X_StoreCode =@xStoreCode";
                    }
                    else
                    {
                        QueryParams.Add("@xStoreCode", xStoreCode);
                        if (bShowAllBranchData == true)
                            Condition = " n_Companyid=@nCompanyID and X_StoreCode =@xStoreCode";
                        else
                            Condition = " n_Companyid=@nCompanyID and X_StoreCode =@xStoreCode and N_BranchID=@nBranchID";

                        _sqlQuery = "Select * from Inv_OnlineStore Where " + Condition + "";
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
                        if (xStoreCode != null)
                        {
                            QueryParams.Add("@nStoreID", Master.Rows[0]["N_StoreID"].ToString());

                            _sqlQuery = "Select * from vw_Inv_OnlineStoreDetail Where N_CompanyID=@nCompanyID and N_StoreID=@nStoreID";

                        }
                        else
                        {
                            QueryParams.Add("@N_StoreID", Master.Rows[0]["N_StoreID"].ToString());

                            _sqlQuery = "Select * from vw_Inv_OnlineStoreDetail Where N_CompanyID=@nCompanyID and N_StoreID=@nStoreID";
                        }

                        Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        Detail = _api.Format(Detail, "details");
                        if (Detail.Rows.Count == 0)
                        {
                            return Ok(_api.Notice("No Results Found"));
                        }
                        ds.Tables.Add(Detail);


                        return Ok(_api.Success(ds));
                    }


                }


            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
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