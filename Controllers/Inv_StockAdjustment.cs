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
    [Route("invStockAdjustment")]
    [ApiController]
    public class Inv_StockAdjustment : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly int FormID;
        private readonly string connectionString;
        private readonly IMyAttachments myAttachments;

        public Inv_StockAdjustment(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            myAttachments = myAtt;
            FormID = 1056;
        }




        [HttpGet("list")]
        public ActionResult InvAdjustmentList(int nFnYearID, int nLocationID, int nBranchID, int nPage, int nSizeperpage, bool b_AllBranchData, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    string sqlCommandCount = "", xCriteria = "";
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string Searchkey = "";
                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nFnYearID);
                    Params.Add("@p3", nBranchID);
                    Params.Add("@p4", nLocationID);

                    if (b_AllBranchData)
                    {
                        if (nLocationID > 0)
                            xCriteria = " N_FnYearID=@p2 and N_CompanyID=@p1 and N_LoactionID=@p4 ";

                        else
                            xCriteria = " N_FnYearID=@p2 and N_CompanyID=@p1 ";
                    }
                    else
                    {
                        if (nLocationID > 0)
                            xCriteria = "and N_FnYearID=@p2 and N_BranchID=@p3 and N_CompanyID=@p1 and N_LoactionID=@p4";

                        else
                            xCriteria = "and N_FnYearID=@p2 and N_CompanyID=@p1 and N_BranchID=@p3";
                    }

                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and ( N_ItemID like '%" + xSearchkey + "%') ";

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by X_RefNo desc";
                    else
                        xSortBy = " order by " + xSortBy;
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ")  N_CompanyID,N_FnYearID,X_RefNo,AdjustDate,N_UserID,N_LoactionID,X_Description,X_LocationName from vw_InvStockAdjustment_Disp where " + xCriteria + Searchkey + " Group By  N_CompanyID,N_FnYearID,X_RefNo,AdjustDate,N_UserID,N_LoactionID,X_Description,X_LocationName";
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ")  N_CompanyID,N_FnYearID,X_RefNo,AdjustDate,N_UserID,N_LoactionID,X_Description,X_LocationName from vw_InvStockAdjustment_Disp where " + xCriteria + Searchkey + "and N_ItemID not in (select top(" + Count + ") N_ItemID from vw_InvStockAdjustment_Disp where " + xCriteria + Searchkey + " ) " + " Group By  N_CompanyID,N_FnYearID,X_RefNo,AdjustDate,N_UserID,N_LoactionID,X_Description,X_LocationName";
                    SortedList OutPut = new SortedList();

                    dt = dLayer.ExecuteDataTable(sqlCommandText + xSortBy, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from vw_InvStockAdjustment_Disp where " + xCriteria + Searchkey;
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
                return BadRequest(_api.Error(User, e));
            }
        }






        [HttpDelete("delete")]
        public ActionResult DeleteData(int nAdjustmentID)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    var nUserID = myFunctions.GetUserID(User);
                    int Results=0;
                    SortedList DelParam = new SortedList();
                        DelParam.Add("N_CompanyID", nCompanyID);
                        DelParam.Add("N_AdjustmentID", nAdjustmentID);
                        Results=dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_StockAdjustment", DelParam, connection, transaction);

                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete"));
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Deleted Sucessfully"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
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
                    int nAdjustmentID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AdjustmentID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());
                    string X_RefNo = MasterTable.Rows[0]["X_RefNo"].ToString();
                    string X_TransType = "IA";
                    if (nAdjustmentID > 0)
                    {
                        SortedList DelParam = new SortedList();
                        DelParam.Add("N_CompanyID", nCompanyID);
                        DelParam.Add("N_AdjustmentID", nAdjustmentID);
                        dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_StockAdjustment", DelParam, connection, transaction);
                    }
                    DocNo = MasterRow["X_RefNo"].ToString();
                    if (X_RefNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("N_YearID", nFnYearID);

                        while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_StockAdjustment Where X_RefNo ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_RefNo = DocNo;


                        if (X_RefNo == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate")); }
                        MasterTable.Rows[0]["X_RefNo"] = X_RefNo;

                    }
                    nAdjustmentID = dLayer.SaveData("Inv_StockAdjustment", "N_AdjustmentID", MasterTable, connection, transaction);
                    if (nAdjustmentID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }

                    for (int i = 0; i < DetailTable.Rows.Count; i++)
                    {
                        DetailTable.Rows[i]["N_AdjustmentID"] = nAdjustmentID;
                    }
                    int nAdjustmentDetailID = dLayer.SaveData("Inv_StockAdjustmentDetails", "N_AdjustmentDetailsID", DetailTable, connection, transaction);
                    if (nAdjustmentDetailID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }
                    SortedList StockParam = new SortedList();
                    StockParam.Add("N_CompanyID", nCompanyID);
                    StockParam.Add("N_AdjustmentID", nAdjustmentID);
                    StockParam.Add("N_UserID", nUserID);

                    try
                    {
                        dLayer.ExecuteNonQueryPro("SP_Inv_StockAdjustmentIns ", StockParam, connection, transaction).ToString();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, ex));
                    }

                    SortedList PostingParam = new SortedList();
                    PostingParam.Add("N_CompanyID", nCompanyID);
                    PostingParam.Add("X_InventoryMode", X_TransType);
                    PostingParam.Add("N_InternalID", nAdjustmentID);
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
                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }


  [HttpGet("listdetails")]
        public ActionResult GetSalesDetails(int nCompanyId, int nFnYearId, string xRefNo, string xTransType, bool showAllBranch, int nBranchId, string xPath)
        {

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            //string sqlCommandText = "";
              string X_MasterSql = "";
          if (showAllBranch)
                    {
                        X_MasterSql = "Select description as X_Description,X_ItemName as description,* from vw_InvStockAdjustment_Disp where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and X_RefNo='" + xRefNo + "'";
                    }
                    else
                    {

                        X_MasterSql = "Select  description as X_Description,X_ItemName as description,* from vw_InvStockAdjustment_Disp where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and N_BranchID=" + nBranchId + " and X_RefNo='" + xRefNo + "'";
                    }
           
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(X_MasterSql, Params, connection);
                  
                    dt.AcceptChanges();

                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                     
                        dt = _api.Format(dt, "master");
                        ds.Tables.Add(dt);
                        
                        return Ok(_api.Success(ds));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }

        }


        
        // [HttpGet("listdetails")]
        // public ActionResult GetSalesDetails(int nCompanyId, int nFnYearId, string xRefNo, string xTransType, bool showAllBranch, int nBranchId, string xPath)
        // {

        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             DataSet dt = new DataSet();
        //             SortedList Params = new SortedList();
        //             DataTable Master = new DataTable();
        //             DataTable Details = new DataTable();
        //             string X_MasterSql = "";
        //             string X_DetailsSql = "";
        //             if (showAllBranch)
        //             {
        //                 X_MasterSql = "Select * from vw_InvStockAdjustment_Disp where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and X_RefNo='" + xRefNo + "'";
        //             }
        //             else
        //             {


        //                 X_MasterSql = "Select * from vw_InvStockAdjustment_Disp where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and N_BranchID=" + nBranchId + " and X_RefNo='" + xRefNo + "'";
        //             }
        //             Master = dLayer.ExecuteDataTable(X_MasterSql, Params, connection);
        //             if (Master.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
        //              Master = _api.Format(Master, "Master");
        //              dt.Tables.Add(Master);
        //             return Ok(_api.Success(dt));

        //         }
        //     }


        //     catch (Exception e)
        //     {
        //         return Ok(_api.Error(User, e));
        //     }
        // }
    }
}

