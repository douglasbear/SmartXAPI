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
    [Route("Inventorytransfer")]
    [ApiController]
    public class Inv_WHTransfer : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;

        public Inv_WHTransfer(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 367;
        }
        private readonly string connectionString;

        [HttpGet("list")]
        public ActionResult GetLocationDetails(int? nCompanyId, string prs, bool bLocationRequired, bool bAllBranchData, int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "";
            if (prs == null || prs == "")
                sqlCommandText = "select * from vw_InvLocation_Disp where N_CompanyID=@p1 order by N_LocationID DESC";
            else
            {
                if (!bLocationRequired)
                {
                    if (bAllBranchData == true)
                        sqlCommandText = "select * from vw_InvLocation_Disp where N_MainLocationID =0 and N_CompanyID=" + nCompanyId;

                    else
                        sqlCommandText = "select * from vw_InvLocation_Disp where  N_MainLocationID =0 and N_CompanyID=" + nCompanyId + " and  N_BranchID=" + nBranchID;

                }
                else
                {
                    sqlCommandText = "select * from vw_InvLocation_Disp where  N_MainLocationID =0 and N_CompanyID=" + nCompanyId + " and  N_BranchID=" + nBranchID;
                }
            }

            Params.Add("@p1", nCompanyId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(_api.Format(dt)));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        [HttpGet("productInformation")]

        public ActionResult GetAllItems(string query, int nCompanyID, int nLocationIDFrom, int nLocationIDTo, int PageSize, int Page, int nCategoryID, string xClass, int nNotItemID, int nNotGridItemID, DateTime dtpInvDate, string xBarcode)
        {

            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            if (nLocationIDTo == 0)
                nLocationIDTo = nLocationIDFrom;

            string qry = "";
            string Category = "";
            string Condition = "";
            if (query != "" && query != null)
            {
                qry = " and (Description like @query or [Item Code] like @query or  X_BarCode like @query or [Part No] like @query or description_Ar like @query) ";
                Params.Add("@query", "%" + query + "%");
            }
            if (xBarcode != "" && xBarcode != null)
            {
                qry = qry + " and X_barcode='" + xBarcode + "'";

            }
            string pageQry = "DECLARE @PageSize INT, @Page INT Select @PageSize=@PSize,@Page=@Offset;WITH PageNumbers AS(Select ROW_NUMBER() OVER(ORDER BY vw_InvItem_Search.N_ItemID) RowNo,";
            string pageQryEnd = ") SELECT * FROM    PageNumbers WHERE   RowNo BETWEEN((@Page -1) *@PageSize + 1)  AND(@Page * @PageSize) ";
            string sqlComandText = "";
            if (dtpInvDate == null || dtpInvDate.ToString() == "")
                sqlComandText = "Select *,dbo.[SP_GenGetStockByDate](vw_InvItem_Search.N_ItemID," + nLocationIDFrom + ",'','location','" + myFunctions.getDateVAL(dtpInvDate.Date) + "') As N_Stock ,dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_StockUnit) As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice  From vw_InvItem_Search Where N_CompanyID=" + nCompanyID + " and (N_ClassID<>1 AND N_ClassID<>4" + qry;
            else
                sqlComandText = " vw_InvItem_Search.*,dbo.[SP_LocationStock](vw_InvItem_Search.N_ItemID," + nLocationIDFrom + ") As N_Stock ,dbo.SP_Cost_Loc(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_ItemUnit," + nLocationIDFrom + ")  As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice  From vw_InvItem_Search Where [Item Code]<>'001' and N_CompanyID=" + nCompanyID + " and N_ClassID<>4 and N_ItemID in  (select N_ItemID from vw_InvItem_Search_WHLink where N_CompanyID=" + nCompanyID + " and N_WareHouseID=" + nLocationIDTo + "  "+qry+" ) " + qry;
            // Select *,dbo.[SP_GenGetStockByDate](vw_InvItem_Search.N_ItemID," + N_LocationID + ",'','location','" + myFunctions.getDateVAL(dtpInvDate.Value.Date) + "') As N_Stock ,dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_StockUnit) As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice  From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=" + myCompanyID._CompanyID + "and (N_ClassID<>1 AND N_ClassID<>4)
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", 0);
            Params.Add("@p3", "001");
            Params.Add("@p4", 1);
            Params.Add("@PSize", PageSize);
            Params.Add("@Offset", Page);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = pageQry + sqlComandText + pageQryEnd;
                    dt = dLayer.ExecuteDataTable(sql, Params, connection);

                    dt.AcceptChanges();
                    dt = myFunctions.AddNewColumnToDataTable(dt, "SubItems", typeof(DataTable), null);

                    foreach (DataRow item in dt.Rows)
                    {
                        if (myFunctions.getIntVAL(item["N_ClassID"].ToString()) == 1)
                        {

                            string subItemSql = "Select *,dbo.[SP_LocationStock](vw_invitemdetails.N_ItemID," + nLocationIDFrom + ") As N_Stock,dbo.SP_Cost_Loc(vw_invitemdetails.N_ItemID,vw_invitemdetails.N_CompanyID,''," + nLocationIDFrom + ") As N_LPrice ,dbo.SP_SellingPrice(vw_invitemdetails.N_ItemID,vw_invitemdetails.N_CompanyID) As N_SPrice from vw_invitemdetails where N_MainItemId=" + myFunctions.getIntVAL(item["N_ItemID"].ToString()) + " and N_CompanyID=" + nCompanyID + " ";
                            DataTable subTbl = dLayer.ExecuteDataTable(subItemSql, connection);
                            item["SubItems"] = subTbl;
                        }
                    }
                    dt.AcceptChanges();
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
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



        [HttpGet("details")]
        public ActionResult EmpMaintenanceList(int nCompanyId, int nFnYearID, bool bAllBranchData, int nBranchID, int nLocationID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, int nFormID)
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
                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearID);
                    Params.Add("@p3", nBranchID);



                    bool CheckClosedYear = Convert.ToBoolean(dLayer.ExecuteScalar("Select B_YearEndProcess From Acc_FnYear Where N_CompanyID=@p1 and N_FnYearID=@p2 ", Params, connection));
                    if(nFormID==367)
                    {
                    if (!CheckClosedYear)
                    {
                        if (bAllBranchData)
                            Criteria = "and B_YearEndProcess=0 and N_Type=1 and ( N_LocationFrom=" + nLocationID + " or N_LocationFrom in (select N_LocationID from Inv_Location where n_CompanyID=@p1 and N_WarehouseID=" + nLocationID + "))";
                        else
                            Criteria = "and B_YearEndProcess=0 and N_Type=1 and ( N_LocationFrom=" + nLocationID + " or N_LocationFrom in (select N_LocationID from Inv_Location where n_CompanyID=@p1 and N_WarehouseID=" + nLocationID + "))";
                    }
                    else
                    {
                        if (bAllBranchData)
                            Criteria = "and N_PurchaseType=0 and X_TransType=@p4  and ( N_LocationFrom=" + nLocationID + " or N_LocationFrom in (select N_LocationID from Inv_Location where n_CompanyID=@p1 and N_WarehouseID=" + nLocationID + "))";
                        else
                            Criteria = "and N_PurchaseType=0 and X_TransType=@p4  and ( N_LocationFrom=" + nLocationID + " or N_LocationFrom in (select N_LocationID from Inv_Location where n_CompanyID=@p1 and N_WarehouseID=" + nLocationID + "))";
                    }
                    }
                    else
                    {
                          if (!CheckClosedYear)
                    {
                        if (bAllBranchData)
                            Criteria = "and B_YearEndProcess=0 and N_Type=1 and ( N_EntryLocationID=" + nLocationID + " )";
                        else
                            Criteria = "and B_YearEndProcess=0 and N_Type=1 and ( N_EntryLocationID=" + nLocationID + ")";
                    }
                    else
                    {
                        if (bAllBranchData)
                            Criteria = "and N_PurchaseType=0 and X_TransType=@p4 and N_FormID=1442  and ( N_EntryLocationID=" + nLocationID + ")";
                        else
                            Criteria = "and N_PurchaseType=0 and X_TransType=@p4 N_FormID=1442   and ( N_EntryLocationID=" + nLocationID + " )";
                    }

                    }


                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and ([Site from] like '%" + xSearchkey + "%' or [Reference No] like '%" + xSearchkey + "%' or [Site To] like '%" + xSearchkey + "%' or Date like '%" + xSearchkey + "%')";

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_TransferID desc";
                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "referenceNo":
                                xSortBy = "[Reference No]" + xSortBy.Split(" ")[1];
                                break;
                            case "siteFrom":
                                xSortBy = "[Site From]" + xSortBy.Split(" ")[1];
                                break;
                            case "siteTo":
                                xSortBy = "[Site To]" + xSortBy.Split(" ")[1];
                                break;

                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }

                    if (Count == 0)
                    {

                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvTransfer_Search where N_CompanyID=@nCompanyId and N_FnYearID=" + nFnYearID + "   " + Searchkey + Criteria + xSortBy;

                    }
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvTransfer_Search where N_CompanyID=@nCompanyId " + Searchkey + Criteria + " and N_TransferID not in (select top(" + Count + ") N_TransferID from vw_InvTransfer_Search where N_CompanyID=@nCompanyId " + Criteria + xSortBy + " ) " + xSortBy;

                    Params.Add("@nCompanyId", nCompanyId);

                    SortedList OutPut = new SortedList();



                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(1) as N_Count  from vw_InvTransfer_Search where N_CompanyID=@nCompanyId " + Searchkey + Criteria;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);

                    return Ok(_api.Success(OutPut));


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
                    bool bAutoReceive = false;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    String xButtonAction="";
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nTransferId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TransferId"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());
                    int nLocationIDfrom = myFunctions.getIntVAL(MasterTable.Rows[0]["n_LocationIDFrom"].ToString());
                     int nLocationIDto =0;
                    if(MasterTable.Columns.Contains("n_LocationIDTo"))
                    {
                       nLocationIDto=myFunctions.getIntVAL(MasterTable.Rows[0]["n_LocationIDTo"].ToString());
                    }
                   
                    int nLocationIDtoInGrid = myFunctions.getIntVAL(DetailTable.Rows[0]["n_LocationIDTo"].ToString());
                    string X_ReferenceNo = MasterTable.Rows[0]["X_ReferenceNo"].ToString();


                    string X_TransType = "TRANSFER";

                    // int nUsercategoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserCategoryID"].ToString());
                    // int nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());
                    // int nLevelID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_Level"].ToString());
                    // int nActionID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ActionTypeID"].ToString());
                    if (nTransferId > 0)
                    {
                        SortedList DelParam = new SortedList();
                        DelParam.Add("N_CompanyID", nCompanyID);
                        DelParam.Add("X_TransType", X_TransType);
                        DelParam.Add("N_VoucherID", nTransferId);
                        dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DelParam, connection, transaction);
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
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_TransferStock Where X_ReferenceNo ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_ReferenceNo = DocNo;

                        xButtonAction="Insert";
                        if (X_ReferenceNo == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate")); }
                        MasterTable.Rows[0]["X_ReferenceNo"] = X_ReferenceNo;

                        //     if(Processed)
                        //        {
                        //     transaction.Rollback();
                        //     return Ok(_api.Error(User,"Unable To Save"));
                        // }

                    }
                    else
                    {
                        xButtonAction="Update"; 
                        dLayer.DeleteData("Inv_TransferStock", "N_TransferId", nTransferId, "", connection, transaction);
                    }






                    nTransferId = dLayer.SaveData("Inv_TransferStock", "N_TransferId", MasterTable, connection, transaction);
                    if (nTransferId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }

                    for (int i = 0; i < DetailTable.Rows.Count; i++)
                    {
                        DetailTable.Rows[i]["N_TransferId"] = nTransferId;
                    }
                    int nTransferDetailsID = dLayer.SaveData("Inv_TransferStockDetails", "N_TransferDetailsID", DetailTable, connection, transaction);
                    if (nTransferDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }


                    else
                    {

                         SortedList StockParam = new SortedList();
                        StockParam.Add("N_CompanyID", nCompanyID);
                        StockParam.Add("N_TransferID", nTransferId);
                        StockParam.Add("@N_WarehouseIdFrom", nLocationIDfrom);
                        StockParam.Add("@N_WarehouseIdTo", nLocationIDto);
                        StockParam.Add("N_UserID", nUserID);

                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Inv_StockTransfer ", StockParam, connection, transaction).ToString();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }

                        SortedList PostingParam = new SortedList();
                        PostingParam.Add("N_CompanyID", nCompanyID);
                        PostingParam.Add("X_InventoryMode", X_TransType);
                        PostingParam.Add("N_InternalID", nTransferId);
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

                        bAutoReceive = Convert.ToBoolean(dLayer.ExecuteScalar("Select isnull(B_AutoReceive,0) From Inv_Location Where N_CompanyID=" + nCompanyID + " and N_LocationID = " + nLocationIDtoInGrid, StockParam, connection, transaction));
                        if (bAutoReceive)
                        {

                            SortedList AutoReceiveParam = new SortedList();
                            AutoReceiveParam.Add("N_CompanyID", nCompanyID);
                            AutoReceiveParam.Add("N_TransferID", nTransferId);
                            try
                            {
                                dLayer.ExecuteNonQueryPro("SP_AutoTransferReceive ", AutoReceiveParam, connection, transaction).ToString();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, ex));
                            }
                        }
                        if(nLocationIDfrom>0)
                        {
                        object nTypeID = dLayer.ExecuteScalar("select N_TypeID from Inv_Location where N_LocationID=" + nLocationIDfrom + "  and N_CompanyID=" + nCompanyID + " ", Params, connection, transaction);
                        if (myFunctions.getIntVAL(nTypeID.ToString()) == 6)
                        {
                            object stock = dLayer.ExecuteScalar("select sum(N_CurrentStock) from Inv_StockMaster where N_LocationID=" + nLocationIDfrom + "  and N_CompanyID=" + nCompanyID + " ", Params, connection, transaction);
                            if (stock == null || myFunctions.getVAL(stock.ToString()) == 0)
                            {
                                dLayer.ExecuteNonQuery("UPDATE Inv_Location  set B_InActive=1 where  N_CompanyID=" + nCompanyID + "  and N_LocationID=" + nLocationIDfrom, Params, connection, transaction);
                            }
                        }
                        }
                        if(myFunctions.getIntVAL(MasterTable.Rows[0]["n_PRSID"].ToString())>0)
                        {
                             dLayer.ExecuteNonQuery("UPDATE Inv_PRS  set N_Processed=1 where  N_CompanyID=" + nCompanyID + "  and N_PrsID=" + myFunctions.getIntVAL(MasterTable.Rows[0]["n_PRSID"].ToString()), Params, connection, transaction);
                        }
                          if(nLocationIDto>0)
                        { 
                            bool bAutoReceiveMaster = Convert.ToBoolean(dLayer.ExecuteScalar("Select isnull(B_AutoReceive,0)  From Inv_Location Where N_CompanyID=" + nCompanyID + " and N_LocationID = " + nLocationIDto, StockParam, connection, transaction));
                            if (bAutoReceiveMaster)
                            {
                                if(myFunctions.getIntVAL(MasterTable.Rows[0]["n_PRSID"].ToString())>0)
                                {
                                     dLayer.ExecuteNonQuery("UPDATE Inv_PRS  set N_Processed=2 where  N_CompanyID=" + nCompanyID + "  and N_PrsID=" + myFunctions.getIntVAL(MasterTable.Rows[0]["n_PRSID"].ToString()), Params, connection, transaction);
                                 }

                            }
                        }









                    }
                    // Activity Log
                        string ipAddress = "";
                        if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                           ipAddress = Request.Headers["X-Forwarded-For"];
                        else
                           ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                           myFunctions.LogScreenActivitys(nFnYearID,nTransferId,X_ReferenceNo,this.FormID,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                        
                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }



        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID, int nTransferId, int nfromLocationID, int nFnYearID)
        {
            int Results = 0;
            string xTransType = "TRANSFER";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))

                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable TransData = new DataTable();
                    string xButtonAction="Delete";
                    string X_ReferenceNo = "";
                    string Sql = "select N_TransferID,X_ReferenceNo from Inv_TransferStock where N_CompanyID="+nCompanyID+" and N_TransferID="+nTransferId+"";
                      int n_PRSID=myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_PrsID from Inv_TransferStock where N_CompanyID="+nCompanyID+" and N_TransferID="+nTransferId+"", connection, transaction).ToString());

                    SortedList deleteParams = new SortedList()
                            {
                                   {"N_CompanyID",nCompanyID},
                                {"X_TransType","TRANSFER"},
                                {"N_VoucherID",nTransferId},
                                {"N_UserID",myFunctions.GetUserID(User)},
                                 {"X_SystemName","WebRequest"}
                            };
                    TransData = dLayer.ExecuteDataTable(Sql, deleteParams, connection, transaction);
                    // Activity Log
                        string ipAddress = "";
                        if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                            ipAddress = Request.Headers["X-Forwarded-For"];
                        else
                            ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                            myFunctions.LogScreenActivitys(myFunctions.getIntVAL( nFnYearID.ToString()),nTransferId,TransData.Rows[0]["X_ReferenceNo"].ToString(),this.FormID,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                        
                    Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", deleteParams, connection, transaction);
                    if (Results > 0)
                    {

                        object nTypeID = dLayer.ExecuteScalar("select N_TypeID from Inv_Location where N_LocationID=" + nfromLocationID + "  and N_CompanyID=" + nCompanyID + " ", connection, transaction);
                        if (myFunctions.getIntVAL(nTypeID.ToString()) == 6)
                        {
                            object stock = dLayer.ExecuteScalar("select sum(N_CurrentStock) from Inv_StockMaster where N_LocationID=" + nfromLocationID + "  and N_CompanyID=" + nCompanyID + " ", connection, transaction);
                            if (stock != null || myFunctions.getVAL(stock.ToString()) > 0)
                            {
                                dLayer.ExecuteNonQuery("UPDATE Inv_Location  set B_InActive=0 where  N_CompanyID=" + nCompanyID + "  and N_LocationID=" + nfromLocationID, connection, transaction);
                            }
                        }



                      
                        if(n_PRSID>0)
                        {
                             dLayer.ExecuteNonQuery("UPDATE Inv_PRS  set N_Processed=0 where  N_CompanyID=" + nCompanyID + "  and N_PrsID=" + n_PRSID, deleteParams, connection, transaction);
                        }



                        transaction.Commit();
                        return Ok(_api.Success("Deleted"));

                    }
                    else
                    {
                        return Ok(_api.Error(User, "Unable to delete"));
                    }

                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }


        [HttpGet("viewdetails")]
        public ActionResult viewDetails(string xReceiptNo, int nBranchID, int nFnYearID,int nPRSID,int nPickListID )
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    SortedList Params = new SortedList();
                    Params.Add("@xReceiptNo", xReceiptNo);
                    Params.Add("@nBranchID", nBranchID);
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

                    Mastersql = "Select Inv_TransferStock.*,Inv_warehouseMasterFrom.X_LocationName As X_WarehouseNameFrom,Inv_warehouseMasterFrom.X_LocationCode As X_LocationCodeFrom,Inv_warehouseMasterTo.X_LocationName As X_WarehouseNameTo,Inv_warehouseMasterTo.X_LocationCode As X_LocationCodeTo,Inv_PRS.X_PRSNo,Inv_PRS.X_Purpose,Inv_PRS.N_PRSID,Inv_Department.N_DepartmentID,Inv_Department.X_DepartmentCode,Inv_Department.X_Department,Wh_PickList.X_PickListCode from Inv_TransferStock  left outer Join Inv_Location As Inv_warehouseMasterFrom on Inv_TransferStock.N_LocationIDFrom = Inv_warehouseMasterFrom.N_LocationID And Inv_TransferStock.N_CompanyID = Inv_warehouseMasterFrom.N_CompanyID  left outer  Join Inv_Location As Inv_warehouseMasterTo on Inv_TransferStock.N_LocationIDTo  = Inv_warehouseMasterTo.N_LocationID And Inv_TransferStock.N_CompanyID = Inv_warehouseMasterTo.N_CompanyID " +
                   "left outer join Inv_PRS on Inv_TransferStock.N_PRSID=Inv_PRS.N_PRSID left outer join Wh_PickList on Inv_TransferStock.N_PickListID=Wh_PickList.N_PickListID And Inv_TransferStock.N_CompanyID = Wh_PickList.N_CompanyID left outer join Inv_Department On Inv_PRS.N_DepartmentID=Inv_Department.N_DepartmentID Where  Inv_TransferStock.N_CompanyID=" + nCompanyID + " and Inv_TransferStock.X_ReferenceNo='" + xReceiptNo + "' and Inv_TransferStock.N_FnYearId=" + nFnYearID + "";
                   if(nPRSID >0)
                   {
                    Mastersql="select * from VW_TransferReqToTransfer where N_CompanyID="+nCompanyID+" and N_FnYearID="+nFnYearID+" and N_PRSID="+nPRSID+"";

                   }
                     if(nPickListID >0)
                   {
                    Mastersql="select * from Vw_WhPickListToTransfer where N_CompanyID="+nCompanyID+" and N_FnYearID="+nFnYearID+" and N_PickListID="+nPickListID+"";

                   }

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    MasterTable = _api.Format(MasterTable, "Master");
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                       if(nPRSID==0 &&nPickListID==0 ){
                    int nTransferId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TransferId"].ToString());
                    int N_LocationIDFrom = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LocationIDFrom"].ToString());
                 
                    // DateTime dTransdate = Convert.ToDateTime(MasterTable.Rows[0]["D_ReceiptDate"].ToString());
                    Params.Add("@nTransferId", nTransferId);
  
                    DetailGetSql = "Select vw_InvTransferStockDetails.*,dbo.[SP_BatchStock](vw_InvTransferStockDetails.N_ItemID," + N_LocationIDFrom + ",vw_InvTransferStockDetails.X_BatchCode,0) As N_Stock ,dbo.SP_Cost_Loc(vw_InvTransferStockDetails.N_ItemID,vw_InvTransferStockDetails.N_CompanyID,''," + N_LocationIDFrom + ") As N_LPrice,dbo.SP_SellingPrice(vw_InvTransferStockDetails.N_ItemID,vw_InvTransferStockDetails.N_CompanyID) As N_UnitSPrice " +
                    " from vw_InvTransferStockDetails  Where vw_InvTransferStockDetails.N_CompanyID=" + nCompanyID + " and vw_InvTransferStockDetails.N_TransferId=" + nTransferId + "";
                       }
                    //convert-----
                    if(nPRSID>0)
                    {
                       DetailGetSql="select * from VW_TransferReqToTransferDetails where   N_CompanyID="+nCompanyID+" and N_PRSID="+nPRSID+"";
                    }
                    if(nPickListID>0)
                    {
                       DetailGetSql="select * from Vw_WhPickListToTransferDetails where   N_CompanyID="+nCompanyID+" and N_PickListID="+nPickListID+"";
                    }
                    int nPicklstID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PickListID"].ToString());
                    if(nPicklstID>0 && nPickListID<=0)
                    {
                         DetailGetSql="select * from Vw_ProductTransferDetails where   N_CompanyID="+nCompanyID+" and X_ReferenceNo=@xReceiptNo";
                    }
                    Details = dLayer.ExecuteDataTable(DetailGetSql, Params, connection);
                    if(!Details.Columns.Contains("N_ClassID"))
                    {
                    Details = myFunctions.AddNewColumnToDataTable(Details, "N_ClassID", typeof(int), 0);
                     

                    foreach (DataRow item in Details.Rows)
                    {
                        object classID = dLayer.ExecuteScalar(" Select N_ClassId,N_ItemId from Inv_ItemMaster where N_ItemID=" + myFunctions.getIntVAL(item["n_ItemID"].ToString()) + " and N_CompanyID=" + nCompanyID, Params, connection);
                        if (classID != null)
                        {
                            item["n_ClassID"] = myFunctions.getIntVAL(classID.ToString());
                        }



                    }
                    Details.AcceptChanges();
                    }





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
//       