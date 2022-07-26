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
            FormID = 506;
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
                    string sqlCommandCount = "", xCriteria = " N_FnYearID=@p2 and N_CompanyID=@p1 ";
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
                            xCriteria = " N_FnYearID=@p2 and N_BranchID=@p3 and N_CompanyID=@p1 and N_LoactionID=@p4";

                        else
                            xCriteria = " N_FnYearID=@p2 and N_CompanyID=@p1 and N_BranchID=@p3";
                    }

                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = " and ( N_ItemID like '%" + xSearchkey + "%'  or  X_RefNo like '%" + xSearchkey + "%' or  X_LocationName like '%" + xSearchkey + "%' or cast([AdjustDate] as VarChar) like '%" + xSearchkey + "%') ";

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by X_RefNo desc";
                    else
                        xSortBy = " order by " + xSortBy;
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ")  N_CompanyID,N_FnYearID,X_RefNo,AdjustDate,N_UserID,N_LoactionID,X_LocationName,X_Remarks from vw_InvStockAdjustment_Disp where " + xCriteria + Searchkey + " Group By  N_CompanyID,N_FnYearID,X_RefNo,AdjustDate,N_UserID,N_LoactionID,X_LocationName,X_Remarks";
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ")  N_CompanyID,N_FnYearID,X_RefNo,AdjustDate,N_UserID,N_LoactionID,X_Description,X_LocationName,X_Remarks from vw_InvStockAdjustment_Disp where " + xCriteria + Searchkey + " and N_ItemID not in (select top(" + Count + ") N_ItemID from vw_InvStockAdjustment_Disp where " + xCriteria + Searchkey + " ) " + " Group By  N_CompanyID,N_FnYearID,X_RefNo,AdjustDate,N_UserID,N_LoactionID,X_Description,X_LocationName,X_Remarks";
                    SortedList OutPut = new SortedList();

                    dt = dLayer.ExecuteDataTable(sqlCommandText + xSortBy, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count   from (select N_CompanyID,N_FnYearID,X_RefNo,AdjustDate,N_UserID,N_LoactionID,X_LocationName,X_Remarks from vw_InvStockAdjustment_Disp where " + xCriteria + Searchkey + " Group By N_CompanyID,N_FnYearID,X_RefNo,AdjustDate,N_UserID,N_LoactionID,X_LocationName,X_Remarks) as AdjustmentCountTable";
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
                    int Results = 0;
                    SortedList DelParam = new SortedList();
                    DelParam.Add("N_CompanyID", nCompanyID);
                    DelParam.Add("N_AdjustmentID", nAdjustmentID);
                    Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_StockAdjustment", DelParam, connection, transaction);

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
                    DataTable StockTable;
                    DataTable DetailsToImport;
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DetailsToImport = ds.Tables["detailsImport"];
                    bool B_isImport = false;
                    if (ds.Tables.Contains("detailsImport"))
                    B_isImport = true;
                    
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nAdjustmentID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AdjustmentID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());
                    int  n_LoactionID =  myFunctions.getIntVAL(MasterTable.Rows[0]["n_LoactionID"].ToString());
                    string X_RefNo = MasterTable.Rows[0]["X_RefNo"].ToString();
                    int n_isSaveDraft = myFunctions.getIntVAL(MasterTable.Rows[0]["b_IsSaveDraft"].ToString());
                    DataTable newTable = new DataTable();
                    bool bStockMisMatch = false;
                    string X_TransType = "IA";

                 


                    if (nAdjustmentID > 0)
                    {
                        SortedList DelParam = new SortedList();
                        DelParam.Add("N_CompanyID", nCompanyID);
                        DelParam.Add("N_AdjustmentID", nAdjustmentID);
                        dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_StockAdjustment", DelParam, connection, transaction);
                    }
                    string stockMasterSql = "select vw_InvItem_Search.N_ItemID,N_CompanyID,[Description], dbo.[SP_LocationStock](vw_InvItem_Search.N_ItemID,"+n_LoactionID+") As N_Stock   From vw_InvItem_Search where   [Item Code]<>'001' and N_CompanyID=" + nCompanyID + " and N_ClassID<>4 ";
                    StockTable = dLayer.ExecuteDataTable(stockMasterSql, Params, connection, transaction);
                    if (n_isSaveDraft == 0)
                    {
                        if (StockTable.Rows.Count > 0)
                        {
                            foreach (DataRow stockItem in StockTable.Rows)
                            {
                                foreach (DataRow DetailItem in DetailTable.Rows)
                                {
                                    if (stockItem["N_ItemID"].ToString() == DetailItem["N_ItemID"].ToString())
                                    {
                                        if (myFunctions.getVAL(stockItem["N_Stock"].ToString()) !=(myFunctions.getVAL(DetailItem["N_QtyOnHand"].ToString())))
                                        {
                                            bStockMisMatch = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if(bStockMisMatch)
                    {
                         return Ok(_api.Success(bStockMisMatch));

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

                    if (B_isImport)
                    {
                        foreach (DataColumn col in DetailsToImport.Columns)
                        {
                            col.ColumnName = col.ColumnName.Replace(" ", "_");
                            col.ColumnName = col.ColumnName.Replace("*", "");
                            col.ColumnName = col.ColumnName.Replace("/", "_");
                        }

                        DetailsToImport.Columns.Add("N_MasterID");
                        DetailsToImport.Columns.Add("X_Type");
                        DetailsToImport.Columns.Add("N_CompanyID");
                        DetailsToImport.Columns.Add("PkeyID");
                        foreach (DataRow dtRow in DetailsToImport.Rows)
                        {
                            dtRow["N_MasterID"] = nAdjustmentID;
                            dtRow["N_CompanyID"] = nCompanyID;
                            dtRow["PkeyID"] = 0;
                        }

                        dLayer.ExecuteNonQuery("delete from Mig_Adjustment ", connection, transaction);
                        dLayer.SaveData("Mig_Adjustment", "PkeyID", "", "", DetailsToImport, connection, transaction);

                        SortedList ProParam = new SortedList();
                        ProParam.Add("N_CompanyID", nCompanyID);
                        ProParam.Add("N_PKeyID", nAdjustmentID);

                        SortedList ValidationParam = new SortedList();
                        ValidationParam.Add("N_CompanyID", nCompanyID);
                        ValidationParam.Add("N_FnYearID", nFnYearID);
                        ValidationParam.Add("X_Type", "inventory adjustment");
                            try
                            {
                                dLayer.ExecuteNonQueryPro("SP_SetupData_Validation", ValidationParam, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, ex));
                            }
                         
                        ProParam.Add("X_Type", "inventory adjustment");
                        DetailTable = dLayer.ExecuteDataTablePro("SP_ScreenDataImport", ProParam, connection,transaction);
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

                    if (n_isSaveDraft == 0)
                    {
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
                    }

                    transaction.Commit();
                SortedList Result = new SortedList();
                Result.Add("n_AdjustmentID", nAdjustmentID);
                Result.Add("x_RefNo", X_RefNo);
                return Ok(_api.Success(Result, "Saved Successfully"));
                    //return Ok(_api.Success("Saved Successfully"));
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
                X_MasterSql = "Select X_ItemName as description,* from vw_InvStockAdjustment_Disp where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and X_RefNo='" + xRefNo + "'";
            }
            else
            {

                X_MasterSql = "Select X_ItemName as description,* from vw_InvStockAdjustment_Disp where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and N_BranchID=" + nBranchId + " and X_RefNo='" + xRefNo + "'";
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

