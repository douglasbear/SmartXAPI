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
using System.Net;
using System.IO;
using System.Drawing.Imaging;
using QRCoder;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("salesinvoice")]
    [ApiController]
    public class Inv_SalesInvoice : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int N_FormID;
        private readonly ITxnHandler txnHandler;
        public Inv_SalesInvoice(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt, ITxnHandler txn)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 64;
            txnHandler = txn;
        }

        [HttpGet("list")]
        public ActionResult GetSalesInvoiceList(int nFnYearId, int nPage, bool bAllBranchData, int nBranchID, int n_LocationID, int nSizeperpage, string xSearchkey, string xSortBy, string screen, int nCustomerID, int nFormID, bool showStockInlist)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int nCompanyId = myFunctions.GetCompanyID(User);
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();

                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";
                    string X_TransType = "SALES";
                    string criteria = "";
                    string cndn = "";
                    string UserPattern = myFunctions.GetUserPattern(User);
                    int nUserID = myFunctions.GetUserID(User);
                    string Pattern = "";
                    string formIDCndn = "";

                    if (nFormID > 0)
                    {
                        if (nFormID == 64)
                            formIDCndn = " and ( N_FormID = 64 or N_FormID = 894 ) ";
                        else
                            formIDCndn = " and N_FormID = @N_FormID ";

                        Params.Add("@N_FormID", nFormID);
                    }
                    bool B_ReduceTime = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("64", "reduceTime_Sales", "N_Value", myFunctions.getIntVAL(nCompanyId.ToString()), dLayer, connection)));

                    if (UserPattern != "")
                    {
                        Pattern = " and Left(X_Pattern,Len(@UserPattern))=@UserPattern ";
                        Params.Add("@UserPattern", UserPattern);
                    }
                    //  else
                    //        {
                    // object HierarchyCount = dLayer.ExecuteScalar("select count(N_HierarchyID) from Sec_UserHierarchy where N_CompanyID="+nCompanyId, Params, connection);

                    // if( myFunctions.getIntVAL(HierarchyCount.ToString())>0)
                    // Pattern = " and N_UserID=" + nUserID+" ";
                    // }

                    int N_decimalPlace = 2;
                    N_decimalPlace = myFunctions.getIntVAL(myFunctions.ReturnSettings("Sales", "Decimal_Place", "N_Value", nCompanyId, dLayer, connection));
                    N_decimalPlace = N_decimalPlace == 0 ? 2 : N_decimalPlace;

                    bool CheckClosedYear = Convert.ToBoolean(dLayer.ExecuteScalar("Select B_YearEndProcess From Acc_FnYear Where N_CompanyID=" + nCompanyId + " and N_FnYearID = " + nFnYearId, Params, connection));
                    bool bLocationChange = myFunctions.CheckPermission(myFunctions.GetCompanyID(User), 564, myFunctions.GetUserCategory(User).ToString(), "", dLayer, connection);
                    string viewName = " [vw_InvSalesInvoiceNo_Search_New_Cloud] ";

                    bool PayModeEnabled = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("64", "ShowPayModeInInvoiceListing", "N_Value", myFunctions.getIntVAL(nCompanyId.ToString()), dLayer, connection)));
                    if (PayModeEnabled)
                        viewName = " vw_InvSalesInvoiceNo_Search_cloud ";

                    if (B_ReduceTime)
                        viewName = " vw_InvSalesInvoiceDashboard_Cloud ";

                    if (nCustomerID > 0)
                        cndn = "and N_CustomerID=@nCustomerID ";

                    if (screen == "Invoice")
                        criteria = " and MONTH(Cast([Invoice Date] as DateTime)) = MONTH(CURRENT_TIMESTAMP) AND YEAR(Cast([Invoice Date] as DateTime)) = YEAR(CURRENT_TIMESTAMP) and b_IsSaveDraft=0";

                    if (xSearchkey != null && xSearchkey.Trim() != "")
                    {
                        if (!B_ReduceTime)
                            Searchkey = " and ([Invoice No] like '%" + xSearchkey + "%' or Customer like '%" + xSearchkey + "%' or x_Notes like '%" + xSearchkey + "%' or x_OrderNo like '%" + xSearchkey + "%' or X_SalesmanName like '%" + xSearchkey + "%' or X_SalesmanName like '%" + xSearchkey + "%' or cast([Invoice Date] as VarChar) like '%" + xSearchkey + "%' or x_CustPONo like '%" + xSearchkey + "%' or X_BillAmt like '%" + xSearchkey + "%' or X_ProjectCode like '%" + xSearchkey + "%')";
                        else
                            Searchkey = " and ([Invoice No] like '%" + xSearchkey + "%' or Customer like '%" + xSearchkey + "%' or x_Notes like '%" + xSearchkey + "%'  or cast([Invoice Date] as VarChar) like '%" + xSearchkey + "%'  or X_BillAmt like '%" + xSearchkey + "%')";
                    }

                    if (CheckClosedYear == false)
                    {
                        if (bAllBranchData == true && bLocationChange == true)
                        {
                            Searchkey = Searchkey + " and  X_TransType = '" + X_TransType + "' and N_SalesType = 0  and N_FnYearID=" + nFnYearId + " and isnull(N_Hold,0)<>1 ";//and B_YearEndProcess=0";
                        }
                        else
                        {
                            Searchkey = Searchkey + " and  X_TransType = '" + X_TransType + "' and N_SalesType = 0  and N_BranchID=" + nBranchID + " and N_FnYearID=" + nFnYearId + " and isnull(N_Hold,0)<>1 and N_LocationID=" + n_LocationID;//+" and B_YearEndProcess=0";
                        }
                    }
                    else
                    {
                        if (bAllBranchData == true && bLocationChange == true)
                        {
                            Searchkey = Searchkey + " and X_TransType = '" + X_TransType + "' and N_SalesType = 0  and N_FnYearID=" + nFnYearId + " and isnull(N_Hold,0)<>1";
                        }
                        else
                        {
                            Searchkey = Searchkey + " and X_TransType = '" + X_TransType + "' and N_SalesType = 0  and N_FnYearID=" + nFnYearId + " and N_BranchID=" + nBranchID + " and N_LocationID =" + n_LocationID + " and isnull(N_Hold,0)<>1";
                        }
                    }
                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_SalesId desc";

                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "invoiceNo":
                                xSortBy = "N_SalesId " + xSortBy.Split(" ")[1];
                                break;
                            case "invoiceDate":
                                xSortBy = "Cast([Invoice Date] as DateTime ) " + xSortBy.Split(" ")[1];
                                break;
                            case "x_BillAmt":
                                xSortBy = "Cast(REPLACE(x_BillAmt,',','') as Numeric(16," + N_decimalPlace + ")) " + xSortBy.Split(" ")[1];
                                break;
                            case "n_BalanceAmt":
                                xSortBy = "Cast(REPLACE(n_BalanceAmt,',','') as Numeric(16," + N_decimalPlace + ")) " + xSortBy.Split(" ")[1];
                                break;
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }


                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from " + viewName + " where N_Hold=0" + Pattern + criteria + formIDCndn + cndn + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from " + viewName + " where isnull(N_Hold,0)=0 " + Pattern + criteria + formIDCndn + cndn + Searchkey + " and N_SalesID not in (select top(" + Count + ") N_SalesID from " + viewName + " where isnull(N_Hold,0)=0 " + Pattern + criteria + formIDCndn + cndn + Searchkey + xSortBy + " ) " + xSortBy;

                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearId);
                    Params.Add("@nCustomerID", nCustomerID);
                    SortedList OutPut = new SortedList();


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    // dt = myFunctions.AddNewColumnToDataTable(dt, "N_BalanceAmt", typeof(double), 0);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "N_DueDays", typeof(string), "");
                    double BalanceAmt = 0;
                    foreach (DataRow var in dt.Rows)
                    {
                        BalanceAmt = myFunctions.getVAL(var["N_BalanceAmt"].ToString());
                        if (BalanceAmt > 0)
                        {
                            if (var["N_InvDueDays"].ToString() != "")
                            {
                                DateTime dtInvoice = new DateTime();
                                DateTime dtDuedate = new DateTime();
                                dtInvoice = Convert.ToDateTime(var["Invoice Date"].ToString());
                                dtDuedate = dtInvoice.AddDays(myFunctions.getIntVAL(var["N_InvDueDays"].ToString()));
                                if (DateTime.Now > dtDuedate)
                                {
                                    var DueDays = (DateTime.Now - dtDuedate).TotalDays;
                                    string Due_Days = Math.Truncate(DueDays).ToString();
                                    var["N_DueDays"] = Due_Days.ToString() + " days";
                                }
                            }
                        }


                    }

                    if (screen == "Invoice")
                    {
                        sqlCommandCount = "SELECT count(1) as N_Count,sum(Cast(REPLACE(TotalAmount,',','') as Numeric(10,2)) ) AS  TotalAmount  FROM Vw_SalesRevenew_Cloud WHERE MONTH(Cast(D_SalesDate as DateTime)) = MONTH(CURRENT_TIMESTAMP) AND YEAR(Cast(D_SalesDate as DateTime)) = YEAR(CURRENT_TIMESTAMP) and N_FnyearID=@P2";
                    }
                    else
                    {
                        sqlCommandCount = "select count(1) as N_Count,sum(Cast(REPLACE(x_BillAmt,',','') as Numeric(16," + N_decimalPlace + ")) ) as TotalAmount from " + viewName + " where N_FnYearID=@p2 and N_Hold=0 " + Pattern + criteria + cndn + formIDCndn + Searchkey + "";
                    }

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
        [HttpGet("listOrder")]
        public ActionResult GetSalesOrderList(int nCompanyId, int nFnYearId, int nCustomerID, int nBranchId, bool bAllBranchData)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "";
            SortedList QueryProject = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    bool B_Project = myFunctions.CheckPermission(nCompanyId, 74, "Administrator", "X_UserCategory", dLayer, connection);
                    bool B_DeliveryNote = myFunctions.CheckPermission(nCompanyId, 729, "Administrator", "X_UserCategory", dLayer, connection);
                    bool B_SalesOrder = myFunctions.CheckPermission(nCompanyId, 81, "Administrator", "X_UserCategory", dLayer, connection);


                    if (B_DeliveryNote)
                    {
                        if (bAllBranchData)
                            sqlCommandText = "select N_CompanyID,N_CustomerID,N_FnYearID,N_BranchID,N_SalesOrderID,N_DeliveryNoteId,N_ProjectID from vw_Inv_DeliveryNotePending where N_CompanyID=@p1 and N_CustomerID=@p3";
                        else
                            sqlCommandText = "select N_CompanyID,N_CustomerID,N_FnYearID,N_BranchID,N_SalesOrderID,N_DeliveryNoteId,N_ProjectID from vw_Inv_DeliveryNotePending where N_CompanyID=@p1 and N_CustomerID=@p3 and N_BranchId=@p4";
                    }
                    if (B_SalesOrder)
                    {
                        if (bAllBranchData)
                            sqlCommandText = "select N_CompanyID,N_CustomerID,N_FnYearID,N_BranchID,N_Processed,B_CancelOrder,B_IsSaveDraft,N_ProjectID,SODate from vw_InvSoSearch_Sales where N_CompanyID=@p1 and B_CancelOrder=0 and N_CustomerID=@p3 and (B_IsSaveDraft=0 or B_IsSaveDraft is null)";
                        else
                            sqlCommandText = "select N_CompanyID,N_CustomerID,N_FnYearID,N_BranchID,N_Processed,B_CancelOrder,B_IsSaveDraft,N_ProjectID,SODate from vw_InvSoSearch_Sales where N_CompanyID=@p1 and B_CancelOrder=0 and N_CustomerID=@p3 and (B_IsSaveDraft=0 or B_IsSaveDraft is null) and N_BranchId=@p4";
                    }
                    else
                    {
                        if (bAllBranchData)
                            sqlCommandText = "select N_CompanyID,N_CustomerID,N_QuotationId,N_FnYearID,D_QuotationDate,N_BranchID,B_YearEndProcess,N_Processed,N_ProjectID from vw_InvSalesQuotationNo_Search where N_CompanyID=@p1 and N_Processed=0 and N_FnYearID=@p2 and N_CustomerID=@p3";
                        else
                            sqlCommandText = "select N_CompanyID,N_CustomerID,N_QuotationId,N_FnYearID,D_QuotationDate,N_BranchID,B_YearEndProcess,N_Processed,N_ProjectID from vw_InvSalesQuotationNo_Search where N_CompanyID=@p1 and N_Processed=0 and N_FnYearID=@p2 and N_CustomerID=@p3 and N_BranchID=@p4";
                    }
                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearId);
                    Params.Add("@p3", nCustomerID);
                    Params.Add("@p4", nBranchId);

                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    dt = _api.Format(dt);
                }
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
        [HttpGet("listTerms")]
        public ActionResult GetTermsList(int nCompanyId, int nFnYearId, int nSalesOrderID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "";
            SortedList QueryProject = new SortedList();
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    sqlCommandText = "select * from vw_Terms where N_CompanyID=@p1 and N_ReferanceID=@p2 and N_TermsID not in(select N_TermsID from inv_sales where n_salesorderid=@p2)";

                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nSalesOrderID);


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    OutPut.Add("Details", _api.Format(dt));
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
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
        [HttpGet("details")]
        public ActionResult GetSalesInvoiceDetails(int nCompanyId, bool bAllBranchData, int nFnYearId, int nBranchId, string xInvoiceNo, int nSalesOrderID, int nDeliveryNoteId, int isProfoma, int nQuotationID, int n_OpportunityID, int nServiceID, string xDeliveryNoteID, int nServiceSheetID, string xServiceSheetID, string xSchSalesID, bool isServiceOrder, int nFormID, bool enableDayWise, string multipleJobOrder, string serviceSheetID)
        {
            if (xInvoiceNo != null)
                xInvoiceNo = xInvoiceNo.Replace("%2F", "/");
            try
            {
                using (SqlConnection Con = new SqlConnection(connectionString))
                {
                    Con.Open();
                    DataSet dsSalesInvoice = new DataSet();
                    SortedList QueryParamsList = new SortedList();
                    DataTable DelDetails = new DataTable();
                    QueryParamsList.Add("@nCompanyID", nCompanyId);
                    QueryParamsList.Add("@nFnYearID", nFnYearId);
                    QueryParamsList.Add("@nBranchId", nBranchId);
                    QueryParamsList.Add("@xTransType", "SALES");
                    QueryParamsList.Add("@nFormID", nFormID);
                    object N_QuotationID = 0;
                    object N_SalesOrderID = 0;
                    string DetailGetSql = "";
                    int N_DeliveryNote = 0;
                    string x_DeliveryNoteNo = "";
                    bool IsDirectMRN = false;
                    string DisplayName = "";
                    int ServiceSheetID = 0;
                    //CRM Quotation Checking
                    if (n_OpportunityID > 0)
                    {
                        N_SalesOrderID = dLayer.ExecuteScalar("Select N_SalesOrderID from Inv_SalesOrder where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_OpportunityID=" + n_OpportunityID, QueryParamsList, Con);
                        if (N_SalesOrderID != null)
                            nSalesOrderID = myFunctions.getIntVAL(N_SalesOrderID.ToString());

                        N_QuotationID = dLayer.ExecuteScalar("Select N_QuotationID from Inv_SalesQuotation where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_OpportunityID=" + n_OpportunityID, QueryParamsList, Con);
                        if (N_QuotationID != null)
                            nQuotationID = myFunctions.getIntVAL(N_QuotationID.ToString());
                    }

                    if (nDeliveryNoteId > 0 || (xDeliveryNoteID != "" && xDeliveryNoteID != null))
                    {
                        DataTable MasterTable = new DataTable();
                        DataTable DeliveryNoteNumber = new DataTable();
                        int N_salesOrderID = 0;
                        string Mastersql = "";
                        // string xDeliveryNo="";



                        if (nDeliveryNoteId > 0)
                        {
                            QueryParamsList.Add("@nDeliveryNoteID", nDeliveryNoteId);
                            Mastersql = "select * from vw_DeliveryNoteDisp where N_CompanyId=@nCompanyID and N_DeliveryNoteId=@nDeliveryNoteID";
                            MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                            if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                            MasterTable = _api.Format(MasterTable, "Master");
                            N_salesOrderID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_salesOrderID"].ToString());
                            xDeliveryNoteID = nDeliveryNoteId.ToString();

                            if (xDeliveryNoteID != null)
                            {
                                object purchaseOrderNo = dLayer.ExecuteScalar("select N_SalesOrderID from Inv_DeliveryNoteDetails where N_CompanyID=" + nCompanyId + " and N_DeliveryNoteID=" + xDeliveryNoteID, QueryParamsList, Con);
                                if (purchaseOrderNo == null)
                                    purchaseOrderNo = 0;
                                if (myFunctions.getVAL(purchaseOrderNo.ToString()) > 0)
                                {
                                    IsDirectMRN = false;
                                }
                                else
                                {
                                    IsDirectMRN = true;
                                }


                            }
                        }
                        else
                        {
                            QueryParamsList.Add("@nDeliveryNoteID", nDeliveryNoteId);
                            string[] X_Delivery = xDeliveryNoteID.Split(",");
                            N_DeliveryNote = myFunctions.getIntVAL(X_Delivery[0].ToString());

                            //  MasterTable = dLayer.ExecuteDataTable(xDeliveryNo, QueryParamsList, Con);

                            Mastersql = "select * from vw_DeliveryNoteDisp where N_CompanyId=@nCompanyID and N_DeliveryNoteId=" + N_DeliveryNote + "";
                            MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                            if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                            MasterTable = _api.Format(MasterTable, "Master");


                        }

                        string DetailSql = "";
                        //  DetailSql = "select * from vw_DeliveryNoteDispDetails where N_CompanyId=@nCompanyID and N_DeliveryNoteID=@nDeliveryNoteID ";



                        string DeliveryNoteAppend = "0";
                        DataTable DeliveryNoteID = new DataTable();

                        if (nFormID != 1601)
                        {
                            if (N_salesOrderID > 0)
                                DeliveryNoteID = dLayer.ExecuteDataTable("select N_DeliveryNoteID from Inv_SalesDetails Where N_SalesOrderID=" + N_salesOrderID + "", QueryParamsList, Con);
                        }

                        if (DeliveryNoteID.Rows.Count > 0)
                        {

                            foreach (DataRow Avar in DeliveryNoteID.Rows)
                            {
                                if (Avar["N_DeliveryNoteID"].ToString() != "0")
                                    DeliveryNoteAppend = DeliveryNoteAppend + "," + Avar["N_DeliveryNoteID"].ToString();
                            }
                            if (xDeliveryNoteID == "" || xDeliveryNoteID == null)
                                DetailSql = "select * from vw_DeliveryNoteDispDetails where N_CompanyId=@nCompanyID and N_SalesOrderID =" + N_salesOrderID + " and N_DeliveryNoteID not in( " + DeliveryNoteAppend + ") order by N_SOdetailsID ASC ";
                            else
                                DetailSql = "select * from vw_DeliveryNoteDispDetails where N_CompanyId=@nCompanyID and N_DeliveryNoteID IN (" + xDeliveryNoteID + ")  and N_DeliveryNoteID not in( " + DeliveryNoteAppend + ") order by N_SOdetailsID ASC ";

                        }
                        else
                        {
                            if ((xDeliveryNoteID == "" || xDeliveryNoteID == null) && N_salesOrderID > 0)
                                DetailSql = "select * from vw_DeliveryNoteDispDetails where N_CompanyId=@nCompanyID and N_SalesOrderID =" + N_salesOrderID + " order by N_SOdetailsID ASC ";
                            else
                                DetailSql = "select * from vw_DeliveryNoteDispDetails where N_CompanyId=@nCompanyID and N_DeliveryNoteID IN (" + xDeliveryNoteID + ")  order by N_SOdetailsID ASC ";

                        }


                        DataTable DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);
                        DetailTable = _api.Format(DetailTable, "Details");


                        int EnableInvoicebasedtax = myFunctions.getIntVAL(myFunctions.ReturnSettings("64", "EnableInvoicebasedtax", "N_Value", nCompanyId, dLayer, Con));
                        if (EnableInvoicebasedtax == 0 && IsDirectMRN == true)
                        {
                            DataTable deliveryDetails = new DataTable();
                            string directDelivery = "SELECT Acc_TaxCategory.X_DisplayName,Inv_DeliveryNoteDetails.N_DeliveryNoteID, Acc_TaxCategory.N_Amount as N_TaxAmt1, Inv_ItemCategory.N_TaxCategoryID as N_TaxCategoryID1, Inv_DeliveryNote.N_FnYearId, Inv_DeliveryNote.N_CompanyId,Inv_ItemMaster.N_ItemID FROM Inv_ItemCategory INNER JOIN Inv_DeliveryNoteDetails INNER JOIN Inv_DeliveryNote ON Inv_DeliveryNoteDetails.N_DeliveryNoteID = Inv_DeliveryNote.N_DeliveryNoteId AND Inv_DeliveryNoteDetails.N_CompanyID = Inv_DeliveryNote.N_CompanyId INNER JOIN Inv_ItemMaster ON Inv_DeliveryNoteDetails.N_ItemID = Inv_ItemMaster.N_ItemID AND Inv_DeliveryNoteDetails.N_CompanyID = Inv_ItemMaster.N_CompanyID INNER JOIN Acc_FnYear ON Inv_DeliveryNote.N_FnYearId = Acc_FnYear.N_FnYearID AND Inv_DeliveryNote.N_CompanyId = Acc_FnYear.N_CompanyID ON Inv_ItemCategory.N_CategoryID = Inv_ItemMaster.N_CategoryID AND Inv_ItemCategory.N_CompanyID = Inv_ItemMaster.N_CompanyID INNER JOIN Acc_TaxCategory ON Inv_ItemCategory.N_TaxCategoryID = Acc_TaxCategory.N_PkeyID AND Inv_ItemCategory.N_CompanyID = Acc_TaxCategory.N_CompanyID where (Inv_DeliveryNoteDetails.n_deliveryNoteID =@nDeliveryNoteID)";
                            deliveryDetails = dLayer.ExecuteDataTable(directDelivery, QueryParamsList, Con);
                            // string DisplayName = "";


                            foreach (DataRow dr in deliveryDetails.Rows)
                            {
                                foreach (DataRow Row in DetailTable.Rows)
                                {

                                    if (dr["N_ItemID"].ToString() == Row["N_ItemID"].ToString())
                                    {
                                        Row["x_DisplayName"] = dr["X_DisplayName"].ToString();
                                        Row["n_TaxPercentage1"] = dr["N_TaxAmt1"];
                                        Row["n_TaxCategoryID1"] = dr["n_TaxCategoryID1"];
                                        Row["n_TaxAmt1"] = dr["N_TaxAmt1"];
                                    }
                                }
                            }
                        }
                        DetailTable.AcceptChanges();

                        if (IsDirectMRN && EnableInvoicebasedtax == 1)
                        {
                            object taxID = dLayer.ExecuteScalar("Select N_Value from Gen_Settings where N_CompanyId=" + nCompanyId + " and X_Description='DefaultTaxCategory' and X_Group='Inventory'", QueryParamsList, Con);
                            if (taxID != null)
                            {
                                object category = dLayer.ExecuteScalar("Select X_DisplayName from Acc_TaxCategory where N_CompanyId=" + nCompanyId + " and X_PkeyCode=" + taxID + " ", QueryParamsList, Con);
                                object taxCatID = dLayer.ExecuteScalar("Select N_PkeyID from Acc_TaxCategory where N_CompanyId=" + nCompanyId + " and X_PkeyCode=" + taxID + " ", QueryParamsList, Con);
                                object percentage = dLayer.ExecuteScalar("Select Cast(REPLACE(N_Amount,',','') as Numeric(10,0)) from Acc_TaxCategory where N_CompanyId=" + nCompanyId + " and X_PkeyCode=" + taxID + " ", QueryParamsList, Con);

                                MasterTable.Rows[0]["X_DisplayName"] = category.ToString();
                                MasterTable.Rows[0]["n_TaxCategoryID"] = myFunctions.getIntVAL(taxCatID.ToString());
                                MasterTable.Rows[0]["n_TaxPercentage"] = myFunctions.getIntVAL(percentage.ToString());
                                MasterTable.Rows[0]["n_TaxAmt"] = myFunctions.getIntVAL(percentage.ToString());
                            }
                            MasterTable.AcceptChanges();

                        }

                        if (nDeliveryNoteId > 0 || N_DeliveryNote > 0)
                        {
                            if (myFunctions.getIntVAL(MasterTable.Rows[0]["N_SalesOrderID"].ToString()) > 0)
                            {

                                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "n_DiscountDisplay", typeof(double), 0);
                                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "n_DiscountAmtF", typeof(double), 0);
                                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "n_TaxAmtF", typeof(double), 0);
                                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_DisplayName", typeof(string), null);
                                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "n_TaxPercentage", typeof(int), 0);



                                string sql1 = "Select * from Inv_SalesOrder  where N_SalesOrderID=" + (myFunctions.getIntVAL(MasterTable.Rows[0]["N_SalesOrderID"].ToString())) + " and N_CompanyId=" + nCompanyId + "";
                                DataTable AdditionalTable = dLayer.ExecuteDataTable(sql1, QueryParamsList, Con);
                                MasterTable.Rows[0]["n_DiscountDisplay"] = myFunctions.getVAL(AdditionalTable.Rows[0]["n_DiscountDisplay"].ToString());
                                MasterTable.Rows[0]["n_DiscountAmtF"] = myFunctions.getVAL(AdditionalTable.Rows[0]["N_DiscountAmtF"].ToString());
                                MasterTable.Rows[0]["n_TaxAmtF"] = myFunctions.getVAL(AdditionalTable.Rows[0]["N_TaxAmtF"].ToString());
                                MasterTable.Rows[0]["n_TaxPercentage"] = myFunctions.getVAL(AdditionalTable.Rows[0]["N_TaxPercentage"].ToString());


                                int N_OthTaxCategoryID = myFunctions.getIntVAL(AdditionalTable.Rows[0]["N_TaxCategoryID"].ToString());
                                string X_OtherTax1 = "";
                                object X_OtherTax = dLayer.ExecuteScalar("Select X_DisplayName from Acc_TaxCategory where N_PkeyID=" + N_OthTaxCategoryID, QueryParamsList, Con);
                                if (X_OtherTax != null)
                                    X_OtherTax1 = X_OtherTax.ToString();
                                MasterTable.Rows[0]["x_DisplayName"] = X_OtherTax1;


                            }
                        }
                        else
                        {
                            // MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_DisplayName", typeof(string), null);
                            //  MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "n_TaxPercentage", typeof(int), 0);
                            // object taxID = dLayer.ExecuteScalar("Select N_Value from Gen_Settings where N_CompanyId=" + nCompanyId+" and X_Description='DefaultTaxCategory' and X_Group='Inventory'", QueryParamsList, Con);
                            // if(taxID!=null)
                            // {
                            //   object category = dLayer.ExecuteScalar("Select X_DisplayName from Acc_TaxCategory where N_PkeyID=" + taxID+" ", QueryParamsList, Con);
                            //   object percentage = dLayer.ExecuteScalar("Select Cast(REPLACE(N_Amount,',','') as Numeric(10,0)) from Acc_TaxCategory where N_PkeyID=" + taxID+" ", QueryParamsList, Con);

                            //     MasterTable.Rows[0]["X_DisplayName"] = category.ToString();
                            //     MasterTable.Rows[0]["N_TaxCategoryID1"] = myFunctions.getIntVAL(taxID.ToString());
                            //     MasterTable.Rows[0]["n_TaxPercentage"] =myFunctions.getIntVAL(percentage.ToString());
                            // }
                            // MasterTable.AcceptChanges();


                        }





                        dsSalesInvoice.Tables.Add(MasterTable);
                        dsSalesInvoice.Tables.Add(DetailTable);





                        return Ok(_api.Success(dsSalesInvoice));

                    }
                    else if (multipleJobOrder != null && multipleJobOrder != "")
                    {
                        string[] X_ServiceSheetID = serviceSheetID.Split(",");
                        ServiceSheetID = myFunctions.getIntVAL(X_ServiceSheetID[0].ToString());
                        string Mastersql = "select * from vw_ServiceTimesheetToSales where N_CompanyId=@nCompanyID and N_SalesOrderId in (" + multipleJobOrder + ") and N_ServiceSheetID in (" + ServiceSheetID + ")";
                        DataTable MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }

                        MasterTable = _api.Format(MasterTable, "Master");

                        if (isProfoma == 1)
                        {
                            MasterTable.Rows[0]["B_IsSaveDraft"] = 1;
                            MasterTable.Rows[0]["B_IsProforma"] = 1;
                        }

                        string DetailSql = "";
                        DetailSql = "select * from vw_ServiceTimesheetDetailsToSales where N_CompanyId=@nCompanyID and N_SalesOrderId in (" + multipleJobOrder + ") and N_ServiceSheetID in (" + serviceSheetID + ")";
                        DataTable DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);
                        DetailTable = _api.Format(DetailTable, "Details");

                        dsSalesInvoice.Tables.Add(MasterTable);
                        dsSalesInvoice.Tables.Add(DetailTable);
                        return Ok(_api.Success(dsSalesInvoice));
                    }
                    else if (isServiceOrder && nSalesOrderID > 0)
                    {
                        QueryParamsList.Add("@nOrderID", nSalesOrderID);
                        string Mastersql = "select *,0 as B_IsProforma from vw_SalesOrderMasterToInvoice where N_CompanyId=@nCompanyID and N_SalesOrderId=@nOrderID";
                        DataTable MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        if (!MasterTable.Columns.Contains("N_OpportunityID"))
                        {
                            MasterTable.Columns.Add("N_OpportunityID");
                            MasterTable.Rows[0]["N_OpportunityID"] = n_OpportunityID.ToString();
                        }
                        MasterTable = _api.Format(MasterTable, "Master");
                        if (isProfoma == 1)
                        {
                            MasterTable.Rows[0]["B_IsSaveDraft"] = 1;
                            MasterTable.Rows[0]["B_IsProforma"] = 1;
                        }

                        string DetailSql = "";
                        DetailSql = "select X_ItemName, X_ItemName_a, 0 AS n_SalesId, d_SalesDate, N_BillAmt, N_DiscountAmt, N_FreightAmt, N_CashReceived, N_UserID, 0 AS n_SalesDetailsID, N_Qty, N_Sprice, X_ItemCode, N_CompanyId,N_CustomerID, X_CustomerName, X_Address, N_MainQty, N_MainSPrice, N_ItemID, X_CustomerCode, X_PhoneNo1, X_PhoneNo2, N_SalesOrderId, N_ItemUnitID, X_ItemUnit, X_BaseUnit, N_UnitQty, N_MinimumMargin,N_QtyDisplay, X_ItemRemarks,"+
                        "+N_ClassID, B_IsIMEI, X_FreeDescription, N_Cost,  unitCost, N_LocationID, N_BranchID,  d_ExpiryDate, N_QuotationID, N_SPriceTypeID, X_QuotationNo,N_TaxCategoryID1, N_TaxPercentage1, N_TaxAmt1, N_TaxCategoryID2, N_TaxPercentage2, N_TaxAmt2, X_DisplayName, N_PkeyID, X_DisplayName2, Expr1, n_TaxPerc1,N_TaxPercentage2 AS n_TaxPerc2, N_SalesOrderDetailsID, X_CustomerName_Ar, X_OrderNo,  n_Stock, n_StockOnHand, n_LPrice,"+
                        "+dbo.SP_SellingPrice(N_ItemID, N_CompanyId) AS n_UnitSPrice, X_PartNo, n_ItemDiscAmt, N_ItemDiscAmtF, N_CashReceivedF, N_MainDiscountF, N_OthTaxAmtF, N_TaxAmtF, N_TaxAmt1F, N_SPriceF AS N_SpriceF, N_TaxAmt2F, N_CostF, N_BillAmtF, N_MainDiscount, N_DiscountAmtF, N_MainSpriceF, N_UnitAddlAmtF, N_AddlAmtF,N_FreightAmtF, N_MainProjectID, X_ProjectName, N_ProjectID, N_Amount, N_Amount2, N_CategoryID, X_Category, N_AWTSPrice,"+
                        "+ N_AWTSPriceF, N_AWTDisc, N_AWTDiscF,N_SalesUnitQty ,N_MainItemID,B_HideInPrint,N_ServiceID,0 as N_MaterialID,0 as N_DisplayQty,0 as N_TaskID  from vw_SalesOrderDetailsToInvoice where N_CompanyId=@nCompanyID and N_SalesOrderId=@nOrderID union select * from vw_ServiceDetailsItems_Sales where N_CompanyId=@nCompanyID and N_ServiceID in (select N_ServiceID from Inv_SalesOrderDetails where N_CompanyId=@nCompanyID and N_SalesOrderId=@nOrderID )";
                        DataTable DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dsSalesInvoice.Tables.Add(MasterTable);
                        dsSalesInvoice.Tables.Add(DetailTable);
                        return Ok(_api.Success(dsSalesInvoice));
                    }
                    else
                  if (nSalesOrderID > 0)
                    {

                        QueryParamsList.Add("@nOrderID", nSalesOrderID);
                        string Mastersql = "select *,0 as B_IsProforma from vw_SalesOrderMasterToInvoice where N_CompanyId=@nCompanyID and N_SalesOrderId=@nOrderID";
                        DataTable MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        if (!MasterTable.Columns.Contains("N_OpportunityID"))
                        {
                            MasterTable.Columns.Add("N_OpportunityID");
                            MasterTable.Rows[0]["N_OpportunityID"] = n_OpportunityID.ToString();
                        }
                        MasterTable = _api.Format(MasterTable, "Master");
                        if (isProfoma == 1)
                        {
                            MasterTable.Rows[0]["B_IsSaveDraft"] = 1;
                            MasterTable.Rows[0]["B_IsProforma"] = 1;
                        }

                        string DetailSql = "";
                        DetailSql = "select * from vw_SalesOrderDetailsToInvoice where N_CompanyId=@nCompanyID and N_SalesOrderId=@nOrderID";
                        DataTable DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);
                        DetailTable = _api.Format(DetailTable, "Details");

                         DataTable dtTermsDetails = dLayer.ExecuteDataTable("Select * from vw_Termsdetails where N_CompanyId=" + nCompanyId + " and N_ReferanceID=" + nSalesOrderID+" order by n_termsid", QueryParamsList, Con);

                    if (dtTermsDetails.Rows.Count > 0)
                    {
                        string Termstype = "";
                        int TermsID = 0;
                        object SeqID ="";
                        foreach (DataRow dr in dtTermsDetails.Rows)
                        {
                            Termstype = dr["x_typename"].ToString();
                            TermsID = myFunctions.getIntVAL(dr["N_TermsID"].ToString());
                            object ProcTermsID = dLayer.ExecuteScalar("Select n_termsid from inv_salesdetails where N_CompanyId=" + nCompanyId + " and N_SalesOrderId=" + nSalesOrderID + " and N_termsID=" + TermsID, QueryParamsList, Con);
                            if (ProcTermsID == null)
                                break;
                        }

                       
                        if (Termstype == "Invoice")
                        {
                            SeqID = dLayer.ExecuteScalar("Select n_sequenceID from vw_Termsdetails where N_CompanyId=" + nCompanyId + " and N_ReferanceID=" + nSalesOrderID + " and X_TypeName='" + Termstype + "' and N_TermsID="+TermsID, QueryParamsList, Con);
                            DetailSql = "select *,'Invoice' as x_typename,0 as n_sequenceid,0 as N_TermsID  from vw_SalesOrderDetailsToInvoice where N_CompanyId=@nCompanyID and N_SalesOrderId=@nOrderID union select * from vw_TermsSalesOrderDetailsToInvoice where N_CompanyId=@nCompanyID and N_SalesOrderId=@nOrderID and n_sequenceid='" + SeqID + "' order by n_sequenceid";
                        }
                        else
                            DetailSql = "select * from vw_TermsSalesOrderDetailsToInvoice where N_CompanyId=@nCompanyID and N_SalesOrderId=@nOrderID and x_typename='" + Termstype + "'";
                        DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);
                        foreach (DataRow dr in DetailTable.Rows)
                        {
                            object obj = dLayer.ExecuteScalar("select N_Percentage from vw_Termsdetails where N_CompanyID=@nCompanyID and N_ReferanceID=@nOrderID and n_sequenceid="+SeqID + " and x_typename='Invoice'", QueryParamsList, Con);
                            object Terms = dLayer.ExecuteScalar("select X_Terms from vw_Termsdetails where N_CompanyID=@nCompanyID and N_ReferanceID=@nOrderID and  n_sequenceid="+SeqID+ " and x_typename='Invoice'", QueryParamsList, Con);
                            object ID = dLayer.ExecuteScalar("select N_TermsID from vw_Termsdetails where N_CompanyID=@nCompanyID and N_ReferanceID=@nOrderID and  n_sequenceid="+SeqID+ " and x_typename='Invoice'", QueryParamsList, Con);
                            if (obj != null && dr["x_typename"].ToString() == "Invoice")
                            {
                                dr["n_SpriceF"] = (myFunctions.getVAL(dr["n_SpriceF"].ToString()) * myFunctions.getVAL(obj.ToString()) / 100).ToString();
                                dr["n_ItemDiscAmtF"] = (myFunctions.getVAL(dr["n_ItemDiscAmtF"].ToString()) * myFunctions.getVAL(obj.ToString()) / 100).ToString();
                                
                                if (Terms != null)
                                {
                                    dr["x_FreeDescription"] = Terms.ToString();
                                    dr["N_TermsID"] = ID.ToString();
                                }
                            }

                        }
                        DetailTable = _api.Format(DetailTable, "Details");
                    }
                      
                        //Eye Optics
                        string sqlPrescription1 = "select * from Inv_Prescription where N_SalesOrderID=@nOrderID";
                        DataTable Prescription = dLayer.ExecuteDataTable(sqlPrescription1, QueryParamsList, Con);
                        Prescription = _api.Format(Prescription, "Prescription");



                        dsSalesInvoice.Tables.Add(MasterTable);
                        dsSalesInvoice.Tables.Add(DetailTable);
                        dsSalesInvoice.Tables.Add(Prescription);
                        return Ok(_api.Success(dsSalesInvoice));

                    }

                    else
                  if (nServiceID > 0)
                    {
                        QueryParamsList.Add("@nServiceID", nServiceID);
                        string Mastersql = "select * from Vw_InvServiceMasterToInvoice where N_CompanyId=@nCompanyID and N_ServiceID=@nServiceID";
                        DataTable MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = _api.Format(MasterTable, "Master");
                        string DetailSql = "";
                        DetailSql = "select * from Vw_ServiceDetailsToInvoice where N_CompanyId=@nCompanyID and N_ServiceID=@nServiceID ";
                        DataTable DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dsSalesInvoice.Tables.Add(MasterTable);
                        dsSalesInvoice.Tables.Add(DetailTable);
                        return Ok(_api.Success(dsSalesInvoice));


                    }
                    else
                  if (nQuotationID > 0)
                    {

                        QueryParamsList.Add("@nQuotationID", nQuotationID);
                        string Mastersql = "select *,0 as B_IsProforma from vw_QuotationToInvoice where N_CompanyId=@nCompanyID and N_QuotationId=@nQuotationID";
                        DataTable MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = _api.Format(MasterTable, "Master");
                        if (isProfoma == 1)
                        {
                            MasterTable.Rows[0]["B_IsSaveDraft"] = 1;
                            MasterTable.Rows[0]["B_IsProforma"] = 1;
                        }

                        Object CRMCustomerID = null;
                        Object CustomerName = null;
                        object CustomerID = dLayer.ExecuteScalar("Select n_customerID from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CrmCompanyID=" + myFunctions.getIntVAL(MasterTable.Rows[0]["n_CrmCompanyID"].ToString()), QueryParamsList, Con);

                        if (CustomerID != null)
                        {
                            object xCustomer = dLayer.ExecuteScalar("Select x_CustomerName from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=" + myFunctions.getIntVAL(CustomerID.ToString()), QueryParamsList, Con);
                            MasterTable.Rows[0]["N_CustomerID"] = CustomerID.ToString();
                            MasterTable.Rows[0]["x_CustomerName"] = xCustomer.ToString();
                            MasterTable.AcceptChanges();
                        }
                        if (n_OpportunityID > 0)
                        {
                            Object N_CRMCompanyID = dLayer.ExecuteScalar("Select n_customerID from crm_opportunity where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_OpportunityID=" + n_OpportunityID, QueryParamsList, Con);
                            if (N_CRMCompanyID != null)
                            {
                                CRMCustomerID = dLayer.ExecuteScalar("Select n_customerID from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CrmCompanyID=" + N_CRMCompanyID, QueryParamsList, Con);
                                CustomerName = dLayer.ExecuteScalar("Select X_CustomerName from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CrmCompanyID=" + N_CRMCompanyID, QueryParamsList, Con);
                                if (!MasterTable.Columns.Contains("N_OpportunityID"))
                                {
                                    MasterTable.Columns.Add("N_OpportunityID");
                                    MasterTable.Rows[0]["N_OpportunityID"] = n_OpportunityID.ToString();
                                }
                                if (CRMCustomerID != null)
                                {
                                    MasterTable.Rows[0]["N_CustomerID"] = CRMCustomerID.ToString();
                                    MasterTable.Rows[0]["x_CustomerName"] = CustomerName.ToString();
                                    MasterTable.AcceptChanges();
                                }
                            }
                        }



                        string DetailSql = "";
                        DetailSql = "select * from vw_QuotationDetailsToInvoice where N_CompanyId=@nCompanyID and N_QuotationId=@nQuotationID";
                        DataTable DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dsSalesInvoice.Tables.Add(MasterTable);
                        dsSalesInvoice.Tables.Add(DetailTable);
                        return Ok(_api.Success(dsSalesInvoice));

                    }
                    else if (n_OpportunityID > 0)
                    {

                        QueryParamsList.Add("@nOpportunityID", n_OpportunityID);
                        string Mastersql = "select *,0 as B_IsProforma from vw_OpportunityToInvoice where N_CompanyId=@nCompanyID and N_OpportunityID=@nOpportunityID";
                        DataTable MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = _api.Format(MasterTable, "Master");
                        if (isProfoma == 1)
                        {
                            MasterTable.Rows[0]["B_IsSaveDraft"] = 1;
                            MasterTable.Rows[0]["B_IsProforma"] = 1;
                        }

                        string DetailSql = "";
                        DetailSql = "select * from vw_OpportunityDetailsToInvoice where N_CompanyId=@nCompanyID and N_OpportunityID=@nOpportunityID";
                        DataTable DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dsSalesInvoice.Tables.Add(MasterTable);
                        dsSalesInvoice.Tables.Add(DetailTable);
                        return Ok(_api.Success(dsSalesInvoice));

                    }
                    else if (nServiceSheetID > 0 || (xServiceSheetID != "" && xServiceSheetID != null))
                    {
                        if (nServiceSheetID > 0)
                            xServiceSheetID = nServiceSheetID.ToString();
                        else
                        {
                            string[] xServiceSheet = xServiceSheetID.Split(",");
                            nServiceSheetID = myFunctions.getIntVAL(xServiceSheet[0].ToString());
                        }

                        QueryParamsList.Add("@nServiceSheetID", nServiceSheetID);
                        string Mastersql = "select * from vw_ServiceTimesheetToSales where N_CompanyId=@nCompanyID and N_ServiceSheetID=@nServiceSheetID";
                        DataTable MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = _api.Format(MasterTable, "Master");
                        if (isProfoma == 1)
                        {
                            MasterTable.Rows[0]["B_IsSaveDraft"] = 1;
                            MasterTable.Rows[0]["B_IsProforma"] = 1;
                        }

                        string DetailSql = "";
                        if (enableDayWise)
                            DetailSql = "select * from vw_ServiceTimesheetDetailsToSalesDaywise where N_CompanyId=@nCompanyID and N_ServiceSheetID in ( " + xServiceSheetID + " )";
                        else
                            DetailSql = "select * from vw_ServiceTimesheetDetailsToSales where N_CompanyId=@nCompanyID and N_ServiceSheetID in ( " + xServiceSheetID + " ) order by N_SalesOrderDetailsID";

                        DataTable DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dsSalesInvoice.Tables.Add(MasterTable);
                        dsSalesInvoice.Tables.Add(DetailTable);
                        return Ok(_api.Success(dsSalesInvoice));

                    }
                    else if (xSchSalesID != "" && xSchSalesID != null)
                    {

                        string[] X_SchID = xSchSalesID.Split(",");
                        int N_SchID = myFunctions.getIntVAL(X_SchID[0].ToString());
                        string Mastersql = "select * from Vw_FeeDetailsToSalesMaster where N_SchSalesID=" + N_SchID + " and N_CompanyID=" + nCompanyId + "";
                        DataTable MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = _api.Format(MasterTable, "Master");

                        string DetailSql = "";
                        DetailSql = "select * from Vw_FeeDetailsToSalesDetails where N_CompanyId=@nCompanyID and  N_SchSalesID in (" + xSchSalesID + ")";
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





                    SortedList mParamsList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"X_ReceiptNo",xInvoiceNo},
                        {"X_TransType","SALES"},
                        {"N_FnYearID",nFnYearId},

                    };
                    if (!bAllBranchData)
                    {
                        mParamsList.Add("N_BranchId", nBranchId);
                    }
                    else
                    {
                        mParamsList.Add("N_BranchId", 0);
                    }
                    DataTable masterTable = dLayer.ExecuteDataTablePro("SP_InvSales_Disp", mParamsList, Con);
                    masterTable = _api.Format(masterTable, "Master");
                    if (masterTable.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                    DataRow MasterRow = masterTable.Rows[0];
                    int nSalesID = myFunctions.getIntVAL(MasterRow["N_SalesID"].ToString());
                    QueryParamsList.Add("@nSalesID", nSalesID);






                    DetailGetSql = "select X_ReceiptNo from Inv_DeliveryNote where N_CompanyID=@nCompanyID and N_DeliveryNoteID in ( select N_DeliveryNoteID from Inv_SalesDetails where N_CompanyID=@nCompanyID and  N_SalesID=@nSalesID)";
                    DelDetails = dLayer.ExecuteDataTable(DetailGetSql, QueryParamsList, Con);
                    if (DelDetails.Rows.Count > 0)
                    {
                        x_DeliveryNoteNo = DelDetails.Rows[0]["X_ReceiptNo"].ToString();
                        for (int j = 1; j < DelDetails.Rows.Count; j++)
                        {
                            x_DeliveryNoteNo = x_DeliveryNoteNo + "," + DelDetails.Rows[j]["X_ReceiptNo"].ToString();
                        }
                    }
                    myFunctions.AddNewColumnToDataTable(masterTable, "X_DeliveryNoteNo", typeof(string), x_DeliveryNoteNo);
                    masterTable.AcceptChanges();


                    int N_TruckID = myFunctions.getIntVAL(MasterRow["N_TruckID"].ToString());
                    object objPlateNo = null;
                    if (N_TruckID > 0)
                    {
                        myFunctions.AddNewColumnToDataTable(masterTable, "X_PlateNo", typeof(string), "");
                        QueryParamsList.Add("@nTruckID", myFunctions.getIntVAL(masterTable.Rows[0]["N_TermsID"].ToString()));
                        objPlateNo = dLayer.ExecuteScalar("Select X_PlateNumber from Inv_TruckMaster where N_TruckID=@nTruckID and N_companyID=@nCompanyID", QueryParamsList, Con);
                        if (objPlateNo != null)
                            masterTable.Rows[0]["X_PlateNo"] = objPlateNo.ToString();
                    }

                    // if (masterTable.Rows[0]["X_TandC"].ToString() == "")
                    //     masterTable.Rows[0]["X_TandC"] = myFunctions.ReturnSettings("64", "TermsandConditions", "X_Value", "N_UserCategoryID", "0", QueryParamsList, dLayer, Con);
                    int N_TermsID = myFunctions.getIntVAL(masterTable.Rows[0]["N_TermsID"].ToString());
                    if (N_TermsID > 0)
                    {
                        myFunctions.AddNewColumnToDataTable(masterTable, "X_Terms", typeof(string), "");
                        QueryParamsList.Add("@nTermsID", myFunctions.getIntVAL(masterTable.Rows[0]["N_TermsID"].ToString()));
                        masterTable.Rows[0]["X_Terms"] = myFunctions.ReturnValue("Inv_Terms", "X_Terms", "N_TermsID =@nTermsID and N_CompanyID =@nCompanyID", QueryParamsList, dLayer, Con);
                    }

                    if (myFunctions.getIntVAL(masterTable.Rows[0]["N_DeliveryNoteId"].ToString()) > 0)
                    {
                        QueryParamsList.Add("@nDeliveryNoteId", myFunctions.getIntVAL(masterTable.Rows[0]["N_DeliveryNoteId"].ToString()));
                        myFunctions.AddNewColumnToDataTable(masterTable, "X_FileNo", typeof(string), "");
                        SortedList ProParamList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"N_FnYearID",nFnYearId},
                        {"N_PkID",myFunctions.getIntVAL(masterTable.Rows[0]["N_DeliveryNoteId"].ToString())},
                        {"Type","DN"}
                    };
                        object objFileNo = dLayer.ExecuteScalarPro("SP_GetSalesOrder", ProParamList, Con);
                        if (objFileNo != null)
                            masterTable.Rows[0]["X_FileNo"] = objFileNo.ToString();
                    }






                    object objPayment = dLayer.ExecuteScalar("SELECT dbo.Inv_PayReceipt.X_Type, dbo.Inv_PayReceiptDetails.N_InventoryId,Inv_PayReceiptDetails.N_Amount FROM  dbo.Inv_PayReceipt INNER JOIN dbo.Inv_PayReceiptDetails ON dbo.Inv_PayReceipt.N_PayReceiptId = dbo.Inv_PayReceiptDetails.N_PayReceiptId Where dbo.Inv_PayReceipt.X_Type='SR' and dbo.Inv_PayReceiptDetails.N_InventoryId=@nSalesID and dbo.Inv_PayReceiptDetails.N_CompanyID=@nCompanyID", QueryParamsList, Con);
                    if (objPayment != null)
                        myFunctions.AddNewColumnToDataTable(masterTable, "B_PaymentProcessed", typeof(Boolean), true);
                    else
                        myFunctions.AddNewColumnToDataTable(masterTable, "B_PaymentProcessed", typeof(Boolean), false);

                    //sales return count(draft and non draft)
                    object objSalesReturn = dLayer.ExecuteScalar("select X_DebitNoteNo from Inv_SalesReturnMaster where N_SalesId =@nSalesID and isnull(B_IsSaveDraft,0)=0 and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", QueryParamsList, Con);

                    object DNQty = dLayer.ExecuteScalar("Select SUM(N_RetQty) from  VW_SALESRETURN_DETAILS_CLOUD where N_CompanyID=" + nCompanyId + " and N_SalesID=@nSalesID", QueryParamsList, Con);
                    object OrderQty1 = dLayer.ExecuteScalar("Select SUM(N_Qty) from Inv_SalesDetails Where N_CompanyID=" + nCompanyId + "and N_SalesID=@nSalesID", QueryParamsList, Con);
                    if (DNQty != null && OrderQty1 != null)
                    {
                        if (myFunctions.getVAL(OrderQty1.ToString()) > 0 && myFunctions.getVAL(OrderQty1.ToString()) != myFunctions.getVAL(DNQty.ToString()))
                        {
                            objSalesReturn = null;
                        }
                    }

                    myFunctions.AddNewColumnToDataTable(masterTable, "X_DebitNoteNo", typeof(string), objSalesReturn);

                    object objSalesReturnDraft = dLayer.ExecuteScalar("select Isnull(Count(N_DebitNoteId),0) from Inv_SalesReturnMaster where N_SalesId =@nSalesID and B_IsSaveDraft=1 and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", QueryParamsList, Con);
                    if (objSalesReturnDraft != null)
                        myFunctions.AddNewColumnToDataTable(masterTable, "N_SalesReturnDraft", typeof(int), myFunctions.getIntVAL(objSalesReturnDraft.ToString()));
                    QueryParamsList.Add("@nCustomerID", masterTable.Rows[0]["N_CustomerID"].ToString());
                    object obPaymentMenthodid = dLayer.ExecuteScalar("Select N_TypeID From vw_InvCustomer Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID and (N_BranchID=0 or N_BranchID=@nBranchID) and B_Inactive = 0", QueryParamsList, Con);
                    if (obPaymentMenthodid != null)
                    {
                        QueryParamsList.Add("@nPaymentMethodID", myFunctions.getIntVAL(obPaymentMenthodid.ToString()));
                        // myFunctions.AddNewColumnToDataTable(masterTable, "N_PaymentMethodID", typeof(int), myFunctions.getIntVAL(obPaymentMenthodid.ToString()));
                        myFunctions.AddNewColumnToDataTable(masterTable, "X_PaymentMethod", typeof(string), myFunctions.ReturnValue("Inv_CustomerType", "X_TypeName", "N_TypeID =@nPaymentMethodID", QueryParamsList, dLayer, Con));
                    }

                    string qry = "";
                    bool B_DeliveryDispatch = myFunctions.CheckPermission(nCompanyId, 948, "Administrator", "X_UserCategory", dLayer, Con);
                    if (B_DeliveryDispatch)
                    {
                        DataTable dtDispatch = new DataTable();
                        qry = "Select * From Inv_DeliveryDispatch Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_InvoiceID=@nSalesID";
                        dtDispatch = dLayer.ExecuteDataTable(qry, QueryParamsList, Con);
                        dtDispatch = _api.Format(dtDispatch, "Delivery Dispatch");
                        dsSalesInvoice.Tables.Add(dtDispatch);
                    }

                    //invoice status
                    object objInvoiceRecievable = null, objBal = null;
                    double N_InvoiceRecievable = 0, N_BalanceAmt = 0;

                    objInvoiceRecievable = dLayer.ExecuteScalar("SELECT isnull((Inv_Sales.N_BillAmt-Inv_Sales.N_DiscountAmt + Inv_Sales.N_FreightAmt +isnull(Inv_Sales.N_OthTaxAmt,0)+ Inv_Sales.N_TaxAmt),0) as N_InvoiceAmount FROM Inv_Sales where Inv_Sales.N_SalesId=@nSalesID and Inv_Sales.N_CompanyID=@nCompanyID", QueryParamsList, Con);
                    objBal = dLayer.ExecuteScalar("SELECT SUM(N_BalanceAmount) from  vw_InvReceivables where N_SalesId=@nSalesID and X_Type= @xTransType and N_CompanyID=@nCompanyID", QueryParamsList, Con);
                    if (objInvoiceRecievable != null)
                        myFunctions.AddNewColumnToDataTable(masterTable, "N_InvoiceRecievable", typeof(double), N_InvoiceRecievable);
                    if (objBal != null)
                        myFunctions.AddNewColumnToDataTable(masterTable, "N_BalanceAmt", typeof(double), N_BalanceAmt);

                    DataTable dtPayment = new DataTable();
                    string qry1 = "SELECT  dbo.Inv_PayReceipt.X_VoucherNo FROM  dbo.Inv_PayReceipt INNER JOIN dbo.Inv_PayReceiptDetails ON dbo.Inv_PayReceipt.N_PayReceiptId = dbo.Inv_PayReceiptDetails.N_PayReceiptId Where dbo.Inv_PayReceipt.X_Type='SR' and dbo.Inv_PayReceiptDetails.N_InventoryId =@nSalesID";
                    dtPayment = dLayer.ExecuteDataTable(qry1, QueryParamsList, Con);
                    string InvoiceNos = "";
                    foreach (DataRow var in dtPayment.Rows)
                        // InvoiceNos += var["X_VoucherNo"].ToString() + " , ";
                        InvoiceNos += var["X_VoucherNo"].ToString();
                    myFunctions.AddNewColumnToDataTable(masterTable, "X_SalesReceiptNos", typeof(string), InvoiceNos);


                    // dLayer.ExecuteDataTable("Select * from vw_SalesAmount_Customer where N_SalesID=@nSalesID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and (N_BranchId=@nBranchId Or N_BranchId=0)", QueryParamsList, Con);

                    //Details
                    SortedList dParamList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"N_SalesID",masterTable.Rows[0]["n_SalesId"].ToString()},
                        {"D_Date",Convert.ToDateTime(masterTable.Rows[0]["d_SalesDate"].ToString())}
                    };
                    DataTable detailTable = dLayer.ExecuteDataTablePro("SP_InvSalesDtls_Disp", dParamList, Con);
                    detailTable = _api.Format(detailTable, "Details");
                    if (detailTable.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }

                    // AdvanceTable  ///////////////////////////////////////////////////////////////////////////////////// 

                    DataTable AdvanceTable = new DataTable();
                    DataTable recievableTable = new DataTable();
                    int branchFlag = 0;
                    if (bAllBranchData) { branchFlag = 1; }
                    SortedList detailParams = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"N_CustomerId", myFunctions.getIntVAL(MasterRow["n_CustomerID"].ToString())},
                        {"D_SalesDate",Convert.ToDateTime(masterTable.Rows[0]["d_SalesDate"].ToString())},
                        {"N_BranchFlag",branchFlag},
                        {"N_BranchID",nBranchId}
                    };
                    recievableTable = dLayer.ExecuteDataTablePro("SP_InvReceivables", detailParams, Con);
                    recievableTable = myFunctions.AddNewColumnToDataTable(recievableTable, "flag", typeof(int), 0);
                    recievableTable = myFunctions.AddNewColumnToDataTable(recievableTable, "n_AmountF", typeof(double), 0);
                    recievableTable = myFunctions.AddNewColumnToDataTable(recievableTable, "pay", typeof(Boolean), false);

                    string settlementSql = "select * from Inv_SalesAdvanceSettlement where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and N_SalesID=" + nSalesID + "";
                    AdvanceTable = dLayer.ExecuteDataTable(settlementSql, QueryParamsList, Con);
                    if (AdvanceTable.Rows.Count > 0)
                    {
                        foreach (DataRow var in AdvanceTable.Rows)
                        {
                            foreach (DataRow var1 in recievableTable.Rows)
                            {
                                if (var1["N_SalesID"].ToString() == var["N_AdvID"].ToString())
                                {
                                    var1["flag"] = 1;
                                    var1["n_AmountF"] = var["n_AdvAmtF"];
                                    var1["x_Notes"] = var["x_Notes"];
                                    var1["pay"] = true;
                                    recievableTable.AcceptChanges();
                                }
                            }
                        }
                    }
                    recievableTable.AcceptChanges();
                    if (AdvanceTable.Rows.Count > 0)
                    {
                        foreach (DataRow var3 in recievableTable.Rows)
                        {
                            if (myFunctions.getIntVAL(var3["flag"].ToString()) == 0)
                            {
                                var3.Delete();

                            }
                        }
                    }
                    recievableTable.AcceptChanges();
                    recievableTable = _api.Format(recievableTable, "AdvanceTable");
                    dsSalesInvoice.Tables.Add(recievableTable);

                    ////////////////////////////////////////////////////////////////
                    DataTable saleamountdetails = new DataTable();
                    if (myFunctions.getIntVAL(masterTable.Rows[0]["n_SalesId"].ToString()) > 0)
                    {
                        object CustomerID = dLayer.ExecuteScalar("Select TOP (1) PERCENT ISNULL(N_CustomerID,0) from vw_SalesAmount_Customer_Cloud where N_SalesID=" + masterTable.Rows[0]["n_SalesId"].ToString() + " and N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId, Con);
                        if (CustomerID != null)
                        {
                            // if (myFunctions.getIntVAL(masterTable.Rows[0]["N_CustomerID"].ToString()) == myFunctions.getIntVAL(CustomerID.ToString()))
                            saleamountdetails = dLayer.ExecuteDataTable("Select distinct * from vw_SalesAmount_Customer_Cloud where N_SalesID=" + masterTable.Rows[0]["n_SalesId"].ToString() + "  and N_CompanyID=" + nCompanyId + "  and n_BranchID =" + nBranchId + " and N_FnYearID=" + nFnYearId + "  "
                                                                    + " Union  select [Customer Code] as X_CustomerCode,[Customer Name] as X_CustomerName,0 as N_SalesID,N_CustomerID,0 as N_Amount,0 as N_AmountF,0 as N_CommissionAmtt,0 as N_CommissionAmtF,0 as N_CommissionPer, N_CompanyID,N_BranchID,N_FnYearID,'' as X_CardNo,0 as TaxID, null as N_PointsOUT,0 as N_AppliedAmt,null as N_AvailPoints,0 as N_SalesAmountID,N_CashTypeID,N_CashTypeID as N_CashTypeID2, "
                                                                    + "N_EnablePopup   from vw_PaymentType_Disp where  N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and n_BranchID =" + nBranchId + " and ISNULL(B_Inactive,0)=0 and N_EnablePopup=1 and  N_CustomerID not in (Select N_CustomerID from  "
                                                                    + " vw_SalesAmount_Customer_Cloud where N_SalesID=" + masterTable.Rows[0]["n_SalesId"].ToString() + "  and N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and n_BranchID =" + nBranchId + " )", Con); // else
                            //     saleamountdetails = dLayer.ExecuteDataTable("Select distinct * from vw_SalesAmount_Customer where N_SalesID=0 and N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId, Con);
                        }
                        // else
                        //     saleamountdetails = dLayer.ExecuteDataTable("Select distinct * from vw_SalesAmount_Customer where N_SalesID=0 and N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId, Con);
                    }

                    DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(masterTable.Rows[0]["N_CustomerID"].ToString()), myFunctions.getIntVAL(masterTable.Rows[0]["N_SalesId"].ToString()), this.N_FormID, myFunctions.getIntVAL(masterTable.Rows[0]["N_FnYearID"].ToString()), User, Con);
                    Attachments = _api.Format(Attachments, "attachments");
                    saleamountdetails = _api.Format(saleamountdetails, "saleamountdetails");

                    SortedList Status = StatusSetup(nSalesID, nFnYearId, myFunctions.getIntVAL(myFunctions.getBoolVAL(masterTable.Rows[0]["B_IsSaveDraft"].ToString())), Con);
                    masterTable = myFunctions.AddNewColumnToDataTable(masterTable, "TxnStatus", typeof(SortedList), Status);

                    bool Invoice2Enable = false;
                    object Invoice2Enableobj = dLayer.ExecuteScalar("select 1 from gen_printtemplates where N_CompanyID =" + nCompanyId + " and N_FormID=1651 and X_RptName<>'' and N_UsercategoryID=" + myFunctions.GetUserCategory(User), Con);
                    if (Invoice2Enableobj != null)
                        Invoice2Enable = true;
                    masterTable = myFunctions.AddNewColumnToDataTable(masterTable, "Invoice2Enable", typeof(bool), Invoice2Enable);


                    //Eye Optics
                    string sqlPrescription = "select * from Inv_Prescription where N_CustomerID=" + myFunctions.getIntVAL(masterTable.Rows[0]["N_CustomerID"].ToString()) + " and N_SalesID=" + masterTable.Rows[0]["n_SalesId"].ToString() + "";
                    DataTable prescription = dLayer.ExecuteDataTable(sqlPrescription, Con);
                    prescription = _api.Format(prescription, "Prescription");


                    dsSalesInvoice.Tables.Add(masterTable);
                    dsSalesInvoice.Tables.Add(detailTable);
                    dsSalesInvoice.Tables.Add(saleamountdetails);
                    dsSalesInvoice.Tables.Add(Attachments);
                    dsSalesInvoice.Tables.Add(prescription);

                    return Ok(_api.Success(dsSalesInvoice));

                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }



        private SortedList StatusSetup(int nSalesID, int nFnYearID, int isDraft, SqlConnection connection)
        {

            object objInvoiceRecievable = null, objBal = null, objPayMode = null;
            double InvoiceRecievable = 0, BalanceAmt = 0;
            SortedList TxnStatus = new SortedList();
            TxnStatus.Add("Label", "");
            TxnStatus.Add("LabelColor", "");
            TxnStatus.Add("Alert", "");
            TxnStatus.Add("DeleteEnabled", true);
            TxnStatus.Add("SaveEnabled", true);
            TxnStatus.Add("ReceiptNumbers", "");
            int nCompanyID = myFunctions.GetCompanyID(User);


            if (isDraft == 1)
            {
                TxnStatus["SaveEnabled"] = true;
                TxnStatus["DeleteEnabled"] = true;
                TxnStatus["Alert"] = "";
                TxnStatus["Label"] = "Draft";
                TxnStatus["LabelColor"] = "Red";
                return TxnStatus;
            }

            objInvoiceRecievable = dLayer.ExecuteScalar("SELECT SUM(isnull((Inv_Sales.N_BillAmt-Inv_Sales.N_DiscountAmt + Inv_Sales.N_FreightAmt +isnull(Inv_Sales.N_OthTaxAmt,0)+isnull(Inv_Sales.N_CessAmt,0) + isnull(Inv_Sales.N_TaxAmt,0)),0)) as N_InvoiceAmount FROM Inv_Sales where Inv_Sales.N_SalesId=" + nSalesID + " and Inv_Sales.N_CompanyID=" + nCompanyID, connection);
            string balanceSql = "SELECT        isnull(SUM(vw_InvReceivables.N_BalanceAmount),0) AS N_BalanceAmount " +
            "FROM vw_InvReceivables RIGHT OUTER JOIN " +
            "Inv_Sales ON vw_InvReceivables.N_CompanyId = Inv_Sales.N_CompanyId AND vw_InvReceivables.N_SalesId = Inv_Sales.N_SalesId AND vw_InvReceivables.N_CustomerId = Inv_Sales.N_CustomerId " +
            "WHERE (vw_InvReceivables.N_SalesId = " + nSalesID + ") AND (vw_InvReceivables.X_Type = 'SALES') AND (vw_InvReceivables.N_CompanyId = " + nCompanyID + ")";

            objBal = dLayer.ExecuteScalar(balanceSql, connection);

            string payModeSql = "select X_PayMode from Vw_InvSalesPayMode where N_SalesId = " + nSalesID + " and N_CompanyId =" + nCompanyID;

            objPayMode = dLayer.ExecuteScalar(payModeSql, connection);
            if (objPayMode == null)
                objPayMode = "";

            if (objPayMode == null)
                objPayMode = "";
            object RetQty = dLayer.ExecuteScalar("select Isnull(Count(N_DebitNoteId),0) from Inv_SalesReturnMaster where N_SalesId =" + nSalesID + " and Isnull(B_IsSaveDraft,0) =0 and N_CompanyID=" + nCompanyID, connection);
            object RetQtyDrft = dLayer.ExecuteScalar("select Isnull(Count(N_DebitNoteId),0) from Inv_SalesReturnMaster where N_SalesId =" + nSalesID + " and Isnull(B_IsSaveDraft,0)=1 and N_CompanyID=" + nCompanyID, connection);
            object DebitNoteNo = dLayer.ExecuteScalar("select X_DebitNoteNo from Inv_SalesReturnMaster where N_SalesId =" + nSalesID + " and N_CompanyID=" + nCompanyID, connection);


            if (objInvoiceRecievable != null)
                InvoiceRecievable = myFunctions.getVAL(objInvoiceRecievable.ToString());
            if (objBal != null)
                BalanceAmt = myFunctions.getVAL(objBal.ToString());

            if ((Math.Round(InvoiceRecievable, 2) == Math.Round(BalanceAmt, 2)) && (Math.Round(InvoiceRecievable, 2) > 0 && Math.Round(BalanceAmt, 2) > 0))
            {
                TxnStatus["Label"] = "Not Paid (" + objPayMode.ToString() + ")";
                TxnStatus["LabelColor"] = "Red";
                TxnStatus["Alert"] = "";
            }
            else
            {
                if (BalanceAmt == 0)
                {
                    //IF PAYMENT DONE
                    TxnStatus["Label"] = "Paid (" + objPayMode.ToString() + ")";
                    TxnStatus["LabelColor"] = "Green";
                    TxnStatus["Alert"] = "Customer Receipt is Processed for this Invoice.";


                    //IF PAYMENT DONE AND HAVING RETURN
                    if (RetQty != null && myFunctions.getIntVAL(RetQty.ToString()) > 0)
                    {
                        TxnStatus["SaveEnabled"] = false;
                        TxnStatus["DeleteEnabled"] = false;
                        TxnStatus["Alert"] = "Sales Return Processed for this invoice.";
                        TxnStatus["Label"] = "Paid (" + objPayMode.ToString() + ") (Return - " + DebitNoteNo + ")";
                        TxnStatus["LabelColor"] = "Green";
                    }
                    else if (RetQtyDrft != null && myFunctions.getIntVAL(RetQtyDrft.ToString()) > 0)
                    {
                        TxnStatus["SaveEnabled"] = true;
                        TxnStatus["DeleteEnabled"] = false;
                        TxnStatus["Alert"] = "Sales Return Processed for this invoice.";
                        TxnStatus["Label"] = "Paid (" + objPayMode.ToString() + ") (Return - " + DebitNoteNo + ")";
                        TxnStatus["LabelColor"] = "Green";
                    }
                }
                else
                {
                    //IF HAVING BALANCE AMOUNT
                    TxnStatus["Alert"] = "Customer Receipt is Processed for this Invoice.";
                    TxnStatus["Label"] = "Partially Paid (" + objPayMode.ToString() + ")";
                    TxnStatus["LabelColor"] = "Green";

                    //IF HAVING BALANCE AMOUNT AND HAVING RETURN
                    if (RetQty != null && myFunctions.getIntVAL(RetQty.ToString()) > 0)
                    {
                        TxnStatus["SaveEnabled"] = false;
                        TxnStatus["DeleteEnabled"] = false;
                        TxnStatus["Alert"] = "Sales Return Processed for this invoice.";
                        TxnStatus["Label"] = "Partially Paid (" + objPayMode.ToString() + ") (Return)";
                        TxnStatus["LabelColor"] = "Green";
                    }
                    else if (RetQtyDrft != null && myFunctions.getIntVAL(RetQtyDrft.ToString()) > 0)
                    {
                        TxnStatus["SaveEnabled"] = true;
                        TxnStatus["DeleteEnabled"] = false;
                        TxnStatus["Alert"] = "Sales Return Processed for this invoice.";
                        TxnStatus["Label"] = "Partially Paid (" + objPayMode.ToString() + ") (Return)";
                        TxnStatus["LabelColor"] = "Green";
                    }
                }


                //PAYMENT NO DISPLAY IN TOP LABEL ON MOUSE HOVER
                DataTable Receipts = dLayer.ExecuteDataTable("SELECT  dbo.Inv_PayReceipt.X_VoucherNo FROM  dbo.Inv_PayReceipt INNER JOIN dbo.Inv_PayReceiptDetails ON dbo.Inv_PayReceipt.N_PayReceiptId = dbo.Inv_PayReceiptDetails.N_PayReceiptId Where dbo.Inv_PayReceipt.X_Type='SR' and dbo.Inv_PayReceiptDetails.N_InventoryId =" + nSalesID + " and dbo.Inv_PayReceiptDetails.N_CompanyID =" + nCompanyID + " and dbo.Inv_PayReceiptDetails.X_TransType='SALES'", connection);
                string InvoiceNos = "";
                foreach (DataRow var in Receipts.Rows)
                {
                    InvoiceNos += var["X_VoucherNo"].ToString() + " , ";
                }
                char[] trim = { ',', ' ' };
                if (InvoiceNos != "")
                {
                    TxnStatus["ReceiptNumbers"] = InvoiceNos.ToString().TrimEnd(trim);
                    char[] trim1 = { ',', ' ' };
                    TxnStatus["Label"] = TxnStatus["Label"].ToString() + " (Payment No: " + InvoiceNos.ToString().TrimEnd(trim1) + ")";
                }

            }

            return TxnStatus;
        }

        //Save....
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            SortedList Result = new SortedList();
            int n_IsCompleted = 0;
            string x_Message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();

                    string ipAddress = "";
                    if (Request.Headers.ContainsKey("X-Forwarded-For"))
                        ipAddress = Request.Headers["X-Forwarded-For"];
                    else
                        ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

                    Result = txnHandler.SalesSaveData(ds, ipAddress, User, dLayer, connection, transaction);

                    n_IsCompleted = myFunctions.getIntVAL(Result["b_IsCompleted"].ToString());
                    x_Message = Result["x_Msg"].ToString();

                    if (n_IsCompleted == 1)
                    {
                        transaction.Commit();
                        return Ok(_api.Success(Result, x_Message));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, x_Message));
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }

        }

        // private SortedList SalesSaveData(DataSet ds)
        // {
        // [HttpPost("Save")]
        // public ActionResult SaveData([FromBody] DataSet ds)
        // {
        //     try
        //     {
        //         DataTable MasterTable;
        //         DataTable DetailTable;
        //         DataTable dtsaleamountdetails; ;
        //         DataTable AdvanceTable; ;
        //         MasterTable = ds.Tables["master"];
        //         DetailTable = ds.Tables["details"];
        //         AdvanceTable = ds.Tables["advanceTable"];

        //         DataTable Approvals;
        //         Approvals = ds.Tables["approval"];
        //         DataRow ApprovalRow = Approvals.Rows[0];

        //         dtsaleamountdetails = ds.Tables["saleamountdetails"];
        //         DataTable Attachment = ds.Tables["attachments"];

        //         SortedList Params = new SortedList();
        //         SortedList QueryParams = new SortedList();
        //         // Auto Gen 
        //         string InvoiceNo = "";
        //         string xButtonAction = "";
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             SqlTransaction transaction;

        //             DataRow MasterRow = MasterTable.Rows[0];
        //             transaction = connection.BeginTransaction();


        //             int N_SalesID = myFunctions.getIntVAL(MasterRow["n_SalesID"].ToString());
        //             int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
        //             int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
        //             int N_BranchID = myFunctions.getIntVAL(MasterRow["n_BranchID"].ToString());
        //             int N_LocationID = myFunctions.getIntVAL(MasterRow["n_LocationID"].ToString());
        //             int N_CustomerID = myFunctions.getIntVAL(MasterRow["n_CustomerID"].ToString());

        //             int N_PaymentMethodID = myFunctions.getIntVAL(MasterRow["n_PaymentMethodID"].ToString());

        //             if(N_PaymentMethodID==0){
        //                   transaction.Rollback();
        //                   return Ok(_api.Error(User, "No payment method selected !!"));
        //             }

        //             int N_DeliveryNoteID = myFunctions.getIntVAL(MasterRow["n_DeliveryNoteId"].ToString());
        //              int N_ServiceID = MasterTable.Columns.Contains("N_ServiceID")? myFunctions.getIntVAL(MasterRow["N_ServiceID"].ToString()):0;
        //             int N_CreatedUser = myFunctions.getIntVAL(MasterRow["n_CreatedUser"].ToString());
        //             int N_UserID = myFunctions.getIntVAL(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        //             int UserCategoryID = myFunctions.getIntVAL(User.FindFirst(ClaimTypes.GroupSid)?.Value);
        //             int N_AmtSplit = 0;
        //             int N_IsProforma = 0;
        //             int N_SaveDraft = myFunctions.getIntVAL(MasterRow["b_IsSaveDraft"].ToString());
        //             N_IsProforma = MasterTable.Columns.Contains("b_IsProforma") ? myFunctions.getIntVAL(MasterRow["b_IsProforma"].ToString()) : 0;
        //             bool B_AllBranchData = false, B_AllowCashPay = false, B_DirectPosting = false;
        //             int N_NextApproverID = 0;
        //             int AdvanceSettlementID = 0;


        //             QueryParams.Add("@nCompanyID", N_CompanyID);
        //             QueryParams.Add("@nFnYearID", N_FnYearID);
        //             QueryParams.Add("@nSalesID", N_SalesID);
        //             QueryParams.Add("@nBranchID", N_BranchID);
        //             QueryParams.Add("@nLocationID", N_LocationID);
        //             QueryParams.Add("@nCustomerID", N_CustomerID); 
        //             int N_FormID = 0;
        //                if (MasterTable.Columns.Contains("N_FormID"))
        //             {
        //                 N_FormID = myFunctions.getIntVAL(MasterRow["N_FormID"].ToString());
        //             }


        //             if (!myFunctions.CheckActiveYearTransaction(N_CompanyID, N_FnYearID, DateTime.ParseExact(MasterTable.Rows[0]["D_SalesDate"].ToString(), "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture), dLayer, connection, transaction))
        //             {
        //                 object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID=@nCompanyID and convert(date ,'" + MasterTable.Rows[0]["D_SalesDate"].ToString() + "') between D_Start and D_End", QueryParams, connection, transaction);
        //                 if (DiffFnYearID != null)
        //                 {
        //                     MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
        //                     N_FnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());
        //                     QueryParams["@nFnYearID"] = N_FnYearID;
        //                 }
        //                 else
        //                 {
        //                     transaction.Rollback();
        //                     return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
        //                 }
        //             }
        //             MasterTable.AcceptChanges();
        //             B_DirectPosting = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select B_DirPosting from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID", QueryParams, connection, transaction).ToString());
        //             object objAllBranchData = dLayer.ExecuteScalar("Select B_ShowAllData From Acc_BranchMaster where N_BranchID=@nBranchID and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
        //             if (objAllBranchData != null)
        //                 B_AllBranchData = myFunctions.getBoolVAL(objAllBranchData.ToString());

        //             if (B_AllBranchData)
        //                 B_AllowCashPay = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select cast(count(N_CustomerID) as bit) from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID  and N_AllowCashPay=1", QueryParams, connection, transaction).ToString());
        //             else
        //                 B_AllowCashPay = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select cast(count(N_CustomerID) as bit) from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID  and N_AllowCashPay=1 and (N_BranchId=@nBranchID or N_BranchId=0)", QueryParams, connection, transaction).ToString());


        //             //saving data
        //             InvoiceNo = MasterRow["x_ReceiptNo"].ToString();

        //             SortedList CustParams = new SortedList();
        //             CustParams.Add("@nCompanyID", N_CompanyID);
        //             CustParams.Add("@N_CustomerID", N_CustomerID);
        //             CustParams.Add("@nFnYearID", N_FnYearID);
        //             object objCustName = dLayer.ExecuteScalar("Select X_CustomerName From Inv_Customer where N_CustomerID=@N_CustomerID and N_CompanyID=@nCompanyID  and N_FnYearID=@nFnYearID", CustParams, connection, transaction);
        //             object objCustCode = dLayer.ExecuteScalar("Select X_CustomerCode From Inv_Customer where N_CustomerID=@N_CustomerID and N_CompanyID=@nCompanyID  and N_FnYearID=@nFnYearID", CustParams, connection, transaction);


        //             if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()) && N_SalesID > 0)
        //             {
        //                 int N_PkeyID = N_SalesID;
        //                 string X_Criteria = "N_SalesID=" + N_PkeyID + " and N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID;
        //                 myFunctions.UpdateApproverEntry(Approvals, "Inv_Sales", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
        //                 N_NextApproverID = myFunctions.LogApprovals(Approvals, N_FnYearID, "SALES", N_PkeyID, InvoiceNo, 1, objCustName.ToString(), 0, "",0, User, dLayer, connection, transaction);
        //                 myAttachments.SaveAttachment(dLayer, Attachment, InvoiceNo, N_SalesID, objCustName.ToString().Trim(), objCustCode.ToString(), N_CustomerID, "Customer Document", User, connection, transaction);

        //                 N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_Sales where N_SalesID=" + N_SalesID + " and N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID, connection, transaction).ToString());
        //                 if (N_SaveDraft == 0)
        //                 {
        //                     SortedList PostingParam = new SortedList();
        //                     PostingParam.Add("N_CompanyID", N_CompanyID);
        //                     PostingParam.Add("X_InventoryMode", "SALES");
        //                     PostingParam.Add("N_InternalID", N_SalesID);
        //                     PostingParam.Add("N_UserID", N_UserID);
        //                     PostingParam.Add("X_SystemName", "ERP Cloud");
        //                     try 
        //                     {
        //                         dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Sales_Posting", PostingParam, connection, transaction);
        //                     }
        //                     catch (Exception ex)
        //                     {
        //                         transaction.Rollback();
        //                         return Ok(_api.Error(User, ex));
        //                     }
        //                     bool B_AmtpaidEnable = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "Show SalesAmt Paid", "N_Value", "N_UserCategoryID", "0", N_CompanyID, dLayer, connection, transaction)));
        //                     if (B_AmtpaidEnable)
        //                     {
        //                         if (!B_DirectPosting)
        //                         {
        //                             if (myFunctions.getVAL(MasterRow["N_CashReceived"].ToString()) > 0)
        //                             {
        //                                 SortedList ParamCustomerRcpt_Ins = new SortedList();
        //                                 ParamCustomerRcpt_Ins.Add("N_CompanyID", N_CompanyID);
        //                                 ParamCustomerRcpt_Ins.Add("N_Fn_Year", N_FnYearID);
        //                                 ParamCustomerRcpt_Ins.Add("N_SalesId", N_SalesID);
        //                                 ParamCustomerRcpt_Ins.Add("N_Amount", myFunctions.getVAL(MasterRow["N_CashReceived"].ToString()));
        //                                 try
        //                                 {
        //                                     dLayer.ExecuteNonQueryPro("SP_CustomerRcpt_Ins", ParamCustomerRcpt_Ins, connection, transaction);
        //                                 }
        //                                 catch (Exception ex)
        //                                 {
        //                                     transaction.Rollback();
        //                                     return Ok(_api.Error(User, "Unable to save Sales Invoice!"));
        //                                 }
        //                             }
        //                         }

        //                     }
        //                 }

        //                 //myFunctions.SendApprovalMail(N_NextApproverID, this.N_FormID, N_PkeyID, "SALES", InvoiceNo, dLayer, connection, transaction, User);
        //                 transaction.Commit();
        //                 return Ok(_api.Success("Sales Approved " + "-" + InvoiceNo));
        //             }


        //             if (N_SaveDraft == 1)
        //             {
        //                 if (N_SalesID == 0 && InvoiceNo != "@Auto")
        //                 {
        //                     object N_DocNumber = dLayer.ExecuteScalar("Select 1 from Inv_Sales Where X_ReceiptNo ='" + InvoiceNo + "' and N_CompanyID= " + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection, transaction);
        //                     if (N_DocNumber == null)
        //                     {
        //                         N_DocNumber = 0;
        //                     }
        //                     if (myFunctions.getVAL(N_DocNumber.ToString()) >= 1)
        //                     {
        //                         transaction.Rollback();
        //                         return Ok(_api.Error(User, "Invoice number already in use"));
        //                     }
        //                 }
        //                 if (InvoiceNo == "@Auto")
        //                 {
        //                     Params.Add("N_CompanyID", MasterRow["n_CompanyId"].ToString());
        //                     Params.Add("N_YearID", MasterRow["n_FnYearId"].ToString());
        //                     if (N_IsProforma == 1)
        //                         Params.Add("N_FormID", 1346);
        //                     else
        //                         Params.Add("N_FormID", this.N_FormID);

        //                     while (true)
        //                     {
        //                         InvoiceNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
        //                         object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_Sales Where X_ReceiptNo ='" + InvoiceNo + "' and N_CompanyID= " + N_CompanyID, connection, transaction);
        //                         if (N_Result == null)
        //                             break;
        //                     }
        //                     if (InvoiceNo == "")
        //                     {
        //                         transaction.Rollback();
        //                         return Ok(_api.Error(User, "Unable to generate Invoice Number"));
        //                     }
        //                     MasterTable.Rows[0]["x_ReceiptNo"] = InvoiceNo;
        //                 }
        //                 xButtonAction = "INSERT";
        //             }
        //             else
        //             {
        //                 object N_Resultval = dLayer.ExecuteScalar("Select 1 from Inv_Sales Where X_ReceiptNo ='" + InvoiceNo + "' and N_CompanyID= " + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection, transaction);
        //                 if (N_Resultval == null)
        //                 {
        //                     N_Resultval = 0;
        //                 }
        //                 if (N_SalesID == 0 && myFunctions.getVAL(N_Resultval.ToString()) >= 1)
        //                 {
        //                     transaction.Rollback();
        //                     return Ok(_api.Error(User, "Invoice number already in use"));
        //                 }

        //                 if (InvoiceNo == "@Auto")
        //                 {

        //                     Params.Add("N_CompanyID", MasterRow["n_CompanyId"].ToString());
        //                     Params.Add("N_YearID", MasterRow["n_FnYearId"].ToString());
        //                     if (N_IsProforma == 1)
        //                         Params.Add("N_FormID", 1346);
        //                     else
        //                         Params.Add("N_FormID", this.N_FormID);
        //                     Params.Add("N_BranchID", MasterRow["n_BranchId"].ToString());
        //                     while (true)
        //                     {
        //                         InvoiceNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
        //                         object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_Sales Where X_ReceiptNo ='" + InvoiceNo + "' and N_CompanyID= " + N_CompanyID, connection, transaction);
        //                         if (N_Result == null)
        //                             break;
        //                     }
        //                     if (InvoiceNo == "")
        //                     {
        //                         transaction.Rollback();
        //                         return Ok(_api.Error(User, "Unable to generate Quotation Number"));
        //                     }
        //                     MasterTable.Rows[0]["x_ReceiptNo"] = InvoiceNo;
        //                     xButtonAction = "INSERT";
        //                 }

        //             }
        //             if (N_SalesID > 0)
        //             {
        //                     string payRecieptqry = "select N_PayReceiptID from  Inv_PayReceipt where N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID + " and N_RefID=" + N_SalesID + " and N_FormID=" + this.N_FormID + "";
        //                     object nRecieptID = dLayer.ExecuteScalar(payRecieptqry, Params, connection, transaction);
        //                     if (nRecieptID != null && myFunctions.getIntVAL(nRecieptID.ToString()) > 0)
        //                     {
        //                         dLayer.ExecuteNonQuery(" delete from Acc_VoucherDetails Where N_CompanyID=" + N_CompanyID + " and N_InventoryID=" + myFunctions.getIntVAL(nRecieptID.ToString()) + " and N_FnYearID=" + N_FnYearID + " and X_TransType = 'SA'", connection, transaction);
        //                         dLayer.ExecuteNonQuery(" delete from Inv_PayReceiptDetails Where N_CompanyID=" + N_CompanyID + " and N_PayReceiptID=" + myFunctions.getIntVAL(nRecieptID.ToString()) + " ", connection, transaction);
        //                         dLayer.ExecuteNonQuery(" delete from Inv_PayReceipt Where N_CompanyID=" + N_CompanyID + " and N_PayReceiptID=" + myFunctions.getIntVAL(nRecieptID.ToString()) + " and  N_FnYearID=" + N_FnYearID + " ", connection, transaction);
        //                        dLayer.DeleteData("Inv_SalesAdvanceSettlement", "N_SalesID", N_SalesID, "N_CompanyID = " + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection, transaction);
        //                     }

        //                 SortedList DeleteParams = new SortedList(){
        //                         {"N_CompanyID",N_CompanyID},
        //                         {"X_TransType","SALES"},
        //                         {"N_VoucherID",N_SalesID}};
        //                 try
        //                 {
        //                     dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
        //                 }
        //                 catch (Exception ex)
        //                 {
        //                     transaction.Rollback();
        //                     return Ok(_api.Error(User, ex));
        //                 }

        //                 dLayer.ExecuteNonQuery("delete from Inv_SaleAmountDetails where N_SalesID=" + N_SalesID + " and N_CompanyID=" + N_CompanyID + " and N_BranchID=" + N_BranchID, connection, transaction);
        //                 dLayer.ExecuteNonQuery("delete from Inv_LoyaltyPointOut where N_TransID=" + N_SalesID + " and N_CompanyID=" + N_CompanyID + " and N_PartyId=" + N_CustomerID, connection, transaction);
        //                 dLayer.ExecuteNonQuery("delete from Inv_ServiceContract where N_SalesID=" + N_SalesID + " and N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID + " and N_BranchID=" + N_BranchID, connection, transaction);
        //                 xButtonAction = "Update";
        //             }
        //             MasterTable.Rows[0]["n_UserID"] = myFunctions.GetUserID(User);
        //             MasterTable.AcceptChanges();

        //             MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);

        //             string dupInvNo = InvoiceNo;
        //             if (MasterRow["x_ReceiptNo"].ToString() != "@Auto")
        //                 dupInvNo = MasterRow["x_ReceiptNo"].ToString();

        //             string DupCriteria = "N_CompanyID=" + N_CompanyID + " and X_ReceiptNo='" + dupInvNo + "' and N_FnyearID=" + N_FnYearID;
        //             N_SalesID = dLayer.SaveData("Inv_Sales", "N_SalesId", DupCriteria, "", MasterTable, connection, transaction);
        //             if (N_SalesID <= 0)
        //             {
        //                 transaction.Rollback();
        //                 return Ok(_api.Error(User, "Unable to save Sales Invoice!"));
        //             }
        //             else
        //             {
        //                 // if (B_UserLevel)
        //                 // {
        //                 //     Inv_WorkFlowCatalog saving code here
        //                 // }                   

        //                 //Inv_WorkFlowCatalog insertion here
        //                 //DataTable dtsaleamountdetails = ds.Tables["saleamountdetails"];

        //                 N_NextApproverID = myFunctions.LogApprovals(Approvals, N_FnYearID, "SALES", N_SalesID, InvoiceNo, 1, objCustName.ToString(), 0, "",0, User, dLayer, connection, transaction);
        //                 N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_Sales where N_SalesID=" + N_SalesID + " and N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID, connection, transaction).ToString());

        //                 DataTable dtloyalitypoints = ds.Tables["loyalitypoints"];
        //                 int N_IsSave = 1;
        //                 int N_CurrentSalesID = 0;
        //                 if (ds.Tables["saleamountdetails"].Rows.Count > 0)
        //                 {
        //                     DataRow Rowsaleamountdetails = ds.Tables["saleamountdetails"].Rows[0];
        //                     // N_IsSave = myFunctions.getIntVAL(Rowsaleamountdetails["n_IsSave"].ToString());
        //                     // dtsaleamountdetails.Columns.Remove("n_IsSave");
        //                     // dtsaleamountdetails.AcceptChanges();
        //                     N_CurrentSalesID = myFunctions.getIntVAL(Rowsaleamountdetails["N_SalesID"].ToString());
        //                 }
        //                 //Activity Log

        //                 string ipAddress = "";
        //                 if (Request.Headers.ContainsKey("X-Forwarded-For"))
        //                     ipAddress = Request.Headers["X-Forwarded-For"];
        //                 else
        //                     ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        //                 SortedList LogParams = new SortedList();
        //                 LogParams.Add("N_CompanyID", N_CompanyID);
        //                 LogParams.Add("N_FnYearID", N_FnYearID);
        //                 LogParams.Add("N_TransID", N_SalesID);
        //                 LogParams.Add("N_FormID", this.N_FormID);
        //                 LogParams.Add("N_UserId", N_UserID);
        //                 LogParams.Add("X_Action", xButtonAction);
        //                 LogParams.Add("X_SystemName", "ERP Cloud");
        //                 LogParams.Add("X_IP", ipAddress);
        //                 LogParams.Add("X_TransCode", InvoiceNo);
        //                 LogParams.Add("X_Remark", " ");
        //                 dLayer.ExecuteNonQueryPro("SP_Log_SysActivity", LogParams, connection, transaction);


        //                 DataRow Rowloyalitypoints = null;
        //                 if (ds.Tables.Contains("loyalitypoints"))
        //                     Rowloyalitypoints = ds.Tables["loyalitypoints"].Rows[0];

        //                 // int N_IsSave = myFunctions.getIntVAL(Rowsaleamountdetails["n_IsSave"].ToString());
        //                 // dtsaleamountdetails.Columns.Remove("n_IsSave");
        //                 // dtsaleamountdetails.AcceptChanges();

        //                 // int N_CurrentSalesID = myFunctions.getIntVAL(Rowsaleamountdetails["N_SalesID"].ToString());
        //                 bool B_EnablePointSystem = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("64", "AllowLoyaltyPoint", "N_Value", "N_UserCategoryID", UserCategoryID.ToString(), N_CompanyID, dLayer, connection, transaction)));
        //                 bool B_SalesOrder = myFunctions.CheckPermission(N_CompanyID, 81, "Administrator", "X_UserCategory", dLayer, connection, transaction);
        //                 //Sales amount details/payment popup
        //                 for (int i = 0; i < dtsaleamountdetails.Rows.Count; i++)
        //                     dtsaleamountdetails.Rows[i]["N_SalesId"] = N_SalesID;
        //                 if (N_AmtSplit == 1)
        //                 {

        //                     if (N_IsSave == 1)
        //                     {

        //                         int N_SalesAmountID = dLayer.SaveData("Inv_SaleAmountDetails", "n_SalesAmountID", dtsaleamountdetails, connection, transaction);
        //                         if (N_SalesAmountID <= 0)
        //                         {
        //                             transaction.Rollback();
        //                             return Ok(_api.Error(User, "Unable to save Sales Invoice!"));
        //                         }
        //                         else
        //                         {
        //                             if (B_EnablePointSystem)
        //                             {
        //                                 if (ds.Tables.Contains("loyalitypoints") && dtloyalitypoints.Rows.Count > 0)
        //                                 {
        //                                     int N_PointOutId = dLayer.SaveData("Inv_LoyaltyPointOut", "n_PointOutId", dtloyalitypoints, connection, transaction);
        //                                     if (N_SalesAmountID <= 0)
        //                                     {
        //                                         transaction.Rollback();
        //                                         return Ok(_api.Error(User, "Unable to save Sales Invoice!"));
        //                                     }
        //                                     else
        //                                     {
        //                                         double N_DiscountAmt = myFunctions.getVAL(Rowloyalitypoints["N_AppliedAmt"].ToString()) + myFunctions.getVAL(MasterRow["N_DiscountAmt"].ToString());
        //                                         dLayer.ExecuteNonQuery("update  Inv_Sales  Set N_DiscountAmt=" + N_DiscountAmt + " where N_SalesID=@nSalesID and N_CompanyID=@nCompanyID and N_CustomerID=@nCustomerID", QueryParams, connection, transaction);
        //                                     }
        //                                 }
        //                             }
        //                         }
        //                     }
        //                     else if (N_IsSave == 0)
        //                     {
        //                         if (N_CurrentSalesID != N_SalesID)
        //                             dLayer.ExecuteNonQuery("update  Inv_SaleAmountDetails set N_SalesID=" + N_SalesID + " where N_SalesID=@nSalesID and N_CompanyID=@nCompanyID and N_BranchID=@nBranchID", QueryParams, connection, transaction);
        //                     }
        //                 }
        //                 else
        //                 {
        //                      if (N_SaveDraft == 0)
        //                     {
        //                     //   foreach (DataRow data in dtsaleamountdetails.Rows)
        //                     //   {
        //                     //          double N_SChrgAmt = 0;
        //                     //         double N_SChrgAmtMax = 0;
        //                     //          object N_ServiceCharge = dLayer.ExecuteScalar("Select ISNULL(N_ServiceCharge , 0) from Inv_Customer where N_CustomerID=" + myFunctions.getVAL(data["N_CustomerID"].ToString()) + " and N_CompanyID=" + N_CompanyID + "and N_FnYearID=" +N_FnYearID, QueryParams, connection, transaction);
        //                     //         object N_ServiceChargeMax = dLayer.ExecuteScalar("Select ISNULL(N_ServiceChargeLimit , 0) from Inv_Customer where N_CustomerID=" +  myFunctions.getVAL(data["N_CustomerID"].ToString()) + " and N_CompanyID=" + N_CompanyID + "and N_FnYearID=" + N_FnYearID,  QueryParams, connection, transaction);
        //                     //         object N_TaxID = dLayer.ExecuteScalar("Select ISNULL(N_TaxCategoryID , 0) from Inv_Customer where N_CustomerID=" +  myFunctions.getVAL(data["N_CustomerID"].ToString()) + " and N_CompanyID=" + N_CompanyID + "and N_FnYearID=" + N_FnYearID,  QueryParams, connection, transaction);
        //                     //          if (myFunctions.getVAL(N_ServiceCharge.ToString()) > 0)
        //                     //         {
        //                     //              N_SChrgAmt = (myFunctions.getVAL(data["N_Amount"].ToString()) * myFunctions.getVAL((N_ServiceCharge.ToString())) / 100);
        //                     //              N_SChrgAmtMax = myFunctions.getVAL(N_ServiceChargeMax.ToString());
        //                     //          if (N_SChrgAmtMax > 0)
        //                     //              {
        //                     //                 if (N_SChrgAmt > N_SChrgAmtMax)
        //                     //                      N_SChrgAmt = myFunctions.getVAL(N_ServiceChargeMax.ToString());
        //                     //                 }
        //                     //          }
        //                     //           double AmountH = myFunctions.getVAL((myFunctions.getVAL(data["N_Amount"].ToString()) * myFunctions.getVAL(data["N_Amount"].ToString())).ToString());
        //                     //           double CommissionAmtH = N_SChrgAmt * (myFunctions.getVAL(data["N_Amount"].ToString()));
        //                     //           if(myFunctions.getVAL(N_ServiceCharge.ToString())>0)
        //                     //         {
        //                     //             data["N_CommissionAmt"]=CommissionAmtH;
        //                     //             data["N_Amount"]=AmountH;
        //                     //             data["N_CommissionPer"]=N_ServiceCharge;
        //                     //            data["N_CommissionPer"]=N_SChrgAmt;
        //                     //          data["N_TaxID"]=N_TaxID;
        //                     //          }
        //                     //   }

        //                     //service charge


        //                 int N_SalesAmountID = dLayer.SaveData("Inv_SaleAmountDetails", "n_SalesAmountID", dtsaleamountdetails, connection, transaction);
        //                 if (N_SalesAmountID <= 0)
        //                 {
        //                    transaction.Rollback();
        //                     return Ok(_api.Error(User, "Unable to save Sales Invoice!"));
        //                 }
        //                     }
        //                 }



        //                 bool B_salesOrder = false;
        //                 bool B_DeliveryNote = false;
        //                 B_DeliveryNote = myFunctions.CheckPermission(N_CompanyID, 729, myFunctions.GetUserCategory(User).ToString(), "N_UserCategoryID", dLayer, connection, transaction);
        //                 if (!B_DeliveryNote)
        //                     B_salesOrder = myFunctions.CheckPermission(N_CompanyID, 81, myFunctions.GetUserCategory(User).ToString(), "N_UserCategoryID", dLayer, connection, transaction);
        //                 bool B_ServiceSheet = myFunctions.CheckPermission(N_CompanyID, 1145, myFunctions.GetUserCategory(User).ToString(), "N_UserCategoryID", dLayer, connection, transaction);
        //                 for (int j = 0; j < DetailTable.Rows.Count; j++)
        //                 {
        //                     // if (B_salesOrder)
        //                     // {
        //                         int nSalesOrderID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesOrderID"].ToString());
        //                         if (nSalesOrderID > 0)
        //                         {
        //                             dLayer.ExecuteNonQuery("Update Inv_SalesOrder Set N_SalesID=" + N_SalesID + ", N_Processed=1 Where N_SalesOrderID=" + nSalesOrderID + " and N_FnYearID=@nFnYearID and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
        //                             if (B_ServiceSheet)
        //                                 dLayer.ExecuteNonQuery("Update Inv_ServiceSheetMaster Set N_Processed=1  Where N_RefID=" + nSalesOrderID + " and N_FnYearID=@nFnYearID and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
        //                         }
        //                     // }
        //                     // else
        //                     // {
        //                         int nQuotationID = myFunctions.getIntVAL(DetailTable.Rows[j]["N_SalesQuotationID"].ToString());
        //                         if (nQuotationID > 0)
        //                             dLayer.ExecuteNonQuery("Update Inv_SalesQuotation Set N_SalesID=" + N_SalesID + ", N_Processed=1 Where N_QuotationID=" + nQuotationID + " and N_FnYearID=@nFnYearID and N_CompanyID=@nCompanyID", QueryParams, connection,transaction);
        //                     // }
        //                 }

        //                 // Warranty Save Code here
        //                 //optical prescription saving here
        //                 for (int j = 0; j < DetailTable.Rows.Count; j++)
        //                 {
        //                     DetailTable.Rows[j]["N_SalesId"] = N_SalesID;
        //                 }
        //                 if (DetailTable.Columns.Contains("n_Stock"))
        //                     DetailTable.Columns.Remove("n_Stock");

        //                 int N_InvoiceDetailId = dLayer.SaveData("Inv_SalesDetails", "n_SalesDetailsID", DetailTable, connection, transaction);
        //                 if (N_InvoiceDetailId <= 0)
        //                 {
        //                     transaction.Rollback();
        //                     return Ok(_api.Error(User, "Unable to save Sales Invoice!"));
        //                 }
        //                 else
        //                 {
        //                     //Advance Settlement Save

        //                     if (AdvanceTable.Rows.Count > 0)
        //                     {
        //                         dLayer.DeleteData("Inv_SalesAdvanceSettlement", "N_SalesID", N_SalesID, "N_CompanyID = " + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection, transaction);
        //                         for (int j = 0; j < AdvanceTable.Rows.Count; j++)
        //                         {
        //                             AdvanceTable.Rows[j]["N_SalesId"] = N_SalesID;
        //                         }
        //                         AdvanceTable.AcceptChanges();
        //                         AdvanceSettlementID = dLayer.SaveData("Inv_SalesAdvanceSettlement", "N_PkeyID", AdvanceTable, connection, transaction);

        //                         SortedList advanceParams = new SortedList();
        //                         advanceParams.Add("@N_CompanyID", N_CompanyID);
        //                         advanceParams.Add("@N_SalesID", N_SalesID);
        //                         try
        //                         {
        //                             dLayer.ExecuteNonQueryPro("SP_InvSalesAdvSettlement", advanceParams, connection, transaction);
        //                         }
        //                         catch (Exception ex)
        //                         {
        //                             transaction.Rollback();
        //                             return Ok(_api.Error(User, ex));

        //                         }





        //                     }
        //                     SortedList StockPostingParams = new SortedList();
        //                     StockPostingParams.Add("N_CompanyID", N_CompanyID);
        //                     StockPostingParams.Add("N_SalesID", N_SalesID);
        //                     StockPostingParams.Add("N_SaveDraft", N_SaveDraft);
        //                     StockPostingParams.Add("N_DeliveryNoteID", N_DeliveryNoteID);
        //                     if (N_DeliveryNoteID == 0 && N_ServiceID == 0 )
        //                     {
        //                         try
        //                         {
        //                             dLayer.ExecuteNonQueryPro("SP_SalesDetails_InsCloud", StockPostingParams, connection, transaction);
        //                         }
        //                         catch (Exception ex)
        //                         {
        //                             transaction.Rollback(); 
        //                             if (ex.Message == "50") 
        //                                 return Ok(_api.Error(User, "Day Closed"));
        //                             else if (ex.Message == "51")
        //                                 return Ok(_api.Error(User, "Year Closed"));
        //                             else if (ex.Message == "52")
        //                                 return Ok(_api.Error(User, "Year Exists"));
        //                             else if (ex.Message == "53")
        //                                 return Ok(_api.Error(User, "Period Closed"));
        //                             else if (ex.Message == "54")
        //                                 return Ok(_api.Error(User, "Txn Date"));
        //                             else if (ex.Message == "55")
        //                                 return Ok(_api.Error(User, "Quantity exceeds!"));
        //                             else
        //                                 return Ok(_api.Error(User, ex));
        //                         }
        //                     }
        //                 }

        //                 if (N_SaveDraft == 0)
        //                 {
        //                     SortedList PostingParam = new SortedList();
        //                     PostingParam.Add("N_CompanyID", N_CompanyID);
        //                     PostingParam.Add("X_InventoryMode", "SALES");
        //                     PostingParam.Add("N_InternalID", N_SalesID);
        //                     PostingParam.Add("N_UserID", N_UserID);
        //                     PostingParam.Add("X_SystemName", "ERP Cloud");
        //                     try
        //                     {
        //                         dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Sales_Posting", PostingParam, connection, transaction);
        //                     }
        //                     catch (Exception ex)
        //                     {
        //                         transaction.Rollback();
        //                         return Ok(_api.Error(User, ex));
        //                     }
        //                     bool B_AmtpaidEnable = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "Show SalesAmt Paid", "N_Value", "N_UserCategoryID", "0", N_CompanyID, dLayer, connection, transaction)));
        //                     if (B_AmtpaidEnable)
        //                     {
        //                         if (!B_DirectPosting)
        //                         {
        //                             if (myFunctions.getVAL(MasterRow["N_CashReceived"].ToString()) > 0)
        //                             {
        //                                 SortedList ParamCustomerRcpt_Ins = new SortedList();
        //                                 ParamCustomerRcpt_Ins.Add("N_CompanyID", N_CompanyID);
        //                                 ParamCustomerRcpt_Ins.Add("N_Fn_Year", N_FnYearID);
        //                                 ParamCustomerRcpt_Ins.Add("N_SalesId", N_SalesID);
        //                                 ParamCustomerRcpt_Ins.Add("N_Amount", myFunctions.getVAL(MasterRow["N_CashReceived"].ToString()));
        //                                 try
        //                                 {
        //                                     dLayer.ExecuteNonQueryPro("SP_CustomerRcpt_Ins", ParamCustomerRcpt_Ins, connection, transaction);
        //                                 }
        //                                 catch (Exception ex)
        //                                 {
        //                                     transaction.Rollback();
        //                                     return Ok(_api.Error(User, "Unable to save Sales Invoice!"));
        //                                 }
        //                             }
        //                         }

        //                     }
        //                     //StatusUpdate
        //                     int tempQtn=0,tempSO=0;
        //                     for (int j = 0; j < DetailTable.Rows.Count; j++)
        //                     {
        //                         int nSalesOrderID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesOrderID"].ToString());
        //                         int nQuotationID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesQuotationID"].ToString());

        //                         if (nSalesOrderID > 0 && tempSO!=nSalesOrderID)
        //                         {
        //                             if(!myFunctions.UpdateTxnStatus(N_CompanyID,nSalesOrderID,81,false,dLayer,connection,transaction))
        //                             {
        //                                 transaction.Rollback();
        //                                 return Ok(_api.Error(User, "Unable To Update Txn Status"));
        //                             }
        //                         }
        //                         tempSO = nSalesOrderID;

        //                         if (nQuotationID > 0 && tempQtn!=nQuotationID)
        //                         {
        //                             if(!myFunctions.UpdateTxnStatus(N_CompanyID,nQuotationID,80,false,dLayer,connection,transaction))
        //                             {
        //                                 transaction.Rollback();
        //                                 return Ok(_api.Error(User, "Unable To Update Txn Status"));
        //                             }
        //                         }
        //                         tempQtn=nQuotationID;
        //                     };
        //                 }
        //                 SortedList CustomerParams = new SortedList();
        //                 CustomerParams.Add("@nCustomerID", N_CustomerID);
        //                 DataTable CustomerInfo = dLayer.ExecuteDataTable("Select X_CustomerCode,X_CustomerName from Inv_Customer where N_CustomerID=@nCustomerID", CustomerParams, connection, transaction);
        //                 if (CustomerInfo.Rows.Count > 0)
        //                 {
        //                     try
        //                     {
        //                         myAttachments.SaveAttachment(dLayer, Attachment, InvoiceNo, N_SalesID, CustomerInfo.Rows[0]["X_CustomerName"].ToString().Trim(), CustomerInfo.Rows[0]["X_CustomerCode"].ToString(), N_CustomerID, "Customer Document", User, connection, transaction);
        //                     }
        //                     catch (Exception ex)
        //                     {
        //                         transaction.Rollback();
        //                         return Ok(_api.Error(User, ex));
        //                     }
        //                 }
        //                 //dispatch saving here

        //             }
        //             //return GetSalesInvoiceDetails(int.Parse(MasterRow["n_CompanyId"].ToString()), int.Parse(MasterRow["n_FnYearId"].ToString()), int.Parse(MasterRow["n_BranchId"].ToString()), InvoiceNo);
        //             SortedList Result = new SortedList();
        //             Result.Add("n_SalesID", N_SalesID);
        //             Result.Add("x_SalesNo", InvoiceNo);

        //             object N_CustomerVendorID = dLayer.ExecuteScalar("Select N_CustomerVendorID From Inv_Customer where N_CustomerID=@N_CustomerID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", CustParams, connection, transaction);

        //             if (N_CustomerVendorID.ToString() != "")
        //             {
        //                 SortedList PurchaseParams = new SortedList();
        //                 PurchaseParams.Add("@N_CompanyID", N_CompanyID);
        //                 PurchaseParams.Add("@N_FnYearID", N_FnYearID);
        //                 PurchaseParams.Add("@N_SalesID", N_SalesID);
        //                 dLayer.ExecuteNonQueryPro("SP_SalesToPurchase_Ins", PurchaseParams, connection, transaction);

        //             }
        //             transaction.Commit();
        //                 if (N_FormID == 64)
        //                     {
        //                    return Ok(_api.Success(Result, "Sales invoice saved"));
        //                     }
        //                   else if(N_FormID == 1601) 
        //                     {
        //                 return Ok(_api.Success(Result,"Rental Sales Saved Successfully"));
        //                      }
        //             return Ok(_api.Success(Result, "Sales invoice saved"));

        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(_api.Error(User, ex));
        //     }
        // }
        //Delete....Draft
        [HttpDelete("deleteDraft")]
        public ActionResult DeleteData(int nFnYearID)
        {
            int Results = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable Details = new DataTable();
                    SortedList ParamList = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nUserID = myFunctions.GetUserID(User);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    string Sql = "select * from Inv_Sales where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and B_IsSaveDraft=1 ";
                    string sqlQuotation = "";
                    Details = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    SqlTransaction transaction = connection.BeginTransaction();
                    int N_SalesID = 0;
                    int N_CustomerId = 0;
                    int N_FormID = 64;
                    object nQuotationID;
                    int count = 0;
                    string ipAddress = "";
                    if (Request.Headers.ContainsKey("X-Forwarded-For"))
                        ipAddress = Request.Headers["X-Forwarded-For"];
                    else
                        ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    SortedList LogParams = new SortedList();
                    LogParams.Add("N_CompanyID", nCompanyID);
                    LogParams.Add("N_FnYearID", nFnYearID);
                    LogParams.Add("N_TransID", 0);
                    LogParams.Add("N_FormID", this.N_FormID);
                    LogParams.Add("N_UserId", nUserID);
                    //LogParams.Add("N_UserID", nUserID);
                    LogParams.Add("X_Action", "");
                    LogParams.Add("X_SystemName", "ERP Cloud");
                    LogParams.Add("X_IP", ipAddress);
                    LogParams.Add("X_TransCode", "");
                    LogParams.Add("X_Remark", " ");
                    LogParams.Add("BulkDraft", 1);

                    dLayer.ExecuteNonQueryPro("SP_Log_SysActivity", LogParams, connection, transaction);


                    if (Details.Rows.Count > 0)
                    {
                        count = Details.Rows.Count;
                        foreach (DataRow item in Details.Rows)
                        {

                            N_SalesID = myFunctions.getIntVAL(item["N_SalesID"].ToString());
                            N_CustomerId = myFunctions.getIntVAL(item["N_CustomerId"].ToString());


                            SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType","SALES"},
                                {"N_VoucherID",N_SalesID}};
                            try
                            {
                                Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, ex));
                            }
                            if (Results > 0)
                            {
                                sqlQuotation = "select N_QuotationID from Inv_Sales where N_SalesID=" + N_SalesID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + "";
                                nQuotationID = dLayer.ExecuteScalar(sqlQuotation, ParamList, connection, transaction);

                                dLayer.ExecuteNonQuery("delete from Inv_DeliveryDispatch where n_InvoiceID=" + N_SalesID + " and n_CompanyID=@nCompanyID", ParamList, connection, transaction);
                                dLayer.ExecuteNonQuery("delete from Inv_SaleAmountDetails where n_SalesID=" + N_SalesID + " and  n_CompanyID=@nCompanyID", ParamList, connection, transaction);
                                dLayer.ExecuteNonQuery("delete from Inv_ServiceContract where n_SalesID=" + N_SalesID + " and n_FnYearID=@nFnYearID and n_CompanyID=@nCompanyID", ParamList, connection, transaction);
                                myAttachments.DeleteAttachment(dLayer, 1, N_SalesID, N_CustomerId, nFnYearID, N_FormID, User, transaction, connection);
                                if (nQuotationID != null)
                                    dLayer.ExecuteNonQuery("update Inv_SalesQuotation set N_Processed=0 where N_QuotationId=" + myFunctions.getIntVAL(nQuotationID.ToString()) + " and N_CompanyId=" + nCompanyID + " and N_FnYearId= @nFnYearID", ParamList, connection, transaction);
                            }

                        }
                    }
                    transaction.Commit();
                    return Ok(_api.Success(" " + count + " Draft  invoices deleted"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }

        }


        //Delete....
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nInvoiceID, int nCustomerID, int nCompanyID, int nFnYearID, int nBranchID, int nQuotationID, string comments, int nFormID)
        {
            if (comments == null)
            {
                comments = "";
            }
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nTransID", nInvoiceID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    SortedList result = new SortedList();
                    string xButtonAction = "Delete";
                    
                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,isNull(N_CustomerId,0) as N_CustomerId,X_ReceiptNo,N_SalesOrderID from Inv_Sales where N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID and N_SalesID=@nTransID";
                    string x_ReceiptNo = "";

                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    int N_CustomerId = myFunctions.getIntVAL(TransRow["N_CustomerId"].ToString());
                    int nSalesOrderID = myFunctions.getIntVAL(TransRow["N_SalesOrderID"].ToString());
                    

                    SortedList CustParams = new SortedList();
                    CustParams.Add("@nCompanyID", nCompanyID);
                    CustParams.Add("@N_CustomerID", N_CustomerId);
                    CustParams.Add("@nFnYearID", nFnYearID);

                    SortedList Params = new SortedList(){
                                {"@nCompanyID",nCompanyID},
                                {"@nFnYearID",nFnYearID},
                                {"@nSalesID",nInvoiceID}};

                    DataTable DetailTable = dLayer.ExecuteDataTable("select n_SalesOrderID,n_SalesQuotationID,N_DeliveryNoteID from Inv_SalesDetails where N_CompanyID=@nCompanyID and N_SalesID=@nSalesID group by n_SalesOrderID,n_SalesQuotationID,N_DeliveryNoteID order by n_SalesOrderID,n_SalesQuotationID,N_DeliveryNoteID", Params, connection);

                    object objCustName = dLayer.ExecuteScalar("Select X_CustomerName From Inv_Customer where N_CustomerID=@N_CustomerID and N_CompanyID=@nCompanyID  and N_FnYearID=@nFnYearID", CustParams, connection);
                    object objCustCode = dLayer.ExecuteScalar("Select X_CustomerCode From Inv_Customer where N_CustomerID=@N_CustomerID and N_CompanyID=@nCompanyID  and N_FnYearID=@nFnYearID", CustParams, connection);
                    object InvoiceNO = dLayer.ExecuteScalar("select X_ReceiptNO from inv_sales where n_salesid=" + nInvoiceID + " and N_CompanyID=" + nCompanyID, Params, connection);
                    object LastInvoiceNO = dLayer.ExecuteScalar("select top(1) N_lastUsedNo from inv_invoicecounter where n_formid=64 and n_companyid=" + nCompanyID + " and N_FnyearID=" + nFnYearID, Params, connection);
                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, this.N_FormID, nInvoiceID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID, 0, 0, User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), comments);


                    SqlTransaction transaction = connection.BeginTransaction();

                    string payRecieptqry = "select N_PayReceiptID from  Inv_PayReceipt where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_RefID=" + nInvoiceID + " and N_FormID=" + this.N_FormID + "";
                    object nRecieptID = dLayer.ExecuteScalar(payRecieptqry, connection, transaction);
                    if (nRecieptID != null && myFunctions.getIntVAL(nRecieptID.ToString()) > 0)
                    {
                        dLayer.ExecuteNonQuery(" delete from Acc_VoucherDetails Where N_CompanyID=" + nCompanyID + " and N_InventoryID=" + myFunctions.getIntVAL(nRecieptID.ToString()) + " and N_FnYearID=" + nFnYearID + " and X_TransType = 'SA'", connection, transaction);
                        dLayer.ExecuteNonQuery(" delete from Inv_PayReceiptDetails Where N_CompanyID=" + nCompanyID + " and N_PayReceiptID=" + myFunctions.getIntVAL(nRecieptID.ToString()) + " ", connection, transaction);
                        dLayer.ExecuteNonQuery(" delete from Inv_PayReceipt Where N_CompanyID=" + nCompanyID + " and N_PayReceiptID=" + myFunctions.getIntVAL(nRecieptID.ToString()) + " and  N_FnYearID=" + nFnYearID + " ", connection, transaction);
                    }
                    dLayer.DeleteData("Inv_SalesAdvanceSettlement", "N_SalesID", nInvoiceID, "N_CompanyID = " + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection, transaction);

                    var xUserCategory = myFunctions.GetUserCategory(User);// User.FindFirst(ClaimTypes.GroupSid)?.Value;
                    var nUserID = myFunctions.GetUserID(User);// User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    object objSalesReturnProcessed = dLayer.ExecuteScalar("Select Isnull(N_DebitNoteId,0) from Inv_SalesReturnMaster where N_CompanyID=" + nCompanyID + " and N_SalesId=" + nInvoiceID + " and isnull(B_IsSaveDraft,0)=0", connection, transaction);
                    object objPaymentProcessed = dLayer.ExecuteScalar("Select Isnull(N_PayReceiptId,0) from Inv_PayReceiptDetails where N_CompanyID=" + nCompanyID + " and N_InventoryId=" + nInvoiceID + " and X_TransType='SALES'", connection, transaction);
                    //Results = dLayer.DeleteData("Inv_SalesInvoice", "n_InvoiceID", N_InvoiceID, "",connection,transaction);
                    if (objSalesReturnProcessed == null)
                        objSalesReturnProcessed = 0;
                    if (objPaymentProcessed == null)
                        objPaymentProcessed = 0;
                    if (myFunctions.getIntVAL(objSalesReturnProcessed.ToString()) == 0 && myFunctions.getIntVAL(objPaymentProcessed.ToString()) == 0)
                    {
                        string X_Criteria = "N_SalesID=" + nInvoiceID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnYearID;
                        string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                        int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());

                        SortedList QueryParams = new SortedList(){
                                    {"@nCompanyID",nCompanyID},
                                    {"@nFnYearID",nFnYearID},
                                    {"@nUserID",nUserID},
                                    {"@xTransType","SALES"},
                                    {"@xSystemName","WebRequest"},
                                    {"@nSalesID",nInvoiceID},
                                    {"@nPartyID",nCustomerID},
                                    {"@nQuotationID",nQuotationID},
                                    {"@nBranchID",nBranchID}
                                   };

                        //Activity Log
                        string ipAddress = "";
                        if (Request.Headers.ContainsKey("X-Forwarded-For"))
                            ipAddress = Request.Headers["X-Forwarded-For"];
                        else
                            ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                        myFunctions.LogScreenActivitys(nFnYearID, nInvoiceID, TransRow["x_ReceiptNo"].ToString(), this.N_FormID, xButtonAction, ipAddress, "", User, dLayer, connection, transaction);





                        string status = myFunctions.UpdateApprovals(Approvals, nFnYearID, "SALES", nInvoiceID, TransRow["X_ReceiptNo"].ToString(), ProcStatus, "Inv_Sales", X_Criteria, objCustName.ToString(), User, dLayer, connection, transaction);
                        if (status != "Error")
                        {
                            if (ButtonTag == "6" || ButtonTag == "0")
                            {
                                SortedList DeleteParams = new SortedList(){
                                        {"N_CompanyID",nCompanyID},
                                        {"N_UserID",nUserID},
                                        {"X_TransType","SALES"},
                                        {"X_SystemName","WebRequest"},
                                        {"N_VoucherID",nInvoiceID}};

                                try
                                {
                                    Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                                }
                                catch (Exception ex)
                                {
                                    if (ex.Message == "53")
                                    {
                                        transaction.Rollback();
                                        return Ok(_api.Error(User, "Period Closed"));
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        return Ok(_api.Error(User, "Unable to delete sales Invoice"));

                                    }


                                }


                                if (Results <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Unable to delete sales Invoice"));
                                }
                                else
                                {

                                    object OSOID = dLayer.ExecuteScalar("Select N_SalesOrderID from Inv_Prescription where n_SalesID=@nSalesID and  n_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                                    if (OSOID == null) OSOID = 0;

                                    if (myFunctions.getIntVAL(OSOID.ToString()) > 0)
                                    {
                                        dLayer.ExecuteNonQuery("update Inv_Prescription set n_SalesID=null where n_SalesID=@nSalesID and  n_CompanyID=@nCompanyID", QueryParams, connection, transaction);

                                    }
                                    else
                                    {
                                        dLayer.ExecuteNonQuery("delete from Inv_Prescription where n_SalesID=@nSalesID and  n_CompanyID=@nCompanyID", QueryParams, connection, transaction);

                                    }
                                    dLayer.ExecuteNonQuery("delete from Inv_DeliveryDispatch where n_InvoiceID=@nSalesID and n_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                                    //   if (N_AmtSplit == 1)
                                    //     {                                                
                                    dLayer.ExecuteNonQuery("delete from Inv_SaleAmountDetails where n_SalesID=@nSalesID and n_BranchID=@nBranchID and n_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                                    dLayer.ExecuteNonQuery("delete from Inv_LoyaltyPointOut where n_SalesID=@nSalesID and n_PartyID=@nPartyID and n_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                                    // }  
                                    dLayer.ExecuteNonQuery("delete from Inv_ServiceContract where n_SalesID=@nSalesID and n_FnYearID=@nFnYearID and n_BranchID=@nBranchID and n_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                                    if (dLayer.ExecuteNonQuery("delete from Inv_StockMaster where n_SalesID=@nSalesID and x_Type='Negative' and n_InventoryID = 0 and n_CompanyID=@nCompanyID", QueryParams, connection, transaction) <= 0)
                                    {
                                        // transaction.Rollback();
                                        // return Ok(_api.Error(User,"Unable to delete sales Invoice"));
                                    }

                                    myAttachments.DeleteAttachment(dLayer, 1, nInvoiceID, N_CustomerId, nFnYearID, N_FormID, User, transaction, connection);

                                    //Invoice Counter Reset
                                    if (InvoiceNO == LastInvoiceNO)
                                    {
                                        dLayer.ExecuteNonQuery("update inv_invoicecounter set N_lastUsedNo=" + (myFunctions.getVAL(InvoiceNO.ToString()) - 1) + " where n_formid=64 and n_companyid=" + nCompanyID + " and N_FnyearID=" + nFnYearID, Params, connection, transaction);
                                    }
                                }
                                
                            }
                            else if (ButtonTag == "4")
                            {
                                dLayer.ExecuteNonQuery("delete from Acc_VoucherDetails_Segments where N_CompanyID=@nCompanyID AND N_FnYearID=@nFnYearID and X_TransType='SALES' AND N_AccTransID  in (select N_AccTransID from Acc_VoucherDetails where N_CompanyID=@nCompanyID AND N_FnYearID=@nFnYearID and X_TransType='SALES' AND X_VoucherNo='"+InvoiceNO+"')", QueryParams, connection, transaction);
                                dLayer.ExecuteNonQuery("delete from Acc_VoucherDetails where N_CompanyID=@nCompanyID AND N_FnYearID=@nFnYearID and X_TransType='SALES' AND X_VoucherNo='"+InvoiceNO+"'", QueryParams, connection, transaction);
                                result.Add("InvoiceNO",InvoiceNO);
                            }

                            if (myFunctions.CheckPermission(nCompanyID, 724, "Administrator", "X_UserCategory", dLayer, connection, transaction))
                                if (myFunctions.CheckPermission(nCompanyID, 81, xUserCategory.ToString(), "N_UserCategoryID", dLayer, connection, transaction))
                                    if (nQuotationID > 0)
                                        dLayer.ExecuteNonQuery("update Inv_SalesQuotation set N_Processed=0 where N_QuotationId= @nQuotationID and N_CompanyId=@nCompanyID and N_FnYearId= @nFnYearID", QueryParams, connection, transaction);
                                                  
                            //StatusUpdate
                            int tempQtn = 0, tempSO = 0, tempDevnote = 0;
                            for (int j = 0; j < DetailTable.Rows.Count; j++)
                            {
                                int nSOID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesOrderID"].ToString());
                                int nSQID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesQuotationID"].ToString());
                                int nDeliveryNoteID = myFunctions.getIntVAL(DetailTable.Rows[j]["N_DeliveryNoteID"].ToString());
                                if (nSOID > 0 && tempSO != nSOID)
                                {
                                    if (!myFunctions.UpdateTxnStatus(nCompanyID, nSOID, 81, true, dLayer, connection, transaction))
                                    {
                                        transaction.Rollback();
                                        return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                    }
                                }
                                tempSO = nSOID;

                                if (nSQID > 0 && tempQtn != nSQID)
                                {
                                    if (!myFunctions.UpdateTxnStatus(nCompanyID, nSQID, 80, true, dLayer, connection, transaction))
                                    {
                                        transaction.Rollback();
                                        return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                    }
                                }
                                tempDevnote = nSQID;

                                if (nDeliveryNoteID > 0)
                                {
                                    if (!myFunctions.UpdateTxnStatus(nCompanyID, nDeliveryNoteID, 884, true, dLayer, connection, transaction))
                                    {
                                        transaction.Rollback();
                                        return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                    }
                                }
                                tempQtn = tempDevnote;
                            };

                            transaction.Commit();
                            if (nFormID == 1741)
                            {
                                return Ok(_api.Success("Optical Invoice " + status + " Successfully"));
                            }
                            else if(nFormID ==1601) 
                            {
                               return Ok(_api.Success("Rental Sales Invoice " + status + " Successfully")); 
                            }
                            else
                                result.Add("ButtonTag",ButtonTag);
                                return Ok(_api.Success(result,"Sales Invoice " + status + " Successfully"));
                        }
                        else
                        {
                            transaction.Rollback();
                            if (nFormID == 1665)
                            {
                                return Ok(_api.Error(User, "Unable to delete Fee Collection"));
                            }
                            else
                            {
                                return Ok(_api.Error(User, "Unable to delete Sales Invoice"));
                            };
                        }
                    }
                    else
                    {
                        transaction.Rollback();
                        if (myFunctions.getIntVAL(objSalesReturnProcessed.ToString()) > 0  && nFormID== 64)
                            return Ok(_api.Error(User, "Sales Return processed! Unable to delete"));
                        else if (myFunctions.getIntVAL(objSalesReturnProcessed.ToString()) > 0 && nFormID == 1601)
                            return Ok(_api.Error(User, " Rental Sales Return processed! Unable to delete"));
                        else if (myFunctions.getIntVAL(objPaymentProcessed.ToString()) > 0)
                            return Ok(_api.Error(User, "Customer Payment processed! Unable to delete"));
                        else
                            return Ok(_api.Error(User, "Unable to delete!"));
                    }

                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }


        }


        [HttpGet("dummy")]
        public ActionResult GetSalesInvoiceDummy(int? nSalesId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(connectionString))
                {
                    Con.Open();
                    string sqlCommandText = "select * from Inv_Sales where N_SalesId=@p1";
                    SortedList mParamList = new SortedList() { { "@p1", nSalesId } };
                    DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList, Con);
                    masterTable = _api.Format(masterTable, "master");

                    string sqlCommandText2 = "select * from Inv_SalesDetails where N_SalesId=@p1";
                    SortedList dParamList = new SortedList() { { "@p1", nSalesId } };
                    DataTable detailTable = dLayer.ExecuteDataTable(sqlCommandText2, dParamList, Con);
                    detailTable = _api.Format(detailTable, "details");

                    string sqlCommandText3 = "select * from Inv_SaleAmountDetails where N_SalesId=@p1";
                    DataTable dtAmountDetails = dLayer.ExecuteDataTable(sqlCommandText3, dParamList, Con);
                    dtAmountDetails = _api.Format(dtAmountDetails, "saleamountdetails");

                    if (detailTable.Rows.Count == 0) { return Ok(new { }); }
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add(masterTable);
                    dataSet.Tables.Add(detailTable);
                    dataSet.Tables.Add(dtAmountDetails);

                    return Ok(dataSet);

                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
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

        [HttpGet("updateSalesPrice")]
        public ActionResult ValidateSellingPrice(int nBranchID, int nItemID, decimal nSPrice)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@nBranchID", nBranchID);
                    Params.Add("@nItemID", nItemID);
                    Params.Add("@nSPrice", nSPrice);
                    DataTable Sprice = dLayer.ExecuteDataTable("Select N_PriceID,N_PriceVal,N_itemId From Inv_ItemPriceMaster  Where N_CompanyID=@nCompanyID and N_BranchID=@nBranchID and N_itemId=@nItemID and N_PriceID in(Select N_PkeyId from Gen_LookupTable where X_Name=@nSPrice and N_CompanyID=@nCompanyID)", Params, connection);
                    return Ok(_api.Success(Sprice));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        [HttpGet("getItemDetails")]
        public ActionResult GetItem(int nLocationID, int nBranchID, string xInputVal, int nCustomerID, string xBatch, string xInvoiceNo)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int N_DefSPriceID = 0;
                int nCompanyID = myFunctions.GetCompanyID(User);

                var UserCategoryID = myFunctions.GetUserCategory(User);
                N_DefSPriceID = myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "DefSPriceTypeID", "N_Value", "N_UserCategoryID", UserCategoryID.ToString(), myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection));
                int nSPriceID = N_DefSPriceID;
                string ItemClass = "", ItemCondition = "";

                ItemCondition = "([Item Code] =@xItemCode)";

                if (ItemCondition != "")
                    ItemCondition += "and B_InActive=0";

                SortedList Params = new SortedList();
                Params.Add("@xItemCode", xInputVal);
                Params.Add("@nLocationID", nLocationID);
                Params.Add("@xBatch", xBatch == null ? "" : xBatch);
                Params.Add("@nCompanyID", nCompanyID);
                Params.Add("@nSPriceID", nSPriceID);
                Params.Add("@nDefSPriceID", N_DefSPriceID);
                Params.Add("@nBranchID", nBranchID);

                bool B_SPRiceType = false;

                object objSPrice = dLayer.ExecuteScalar("Select Isnull(N_Value,0) from Gen_Settings where N_CompanyID=@nCompanyID and X_Group='Inventory' and X_Description='Selling Price Calculation'", Params, connection);
                if (objSPrice != null)
                {
                    if (myFunctions.getIntVAL(objSPrice.ToString()) == 4)
                        B_SPRiceType = true;
                    else
                        B_SPRiceType = false;

                }


                string SQL = "";
                // if (xBatch != "")
                //     SQL = "Select *,dbo.SP_BatchStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch,NULL)As N_AvlStock ,dbo.SP_Cost_Loc(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_ItemUnit,@nLocationID) As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice,vw_InvItem_Search.N_TaxCategoryID, vw_InvItem_Search.X_DisplayName  From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nCompanyID";
                // else
                //     SQL = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch, 'location')As N_AvlStock ,dbo.SP_Cost_Loc(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_ItemUnit,@nLocationID) As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice,vw_InvItem_Search.N_TaxCategoryID, vw_InvItem_Search.X_DisplayName  From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nCompanyID";
                // if (B_SPRiceType)
                // {
                //     if (nSPriceID > 0)
                //     {
                //         SQL = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch, 'location') As N_AvlStock ,dbo.SP_BatchStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch,NULL) As N_Stock ,dbo.SP_Cost_Loc(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_ItemUnit,@nLocationID) As N_LPrice ,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nSPriceID,@nBranchID) As N_SPrice  From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nCompanyID";
                //     }
                //     else
                //         SQL = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch, 'location')As N_AvlStock ,dbo.SP_BatchStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch,NULL) As N_Stock ,dbo.SP_Cost_Loc(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_ItemUnit,@nLocationID) As N_LPrice ,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nDefSPriceID,@nBranchID) As N_SPrice  From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nCompanyID";
                // }
                // if (IsSerial)
                // {
                //     SQL = "Select vw_InvItem_Search.*,dbo.SP_Stock(vw_InvItem_Search.N_ItemID) As N_Stock,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch, 'location')As N_AvlStock, case when ISNULL(dbo.SP_Cost_IMEI(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,isnull(vw_InvItem_Search.X_SalesUnit,vw_InvItem_Search.X_StockUnit), Inv_StockMaster_IMEI.N_StockID),0)=0 Then dbo.SP_Cost_Loc(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit,@nLocationID) else dbo.SP_Cost_IMEI(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,isnull(vw_InvItem_Search.X_SalesUnit,vw_InvItem_Search.X_StockUnit), Inv_StockMaster_IMEI.N_StockID) end As N_LPrice,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice,Inv_StockMaster_IMEI.*  From vw_InvItem_Search " +
                //    " Inner Join Inv_StockMaster On vw_InvItem_Search.N_ItemID = Inv_StockMaster.N_ItemID and vw_InvItem_Search.N_CompanyID =Inv_StockMaster.N_CompanyID " +
                //    " Left Outer Join Inv_StockMaster_IMEI On Inv_StockMaster.N_StockID = Inv_StockMaster_IMEI.N_StockID and Inv_StockMaster.N_CompanyID =Inv_StockMaster_IMEI.N_CompanyID " +
                //    " Where " + ItemCondition + " and vw_InvItem_Search.N_CompanyID=@nCompanyID and Inv_StockMAster_IMEI.N_Status<>1 Order By Inv_StockMaster_IMEI.N_Status, Inv_StockMaster.N_StockID";
                //     if (B_SPRiceType)
                //     {
                //         if (nSPriceID > 0)
                //         {
                //             SQL = "Select vw_InvItem_Search.*,dbo.SP_Stock(vw_InvItem_Search.N_ItemID) As N_Stock ,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch, 'location')As N_AvlStock, case when ISNULL(dbo.SP_Cost_IMEI(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit, Inv_StockMaster_IMEI.N_StockID),0)=0 Then dbo.SP_Cost_Loc(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit,@nLocationID) else dbo.SP_Cost_IMEI(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit, Inv_StockMaster_IMEI.N_StockID) end As N_LPrice,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nSPriceID,@nBranchID) As N_SPrice,Inv_StockMaster_IMEI.*  From vw_InvItem_Search " +
                //      " Inner Join Inv_StockMaster On vw_InvItem_Search.N_ItemID = Inv_StockMaster.N_ItemID and vw_InvItem_Search.N_CompanyID =Inv_StockMaster.N_CompanyID " +
                //      " Left Outer Join Inv_StockMaster_IMEI On Inv_StockMaster.N_StockID = Inv_StockMaster_IMEI.N_StockID and Inv_StockMaster.N_CompanyID =Inv_StockMaster_IMEI.N_CompanyID " +
                //      " Where " + ItemCondition + " and vw_InvItem_Search.N_CompanyID=@nCompanyID and Inv_StockMAster_IMEI.N_Status<>1 Order By Inv_StockMaster_IMEI.N_Status, Inv_StockMaster.N_StockID";
                //         }
                //         else
                //             SQL = "Select vw_InvItem_Search.*,dbo.SP_Stock(vw_InvItem_Search.N_ItemID) As N_Stock ,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch, 'location')As N_AvlStock, case when ISNULL(dbo.SP_Cost_IMEI(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit, Inv_StockMaster_IMEI.N_StockID),0)=0 Then dbo.SP_Cost_Loc(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit,@nLocationID) else dbo.SP_Cost_IMEI(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit, Inv_StockMaster_IMEI.N_StockID) end As N_LPrice,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nDefSPriceID,@nBranchID) As N_SPrice,Inv_StockMaster_IMEI.*  From vw_InvItem_Search " +
                //    " Inner Join Inv_StockMaster On vw_InvItem_Search.N_ItemID = Inv_StockMaster.N_ItemID and vw_InvItem_Search.N_CompanyID =Inv_StockMaster.N_CompanyID " +
                //    " Left Outer Join Inv_StockMaster_IMEI On Inv_StockMaster.N_StockID = Inv_StockMaster_IMEI.N_StockID and Inv_StockMaster.N_CompanyID =Inv_StockMaster_IMEI.N_CompanyID " +
                //    " Where " + ItemCondition + " and vw_InvItem_Search.N_CompanyID=@nCompanyID and Inv_StockMAster_IMEI.N_Status<>1 Order By Inv_StockMaster_IMEI.N_Status, Inv_StockMaster.N_StockID";
                //     }
                // }


                if (xBatch != "")
                    SQL = "Select *,dbo.SP_BatchStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch,NULL)As N_AvlStock ,0 As N_LPrice ,0 As N_SPrice,vw_InvItem_Search.N_TaxCategoryID, vw_InvItem_Search.X_DisplayName  From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nCompanyID";
                else
                    SQL = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch, 'location')As N_AvlStock ,0 As N_LPrice ,0 As N_SPrice,vw_InvItem_Search.N_TaxCategoryID, vw_InvItem_Search.X_DisplayName  From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nCompanyID";
                if (B_SPRiceType)
                {
                    if (nSPriceID > 0)
                    {
                        SQL = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch, 'location') As N_AvlStock ,0 As N_Stock ,0 As N_LPrice ,0 As N_SPrice  From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nCompanyID";
                    }
                    else
                        SQL = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch, 'location')As N_AvlStock ,0 As N_Stock ,dbo.SP_Cost_Loc(vw_InvItem_Search.N_ItemID,0 As N_LPrice ,0 As N_SPrice  From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nCompanyID";
                }
                // if (IsSerial)
                // {
                //     SQL = "Select vw_InvItem_Search.*,dbo.SP_Stock(vw_InvItem_Search.N_ItemID) As N_Stock,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch, 'location')As N_AvlStock, case when ISNULL(dbo.SP_Cost_IMEI(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,isnull(vw_InvItem_Search.X_SalesUnit,vw_InvItem_Search.X_StockUnit), Inv_StockMaster_IMEI.N_StockID),0)=0 Then dbo.SP_Cost_Loc(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit,@nLocationID) else dbo.SP_Cost_IMEI(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,isnull(vw_InvItem_Search.X_SalesUnit,vw_InvItem_Search.X_StockUnit), Inv_StockMaster_IMEI.N_StockID) end As N_LPrice,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice,Inv_StockMaster_IMEI.*  From vw_InvItem_Search " +
                //    " Inner Join Inv_StockMaster On vw_InvItem_Search.N_ItemID = Inv_StockMaster.N_ItemID and vw_InvItem_Search.N_CompanyID =Inv_StockMaster.N_CompanyID " +
                //    " Left Outer Join Inv_StockMaster_IMEI On Inv_StockMaster.N_StockID = Inv_StockMaster_IMEI.N_StockID and Inv_StockMaster.N_CompanyID =Inv_StockMaster_IMEI.N_CompanyID " +
                //    " Where " + ItemCondition + " and vw_InvItem_Search.N_CompanyID=@nCompanyID and Inv_StockMAster_IMEI.N_Status<>1 Order By Inv_StockMaster_IMEI.N_Status, Inv_StockMaster.N_StockID";
                //     if (B_SPRiceType)
                //     {
                //         if (nSPriceID > 0)
                //         {
                //             SQL = "Select vw_InvItem_Search.*,dbo.SP_Stock(vw_InvItem_Search.N_ItemID) As N_Stock ,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch, 'location')As N_AvlStock, case when ISNULL(dbo.SP_Cost_IMEI(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit, Inv_StockMaster_IMEI.N_StockID),0)=0 Then dbo.SP_Cost_Loc(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit,@nLocationID) else dbo.SP_Cost_IMEI(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit, Inv_StockMaster_IMEI.N_StockID) end As N_LPrice,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nSPriceID,@nBranchID) As N_SPrice,Inv_StockMaster_IMEI.*  From vw_InvItem_Search " +
                //      " Inner Join Inv_StockMaster On vw_InvItem_Search.N_ItemID = Inv_StockMaster.N_ItemID and vw_InvItem_Search.N_CompanyID =Inv_StockMaster.N_CompanyID " +
                //      " Left Outer Join Inv_StockMaster_IMEI On Inv_StockMaster.N_StockID = Inv_StockMaster_IMEI.N_StockID and Inv_StockMaster.N_CompanyID =Inv_StockMaster_IMEI.N_CompanyID " +
                //      " Where " + ItemCondition + " and vw_InvItem_Search.N_CompanyID=@nCompanyID and Inv_StockMAster_IMEI.N_Status<>1 Order By Inv_StockMaster_IMEI.N_Status, Inv_StockMaster.N_StockID";
                //         }
                //         else
                //             SQL = "Select vw_InvItem_Search.*,dbo.SP_Stock(vw_InvItem_Search.N_ItemID) As N_Stock ,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,@xBatch, 'location')As N_AvlStock, case when ISNULL(dbo.SP_Cost_IMEI(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit, Inv_StockMaster_IMEI.N_StockID),0)=0 Then dbo.SP_Cost_Loc(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit,@nLocationID) else dbo.SP_Cost_IMEI(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit, Inv_StockMaster_IMEI.N_StockID) end As N_LPrice,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nDefSPriceID,@nBranchID) As N_SPrice,Inv_StockMaster_IMEI.*  From vw_InvItem_Search " +
                //    " Inner Join Inv_StockMaster On vw_InvItem_Search.N_ItemID = Inv_StockMaster.N_ItemID and vw_InvItem_Search.N_CompanyID =Inv_StockMaster.N_CompanyID " +
                //    " Left Outer Join Inv_StockMaster_IMEI On Inv_StockMaster.N_StockID = Inv_StockMaster_IMEI.N_StockID and Inv_StockMaster.N_CompanyID =Inv_StockMaster_IMEI.N_CompanyID " +
                //    " Where " + ItemCondition + " and vw_InvItem_Search.N_CompanyID=@nCompanyID and Inv_StockMAster_IMEI.N_Status<>1 Order By Inv_StockMaster_IMEI.N_Status, Inv_StockMaster.N_StockID";
                //     }
                // }
                DataTable ItemDetails = dLayer.ExecuteDataTable(SQL, Params, connection);
                if (ItemDetails.Rows.Count != 1)
                    return Ok(_api.Warning("Invalid Item"));

                ItemClass = ItemDetails.Rows[0]["N_ClassID"].ToString();

                string Query = "select isnull(s.N_Qty,'0') from Inv_SalesDetails s inner join Inv_Sales sm on s.N_SalesID = sm.N_SalesId inner join Inv_ItemMaster i on s.N_ItemID = i.N_ItemID where i.X_ItemCode =@xItemCode and sm.X_ReceiptNo = '" + xInvoiceNo + "'";
                if (xBatch != "")
                    Query += "and X_BatchCode = '" + xBatch + "' ";
                object O_InvoicedQty = dLayer.ExecuteScalar(Query, Params, connection);
                int X_Stock = myFunctions.getIntVAL(ItemDetails.Rows[0]["N_AvlStock"].ToString());
                double stock = O_InvoicedQty != null ? myFunctions.getVAL(O_InvoicedQty.ToString()) + X_Stock : X_Stock;
                if (myFunctions.getIntVAL(ItemClass.ToString()) == 4)
                {
                    ItemDetails.Rows[0]["n_Qty"] = 1;
                }

                int N_ItemID = myFunctions.getIntVAL(ItemDetails.Rows[0]["N_ItemID"].ToString());
                object Mrp = dLayer.ExecuteScalar("Select top 1 N_Mrp from Inv_PurchaseDetails where N_ItemId=" + N_ItemID.ToString() + " and N_CompanyID=" + nCompanyID + " Order By N_PurchaseDetailsId desc", connection);
                myFunctions.AddNewColumnToDataTable(ItemDetails, "Mrp", typeof(decimal), Mrp);
                myFunctions.AddNewColumnToDataTable(ItemDetails, "stock", typeof(double), stock);
                myFunctions.AddNewColumnToDataTable(ItemDetails, "N_DefSPriceID", typeof(int), N_DefSPriceID);
                string message = null;




                bool B_LastPurchaseCost = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings(this.N_FormID.ToString(), "LastPurchaseCost", "N_Value", "N_UserCategoryID", UserCategoryID.ToString(), myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                object LastPurchaseCost = 0;
                bool B_NegStockEnabled = true;

                if (B_LastPurchaseCost)
                {
                    LastPurchaseCost = dLayer.ExecuteScalar("Select TOP(1) ISNULL(N_LPrice,0) from Inv_StockMaster Where N_ItemID=" + N_ItemID.ToString() + " and N_CompanyID=@nCompanyID and N_LocationID=@nLocationID and (X_Type='Purchase' or X_Type='Opening'or X_Type='TransferRecive') Order by N_StockID Desc", Params, connection);
                }
                myFunctions.AddNewColumnToDataTable(ItemDetails, "LastPurchaseCost", typeof(decimal), LastPurchaseCost);



                if (ItemClass != "1" && ItemClass != "4")
                {
                    B_NegStockEnabled = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "Negative Stock Enabled", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    if (!B_NegStockEnabled)
                        if (stock <= 0)
                        {
                            //message="There is no enough stock qty to proceed... Do You Want To Continue";
                            message = "There is no enough stock";
                        }
                }

                object value = dLayer.ExecuteScalar("select N_DiscPerc from inv_CustomerDiscount where N_ProductID = '" + N_ItemID + "' and N_CustomerID = '" + nCustomerID + "' and N_CompanyID = '" + nCompanyID + "'", connection);
                myFunctions.AddNewColumnToDataTable(ItemDetails, "N_DiscPerc", typeof(decimal), value);

                // ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "SubItems", typeof(DataTable), null);

                // foreach (DataRow item in ItemDetails.Rows)
                // {
                //     if (myFunctions.getIntVAL(item["N_ClassID"].ToString()) == 1 || myFunctions.getIntVAL(item["N_ClassID"].ToString()) == 3)
                //     {
                //         string subItemSql = "select X_ItemName,N_Qty,N_ItemID,N_MainItemID,N_CompanyID,N_ItemDetailsID,X_ItemCode,X_ItemUnit from vw_InvItemDetails where N_MainItemID=" + myFunctions.getIntVAL(item["N_ItemID"].ToString()) + " and N_CompanyID=" + nCompanyId;
                //         DataTable subTbl = dLayer.ExecuteDataTable(subItemSql, connection);
                //         item["SubItems"] = subTbl;
                //     }
                // }
                // ItemDetails.AcceptChanges();


                ItemDetails.AcceptChanges();
                ItemDetails = _api.Format(ItemDetails);
                return Ok(_api.Success(ItemDetails, message));

            }

        }
        [HttpGet("deliveryNoteProduct")]
        public ActionResult ProductList(int nFnYearID, int nCustomerID, bool bAllbranchData, int nBranchID,bool isShipping,int nDivisionID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);

            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nCustomerID", nCustomerID);
            Params.Add("@nDivisionID", nDivisionID);



            string sqlCommandText = "";
            string criteria = "";

            if(nDivisionID>0){
                criteria = " and Inv_DeliveryNote.N_DivisionID=@nDivisionID";
            }
            else{
                criteria="";
            }
            if(isShipping)
            {
                 if (bAllbranchData)
                sqlCommandText = " SELECT        Inv_DeliveryNote.N_CustomerId, Inv_DeliveryNote.N_CompanyId, Inv_Customer.X_CustomerCode, Inv_Customer.X_CustomerName,"
                                 +"Inv_DeliveryNote.N_SalesOrderID, Inv_DeliveryNote.N_DeliveryNoteId, Inv_DeliveryNote.N_BranchId, Inv_DeliveryNote.X_CustPONo, Inv_DeliveryNote.X_ReceiptNo AS X_DeliveryNoteNo "
                                 +"FROM            Inv_DeliveryNote INNER JOIN    Inv_DeliveryNoteDetails ON Inv_DeliveryNoteDetails.N_CompanyID = Inv_DeliveryNote.N_CompanyId AND Inv_DeliveryNoteDetails.N_DeliveryNoteID = Inv_DeliveryNote.N_DeliveryNoteId "
                                 +"LEFT OUTER JOIN        Inv_Customer ON  Inv_DeliveryNote.N_CustomerId = Inv_Customer.N_CustomerID AND  Inv_DeliveryNote.N_FnYearId = Inv_Customer.N_FnYearID "
                                 +"WHERE        (Inv_DeliveryNote.N_CompanyId = @nCompanyID) AND (Inv_DeliveryNote.N_CustomerId = @nCustomerID) AND  (isnull(Inv_DeliveryNote.B_IsSaveDraft,0) = 0) AND (isnull(Inv_DeliveryNote.B_BeginingBalEntry,0) = 0) AND "
                                 +"(Inv_DeliveryNote.N_DeliveryNoteID NOT IN (SELECT        ISNULL(N_DeliveryNoteID, 0) AS N_DeliveryNoteID   FROM   Inv_ShippingDetails  WHERE        (N_CompanyID = @nCompanyID)"
                                 +"GROUP BY N_DeliveryNoteID))  Group by Inv_DeliveryNote.N_CustomerId, Inv_DeliveryNote.N_CompanyId, Inv_Customer.X_CustomerCode, Inv_Customer.X_CustomerName, Inv_DeliveryNote.N_SalesOrderID,"
                                 +"Inv_DeliveryNote.N_DeliveryNoteId, Inv_DeliveryNote.N_BranchId, Inv_DeliveryNote.X_CustPONo, Inv_DeliveryNote.X_ReceiptNo,Inv_DeliveryNote.B_BeginingBalEntry ";
            else
                sqlCommandText = "  SELECT        Inv_DeliveryNote.N_CustomerId, Inv_DeliveryNote.N_CompanyId, Inv_Customer.X_CustomerCode, Inv_Customer.X_CustomerName,"
                                 +"Inv_DeliveryNote.N_SalesOrderID, Inv_DeliveryNote.N_DeliveryNoteId, Inv_DeliveryNote.N_BranchId, Inv_DeliveryNote.X_CustPONo, Inv_DeliveryNote.X_ReceiptNo AS X_DeliveryNoteNo "
                                 +"FROM            Inv_DeliveryNote INNER JOIN    Inv_DeliveryNoteDetails ON Inv_DeliveryNoteDetails.N_CompanyID = Inv_DeliveryNote.N_CompanyId AND Inv_DeliveryNoteDetails.N_DeliveryNoteID = Inv_DeliveryNote.N_DeliveryNoteId "
                                 +"LEFT OUTER JOIN        Inv_Customer ON  Inv_DeliveryNote.N_CustomerId = Inv_Customer.N_CustomerID AND  Inv_DeliveryNote.N_FnYearId = Inv_Customer.N_FnYearID "
                                 +"WHERE        (Inv_DeliveryNote.N_CompanyId = @nCompanyID) AND (Inv_DeliveryNote.N_CustomerId = @nCustomerID) AND  (isnull(Inv_DeliveryNote.B_IsSaveDraft,0) = 0)  AND (Inv_DeliveryNote.N_BranchID = " + nBranchID + ")  AND (isnull(Inv_DeliveryNote.B_BeginingBalEntry,0) = 0) AND "
                                 +"(Inv_DeliveryNote.N_DeliveryNoteID NOT IN (SELECT        ISNULL(N_DeliveryNoteID, 0) AS N_DeliveryNoteID   FROM   Inv_ShippingDetails  WHERE        (N_CompanyID = @nCompanyID)"
                                 +"GROUP BY N_DeliveryNoteID))  Group by Inv_DeliveryNote.N_CustomerId, Inv_DeliveryNote.N_CompanyId, Inv_Customer.X_CustomerCode, Inv_Customer.X_CustomerName, Inv_DeliveryNote.N_SalesOrderID,"
                                 +"Inv_DeliveryNote.N_DeliveryNoteId, Inv_DeliveryNote.N_BranchId, Inv_DeliveryNote.X_CustPONo, Inv_DeliveryNote.X_ReceiptNo,Inv_DeliveryNote.B_BeginingBalEntry";
            }
            else{
            if (bAllbranchData)
                sqlCommandText = " SELECT        Inv_DeliveryNote.N_CustomerId, Inv_DeliveryNote.N_CompanyId, Inv_Customer.X_CustomerCode, Inv_Customer.X_CustomerName, Inv_CustomerProjects.X_ProjectName, Inv_DeliveryNote.N_SalesOrderID, "
                                + "Inv_DeliveryNote.N_DeliveryNoteId, Inv_DeliveryNote.N_BranchId, Inv_DeliveryNote.X_CustPONo, Inv_DeliveryNote.X_ReceiptNo AS X_DeliveryNoteNo "
                                + " FROM            Inv_DeliveryNote INNER JOIN  "
                                + " Inv_DeliveryNoteDetails ON Inv_DeliveryNoteDetails.N_CompanyID = Inv_DeliveryNote.N_CompanyId AND Inv_DeliveryNoteDetails.N_DeliveryNoteID = Inv_DeliveryNote.N_DeliveryNoteId LEFT OUTER JOIN "
                                + "Inv_Customer ON  Inv_DeliveryNote.N_CustomerId = Inv_Customer.N_CustomerID AND  "
                                + "Inv_DeliveryNote.N_FnYearId = Inv_Customer.N_FnYearID LEFT OUTER JOIN "
                                + "Inv_CustomerProjects ON Inv_DeliveryNote.N_ProjectID = Inv_CustomerProjects.N_ProjectID AND Inv_DeliveryNote.N_CompanyId = Inv_CustomerProjects.N_CompanyID "
                                + "WHERE        (Inv_DeliveryNote.N_CompanyId = @nCompanyID) AND (Inv_DeliveryNote.N_CustomerId = @nCustomerID) AND (isnull(Inv_DeliveryNote.B_IsSaveDraft,0) = 0) AND (isnull(Inv_DeliveryNote.B_BeginingBalEntry,0) = 0) "+criteria+" AND (Inv_DeliveryNote.N_DeliveryNoteID NOT IN "
                                + "    (SELECT        ISNULL(N_DeliveryNoteID, 0) AS N_DeliveryNoteID "
                                + "     FROM            Inv_SalesDetails  "
                                + "   WHERE        (N_CompanyID = @nCompanyID)  "
                                 + "   GROUP BY N_DeliveryNoteID))  "
                                + " Group by Inv_DeliveryNote.N_CustomerId, Inv_DeliveryNote.N_CompanyId, Inv_Customer.X_CustomerCode, Inv_Customer.X_CustomerName, Inv_CustomerProjects.X_ProjectName, Inv_DeliveryNote.N_SalesOrderID,  "
                                + "       Inv_DeliveryNote.N_DeliveryNoteId, Inv_DeliveryNote.N_BranchId, Inv_DeliveryNote.X_CustPONo, Inv_DeliveryNote.X_ReceiptNo,Inv_DeliveryNote.B_BeginingBalEntry "
                                 + " order by Inv_DeliveryNote.X_ReceiptNo";
            else
                sqlCommandText = " SELECT        Inv_DeliveryNote.N_CustomerId, Inv_DeliveryNote.N_CompanyId, Inv_Customer.X_CustomerCode, Inv_Customer.X_CustomerName, Inv_CustomerProjects.X_ProjectName, Inv_DeliveryNote.N_SalesOrderID, "
                                + "Inv_DeliveryNote.N_DeliveryNoteId, Inv_DeliveryNote.N_BranchId, Inv_DeliveryNote.X_CustPONo, Inv_DeliveryNote.X_ReceiptNo AS X_DeliveryNoteNo "
                                + " FROM            Inv_DeliveryNote INNER JOIN  "
                                + " Inv_DeliveryNoteDetails ON  Inv_DeliveryNoteDetails.N_DeliveryNoteID = Inv_DeliveryNote.N_DeliveryNoteId LEFT OUTER JOIN "
                                + "Inv_Customer ON Inv_DeliveryNote.N_CompanyId = Inv_Customer.N_CompanyID AND Inv_DeliveryNote.N_CustomerId = Inv_Customer.N_CustomerID AND  "
                                + "Inv_DeliveryNote.N_FnYearId = Inv_Customer.N_FnYearID LEFT OUTER JOIN "
                                + "Inv_CustomerProjects ON Inv_DeliveryNote.N_ProjectID = Inv_CustomerProjects.N_ProjectID AND Inv_DeliveryNote.N_CompanyId = Inv_CustomerProjects.N_CompanyID "
                                + "WHERE        (Inv_DeliveryNote.N_CompanyId = @nCompanyID) AND (Inv_DeliveryNote.N_CustomerId = @nCustomerID) AND (isnull(Inv_DeliveryNote.B_BeginingBalEntry,0) = 0) AND (isnull(Inv_DeliveryNote.B_IsSaveDraft,0) = 0)  AND (Inv_DeliveryNote.N_BranchID = " + nBranchID + ") "+criteria+" AND (Inv_DeliveryNote.N_DeliveryNoteID NOT IN "
                                + "    (SELECT        ISNULL(N_DeliveryNoteID, 0) AS N_DeliveryNoteID "
                                + "     FROM            Inv_SalesDetails  "
                                + "   WHERE        (N_CompanyID = @nCompanyID)  "
                                 + "   GROUP BY N_DeliveryNoteID))  "
                                + " Group by Inv_DeliveryNote.N_CustomerId, Inv_DeliveryNote.N_CompanyId, Inv_Customer.X_CustomerCode, Inv_Customer.X_CustomerName, Inv_CustomerProjects.X_ProjectName, Inv_DeliveryNote.N_SalesOrderID,  "
                                + "       Inv_DeliveryNote.N_DeliveryNoteId, Inv_DeliveryNote.N_BranchId, Inv_DeliveryNote.X_CustPONo, Inv_DeliveryNote.X_ReceiptNo,Inv_DeliveryNote.B_BeginingBalEntry "
                                 + " order by Inv_DeliveryNote.X_ReceiptNo";
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




        [HttpGet("invStockInfo")]
        public ActionResult GetInvStockInfoList(int nCompanyID, int nItemID, int nLocationID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // int nCompanyID = myFunctions.GetCompanyID(User);
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();

                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";

                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nItemID);
                    // Params.Add("@p3",nFnYearID);
                    Params.Add("@p4", nLocationID);

                    if (nItemID > 0)
                    {

                        sqlCommandText = "select * from vw_StockPOSOQty where N_CompanyID=@p1 and  N_ItemID=@p2 and N_LocationID=@p4  ";

                    }
                    SortedList OutPut = new SortedList();

                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count from vw_StockPOSOQty where N_CompanyID=@p1 and n_ItemID=@p2  ";
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

        [HttpGet("serviceTimesheetSearch")]
        public ActionResult GetTimesheetList(int nCustomerId)
        {
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string crieteria = "";

            if (nCustomerId > 0)
                crieteria = " where N_FormID=1145 and N_CustomerID=@nCustomerId and N_CompanyID=@nCompanyId and N_ServiceSheetID not in (select ISNULL(N_ServiceSheetID, 0) from Inv_SalesDetails where N_CompanyID=@nCompanyId)";
            else
                crieteria = " where N_FormID=1145 and N_CompanyID=@nCompanyId and N_ServiceSheetID not in (select ISNULL(N_ServiceSheetID, 0) from Inv_SalesDetails where N_CompanyID=@nCompanyId)";

            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nCustomerId", nCustomerId);
            string sqlCommandText = "select N_CompanyID,N_ServiceSheetID,X_ServiceSheetCode,D_DateFrom,D_DateTo,N_SOID,X_OrderNo,N_CustomerID,X_CustomerName,N_ProjectID,X_ProjectName,X_DateFrom,X_DateTo,X_PurchaseOrderNo from vw_Inv_ServiceTimesheet " + crieteria + " order by N_ServiceSheetID DESC,X_ServiceSheetCode";
            try
            {
                DataTable ServiceTimesheetList = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    ServiceTimesheetList = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    ServiceTimesheetList = _api.Format(ServiceTimesheetList);
                    if (ServiceTimesheetList.Rows.Count == 0) { return Ok(_api.Notice("No Service Timesheet Found")); }
                }
                return Ok(_api.Success(ServiceTimesheetList));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
    }
}
