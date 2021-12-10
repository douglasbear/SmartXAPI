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

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("InvtransferReceive")]
    [ApiController]
    public class Inv_TransferReceive : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;

        public Inv_TransferReceive(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1056;
        }
        private readonly string connectionString;

        [HttpGet("list")]
        public ActionResult TransferReceiveList(int nCompanyId, int nFnYearID, bool bAllBranchData, int nBranchID, int nLocationID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //int nCompanyId = myFunctions.GetCompanyID(User);
                    int nUserID = myFunctions.GetUserID(User);
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    string sqlCommandCount = "";
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string Criteria = "";
                    string Searchkey = "";
                    string X_TransType = "STOCK RECEIVE";
                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearID);
                    Params.Add("@p3", nBranchID);
                    Params.Add("@p4", nLocationID);
                 

                    bool CheckClosedYear = Convert.ToBoolean(dLayer.ExecuteScalar("Select B_YearEndProcess From Acc_FnYear Where N_CompanyID=@p1 and N_FnYearID=@p2 ", Params, connection));

                    if (!CheckClosedYear)
                    {
                        if (bAllBranchData)
                            Criteria = "and N_FnYearID=@p2 and B_YearEndProcess=0 and N_CompanyID=@p1 ";
                        else
                            Criteria = "and N_FnYearID=@p2 and  B_YearEndProcess=0 and N_BranchID=@p3 and N_CompanyID=@p1 and N_LocationID=@p4";
                    }
                    else
                    {
                        if (bAllBranchData)
                            Criteria = "and N_PurchaseType=0 and N_FnYearID=@p2 and N_CompanyID=@p1";
                        else
                            Criteria = "and N_PurchaseType=0 and N_FnYearID=@p2 and N_LocationID=@p4 and N_BranchID=@p3 and N_CompanyID=@p1";
                    }


                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and [Site from] like '%" + xSearchkey + "%'";

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_ReceivableId asc";
                    else
                        xSortBy = " order by " + xSortBy;

                    if (Count == 0)
                    {

                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvReceivableStock_Search where N_CompanyID=@nCompanyId and N_FnYearID=" + nFnYearID + "   " + Searchkey + Criteria + xSortBy;

                    }
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvReceivableStock_Search where N_CompanyID=@nCompanyId " + Searchkey + Criteria + " and N_ReceivableId not in (select top(" + Count + ") N_ReceivableId from vw_InvReceivableStock_Search where N_CompanyID=@nCompanyId " + Criteria + xSortBy + " ) " + xSortBy;
                    Params.Add("@nCompanyId", nCompanyId);

                    SortedList OutPut = new SortedList();



                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_InvReceivableStock_Search where N_CompanyID=@nCompanyId " + Searchkey + Criteria;
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


        [HttpGet("transferList")]
        public ActionResult GettransferList( int nFnYearID )
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearID);
            string sqlCommandText = "select [reference No] as x_reference,* from vw_InvTransfer_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_Processed=0";
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
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nReceivableId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ReceivableId"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());
                    int N_TransferId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TransferId"].ToString());
                    int N_LocationID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LocationID"].ToString());
                    int N_LocationIDFrom = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LocationIDfrom"].ToString());
                    MasterTable.Columns.Remove("N_LocationIDfrom");
                
                    string X_ReferenceNo = MasterTable.Rows[0]["X_ReferenceNo"].ToString();
                    string X_TransType = "STOCK RECEIVE";

                    // int nUsercategoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserCategoryID"].ToString());
                    // int nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());
                    // int nLevelID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_Level"].ToString());
                    // int nActionID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ActionTypeID"].ToString());
                    if (nReceivableId > 0)
                    {
                        SortedList DelParam = new SortedList();
                        DelParam.Add("N_CompanyID", nCompanyID);
                        DelParam.Add("X_TransType", X_TransType);
                        DelParam.Add("N_VoucherID", nReceivableId);
                        // DelParam.Add("a", 0);
                        // DelParam.Add("b", "");
                        // DelParam.Add("c", 0);
                        dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", DelParam, connection, transaction);
                        // dba.ExecuteNonQuery("SP_Delete_Trans_With_PurchaseAccounts " + nCompanyID + ",'" + X_TransType + "'," +nReceivableId + ",0,'',0", DelParam, connection, transaction);
                    }
                    DocNo = MasterRow["X_ReferenceNo"].ToString();
                    // int nSavedraft = myFunctions.getIntVAL(MasterTable.Rows[0]["N_SaveDraft"].ToString());
                    if (X_ReferenceNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("N_YearID", nFnYearID);

                        while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_ReceivableStock Where X_ReferenceNo ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_ReferenceNo = DocNo;


                        if (X_ReferenceNo == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate")); }
                        MasterTable.Rows[0]["X_ReferenceNo"] = X_ReferenceNo;

                    }
                    nReceivableId = dLayer.SaveData("Inv_ReceivableStock", "N_ReceivableId", MasterTable, connection, transaction);
                    if (nReceivableId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }

                    for (int i = 0; i < DetailTable.Rows.Count; i++)
                    {
                        DetailTable.Rows[i]["N_ReceivableId"] = nReceivableId;
                    }
                    int N_ReceivableDetailsID = dLayer.SaveData("Inv_ReceivableStockDetails", "N_ReceivableDetailsID", DetailTable, connection, transaction);
                    if (N_ReceivableDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }
                    else
                    {
                        dLayer.ExecuteNonQuery("Update Inv_TransferStock Set N_Processed=1  Where N_TransferId=" + N_TransferId + " and N_CompanyID=" + nCompanyID,Params,connection,transaction);

                        SortedList StockParam = new SortedList();
                        StockParam.Add("N_CompanyID", nCompanyID);
                        StockParam.Add("@N_ReceiveId", nReceivableId);
                         StockParam.Add("@N_WarehouseIdFrom", N_LocationIDFrom);
                          StockParam.Add("@N_WarehouseIdTo", N_LocationID);
                        StockParam.Add("N_UserID", nUserID);
                        StockParam.Add("X_SystemName", "");

                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Inv_ReceivableStock ", StockParam, connection, transaction).ToString();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }

                        SortedList PostingParam = new SortedList();
                        PostingParam.Add("N_CompanyID", nCompanyID);
                        PostingParam.Add("X_InventoryMode", X_TransType);
                        PostingParam.Add("N_InternalID", nReceivableId);
                        PostingParam.Add("N_UserID", nUserID);
                        PostingParam.Add("X_SystemName", "WebRequest");
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Acc_InventoryPosting ", PostingParam, connection, transaction).ToString();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }






                    }



                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }


               [HttpGet("listDetails")]
        public ActionResult viewDetails(string xRefNo, int nFnYearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    SortedList Params = new SortedList();
                    Params.Add("@xReceiptNo", xRefNo);
                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));

                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable Details = new DataTable();
                    string Mastersql = "";
                    //string DetailSql = "";
                    string DetailGetSql = "";


                    // if (bAllBranchData)
                    //  xCondition="X_ReceiptNo=@xReceiptNo and N_CompanyId=@nCompanyID";
                    // else
                    //     xCondition="X_ReceiptNo=@xReceiptNo and N_CompanyId=@nCompanyID and N_BranchID=@nBranchID";

                    Mastersql = "Select * from vw_Inv_ReceivableStock where N_CompanyOD=" + nCompanyID + " and [MemoNo]='" + xRefNo + "' and N_FnYearId=" + nFnYearID + "";

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    MasterTable = _api.Format(MasterTable, "Master");
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int N_ReceivableId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ReceivableId"].ToString());
                ////////
                    Params.Add("@N_ReceivableId", N_ReceivableId);
                    DetailGetSql ="Select * from vw_InvReceivableStockDetails  Where N_CompanyID=" +nCompanyID+ " and N_ReceivableId=" + N_ReceivableId+"";
                    Details = dLayer.ExecuteDataTable(DetailGetSql, Params, connection);

                    Details = _api.Format(Details, "Details");
                    dt.Tables.Add(Details);
                    dt.Tables.Add(MasterTable);
                    return Ok(_api.Success(dt));

                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }








    }
}