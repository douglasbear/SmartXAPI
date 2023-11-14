using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    [Route("salesorder")]
    [ApiController]
    public class Inv_SalesOrderController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly ITaskController taskController;
        private readonly string connectionString;
        private readonly int FormID;

        public Inv_SalesOrderController(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt, ITaskController task)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            taskController = task;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 81;
        }


        [HttpGet("list")]
        public ActionResult GetSalesOrderotationList(int? nCompanyId, int nFnYearId, bool bAllBranchData, int nBranchID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, string screen, int nCustomerID, bool salesOrder, int nFormID, string xImeiNo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";
                    string criteria = "";
                    string serviceOrderCriteria = "";
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int N_decimalPlace = 2;
                    string custPortalOrder = "";
                    string RentalOrderCriteria = "";
                    N_decimalPlace = myFunctions.getIntVAL(myFunctions.ReturnSettings("Sales", "Decimal_Place", "N_Value", nCompanyID, dLayer, connection));
                    N_decimalPlace = N_decimalPlace == 0 ? 2 : N_decimalPlace;
                    string historyOrder = "";
                    object serviceID = "";
                    if (nFormID == 1546)
                    {
                        serviceOrderCriteria = "and N_FormID=1546 ";
                    }
                    if (nFormID == 1571)
                    {
                        RentalOrderCriteria = "and N_FormID=1571 ";
                    }
                    if(nFormID ==81)
                    {
                        RentalOrderCriteria="and N_FormID=81 ";
                    }
                    if(xImeiNo!=""&& xImeiNo!=null)
                    {
                        object nDeviceID = dLayer.ExecuteScalar("select N_DeviceID From Inv_Device where X_SerialNo='" + xImeiNo + "' and N_CompanyID=" + nCompanyID + " ", Params, connection);
                        if (nDeviceID != null)
                        {
                            serviceID = dLayer.ExecuteScalar("select N_ServiceInfoID from Inv_ServiceInfo where N_DeviceID=" + nDeviceID + " and N_CompanyID=" + nCompanyID + "", Params, connection);
                        }
                        historyOrder = " and N_SalesOrderID in (select N_SalesOrderID from Inv_SalesOrderDetails where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and N_ServiceID=" + myFunctions.getIntVAL(serviceID.ToString()) + ")";


                    }


                    string UserPattern = myFunctions.GetUserPattern(User);
                    int nUserID = myFunctions.GetUserID(User);
                    string Pattern = "";
                    if (UserPattern != "")
                    {
                        Pattern = " and Left(X_Pattern,Len(@UserPattern))=@UserPattern ";
                        Params.Add("@UserPattern", UserPattern);
                    }
                    if (nCustomerID > 0)
                    {
                        custPortalOrder = " and N_UserID=" + nCustomerID + "";
                    }
                    // else
                    // {
                    //                     object HierarchyCount = dLayer.ExecuteScalar("select count(N_HierarchyID) from Sec_UserHierarchy where N_CompanyID="+nCompanyId, Params, connection);

                    //     if( myFunctions.getIntVAL(HierarchyCount.ToString())>0)
                    //         Pattern = " and N_UserID=" + nUserID;

                    // }

                    bool CheckClosedYear = Convert.ToBoolean(dLayer.ExecuteScalar("Select B_YearEndProcess From Acc_FnYear Where N_CompanyID=" + nCompanyId + " and N_FnYearID = " + nFnYearId, Params, connection));

                    if (screen == "Order")
                        criteria = "and MONTH(Cast(D_OrderDate as DateTime)) = MONTH(CURRENT_TIMESTAMP) and YEAR(D_OrderDate)= YEAR(CURRENT_TIMESTAMP)";

                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and ([Order No] like '%" + xSearchkey + "%' or Customer like '%" + xSearchkey + "%' or X_SalesmanName like '%" + xSearchkey + "%' or x_CustomerPO like '%" + xSearchkey + "%' or X_ProjectName like '%" + xSearchkey + "%' or X_PrjType like '%" + xSearchkey + "%' or X_ActionStatus like '%" + xSearchkey + "%')";

                    if ((xSortBy == null || xSortBy.Trim() == "") && salesOrder == false)
                    {
                        xSortBy = "orderDate asc";
                    }
                    else
                    {
                        xSortBy = "orderNo desc";
                    }

                    switch (xSortBy.Split(" ")[0])
                    {
                        case "orderNo":
                            xSortBy = "N_SalesOrderId " + xSortBy.Split(" ")[1];
                            break;
                        case "x_ClosingRemarks":
                            xSortBy = "X_ActionStatus" + xSortBy.Split(" ")[1];
                            break;
                        case "orderDate":
                            xSortBy = "Cast([Order Date] as DateTime )" + xSortBy.Split(" ")[1];
                            break;
                        case "n_Amount":
                            xSortBy = "Cast(REPLACE(n_Amount,',','') as Numeric(10," + N_decimalPlace + ")) " + xSortBy.Split(" ")[1];
                            break;
                        default: break;
                    }
                    xSortBy = " order by " + xSortBy;



                    if (CheckClosedYear == false)
                    {
                        if (bAllBranchData == true)
                        {
                            Searchkey ="";
                        }
                        else
                        {
                            Searchkey = Searchkey + " and N_BranchID=" + nBranchID + "  and B_YearEndProcess =0";
                        }
                    }
                    else
                    {
                        if (bAllBranchData == true)
                        {
                            Searchkey = "";
                        }
                        else
                        {
                            Searchkey = Searchkey + "  and N_BranchID=" + nBranchID + "  and B_YearEndProcess =0";
                        }
                    }


                    if (salesOrder == false)
                    {
                        if (Count == 0)
                            sqlCommandText = "select top(" + nSizeperpage + ") * from vw_pendingSO where N_CompanyID=@p1 and N_FnYearID=@p2 " + Pattern + criteria + custPortalOrder + serviceOrderCriteria + RentalOrderCriteria + Searchkey + " " + xSortBy;
                        else
                            sqlCommandText = "select top(" + nSizeperpage + ") * from vw_pendingSO where N_FnYearID=@p2 " + Pattern + criteria + custPortalOrder + serviceOrderCriteria + RentalOrderCriteria + Searchkey + " and N_SalesOrderId not in (select top(" + Count + ") N_SalesOrderId from vw_pendingSO where  N_FnYearID=@p2 " + Pattern + criteria + xSortBy + " ) " + xSortBy;
                    }

                    if (salesOrder == true)
                    {
                        if (Count == 0)
                        {
                            sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvSalesOrderNo_Search_Cloud where N_CompanyID=@p1 and N_FnYearID=@p2 " + Pattern + criteria + custPortalOrder + serviceOrderCriteria + RentalOrderCriteria + Searchkey + historyOrder + " " + xSortBy;
                        }
                        else
                            sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvSalesOrderNo_Search_Cloud where N_FnYearID=@p2 " + Pattern + criteria + custPortalOrder + serviceOrderCriteria + RentalOrderCriteria + Searchkey + " and N_SalesOrderId not in (select top(" + Count + ") N_SalesOrderId from vw_InvSalesOrderNo_Search_Cloud where  N_FnYearID=@p2 " + Pattern + criteria + xSortBy + " ) " + xSortBy;
                    }


                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearId);
                    SortedList OutPut = new SortedList();



                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    if (salesOrder == false)
                    {
                        sqlCommandCount = "select count(1) as N_Count,sum(Cast(REPLACE(n_Amount,',','') as Numeric(16," + N_decimalPlace + ")) ) as TotalAmount from vw_pendingSO where N_CompanyID=@p1 and N_FnYearID=@p2 " + Pattern + criteria + custPortalOrder + serviceOrderCriteria + Searchkey + "";
                    }
                    else
                    {
                        sqlCommandCount = "select count(1) as N_Count,sum(Cast(REPLACE(n_Amount,',','') as Numeric(16," + N_decimalPlace + ")) ) as TotalAmount from vw_InvSalesOrderNo_Search_Cloud where N_CompanyID=@p1 and N_FnYearID=@p2 " + Pattern + criteria + custPortalOrder + serviceOrderCriteria + Searchkey + "";
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
        [HttpGet("details")]
        public ActionResult GetSalesOrderDetails(int? nCompanyID, string xOrderNo, int nFnYearID, int nLocationID, bool bAllBranchData, int nBranchID, int nQuotationID, int n_OpportunityID, int nClaimID, string x_WarrantyNo,int nFormID)
        {
        
                if (xOrderNo != null)
                xOrderNo = xOrderNo.Replace("%2F", "/");
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            DataTable DataTable = new DataTable();
            DataTable Prescription = new DataTable();

            string Mastersql = "";
            string DetailSql = "";
            string crieteria = "";
            

                 if(nFormID>0)
                    {
                    crieteria = " and N_FormID = @nFormID ";
                    }
            if (bAllBranchData == true)
            {
                Mastersql = "SP_InvSalesOrder_Disp @nCompanyID,@xOrderNo,1,0,@nFnYearID";
            }
            else
            {
                Mastersql = "SP_InvSalesOrder_Disp @nCompanyID,@xOrderNo,1,@nBranchID,@nFnYearID";
                Params.Add("@nBranchID", nBranchID);
            }

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nFormID", nFormID);


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //CRM Quotation Checking
                    object N_QuotationID = 0;
                    if (n_OpportunityID > 0)
                    {
                        N_QuotationID = dLayer.ExecuteScalar("Select N_QuotationID from Inv_SalesQuotation where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_OpportunityID=" + n_OpportunityID, Params, connection);
                        if (N_QuotationID != null)
                            nQuotationID = myFunctions.getIntVAL(N_QuotationID.ToString());
                    }

                    if (nQuotationID > 0)
                    {

                        Params.Add("@nQuotationID", nQuotationID);
                        Mastersql = "select * from vw_Inv_SalesQuotationMaster_Disp where N_CompanyId=@nCompanyID and N_QuotationId=@nQuotationID";
                        MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = _api.Format(MasterTable, "Master");
                        if (!MasterTable.Columns.Contains("N_OpportunityID"))
                        {
                            MasterTable.Columns.Add("N_OpportunityID");
                            MasterTable.Rows[0]["N_OpportunityID"] = n_OpportunityID.ToString();
                        }

                        if (myFunctions.getIntVAL(MasterTable.Rows[0]["N_CustomerId"].ToString()) == 0)
                        {
                            object CustomerID = dLayer.ExecuteScalar("select N_CustomerId from Inv_Customer where N_CompanyID=@nCompanyID and N_CrmCompanyID=" + MasterTable.Rows[0]["n_CrmCompanyID"].ToString(), Params, connection);
                            object CustomerName = dLayer.ExecuteScalar("select X_CustomerName from Inv_Customer where N_CompanyID=@nCompanyID and N_CrmCompanyID=" + MasterTable.Rows[0]["n_CrmCompanyID"].ToString(), Params, connection);
                            if (CustomerID != null)
                            {
                                MasterTable.Rows[0]["N_CustomerId"] = CustomerID.ToString();
                                MasterTable.Rows[0]["X_CustomerName"] = CustomerName.ToString();
                            }

                        }
                        DetailSql = "";
                        DetailSql = "select * from vw_Inv_SalesQuotationDetails_Disp where N_CompanyId=@nCompanyID and N_QuotationId=@nQuotationID";
                        DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dt.Tables.Add(MasterTable);
                        dt.Tables.Add(DetailTable);
                        return Ok(_api.Success(dt));

                    }
                    if (n_OpportunityID > 0)
                    {
                        Params.Add("@nOpportunityID", n_OpportunityID);
                        Mastersql = "select * from vw_OpportunitytoSalesOrderMaster where N_CompanyId=@nCompanyID and N_OpportunityID=@nOpportunityID";
                        MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }

                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "n_CRMCustID", typeof(string), 0);
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "n_InvCustID", typeof(string), 0);
                        object nCRMCustID =dLayer.ExecuteScalar("select isNull(N_CrmCompanyID, 0) from Inv_Customer where N_CompanyId=@nCompanyID and N_CrmCompanyID="+myFunctions.getIntVAL(MasterTable.Rows[0]["N_CRMCompanyID"].ToString()), Params, connection);
                        object nInvCustID =dLayer.ExecuteScalar("select isNull(N_CrmCompanyID, 0) from Inv_Customer where N_CompanyId=@nCompanyID and N_CrmCompanyID="+myFunctions.getIntVAL(MasterTable.Rows[0]["N_InvoiceToID"].ToString()), Params, connection);

                        if (nCRMCustID != null)
                            MasterTable.Rows[0]["n_CRMCustID"] = myFunctions.getIntVAL(nCRMCustID.ToString());

                        if (nInvCustID != null)
                            MasterTable.Rows[0]["n_InvCustID"] = myFunctions.getIntVAL(nInvCustID.ToString());

                        MasterTable = _api.Format(MasterTable, "Master");
                        DetailSql = "";
                        DetailSql = "select * from vw_OpportunitytoSalesOrderDetails where N_CompanyId=@nCompanyID and N_OpportunityID=@nOpportunityID";
                        DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dt.Tables.Add(MasterTable);
                        dt.Tables.Add(DetailTable);
                        return Ok(_api.Success(dt));

                    }
                    if (nClaimID > 0)
                    {
                        Params.Add("@nClaimID", nClaimID);
                        Mastersql = "select * from vw_WarrantyToSO where N_CompanyId=@nCompanyID and N_ClaimID=@nClaimID";
                        MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_WarrantyNo", typeof(string), x_WarrantyNo);
                        MasterTable.AcceptChanges();
                        MasterTable = _api.Format(MasterTable, "Master");
                        DetailSql = "";
                        DetailSql = "select * from vw_WarrantyToSODetails where N_CompanyId=@nCompanyID and N_ClaimID=@nClaimID and N_ClassID=4";
                        string MaterialDetailssql = "select * from vw_WarrantyToSODetails where N_CompanyId=@nCompanyID and N_ClaimID=@nClaimID and N_ClassID<>4";
                        DataTable MaterialDetails = dLayer.ExecuteDataTable(MaterialDetailssql, Params, connection);


                        DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                        DetailTable = myFunctions.AddNewColumnToDataTable(DetailTable, "N_MaterialMainItemID", typeof(int), 0);
                        foreach (DataRow kvar in DetailTable.Rows)
                        {
                            kvar["N_MaterialMainItemID"] = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_ItemID from Inv_WarrantyClaimDetails where N_ClaimID=" + nClaimID + " and N_CompanyID=" + nCompanyID + " and N_ServiceItemID=" + myFunctions.getIntVAL(kvar["N_ItemID"].ToString()) + "", Params, connection).ToString());


                        }
                        DetailTable.AcceptChanges();
                        DetailTable = _api.Format(DetailTable, "Details");
                        MaterialDetails = _api.Format(MaterialDetails, "MaterialDetails");
                        dt.Tables.Add(MasterTable);
                        dt.Tables.Add(DetailTable);
                        dt.Tables.Add(MaterialDetails);
                        return Ok(_api.Success(dt));
                    }


                    Params.Add("@xOrderNo", xOrderNo);

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    MasterTable = _api.Format(MasterTable, "Master");

                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList DetailParams = new SortedList();
                    int N_OthTaxCategoryID = myFunctions.getIntVAL(MasterRow["N_OthTaxCategoryID"].ToString());
                    int N_SOrderID = myFunctions.getIntVAL(MasterRow["n_SalesOrderId"].ToString());

                    object POrderID =dLayer.ExecuteScalar("select N_POrderID from Inv_PurchaseOrder where N_CompanyID=@nCompanyID and N_SOId="+N_SOrderID, Params, connection);
                    if (POrderID != null)
                    {
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "POrderID", typeof(int), myFunctions.getIntVAL(POrderID.ToString()));
                    }

                    DetailParams.Add("@nSOrderID", N_SOrderID);
                    DetailParams.Add("@nCompanyID", nCompanyID);
                    object N_SalesOrderTypeID = dLayer.ExecuteScalar("Select N_OrderTypeID from Inv_SalesOrder where N_SalesOrderId=@nSOrderID and N_CompanyID=@nCompanyID", DetailParams, connection);
                    DetailParams.Add("@nSalesOrderTypeID", N_SalesOrderTypeID);
                    if (!MasterTable.Columns.Contains("N_OrderTypeID"))
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_OrderTypeID", typeof(string), N_SalesOrderTypeID);
                    if (!MasterTable.Columns.Contains("SalesOrderType"))
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "SalesOrderType", typeof(string), "");
                    if (!MasterTable.Columns.Contains("D_ContractEndDate"))
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "D_ContractEndDate", typeof(string), null);
                    MasterTable.Rows[0]["SalesOrderType"] = "";
                    if (N_SalesOrderTypeID.ToString() != "")

                    {
                        MasterTable.Rows[0]["SalesOrderType"] = dLayer.ExecuteScalar("Select X_TypeName from Gen_Defaults where N_DefaultId=50 and N_TypeId=@nSalesOrderTypeID", DetailParams, connection);
                        MasterTable.Rows[0]["D_ContractEndDate"] = dLayer.ExecuteScalar("Select D_ContractEndDate from Inv_SalesOrder where N_SalesOrderId=@nSOrderID and N_CompanyID=@nCompanyID", DetailParams, connection);
                    }
                    DetailParams.Add("n_LocationID", MasterRow["N_LocationID"]);
                    string Location = Convert.ToString(dLayer.ExecuteScalar("select X_LocationName from Inv_Location where N_CompanyID=@nCompanyID and N_LocationID=@n_LocationID", DetailParams, connection));
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_LocationName", typeof(string), Location);
                    object InSales = null, InDeliveryNote = null, CancelStatus = null, isProforma = false ; object DispatchNo = null;
                    if (myFunctions.getIntVAL(N_SalesOrderTypeID.ToString()) != 175)
                    {
                        DispatchNo = dLayer.ExecuteScalar("select x_dispatchNo from inv_materialDispatch where N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);
                       if (Convert.ToBoolean(MasterRow["N_Processed"]))
                        {
                            InSales = dLayer.ExecuteScalar("select x_ReceiptNo from Inv_Sales where N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);
                            isProforma = dLayer.ExecuteScalar("select isnull(B_IsProforma,0) from Inv_Sales where N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);
                            InDeliveryNote = dLayer.ExecuteScalar("select x_ReceiptNo from Inv_DeliveryNote where N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);
                            CancelStatus = dLayer.ExecuteScalar("select 1 from Inv_SalesOrder where B_CancelOrder=1 and N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);
                            
                        } 
                        
                       
                            object dispatchDone=null;
                           dispatchDone=dLayer.ExecuteScalar("select x_DispatchNo from Inv_MaterialDispatch where N_CompanyID=@nCompanyID and N_SalesOrderID=@nSOrderID", DetailParams, connection);
                           if (dispatchDone != null){
                           MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "DispatchDone", typeof(int), dispatchDone != null ? 1 : 0);
                        }
                       
                       
                    }
                    if (InDeliveryNote != null)
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "TxnStatus", typeof(string), InDeliveryNote != null ? "Delivery Note Processed" : "");
                    if (InDeliveryNote != null && InSales != null)
                    {
                        if (MasterTable.Columns.Contains("TxnStatus"))
                            MasterTable.Columns.Remove("TxnStatus");
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "TxnStatus", typeof(string), InSales != null ? "Invoice Processed" : "");
                    }
                    else if (InSales != null)
                    {
                        if (MasterTable.Columns.Contains("TxnStatus"))
                            MasterTable.Columns.Remove("TxnStatus");
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "TxnStatus", typeof(string), InSales != null ? "Invoice Processed" : "");
                    }
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "salesDone", typeof(int), InSales != null ? 1 : 0);
                      

                    object DNQty = dLayer.ExecuteScalar("SELECT SUM(Inv_DeliveryNoteDetails.N_Qty * Inv_ItemUnit.N_Qty) FROM Inv_DeliveryNoteDetails INNER JOIN Inv_ItemUnit ON Inv_DeliveryNoteDetails.N_ItemUnitID = Inv_ItemUnit.N_ItemUnitID AND Inv_DeliveryNoteDetails.N_CompanyID = Inv_ItemUnit.N_CompanyID AND Inv_DeliveryNoteDetails.N_ItemID = Inv_ItemUnit.N_ItemID where Inv_DeliveryNoteDetails.N_CompanyID=" + nCompanyID + " and Inv_DeliveryNoteDetails.N_SalesOrderID=" + myFunctions.getIntVAL(N_SOrderID.ToString()), DetailParams, connection);
                    object OrderQty1 = dLayer.ExecuteScalar("select SUM(Inv_SalesOrderDetails.N_Qty) from Inv_SalesOrderDetails where N_CompanyID=" + nCompanyID + " and N_SalesOrderId=" + myFunctions.getIntVAL(N_SOrderID.ToString()), DetailParams, connection);
                    if (DNQty != null && OrderQty1 != null)
                    {
                        if (myFunctions.getVAL(OrderQty1.ToString()) > myFunctions.getVAL(DNQty.ToString()))
                        {
                            InDeliveryNote = null;
                        }
                        else
                        {
                            InDeliveryNote = 1;
                        }
                    }

                   object N_SaleinvID = dLayer.ExecuteScalar("Select N_SalesID from inv_Sales where N_SalesOrderId=@nSOrderID and N_CompanyID=@nCompanyID", DetailParams, connection);
                    if(N_SaleinvID!=null)
                   {
                     MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "n_RentalInvID", typeof(int), N_SaleinvID);
                   }

                   object countOfOrder =  dLayer.ExecuteScalar("Select count(1) from vw_pendingSO Where N_CompanyID = " + nCompanyID + "  and N_SalesOrderId=" + myFunctions.getIntVAL(N_SOrderID.ToString()), DetailParams, connection);
                   
                   

                   
                   {
                    if(myFunctions.getIntVAL(countOfOrder.ToString())>0)
                    {
                         InDeliveryNote = null;
                    }
                   }
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_SalesReceiptNo", typeof(string), InSales);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "DeliveryNoteDone", typeof(int), InDeliveryNote != null ? 1 : 0);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_DeliveryNoteNo", typeof(string), InDeliveryNote);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "SalesOrderCanceled", typeof(string), CancelStatus);

                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "ChkCancelOrderEnabled", typeof(bool), true);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "isProformaDone", typeof(bool), isProforma);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_DispatchNo", typeof(string), DispatchNo);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "debitNoteDone", typeof(bool), false);                           

                    if (InSales != null)
                    {
                        object InvoicedQty = dLayer.ExecuteScalar("select SUM(Inv_SalesOrderDetails.N_Qty) from Inv_SalesOrderDetails where N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);
                        object OrderQty = dLayer.ExecuteScalar("select SUM(Inv_SalesDetails.N_Qty) from Inv_SalesDetails where N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);
                        if (InvoicedQty != null && OrderQty != null)
                        {
                            if (InvoicedQty.ToString() != OrderQty.ToString())
                                MasterTable.Rows[0]["ChkCancelOrderEnabled"] = true;
                        }
                    }

                    int N_ProjectID = myFunctions.getIntVAL(MasterRow["N_ProjectID"].ToString());
                    //MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_ProjectName", typeof(string), "");

                    // if (N_ProjectID > 0)
                    // {
                    //     DetailParams.Add("@nProjectID", N_ProjectID);
                    //     MasterTable.Rows[0]["X_ProjectName"] = Convert.ToString(dLayer.ExecuteScalar("select X_ProjectName from Inv_CustomerProjects where N_CompanyID=@nCompanyID and N_ProjectID=@nProjectID", DetailParams, connection));
                    // }
                    //MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_SalesmanName", typeof(string), "");

                    // if (MasterRow["N_SalesmanID"].ToString() != "")
                    // {
                    //     DetailParams.Add("@nSalesmanID", MasterRow["N_SalesmanID"].ToString());
                    //     MasterTable.Rows[0]["X_SalesmanName"] = Convert.ToString(dLayer.ExecuteScalar("select X_SalesmanName from Inv_Salesman where N_CompanyID=@nCompanyID and N_SalesmanID=@nSalesmanID", DetailParams, connection));
                    // }







                    DetailSql = "SP_InvSalesOrderDtls_Disp @nCompanyID,@nSOrderID,@nFnYearID,1,@nLocationID";
                    SortedList NewParams = new SortedList();
                    NewParams.Add("@nLocationID", nLocationID);
                    NewParams.Add("@nFnYearID", nFnYearID);
                    NewParams.Add("@nCompanyID", nCompanyID);
                    NewParams.Add("@nSOrderID", N_SOrderID);
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, NewParams, connection);
                    DetailTable = _api.Format(DetailTable, "Details");

                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_CustomerName", typeof(string), "");
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_PhoneNo1", typeof(string), "");
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "customer_PONo", typeof(string), "");
                    DetailTable = myFunctions.AddNewColumnToDataTable(DetailTable, "X_UpdatedSPrice", typeof(string), "");
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "b_taskFlag", typeof(bool), true);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_CustomerName_Ar", typeof(string), "");
                   // MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_", typeof(string), "");
                    if(nFormID==1546){
                      if (DetailTable.Rows.Count > 0){
                            foreach (DataRow var in DetailTable.Rows)
                        {
                          
                        if(myFunctions.getIntVAL(var["N_StatusID"].ToString()) != 5)
                            MasterTable.Rows[0]["b_taskFlag"]=false;    
                         }

                            }
                    
                   MasterTable.AcceptChanges();
                    }
              
                    if (DetailTable.Rows.Count != 0)
                    {
                        MasterTable.Rows[0]["x_CustomerName"]= DetailTable.Rows[0]["x_CustomerName"];
                        MasterTable.Rows[0]["x_PhoneNo1"]= DetailTable.Rows[0]["x_PhoneNo1"];
                        MasterTable.Rows[0]["x_CustomerName_Ar"]=DetailTable.Rows[0]["x_CustomerName_Ar"];
                        MasterTable.Rows[0]["customer_PONo"] = DetailTable.Rows[0]["customer_PONo"];
                    }
                    SortedList Param = new SortedList();
                    Param.Add("@nCompanyID", nCompanyID);
                    Param.Add("@nSPriceTypeID", "");
                    foreach (DataRow var in DetailTable.Rows)
                    {
                        if (var["N_SPriceTypeID"].ToString() != "")
                        {
                            Params["@nSPriceTypeID"] = var["N_SPriceTypeID"].ToString();
                            var["X_UpdatedSPrice"] = Convert.ToString(dLayer.ExecuteScalar("select X_Name from Gen_LookupTable where N_CompanyID=@nCompanyID and N_ReferId=3 and N_PkeyId=@nSPriceTypeID", Param, connection));
                        }
                    }

                    DetailTable = dLayer.ExecuteDataTable(DetailSql, NewParams, connection);
                    DetailTable = _api.Format(DetailTable, "Details");

                    Object CTSCount=dLayer.ExecuteScalar("select count(*) from Inv_ServiceTimesheet where N_SOID="+N_SOrderID+" and N_CompanyID="+nCompanyID, Params, connection);
                    int nCTSCount = myFunctions.getIntVAL(CTSCount.ToString());
                    if (nCTSCount > 0)
                    {
                       
                        if (MasterTable.Rows.Count > 0)
                        {
                            MasterTable.Columns.Add("b_CTSProcessed");
                            MasterTable.Rows[0]["b_CTSProcessed"]=true;
                            
                          
                        }


                    }     


                  
                    DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(MasterTable.Rows[0]["N_CustomerID"].ToString()), myFunctions.getIntVAL(MasterTable.Rows[0]["N_SalesOrderId"].ToString()), this.FormID, myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString()), User, connection);
                    Attachments = _api.Format(Attachments, "attachments");

                    string TermsSql = "select * from vw_Termsdetails Where N_CompanyID=@nCompanyID and N_ReferanceID=" + N_SOrderID + " and X_Type='SO'";
                    DataTable Terms = dLayer.ExecuteDataTable(TermsSql, Params, connection);
                    foreach (DataRow dr in Terms.Rows)
                        {
                            object TermsSales = dLayer.ExecuteScalar("select N_TermsID from Inv_salesdetails where N_CompanyID=@nCompanyID and n_salesorderid="+N_SOrderID+ " and N_TermsID="+dr["N_TermsID"].ToString(), Params, connection);
                            object TermsDebit = dLayer.ExecuteScalar("select N_TermsID from Inv_BalanceAdjustmentMasterDetails where N_CompanyID=@nCompanyID and N_TermsID="+dr["N_TermsID"].ToString(), Params, connection);
                            if(TermsSales!=null)
                            {
                               if(myFunctions.getIntVAL(TermsSales.ToString())>0) 
                               {
                                 dr["N_Paidamt"] = myFunctions.getVAL(dr["N_Amount"].ToString());


                        object termamount=  dLayer.ExecuteScalar("select SUM(N_Amount) from Inv_Terms where N_CompanyID=@nCompanyID and N_ReferanceId=@nSOrderID and N_TypeID=451", DetailParams, connection);
                        object orderamount= dLayer.ExecuteScalar("select SUM(N_TaxAmt + N_BillAmt)  from Inv_Sales where  N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);
                        object retamount=  dLayer.ExecuteScalar("select N_Amount from Inv_Terms where N_CompanyID=@nCompanyID and N_ReferanceId=@nSOrderID and N_TypeID=468", DetailParams, connection);
                        
                      if (myFunctions.getVAL(termamount.ToString()) != myFunctions.getVAL(orderamount.ToString())+myFunctions.getVAL(retamount.ToString()))
                        {
                            MasterTable.Rows[0]["salesDone"] = 0;
                        }

                               }
                            }
                            if(TermsDebit!=null)
                            {
                               if(myFunctions.getIntVAL(TermsDebit.ToString())>0) 
                               {
                                 dr["N_Paidamt"] = myFunctions.getVAL(dr["N_Amount"].ToString());
                                 MasterTable.Rows[0]["debitNoteDone"] = true;
                               }
                            }

                        }
                       
                       
                       
                       


                    Terms = _api.Format(Terms, "Terms");

                    string RentalScheduleSql = "SELECT * FROM  vw_RentalScheduleItems  Where N_CompanyID=@nCompanyID and N_TransID=" + N_SOrderID +crieteria;
                    DataTable RentalSchedule = dLayer.ExecuteDataTable(RentalScheduleSql, Params, connection);
                    RentalSchedule = _api.Format(RentalSchedule, "RentalSchedule");


                     //EYE OPTICS
                    string prescriptionSql = "select * from Inv_Prescription where N_CustomerID="+myFunctions.getIntVAL(MasterTable.Rows[0]["N_CustomerID"].ToString())+" and N_SalesOrderID="+N_SOrderID+" and N_CompanyID=@nCompanyID";
                    Prescription = dLayer.ExecuteDataTable(prescriptionSql, Params, connection);
                      Prescription = _api.Format(Prescription, "Prescription");

                    dt.Tables.Add(Attachments);
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    dt.Tables.Add(Terms);
                    dt.Tables.Add(RentalSchedule);
                    dt.Tables.Add(Prescription);
                }
                return Ok(_api.Success(dt));
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataTable Attachment = ds.Tables["attachments"];
                    DataTable Prescription = ds.Tables["prescription"];
                    DataTable Terms = ds.Tables["terms"];
                    DataTable rentalItem = ds.Tables["segmentTable"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    SortedList Result = new SortedList();
                    SortedList QueryParams = new SortedList();
                    String xButtonAction="";
                    DataTable Approvals;
                    Approvals = ds.Tables["approval"];
                    DataRow ApprovalRow = Approvals.Rows[0];
                    DataTable CustomerInfo;

                    int n_SalesOrderId = myFunctions.getIntVAL(MasterRow["n_SalesOrderId"].ToString());
                    int n_SOId = myFunctions.getIntVAL(MasterRow["n_SalesOrderId"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int N_BranchID = myFunctions.getIntVAL(MasterRow["n_BranchID"].ToString());
                    int N_LocationID = myFunctions.getIntVAL(MasterRow["n_LocationID"].ToString());
                    int N_QuotationID = myFunctions.getIntVAL(MasterRow["n_QuotationID"].ToString());
                    string x_OrderNo = MasterRow["x_OrderNo"].ToString();
                    string isAuto = MasterRow["x_OrderNo"].ToString();
                    int N_CustomerId = myFunctions.getIntVAL(MasterRow["n_CustomerId"].ToString());
                    bool B_IsService = true;
                    int N_FormID = 0;
                    int N_NextApproverID = 0;
                    int N_SaveDraft = myFunctions.getIntVAL(MasterRow["b_IsSaveDraft"].ToString());
                    int nDivisionID = 0;
                    if (MasterTable.Columns.Contains("n_DivisionID"))
                    {
                       nDivisionID=myFunctions.getIntVAL(MasterRow["n_DivisionID"].ToString());
                    }
                    int n_OpportunityID = 0;
                    if (MasterTable.Columns.Contains("n_OpportunityID"))
                    {
                       n_OpportunityID=myFunctions.getIntVAL(MasterRow["n_OpportunityID"].ToString());
                    }

                    CustomerInfo = dLayer.ExecuteDataTable("Select X_CustomerCode,X_CustomerName from Inv_Customer where N_CustomerID="+ N_CustomerId, QueryParams, connection, transaction);
                    if (MasterTable.Columns.Contains("N_FormID"))
                    {
                        N_FormID = myFunctions.getIntVAL(MasterRow["N_FormID"].ToString());
                    }
                   if (!myFunctions.CheckActiveYearTransaction(N_CompanyID, N_FnYearID, DateTime.ParseExact(MasterTable.Rows[0]["D_OrderDate"].ToString(), "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture), dLayer, connection, transaction))
                  {
                   object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID="+N_CompanyID+" and convert(date ,'" + MasterTable.Rows[0]["D_OrderDate"].ToString() + "') between D_Start and D_End", connection, transaction);
                  if (DiffFnYearID != null)
                   {
                    MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
                    N_FnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());

                    QueryParams["@nFnYearID"] = N_FnYearID;
                    QueryParams["@N_CustomerId"] = N_CustomerId;
                    QueryParams["@N_CompanyID"] = N_CompanyID;


                    SortedList PostingParam = new SortedList();
                    PostingParam.Add("N_PartyID", N_CustomerId);
                    PostingParam.Add("N_FnyearID", N_FnYearID);
                    PostingParam.Add("N_CompanyID", N_CompanyID);
                    PostingParam.Add("X_Type", "customer");


                     object custCount = dLayer.ExecuteScalar("Select count(*) From Inv_Customer where N_FnYearID=@nFnYearID and N_CompanyID=@N_CompanyID and N_CustomerID=@N_CustomerId", QueryParams, connection, transaction);
                      
                      if(myFunctions.getIntVAL(custCount.ToString())==0){
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
                    // transaction.Rollback();
                    // return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
                    // Result.Add("b_IsCompleted", 0);
                    // Result.Add("x_Msg", "Transaction date must be in the active Financial Year.");
                      return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
                   }
                 }
                 MasterTable.AcceptChanges();
                object B_YearEndProcess=dLayer.ExecuteScalar("Select B_YearEndProcess from Acc_FnYear Where N_CompanyID="+N_CompanyID+" and convert(date ,'" + MasterTable.Rows[0]["D_OrderDate"].ToString() + "') between D_Start and D_End", connection, transaction);
                if(myFunctions.getBoolVAL(B_YearEndProcess.ToString()))
                {
                     return Ok(_api.Error(User, "Year Closed"));
                }

                if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()) && n_SalesOrderId > 0)
                {
                    int N_PkeyID = n_SalesOrderId;
                    string X_Criteria = "N_SalesOrderId=" + N_PkeyID + " and N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID;
                    myFunctions.UpdateApproverEntry(Approvals, "Inv_SalesOrder", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                    N_NextApproverID = myFunctions.LogApprovals(Approvals, N_FnYearID, "BOOK ORDER", N_PkeyID, x_OrderNo, 1, CustomerInfo.Rows[0]["X_CustomerName"].ToString().Trim(), 0, "", 0, User, dLayer, connection, transaction);

                    if (CustomerInfo.Rows.Count > 0)
                    {
                        try
                        {
                            myAttachments.SaveAttachment(dLayer, Attachment, x_OrderNo, n_SalesOrderId, CustomerInfo.Rows[0]["X_CustomerName"].ToString().Trim(), CustomerInfo.Rows[0]["X_CustomerCode"].ToString(), N_CustomerId, "Customer Document", User, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }
                    }

                    N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_SalesOrder where N_SalesOrderId=" + N_PkeyID + " and N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID, connection, transaction).ToString());

                    if (N_FormID == 1757)
                    {
                        return Ok(_api.Success(Result, "Book Order Approved " + "-" + x_OrderNo));
                    }
                }

                    if (x_OrderNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", N_FormID);
                        Params.Add("N_BranchID", N_BranchID);
                        if(nDivisionID!=0)
                            Params.Add("N_DivisionID", nDivisionID);
                        x_OrderNo = dLayer.GetAutoNumber("Inv_SalesOrder", "X_OrderNo", Params, connection, transaction);
                        xButtonAction="Insert"; 
                        if (x_OrderNo == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Sales Order Number");
                        }
                        MasterTable.Rows[0]["X_OrderNo"] = x_OrderNo;
                    }
                    x_OrderNo = MasterTable.Rows[0]["X_OrderNo"].ToString();

                    MasterTable.Rows[0]["N_UserID"] = myFunctions.GetUserID(User);
                    
                    if (n_SalesOrderId > 0)
                    {
                        try
                        {
                            dLayer.ExecuteScalar("SP_Delete_Trans_With_Accounts " + N_CompanyID + ",'Sales Order'," + n_SalesOrderId.ToString(), connection, transaction);
                            dLayer.ExecuteScalar("delete from Inv_DeliveryDispatch where N_SOrderID=" + n_SalesOrderId.ToString() + " and N_CompanyID=" + N_CompanyID, connection, transaction);
                            
                            
                            xButtonAction="Update"; 
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }
                    }

                    string DupCriteria = "N_CompanyID=" + N_CompanyID + " and X_OrderNo='" + x_OrderNo + "' and N_FnYearID=" + N_FnYearID + "";

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        object objService = dLayer.ExecuteScalar("select n_classid from inv_itemmaster where N_CompanyID=" + N_CompanyID + " and N_ItemID=" + DetailTable.Rows[j]["n_ItemId"], connection, transaction);
                        if (objService.ToString() != "4")
                            B_IsService = false;
                    }
                    DataColumnCollection columns = MasterTable.Columns;
                    if (columns.Contains("b_IsService"))
                    {
                        MasterTable.Rows[0]["b_IsService"] = B_IsService;
                    }
                    //MasterTable.Columns.Add("b_IsService", typeof(bool)); 

                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);
                    

                    n_SalesOrderId = dLayer.SaveData("Inv_SalesOrder", "N_SalesOrderID", DupCriteria, "", MasterTable, connection, transaction);
                    if (n_SalesOrderId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save!.. Order No Already Exists"));
                    }
                    else
                    {
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, N_FnYearID, "BOOK ORDER", n_SalesOrderId, x_OrderNo, 1, CustomerInfo.Rows[0]["X_CustomerName"].ToString().Trim(), 0, "",0, User, dLayer, connection, transaction);
                        N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_SalesOrder where N_SalesOrderId=" + n_SalesOrderId + " and N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID, connection, transaction).ToString());
                    }



                   if(nDivisionID>0)
                    {
                    if (DetailTable.Rows.Count > 0)
                    {
                     object xLevelsql = dLayer.ExecuteScalar("select X_LevelPattern from Inv_DivisionMaster where N_CompanyID=" + N_CompanyID + " and N_DivisionID=" + nDivisionID + " and N_GroupID=0", Params, connection,transaction);
                      
                       if (xLevelsql != null && xLevelsql.ToString() != "")
                        {
                        for (int j = 0; j < DetailTable.Rows.Count; j++)
                        {

                            //  detailTable.Rows[j]["N_SalesId"] = N_SalesID;
                            object xLevelPattern = dLayer.ExecuteScalar("SELECT  Inv_DivisionMaster.X_LevelPattern FROM  Inv_DivisionMaster LEFT OUTER JOIN    Inv_ItemCategory ON Inv_DivisionMaster.N_DivisionID = Inv_ItemCategory.N_DivisionID AND Inv_DivisionMaster.N_CompanyID = Inv_ItemCategory.N_CompanyID RIGHT OUTER JOIN "+
                            "Inv_ItemMaster ON Inv_ItemCategory.N_CompanyID = Inv_ItemMaster.N_CompanyID AND Inv_ItemCategory.N_CategoryID = Inv_ItemMaster.N_CategoryID  where Inv_ItemMaster.N_ItemID="+ DetailTable.Rows[j]["N_ItemID"]+" and Inv_ItemMaster.N_CompanyID="+N_CompanyID+" ", Params, connection,transaction);
                           // object xLevelPattern = dLayer.ExecuteScalar("select X_LevelPattern from Acc_CostCentreMaster where N_CompanyID=" + N_CompanyID + " and N_CostCentreID=" + nDivisionID + " and N_GroupID=0", Params, connection);
                             if (xLevelsql.ToString() != xLevelPattern.ToString().Substring(0, 3))
                             {
                                 Result.Add("b_IsCompleted", 0);
                                // Result.Add("x_Msg", "Unable To save!...Division Mismatch");
                                return Ok(_api.Error(User, "Unable To save!...Division Mismatch"));
                                //  return Result;
                             }
                           
                        }
                        }
                    
                    }
                    }
                    if (n_OpportunityID > 0)
                        dLayer.ExecuteNonQuery("Update CRM_Opportunity Set  N_ClosingStatusID=1 Where n_OpportunityID=" + n_OpportunityID + " and N_CompanyID=" + N_CompanyID.ToString(), connection, transaction);


                    if (N_QuotationID > 0)
                        dLayer.ExecuteNonQuery("Update Inv_SalesQuotation Set  N_Processed=1, N_StatusID=1 Where N_QuotationID=" + N_QuotationID + " and N_FnYearID=" + N_FnYearID + " and N_CompanyID=" + N_CompanyID.ToString(), connection, transaction);

                    // for (int j = 0; j < DetailTable.Rows.Count; j++)
                    // {
                    //     DetailTable.Rows[j]["n_SalesOrderId"] = n_SalesOrderId;

                    // }

                    // int N_QuotationDetailId = dLayer.SaveData("Inv_SalesOrderDetails", "N_SalesOrderDetailsID", DetailTable, connection, transaction);

                    int N_QuotationDetailId = 0;
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_SalesOrderId"] = n_SalesOrderId;

                        N_QuotationDetailId = dLayer.SaveDataWithIndex("Inv_SalesOrderDetails", "N_SalesOrderDetailsID", "", "", j, DetailTable, connection, transaction);


                        if (N_QuotationDetailId > 0)
                        {
                            for (int k = 0; k < rentalItem.Rows.Count; k++)
                            {

                                if (myFunctions.getIntVAL(rentalItem.Rows[k]["rowID"].ToString()) == j)
                                {

                                    rentalItem.Rows[k]["n_TransID"] = n_SalesOrderId;
                                    rentalItem.Rows[k]["n_TransDetailsID"] = N_QuotationDetailId;


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
                        if (n_SOId > 0)
                        {
                            int FormID = myFunctions.getIntVAL(rentalItem.Rows[0]["n_FormID"].ToString());
                            dLayer.ExecuteScalar("delete from Inv_RentalSchedule where N_TransID=" + n_SalesOrderId.ToString() + "  and N_FormID="+ FormID + " and N_CompanyID=" + N_CompanyID, connection, transaction);
                    
                        }
                        dLayer.SaveData("Inv_RentalSchedule", "N_ScheduleID", rentalItem, connection, transaction);

                    }
                    //FOR EYE OPTICS 
                    if (n_SOId > 0)
                        {
                            object presID=dLayer.ExecuteScalar("select isnull(N_PrescriptionID,0) from Inv_Prescription where N_SalesOrderID=" + n_SOId.ToString() + " and N_CompanyID=" + N_CompanyID+"",connection, transaction);
                            if(presID==null){presID=0;}
                            int nPrescriptionID=myFunctions.getIntVAL(presID.ToString());
                            dLayer.ExecuteScalar("delete from Inv_Prescription where N_SalesOrderID=" + n_SOId.ToString() + " and N_CompanyID=" + N_CompanyID, connection, transaction);
                            if(Prescription.Rows.Count>0)
                            {
                                if(nPrescriptionID>0)
                                    Prescription.Rows[0]["N_PrescriptionID"]=nPrescriptionID;
                            }
                            Prescription.AcceptChanges();
                    
                        }
                    if(Prescription.Rows.Count >0 )
                    { 
                        if (Prescription.Columns.Contains("N_SalesOrderID"))
                        {
                        Prescription.Rows[0]["N_SalesOrderID"]=n_SalesOrderId;
                        Prescription.AcceptChanges();
                        dLayer.SaveData("Inv_Prescription", "N_PrescriptionID", Prescription, connection, transaction); 
                        }

                    }
                

                    if (N_QuotationDetailId <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save sales order");
                    }
                    else
                    {
                        if (N_FormID == 1546)
                        {
                            int N_AssigneeID = 0;
                            int N_CreatorID = 0;

                            string X_TaskSummary = "";
                            string X_TaskSummarySql = "";
                            string X_TaskDescription = "";
                            string X_TaskDescriptionSql = "";

                            int N_SubmitterID = 0;
                            int N_ClosedUserID = 0;
                            DateTime D_DueDate;
                            DateTime D_StartDate;
                            DateTime D_EntryDate;
                            int N_Status = 0;
                            string assigneeSql = "";
                            string creatorstring = "";
                            string dueDateSql = "";
                            int salesOrderDetailsID = 0;
                            bool Status = false;
                            string taskCountsql = "";
                            object taskCount;
                            string startDateSql="";
                            string priority="";
                            string category="";
                            int priorityID=0;
                            int nCategoryID=0;



                            DataTable Details = dLayer.ExecuteDataTable("select * from Inv_SalesOrderDetails where N_CompanyID=" + N_CompanyID + " and N_SalesOrderID=" + n_SalesOrderId + "", Params, connection, transaction);
                            foreach (DataRow var in Details.Rows)
                            {
                                if (myFunctions.getIntVAL(var["N_ServiceID"].ToString()) == 0) { continue; }
                                taskCountsql = "select count(1) from Tsk_TaskMaster  where N_CompanyID=" + N_CompanyID + " and N_ServiceDetailsID=" + myFunctions.getIntVAL(var["N_SalesOrderDetailsID"].ToString()) + "";
                                taskCount = dLayer.ExecuteScalar(taskCountsql, Params, connection, transaction);
                                if (myFunctions.getIntVAL(taskCount.ToString()) > 0)
                                {
                                    continue;
                                }
                                assigneeSql = "select N_AssignedTo from Inv_ServiceInfo where N_CompanyID=" + N_CompanyID + " and N_ServiceInfoID =" + myFunctions.getIntVAL(var["N_ServiceID"].ToString()) + "";
                                creatorstring = "select N_UserID from Inv_ServiceInfo where N_CompanyID=" + N_CompanyID + " and N_ServiceInfoID =" + myFunctions.getIntVAL(var["N_ServiceID"].ToString()) + "";
                                X_TaskDescriptionSql = "select X_ServiceDescription from Inv_ServiceInfo where N_CompanyID=" + N_CompanyID + " and N_ServiceInfoID =" + myFunctions.getIntVAL(var["N_ServiceID"].ToString()) + "";
                                X_TaskSummarySql = "select X_ServiceItem from Inv_ServiceInfo where N_CompanyID=" + N_CompanyID + " and N_ServiceInfoID =" + myFunctions.getIntVAL(var["N_ServiceID"].ToString()) + "";
                                dueDateSql = "select D_DeliveryDate from Inv_ServiceInfo where N_CompanyID=" + N_CompanyID + " and N_ServiceInfoID =" + myFunctions.getIntVAL(var["N_ServiceID"].ToString()) + "";
                                startDateSql="select D_StartDate from Inv_ServiceInfo where N_CompanyID=" + N_CompanyID + " and N_ServiceInfoID =" + myFunctions.getIntVAL(var["N_ServiceID"].ToString()) + "";
                                priority = "select N_PriorityID from Inv_ServiceInfo where N_CompanyID=" + N_CompanyID + " and N_ServiceInfoID =" + myFunctions.getIntVAL(var["N_ServiceID"].ToString()) + "";
                                category = "select N_CategoryID from Inv_ServiceInfo where N_CompanyID=" + N_CompanyID + " and N_ServiceInfoID =" + myFunctions.getIntVAL(var["N_ServiceID"].ToString()) + "";
                                N_AssigneeID = myFunctions.getIntVAL(dLayer.ExecuteScalar(assigneeSql, Params, connection, transaction).ToString());
                                if (N_AssigneeID <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok("Select an Assignee for Service");
                                }
                                N_CreatorID = myFunctions.getIntVAL(dLayer.ExecuteScalar(creatorstring, Params, connection, transaction).ToString());
                                N_ClosedUserID = myFunctions.getIntVAL(dLayer.ExecuteScalar(creatorstring, Params, connection, transaction).ToString());
                                N_SubmitterID = myFunctions.getIntVAL(dLayer.ExecuteScalar(creatorstring, Params, connection, transaction).ToString());
                                X_TaskDescription = dLayer.ExecuteScalar(X_TaskDescriptionSql, Params, connection, transaction).ToString();
                                X_TaskSummary = dLayer.ExecuteScalar(X_TaskSummarySql, Params, connection, transaction).ToString();
                                
                                if(X_TaskSummary==""){
                                string itemName="select X_Itemname from Inv_ItemMaster where N_ItemID=" + myFunctions.getIntVAL(var["N_ItemID"].ToString()) + " and N_CompanyID=" + N_CompanyID+"";
                                X_TaskSummary = dLayer.ExecuteScalar(itemName, Params, connection, transaction).ToString();

                                 dLayer.ExecuteNonQuery("Update Inv_ServiceInfo Set  X_ServiceItem='" + X_TaskSummary + "' Where N_ServiceInfoID =" + myFunctions.getIntVAL(var["N_ServiceID"].ToString()) + " and N_CompanyID=" + N_CompanyID+"", connection, transaction);

                                }
                                D_DueDate = Convert.ToDateTime(dLayer.ExecuteScalar(dueDateSql, Params, connection, transaction).ToString());
                                D_StartDate = Convert.ToDateTime(dLayer.ExecuteScalar(startDateSql, Params, connection, transaction).ToString());
                                D_EntryDate = Convert.ToDateTime(MasterTable.Rows[0]["D_EntryDate"].ToString());
                                salesOrderDetailsID = myFunctions.getIntVAL(var["N_SalesOrderDetailsID"].ToString());
                                int N_ProjectID= myFunctions.getIntVAL(MasterTable.Rows[0]["N_ProjectID"].ToString());
                                priorityID = myFunctions.getIntVAL(dLayer.ExecuteScalar(priority, Params, connection, transaction).ToString());
                                nCategoryID = myFunctions.getIntVAL(dLayer.ExecuteScalar(category, Params, connection, transaction).ToString());
                                N_Status = 2;
                                Status = taskController.SaveGeneralTask(N_CompanyID, X_TaskSummary, X_TaskDescription, N_AssigneeID, N_CreatorID, N_SubmitterID, N_ClosedUserID, D_DueDate, D_StartDate, D_EntryDate, N_Status, salesOrderDetailsID, N_ProjectID,priorityID,nCategoryID, connection, transaction);
                                if (Status == false)
                                {
                                    transaction.Rollback();
                                    return Ok("Unable to save sales order");
                                }
                            }
                        }
                        
                        //StatusUpdate
                        int tempQuotationID = 0;
                        for (int j = 0; j < DetailTable.Rows.Count; j++)
                        {
                            if (N_QuotationID > 0 && N_QuotationID != tempQuotationID)
                            {
                                if (!myFunctions.UpdateTxnStatus(N_CompanyID, N_QuotationID, 80, false, dLayer, connection, transaction)||
                                    !myFunctions.UpdateTxnStatus(N_CompanyID, N_QuotationID, N_FormID, false, dLayer, connection, transaction))
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                }
                            }
                            tempQuotationID = N_QuotationID;
                        };

                          // Activity Log
                         string ipAddress = "";
                        if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                         ipAddress = Request.Headers["X-Forwarded-For"];
                         else
                       ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(N_FnYearID,n_SalesOrderId,x_OrderNo,81,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                          

                        SortedList CustomerParams = new SortedList();
                        CustomerParams.Add("@nCustomerID", N_CustomerId);
                        
                        if (CustomerInfo.Rows.Count > 0)
                        {
                            try
                            {
                                myAttachments.SaveAttachment(dLayer, Attachment, x_OrderNo, n_SalesOrderId, CustomerInfo.Rows[0]["X_CustomerName"].ToString().Trim(), CustomerInfo.Rows[0]["X_CustomerCode"].ToString(), N_CustomerId, "Customer Document", User, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, ex));
                            }
                        }

                    }
                    if (Terms.Rows.Count > 0)
                    {
                        object ItemID = dLayer.ExecuteScalar("Select N_ItemID from Inv_ItemMaster where N_CompanyID=" + N_CompanyID + " and X_ItemCode='005'", connection, transaction);
                        object TaxCatID = dLayer.ExecuteScalar("select n_pkeyid from Acc_TaxCategory where X_CategoryName='Exempt' and N_CompanyID="+N_CompanyID, connection,transaction);
                        Terms.Columns.Add("N_TaxCategoryID");
                        for (int j = 0; j < Terms.Rows.Count; j++)
                        {
                            if(Terms.Rows[j]["x_Type"].ToString()=="Retention" || Terms.Rows[j]["x_Type"].ToString()=="Retention Reversal")
                            {
                                Terms.Rows[j]["n_ItemID"] = ItemID;
                            }
                            Terms.Rows[j]["n_ReferanceID"] = n_SalesOrderId;
                            Terms.Rows[j]["x_Type"] = "SO";
                            Terms.Rows[j]["N_TaxCategoryID"] = TaxCatID;


                        }
                        dLayer.DeleteData("Inv_Terms", "N_ReferanceID", n_SalesOrderId, "", connection, transaction);
                        dLayer.SaveData("Inv_Terms", "N_TermsID", Terms, connection, transaction);


                    }



                    transaction.Commit();
                    // SortedList Result = new SortedList();
                    Result.Add("n_SalesOrderID", n_SalesOrderId);
                    Result.Add("x_SalesOrderNo", x_OrderNo);


                                   
                 if (N_FormID == 81)
                 {
                   return Ok(_api.Success(Result, "Sales Order Saved"));
                 }
                 else if(N_FormID == 1571) 
                 {
                 return Ok(_api.Success(Result,"Job Order Saved Successfully"));
                  }
                else if(N_FormID == 1740) 
                 {
                return Ok(_api.Success(Result,"Optical Order Saved Successfully")); 
                  }
                else if(N_FormID == 1546) 
                 {
                 return Ok(_api.Success(Result,"Service Order Saved Successfully"));
                  }

                     return Ok(_api.Success(Result, "Sales Order Saved"));
                  



                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
        //Delete....
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nSalesOrderID, int nBranchID, int nFnYearID, int nFormID, string comments)
        {
            int Results = 0;
            if (comments == null)
            {
                comments = "";
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nTransID", nSalesOrderID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,N_CustomerId,x_OrderNo from Inv_SalesOrder where N_SalesOrderId=@nTransID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID";
                    string xButtonAction="Delete";
                    String x_OrderNo="";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    int N_CustomerId = myFunctions.getIntVAL(TransRow["N_CustomerId"].ToString());

                    string SqlQtnID = "select N_QuotationID from Inv_SalesOrder where N_SalesOrderId=@nTransID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID";
                    object QtnID = dLayer.ExecuteScalar(SqlQtnID, ParamList, connection);
                    int SQID = 0;
                    if (QtnID != null)
                        SQID = myFunctions.getIntVAL(QtnID.ToString());

                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, nFormID, nSalesOrderID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID, 0, 0, User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), comments);

                    SqlTransaction transaction = connection.BeginTransaction();
                    var xUserCategory = myFunctions.GetUserCategory(User);// User.FindFirst(ClaimTypes.GroupSid)?.Value;
                    var nUserID = myFunctions.GetUserID(User);// User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    string X_Criteria = "N_SalesOrderId=" + nSalesOrderID + " and N_CompanyID=" + myFunctions.GetCompanyID(User);
                    string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    string transType="SALES ORDER";
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());
                    int N_IsApprovalSystem = Approvals.Rows.Count > 0 ? myFunctions.getIntVAL(Approvals.Rows[0]["isApprovalSystem"].ToString()) : 0;
                     if(nFormID==1757)
                      transType="BOOK ORDER";
                       if(nFormID==81)
                      transType="SALES ORDER"; 
                      if(nFormID==1740)
                      transType="OPTICAL ORDER";
                      if(nFormID==1571)
                      transType="JOB ORDER";


                    string status = myFunctions.UpdateApprovals(Approvals, nFnYearID, transType, nSalesOrderID, TransRow["X_OrderNo"].ToString(), ProcStatus, "Inv_SalesOrder", X_Criteria, "", User, dLayer, connection, transaction);
                        
                    if (status != "Error")
                    {
                        object objProcessed = dLayer.ExecuteScalar("Select Isnull(N_SalesID,0) from Inv_SalesDetails where N_CompanyID=" + nCompanyID + " and N_SalesOrderId=" + nSalesOrderID + "", connection, transaction);
                        if (objProcessed == null) objProcessed = 0;

                        object objDelProcessed = dLayer.ExecuteScalar("Select Isnull(N_DeliveryNoteID,0) from Inv_DeliveryNoteDetails where N_CompanyID=" + nCompanyID + " and N_SalesOrderId=" + nSalesOrderID + "", connection, transaction);
                        if (objDelProcessed == null) objDelProcessed = 0;
                        if (myFunctions.getIntVAL(objDelProcessed.ToString()) > 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Delivery Note processed! Unable to delete Sales Order"));
                        }

                        object objtskProcessed = dLayer.ExecuteScalar("Select count(*) from Tsk_TaskMaster where N_CompanyID=" + nCompanyID + " and N_ServiceDetailsID in (select N_SalesOrderDetailsID from  Inv_SalesOrderDetails where N_CompanyId=" + nCompanyID + "  and N_SalesOrderid="+nSalesOrderID+")", connection, transaction);
                        if (objtskProcessed == null) objtskProcessed = 0;
                        if (ButtonTag == "6" || ButtonTag == "0")
                        {
                            if(myFunctions.getIntVAL(objtskProcessed.ToString()) > 0)
                            {
                                dLayer.ExecuteScalar("delete from Tsk_TaskStatus where  N_CompanyID=" + nCompanyID + " and  N_TaskID in (select N_TaskID from Tsk_TaskMaster where N_CompanyId=" + nCompanyID + " and N_ServiceDetailsID in (select N_SalesOrderDetailsID from  Inv_SalesOrderDetails where N_CompanyId=" + nCompanyID + "  and N_SalesOrderid="+nSalesOrderID+"))", connection, transaction);
                                dLayer.ExecuteScalar("delete from Tsk_TaskComments where  N_ActionID in (select N_TaskID from Tsk_TaskMaster where N_CompanyId=" + nCompanyID + " and N_ServiceDetailsID in (select N_SalesOrderDetailsID from  Inv_SalesOrderDetails where N_CompanyId=" + nCompanyID + "  and N_SalesOrderid="+nSalesOrderID+"))", connection, transaction);
                                dLayer.ExecuteScalar("delete from Tsk_TaskMaster where  N_CompanyID=" + nCompanyID + " and N_ServiceDetailsID in (select N_SalesOrderDetailsID from  Inv_SalesOrderDetails where N_CompanyId=" + nCompanyID + "  and N_SalesOrderid="+nSalesOrderID+")", connection, transaction);
                               
                                
                            }

                            // Activity Log
                            string ipAddress = "";
                            if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                                ipAddress = Request.Headers["X-Forwarded-For"];
                            else
                                ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                            myFunctions.LogScreenActivitys(myFunctions.getIntVAL( nFnYearID.ToString()),nSalesOrderID,TransRow["x_OrderNo"].ToString(),81,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);

                            if (myFunctions.getIntVAL(objProcessed.ToString()) == 0)
                            {
                                SortedList DeleteParams = new SortedList(){
                                    {"N_CompanyID",nCompanyID},
                                    {"X_TransType","SALES ORDER"},
                                    {"N_VoucherID",nSalesOrderID},
                                    {"N_UserID",nUserID},
                                    {"X_SystemName","WebRequest"},
                                    {"N_BranchID",nBranchID}};
                                Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                                if (Results <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Unable to delete Sales Order"));
                                }
                                else
                                {
                                    dLayer.ExecuteScalar("delete from Inv_Prescription where N_SalesOrderID=" + nSalesOrderID.ToString() + " and N_CompanyID=" + nCompanyID, connection, transaction);
                                    dLayer.ExecuteScalar("delete from Inv_RentalSchedule where N_TransID=" + nSalesOrderID.ToString() + "  and N_FormID=1571 and N_CompanyID=" + nCompanyID, connection, transaction);

                                    myAttachments.DeleteAttachment(dLayer, 1, nSalesOrderID, N_CustomerId, nFnYearID, this.FormID, User, transaction, connection);
                                    if (SQID > 0)//Updating SQ Status
                                    {
                                        if (!myFunctions.UpdateTxnStatus(nCompanyID, SQID, 80, true, dLayer, connection, transaction))
                                        {
                                            transaction.Rollback();
                                            return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                        }
                                    }
                                    dLayer.ExecuteScalar("delete from Inv_Prescription where N_SalesOrderID=" + nSalesOrderID.ToString() + "  and  N_CompanyID=" + nCompanyID, connection, transaction);                                       
                                
                                }
                            }
                            else if(nFormID==1740)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, "Optical invoice processed! Unable to delete Optical Order"));
                            }
                            else
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, "Sales invoice processed! Unable to delete Sales Order"));
                            }
                        }

                        transaction.Commit();
                        if(nFormID==1740)
                        {
                            return Ok(_api.Success("Optical Order " + status + " Successfully")); 
                        }
                        if(nFormID==1757)
                        {
                            return Ok(_api.Success("Book Order " + status + " Successfully")); 
                        }
                        if(nFormID==1571)
                        {
                            return Ok(_api.Success("Job Order " + status + " Successfully"));
                        }
                        else
                            return Ok(_api.Success("Sales Order " + status + " Successfully"));
                                               
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete Sales Order"));
                    }

                    // connection.Open();
                    // SqlTransaction transaction = connection.BeginTransaction();
                    // Results = dLayer.DeleteData("Inv_SalesOrderDetails", "N_SalesOrderID", nSalesOrderID, "", connection, transaction);
                    // if (Results <= 0)
                    // {
                    //     transaction.Rollback();
                    //     return Ok(_api.Error(User,"Unable to delete sales order"));
                    // }
                    // else
                    // {
                    // Results = dLayer.DeleteData("Inv_SalesOrder", "N_SalesOrderID", nSalesOrderID, "", connection, transaction);

                    // }

                    // if (Results > 0)
                    // {
                    //     transaction.Commit();
                    //     return Ok(_api.Error(User,"Sales order deleted"));
                    // }
                    // else
                    // {
                    //     transaction.Rollback();
                    //     return Ok(_api.Error(User,"Unable to delete sales order"));
                    // }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }


        }




        [HttpGet("getsettings")]
        public ActionResult GetSettings(int? nCompanyID)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    bool B_SAlesmanEnabled = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "LastSPrice_InGrid", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    bool B_CustomerProjectEnabled = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "CustomerProject Enabled", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    bool X_NotesEnabled = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "Notes Enabled", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID", nCompanyID);
                    bool B_SPRiceType = false;
                    object res = dLayer.ExecuteScalar("Select Isnull(N_Value,0) from Gen_Settings where N_CompanyID=@nCompanyID and X_Group='Inventory' and X_Description='Selling Price Calculation'", Params, connection);
                    if (res != null)
                    {
                        if (myFunctions.getIntVAL(res.ToString()) == 4)
                            B_SPRiceType = true;
                    }
                    SortedList Results = new SortedList();
                    Results.Add("B_SAlesmanEnabled", B_SAlesmanEnabled);
                    Results.Add("B_CustomerProjectEnabled", B_CustomerProjectEnabled);
                    Results.Add("X_NotesEnabled", X_NotesEnabled);
                    Results.Add("B_SPRiceType", B_SPRiceType);
                    return Ok(_api.Success(Results));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        [HttpGet("getItem")]
        public ActionResult GetItem(int nCompanyID, int nLocationID, int nBranchID, string dDate, string InputVal, int nCustomerID)
        {
            string ItemCondition = "";
            object subItemPrice, subPprice, subMrp;
            string sql = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int N_DefSPriceID = 0;
                var UserCategoryID = myFunctions.GetUserCategory(User);
                N_DefSPriceID = myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "DefSPriceTypeID", "N_Value", "N_UserCategoryID", UserCategoryID.ToString(), myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection));
                int nSPriceID = N_DefSPriceID;
                DateTime dateVal = myFunctions.GetFormatedDate(dDate.ToString());
                SortedList paramList = new SortedList();
                paramList.Add("@nCompanyID", nCompanyID);
                paramList.Add("@nLocationID", nLocationID);
                paramList.Add("@nBranchID", nBranchID);
                paramList.Add("@date", myFunctions.getDateVAL(dateVal));
                paramList.Add("@nSPriceID", N_DefSPriceID);

                ItemCondition = "[Item Code] ='" + InputVal + "'";
                DataTable SubItems = new DataTable();
                DataTable LastSalesPrice = new DataTable();
                DataTable ItemDetails = new DataTable();
                // if (B_BarcodeBilling)
                //     ItemCondition = "([Item Code] ='" + InputVal + "' OR X_Barcode ='" + InputVal + "')";
                bool B_SPRiceType = false;

                object res = dLayer.ExecuteScalar("Select Isnull(N_Value,0) from Gen_Settings where N_CompanyID=@nCompanyID and X_Group='Inventory' and X_Description='Selling Price Calculation'", paramList, connection);
                if (res != null)
                {
                    if (myFunctions.getIntVAL(res.ToString()) == 4)
                        B_SPRiceType = true;
                    else
                        B_SPRiceType = false;

                }

                string X_DefSPriceType = "";

                if (B_SPRiceType)
                {
                    X_DefSPriceType = "";

                    res = dLayer.ExecuteScalar("select X_Name from Gen_LookupTable where N_PkeyId=@nDefSPriceID and N_ReferId=3 and N_CompanyID=@nCompanyID", paramList, connection);
                    if (res != null)
                        X_DefSPriceType = res.ToString();

                }

                sql = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_Stock, dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as N_StockTotal, dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit) As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nCompanyID";

                if (B_SPRiceType)
                {
                    if (nSPriceID > 0)
                    {
                        sql = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_Stock, dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as N_StockTotal ,dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit) As N_LPrice ,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nSPriceID,@nBranchID) As N_SPrice From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nCompanyID";
                    }
                    else
                        sql = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_Stock, dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as N_StockTotal, dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit) As N_LPrice ,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nDefSPriceID, @nBranchID) As N_SPrice From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nComapanyID";

                }

                ItemDetails = dLayer.ExecuteDataTable(sql, paramList, connection);

                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "LastSalesPrice", typeof(string), "0.00");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "X_DefSPriceType", typeof(string), "");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "N_DefSPriceID", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "LastPurchaseCost", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "SpriceSum", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "PpriceSum", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "Mrpsum", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "N_BaseUnitQty", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "N_SellingPrice", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "CustomerDiscount", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "N_PriceID", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "N_PriceVal", typeof(string), "0");

                if (ItemDetails.Rows.Count == 1)
                {
                    DataRow ItemDetailRow = ItemDetails.Rows[0];
                    string ItemClass = ItemDetailRow["N_ClassID"].ToString();

                    int N_ItemID = myFunctions.getIntVAL(ItemDetailRow["N_ItemID"].ToString());
                    SortedList qParam2 = new SortedList();
                    qParam2.Add("@nItemID", N_ItemID);
                    qParam2.Add("@nCompanyID", nCompanyID);


                    if (ItemClass == "1")
                    {

                        SortedList qParam3 = new SortedList();
                        qParam3.Add("@nItemID", 0);
                        qParam3.Add("@nCompanyID", nCompanyID);
                        SubItems = dLayer.ExecuteDataTable("Select *,dbo.SP_Stock(vw_invitemdetails.N_ItemID) As N_Stock from vw_invitemdetails where N_MainItemId=@nItemID and N_CompanyID=@nCompanyID order by X_Itemname", qParam3, connection);
                        double SpriceSum = 0, PpriceSum = 0, Mrpsum = 0;
                        foreach (DataRow var in SubItems.Rows)
                        {

                            if (var["N_ItemDetailsID"].ToString() != "")
                            {
                                qParam3["@nItemID"] = var["N_ItemId"].ToString();
                                subItemPrice = dLayer.ExecuteScalar("Select top 1 N_Sprice from Inv_StockMaster where N_ItemId=@nItemID and N_CompanyID=@nCompanyID order by n_stockid desc", qParam3, connection);
                                subPprice = dLayer.ExecuteScalar("Select top 1 N_Sprice from Inv_StockMaster where N_ItemId=@nItemID and N_CompanyID=@nCompanyID order by n_stockid desc", qParam3, connection);
                                subMrp = dLayer.ExecuteScalar("Select top 1 N_Mrp from Inv_PurchaseDetails where N_ItemId=@nItemID and N_CompanyID=@nCompanyID Order By N_PurchaseDetailsId desc", qParam3, connection);
                                if (subItemPrice != null) SpriceSum = myFunctions.getVAL(subItemPrice.ToString()) * myFunctions.getVAL(var["N_Qty"].ToString()) + SpriceSum;
                                if (subPprice != null) PpriceSum = myFunctions.getVAL(subPprice.ToString()) + PpriceSum;
                                if (subMrp != null) Mrpsum = myFunctions.getVAL(subMrp.ToString()) + Mrpsum;
                            }
                        }
                        ItemDetails.Rows[0]["SpriceSum"] = SpriceSum;
                        ItemDetails.Rows[0]["PpriceSum"] = PpriceSum;
                        ItemDetails.Rows[0]["Mrpsum"] = Mrpsum;
                    }







                    object objSPrice = dLayer.ExecuteScalar("Select Isnull(N_Value,0) from Gen_Settings where N_CompanyID=@nCompanyID and X_Group='Inventory' and X_Description='Selling Price Calculation'", connection);

                    string X_ItemUnit = ItemDetailRow["X_ItemUnit"].ToString();
                    qParam2.Add("@X_ItemUnit", X_ItemUnit);
                    DataTable SellingPrice = dLayer.ExecuteDataTable("Select N_Qty,N_SellingPrice from Inv_ItemUnit Where N_CompanyID=@nCompanyID and N_ItemID = @nItemID and X_ItemUnit=@X_ItemUnit", qParam2, connection);

                    if (SellingPrice.Rows.Count > 0)
                    {
                        ItemDetails.Rows[0]["N_Qty"] = SellingPrice.Rows[0]["N_Qty"].ToString();
                        ItemDetails.Rows[0]["N_SellingPrice"] = SellingPrice.Rows[0]["N_SellingPrice"].ToString();
                        if (myFunctions.getVAL(SellingPrice.Rows[0]["N_SellingPrice"].ToString()) > 0)
                        {
                            bool B_LastPurchaseCost = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings(this.FormID.ToString(), "LastPurchaseCost", "N_Value", "N_UserCategoryID", UserCategoryID.ToString(), myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                            if (B_LastPurchaseCost)
                            {
                                object LastPurchaseCost = dLayer.ExecuteScalar("Select TOP(1) ISNULL(N_LPrice,0) from Inv_StockMaster Where N_ItemID=@nItemID and N_CompanyID=@nCompanyID and  (X_Type='Purchase' or X_Type='Opening') Order by N_StockID Desc", qParam2, connection);
                                ItemDetails.Rows[0]["LastPurchaseCost"] = B_LastPurchaseCost;
                            }

                        }
                    }

                    qParam2.Add("@nCustomerID", nCustomerID);
                    object value = dLayer.ExecuteScalar("select N_DiscPerc from inv_CustomerDiscount where N_ProductID =@nItemID and N_CustomerID =@nCustomerID and N_CompanyID =@nCompanyID", qParam2, connection);
                    if (value != null)
                    {
                        ItemDetails.Rows[0]["CustomerDiscount"] = value;
                    }

                    SortedList paramList4 = new SortedList();
                    paramList4.Add("@nBranchID", nBranchID);
                    paramList4.Add("@nItemID", N_ItemID);
                    paramList4.Add("@nCompanyID", nCompanyID);
                    paramList4.Add("@xDefultSPriceID", X_DefSPriceType);

                    DataTable PriceMaster = dLayer.ExecuteDataTable("Select N_PriceID,N_PriceVal From Inv_ItemPriceMaster  Where N_CompanyID=@nCompanyID and N_BranchID=@nBranchID and N_itemId=@nItemID and N_PriceID in(Select N_PkeyId from Gen_LookupTable where X_Name=@xDefultSPriceID and N_CompanyID=@nCompanyID)", paramList4, connection);


                    if (PriceMaster.Rows.Count > 0)
                    {
                        ItemDetails.Rows[0]["SpriceSum"] = PriceMaster.Rows[0]["N_PriceID"].ToString();
                        ItemDetails.Rows[0]["PpriceSum"] = PriceMaster.Rows[0]["N_PriceVal"].ToString();
                    }


                }



                return Ok(_api.Success(_api.Format(ItemDetails)));
            }
        }



        [HttpGet("validateItemPrice")]
        public ActionResult SalesPriceValidation(int nCompanyID, int nLocationID, int nBranchID, int nItemID, int nCustomerID)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    bool B_SPRiceType = false;
                    string X_DefSPriceType = "";
                    int N_DefSPriceID = 0;
                    DataTable SalePrice = new DataTable();
                    SortedList Params = new SortedList();
                    SortedList OutPut = new SortedList();
                    Params.Add("@nCompanyID", nCompanyID);

                    object res = dLayer.ExecuteScalar("Select Isnull(N_Value,0) from Gen_Settings where N_CompanyID=@nCompanyID and X_Group='Inventory' and X_Description='Selling Price Calculation'", Params, connection);
                    if (res != null)
                    {
                        if (myFunctions.getIntVAL(res.ToString()) == 4)
                            B_SPRiceType = true;
                        else
                            B_SPRiceType = false;

                    }
                    if (B_SPRiceType)
                    {
                        X_DefSPriceType = "";
                        var UserCategoryID = User.FindFirst(ClaimTypes.GroupSid)?.Value;
                        N_DefSPriceID = myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "DefSPriceTypeID", "N_Value", "N_UserCategoryID", UserCategoryID, myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection));
                        Params.Add("@nDefSPriceID", N_DefSPriceID);
                        object res2 = dLayer.ExecuteScalar("select X_Name from Gen_LookupTable where N_PkeyId=@nDefSPriceID and N_ReferId=3 and N_CompanyID=@nCompanyID", Params, connection);
                        if (res2 != null)
                            X_DefSPriceType = res.ToString();
                        else X_DefSPriceType = "";

                    }
                    Params.Add("@nBranchID", nBranchID);
                    Params.Add("@nItemID", nItemID);
                    Params.Add("@xDefSPriceType", X_DefSPriceType);
                    SalePrice = dLayer.ExecuteDataTable("Select N_PriceID,N_PriceVal From Inv_ItemPriceMaster  Where N_CompanyID=@nCompanyID and N_BranchID=@nBranchID and N_itemId=@nItemID and N_PriceID in(Select N_PkeyId from Gen_LookupTable where X_Name=@xDefSPriceType and N_CompanyID=@nCompanyID)", Params, connection);


                    if (SalePrice.Rows.Count > 0)
                    {
                        OutPut.Add("N_PriceID", SalePrice.Rows[0]["N_PriceID"]);
                        OutPut.Add("N_PriceVal", SalePrice.Rows[0]["N_PriceVal"]);

                    }
                    else
                    {
                        OutPut.Add("N_PriceID", 0);
                        OutPut.Add("N_PriceVal", "");
                    }

                    Params.Add("@nCustomerID", nCustomerID);
                    object value = dLayer.ExecuteScalar("select N_DiscPerc from inv_CustomerDiscount where N_ProductID =@nItemID and N_CustomerID =@nCustomerID and N_CompanyID =@nCompanyID", Params, connection);
                    if (value != null)
                    {
                        OutPut.Add("N_DiscPerc", value);
                    }
                    else
                    {
                        OutPut.Add("N_DiscPerc", "");
                    }



                    return Ok(_api.Success(OutPut));
                }
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }



        [HttpGet("getItemList")]
        public ActionResult GetItemList(int nCompanyID, int nLocationID, int nBranchID, string query, int PageSize, int Page)
        {
            string sql = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int N_DefSPriceID = 0;
                var UserCategoryID = User.FindFirst(ClaimTypes.GroupSid)?.Value;
                N_DefSPriceID = myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "DefSPriceTypeID", "N_Value", "N_UserCategoryID", UserCategoryID, myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection));
                int nSPriceID = N_DefSPriceID;
                SortedList paramList = new SortedList();
                paramList.Add("@nCompanyID", nCompanyID);
                paramList.Add("@nLocationID", nLocationID);
                paramList.Add("@nBranchID", nBranchID);
                paramList.Add("@PSize", PageSize);
                paramList.Add("@Offset", Page);



                DataTable ItemDetails = new DataTable();
                string qry = "";
                if (query != "" && query != null)
                {
                    qry = " and (Description like @query or [Item Code] like @query) order by [Item Code],Description";
                    paramList.Add("@query", "%" + query + "%");
                }

                string pageQry = "DECLARE @PageSize INT, @Page INT Select @PageSize=@PSize,@Page=@Offset;WITH PageNumbers AS(Select ROW_NUMBER() OVER(ORDER BY N_ItemID) RowNo,";
                string pageQryEnd = ") SELECT * FROM    PageNumbers WHERE   RowNo BETWEEN((@Page -1) *@PageSize + 1)  AND(@Page * @PageSize)";

                bool B_SPRiceType = false;

                object res = dLayer.ExecuteScalar("Select Isnull(N_Value,0) from Gen_Settings where N_CompanyID=@nCompanyID and X_Group='Inventory' and X_Description='Selling Price Calculation'", paramList, connection);
                if (res != null)
                {
                    if (myFunctions.getIntVAL(res.ToString()) == 4)
                        B_SPRiceType = true;
                    else
                        B_SPRiceType = false;

                }

                sql = " *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_Stock, dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as N_StockTotal, dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit) As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice From vw_InvItem_Search Where  N_CompanyID=@nCompanyID ";

                if (B_SPRiceType)
                {
                    if (nSPriceID > 0)
                    {
                        sql = " *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_Stock, dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as N_StockTotal ,dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit) As N_LPrice ,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nSPriceID,@nBranchID) As N_SPrice From vw_InvItem_Search Where N_CompanyID=@nCompanyID ";
                    }
                    else
                        sql = " *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_Stock, dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as N_StockTotal, dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit) As N_LPrice ,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nDefSPriceID, @nBranchID) As N_SPrice From vw_InvItem_Search Where N_CompanyID=@nComapanyID ";

                }

                ItemDetails = dLayer.ExecuteDataTable(pageQry + sql + pageQryEnd + qry, paramList, connection);
                if (ItemDetails.Rows.Count == 0)
                {
                    return Ok(_api.Error(User, "No Items Found"));
                }
                return Ok(_api.Success(_api.Format(ItemDetails)));
            }
        }



        [HttpGet("invScheduledOrder")]
        public ActionResult GetInvScheduledOrder(int nCompanyID, int nItemID, DateTime dPeriodFrom, DateTime dPeriodTo)
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
                    Params.Add("@p4", dPeriodFrom);
                    Params.Add("@p5", dPeriodTo);

                    sqlCommandText = "select * from vw_ScheduledRentalOrders where N_CompanyID=@p1 and N_ItemID=@p2 and ((D_PeriodFrom<=@p4 and isNull(D_PeriodTo,getDate())>=@p4) OR (D_PeriodFrom<=@p5 and isNull(D_PeriodTo,getDate())>=@p5) OR(D_PeriodFrom>=@p4 and D_PeriodFrom<=@p5))";


                    SortedList OutPut = new SortedList();

                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count from vw_ScheduledRentalOrders  where N_CompanyID=@p1 and N_ItemID=@p2 and ((D_PeriodFrom<=@p4 and isNull(D_PeriodTo,getDate())>=@p4) OR (D_PeriodFrom<=@p5 and isNull(D_PeriodTo,getDate())>=@p5) OR(D_PeriodFrom>=@p4 and D_PeriodFrom<=@p5))";
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



        [HttpGet("salesOrderNo")]
        public ActionResult GetInvSalesOrderNo( int nFormID, DateTime dDateFrom, DateTime dDateTo )
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                   
                    string sqlCommandText = "";

                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nFormID);
                    Params.Add("@p3", dDateFrom);
                    Params.Add("@p4", dDateTo);

                    if (nFormID==1145)
                        sqlCommandText = "select * from vw_SalesorderDelivery where N_CompanyID=@p1 and ((D_DeliveryDate<=@p3) or (D_DeliveryDate>=@p3 AND D_DeliveryDate<=@p4)) AND ISNULL(D_DeliveryReturnDate,@p3)>=@p3 and N_SalesOrderId NOT IN (select N_SOID from Inv_ServiceTimesheet where N_FormID=@p2 and N_CompanyID=@p1 and D_DateFrom=@p3 and D_DateTo=@p4)";
                    else
                        sqlCommandText = "select * from vw_SO_PO_List where N_CompanyID=@p1";

                    SortedList OutPut = new SortedList();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    OutPut.Add("Details", _api.Format(dt));
                    return Ok(_api.Success(OutPut));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        
        [HttpGet("prescriptionDetails")]
        public ActionResult GetPrescriptionDetails(int nCompanyID,int nCustomerID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText ="select top 1 * from Inv_Prescription where N_CompanyID=@p1 and N_CustomerID=@p2 order by N_PrescriptionID desc";

            Params.Add("@p1",nCompanyID);
            Params.Add("@p2", nCustomerID);


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                return Ok(_api.Success(dt));
              
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }


    }
}