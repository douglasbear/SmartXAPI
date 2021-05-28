
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
        public ActionResult GetAssetPurchaseList(int? nCompanyId, int nFnYearId,int nFormID,int nVendorID,int nBranchID,bool bAllBranchData, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
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
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_VendorID,N_AssetInventoryID,N_FnYearID,N_BranchID,N_FormID,[Invoice No],[Invoice Date],Vendor,X_VendorInvoice,X_Description,NetAmount,X_TypeName from vw_InvAssetInventoryInvoiceNo_Search where "+sqlCondition+" " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_VendorID,N_AssetInventoryID,N_FnYearID,N_BranchID,N_FormID,[Invoice No],[Invoice Date],Vendor,X_VendorInvoice,X_Description,NetAmount,X_TypeName from vw_InvAssetInventoryInvoiceNo_Search where "+sqlCondition+" " + Searchkey + " and N_AssetInventoryID not in (select top(" + Count + ") N_AssetInventoryID from vw_InvAssetInventoryInvoiceNo_Search where "+sqlCondition+" " + xSortBy + " ) " + xSortBy;

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
                    sqlCommandCount = "select count(*) as N_Count,sum(Cast(REPLACE(NetAmount,',','') as Numeric(10,2)) ) as TotalAmount from vw_InvAssetInventoryInvoiceNo_Search where "+sqlCondition+" " + Searchkey + "";
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
                return Ok(_api.Error(e));
            }
        }
  [HttpGet("categorylist")]
        public ActionResult ListCategory(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nFnYearID",nFnYearID);
            string sqlCommandText="Select * from vw_InvAssetCategory_Disp Where N_CompanyID=@nCompanyID and N_FnyearID=@nFnYearID";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
                return Ok(_api.Error(e));
            }
        }


        [HttpGet("ponoList")]
        public ActionResult ListPonumber(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nFnYearID",nFnYearID);
            string sqlCommandText="Select * from vw_Ass_POSearch Where N_POType=266 and N_Processed=0 and B_IsSaveDraft <> 1";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
                return Ok(_api.Error(e));
            }
        }
 
        
        [HttpGet("assetList")]
        public ActionResult ListAssetName(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nFnYearID",nFnYearID);
            string sqlCommandText="Select * from Vw_AssetDashboard Where N_CompanyID=@nCompanyID and N_FnyearID=@nFnYearID";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
                return Ok(_api.Error(e));
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

                    int N_POTypeID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select ISNULL(N_TypeId,0) From Gen_Defaults Where X_TypeName=@Type and N_DefaultId=36", Params, connection).ToString());
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
                return Ok(_api.Error(e));
            }
        }

        [HttpPost("Save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            
            DataTable MasterTable;
            DataTable DetailTable;
            DataTable PurchaseTable = new DataTable();
            DataTable TransactionTable = new DataTable();
            DataTable AssMasterTable = new DataTable();
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            SortedList Params = new SortedList();
            // Auto Gen
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction=connection.BeginTransaction();
                    string ReturnNo="",xTransType="";
                    int N_AssetInventoryID =myFunctions.getIntVAL(MasterTable.Rows[0]["N_AssetInventoryID"].ToString());
                    int FormID =myFunctions.getIntVAL(MasterTable.Rows[0]["N_FormID"].ToString());
                    int TypeID =myFunctions.getIntVAL(MasterTable.Rows[0]["N_TypeID"].ToString());
                    int POrderID =myFunctions.getIntVAL(MasterTable.Rows[0]["N_POrderID"].ToString());
                    int N_UserID=myFunctions.GetUserID(User);

                    if(FormID==1293) xTransType="AR";
                    else xTransType="AP";

                    var X_InvoiceNo = MasterTable.Rows[0]["X_InvoiceNo"].ToString();
                    if(X_InvoiceNo=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID",MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID",FormID);
                        Params.Add("N_BranchID",MasterTable.Rows[0]["n_BranchId"].ToString());
                        ReturnNo =  dLayer.GetAutoNumber("Ass_PurchaseMaster","X_InvoiceNo", Params,connection,transaction);
                        if(ReturnNo==""){transaction.Rollback(); return Ok(_api.Warning("Unable to generate Invoice Number"));}
                        MasterTable.Rows[0]["X_InvoiceNo"] = ReturnNo;
                    }

                    if(N_AssetInventoryID>0)
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
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(ex));
                        }
                    }

                    N_AssetInventoryID=dLayer.SaveData("Ass_PurchaseMaster","N_AssetInventoryID",MasterTable,connection,transaction);                    
                    if(N_AssetInventoryID<=0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Error"));
                    }
                    int PurchaseID=0;
                    if (!(FormID == 1293 && TypeID == 281))
                    {
                        SortedList PurchaseParams = new SortedList();
                        PurchaseParams.Add("@N_AssetInventoryID",N_AssetInventoryID);
                        PurchaseParams.Add("@xTransType",xTransType);
                        string sqlCommandText="select N_CompanyID,N_FnYearID,0 AS N_PurchaseID,X_InvoiceNo,D_EntryDate,D_InvoiceDate,N_InvoiceAmt AS N_InvoiceAmtF,N_DiscountAmt AS N_DiscountAmtF,N_CashPaid AS N_CashPaidF,N_FreightAmt AS N_FreightAmtF,N_UserID,N_POrderID,4 AS N_PurchaseType,@xTransType AS X_TransType,N_AssetInventoryID AS N_PurchaseRefID,N_BranchID,N_InvoiceAmt,N_DiscountAmt,N_CashPaid,N_FreightAmt,N_TaxAmt,N_TaxAmtF,N_TaxCategoryId,X_VendorInvoice,N_VendorId,N_VendorId AS N_ActVendorID from Ass_PurchaseMaster where N_AssetInventoryID=@N_AssetInventoryID";

                        PurchaseTable = dLayer.ExecuteDataTable(sqlCommandText, PurchaseParams, connection);
                        PurchaseTable = _api.Format(PurchaseTable, "Purchase");

                        PurchaseID=dLayer.SaveData("Inv_Purchase","N_PurchaseID",PurchaseTable,connection,transaction); 
                        if(PurchaseID<=0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Error"));
                        }
                    }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                    {
                        DetailTable.Rows[j]["N_AssetInventoryID"]=N_AssetInventoryID;
                    }
                    int N_AssetInventoryDetailsID=0,N_ActionID=0,N_MasterID=0;
                    if (PurchaseID > 0 ||(FormID ==1293 && TypeID ==281))
                    {
                        if(FormID ==1293)
                        {
                            N_AssetInventoryDetailsID=dLayer.SaveData("Ass_PurchaseDetails","N_AssetInventoryDetailsID",DetailTable,connection,transaction);                    
                            if(N_AssetInventoryDetailsID<=0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error("Error"));
                            }
                            for (int k = 0 ;k < TransactionTable.Rows.Count;k++)
                            {
                                TransactionTable.Rows[k]["N_AssetInventoryID"]=N_AssetInventoryID;
                                TransactionTable.Rows[k]["X_Reference"]=ReturnNo;
                                TransactionTable.Rows[k]["N_AssetInventoryDetailsID"]=DetailTable.Rows[k]["N_AssetInventoryDetailsID"];
                            }
                            N_ActionID=dLayer.SaveData("Ass_Transactions","N_ActionID",TransactionTable,connection,transaction);                    
                            if(N_ActionID<=0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error("Error"));
                            }
                        }
                        else
                        {
                            if(POrderID>0)
                            {
                                for (int k = 0 ;k < DetailTable.Rows.Count;k++)
                                {
                                    dLayer.ExecuteNonQuery("Update Inv_PurchaseOrderDetails Set N_Processed=1  Where N_POrderID=" + POrderID + " and N_POrderDetailsID=" + DetailTable.Rows[k]["N_POrderDetailsID"] + " and N_CompanyID=" + MasterTable.Rows[0]["n_CompanyId"], connection, transaction);                                  
                                }
                                dLayer.ExecuteNonQuery("Update Inv_PurchaseOrder  Set N_Processed=1,N_PurchaseID="+N_AssetInventoryID+"   Where N_POrderID=" + POrderID + " and N_CompanyID=" + MasterTable.Rows[0]["n_CompanyId"], connection, transaction);                                  
                                
                                int nCount=DetailTable.Rows.Count;
                                for (int j = 0 ;j < nCount;j++)
                                {
                                    int Qty=myFunctions.getIntVAL(DetailTable.Rows[j]["N_PurchaseQty"].ToString());
                                    if(Qty>1)
                                    {
                                        for (int l = 0 ;l < Qty-1;l++)
                                        {
                                            DetailTable.Rows.Add(DetailTable.Rows[j]);
                                            TransactionTable.Rows.Add(TransactionTable.Rows[j]);
                                            AssMasterTable.Rows.Add(AssMasterTable.Rows[j]);
                                        }
                                    }
                                }
                                for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                                {
                                    DetailTable.Rows[j]["N_PurchaseQty"]=1;
                                }
                                N_AssetInventoryDetailsID=dLayer.SaveData("Ass_PurchaseDetails","N_AssetInventoryDetailsID",DetailTable,connection,transaction);                    
                                if(N_AssetInventoryDetailsID<=0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error("Error"));
                                }
                                for (int k = 0 ;k < AssMasterTable.Rows.Count;k++)
                                {
                                    AssMasterTable.Rows[k]["N_AssetInventoryDetailsID"]=DetailTable.Rows[k]["N_AssetInventoryDetailsID"];
                                    int N_ItemCodeId = 0;
                                    object ItemCodeID= dLayer.ExecuteScalar("Select ISNULL(MAX(N_ItemCodeId),0)+1 FROM Ass_AssetMaster  where N_CompanyID=" + AssMasterTable.Rows[k]["N_CompanyID"] + " and X_CategoryPrefix='" + AssMasterTable.Rows[k]["X_CategoryPrefix"] + "'", connection, transaction);                                  
                                    if(ItemCodeID!=null)
                                        N_ItemCodeId=myFunctions.getIntVAL(ItemCodeID.ToString());

                                    string X_ItemCode="";

                                    if (AssMasterTable.Rows[k]["X_ItemCode"].ToString().Trim() == AssMasterTable.Rows[k]["X_CategoryPrefix"].ToString() + "@Auto".Trim())
                                    {
                                        if (AssMasterTable.Rows[k]["X_CategoryPrefix"].ToString() == "")
                                            X_ItemCode = "Asset" + "" + N_ItemCodeId.ToString("0000");
                                        else
                                            X_ItemCode = AssMasterTable.Rows[k]["X_CategoryPrefix"].ToString() + "" + N_ItemCodeId.ToString("0000");
                                    }
                                    else
                                    {
                                        X_ItemCode = AssMasterTable.Rows[k]["X_ItemCode"].ToString();
                                    }
                                    if(myFunctions.getIntVAL(AssMasterTable.Rows[k]["N_ItemID"].ToString())==0)
                                    {
                                        int ItemCodeCount=myFunctions.getIntVAL(dLayer.ExecuteScalar("Select count(X_ItemCode) FROM Ass_AssetMaster  where N_CompanyID=" + AssMasterTable.Rows[k]["N_CompanyID"] + " and ltrim(X_ItemCode)='" + X_ItemCode + "'", connection, transaction).ToString());                                  
                                        if(ItemCodeCount>0)
                                        {
                                            transaction.Rollback();
                                            return Ok(_api.Error("Item Exists"));
                                        }
                                    }
                                    AssMasterTable.Rows[k]["X_ItemCode"]=X_ItemCode;
                                }
                                N_MasterID=dLayer.SaveData("Ass_AssetMaster","N_ItemID",AssMasterTable,connection,transaction); 
                                if(N_MasterID<=0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error("Error"));
                                }   

                                for (int k = 0 ;k < TransactionTable.Rows.Count;k++)
                                {
                                    TransactionTable.Rows[k]["N_AssetInventoryID"]=N_AssetInventoryID;
                                    TransactionTable.Rows[k]["X_Reference"]=ReturnNo;
                                    TransactionTable.Rows[k]["N_AssetInventoryDetailsID"]=DetailTable.Rows[k]["N_AssetInventoryDetailsID"];
                                    TransactionTable.Rows[k]["N_ItemId"]=AssMasterTable.Rows[k]["N_ItemID"];
                                }
                                N_ActionID=dLayer.SaveData("Ass_Transactions","N_ActionID",TransactionTable,connection,transaction); 
                                if(N_ActionID<=0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error("Error"));
                                }                   
                            }
                        }
                    }

                    // SortedList PostingParam = new SortedList();
                    // PostingParam.Add("N_CompanyID", N_CompanyID);
                    // PostingParam.Add("X_InventoryMode", "SALES");
                    // PostingParam.Add("N_InternalID", N_SalesID);
                    // PostingParam.Add("N_UserID", N_UserID);
                    // PostingParam.Add("X_SystemName", "ERP Cloud");
                    // try
                    // {
                    //     dLayer.ExecuteNonQueryPro("SP_Acc_InventoryPosting", PostingParam, connection, transaction);
                    // }
                    // catch (Exception ex)
                    // {
                    //     transaction.Rollback();
                    //     return Ok(_api.Error(ex));
                    // }

                    SortedList Result = new SortedList();
                    Result.Add("N_AssetInventoryID",N_AssetInventoryID);
                    Result.Add("X_InvoiceNo",ReturnNo);
                    transaction.Commit();
                    return Ok(_api.Success(Result,"Asset Purchase Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

       [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID,int N_AssetInventoryID,int FormID)
        {
            int Results = 0;
 
            SortedList Params = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@N_AssetInventoryID", N_AssetInventoryID);
                    if (N_AssetInventoryID > 0)
                    {
                        if (FormID == 129)
                            dLayer.ExecuteNonQuery("DELETE FROM Ass_AssetMaster WHERE Ass_AssetMaster.N_CompanyID = @nCompanyID AND Ass_AssetMaster.N_AssetInventoryDetailsID IN (SELECT N_AssetInventoryDetailsID FROM Ass_PurchaseDetails WHERE Ass_PurchaseDetails.N_AssetInventoryID =@N_AssetInventoryID AND Ass_PurchaseDetails.N_CompanyID = @nCompanyID AND Ass_PurchaseDetails.N_FnYearID = @nCompanyID)", Params, connection, transaction);

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
                        return Ok(_api.Error("Unable to delete Asset Purchase"));
                    }


                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
   } 
  
 }