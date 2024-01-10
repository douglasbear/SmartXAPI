// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using System;
// using SmartxAPI.GeneralFunctions;
// using System.Data;
// using System.Collections;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Data.SqlClient;
// using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("goodsreceivenote")]
    [ApiController]
    public class Inv_GoodsReceiveNote : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        public Inv_GoodsReceiveNote(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("list")]
        public ActionResult GetGoodsReceiveList(int? nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,bool bAllBranchData,int nBranchID, int nFormID)
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
            string UserPattern = myFunctions.GetUserPattern(User);
               string Pattern = "";
               
              if (UserPattern != "")
                {
                    Pattern = " and Left(X_Pattern,Len(@UserPattern))=@UserPattern ";
                    Params.Add("@UserPattern",UserPattern);

                }
                // else
                // {
                //     object HierarchyCount = dLayer.ExecuteScalar("select count(N_HierarchyID) from Sec_UserHierarchy where N_CompanyID="+nCompanyId,Params,connection);


                //     if(myFunctions.getIntVAL(HierarchyCount.ToString())>0)
                //     Pattern = " and N_CreatedUser=" + nUserID;
                // }

                //     if(myFunctions.getIntVAL(HierarchyCount.ToString())>0)
                //     Pattern = " and N_UserID=" + nUserID;
                // }
                //     if(myFunctions.getIntVAL(HierarchyCount.ToString())>0)
                //     Pattern = " and N_CreatedUser=" + nUserID;
                // }


            if (xSearchkey != null && xSearchkey.Trim() != "")

            
                Searchkey = "and ([MRN No] like '%" + xSearchkey + "%' or X_VendorName like '%" + xSearchkey + "%' or x_InvoiceNo like '%" + xSearchkey + "%' or D_MRNDate like '%" + xSearchkey + "%' or X_VendorInvoice like '%" + xSearchkey +"%' or x_Description like '%" + xSearchkey + "%' or orderNo like '%" + xSearchkey + "%')";

                        if (bAllBranchData == true)
                        {
                            Searchkey = Searchkey + " ";
                        }
                        else
                        {
                            Searchkey = Searchkey + " and N_BranchID=" + nBranchID + " ";
                        }
            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_MRNID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "[MRN No]":
                        xSortBy = "N_MRNID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }
            int Count = (nPage - 1) * nSizeperpage;
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_VendorID,N_MRNID,N_FnYearID,D_MRNDate,N_BranchID,B_YearEndProcess,B_IsDirectMRN,[MRN No] AS MRNNo,X_VendorName,MRNDate,OrderNo,X_VendorInvoice,x_Description,x_InvoiceNo,N_FormID,X_VendorName_Ar from vw_InvMRNNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and B_IsDirectMRN=1 and N_FormID=@p3 " + Pattern + Searchkey + " " + "Group By  N_CompanyID,N_VendorID,N_MRNID,N_FnYearID,D_MRNDate,N_BranchID,B_YearEndProcess,B_IsDirectMRN,[MRN No],X_VendorName,MRNDate,OrderNo,X_VendorInvoice,x_Description,x_InvoiceNo,N_FormID,X_VendorName_Ar" + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_VendorID,N_MRNID,N_FnYearID,D_MRNDate,N_BranchID,B_YearEndProcess,B_IsDirectMRN,[MRN No] AS MRNNo,X_VendorName,MRNDate,OrderNo,X_VendorInvoice,x_Description,x_InvoiceNo,N_FormID,X_VendorName_Ar from vw_InvMRNNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and B_IsDirectMRN=1 and N_FormID=@p3 "+ Pattern + Searchkey + " and N_MRNID not in (select top(" + Count + ") N_MRNID from Inv_MRN where N_CompanyID=@p1 and N_FnYearID=@p2 and B_IsDirectMRN=1 and N_FormID=@p3 "+ Pattern + Searchkey + " " + xSortBy + " ) " +  "Group By  N_CompanyID,N_VendorID,N_MRNID,N_FnYearID,D_MRNDate,N_BranchID,B_YearEndProcess,B_IsDirectMRN,[MRN No],X_VendorName,MRNDate,OrderNo,X_VendorInvoice,x_Description,x_InvoiceNo,N_FormID,X_VendorName_Ar" + xSortBy;
            // sqlCommandText = "select * from Inv_MRNDetails where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", nFormID);
            SortedList OutPut = new SortedList();

          
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count from vw_InvMRNNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and B_IsDirectMRN=1 and N_FormID=@p3 " + Searchkey + "";
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
                return Ok(_api.Error(User,e));
            }
        }
        [HttpGet("listdetails")]
        public ActionResult GetGoodsReceiveDetails(int nCompanyId, int nFnYearId, string nMRNNo, bool showAllBranch, int nBranchId, string poNo,int nFormID)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable dtGoodReceive = new DataTable();
            DataTable dtGoodReceiveDetails = new DataTable();
            DataTable dtFreightCharges = new DataTable();
            DataTable RentalSchedule = new DataTable();
            int N_GRNID = 0;
            int N_POrderID = 0;

            Params.Add("@CompanyID", nCompanyId);
            Params.Add("@YearID", nFnYearId);
            Params.Add("@TransType", "GRN");
            Params.Add("@BranchID", nBranchId);
            Params.Add("@nFormID", nFormID);
            string X_MasterSql = "";
            string X_DetailsSql = "";
            string X_FreightSql = "";
            string crieteria = "";
            string RentalScheduleSql = "";

         
            if(nFormID>0)
             {
            crieteria = " and Inv_PurchaseOrder.N_FormID = @nFormID ";
             }
            if (nMRNNo != null)
            {
                Params.Add("@GRNNo", nMRNNo);
                X_MasterSql = "select N_CompanyID,N_VendorID,N_MRNID,N_FnYearID,D_MRNDate,N_BranchID,B_YearEndProcess,B_IsDirectMRN,[MRN No] AS x_MRNNo,X_VendorName,MRNDate,OrderNo,X_VendorInvoice,x_Description,N_FreightAmt,N_CreatedUser,D_CreatedDate,N_ExchangeRate,N_CurrencyID,X_CurrencyName,OrderDate,isnull(N_Processed,0) as N_Processed,N_PurchaseID,X_InvoiceNo,X_ProjectCode,X_ProjectName,N_ProjectID,N_FormID,N_Decimal,X_VendorName_Ar,N_POrderid,N_DivisionID,X_DivisionName,N_StatusID,N_LastActionID from vw_InvMRNNo_Search where N_CompanyID=@CompanyID and [MRN No]=@GRNNo and N_FnYearID=@YearID " + (showAllBranch ? "" : " and  N_BranchId=@BranchID");
            }
            if (poNo != null)
            {
                X_MasterSql = "Select Inv_PurchaseOrder.*,Inv_Location.X_LocationName,Acc_CurrencyMaster.X_CurrencyName,Acc_CurrencyMaster.N_Decimal,Inv_DivisionMaster.X_DivisionName,Inv_Vendor.* from Inv_PurchaseOrder Inner Join Inv_Vendor On Inv_PurchaseOrder.N_VendorID=Inv_Vendor.N_VendorID and Inv_PurchaseOrder.N_CompanyID=Inv_Vendor.N_CompanyID and Inv_PurchaseOrder.N_FnYearID=Inv_Vendor.N_FnYearID LEFT OUTER JOIN Inv_DivisionMaster ON Inv_PurchaseOrder.N_DivisionID = Inv_DivisionMaster.N_DivisionID AND Inv_PurchaseOrder.N_CompanyID = Inv_DivisionMaster.N_CompanyID LEFT OUTER JOIN Inv_Location ON Inv_Location.N_LocationID=Inv_PurchaseOrder.N_LocationID LEFT OUTER JOIN   Acc_CurrencyMaster ON Inv_PurchaseOrder.N_CompanyID = Acc_CurrencyMaster.N_CompanyID AND Inv_PurchaseOrder.N_CurrencyID = Acc_CurrencyMaster.N_CurrencyID Where Inv_PurchaseOrder.N_CompanyID=" + nCompanyId + " and X_POrderNo='" + poNo + "' "+crieteria+" and isNull(Inv_PurchaseOrder.B_IsSaveDraft, 0)<>1";
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dtGoodReceive = dLayer.ExecuteDataTable(X_MasterSql, Params, connection);
                    if (dtGoodReceive.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                    dtGoodReceive = _api.Format(dtGoodReceive, "Master");

                    SortedList Status = StatusSetup(dtGoodReceive, connection);
                    dtGoodReceive = myFunctions.AddNewColumnToDataTable(dtGoodReceive, "TxnProcessStatus", typeof(SortedList), Status);

                    if (poNo != null)
                    {
                        N_POrderID = myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_POrderid"].ToString());
                    }
                    else
                    {
                        N_GRNID = myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_MRNID"].ToString());

                    }
                    if (N_GRNID != 0)
                    {
                        X_DetailsSql = "Select * from vw_InvMRNDetails  Where vw_InvMRNDetails.N_CompanyID=@CompanyID and vw_InvMRNDetails.N_MRNID=" + N_GRNID + (showAllBranch ? "" : " and vw_InvMRNDetails.N_BranchId=@BranchID");
                    }
                    if (N_POrderID != 0)
                    {
                        X_DetailsSql = "Select *,dbo.SP_Cost(vw_POMrn_PendingDetail.N_ItemID,vw_POMrn_PendingDetail.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_POMrn_PendingDetail.N_ItemID,vw_POMrn_PendingDetail.N_CompanyID) As N_UnitSPrice from vw_POMrn_PendingDetail Where N_CompanyID=" + nCompanyId + " and N_POrderID=" + N_POrderID + " order by N_POrderDetailsID";

                        RentalScheduleSql = "SELECT * FROM  vw_RentalScheduleItems  Where N_CompanyID="+ nCompanyId +" and N_TransID="+ N_POrderID +" and N_FormID=@nFormID";
                        RentalSchedule = dLayer.ExecuteDataTable(RentalScheduleSql, Params, connection);
                        RentalSchedule = _api.Format(RentalSchedule, "RentalSchedule");

                    }

                    SqlTransaction transaction = connection.BeginTransaction();
                    if (dtGoodReceive.Columns.Contains("N_MRNID") && myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_MRNID"].ToString()) > 0) 
                    {
                        int nGRNID = myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_MRNID"].ToString());
                        dtGoodReceive = myFunctions.AddNewColumnToDataTable(dtGoodReceive, "isTimesheetDone", typeof(bool), false);

                        if (myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_FormID"].ToString()) == 1593)
                        {
                            DataTable detailsData = dLayer.ExecuteDataTable("select N_MRNDetailsID from Inv_MRNDetails where N_CompanyID="+nCompanyId+" and N_MRNID="+nGRNID+" order by N_MRNDetailsID", Params, connection, transaction);
                            DataTable itemData = dLayer.ExecuteDataTable("select N_MRNDetailID from Inv_ServiceTimesheetItems where N_CompanyID="+nCompanyId+" and N_MRNID="+nGRNID+" group by N_MRNDetailID order by N_MRNDetailID", Params, connection, transaction);

                            foreach (DataRow Avar in detailsData.Rows)
                            {
                                foreach (DataRow Kvar in itemData.Rows)
                                {
                                    if (myFunctions.getIntVAL(Avar["N_MRNDetailsID"].ToString()) == myFunctions.getIntVAL(Kvar["N_MRNDetailID"].ToString()))
                                    {
                                        dtGoodReceive.Rows[0]["isTimesheetDone"] = true;
                                    } else {
                                        dtGoodReceive.Rows[0]["isTimesheetDone"] = false;
                                    }
                                }
                            }
                        }

                        object objReturnProcessed = dLayer.ExecuteScalar("select ISNULL(N_MRNID,0) AS N_MRNID from vw_MRNToMRNReturn where N_MRNID="+nGRNID+" and N_CompanyID="+nCompanyId+" and N_FnYearID="+nFnYearId+" group by N_MRNID having SUM(N_BalanceQty)<=0" , connection, transaction);
                        if (objReturnProcessed == null)
                            objReturnProcessed = 0;

                        dtGoodReceive = myFunctions.AddNewColumnToDataTable(dtGoodReceive, "N_ReturnProcessed", typeof(int), 0);

                        if (myFunctions.getIntVAL(objReturnProcessed.ToString()) != 0) {
                            dtGoodReceive.Rows[0]["N_ReturnProcessed"] = 1;
                        };
                   }
                    transaction.Commit();

                    dtGoodReceiveDetails = dLayer.ExecuteDataTable(X_DetailsSql, Params, connection);
                    dtGoodReceiveDetails = _api.Format(dtGoodReceiveDetails, "Details");

            

                    X_FreightSql = "Select *,X_ShortName as X_CurrencyName FROM vw_InvPurchaseFreights WHERE N_PurchaseID=" + N_GRNID;
                    dtFreightCharges = dLayer.ExecuteDataTable(X_FreightSql, Params, connection);
                    dtFreightCharges = _api.Format(dtFreightCharges, "freightCharges");
                    
                    if (N_POrderID != 0)
                    {
                    }
                    else
                    {
                        DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_VendorID"].ToString()), myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_MRNID"].ToString()), myFunctions.getIntVAL(dtGoodReceive.Rows[0]["n_FormID"].ToString()), myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_FnYearID"].ToString()), User, connection);
                        Attachments = _api.Format(Attachments, "attachments");
                        dt.Tables.Add(Attachments);

                        RentalScheduleSql = "SELECT * FROM  vw_RentalScheduleItems  Where N_CompanyID="+ nCompanyId +" and N_TransID="+ N_GRNID +" and N_FormID=@nFormID";
                        RentalSchedule = dLayer.ExecuteDataTable(RentalScheduleSql, Params, connection);
                        RentalSchedule = _api.Format(RentalSchedule, "RentalSchedule");
                       
                    }

                    dt.Tables.Add(dtGoodReceive);
                     dt.Tables.Add(dtGoodReceiveDetails);
                     dt.Tables.Add(dtFreightCharges);
                     dt.Tables.Add(RentalSchedule);
                  
                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpGet("fillfreight")]
        public ActionResult fillMRNFreight(int nCompanyId, int nFnYearId, int nMrnId)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable dtFreightList = new DataTable();

            Params.Add("@CompanyID", nCompanyId);
            Params.Add("@YearID", nFnYearId);
            Params.Add("@MRNID", nMrnId);
            string x_SqlQurery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    x_SqlQurery = "select * from vw_InvMRNFreights where N_CompanyID = @CompanyID and N_FnYearID = @YearID and N_MRNID = @MRNID";
                    dtFreightList = dLayer.ExecuteDataTable(x_SqlQurery, Params, connection);
                    if (dtFreightList.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                    dtFreightList = _api.Format(dtFreightList, "Master");
                    dt.Tables.Add(dtFreightList);
                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }


        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            DataTable MasterTable;
            DataTable DetailTable;
            DataTable GRNFreight;
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            GRNFreight = ds.Tables["freightCharges"];
            DataTable Attachment = ds.Tables["attachments"];
            DataTable rentalItem = ds.Tables["segmentTable"];
            SortedList Params = new SortedList();
            // Auto Gen
            string GRNNo = "";
            DataRow masterRow = MasterTable.Rows[0];
            var values = masterRow["X_MRNNo"].ToString();
            int N_GRNID = 0;
            int N_SaveDraft = 0;
            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nFnYearID = myFunctions.getIntVAL(masterRow["n_FnYearId"].ToString());
            int n_OldFnYearId =myFunctions.getIntVAL(masterRow["n_FnYearId"].ToString());
            int n_POrderID = myFunctions.getIntVAL(masterRow["n_POrderID"].ToString());
            int n_FormID = myFunctions.getIntVAL(masterRow["n_FormID"].ToString());
             String xButtonAction="";
             int nDivisionID = 0;
                    if (MasterTable.Columns.Contains("n_DivisionID"))
                    {
                       nDivisionID=myFunctions.getIntVAL(masterRow["n_DivisionID"].ToString());
                    }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();
                    N_GRNID = myFunctions.getIntVAL(masterRow["N_MRNID"].ToString());
                    int N_VendorID = myFunctions.getIntVAL(masterRow["n_VendorID"].ToString());

                    if (!myFunctions.CheckActiveYearTransaction(nCompanyID, nFnYearID, DateTime.ParseExact(MasterTable.Rows[0]["D_MRNDate"].ToString(), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture), dLayer, connection, transaction))
                    {
                        object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID="+nCompanyID+" and convert(date ,'" + MasterTable.Rows[0]["D_MRNDate"].ToString() + "') between D_Start and D_End", connection, transaction);
                        if (DiffFnYearID != null)
                        {
                            MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
                            nFnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());
                            //QueryParams["@nFnYearID"] = nFnYearID;

                            
                           SortedList QueryParams = new SortedList();
                            QueryParams["@nFnYearID"] = nFnYearID;
                            QueryParams["@nCompanyID"] = nCompanyID;
                            QueryParams["@N_VendorID"] = N_VendorID;
                            
                              SortedList PostingParam = new SortedList();
                              PostingParam.Add("N_PartyID", N_VendorID);
                              PostingParam.Add("N_FnyearID", nFnYearID);
                              PostingParam.Add("N_CompanyID", nCompanyID);
                              PostingParam.Add("X_Type", "vendor");


                             object vendorCount = dLayer.ExecuteScalar("Select count(*) From Inv_Vendor where N_FnYearID=@nFnYearID and N_CompanyID=@nCompanyID and N_VendorID=@N_VendorID", QueryParams, connection, transaction);
                      
                               if(myFunctions.getIntVAL(vendorCount.ToString())==0){
                                try 
                                  {
                                     dLayer.ExecuteNonQueryPro("SP_CratePartyBackYear", PostingParam, connection, transaction);
                                  }
                                  catch (Exception ex)
                                  {
                                    transaction.Rollback();
                                     throw ex;
                                  }
                                  }
                        }
                        else
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
                        }
                    }

                object B_YearEndProcess=dLayer.ExecuteScalar("Select B_YearEndProcess from Acc_FnYear Where N_CompanyID="+nCompanyID+" and convert(date ,'" + MasterTable.Rows[0]["D_MRNDate"].ToString() + "') between D_Start and D_End", connection, transaction);
                 if(myFunctions.getBoolVAL(B_YearEndProcess.ToString()))
                 {
                     return Ok(_api.Error(User, "Year Closed"));
                 }


                    if (N_GRNID > 0)
                    {
                        if (CheckProcessed(N_GRNID))
                            return Ok(_api.Error(User,"Transaction Started!"));
                    }
                    if (values == "@Auto")
                    {
                        N_SaveDraft = myFunctions.getIntVAL(masterRow["b_IsSaveDraft"].ToString());
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", n_FormID);
                        Params.Add("N_BranchID", masterRow["n_BranchId"].ToString());

                        GRNNo = dLayer.GetAutoNumber("Inv_MRN", "X_MRNNo", Params, connection, transaction);
                          xButtonAction="Insert"; 
                        if (GRNNo == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,"Unable to generate Document Number"));
                        }
                        MasterTable.Rows[0]["X_MRNNo"] = GRNNo;
                    }

                    GRNNo = MasterTable.Rows[0]["X_MRNNo"].ToString();

                    if (N_GRNID > 0)
                    {
                        if (n_POrderID > 0)
                        {
                            dLayer.ExecuteScalar("Update Inv_PurchaseOrder Set N_Processed=1  Where N_POrderID=" + n_POrderID + " and N_CompanyID=" + nCompanyID, connection, transaction);
                           
                        }

                        SortedList StockUpdateParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
	                            {"N_TransID",N_GRNID},
	                            {"X_TransType", "GRN"}};

                        dLayer.ExecuteNonQueryPro("SP_StockDeleteUpdate", StockUpdateParams, connection, transaction);

                        SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",masterRow["n_CompanyId"].ToString()},
                                {"X_TransType","GRN"},
                                {"N_VoucherID",N_GRNID},
                                {"N_UserID",nUserID},
                                {"X_SystemName","WebRequest"},
                                {"B_MRNVisible","0"}};

                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", DeleteParams, connection, transaction);
                               xButtonAction="Update"; 
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,ex));
                        }
                    }

                    N_GRNID = dLayer.SaveData("Inv_MRN", "N_MRNID", MasterTable, connection, transaction);

                    if (N_GRNID > 0)
                    {
                        if (n_FormID == 1593)
                        {
                            object serviceSheetID = dLayer.ExecuteScalar("select N_ServiceSheetID from Inv_ServiceTimesheet where N_CompanyID="+nCompanyID+" and N_POID in (SELECT Inv_PurchaseOrder.N_POrderID FROM Inv_PurchaseOrder INNER JOIN Inv_MRN ON Inv_PurchaseOrder.N_POrderID = Inv_MRN.N_POrderid AND Inv_PurchaseOrder.N_CompanyID = Inv_MRN.N_CompanyID where Inv_MRN.N_CompanyID="+nCompanyID+" and Inv_MRN.N_MRNID="+N_GRNID+") and convert(date ,'" + MasterTable.Rows[0]["d_MRNDate"].ToString() + "') BETWEEN D_DateFrom AND D_DateTo", Params, connection, transaction);
                            if (serviceSheetID == null) serviceSheetID = 0;
                            if (myFunctions.getIntVAL(serviceSheetID.ToString()) > 0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User,"Unable to save,Timesheet Processed for the date"));
                            }
                        }
                    }

                    if (N_GRNID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save Goods Receive Note!!"));
                    }
                    // for (int j = 0; j < DetailTable.Rows.Count; j++)
                    // {
                    //     DetailTable.Rows[j]["N_MRNID"] = N_GRNID;
                    // }
                    // int N_MRNDetailsID = dLayer.SaveData("Inv_MRNDetails", "N_MRNDetailsID", DetailTable, connection, transaction);

                    
                   if(nDivisionID>0)
                    {
                    if (DetailTable.Rows.Count > 0)
                    {
                     object xLevelsql = dLayer.ExecuteScalar("select X_LevelPattern from Inv_DivisionMaster where N_CompanyID=" + nCompanyID + " and N_DivisionID=" + nDivisionID + " and N_GroupID=0", Params, connection,transaction);
                      
                       if (xLevelsql != null && xLevelsql.ToString() != "")
                        {
                        for (int j = 0; j < DetailTable.Rows.Count; j++)
                        {

                            //  detailTable.Rows[j]["N_SalesId"] = N_SalesID;
                            object xLevelPattern = dLayer.ExecuteScalar("SELECT  Inv_DivisionMaster.X_LevelPattern FROM  Inv_DivisionMaster LEFT OUTER JOIN    Inv_ItemCategory ON Inv_DivisionMaster.N_DivisionID = Inv_ItemCategory.N_DivisionID AND Inv_DivisionMaster.N_CompanyID = Inv_ItemCategory.N_CompanyID RIGHT OUTER JOIN "+
                            "Inv_ItemMaster ON Inv_ItemCategory.N_CompanyID = Inv_ItemMaster.N_CompanyID AND Inv_ItemCategory.N_CategoryID = Inv_ItemMaster.N_CategoryID  where Inv_ItemMaster.N_ItemID="+ DetailTable.Rows[j]["N_ItemID"]+" and Inv_ItemMaster.N_CompanyID="+nCompanyID+" ", Params, connection,transaction);
                           // object xLevelPattern = dLayer.ExecuteScalar("select X_LevelPattern from Acc_CostCentreMaster where N_CompanyID=" + N_CompanyID + " and N_CostCentreID=" + nDivisionID + " and N_GroupID=0", Params, connection);
                             if (xLevelsql.ToString() != xLevelPattern.ToString().Substring(0, 3))
                             {
                                return Ok(_api.Error(User, "Unable To save!...Division Mismatch"));
                             }
                           
                        }
                        }
                    
                    }
                    }

                    int N_MRNDetailsID = 0;
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_MRNID"] = N_GRNID;

                        N_MRNDetailsID = dLayer.SaveDataWithIndex("Inv_MRNDetails", "N_MRNDetailsID", "", "", j, DetailTable, connection, transaction);

                        if (N_MRNDetailsID > 0)
                        {
                            for (int k = 0; k < rentalItem.Rows.Count; k++)
                            {
                                if (myFunctions.getIntVAL(rentalItem.Rows[k]["rowID"].ToString()) == j)
                                {

                                    rentalItem.Rows[k]["n_TransID"] = N_GRNID;
                                    rentalItem.Rows[k]["n_TransDetailsID"] = N_MRNDetailsID;

                                    rentalItem.AcceptChanges();
                                }
                                rentalItem.AcceptChanges();
                            }
                            rentalItem.AcceptChanges();
                        }
                        DetailTable.AcceptChanges();
                    }

                    if (rentalItem.Columns.Contains("rowID"))
                    rentalItem.Columns.Remove("rowID");
                    rentalItem.AcceptChanges();

                    if(rentalItem.Rows.Count > 0)
                    {
                        if (N_GRNID > 0)
                        {
                            int FormID = myFunctions.getIntVAL(rentalItem.Rows[0]["n_FormID"].ToString());
                            dLayer.ExecuteScalar("delete from Inv_RentalSchedule where N_TransID=" + N_GRNID.ToString() + " and N_FormID="+ FormID + " and N_CompanyID=" + nCompanyID, connection, transaction);
                        }
                        dLayer.SaveData("Inv_RentalSchedule", "N_ScheduleID", rentalItem, connection, transaction);

                    }

                    if (N_MRNDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save Goods Receive Note!"));
                    }
                    if (N_SaveDraft == 0)
                    {
                        try
                        {
                            if (n_POrderID > 0)
                            {
                                dLayer.ExecuteScalar("Update Inv_PurchaseOrder Set N_Processed=1  Where N_POrderID=" + n_POrderID + " and N_CompanyID=" + nCompanyID, connection, transaction);

                            }
                            
                            SortedList StockPosting = new SortedList();
                            StockPosting.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
                            StockPosting.Add("N_MRNID", N_GRNID);
                            StockPosting.Add("N_UserID", nUserID);
                            StockPosting.Add("X_SystemName", "ERP Cloud");
                            dLayer.ExecuteNonQueryPro("[SP_Inv_AllocateNegStock_MRN]", StockPosting, connection, transaction);

                            SortedList PostingParam = new SortedList();
                            PostingParam.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
                            PostingParam.Add("X_InventoryMode", "GRN");
                            PostingParam.Add("N_InternalID", N_GRNID);
                            PostingParam.Add("N_UserID", nUserID);
                            PostingParam.Add("X_SystemName", "ERP Cloud");
                            dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Purchase_Posting", PostingParam, connection, transaction);

                            SortedList StockOutParam = new SortedList();
                            StockOutParam.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());

                            dLayer.ExecuteNonQueryPro("SP_StockOutUpdate", StockOutParam, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,"Error Occurred in MRN Posting , Please check account mapping"));
                        }

                        if (n_POrderID > 0)
                        {
                            // if(!myFunctions.UpdateTxnStatus(nCompanyID,n_POrderID,82,false,dLayer,connection,transaction))
                            //         {
                            //             transaction.Rollback();
                            //             return Ok(_api.Error(User, "Unable To Update Txn Status"));
                            //         }

                            if (myFunctions.getIntVAL(masterRow["n_FormID"].ToString()) == 1593)
                            {
                                if(!myFunctions.UpdateTxnStatus(nCompanyID,n_POrderID,1586,false,dLayer,connection,transaction))
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                }
                            }
                            else
                            {
                                if(!myFunctions.UpdateTxnStatus(nCompanyID,n_POrderID,82,false,dLayer,connection,transaction))
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                }
                            }
                        }
                        if (N_GRNID > 0)
                        {
                            if (myFunctions.getIntVAL(masterRow["n_FormID"].ToString()) == 1593)
                            {
                                if(!myFunctions.UpdateTxnStatus(nCompanyID,N_GRNID,1593,true,dLayer,connection,transaction))
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                }
                            }
                        }
                    }

                                                        //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(nFnYearID,N_GRNID,GRNNo,555,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);


                    SortedList VendorParams = new SortedList();
                    VendorParams.Add("@nVendorID", N_VendorID);
                    DataTable VendorInfo = dLayer.ExecuteDataTable("Select X_VendorCode,X_VendorName from Inv_Vendor where N_VendorID=@nVendorID", VendorParams, connection, transaction);
                    if (VendorInfo.Rows.Count > 0)
                    {
                        try
                        {
                            myAttachments.SaveAttachment(dLayer, Attachment, GRNNo, N_GRNID, VendorInfo.Rows[0]["X_VendorName"].ToString().Trim(), VendorInfo.Rows[0]["X_VendorCode"].ToString(), N_VendorID, "Vendor Document", User, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,ex));
                        }
                    }

                    if (GRNFreight.Rows.Count > 0)
                    {
                        if (!GRNFreight.Columns.Contains("N_PurchaseID"))
                        {
                            GRNFreight.Columns.Add("N_PurchaseID");
                        }
                        foreach (DataRow var in GRNFreight.Rows)
                        {
                            var["N_PurchaseID"] = N_GRNID;
                        }
                        dLayer.SaveData("Inv_PurchaseFreights", "N_PurchaseFreightID", GRNFreight, connection, transaction);
                    }
                    transaction.Commit();
                }
                SortedList Result = new SortedList();
                Result.Add("N_MRNID", N_GRNID);
                Result.Add("X_MRNNo", GRNNo);

                if (n_FormID == 1593)
                    return Ok(_api.Success(Result, "Rental MRN Successfully Saved"));
                else
                    return Ok(_api.Success(Result, "Material Receipt Note Successfully Saved"));
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }
        private bool CheckProcessed(int nGRNID)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            object Processed = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlCommand = "Select Isnull(Count(Inv_PurchaseDetails.N_RsID),0) FROM Inv_Purchase INNER JOIN Inv_PurchaseDetails ON Inv_Purchase.N_PurchaseID = Inv_PurchaseDetails.N_PurchaseID Where  Inv_Purchase.N_CompanyID=@p1 and Inv_PurchaseDetails.N_RsID=@p2 and isNull(B_IsSaveDraft, 0) = 0";
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", nGRNID);
                Processed = dLayer.ExecuteScalar(sqlCommand, Params, connection);

            }
            if (Processed != null)
            {
                if (myFunctions.getIntVAL(Processed.ToString()) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        //Delete....
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nGRNID, int nFormID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            int Results = 0;

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nTransID", nGRNID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    string Sql = "select N_VendorID,N_FnYearID,X_MRNNo from Inv_MRN where N_MRNID=@nTransID and N_CompanyID=@nCompanyID";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                     string xButtonAction = " Delete";
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User,"Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    int VendorID = myFunctions.getIntVAL(TransRow["N_VendorID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(TransRow["N_FnYearID"].ToString());

                    SqlTransaction transaction = connection.BeginTransaction();

                    object objReturnProcessed = dLayer.ExecuteScalar("Select Isnull(N_MRNReturnID,0) from Inv_MRNReturn where N_CompanyID=" + nCompanyID + " and N_MRNID=" + nGRNID , connection, transaction);
                    if (objReturnProcessed == null)
                        objReturnProcessed = 0;

                    if (myFunctions.getIntVAL(objReturnProcessed.ToString()) != 0)
                        return Ok(_api.Error(User, "Return processed! Unable to delete"));

                    object objPurchaseProcessed = dLayer.ExecuteScalar("Select Isnull(N_PurchaseID,0) from Inv_Purchase where N_CompanyID=" + nCompanyID + " and N_RsID=" + nGRNID + " and isNull(B_IsSaveDraft, 0) = 0", connection, transaction);
                    if (objPurchaseProcessed == null)
                        objPurchaseProcessed = 0;

                    if (myFunctions.getIntVAL(objPurchaseProcessed.ToString()) == 0)
                    {

                          SortedList StockUpdateParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
	                            {"N_TransID",nGRNID},
	                            {"X_TransType", "GRN"}};

                        dLayer.ExecuteNonQueryPro("SP_StockDeleteUpdate", StockUpdateParams, connection, transaction);
                             //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(myFunctions.getIntVAL( nFnYearID.ToString()),nGRNID,TransRow["X_MRNNo"].ToString(),555,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);


                        SortedList DeleteParams = new SortedList(){
                            {"N_CompanyID",nCompanyID},
                            {"X_TransType","GRN"},
                            {"N_VoucherID",nGRNID},
                            {"N_UserID",nUserID},
                            {"X_SystemName","WebRequest"},
                            {"@B_MRNVisible","0"}};
                        DataTable DetailTable = dLayer.ExecuteDataTable("SELECT Inv_PurchaseOrderDetails.N_POrderID FROM Inv_PurchaseOrderDetails INNER JOIN Inv_MRNDetails ON Inv_PurchaseOrderDetails.N_CompanyID = Inv_MRNDetails.N_CompanyID AND Inv_PurchaseOrderDetails.N_POrderDetailsID = Inv_MRNDetails.N_POrderDetailsID where Inv_MRNDetails.N_CompanyID=@nCompanyID and Inv_MRNDetails.N_MRNID=@nTransID  group by Inv_PurchaseOrderDetails.N_POrderID", ParamList, connection, transaction);
                    
                        Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", DeleteParams, connection, transaction);
                        if (Results <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,"Unable to Delete Goods Receive Note"));
                        } else {
                            dLayer.ExecuteScalar("delete from Inv_RentalSchedule where N_TransID=" + nGRNID.ToString() + " and N_FormID=1593 and N_CompanyID=" + nCompanyID, connection, transaction);
                        }

                        SortedList StockOutParam = new SortedList();
                        StockOutParam.Add("N_CompanyID", nCompanyID);

                        dLayer.ExecuteNonQueryPro("SP_StockOutUpdate", StockOutParam, connection, transaction);

                        myAttachments.DeleteAttachment(dLayer, 1, nGRNID, VendorID, nFnYearID, nFormID, User, transaction, connection);
                            int tempPOrderID=0;
                            for (int j = 0; j < DetailTable.Rows.Count; j++)
                            {
                                int n_POrderID = myFunctions.getIntVAL(DetailTable.Rows[j]["N_POrderID"].ToString());
                                if (n_POrderID > 0 && tempPOrderID!=n_POrderID)
                                {
                                    // if(!myFunctions.UpdateTxnStatus(nCompanyID,n_POrderID,82,true,dLayer,connection,transaction))
                                    // {
                                    //     transaction.Rollback();
                                    //     return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                    // }
                                    if (nFormID == 1593)
                                    {
                                        if(!myFunctions.UpdateTxnStatus(nCompanyID,n_POrderID,1586,false,dLayer,connection,transaction))
                                        {
                                            transaction.Rollback();
                                            return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                        }
                                    }
                                    else
                                    {
                                        if(!myFunctions.UpdateTxnStatus(nCompanyID,n_POrderID,82,true,dLayer,connection,transaction))
                                        {
                                            transaction.Rollback();
                                            return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                        }
                                    }
                                }
                                tempPOrderID=n_POrderID;
                            };
                        transaction.Commit();
                        if (nFormID == 1593 ) 
                        return Ok(_api.Success("Rental MRN Deleted"));
                        else 
                        return Ok(_api.Success("Material Receipt Note Deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                        if (myFunctions.getIntVAL(objPurchaseProcessed.ToString()) > 0)
                            return Ok(_api.Error(User, "Purchase invoice processed! Unable to delete"));
                        else
                            return Ok(_api.Error(User, "Unable to delete!"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("freighttype")]
        public ActionResult GetFreightType(int nCompanyID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("N_CompanyID", nCompanyID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTablePro("SP_FillFreightReasons", Params, connection);
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
                return Ok( _api.Error(User,e));
            }
        }

        private SortedList StatusSetup(DataTable dtGoodReceive, SqlConnection connection)
        {
            SortedList TxnStatus = new SortedList();
            TxnStatus.Add("Label", "");
            TxnStatus.Add("LabelColor", "");
            TxnStatus.Add("Alert", "");

            int nCompanyID = myFunctions.GetCompanyID(User);
            if (dtGoodReceive.Columns.Contains("N_MRNID")) 
            {
            int nGRNID = myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_MRNID"].ToString());
            object objPurchaseProcessed = dLayer.ExecuteScalar("Select Isnull(N_PurchaseID,0) from Inv_Purchase where N_CompanyID=" + nCompanyID + " and N_RsID=" + nGRNID + " and isNull(B_IsSaveDraft, 0) = 0", connection);
            if (objPurchaseProcessed == null)
                objPurchaseProcessed = 0;

            if (myFunctions.getIntVAL(objPurchaseProcessed.ToString()) != 0)
                {
                    TxnStatus["Label"] = "Invoice Processed";
                    TxnStatus["LabelColor"] = "Green";
                    TxnStatus["Alert"] = "";
                }

            return TxnStatus;
            }
            else 
            return null;
        }

    }
}