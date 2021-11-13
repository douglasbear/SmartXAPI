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
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int FormID;
        public Inv_DeliveryNote(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 884;
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
                Searchkey = "and ([Invoice No] like '%" + xSearchkey + "%' or Customer like '%" + xSearchkey + "%' or x_Notes like '%" + xSearchkey + "%' or X_OrderNo like '%" + xSearchkey + "%' or [Invoice Date] like '%" + xSearchkey + "%' or D_DeliveryDate like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by [Invoice No] desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "invoiceNo":
                        xSortBy = "[Invoice No] " + xSortBy.Split(" ")[1];
                        break;
                    case "invoiceDate":
                        xSortBy = "Cast([Invoice Date] as DateTime )" + xSortBy.Split(" ")[1];
                        break;
                    case "d_DeliveryDate":
                        xSortBy = "Cast(D_DeliveryDate as DateTime )" + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }
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
                    sqlCommandCount = "select count(*) as N_Count  from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "";
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
        [HttpGet("details")]
        public ActionResult GetDeliveryNoteDetails(int nFnYearId, int nBranchId, string xInvoiceNo, int nSalesOrderID)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
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
                    if (nSalesOrderID > 0)
                    {
                        QueryParamsList.Add("@nSalesorderID", nSalesOrderID);
                        string Mastersql = "select * from vw_SalesOrdertoDeliveryNote where N_CompanyId=@nCompanyID and N_SalesOrderId=@nSalesorderID";
                        DataTable MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = _api.Format(MasterTable, "Master");
                        string DetailSql = "";
                        DetailSql = "select * from vw_SalesOrdertoDeliveryNoteDetails where N_CompanyId=@nCompanyID and N_SalesOrderId=@nSalesorderID";
                        DataTable DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);




                        SortedList DelParams = new SortedList();
                        DelParams.Add("N_CompanyID", nCompanyId);
                        DelParams.Add("N_SalesOrderID", nSalesOrderID);
                        DelParams.Add("FnYearID", nFnYearId);
                        DelParams.Add("@N_Type", 0);
                        DataTable OrderToDel = dLayer.ExecuteDataTablePro("SP_InvSalesOrderDtlsInDelNot_Disp", DelParams, Con);
                        foreach (DataRow Avar in OrderToDel.Rows)
                        {
                            foreach (DataRow Kvar in DetailTable.Rows)
                            {
                                if (myFunctions.getIntVAL(Avar["N_SalesOrderDetailsID"].ToString()) == myFunctions.getIntVAL(Kvar["N_SalesOrderDetailsID"].ToString()))
                                {
                                    Kvar["N_QtyDisplay"] = Avar["N_QtyDisplay"];
                                    Kvar["N_Qty"] = Avar["N_Qty"];

                                }
                            }
                        }
                        DetailTable.AcceptChanges();

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
                    var nFormID = this.FormID;
                    int N_DelID = myFunctions.getIntVAL(MasterRow["N_deliverynoteid"].ToString());
                    int N_SalesOrderID = myFunctions.getIntVAL(MasterRow["n_SalesOrderID"].ToString());
                    QueryParamsList.Add("@nDelID", N_DelID);
                    QueryParamsList.Add("@nSaleOrderID", N_SalesOrderID);
                    object InSales = dLayer.ExecuteScalar("select x_ReceiptNo from Inv_Sales where N_CompanyID=@nCompanyID and N_deliverynoteid=@nDelID and N_FnYearID=@nFnYearID", QueryParamsList, Con);
                    masterTable = myFunctions.AddNewColumnToDataTable(masterTable, "x_SalesReceiptNo", typeof(string), InSales);

                    object InSalesOrder = dLayer.ExecuteScalar("select x_OrderNo from Inv_SalesOrder where N_CompanyID=@nCompanyID and N_SalesOrderID=@nSaleOrderID and N_FnYearID=@nFnYearID", QueryParamsList, Con);
                    masterTable = myFunctions.AddNewColumnToDataTable(masterTable, "x_OrderNo", typeof(string), InSalesOrder);


                    QueryParamsList.Add("@nSalesID", myFunctions.getIntVAL(MasterRow["N_TruckID"].ToString()));




                    masterTable = myFunctions.AddNewColumnToDataTable(masterTable, "N_SalesId", typeof(int), 0);
                    masterTable = myFunctions.AddNewColumnToDataTable(masterTable, "isSalesDone", typeof(bool), false);


                    if (myFunctions.getIntVAL(masterTable.Rows[0]["N_DeliveryNoteId"].ToString()) > 0)
                    {
                        QueryParamsList.Add("@nDeliveryNoteId", myFunctions.getIntVAL(masterTable.Rows[0]["N_DeliveryNoteId"].ToString()));

                        DataTable SalesData = dLayer.ExecuteDataTable("select X_ReceiptNo,N_SalesId from Inv_Sales where N_DeliveryNoteId=@nDeliveryNoteId and N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID", QueryParamsList, Con);
                        if (SalesData.Rows.Count > 0)
                        {
                            masterTable.Rows[0]["X_SalesReceiptNo"] = SalesData.Rows[0]["X_ReceiptNo"].ToString();
                            masterTable.Rows[0]["N_SalesId"] = myFunctions.getIntVAL(SalesData.Rows[0]["N_SalesId"].ToString());
                            masterTable.Rows[0]["isSalesDone"] = true;
                        }
                    }

                    //Details
                    SortedList dParamList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"N_SalesID",masterTable.Rows[0]["n_DeliveryNoteId"].ToString()}
                    };
                    DataTable detailTable = dLayer.ExecuteDataTablePro("SP_InvDeliveryNoteDtls_Disp", dParamList, Con);
                    detailTable = _api.Format(detailTable, "Details");
                    DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(masterTable.Rows[0]["N_CustomerID"].ToString()), myFunctions.getIntVAL(masterTable.Rows[0]["n_DeliveryNoteId"].ToString()), this.FormID, myFunctions.getIntVAL(masterTable.Rows[0]["N_FnYearID"].ToString()), User, Con);
                    Attachments = _api.Format(Attachments, "attachments");

                    if (detailTable.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                    dsSalesInvoice.Tables.Add(masterTable);
                    dsSalesInvoice.Tables.Add(detailTable);
                    dsSalesInvoice.Tables.Add(Attachments);

                    return Ok(_api.Success(dsSalesInvoice));

                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
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
                DataTable Attachment = ds.Tables["attachments"];
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
                    bool B_SalesOrder = myFunctions.CheckPermission(N_CompanyID, 81, "Administrator", "X_UserCategory", dLayer, connection, transaction);
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
                        if (InvoiceNo == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Delivery Number")); }
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
                            try
                            {
                                dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, ex));
                            }
                        }
                    }
                    N_DeliveryNoteID = dLayer.SaveData("Inv_DeliveryNote", "N_DeliveryNoteId", MasterTable, connection, transaction);
                    if (N_DeliveryNoteID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save Delivery Invoice!"));
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
                        return Ok(_api.Error(User, "Unable to save Delivery Note!"));
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
                            try
                            {
                                dLayer.ExecuteNonQueryPro("SP_DeliveryNoteDetails_InsNew", ParamInsNew, connection, transaction);
                                dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Sales_Posting", ParamSales_Posting, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                if (ex.Message == "50")
                                    return Ok(_api.Error(User, "Day Closed"));
                                else if (ex.Message == "51")
                                    return Ok(_api.Error(User, "Year Closed"));
                                else if (ex.Message == "52")
                                    return Ok(_api.Error(User, "Year Exists"));
                                else if (ex.Message == "53")
                                    return Ok(_api.Error(User, "Period Closed"));
                                else if (ex.Message == "54")
                                    return Ok(_api.Error(User, "Txn Date"));
                                else if (ex.Message == "55")
                                    return Ok(_api.Error(User, "Product is not available for delivery"));
                                else return Ok(_api.Error(User, ex));
                            }
                            SortedList CustomerParams = new SortedList();
                            CustomerParams.Add("@nCustomerID", N_CustomerID);
                            DataTable CustomerInfo = dLayer.ExecuteDataTable("Select X_CustomerCode,X_CustomerName from Inv_Customer where N_CustomerID=@nCustomerID", CustomerParams, connection, transaction);
                            if (CustomerInfo.Rows.Count > 0)
                            {
                                try
                                {
                                    myAttachments.SaveAttachment(dLayer, Attachment, InvoiceNo, N_DeliveryNoteID, CustomerInfo.Rows[0]["X_CustomerName"].ToString().Trim(), CustomerInfo.Rows[0]["X_CustomerCode"].ToString(), N_CustomerID, "Customer Document", User, connection, transaction);
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, ex));
                                }
                            }
                        }
                        transaction.Commit();
                        return Ok(_api.Success("Delivery Note saved" + ":" + InvoiceNo));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
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
                        return Ok(_api.Error(User, "Unable to delete delivery note"));
                    }
                    else
                    {
                        dLayer.ExecuteNonQuery("delete from Inv_StockMaster where N_SalesID=@nDeliveryNoteID and n_CompanyID=@nCompanyID", QueryParams, connection, transaction);

                        myAttachments.DeleteAttachment(dLayer, 1, nDeliveryNoteID, nCustomerID, nFnYearID, this.FormID, User, transaction, connection);
                    }
                    //Attachment delete code here

                    transaction.Commit();
                    return Ok(_api.Success("Delivery note deleted"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
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
                return Ok(_api.Error(User, e));
            }
        }
    }
}