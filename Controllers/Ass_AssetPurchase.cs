
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
    [Route("assetPurchase")]
    [ApiController]
    public class Ass_AssetPurchase : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int N_FormID;
        public Ass_AssetPurchase(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 129;
        }


        [HttpGet("list")]
        public ActionResult GetAssetPurchaseList(int? nCompanyId, int nFnYearId, int nFormID, int nVendorID, int nBranchID, bool bAllBranchData, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            DataTable CountTable = new DataTable();
            SortedList Params = new SortedList();
            DataSet dataSet = new DataSet();
            string sqlCommandText = "";
            string sqlCondition = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([Invoice No] like '%" + xSearchkey + "%' or Vendor like '%" + xSearchkey + "%' or X_Description like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_AssetInventoryID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "invoiceNo":
                        xSortBy = "N_AssetInventoryID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            if (bAllBranchData)
            {
                if (nVendorID > 0)
                    sqlCondition = "N_CompanyID=@p1 and N_FnYearID=@p2 and N_VendorID=@p4 and isnull(N_FormID,129)=@p5";
                else
                    sqlCondition = "N_CompanyID=@p1 and N_FnYearID=@p2 and isnull(N_FormID,129)=@p5";
            }
            else
            {
                if (nVendorID > 0)
                    sqlCondition = "N_CompanyID=@p1 and N_FnYearID=@p2 and N_VendorID=@p4 and N_BranchID=@p3 and isnull(N_FormID,129)=@p5";
                else
                    sqlCondition = "N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 and isnull(N_FormID,129)=@p5";
            }


            int Count = (nPage - 1) * nSizeperpage;
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_VendorID,N_AssetInventoryID,N_FnYearID,N_BranchID,N_FormID,[Invoice No],[Invoice Date],Vendor,X_VendorInvoice,X_Description,NetAmount,X_TypeName from vw_InvAssetInventoryInvoiceNo_Search where " + sqlCondition + " " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_VendorID,N_AssetInventoryID,N_FnYearID,N_BranchID,N_FormID,[Invoice No],[Invoice Date],Vendor,X_VendorInvoice,X_Description,NetAmount,X_TypeName from vw_InvAssetInventoryInvoiceNo_Search where " + sqlCondition + " " + Searchkey + " and N_AssetInventoryID not in (select top(" + Count + ") N_AssetInventoryID from vw_InvAssetInventoryInvoiceNo_Search where " + sqlCondition + " " + xSortBy + " ) " + xSortBy;

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", nBranchID);
            Params.Add("@p4", nVendorID);
            Params.Add("@p5", nFormID);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count,sum(Cast(REPLACE(NetAmount,',','') as Numeric(10,2)) ) as TotalAmount from vw_InvAssetInventoryInvoiceNo_Search where " + sqlCondition + " " + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    string TotalSum = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                        TotalSum = drow["TotalAmount"].ToString();
                    }
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    OutPut.Add("TotalSum", TotalSum);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(OutPut));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        [HttpGet("categorylist")]
        public ActionResult ListCategory(int nFnYearID,int nFormID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText="";
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nFormID", nFormID);
            if(nFormID==1766){
             sqlCommandText = "Select * from vw_InvAssetCategory_Disp Where N_CompanyID=@nCompanyID and N_FnyearID=@nFnYearID and N_FormID=@nFormID";

            }else{
               sqlCommandText = "Select * from vw_InvAssetCategory_Disp Where N_CompanyID=@nCompanyID and N_FnyearID=@nFnYearID and ISNULL(N_FormID,0)=0";
            }
            
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


        [HttpGet("ponoList")]
        public ActionResult ListPonumber(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            string sqlCommandText = "SELECT N_CompanyID, N_FnYearID, N_POrderID, X_POrderNo, D_POrderDate, N_VendorID, N_TotAmt, B_IsSaveDraft, N_POType, X_VendorCode, X_VendorName, N_Processed,N_ProductTypeID FROM vw_Ass_POSearch WHERE (N_ProductTypeID = 276) AND (N_Processed = 0) AND (B_IsSaveDraft <> 1) and N_POrderID not in (select N_POrderID from Ass_PurchaseMaster) GROUP BY N_CompanyID, N_FnYearID, N_POrderID, X_POrderNo, D_POrderDate, N_VendorID, N_TotAmt, B_IsSaveDraft, N_POType, X_VendorCode, X_VendorName, N_Processed,N_ProductTypeID";
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


        [HttpGet("assetList")]
        public ActionResult ListAssetName(bool isRentalItem,bool isSales)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string RentalItem="";
            string salesItem="";
             Params.Add("@nCompanyID",nCompanyID);
            // Params.Add("@nFnYearID",nFnYearID);

         

          if(isRentalItem==true){
                       RentalItem=RentalItem+ " and N_ItemID NOT IN (select isnull(N_AssItemID,0) from inv_itemmaster where N_CompanyId=@nCompanyID)";
                    }

            if(isSales){
                salesItem=salesItem+ " and N_status<>2 and N_status<>5";
            }        

            string sqlCommandText = "Select * from Vw_AssetDashboard Where N_CompanyID=@nCompanyID"+RentalItem+salesItem;
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

        [HttpGet("poList")]
        public ActionResult GetPurchaseOrderList(int nLocationID, string type, string query, int PageSize, int Page)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            string Feilds = "";
            string X_Crieteria = "";
            string X_VisibleFieldList = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Params.Add("@Type", type);
                    Params.Add("@CompanyID", nCompanyId);
                    Params.Add("@LocationID", nLocationID);
                    Params.Add("@PSize", PageSize);
                    Params.Add("@Offset", Page);

                    // string pageQry = "DECLARE @PageSize INT, @Page INT Select @PageSize=@PSize,@Page=@Offset;WITH PageNumbers AS(Select ROW_NUMBER() OVER(ORDER BY N_ItemID) RowNo,";
                    // string pageQryEnd = ") SELECT * FROM    PageNumbers WHERE   RowNo BETWEEN((@Page -1) *@PageSize + 1)  AND(@Page * @PageSize)";

                    int N_POTypeID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select ISNULL(max(N_TypeId),0) From Gen_Defaults Where X_TypeName=@Type and N_DefaultId=36", Params, connection).ToString());
                    X_VisibleFieldList = myFunctions.ReturnSettings("65", "Item Search List", "X_Value", myFunctions.getIntVAL(nCompanyId.ToString()), dLayer, connection);
                    if (N_POTypeID == 121)
                    {
                        Feilds = "N_CompanyID,N_ItemID,[Item Class],B_Inactive,N_WarehouseID,N_BranchID,[Item Code],N_ItemTypeID";
                        X_Crieteria = "N_CompanyID=@CompanyID and B_Inactive=0 and [Item Code]<>'001' and ([Item Class]='Stock Item' Or [Item Class]='Non Stock Item' Or [Item Class]='Expense Item' Or [Item Class]='Assembly Item' ) and N_WarehouseID=@LocationID and N_ItemTypeID<>1";
                    }
                    else if (N_POTypeID == 122)
                    {
                        Feilds = "N_CompanyID,N_ItemID,[Item Class],B_Inactive,N_WarehouseID,N_BranchID,[Item Code],N_ItemTypeID";
                        X_Crieteria = "N_CompanyID=@CompanyID and B_Inactive=0 and [Item Code]<>'001' and ([Item Class]='Stock Item' Or [Item Class]='Non Stock Item' Or [Item Class]='Expense Item' Or [Item Class]='Assembly Item' ) and N_WarehouseID=@LocationID and N_ItemTypeID=1";
                    }

                    sqlCommandText = "select " + Feilds + "," + X_VisibleFieldList + " from vw_ItemDisplay where " + X_Crieteria + " Order by [Item Code]";
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }

                dt = _api.Format(dt);
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {

            DataTable MasterTable;
            DataTable DetailTable;
            DataTable PurchaseTable = new DataTable();
            DataTable TransactionTable = new DataTable();
            DataTable AssMasterTable = new DataTable();
            DataTable DetailTableNew = new DataTable();
            DataTable AssMasterTableNew = new DataTable();
            DataTable TransactionTableNew = new DataTable();
           DataTable DetailTableNew1 = new DataTable();
          
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            TransactionTable = ds.Tables["transactions"];
            AssMasterTable = ds.Tables["assetmaster"];
            SortedList Params = new SortedList();
            DataTable Attachment = ds.Tables["attachments"];
            
            // Auto Gen
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    string ReturnNo = "", xTransType = "";
                    int N_AssetInventoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AssetInventoryID"].ToString());
                    int FormID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FormID"].ToString());
                    int TypeID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TypeID"].ToString());
                    int POrderID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_POrderID"].ToString());
                    int N_UserID = myFunctions.GetUserID(User);
                     int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int N_LocationID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_LocationID"].ToString());

                     String xButtonAction="";
                     int nAssetType =0;
                    if (FormID == 1293) xTransType = "AR";
                    else xTransType = "AP";

                    var X_InvoiceNo = MasterTable.Rows[0]["X_InvoiceNo"].ToString();

                    if (!myFunctions.CheckActiveYearTransaction(N_CompanyID, N_FnYearID, DateTime.ParseExact(MasterTable.Rows[0]["d_InvoiceDate"].ToString(), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture), dLayer, connection, transaction))
                    {
                        object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID=" + MasterTable.Rows[0]["n_CompanyId"] + " and convert(date ,'" + MasterTable.Rows[0]["d_InvoiceDate"].ToString() + "') between D_Start and D_End", Params, connection, transaction);
                        if (DiffFnYearID != null)
                        {
                            MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
                            N_FnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());
                            Params["@nFnYearID"] = N_FnYearID;
                        }
                        else
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
                        }
                    }

                    if (X_InvoiceNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID", MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID", FormID);
                        Params.Add("N_BranchID", MasterTable.Rows[0]["n_BranchId"].ToString());
                        ReturnNo = dLayer.GetAutoNumber("Ass_PurchaseMaster", "X_InvoiceNo", Params, connection, transaction);
                         xButtonAction="Insert"; 
                        if (ReturnNo == "") { transaction.Rollback(); return Ok(_api.Warning("Unable to generate Invoice Number")); }
                        MasterTable.Rows[0]["X_InvoiceNo"] = ReturnNo;
                        X_InvoiceNo = ReturnNo;
                    }
                    if (MasterTable.Columns.Contains("n_LocationID"))
                        MasterTable.Columns.Remove("n_LocationID");

                    if (N_AssetInventoryID > 0)
                    {
                        SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString()},
                                {"X_TransType",xTransType},
                                {"N_VoucherID",N_AssetInventoryID},
                                {"N_UserID",N_UserID},
                                {"X_SystemName",System.Environment.MachineName}};
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts ", DeleteParams, connection, transaction);
                             xButtonAction="Update"; 
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }
                    }
                    TransactionTable = myFunctions.AddNewColumnToDataTable(TransactionTable, "n_Qty", typeof(float), 0.0);

                    N_AssetInventoryID = dLayer.SaveData("Ass_PurchaseMaster", "N_AssetInventoryID", MasterTable, connection, transaction);
                    if (N_AssetInventoryID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Error"));
                    }
                    int PurchaseID = 0;
                    if (!(FormID == 1293 && TypeID == 281))
                    {
                        SortedList PurchaseParams = new SortedList();
                        PurchaseParams.Add("@N_AssetInventoryID", N_AssetInventoryID);
                        PurchaseParams.Add("@xTransType", xTransType);
                        string sqlCommandText = "select N_CompanyID,N_FnYearID,0 AS N_PurchaseID,X_InvoiceNo,D_EntryDate,D_InvoiceDate,N_InvoiceAmt AS N_InvoiceAmtF,N_DiscountAmt AS N_DiscountAmtF,N_CashPaid AS N_CashPaidF,N_FreightAmt AS N_FreightAmtF,N_UserID,N_POrderID,4 AS N_PurchaseType,@xTransType AS X_TransType,N_AssetInventoryID AS N_PurchaseRefID,N_BranchID,N_InvoiceAmt,N_DiscountAmt,N_CashPaid,N_FreightAmt,N_TaxAmt,N_TaxAmtF,N_TaxCategoryId,X_VendorInvoice,N_VendorId,N_VendorId AS N_ActVendorID from Ass_PurchaseMaster where N_AssetInventoryID=@N_AssetInventoryID";

                        PurchaseTable = dLayer.ExecuteDataTable(sqlCommandText, PurchaseParams, connection, transaction);
                        PurchaseTable = _api.Format(PurchaseTable, "Purchase");

                        PurchaseID = dLayer.SaveData("Inv_Purchase", "N_PurchaseID", PurchaseTable, connection, transaction);
                        if (PurchaseID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Error"));
                        }
                    }

                    AssMasterTable = myFunctions.AddNewColumnToDataTable(AssMasterTable, "n_AssItemName", typeof(string), "");
                    AssMasterTable = myFunctions.AddNewColumnToDataTable(AssMasterTable, "n_Qty", typeof(float), 0.0);
                    AssMasterTable = myFunctions.AddNewColumnToDataTable(AssMasterTable, "x_Author",  typeof(string), "");
                    AssMasterTable = myFunctions.AddNewColumnToDataTable(AssMasterTable, "n_FormID", typeof(int), 0);
                    for (int j = 0; j < AssMasterTable.Rows.Count; j++)
                    {
                        AssMasterTable.Rows[j]["n_AssItemName"] = AssMasterTable.Rows[j]["x_AssItemName"];
                    }
                    AssMasterTable.Columns.Remove("x_AssItemName");

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_AssetInventoryID"] = N_AssetInventoryID;
                    }
                    int N_AssetInventoryDetailsID = 0, N_ActionID = 0, N_MasterID = 0;
                    if (PurchaseID > 0 || (FormID == 1293 && TypeID == 281))
                    {
                        if (FormID == 1293)
                        {
                            
                            for (int j = 0; j < DetailTable.Rows.Count; j++)
                            {
                                N_AssetInventoryDetailsID = dLayer.SaveDataWithIndex("Ass_PurchaseDetails", "N_AssetInventoryDetailsID", "", "", j, DetailTable, connection, transaction);
                                if (N_AssetInventoryDetailsID <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Error"));
                                }
                                TransactionTable.Rows[j]["N_AssetInventoryID"] = N_AssetInventoryID;
                                TransactionTable.Rows[j]["X_Reference"] = X_InvoiceNo;
                                TransactionTable.Rows[j]["N_AssetInventoryDetailsID"] = N_AssetInventoryDetailsID;
                                TransactionTable.Rows[j]["X_Type"] = "Revaluation";

                                N_ActionID = dLayer.SaveDataWithIndex("Ass_Transactions", "N_ActionID", "", "", j, TransactionTable, connection, transaction);
                                if (N_ActionID <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Error"));
                                }
                            }
                        }
                        else
                        {
                            if (POrderID > 0)
                            {
                                for (int k = 0; k < DetailTable.Rows.Count; k++)
                                {
                                    dLayer.ExecuteNonQuery("Update Inv_PurchaseOrderDetails Set N_Processed=1  Where N_POrderID=" + POrderID + " and N_POrderDetailsID=" + DetailTable.Rows[k]["N_POrderDetailsID"] + " and N_CompanyID=" + MasterTable.Rows[0]["n_CompanyId"], connection, transaction);
                                }
                                dLayer.ExecuteNonQuery("Update Inv_PurchaseOrder  Set N_Processed=1,N_PurchaseID=" + N_AssetInventoryID + "   Where N_POrderID=" + POrderID + " and N_CompanyID=" + MasterTable.Rows[0]["n_CompanyId"], connection, transaction);
                            }

                            DetailTableNew = DetailTable.Clone();
                            AssMasterTableNew = AssMasterTable.Clone();
                            TransactionTableNew = TransactionTable.Clone();
                             DetailTableNew1 = DetailTable.Clone();

                            DetailTableNew.Rows.Clear();
                            AssMasterTableNew.Rows.Clear();
                            TransactionTableNew.Rows.Clear();
                            // DetailTableNew1.Rows.Clear();

                            int nCount = DetailTable.Rows.Count;
                            for (int j = 0; j < nCount; j++)
                            {
                                float Qty = myFunctions.getFloatVAL(DetailTable.Rows[j]["N_PurchaseQty"].ToString());
                                 nAssetType = myFunctions.getIntVAL(DetailTable.Rows[j]["n_AssetType"].ToString());
                                

                                if(nAssetType == 1)
                                {
                                    if (Qty > 1)
                                    {
                                        for (int l = 0; l < Qty; l++)
                                        {
                                            var newRow = DetailTableNew.NewRow();
                                            var newRow2 = AssMasterTableNew.NewRow();
                                            var newRow3 = TransactionTableNew.NewRow();
                                            var newRow4 = DetailTableNew1.NewRow();

                                            newRow.ItemArray = DetailTable.Rows[j].ItemArray;
                                            newRow2.ItemArray = AssMasterTable.Rows[j].ItemArray;
                                            newRow3.ItemArray = TransactionTable.Rows[j].ItemArray;
                                            newRow4.ItemArray = DetailTable.Rows[j].ItemArray;

                                            DetailTableNew.Rows.Add(newRow);
                                            AssMasterTableNew.Rows.Add(newRow2);
                                            TransactionTableNew.Rows.Add(newRow3);
                                            DetailTableNew1.Rows.Add(newRow4);
                                        }
                                    }  
                                    else
                                    {
                                        var newRow = DetailTableNew.NewRow();
                                        var newRow2 = AssMasterTableNew.NewRow();
                                        var newRow3 = TransactionTableNew.NewRow();
                                        var newRow4 = DetailTableNew1.NewRow();

                                        newRow.ItemArray = DetailTable.Rows[j].ItemArray;
                                        newRow2.ItemArray = AssMasterTable.Rows[j].ItemArray;
                                        newRow3.ItemArray = TransactionTable.Rows[j].ItemArray;
                                        newRow4.ItemArray = DetailTable.Rows[j].ItemArray;

                                        DetailTableNew.Rows.Add(newRow);
                                        AssMasterTableNew.Rows.Add(newRow2);
                                        TransactionTableNew.Rows.Add(newRow3);
                                        DetailTableNew1.Rows.Add(newRow4);
                                    }
                                    
                                }                            
                                else
                                {
                                    var newRow = DetailTableNew.NewRow();
                                    var newRow2 = AssMasterTableNew.NewRow();
                                    var newRow3 = TransactionTableNew.NewRow();
                                    var newRow4 = DetailTableNew1.NewRow();

                                    newRow.ItemArray = DetailTable.Rows[j].ItemArray;
                                    newRow2.ItemArray = AssMasterTable.Rows[j].ItemArray;
                                    newRow3.ItemArray = TransactionTable.Rows[j].ItemArray;
                                    newRow4.ItemArray = DetailTable.Rows[j].ItemArray;

                                    DetailTableNew.Rows.Add(newRow);
                                    AssMasterTableNew.Rows.Add(newRow2);
                                    TransactionTableNew.Rows.Add(newRow3);
                                    DetailTableNew1.Rows.Add(newRow4);
                                }
                                
                            }
                           
                          
                            DetailTableNew.Columns.Remove("n_AssetType");

                            for (int j = 0; j < DetailTableNew.Rows.Count; j++)
                            {
                             nAssetType = myFunctions.getIntVAL(DetailTableNew1.Rows[j]["n_AssetType"].ToString());

                                if(nAssetType==1)
                                {
                                    DetailTableNew.Rows[j]["N_TaxAmt1"] = myFunctions.getVAL(DetailTableNew.Rows[j]["N_TaxAmt1"].ToString()) /myFunctions.getIntVAL(DetailTableNew.Rows[j]["N_PurchaseQty"].ToString());
                                    DetailTableNew.Rows[j]["N_PurchaseQty"] = 1;
                                    
                                }
                                //}
                                N_AssetInventoryDetailsID = dLayer.SaveDataWithIndex("Ass_PurchaseDetails", "N_AssetInventoryDetailsID", "", "", j, DetailTableNew, connection, transaction);
                                if (N_AssetInventoryDetailsID <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Error"));
                                }
                                // for (int k = 0 ;k < AssMasterTableNew.Rows.Count;k++)
                                // {
                                // AssMasterTableNew.Rows[k]["N_AssetInventoryDetailsID"]=DetailTableNew.Rows[k]["N_AssetInventoryDetailsID"];
                                AssMasterTableNew.Rows[j]["N_AssetInventoryDetailsID"] = N_AssetInventoryDetailsID;
                                int N_ItemCodeId = 0;
                                object ItemCodeID = dLayer.ExecuteScalar("Select ISNULL(MAX(N_ItemCodeId),0)+1 FROM Ass_AssetMaster  where N_CompanyID=" + AssMasterTableNew.Rows[j]["N_CompanyID"] + " and X_CategoryPrefix='" + AssMasterTableNew.Rows[j]["X_CategoryPrefix"] + "'", connection, transaction);
                                if (ItemCodeID != null)
                                    N_ItemCodeId = myFunctions.getIntVAL(ItemCodeID.ToString());

                                string X_ItemCode = "";

                                if (AssMasterTableNew.Rows[j]["X_ItemCode"].ToString().Trim() == AssMasterTableNew.Rows[j]["X_CategoryPrefix"].ToString() + "@Auto".Trim())
                                {
                                    if (AssMasterTableNew.Rows[j]["X_CategoryPrefix"].ToString() == "")
                                        X_ItemCode = "Asset" + "" + N_ItemCodeId.ToString("0000");
                                    else
                                    {
                                        X_ItemCode = AssMasterTableNew.Rows[j]["X_CategoryPrefix"].ToString() + "" + N_ItemCodeId.ToString("0000");
                                    }

                                    for (int l = 0; l < AssMasterTableNew.Rows.Count; l++)
                                    {
                                        if (X_ItemCode == AssMasterTableNew.Rows[l]["X_ItemCode"].ToString())
                                        {
                                            N_ItemCodeId += 1;

                                            if (AssMasterTableNew.Rows[j]["X_CategoryPrefix"].ToString() == "")
                                                X_ItemCode = "Asset" + "" + N_ItemCodeId.ToString("0000");
                                            else
                                                X_ItemCode = AssMasterTableNew.Rows[j]["X_CategoryPrefix"].ToString() + "" + N_ItemCodeId.ToString("0000");

                                            l = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    X_ItemCode = AssMasterTableNew.Rows[j]["X_ItemCode"].ToString();
                                }

                                if (myFunctions.getIntVAL(AssMasterTableNew.Rows[j]["N_ItemID"].ToString()) == 0)
                                {
                                    int ItemCodeCount = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select count(X_ItemCode) FROM Ass_AssetMaster  where N_CompanyID=" + AssMasterTableNew.Rows[j]["N_CompanyID"] + " and ltrim(X_ItemCode)='" + X_ItemCode + "'", connection, transaction).ToString());
                                    if (ItemCodeCount > 0)
                                    {
                                        transaction.Rollback();
                                        return Ok(_api.Error(User, "Item Exists"));
                                    }
                                }
                                AssMasterTableNew.Rows[j]["X_ItemCode"] = X_ItemCode;
                                AssMasterTableNew.Rows[j]["N_ItemCodeId"] = N_ItemCodeId;
                                AssMasterTableNew.Rows[j]["X_Author"] = DetailTableNew.Rows[j]["X_Author"];
                                if(FormID==1755){
                                  AssMasterTableNew.Rows[j]["N_FormID"]=1761;
                                }
                                
                                  if(nAssetType==1){
                                    AssMasterTableNew.Rows[j]["N_Qty"] = 1;
                                  } 
                                  else{
                                    AssMasterTableNew.Rows[j]["N_Qty"] = DetailTableNew.Rows[j]["N_PurchaseQty"];
                                  }
                                //}
                                N_MasterID = dLayer.SaveDataWithIndex("Ass_AssetMaster", "N_ItemID", "", "", j, AssMasterTableNew, connection, transaction);
                                if (N_MasterID <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Error"));
                                }

                                // for (int k = 0 ;k < TransactionTableNew.Rows.Count;k++)
                                // {
                                TransactionTableNew.Rows[j]["N_AssetInventoryID"] = N_AssetInventoryID;
                                TransactionTableNew.Rows[j]["X_Reference"] = X_InvoiceNo;
                                TransactionTableNew.Rows[j]["N_AssetInventoryDetailsID"] = N_AssetInventoryDetailsID;
                                //TransactionTableNew.Rows[j]["N_AssetInventoryDetailsID"]=DetailTableNew.Rows[k]["N_AssetInventoryDetailsID"];
                                //TransactionTableNew.Rows[j]["N_ItemId"]=AssMasterTableNew.Rows[k]["N_ItemID"];
                                TransactionTableNew.Rows[j]["N_ItemId"] = N_MasterID;
                                TransactionTableNew.Rows[j]["X_Type"] = "Acquisition";
                                 if(nAssetType==1){
                                    TransactionTableNew.Rows[j]["N_Qty"] = 1;
                                 }
                                 else{
                                    TransactionTableNew.Rows[j]["N_Qty"] = DetailTableNew.Rows[j]["N_PurchaseQty"];
                                 }
                                    
                                N_ActionID = dLayer.SaveDataWithIndex("Ass_Transactions", "N_ActionID", "", "", j, TransactionTableNew, connection, transaction);
                                if (N_ActionID <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Error"));
                                }
                            }
                            // }
                        }
                    }

                                     //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(N_FnYearID,N_AssetInventoryID,X_InvoiceNo,129,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                      SortedList assetPurchaseParams = new SortedList();
                           assetPurchaseParams.Add("@N_AssetInventoryID", N_AssetInventoryID);

                     DataTable  assetPurchaseInfo = dLayer.ExecuteDataTable("Select X_InvoiceNo from Ass_PurchaseMaster where N_AssetInventoryID=@N_AssetInventoryID", assetPurchaseParams, connection, transaction);
                        if (assetPurchaseInfo.Rows.Count > 0)
                        {
                            try
                            {
                                myAttachments.SaveAttachment(dLayer, Attachment, X_InvoiceNo, N_AssetInventoryID, assetPurchaseInfo.Rows[0]["X_InvoiceNo"].ToString(), assetPurchaseInfo.Rows[0]["X_InvoiceNo"].ToString(), N_AssetInventoryID, "Asset Purchase Document", User, connection, transaction);
                            }
                             catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, ex));
                            }
                        }

                   if(FormID==1755){
                    
                    SortedList BookParam = new SortedList();
                    BookParam.Add("N_CompanyID", MasterTable.Rows[0]["n_CompanyId"].ToString());
                    BookParam.Add("N_FnYearID", N_FnYearID);
                    BookParam.Add("N_AssetInventoryID", N_AssetInventoryID);
                    BookParam.Add("N_LocationID", N_LocationID);

                        try
                    {
                        dLayer.ExecuteNonQueryPro("SP_BookMaster_Ins", BookParam, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, ex));
                    }
                }

                    SortedList PostingParam = new SortedList();
                    PostingParam.Add("N_CompanyID", MasterTable.Rows[0]["n_CompanyId"].ToString());
                    PostingParam.Add("X_InventoryMode", xTransType);
                    PostingParam.Add("N_InternalID", N_AssetInventoryID);
                    PostingParam.Add("N_UserID", N_UserID);
                    PostingParam.Add("X_SystemName", "ERP Cloud");
                    try
                    {
                        dLayer.ExecuteNonQueryPro("SP_Acc_InventoryPosting", PostingParam, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, ex));
                    }

                    SortedList Result = new SortedList();
                    Result.Add("N_AssetInventoryID", N_AssetInventoryID);
                    Result.Add("X_InvoiceNo", X_InvoiceNo);
                    transaction.Commit();
                    return Ok(_api.Success(Result, "Asset Purchase Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("details")]
        public ActionResult AssPurchaseDetails(string xInvoiceNo, int nFnYearID, int nBranchId, bool bAllBranchData)
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

                    string Mastersql = "";
                    string DetailSql = "";
                    string strCondition = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@xInvoiceNo", xInvoiceNo);
                    Params.Add("@nFnYearID", nFnYearID);
                    Params.Add("@nBranchId", nBranchId);

                    if (bAllBranchData)
                        Mastersql = "Select * from vw_Ass_PurchaseMaster_Disp Where N_CompanyID=@nCompanyID and X_InvoiceNo=@xInvoiceNo and N_FnYearID=@nFnYearID";
                    else
                        Mastersql = "Select * from vw_Ass_PurchaseMaster_Disp Where N_CompanyID=@nCompanyID and X_InvoiceNo=@xInvoiceNo and N_FnYearID=@nFnYearID and N_BranchID=@nBranchId";

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int N_AssetInventoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AssetInventoryID"].ToString());
                    Params.Add("@N_AssetInventoryID", N_AssetInventoryID);

                    MasterTable = _api.Format(MasterTable, "Master");

                    DetailSql = "Select vw_InvAssetInventoryDetails.*  from vw_InvAssetInventoryDetails Where vw_InvAssetInventoryDetails.N_CompanyID=@nCompanyID and vw_InvAssetInventoryDetails.N_AssetInventoryID=@N_AssetInventoryID and N_FnYearID=@nFnYearID order by N_AssetInventoryDetailsID";

                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");

                     DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(MasterTable.Rows[0]["N_AssetInventoryID"].ToString()), myFunctions.getIntVAL(MasterTable.Rows[0]["N_AssetInventoryID"].ToString()), this.N_FormID, myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString()), User, connection);
                    Attachments = _api.Format(Attachments, "attachments");
                    dt.Tables.Add(Attachments);

                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(_api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("poDetails")]
        public ActionResult PODetails(string xPOrderNo, int nFnYearID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();

                    string Mastersql = "";
                    string DetailSql = "";

                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@xPOrderNo", xPOrderNo);
                    Params.Add("@nFnYearID", nFnYearID);

                    Mastersql = "select * from vw_Ass_PO where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and X_POrderNo=@xPOrderNo and N_POType=266";

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int N_POrderID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_POrderID"].ToString());
                    Params.Add("@N_POrderID", N_POrderID);

                    MasterTable = _api.Format(MasterTable, "Master");

                    DetailSql = "Select * from vw_AssetPurchasefromPO_Details Where N_CompanyID=@nCompanyID and N_POrderID=@N_POrderID and N_Processed=0";

                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(_api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID, int N_AssetInventoryID, int FormID,int nFnYearID)
        {
            int Results = 0;
             string xTransType = "";
              int N_UserID = myFunctions.GetUserID(User);

            SortedList Params = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData=new DataTable();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Params.Add("@nFnYearID",nFnYearID);
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@N_AssetInventoryID", N_AssetInventoryID);
                    string xButtonAction="Delete";
                    string X_InvoiceNo="";
                    string Sql = "select N_AssetInventoryID,X_InvoiceNo from Ass_PurchaseMaster where N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID and N_AssetInventoryID=@N_AssetInventoryID";
                     TransData=dLayer.ExecuteDataTable(Sql,Params,connection,transaction);

                   if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                     DataRow TransRow=TransData.Rows[0];
                      object n_FnYearID = dLayer.ExecuteScalar("select N_FnyearID from Ass_PurchaseMaster where N_AssetInventoryID =" + N_AssetInventoryID + " and N_CompanyID=" + nCompanyID, Params, connection,transaction);
                
                        object catCount = dLayer.ExecuteScalar("select count(*) from Ass_PurchaseDetails where N_AssetInventoryID=" + N_AssetInventoryID + " and N_AssetInventoryDetailsID in (SELECT Ass_AssetMaster.N_AssetInventoryDetailsID FROM   Ass_SalesDetails INNER JOIN   Ass_AssetMaster ON Ass_SalesDetails.N_ItemID = Ass_AssetMaster.N_ItemID AND Ass_SalesDetails.N_CompanyID = Ass_AssetMaster.N_CompanyID where Ass_AssetMaster.N_CompanyID=" + nCompanyID+")", connection, transaction);
                        catCount = catCount == null ? 0 : catCount;
                        if (myFunctions.getIntVAL(catCount.ToString()) > 0){
                            return Ok(_api.Error(User, "Already In Use !!"));
                        }

                     //Activity Log
                // string ipAddress = "";
                // if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                //     ipAddress = Request.Headers["X-Forwarded-For"];
                // else
                //     ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                //        myFunctions.LogScreenActivitys(myFunctions.getIntVAL( n_FnYearID.ToString()),N_AssetInventoryID,TransRow["X_InvoiceNo"].ToString(),129,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);

        
                    if (FormID == 1293)
                     xTransType = "AR";
                    else xTransType = "AP";
                    if (N_AssetInventoryID > 0)
                    {
                             SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",@nCompanyID},
                                {"X_TransType",xTransType},
                                {"N_VoucherID",N_AssetInventoryID},
                                {"N_UserID",N_UserID},
                                {"X_SystemName",System.Environment.MachineName}};
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts ", DeleteParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }

                        if (FormID == 129 || FormID ==1755)
                        {
                            dLayer.ExecuteNonQuery("DELETE FROM Ass_AssetMaster WHERE Ass_AssetMaster.N_CompanyID = @nCompanyID AND Ass_AssetMaster.N_AssetInventoryDetailsID IN (SELECT N_AssetInventoryDetailsID FROM Ass_PurchaseDetails WHERE Ass_PurchaseDetails.N_AssetInventoryID =@N_AssetInventoryID AND Ass_PurchaseDetails.N_CompanyID = @nCompanyID)", Params, connection, transaction);
                            dLayer.ExecuteNonQuery("DELETE FROM Ass_Transactions WHERE Ass_Transactions.N_CompanyID = @nCompanyID AND Ass_Transactions.N_AssetInventoryDetailsID IN (SELECT N_AssetInventoryDetailsID FROM Ass_PurchaseDetails WHERE Ass_PurchaseDetails.N_AssetInventoryID =@N_AssetInventoryID AND Ass_PurchaseDetails.N_CompanyID = @nCompanyID)", Params, connection, transaction);
                        }

                        dLayer.ExecuteNonQuery("delete from Ass_PurchaseDetails Where N_AssetInventoryID=@N_AssetInventoryID and N_CompanyID=@nCompanyID", Params, connection, transaction);
                        dLayer.ExecuteNonQuery("delete from Ass_PurchaseMaster Where N_AssetInventoryID=@N_AssetInventoryID and N_CompanyID=@nCompanyID", Params, connection, transaction);
                         

                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Success("unable to delete Asset Purchase "));
                    }
                    if (Results >= 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Asset Purchase deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete Asset Purchase"));
                    }


                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("bulkAssetList")]
        public ActionResult ListBulkAssetName(bool isRentalItem,bool isSales)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string RentalItem="";
            string salesItem="";
            Params.Add("@nCompanyID",nCompanyID);

            if(isRentalItem==true){
                RentalItem=RentalItem+ " and N_ItemID NOT IN (select isnull(N_AssItemID,0) from inv_itemmaster where N_CompanyId=@nCompanyID)";
            }

            if(isSales){
                salesItem=salesItem+ " and N_status<>2 and N_status<>5";
            }        

            string sqlCommandText = "Select N_AssItemName AS X_ItemName, COUNT(*) AS N_AvlQty from Ass_AssetMaster Where N_CompanyID=@nCompanyID"+RentalItem+salesItem+" GROUP BY N_AssItemName";

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
    }
}