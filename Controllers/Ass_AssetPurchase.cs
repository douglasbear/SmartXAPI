
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
        public ActionResult GetAssetPurchaseList(int? nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            DataTable CountTable = new DataTable();
            SortedList Params = new SortedList();
            DataSet dataSet = new DataSet();
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([Invoice No] like '%" + xSearchkey + "%' or Vendor like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_PurchaseID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "invoiceNo":
                        xSortBy = "N_PurchaseID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }
            int Count = (nPage - 1) * nSizeperpage;
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") N_PurchaseID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],InvoiceNetAmt,X_BranchName,X_Description from vw_InvPurchaseInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_PurchaseID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],InvoiceNetAmt,X_BranchName,X_Description from vw_InvPurchaseInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and N_PurchaseID not in (select top(" + Count + ") N_PurchaseID from vw_InvPurchaseInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + xSortBy + " ) " + xSortBy;

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count,sum(Cast(REPLACE(InvoiceNetAmt,',','') as Numeric(10,2)) ) as TotalAmount from vw_InvPurchaseInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "";
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
                    DataTable PurchaseTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    PurchaseTable = ds.Tables["purchase"];
                    SortedList Params = new SortedList();
                    // Auto Gen
                    try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                    connection.Open();
                    SqlTransaction transaction=connection.BeginTransaction();
                    string ReturnNo="";
                    int N_AssetInventoryID =myFunctions.getIntVAL(MasterTable.Rows[0]["N_AssetInventoryID"].ToString());
                    int FormID =myFunctions.getIntVAL(MasterTable.Rows[0]["N_FormID"].ToString());
                    int TypeID =myFunctions.getIntVAL(MasterTable.Rows[0]["N_TypeID"].ToString());
                    int N_UserID=myFunctions.GetUserID(User);

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
                                {"X_TransType","PURCHASE RETURN"},
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
                    }
                    int PurchaseID=0;
                    if (!(FormID == 1293 && TypeID == 281))
                    {
                        PurchaseTable.Rows[0]["N_PurchaseRefID"]=N_AssetInventoryID;
                        PurchaseTable.Rows[0]["X_InvoiceNo"] = MasterTable.Rows[0]["X_InvoiceNo"].ToString();;
                        PurchaseID=dLayer.SaveData("Inv_Purchase","N_PurchaseID",PurchaseTable,connection,transaction); 
                        if(PurchaseID<=0)
                        {
                            transaction.Rollback();
                        }
                    }
                    if (PurchaseID > 0 ||(FormID ==1293 && TypeID ==281))
                    {

                    }
                    // for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                    // {
                    //     DetailTable.Rows[j]["N_CreditNoteID"]=N_CreditNoteID;
                    // }
                    // int N_QuotationDetailId=dLayer.SaveData("Inv_PurchaseReturnDetails","n_CreditNoteDetailsID",DetailTable,connection,transaction);                    
                    // transaction.Commit();

                    //      SortedList InsParams = new SortedList(){
                    //             {"N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString()},
                    //             {"N_CreditNoteID",N_CreditNoteID}};    
                    //     dLayer.ExecuteNonQueryPro("[SP_PurchaseReturn_Ins]", InsParams, connection, transaction);

                    // SortedList PostParams = new SortedList(){
                    //             {"N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString()},
                    //             {"X_InventoryMode","PURCHASE RETURN"},
                    //             {"N_InternalID",N_CreditNoteID},
                    //             {"N_UserID",N_UserID}};
                    // dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Purchase_Posting", PostParams, connection, transaction);

                     SortedList Result = new SortedList();
                    // Result.Add("n_PurchaseReturnID",N_CreditNoteID);
                    Result.Add("x_PurchaseReturnNo",ReturnNo);
                    return Ok(_api.Success(Result,"Purchase Return Saved"));
                    }
                }
                catch (Exception ex)
                {
                    return Ok(_api.Error(ex));
                }
        }

       [HttpDelete("delete")]
        public ActionResult DeleteData(int N_AssetInventoryID,int FormID)
        {
            int Results = 0;

            SortedList Params = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    //Params.Add("@nBranchID", nBranchID);
                    if (N_AssetInventoryID > 0)
                    {
                        // Results = dLayer.DeleteData("Acc_BranchMaster", "N_BranchID", nBranchID, "", connection);

                        // Results = dLayer.DeleteData("Inv_Location", "N_BranchID", nBranchID, "B_IsDefault=1", connection);
                        // if (FormID == 129)
                        //     dba.ExecuteNonQuery("DELETE FROM Ass_AssetMaster WHERE Ass_AssetMaster.N_CompanyID = " + myCompanyID._CompanyID + " AND Ass_AssetMaster.N_AssetInventoryDetailsID IN (SELECT N_AssetInventoryDetailsID FROM Ass_PurchaseDetails WHERE Ass_PurchaseDetails.N_AssetInventoryID =" + N_AssetInventoryID + " AND Ass_PurchaseDetails.N_CompanyID = " + myCompanyID._CompanyID + " AND Ass_PurchaseDetails.N_FnYearID = " + myCompanyID._CompanyID + ")", "TEXT", new DataTable());
                        // dba.ExecuteNonQuery("delete from Ass_PurchaseDetails Where N_AssetInventoryID=" + N_AssetInventoryID+ " and N_CompanyID=" + myCompanyID._CompanyID.ToString(), "TEXT", new DataTable());
                        // dba.ExecuteNonQuery("delete from Ass_PurchaseMaster Where N_AssetInventoryID=" + N_AssetInventoryID + " and N_CompanyID=" + myCompanyID._CompanyID.ToString(), "TEXT", new DataTable());
                        

                    }
                    else
                    {
                        return Ok(_api.Success("unable to delete Asset Purchase "));
                    }
                    if (Results >= 0)
                    {
                        return Ok(_api.Success("Asset Purchase deleted"));
                    }
                    else
                    {
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