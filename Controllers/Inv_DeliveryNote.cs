using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("deliverynote")]
    [ApiController]
    public class Inv_DeliveryNote : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        public Inv_DeliveryNote(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetDeliveryNoteList(int? nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and [Invoice No] like '%" + xSearchkey + "%' or Customer like '%"+ xSearchkey + "%'";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_DeliveryNoteId desc";
            else
                xSortBy = " order by " + xSortBy;


            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and N_DeliveryNoteID not in (select top(" + Count + ") N_DeliveryNoteID from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 "+Searchkey;
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
                return Ok(_api.Error(e));
            }
        }
        [HttpGet("details")]
        public ActionResult GetDeliveryNoteDetails(int nFnYearId, int nBranchId, string xInvoiceNo,int nSalesOrderID)
        {
                int nCompanyId= myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection Con = new SqlConnection(connectionString))
                {
                    Con.Open();

                    
                    DataSet dsSalesInvoice = new DataSet();
                    SortedList QueryParamsList = new SortedList();
                    QueryParamsList.Add("@nCompanyID", nCompanyId);
                    QueryParamsList.Add("@nFnYearID", nFnYearId);
                    QueryParamsList.Add("@nBranchId", nBranchId);
                    QueryParamsList.Add("@xTransType", "DELIVERY");

                    SortedList mParamsList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"X_ReceiptNo",xInvoiceNo},
                        {"X_TransType","DELIVERY"},
                        {"N_FnYearID",nFnYearId},
                        {"N_BranchId",nBranchId}
                    };
                    if(nSalesOrderID>0)
                    {
                        QueryParamsList.Add("@nSalesorderID", nSalesOrderID);
                        string Mastersql = "select * from vw_SalesOrdertoDeliveryNote where N_CompanyId=@nCompanyID and N_SalesOrderId=@nSalesorderID";
                        DataTable MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = _api.Format(MasterTable, "Master");
                        string DetailSql = "";
                        DetailSql = "select * from vw_SalesOrdertoDeliveryNoteDetails where N_CompanyId=@nCompanyID and N_SalesOrderId=@nSalesorderID";
                        DataTable DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dsSalesInvoice.Tables.Add(MasterTable);
                        dsSalesInvoice.Tables.Add(DetailTable);
                        return Ok(_api.Success(dsSalesInvoice));
                    }
                    else
                    {
                         QueryParamsList.Add("@xInvoiceNo", xInvoiceNo);
                    }
                    DataTable masterTable = dLayer.ExecuteDataTablePro("SP_InvDeliveryNote_Disp", mParamsList, Con);

                   
                    masterTable = _api.Format(masterTable, "Master");
                    if (masterTable.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                    DataRow MasterRow = masterTable.Rows[0];
                    int N_DelID = myFunctions.getIntVAL(MasterRow["N_deliverynoteid"].ToString());
                    QueryParamsList.Add("@nDelID", nCompanyId);
                    object InSales = dLayer.ExecuteScalar("select x_ReceiptNo from Inv_Sales where N_CompanyID=@nCompanyID and N_deliverynoteid=@nDelID", QueryParamsList, Con);
                    masterTable = myFunctions.AddNewColumnToDataTable(masterTable, "x_SalesReceiptNo", typeof(string), InSales);

                    QueryParamsList.Add("@nSalesID", myFunctions.getIntVAL(MasterRow["N_TruckID"].ToString()));

                    //Details
                    SortedList dParamList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"N_SalesID",masterTable.Rows[0]["n_DeliveryNoteId"].ToString()}
                    };
                    DataTable detailTable = dLayer.ExecuteDataTablePro("SP_InvDeliveryNoteDtls_Disp", dParamList, Con);
                    detailTable = _api.Format(detailTable, "Details");
                    if (detailTable.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                    dsSalesInvoice.Tables.Add(masterTable);
                    dsSalesInvoice.Tables.Add(detailTable);

                    return Ok(_api.Success(dsSalesInvoice));

                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        //Save....
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;

                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];

                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();
                // Auto Gen 
                string InvoiceNo = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    DataRow MasterRow = MasterTable.Rows[0];
                    transaction = connection.BeginTransaction();


                    int N_DeliveryNoteID = myFunctions.getIntVAL(MasterRow["n_DeliveryNoteId"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int N_BranchID = myFunctions.getIntVAL(MasterRow["n_BranchID"].ToString());
                    int N_LocationID = myFunctions.getIntVAL(MasterRow["n_LocationID"].ToString());
                    int N_CustomerID = myFunctions.getIntVAL(MasterRow["n_CustomerID"].ToString());
                    int N_UserID = myFunctions.getIntVAL(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    int UserCategoryID = myFunctions.getIntVAL(User.FindFirst(ClaimTypes.GroupSid)?.Value);
                    //int N_AmtSplit = 0;
                    int N_SaveDraft = myFunctions.getIntVAL(MasterRow["b_IsSaveDraft"].ToString());
                    bool B_AllBranchData = false, B_AllowCashPay = false;
                    bool B_SalesOrder = myFunctions.CheckPermission(N_CompanyID, 81, "Administrator","X_UserCategory", dLayer, connection, transaction);
                    bool B_SRS = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("729", "SRSinDeliveryNote", "N_Value", N_CompanyID, dLayer, connection, transaction)));
                    QueryParams.Add("@nCompanyID", N_CompanyID);
                    QueryParams.Add("@nFnYearID", N_FnYearID);
                    QueryParams.Add("@nSalesID", N_DeliveryNoteID);
                    QueryParams.Add("@nBranchID", N_BranchID);
                    QueryParams.Add("@nLocationID", N_LocationID);
                    QueryParams.Add("@nCustomerID", N_CustomerID);

                    //B_DirectPosting = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select B_DirPosting from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID", QueryParams, connection, transaction).ToString());
                    object objAllBranchData = dLayer.ExecuteScalar("Select B_ShowAllData From Acc_BranchMaster where N_BranchID=@nBranchID and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                    if (objAllBranchData != null)
                        B_AllBranchData = myFunctions.getBoolVAL(objAllBranchData.ToString());

                    if (B_AllBranchData)
                        B_AllowCashPay = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select cast(count(N_CustomerID) as bit) from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID  and N_AllowCashPay=1", QueryParams, connection, transaction).ToString());
                    else
                        B_AllowCashPay = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select cast(count(N_CustomerID) as bit) from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID  and N_AllowCashPay=1 and (N_BranchId=@nBranchID or N_BranchId=0)", QueryParams, connection, transaction).ToString());


                    //saving data
                    var values = MasterRow["x_ReceiptNo"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterRow["n_CompanyId"].ToString());
                        Params.Add("N_YearID", MasterRow["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 729);
                        Params.Add("N_BranchID", MasterRow["n_BranchId"].ToString());
                        InvoiceNo = dLayer.GetAutoNumber("Inv_DeliveryNote", "x_ReceiptNo", Params, connection, transaction);
                        if (InvoiceNo == "") { return Ok(_api.Error("Unable to generate Delivery Number")); }
                        MasterTable.Rows[0]["x_ReceiptNo"] = InvoiceNo;
                    }
                    else
                    {
                        if (N_DeliveryNoteID > 0)
                        {
                            SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",N_CompanyID},
                                {"X_TransType","DELIVERY"},
                                {"N_VoucherID",N_DeliveryNoteID}};
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                        }
                    }
                    N_DeliveryNoteID = dLayer.SaveData("Inv_DeliveryNote", "N_DeliveryNoteId", MasterTable, connection, transaction);
                    if (N_DeliveryNoteID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Delivery Invoice!"));
                    }
                    // if (B_UserLevel)
                    // {
                    //     Inv_WorkFlowCatalog saving code here
                    // }
                    int N_PRSID = 0;
                    int N_SalesOrderID = 0;
                    int N_SalesQuotationID = 0;
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_DeliveryNoteID"] = N_DeliveryNoteID;
                        N_PRSID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_RsID"].ToString());
                        N_SalesOrderID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesOrderID"].ToString());
                        N_SalesQuotationID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesQuotationID"].ToString());
                        if (B_SalesOrder)
                        {
                            if (B_SRS)
                            {
                                if (N_PRSID > 0)
                                    dLayer.ExecuteNonQuery("update  Inv_PRS set N_DeliveryNoteID=" + N_DeliveryNoteID + ", N_Processed=3 where N_PRSID=" + N_PRSID + " and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection, transaction);
                            }
                            if (N_SalesOrderID > 0)
                                dLayer.ExecuteNonQuery("update  Inv_SalesOrder set N_SalesID=" + N_DeliveryNoteID + ", N_Processed=1 where N_SalesOrderID=" + N_SalesOrderID + " and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection, transaction);

                        }

                        else
                        {
                            if (N_SalesQuotationID > 0)
                                dLayer.ExecuteNonQuery("update  Inv_SalesQuotation set N_SalesID=" + N_DeliveryNoteID + ", N_Processed=1 where N_QuotationID=" + N_SalesQuotationID + " and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection, transaction);
                        }
                    }
                    int N_DeliveryNoteDetailsID = dLayer.SaveData("Inv_DeliveryNoteDetails", "n_DeliveryNoteDetailsID", DetailTable, connection, transaction);
                    if (N_DeliveryNoteDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Delivery Note!"));
                    }
                    else
                    {
                        if (N_SaveDraft == 0)
                        {

                            SortedList ParamInsNew = new SortedList();
                            ParamInsNew.Add("N_CompanyID", N_CompanyID);
                            ParamInsNew.Add("N_SalesID", N_DeliveryNoteID);
                            ParamInsNew.Add("N_SaveDraft", 0);

                            SortedList ParamSales_Posting = new SortedList();
                            ParamSales_Posting.Add("N_CompanyID", N_CompanyID);
                            ParamSales_Posting.Add("X_InventoryMode", "DELIVERY");
                            ParamSales_Posting.Add("N_InternalID", N_DeliveryNoteID);
                            ParamSales_Posting.Add("N_UserID", N_UserID);
                            ParamSales_Posting.Add("X_SystemName", "ERP Cloud");

                            dLayer.ExecuteNonQueryPro("SP_DeliveryNoteDetails_InsNew", ParamInsNew, connection, transaction);
                            dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Sales_Posting", ParamSales_Posting, connection, transaction);
                        }
                        //dispatch saving here
                        transaction.Commit();
                    }
                    //return GetSalesInvoiceDetails(int.Parse(MasterRow["n_CompanyId"].ToString()), int.Parse(MasterRow["n_FnYearId"].ToString()), int.Parse(MasterRow["n_BranchId"].ToString()), InvoiceNo);
                    return Ok(_api.Success("Delivery Note saved" + ":" + InvoiceNo));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
        // private bool ValidateIMEIs(int row,DataSet ds,int nSalesID,string imeiFrom,string imeiTo,SortedList QueryParams,SqlConnection connection,SqlTransaction transaction)
        // {
        //     if (ds.Tables.Contains("ValidateIMEIs"))
        //         ds.Tables.Remove("ValidateIMEIs");
        //     if (nSalesID == 0)
        //     {
        //         if (Regex.Matches(imeiFrom, @"[a-zA-Z]").Count > 0)
        //         dLayer.ExecuteScalar("Select TOP (1) ISNULL(N_CustomerID,0) from vw_SalesAmount_Customer where N_SalesID=@nSalesID", QueryParams, connection, transaction);
        //         ds.Tables["ValidateIMEIs"] = dLayer.ExecuteDataTable("Select TOP (1) ISNULL(N_CustomerID,0) from vw_SalesAmount_Customer where N_SalesID=@nSalesID", QueryParams, connection);
        //             dba.FillDataSet(ref ds, "ValidateIMEIs", "select Isnull(Count(*),0) As N_IMEICount from Inv_StockMaster_IMEI where N_IMEI='" + flxSales.get_TextMatrix(row, mcSerialFrom) + "' and N_Status = 0 and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());
        //         else
        //             dba.FillDataSet(ref ds, "ValidateIMEIs", "select Isnull(Count(*),0) As N_IMEICount from Inv_StockMaster_IMEI where ISNUMERIC(N_IMEI)>0  and convert(decimal(38),N_IMEI) between  " + flxSales.get_TextMatrix(row, mcSerialFrom) + " and  " + flxSales.get_TextMatrix(row, mcSerialTo) + " and N_Status = 0 and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());
        //         if (ds.Tables["ValidateIMEIs"].Rows.Count != 0)
        //         {
        //             if (myFunctions.getLongIntVAL(ds.Tables["ValidateIMEIs"].Rows[0]["N_IMEICount"].ToString()) != myFunctions.getLongIntVAL(flxSales.get_TextMatrix(row, mcQuantity)))
        //             {
        //                 msg.msgInformation("Could not sale some items (Already sold out/Item not found) entered in row number " + row.ToString());
        //                 return false;
        //             }
        //         }
        //     }
        //     else
        //     {
        //         if (Regex.Matches(imeiFrom, @"[a-zA-Z]").Count > 0)
        //         {
        //             if (imeiFrom != imeiTo)
        //             {
        //                 int N_Status = myFunctions.getIntVAL(dba.ExecuteSclar("Select N_Status from Inv_StockMaster_IMEI Where N_IMEI='" + flxSales.get_TextMatrix(row, mcSerialTo) + "' and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable()).ToString());
        //                 if (N_Status == 1)
        //                 {
        //                     msg.msgInformation("Could not sale some items (Already sold out/Item not found) entered in row number " + row.ToString());
        //                     return false;
        //                 }
        //                 else
        //                 {
        //                     dba.FillDataSet(ref ds, "ValidateIMEIs", "select Isnull(Count(*),0) As N_IMEICount from Inv_StockMaster_IMEI where N_IMEI='" + flxSales.get_TextMatrix(row, mcSerialFrom) + "' and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());
        //                     if (ds.Tables["ValidateIMEIs"].Rows.Count != 0)
        //                     {
        //                         if (myFunctions.getLongIntVAL(ds.Tables["ValidateIMEIs"].Rows[0]["N_IMEICount"].ToString()) != myFunctions.getLongIntVAL(flxSales.get_TextMatrix(row, mcQuantity)))
        //                         {
        //                             msg.msgInformation("Could not sale some items (Already sold out/Item not found) entered in row number " + row.ToString());
        //                             return false;
        //                         }
        //                     }
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             if (X_PrevImeito != myFunctions.getVAL(flxSales.get_TextMatrix(row, mcSerialTo)))
        //             {
        //                 int N_Status = myFunctions.getIntVAL(dba.ExecuteSclar("Select N_Status from Inv_StockMaster_IMEI Where N_IMEI ='" + flxSales.get_TextMatrix(row, mcSerialTo) + "' and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable()).ToString());
        //                 if (N_Status == 1)
        //                 {
        //                     msg.msgInformation("Could not sale some items (Already sold out/Item not found) entered in row number " + row.ToString());
        //                     return false;
        //                 }
        //                 else
        //                 {
        //                     dba.FillDataSet(ref dsSales, "ValidateIMEIs", "select Isnull(Count(*),0) As N_IMEICount from Inv_StockMaster_IMEI where ISNUMERIC(N_IMEI)>0  and convert(decimal(38),N_IMEI) between  " + flxSales.get_TextMatrix(row, mcSerialFrom) + " and  " + flxSales.get_TextMatrix(row, mcSerialTo) + " and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());
        //                     if (dsSales.Tables["ValidateIMEIs"].Rows.Count != 0)
        //                     {
        //                         if (myFunctions.getLongIntVAL(dsSales.Tables["ValidateIMEIs"].Rows[0]["N_IMEICount"].ToString()) != myFunctions.getLongIntVAL(flxSales.get_TextMatrix(row, mcQuantity)))
        //                         {
        //                             msg.msgInformation("Could not sale some items (Already sold out/Item not found) entered in row number " + row.ToString());
        //                             return false;
        //                         }
        //                     }
        //                 }
        //             }
        //         }
        //     }         
        //     return true;
        // }
        //Delete....
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nDeliveryNoteID, int nCustomerID, int nCompanyID, int nFnYearID, int nBranchID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    var xUserCategory = User.FindFirst(ClaimTypes.GroupSid)?.Value;
                    var nUserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    //Results = dLayer.DeleteData("Inv_SalesInvoice", "n_InvoiceID", N_InvoiceID, "",connection,transaction);
                    SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"N_UserID",nUserID},
                                {"X_TransType","DELIVERY"},
                                {"X_SystemName","WebRequest"},
                                {"N_VoucherID",nDeliveryNoteID}};

                    SortedList QueryParams = new SortedList(){
                                {"@nCompanyID",nCompanyID},
                                {"@nFnYearID",nFnYearID},
                                {"@nUserID",nUserID},
                                {"@xTransType","DELIVERY"},
                                {"@xSystemName","WebRequest"},
                                {"@nDeliveryNoteID",nDeliveryNoteID},
                                {"@nPartyID",nCustomerID},
                                {"@nBranchID",nBranchID}};

                    Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to delete delivery note"));
                    }
                    else
                    {
                        dLayer.ExecuteNonQuery("delete from Inv_StockMaster where N_SalesID=@nDeliveryNoteID and n_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                    }
                    //Attachment delete code here

                    transaction.Commit();
                    return Ok(_api.Success("Delivery note deleted"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }


        }

        [HttpGet("deliveryNoteSearch")]
        public ActionResult GetInvoiceList(int? nCompanyId, int nCustomerId, bool bAllBranchData, int nBranchId, int nLocationId)
        {
            SortedList Params = new SortedList();

            string crieteria = "";


            if (bAllBranchData == true)
            {
                if (nCustomerId > 0)
                    crieteria = " where X_TransType='DELIVERY' and N_DeliveryType = 0 and N_CustomerID=@nCustomerId and N_CompanyID=@nCompanyId and B_IsSaveDraft=0";
                else
                    crieteria = " where X_TransType='DELIVERY' and N_DeliveryType = 0 and N_CompanyID=@nCompanyId and B_IsSaveDraft=0";
            }
            else
            {
                if (nCustomerId > 0)
                    crieteria = " where X_TransType='DELIVERY' and N_DeliveryType = 0 and N_CustomerID=@nCustomerId and N_CompanyID=@nCompanyId and N_BranchID=@nBranchId and N_LocationID=@nLocationId and B_IsSaveDraft=0";
                else
                    crieteria = " where X_TransType='DELIVERY' and N_DeliveryType = 0 and N_CompanyID=@nCompanyId and N_BranchID=@nBranchId and N_LocationID=@nLocationId and B_IsSaveDraft=0";
            }

            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nCustomerId", nCustomerId);
            Params.Add("@bAllBranchData", bAllBranchData);
            Params.Add("@nBranchId", nBranchId);
            Params.Add("@nLocationId", nLocationId);
            string sqlCommandText = "select [Invoice No],[Invoice Date],[Customer] as X_CustomerName,N_CompanyID,N_CustomerID,N_DeliveryNoteId,N_DeliveryType,X_TransType,N_FnYearID,N_BranchID,X_LocationName,N_LocationID,B_IsSaveDraft from vw_InvDeliveryNote_Search " + crieteria + " order by N_DeliveryNoteId DESC,[Invoice No]";
            try
            {
                DataTable SalesInvoiceList = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SalesInvoiceList = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SalesInvoiceList = _api.Format(SalesInvoiceList);
                    if (SalesInvoiceList.Rows.Count == 0) { return Ok(_api.Notice("No Sales Invoices Found")); }
                }
                return Ok(_api.Success(SalesInvoiceList));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
    }
}