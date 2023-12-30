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
using System;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("purchaseinvoice")]
    [ApiController]
    public class Inv_PurchaseInvoice : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int N_FormID;
        private readonly ITxnHandler txnHandler;
        public Inv_PurchaseInvoice(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf, IMyAttachments myAtt, ITxnHandler txn)
        {
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 65;
            txnHandler = txn;
        }


        [HttpGet("list")]
        public ActionResult GetPurchaseInvoiceList(int? nCompanyId, int nFnYearId, bool bAllBranchData, int nBranchID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, string screen, int nFormID)
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
                    string criteria = "";
                    string X_TransType = "PURCHASE";
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int N_decimalPlace = 2;
                    //  string UserPattern = myFunctions.GetUserPattern(User);
                    int nUserID = myFunctions.GetUserID(User);
                    //  string Pattern = "";
                    N_decimalPlace = myFunctions.getIntVAL(myFunctions.ReturnSettings("Purchase", "Decimal_Place", "N_Value", nCompanyID, dLayer, connection));
                    N_decimalPlace = N_decimalPlace == 0 ? 2 : N_decimalPlace;

                    //     if (UserPattern != "")
                    //     {
                    //         Pattern = "and Left(X_Pattern,Len(@UserPattern))=@UserPattern ";
                    //         Params.Add("@UserPattern",UserPattern);
                    //     }
                    //     else{
                    //      object HierarchyCount = dLayer.ExecuteScalar("select count(N_HierarchyID) from Sec_UserHierarchy where N_CompanyID="+nCompanyId, Params, connection);

                    //    if( myFunctions.getIntVAL(HierarchyCount.ToString())>0)
                    //     Pattern = " and N_CreatedUser=" + nUserID;
                    //     }




                    bool CheckClosedYear = Convert.ToBoolean(dLayer.ExecuteScalar("Select B_YearEndProcess From Acc_FnYear Where N_CompanyID=" + nCompanyId + " and N_FnYearID = " + nFnYearId, Params, connection));
                    if (screen == "Invoice")
                        criteria = " and MONTH(Cast(InvoiceDate as DateTime)) = MONTH(CURRENT_TIMESTAMP) and YEAR(InvoiceDate)= YEAR(CURRENT_TIMESTAMP) ";
                    if (screen == "DraftedInvoice")
                        criteria = " and MONTH(Cast(InvoiceDate as DateTime)) = MONTH(CURRENT_TIMESTAMP) and YEAR(InvoiceDate)= YEAR(CURRENT_TIMESTAMP) and ISNULL(B_IsSaveDraft,0)=1 and X_TransType='PURCHASE' ";

                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and ([Invoice No] like '%" + xSearchkey + "%' or Vendor like '%" + xSearchkey + "%' or x_BranchName like '%" + xSearchkey + "%' or x_VendorInvoice like '%" + xSearchkey + "%' or [Invoice Date] like '%" + xSearchkey + "%' or invoiceNetAmt like '%" + xSearchkey + "%' or x_Description like '%" + xSearchkey + "%' or X_ProjectCode like '%" + xSearchkey + "%' or X_ProjectName like '%" + xSearchkey + "%' )";

                    if (CheckClosedYear == false)
                    {
                        if (bAllBranchData == true)
                        {
                            Searchkey = Searchkey + " and X_TransType='" + X_TransType + "' and  N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and B_YearEndProcess=0 and N_PurchaseType = 0 ";
                        }
                        else
                        {
                            Searchkey = Searchkey + "and X_TransType='" + X_TransType + "' and  N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and B_YearEndProcess=0 and N_PurchaseType = 0  and N_BranchID=" + nBranchID + "";
                        }
                    }
                    else
                    {
                        if (bAllBranchData == true)
                        {
                            Searchkey = Searchkey + " and X_TransType='" + X_TransType + "' and  N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and B_YearEndProcess=1 and N_PurchaseType = 0 ";
                        }
                        else
                        {
                            Searchkey = Searchkey + "and X_TransType='" + X_TransType + "' and  N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and B_YearEndProcess=1 and N_PurchaseType = 0  and N_BranchID=" + nBranchID + "";
                        }
                    }
                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_PurchaseID desc";
                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "invoiceNo":
                                xSortBy = "N_PurchaseID " + xSortBy.Split(" ")[1];
                                break;
                            case "invoiceDate":
                                xSortBy = "Cast([Invoice Date] as DateTime ) " + xSortBy.Split(" ")[1];
                                break;
                            case "invoiceNetAmt":
                                xSortBy = "Cast(REPLACE(InvoiceNetAmt,',','') as Numeric(10," + N_decimalPlace + ")) " + xSortBy.Split(" ")[1];
                                break;
                            case "x_ProjectName":
                                xSortBy = "[x_ProjectName] " + xSortBy.Split(" ")[1];
                                break;
                            case "x_ProjectCode":
                                xSortBy = "[x_ProjectCode] " + xSortBy.Split(" ")[1];
                                break;
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }


                    int Count = (nPage - 1) * nSizeperpage;
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") N_PurchaseID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],InvoiceNetAmt,X_BranchName,X_Description,N_PaymentMethod,N_FnYearID,N_BranchID,N_LocationID,N_VendorID,N_InvDueDays,B_IsSaveDraft,N_BalanceAmt,X_DueDate,X_POrderNo,d_PrintDate,x_VendorInvoice,N_FormID,X_VendorName_Ar,X_ProjectCode,X_ProjectName from vw_InvPurchaseInvoiceNo_Search_Cloud where N_CompanyID=@p1 and N_FnYearID=@p2 and isNull(N_FormID, 65)=@p3 " + criteria + Searchkey + " " + " Group By N_PurchaseID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],InvoiceNetAmt,X_BranchName,X_Description,N_PaymentMethod,N_FnYearID,N_BranchID,N_LocationID,N_VendorID,N_InvDueDays,B_IsSaveDraft,N_BalanceAmt,X_DueDate,X_POrderNo,d_PrintDate,x_VendorInvoice,N_FormID,X_VendorName_Ar,X_ProjectCode,X_ProjectName" + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") N_PurchaseID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],InvoiceNetAmt,X_BranchName,X_Description,N_PaymentMethod,N_FnYearID,N_BranchID,N_LocationID,N_VendorID,N_InvDueDays,B_IsSaveDraft,N_BalanceAmt,X_DueDate,X_POrderNo,d_PrintDate,x_VendorInvoice,N_FormID,X_VendorName_Ar,X_ProjectCode,X_ProjectName from vw_InvPurchaseInvoiceNo_Search_Cloud where N_CompanyID=@p1 and N_FnYearID=@p2 and isNull(N_FormID, 65)=@p3 " + criteria + Searchkey + " and  N_PurchaseID not in (select top(" + Count + ") N_PurchaseID from vw_InvPurchaseInvoiceNo_Search_Cloud where N_CompanyID=@p1 and N_FnYearID=@p2 and isNull(N_FormID, 0)=@p3 " + criteria + Searchkey + xSortBy + " ) " + "Group By N_PurchaseID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],InvoiceNetAmt,X_BranchName,X_Description,N_PaymentMethod,N_FnYearID,N_BranchID,N_LocationID,N_VendorID,N_InvDueDays,B_IsSaveDraft,N_BalanceAmt,X_DueDate,X_POrderNo,d_PrintDate,x_VendorInvoice,N_FormID,X_VendorName_Ar,X_ProjectCode,X_ProjectName" + xSortBy;

                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearId);
                    Params.Add("@p3", nFormID);
                    SortedList OutPut = new SortedList();


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    //dt = myFunctions.AddNewColumnToDataTable(dt, "N_BalanceAmt", typeof(double), 0);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "N_DueDays", typeof(string), "");
                    foreach (DataRow var in dt.Rows)
                    {
                        double BalanceAmt = 0;
                        // object objBal = dLayer.ExecuteScalar("SELECT  Sum(PurchaseBalanceAmt) from  vw_InvPayables Where  N_VendorID=" + var["N_VendorID"] + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_PurchaseID = " + var["N_PurchaseID"], Params, connection);


                        BalanceAmt = myFunctions.getVAL(var["N_BalanceAmt"].ToString());
                        if (BalanceAmt > 0)
                        {
                            //var["N_BalanceAmt"] = BalanceAmt;
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


                    sqlCommandCount = "select count(1) as N_Count,sum(Cast(REPLACE(InvoiceNetAmt,',','') as Numeric(16," + N_decimalPlace + ")) ) as TotalAmount from vw_InvPurchaseInvoiceNo_Search_Cloud where  N_CompanyID=@p1 and N_FnYearID=@p2 and isNull(N_FormID, 65)=@p3 " + criteria + " " + Searchkey + "";
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
                        //return Ok(_api.Warning("No Results Found"));
                        return Ok(_api.Success(OutPut));
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
        public ActionResult GetPurchaseOrderList(int? nCompanyId, int nFnYearId, int nVendorId, bool showAllBranch)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "";
            sqlCommandText = "select top(30) N_PurchaseID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],InvoiceNetAmt from vw_InvPurchaseInvoiceNo_Search_Cloud where N_CompanyID=@p1 and N_FnYearID=@p2";

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

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
                    return StatusCode(200, _api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(dt);
                }
            }
            catch (Exception e)
            {
                return StatusCode(404, _api.Error(User, e.Message));
            }
        }
        [HttpGet("listdetails")]
        public ActionResult GetPurchaseInvoiceDetails(int nCompanyId, int nFnYearId, string nPurchaseNO, bool showAllBranch, int nBranchId, string xPOrderNo, string xGrnNo, string multipleGrnNo, int nServiceSheetID, bool invoiceTax, string multipleRentalPO, bool enableDayWise, string xServiceSheetID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                DataSet dt = new DataSet();
                SortedList Params = new SortedList();
                DataTable dtPurchaseInvoice = new DataTable();
                DataTable dtPurchaseInvoiceDetails = new DataTable();
                DataTable dtFreightCharges = new DataTable();
                int N_PurchaseID = 0;
                int N_POrderID = 0;
                int nCompanyID = myFunctions.GetCompanyID(User);
                int N_decimalPlace = 2;
                bool IsDirectMRN = false;
                N_decimalPlace = myFunctions.getIntVAL(myFunctions.ReturnSettings("Purchase", "Decimal_Place", "N_Value", nCompanyID, dLayer, connection));
                N_decimalPlace = N_decimalPlace == 0 ? 2 : N_decimalPlace;
                int ServiceSheetID = 0;

                if (xGrnNo != null)
                {
                    object purchaseOrderNo = dLayer.ExecuteScalar("select N_POrderid from Inv_MRN where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and X_MRNNo='" + xGrnNo + "'", Params, connection);

                    if (myFunctions.getVAL(purchaseOrderNo.ToString()) > 0)
                    {
                        IsDirectMRN = false;
                    }
                    else
                    {
                        IsDirectMRN = true;
                    }
                }

                if (nPurchaseNO != null)
                    nPurchaseNO = nPurchaseNO.Replace("%2F", "/");







                Params.Add("@CompanyID", nCompanyId);
                Params.Add("@YearID", nFnYearId);
                Params.Add("@TransType", "PURCHASE");
                Params.Add("@BranchID", nBranchId);
                string X_MasterSql = "";
                string X_DetailsSql = "";
                string X_FreightSql = "";

                if (xGrnNo == null) xGrnNo = "";

                if (nPurchaseNO != null)

                     {
                    Params.Add("@PurchaseNo", nPurchaseNO);
                    X_MasterSql = "select * from vw_Inv_PurchaseDisp where N_CompanyID=@CompanyID and X_InvoiceNo=@PurchaseNo and N_FnYearID=@YearID and X_TransType=@TransType" + (showAllBranch ? " order by N_PurchaseID " : " and  N_BranchId=@BranchID order by N_PurchaseID");
                }
                else if (xPOrderNo != null)
                {
                    Params.Add("@POrderNo", xPOrderNo);
                    X_MasterSql = "select * from vw_Inv_PurchaseOrderAsInvoiceMaster where N_CompanyID=@CompanyID and X_POrderNo=@POrderNo and N_FnYearID=@YearID " + (showAllBranch ? "" : " and  N_BranchId=@BranchID");
                }
                else if (nServiceSheetID > 0)
                {
                    Params.Add("@nServiceSheetID", nServiceSheetID);
                    X_MasterSql = "select * from vw_InvVendorSTAsInvoiceMaster where N_CompanyID=@CompanyID and N_ServiceSheetID=@nServiceSheetID and N_FnYearID=@YearID " + (showAllBranch ? "" : " and  N_BranchId=@BranchID");
                }

                try
                {

                    if (xGrnNo != "")
                    {
                        Params.Add("@xGrnNo", xGrnNo);
                        object res = dLayer.ExecuteScalar("select X_InvoiceNo from Inv_Purchase where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and N_RsID in ( Select N_MRNID from Inv_MRN where N_CompanyID = " + nCompanyId + " and N_FnYearID = " + nFnYearId + " and X_MRNNo='" + xGrnNo + "')", Params, connection);
                        if (res != null)
                        {
                            nPurchaseNO = res.ToString();
                            X_MasterSql = "select * from vw_Inv_PurchaseDisp where N_CompanyID=@CompanyID and X_InvoiceNo='" + nPurchaseNO + "' and N_FnYearID=@YearID and X_TransType=@TransType" + (showAllBranch ? "" : " and  N_BranchId=@BranchID");
                        }
                        else
                        {
                            X_MasterSql = "select * from vw_Inv_MRNAsInvoiceMaster where N_CompanyID=@CompanyID and X_MRNNo=@xGrnNo and N_FnYearID=@YearID and B_IsSaveDraft<>1 " + (showAllBranch ? "" : " and  N_BranchId=@BranchID");
                        }
                    }
                    if (multipleGrnNo != null && multipleGrnNo != "")
                    {
                        X_MasterSql = "select * from vw_Inv_MRNAsInvoiceMaster where N_CompanyID=@CompanyID and N_MRNID in (" + multipleGrnNo + ") and B_IsSaveDraft<>1 " + (showAllBranch ? "" : " and  N_BranchId=@BranchID");

                    }
                    if (multipleRentalPO != null && multipleRentalPO != "")
                    {
                        string[] X_POrderID = multipleRentalPO.Split(",");
                        int N_POID = myFunctions.getIntVAL(X_POrderID[0].ToString());
                        string[] X_ServiceSheetID = xServiceSheetID.Split(",");
                        ServiceSheetID = myFunctions.getIntVAL(X_ServiceSheetID[0].ToString());
                        X_MasterSql = "select * from vw_InvVendorSTAsInvoiceMaster where N_CompanyID=@CompanyID and N_ServiceSheetID in (" + ServiceSheetID + ") and N_POrderID in (" + N_POID + ")" + (showAllBranch ? "" : " and  N_BranchId=@BranchID");
                    }


                    dtPurchaseInvoice = dLayer.ExecuteDataTable(X_MasterSql, Params, connection);

                    if (IsDirectMRN && !invoiceTax)
                    {
                        object taxID = dLayer.ExecuteScalar("Select N_Value from Gen_Settings where N_CompanyId=" + nCompanyId + " and X_Description='DefaultTaxCategory' and X_Group='Inventory'", Params, connection);

                        if (taxID == null)
                        {
                            taxID = 0;
                        }
                        if (taxID != null && myFunctions.getIntVAL(taxID.ToString()) != 0)
                        {
                            object category = dLayer.ExecuteScalar("Select X_DisplayName from Acc_TaxCategory where N_CompanyId=" + nCompanyId + " and X_PkeyCode=" + taxID + " ", Params, connection);
                            object taxCatID = dLayer.ExecuteScalar("Select N_PkeyID from Acc_TaxCategory where N_CompanyId=" + nCompanyId + " and X_PkeyCode=" + taxID + " ", Params, connection);
                            object percentage = dLayer.ExecuteScalar("Select Cast(REPLACE(N_Amount,',','') as Numeric(10,0)) from Acc_TaxCategory where N_PkeyID=" + taxID + " ", Params, connection);

                            dtPurchaseInvoice.Rows[0]["X_DisplayName"] = category.ToString();
                            dtPurchaseInvoice.Rows[0]["N_TaxCategoryId"] = myFunctions.getIntVAL(taxCatID.ToString());
                            dtPurchaseInvoice.Rows[0]["n_TaxPercentage"] = myFunctions.getIntVAL(percentage.ToString());
                        }



                    }


                    object objPayment = dLayer.ExecuteScalar("SELECT dbo.Inv_PayReceipt.X_Type, dbo.Inv_PayReceiptDetails.N_InventoryId FROM dbo.Inv_PayReceipt INNER JOIN dbo.Inv_PayReceiptDetails ON dbo.Inv_PayReceipt.N_PayReceiptId = dbo.Inv_PayReceiptDetails.N_PayReceiptId Where dbo.Inv_PayReceipt.X_Type='PP' and dbo.Inv_PayReceiptDetails.X_TransType='PURCHASE' and  dbo.Inv_PayReceipt.B_IsDraft <> 1 and dbo.Inv_PayReceiptDetails.N_InventoryId in (select N_PurchaseID from Inv_Purchase where X_InvoiceNo='" + nPurchaseNO + "' and N_CompanyID=@CompanyID and N_FnYearID=@YearID)", Params, connection);
                    if (objPayment != null)
                        myFunctions.AddNewColumnToDataTable(dtPurchaseInvoice, "B_PaymentProcessed", typeof(Boolean), true);
                    else
                        myFunctions.AddNewColumnToDataTable(dtPurchaseInvoice, "B_PaymentProcessed", typeof(Boolean), false);



                    if (dtPurchaseInvoice.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                    dtPurchaseInvoice = _api.Format(dtPurchaseInvoice, "Master");
                    N_PurchaseID = myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["N_PurchaseID"].ToString());
                    N_POrderID = myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["N_POrderID"].ToString());
                    if (nPurchaseNO != null)

                    {
                        SortedList Status = StatusSetup(N_PurchaseID, nFnYearId, showAllBranch, dtPurchaseInvoice, connection);
                        dtPurchaseInvoice = myFunctions.AddNewColumnToDataTable(dtPurchaseInvoice, "TxnStatus", typeof(SortedList), Status);
                    }

                    //PURCHASE INVOICE DETAILS
                    object mrnCount = dLayer.ExecuteScalar("SELECT count(Sec_UserPrevileges.N_MenuID) as Count FROM Sec_UserPrevileges INNER JOIN Sec_UserCategory ON Sec_UserPrevileges.N_UserCategoryID = Sec_UserCategory.N_UserCategoryID and Sec_UserPrevileges.N_MenuID=555 and Sec_UserCategory.N_CompanyID=" + nCompanyID + " and Sec_UserPrevileges.B_Visible=1", connection);
                    bool B_MRNVisible = myFunctions.getIntVAL(mrnCount.ToString()) > 0 ? true : false;
                    if (nPurchaseNO != null)
                    {
                        if (B_MRNVisible)
                            X_DetailsSql = "SELECT vw_InvPurchaseDetails.*,dbo.SP_SellingPrice(vw_InvPurchaseDetails.N_ItemID, vw_InvPurchaseDetails.N_CompanyID) AS N_UnitSPrice,dbo.SP_SellingPrice(vw_InvPurchaseDetails.N_ItemID, vw_InvPurchaseDetails.N_CompanyID) AS N_SPrice, Inv_MRN.B_IsDirectMRN, Inv_MRNDetails.N_MRNDetailsID FROM vw_InvPurchaseDetails LEFT OUTER JOIN Inv_MRNDetails ON vw_InvPurchaseDetails.N_CompanyID = Inv_MRNDetails.N_CompanyID AND  vw_InvPurchaseDetails.N_PurchaseDetailsID = Inv_MRNDetails.N_PurchaseDetailsID LEFT OUTER JOIN Inv_PurchaseOrder ON vw_InvPurchaseDetails.N_POrderID = Inv_PurchaseOrder.N_POrderID LEFT OUTER JOIN Inv_MRN ON vw_InvPurchaseDetails.N_RsID = Inv_MRN.N_MRNID  Where vw_InvPurchaseDetails.N_CompanyID=@CompanyID and vw_InvPurchaseDetails.N_PurchaseID=" + N_PurchaseID + (showAllBranch ? "" : " and vw_InvPurchaseDetails.N_BranchId=@BranchID");
                        else
                            X_DetailsSql = "SELECT vw_InvPurchaseDetails.*, Inv_PurchaseOrder.X_POrderNo, dbo.SP_Cost(vw_InvPurchaseDetails.N_ItemID, vw_InvPurchaseDetails.N_CompanyID, '') AS N_UnitLPrice,dbo.SP_SellingPrice(vw_InvPurchaseDetails.N_ItemID, vw_InvPurchaseDetails.N_CompanyID) AS N_UnitSPrice, Inv_MRNDetails.N_MRNDetailsID FROM vw_InvPurchaseDetails LEFT OUTER JOIN Inv_MRNDetails ON vw_InvPurchaseDetails.N_CompanyID = Inv_MRNDetails.N_CompanyID AND vw_InvPurchaseDetails.N_PurchaseDetailsID = Inv_MRNDetails.N_PurchaseDetailsID LEFT OUTER JOIN Inv_PurchaseOrder ON vw_InvPurchaseDetails.N_POrderID = Inv_PurchaseOrder.N_POrderID Where vw_InvPurchaseDetails.N_CompanyID=@CompanyID and vw_InvPurchaseDetails.N_PurchaseID=" + N_PurchaseID + (showAllBranch ? "" : " and vw_InvPurchaseDetails.N_BranchId=@BranchID");
                    }
                    else if (xPOrderNo != null)
                    {
                        X_DetailsSql = "select * from vw_POInvoice_PendingDetail where  N_POrderID=" + N_POrderID + " AND N_CompanyID = @CompanyID ORDER BY N_POrderDetailsID";
                    }
                    else if (xGrnNo != null && xGrnNo != "")
                    {
                        int n_MRNID = myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["N_MRNID"].ToString());

                        int EnableInvoicebasedtax = myFunctions.getIntVAL(myFunctions.ReturnSettings("65", "EnableInvoicebasedtax", "N_Value", nCompanyID, dLayer, connection));
                        if (N_POrderID > 0 || EnableInvoicebasedtax == 1)
                            X_DetailsSql = "Select *,dbo.SP_Cost(vw_InvMRNDetails.N_ItemID,vw_InvMRNDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvMRNDetails.N_ItemID,vw_InvMRNDetails.N_CompanyID) As N_UnitSPrice,dbo.SP_SellingPrice(vw_InvMRNDetails.N_ItemID,vw_InvMRNDetails.N_CompanyID) As N_SPrice,N_MRNID AS N_RsID   from vw_InvMRNDetails Where N_CompanyID=@CompanyID and N_MRNID=" + n_MRNID;
                        else
                        {

                            X_DetailsSql = "Select *,dbo.SP_Cost(vw_InvMRNDetailsDirect.N_ItemID,vw_InvMRNDetailsDirect.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvMRNDetailsDirect.N_ItemID,vw_InvMRNDetailsDirect.N_CompanyID) As N_SPrice,dbo.SP_SellingPrice(vw_InvMRNDetailsDirect.N_ItemID,vw_InvMRNDetailsDirect.N_CompanyID) As N_UnitSPrice,dbo.SP_SellingPrice(vw_InvMRNDetailsDirect.N_ItemID,vw_InvMRNDetailsDirect.N_CompanyID) As N_SPrice, N_MRNID AS N_RsID   from vw_InvMRNDetailsDirect Where N_CompanyID=@CompanyID and N_MRNID=" + n_MRNID;

                        }

                    }
                    else if (nServiceSheetID > 0)
                    {
                        // X_DetailsSql = "select * from vw_InvVendorSTAsInvoiceDetails where N_CompanyID=@CompanyID and N_ServiceSheetID=" + nServiceSheetID + (showAllBranch ? "" : " and  N_BranchId=@BranchID");

                        if (enableDayWise)
                            X_DetailsSql = "select * from vw_InvVendorSTAsInvoiceDetailsDaywise where N_CompanyID=@CompanyID and N_ServiceSheetID=" + nServiceSheetID + (showAllBranch ? "" : " and  N_BranchId=@BranchID");
                        else
                            X_DetailsSql = "select * from vw_InvVendorSTAsInvoiceDetails where N_CompanyID=@CompanyID and N_ServiceSheetID=" + nServiceSheetID + (showAllBranch ? "" : " and  N_BranchId=@BranchID");
                    }
                    if (multipleRentalPO != null && multipleRentalPO != "")
                    {
                        X_DetailsSql = "select * from vw_InvVendorSTAsInvoiceDetails where N_CompanyID=@CompanyID and N_ServiceSheetID in ("+ xServiceSheetID +") and N_POrderID in (" + multipleRentalPO + ")" + (showAllBranch ? "" : " and  N_BranchId=@BranchID");
                    }

                    //multiple GRN From Invoice

                    object Tax1ID = "";
                    if (multipleGrnNo != null && multipleGrnNo != "")
                        X_DetailsSql = "Select *,dbo.SP_Cost(vw_InvMRNDetailsDirect.N_ItemID,vw_InvMRNDetailsDirect.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvMRNDetailsDirect.N_ItemID,vw_InvMRNDetailsDirect.N_CompanyID) As N_UnitSPrice,dbo.SP_SellingPrice(vw_InvMRNDetailsDirect.N_ItemID,vw_InvMRNDetailsDirect.N_CompanyID) As N_SPrice,N_MrnID AS N_RsID  from vw_InvMRNDetailsDirect Where N_CompanyID=@CompanyID and N_MRNID in (" + multipleGrnNo + ")";


                    dtPurchaseInvoiceDetails = dLayer.ExecuteDataTable(X_DetailsSql, Params, connection);

                    string x_PONo = dtPurchaseInvoiceDetails.Rows[0]["X_POrderNo"].ToString();
                    for (int j = 1; j < dtPurchaseInvoiceDetails.Rows.Count; j++)
                    {
                        if (dtPurchaseInvoiceDetails.Rows[j]["X_POrderNo"].ToString() != dtPurchaseInvoiceDetails.Rows[j - 1]["X_POrderNo"].ToString())
                            x_PONo = x_PONo + "," + dtPurchaseInvoiceDetails.Rows[j]["X_POrderNo"].ToString();
                    };
                    myFunctions.AddNewColumnToDataTable(dtPurchaseInvoice, "X_PONo", typeof(string), x_PONo);
                    dtPurchaseInvoice.AcceptChanges();

                    if (multipleGrnNo != null && multipleGrnNo != "")
                    {
                        foreach (DataRow Row in dtPurchaseInvoiceDetails.Rows)
                        {
                            if (myFunctions.getIntVAL(Row["n_POrderDetailsID"].ToString()) > 0)
                            {
                                DataTable deliveryDetails = new DataTable();
                                string directDelivery = "SELECT  Acc_TaxCategory.X_DisplayName, Inv_PurchaseOrderDetails.N_TaxCategoryID1, Inv_PurchaseOrderDetails.N_TaxAmt1, Inv_PurchaseOrderDetails.N_TaxPercentage1 FROM  Inv_PurchaseOrderDetails INNER JOIN Acc_TaxCategory ON Inv_PurchaseOrderDetails.N_TaxCategoryID1 = Acc_TaxCategory.N_PkeyID AND Inv_PurchaseOrderDetails.N_CompanyID = Acc_TaxCategory.N_CompanyID where Inv_PurchaseOrderDetails.N_POrderDetailsID=" + myFunctions.getIntVAL(Row["n_POrderDetailsID"].ToString());
                                deliveryDetails = dLayer.ExecuteDataTable(directDelivery, Params, connection);

                                if (deliveryDetails.Rows.Count > 0)
                                {
                                    Row["x_DisplayName1"] = deliveryDetails.Rows[0]["X_DisplayName"];
                                    Row["n_TaxPercentage1"] = deliveryDetails.Rows[0]["N_TaxPercentage1"];
                                    Row["n_TaxCategoryID1"] = deliveryDetails.Rows[0]["N_TaxCategoryID1"];
                                }
                                else
                                {
                                    DataTable OrderData = new DataTable();
                                    string purchaseData = "SELECT Acc_TaxCategory.X_DisplayName, Inv_PurchaseOrder.N_TaxCategoryID, Inv_PurchaseOrder.N_TaxPercentage, Inv_PurchaseOrder.N_TaxAmt FROM  Inv_PurchaseOrder LEFT OUTER JOIN Acc_TaxCategory ON Inv_PurchaseOrder.N_TaxCategoryID = Acc_TaxCategory.N_PkeyID AND Inv_PurchaseOrder.N_CompanyID = Acc_TaxCategory.N_CompanyID where Inv_PurchaseOrder.N_POrderID=" + myFunctions.getIntVAL(Row["n_POrderID"].ToString());
                                    OrderData = dLayer.ExecuteDataTable(purchaseData, Params, connection);
                                    Row["x_DisplayName1"] = OrderData.Rows[0]["X_DisplayName"];
                                    Row["n_TaxPercentage1"] = OrderData.Rows[0]["N_TaxPercentage"];
                                    Row["n_TaxCategoryID1"] = OrderData.Rows[0]["N_TaxCategoryID"];
                                }

                                //    Row["N_TaxAmt1"]=deliveryDetails.Rows[0]["N_TaxAmt1"];


                            }
                        }
                        dtPurchaseInvoiceDetails.AcceptChanges();
                    }



                    X_FreightSql = "Select *,X_ShortName as X_CurrencyName FROM vw_InvPurchaseFreights WHERE N_PurchaseID=" + N_PurchaseID;
                    dtFreightCharges = dLayer.ExecuteDataTable(X_FreightSql, Params, connection);

                    if (nPurchaseNO != null)
                    {
                        object RetQty = dLayer.ExecuteScalar("select X_CreditNoteNo from Inv_PurchaseReturnMaster where N_PurchaseId =" + N_PurchaseID + " and N_CompanyID=@CompanyID and N_FnYearID=@YearID", Params, connection);
                        if (RetQty != null)
                        {
                            dtPurchaseInvoice = myFunctions.AddNewColumnToDataTable(dtPurchaseInvoice, "IsReturnDone", typeof(bool), true);
                            dtPurchaseInvoice = myFunctions.AddNewColumnToDataTable(dtPurchaseInvoice, "X_ReturnCode", typeof(string), RetQty.ToString());
                        }
                        else
                        {
                            dtPurchaseInvoice = myFunctions.AddNewColumnToDataTable(dtPurchaseInvoice, "IsReturnDone", typeof(bool), false);
                            dtPurchaseInvoice = myFunctions.AddNewColumnToDataTable(dtPurchaseInvoice, "X_ReturnCode", typeof(string), "");
                        }
                        object N_Qty = dLayer.ExecuteScalar("select Sum(N_Qty) from vw_InvPurchaseDetails_Display where N_CompanyID=" + nCompanyId + " and N_PurchaseID=" + N_PurchaseID + " ", Params, connection);
                        object N_RetQty = dLayer.ExecuteScalar("select Sum(RetQty) from vw_InvPurchaseDetails_Display where N_CompanyID=" + nCompanyId + " and N_PurchaseID=" + N_PurchaseID + " ", Params, connection);
                        if (N_RetQty != null && myFunctions.getVAL(N_RetQty.ToString()) > 0)
                        {
                            if (myFunctions.getVAL(N_Qty.ToString()) > (myFunctions.getVAL(N_RetQty.ToString())))
                            {
                                dtPurchaseInvoice.Rows[0]["IsReturnDone"] = false;
                                dtPurchaseInvoice.Rows[0]["X_ReturnCode"] = "";

                            }
                        }
                        // int nPaymentMethod = mastr
                    }


                    dtPurchaseInvoiceDetails = _api.Format(dtPurchaseInvoiceDetails, "Details");
                    DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["N_VendorID"].ToString()), myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["N_PurchaseID"].ToString()), this.N_FormID, myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["N_FnYearID"].ToString()), User, connection);
                    Attachments = _api.Format(Attachments, "attachments");
                    dtFreightCharges = _api.Format(dtFreightCharges, "freightCharges");



                    dt.Tables.Add(dtPurchaseInvoice);
                    dt.Tables.Add(Attachments);
                    dt.Tables.Add(dtPurchaseInvoiceDetails);
                    dt.Tables.Add(dtFreightCharges);
                    return Ok(_api.Success(dt));
                }
                catch (Exception e)
                {
                    return Ok(_api.Error(User, e));
                }



            }


        }





        private SortedList StatusSetup(int nPurchaseID, int nFnYearID, bool showAllBranch, DataTable dtPurchaseInvoice, SqlConnection connection)
        {




            SortedList TxnStatus = new SortedList();
            TxnStatus.Add("Label", "");
            TxnStatus.Add("LabelColor", "");
            TxnStatus.Add("Alert", "");
            TxnStatus.Add("DeleteEnabled", true);
            TxnStatus.Add("SaveEnabled", true);
            TxnStatus.Add("ReceiptNumbers", "");
            int nCompanyID = myFunctions.GetCompanyID(User);
            int N_PaymentMethod = myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["N_PaymentMethod"].ToString());
            string x_InvoiceNo = (dtPurchaseInvoice.Rows[0]["x_InvoiceNo"].ToString());
            int n_BranchId = myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["n_BranchId"].ToString());
            string x_TransType = (dtPurchaseInvoice.Rows[0]["x_TransType"].ToString());
            int n_LocationID = myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["n_LocationID"].ToString());
            bool b_AllowCashPay = myFunctions.getBoolVAL(dtPurchaseInvoice.Rows[0]["b_AllowCashPay"].ToString());
            int n_VendorID = myFunctions.getIntVAL(dtPurchaseInvoice.Rows[0]["n_VendorID"].ToString());
            object objPaid = null, objBal = null;
            double InvoicePaidAmt = 0, BalanceAmt = 0;
            string PurchaseID = "";

            string PurchaseSql = "Select N_PurchaseID from vw_Inv_PurchaseDisp Where N_CompanyID=" + nCompanyID + " and X_InvoiceNo='" + x_InvoiceNo + "' and N_FnYearID=" + nFnYearID + " and X_TransType='PURCHASE'";
            DataTable PurchaseTable = dLayer.ExecuteDataTable(PurchaseSql, connection);
            foreach (DataRow kvar in PurchaseTable.Rows)
            {
                PurchaseID += PurchaseID == "" ? kvar["N_PurchaseID"].ToString() : " , " + kvar["N_PurchaseID"].ToString();

            }

            if (N_PaymentMethod == 2)
            {

                objPaid = dLayer.ExecuteScalar("SELECT  isnull(Sum(dbo.Inv_PayReceiptDetails.N_Amount),0) as PaidAmount FROM  dbo.Inv_PayReceipt INNER JOIN dbo.Inv_PayReceiptDetails ON dbo.Inv_PayReceipt.N_PayReceiptId = dbo.Inv_PayReceiptDetails.N_PayReceiptId Where dbo.Inv_PayReceipt.X_Type='PP' and dbo.Inv_PayReceiptDetails.X_TransType='PURCHASE' and  isnull(dbo.Inv_PayReceipt.B_IsDraft,0) <> 1 and dbo.Inv_PayReceiptDetails.N_InventoryId in (" + PurchaseID + ") group by dbo.Inv_PayReceiptDetails.N_PayReceiptId", connection);
            }
            else
            {
                if (showAllBranch == true)
                    objPaid = dLayer.ExecuteScalar("Select N_CashPaid from vw_Inv_PurchaseDisp Where N_CompanyID=" + nCompanyID + " and X_InvoiceNo='" + x_InvoiceNo + "' and N_FnYearID=" + nFnYearID + " and X_TransType='" + x_TransType + "'", connection);
                else
                    objPaid = dLayer.ExecuteScalar("Select N_CashPaid from vw_Inv_PurchaseDisp Where N_CompanyID=" + nCompanyID + " and X_InvoiceNo='" + x_InvoiceNo + "' and N_FnYearID=" + nFnYearID + " and N_BranchId=" + n_BranchId + " and N_LocationID =" + n_LocationID + "  and X_TransType='" + x_TransType + "'", connection);
            }

            if (objPaid == null && N_PaymentMethod == 2)
            {
                if (showAllBranch == true)
                    objPaid = dLayer.ExecuteScalar("Select N_CashPaid from vw_Inv_PurchaseDisp Where N_CompanyID=" + nCompanyID + " and X_InvoiceNo='" + x_InvoiceNo + "' and N_FnYearID=" + nFnYearID + " and X_TransType='" + x_TransType + "'", connection);
                else
                    objPaid = dLayer.ExecuteScalar("Select N_CashPaid from vw_Inv_PurchaseDisp Where N_CompanyID=" + nCompanyID + " and X_InvoiceNo='" + x_InvoiceNo + "' and N_FnYearID=" + nFnYearID + " and N_BranchId=" + n_BranchId + " and N_LocationID =" + n_LocationID + "  and X_TransType='" + x_TransType + "'", connection);
            }

            objBal = dLayer.ExecuteScalar("SELECT  Sum(PurchaseBalanceAmt) from  vw_InvPayables Where  N_VendorID=" + n_VendorID + " and N_CompanyID=" + nCompanyID + " and N_PurchaseID = " + nPurchaseID, connection);
            object RetQty = dLayer.ExecuteScalar("select Isnull(Count(N_CreditNoteId),0) from Inv_PurchaseReturnMaster where  N_PurchaseId=" + nPurchaseID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, connection);



            if (objPaid != null)
                InvoicePaidAmt = myFunctions.getVAL(objPaid.ToString());
            if (objBal != null)
                BalanceAmt = myFunctions.getVAL(objBal.ToString());






            if (InvoicePaidAmt == 0)
            {
                if (myFunctions.getIntVAL(RetQty.ToString()) > 0)
                {
                    if (BalanceAmt == 0)
                    {
                        TxnStatus["Label"] = "Paid(Return)";
                        TxnStatus["LabelColor"] = "Green";
                        TxnStatus["Alert"] = "";
                    }
                    else
                    {


                        TxnStatus["Label"] = "NotPaid(Return)";
                        TxnStatus["LabelColor"] = "Red";
                        TxnStatus["Alert"] = "";
                    }
                }

                else
                {
                    TxnStatus["Label"] = "Not Paid ";
                    TxnStatus["LabelColor"] = "Green";
                    TxnStatus["Alert"] = "";
                }
            }
            else
            {
                if (myFunctions.getIntVAL(RetQty.ToString()) > 0)
                {
                    if (BalanceAmt == 0)
                    {
                        TxnStatus["Label"] = "Paid(Return)";
                        TxnStatus["LabelColor"] = "Green";
                        TxnStatus["Alert"] = "";
                    }
                    else
                    {
                        TxnStatus["Label"] = "Partially Paid(Return)";
                        TxnStatus["LabelColor"] = "Green";
                        TxnStatus["Alert"] = "";
                    }
                }
                else
                {
                    if (BalanceAmt == 0)
                    {
                        TxnStatus["Label"] = "Paid";
                        TxnStatus["LabelColor"] = "Green";
                        TxnStatus["Alert"] = "";
                    }
                    else
                    {
                        TxnStatus["Label"] = "Partially Paid";
                        TxnStatus["LabelColor"] = "Green";
                        TxnStatus["Alert"] = "";
                    }

                }
                DataTable dtpayReceipt = dLayer.ExecuteDataTable("SELECT  dbo.Inv_PayReceipt.X_VoucherNo FROM  dbo.Inv_PayReceipt INNER JOIN dbo.Inv_PayReceiptDetails ON dbo.Inv_PayReceipt.N_PayReceiptId = dbo.Inv_PayReceiptDetails.N_PayReceiptId Where dbo.Inv_PayReceiptDetails.X_TransType='PURCHASE' and dbo.Inv_PayReceiptDetails.N_InventoryId in (" + nPurchaseID + ")", connection);
                string InvoiceNos = "";
                if (N_PaymentMethod == 2)
                {
                    foreach (DataRow var in dtpayReceipt.Rows)
                    {
                        InvoiceNos += var["X_VoucherNo"].ToString() + " , ";
                    }
                    char[] trim = { ',', ' ' };
                    TxnStatus["Label"] = TxnStatus["Label"].ToString() + " (Payment No: " + InvoiceNos.ToString().TrimEnd(trim) + ")";
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


                    Result = txnHandler.PurchaseSaveData(ds, ipAddress, User, dLayer, connection, transaction);

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


        //Save....
        //private SortedList PurchaseSaveData([FromBody] DataSet ds,IDataAccessLayer dLayer,SqlConnection connection, SqlTransaction transaction)
        // {

        //     DataTable MasterTable;
        //     DataTable DetailTable;
        //     DataTable DetailsToImport;
        //     DataTable WarrantyInfo;
        //     MasterTable = ds.Tables["master"];
        //     DetailTable = ds.Tables["details"];
        //     WarrantyInfo = ds.Tables["warrantyInformation"];
        //     DataTable Approvals;
        //     Approvals = ds.Tables["approval"];
        //     DataRow ApprovalRow = Approvals.Rows[0];
        //     DataTable Attachment = ds.Tables["attachments"];
        //     DataTable PurchaseFreight = ds.Tables["freightCharges"];
        //     SortedList Params = new SortedList();
        //     SortedList Result = new SortedList();
        //     // Auto Gen
        //     string InvoiceNo = "";
        //     DataRow masterRow = MasterTable.Rows[0];
        //     var values = masterRow["x_InvoiceNo"].ToString();
        //     int N_PurchaseID = 0;
        //     int N_SaveDraft = 0;
        //     int nUserID = myFunctions.GetUserID(User);
        //     int nCompanyID = myFunctions.GetCompanyID(User);
        //     int nFnYearID = myFunctions.getIntVAL(masterRow["n_FnYearId"].ToString());
        //     int n_POrderID = myFunctions.getIntVAL(masterRow["N_POrderID"].ToString());
        //     var vendorInvoice="";
        //     if(MasterTable.Columns.Contains("X_VendorInvoice"))
        //           vendorInvoice = masterRow["X_VendorInvoice"].ToString();
        //     int n_MRNID = 0;
        //     if (MasterTable.Columns.Contains("N_RsID"))
        //         n_MRNID = myFunctions.getIntVAL(masterRow["N_RsID"].ToString());
        //     int Dir_Purchase = 1;
        //     int b_FreightAmountDirect = myFunctions.getIntVAL(masterRow["b_FreightAmountDirect"].ToString());
        //     DetailsToImport = ds.Tables["detailsImport"];
        //     bool B_isImport = false;

        //     if (ds.Tables.Contains("detailsImport"))
        //         B_isImport = true;


        //     // try
        //     // {
        //     //     using (SqlConnection connection = new SqlConnection(connectionString))
        //     //     {
        //     //         connection.Open();
        //     //         SqlTransaction transaction;
        //     //         transaction = connection.BeginTransaction();
        //             N_PurchaseID = myFunctions.getIntVAL(masterRow["n_PurchaseID"].ToString());
        //             int N_VendorID = myFunctions.getIntVAL(masterRow["n_VendorID"].ToString());
        //             int N_NextApproverID = 0;

        //             if (!myFunctions.CheckActiveYearTransaction(nCompanyID, nFnYearID, Convert.ToDateTime(MasterTable.Rows[0]["D_InvoiceDate"].ToString()), dLayer, connection, transaction))
        //             {
        //                 object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID=" + nCompanyID + " and convert(date ,'" + MasterTable.Rows[0]["D_InvoiceDate"].ToString() + "') between D_Start and D_End", Params, connection, transaction);
        //                 if (DiffFnYearID != null)
        //                 {
        //                     MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
        //                     nFnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());
        //                 }
        //                 else
        //                  {
        //                 //     transaction.Rollback();
        //                 //     return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
        //                     Result.Add("b_IsCompleted", 0);
        //                     Result.Add("x_Msg", "Transaction date must be in the active Financial Year.");
        //                     return Result;
        //                 }
        //             }
        //             MasterTable.AcceptChanges();

        //             object mrnCount = dLayer.ExecuteScalar("SELECT count(Sec_UserPrevileges.N_MenuID) as Count FROM Sec_UserPrevileges INNER JOIN Sec_UserCategory ON Sec_UserPrevileges.N_UserCategoryID = Sec_UserCategory.N_UserCategoryID and Sec_UserPrevileges.N_MenuID=555 and Sec_UserCategory.N_CompanyID=" + nCompanyID + " and Sec_UserPrevileges.B_Visible=1", connection, transaction);
        //             bool B_MRNVisible = myFunctions.getIntVAL(mrnCount.ToString()) > 0 ? true : false;

        //             if (B_MRNVisible && n_MRNID != 0) Dir_Purchase = 0;

        //             if (N_PurchaseID > 0)
        //             {
        //                 if (CheckProcessed(N_PurchaseID))
        //                 {
        //                     //  transaction.Rollback();
        //                     //  return Ok(_api.Error(User, "Transaction Started!"));
        //                     Result.Add("b_IsCompleted", 0);
        //                     Result.Add("x_Msg", "Transaction Started!");
        //                     return Result;
        //                 }


        //                 object Dir_PurchaseCount = dLayer.ExecuteScalar("SELECT COUNT(Inv_Purchase.N_PurchaseID) FROM Inv_Purchase INNER JOIN Inv_MRN ON Inv_Purchase.N_CompanyID = Inv_MRN.N_CompanyID AND Inv_Purchase.N_RsID = Inv_MRN.N_MRNID AND Inv_Purchase.N_FnYearID = Inv_MRN.N_FnYearID " +
        //                                                                 " WHERE Inv_MRN.B_IsDirectMRN=0 and Inv_Purchase.N_CompanyID=" + nCompanyID + " and Inv_Purchase.N_PurchaseID=" + N_PurchaseID, connection, transaction);

        //                 if (myFunctions.getIntVAL(Dir_PurchaseCount.ToString()) > 0)
        //                     Dir_Purchase = 1;
        //             }

        //             SortedList VendParams = new SortedList();
        //             VendParams.Add("@nCompanyID", nCompanyID);
        //             VendParams.Add("@N_VendorID", N_VendorID);
        //             VendParams.Add("@nFnYearID", nFnYearID);
        //             object objVendorName = dLayer.ExecuteScalar("Select X_VendorName From Inv_Vendor where N_VendorID=@N_VendorID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", VendParams, connection, transaction);
        //             object objVendorCode = dLayer.ExecuteScalar("Select X_VendorCode From Inv_Vendor where N_VendorID=@N_VendorID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", VendParams, connection, transaction);

        //             if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()) && N_PurchaseID > 0)
        //             {
        //                 int N_PkeyID = N_PurchaseID;
        //                 string X_Criteria = "N_PurchaseID=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
        //                 myFunctions.UpdateApproverEntry(Approvals, "Inv_Purchase", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
        //                 N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "PURCHASE", N_PkeyID, values, 1, objVendorName.ToString(), 0, "", 0, User, dLayer, connection, transaction);
        //                 myAttachments.SaveAttachment(dLayer, Attachment, values, N_PurchaseID, objVendorName.ToString().Trim(), objVendorCode.ToString(), N_VendorID, "Vendor Document", User, connection, transaction);

        //                 N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_Purchase where N_PurchaseID=" + N_PurchaseID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, connection, transaction).ToString());
        //                 if (N_SaveDraft == 0)
        //                 {
        //                     try
        //                     {
        //                         // SortedList PostingMRNParam = new SortedList();
        //                         // PostingMRNParam.Add("N_CompanyID", nCompanyID);
        //                         // PostingMRNParam.Add("N_PurchaseID", N_PurchaseID);
        //                         // PostingMRNParam.Add("N_UserID", nUserID);
        //                         // PostingMRNParam.Add("X_SystemName", "ERP Cloud");
        //                         // PostingMRNParam.Add("X_UseMRN", "");
        //                         // PostingMRNParam.Add("N_SaveDraft", N_SaveDraft);
        //                         // PostingMRNParam.Add("N_MRNID", 0);

        //                         // dLayer.ExecuteNonQueryPro("[SP_Inv_MRNposting]", PostingMRNParam, connection, transaction);

        //                         SortedList PostingMRNParam = new SortedList();
        //                         PostingMRNParam.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
        //                         PostingMRNParam.Add("N_PurchaseID", N_PurchaseID);
        //                         PostingMRNParam.Add("N_UserID", nUserID);
        //                         PostingMRNParam.Add("X_SystemName", "ERP Cloud");
        //                         PostingMRNParam.Add("X_UseMRN", "");
        //                         PostingMRNParam.Add("N_SaveDraft", N_SaveDraft);
        //                         PostingMRNParam.Add("B_DirectPurchase", Dir_Purchase);
        //                         PostingMRNParam.Add("N_MRNID", n_MRNID);

        //                         dLayer.ExecuteNonQueryPro("[SP_Inv_MRNprocessing]", PostingMRNParam, connection, transaction);


        //                         SortedList PostingParam = new SortedList();
        //                         PostingParam.Add("N_CompanyID", nCompanyID);
        //                         PostingParam.Add("X_InventoryMode", "PURCHASE");
        //                         PostingParam.Add("N_InternalID", N_PurchaseID);
        //                         PostingParam.Add("N_UserID", nUserID);
        //                         PostingParam.Add("X_SystemName", "ERP Cloud");
        //                         PostingParam.Add("MRN_Flag", Dir_Purchase==0 ? "1" : "0");

        //                         dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Purchase_Posting", PostingParam, connection, transaction);
        //                     }
        //                     catch (Exception ex)
        //                     {
        //                         // transaction.Rollback();
        //                         // return Ok(_api.Error(User, ex.Message));
        //                         Result.Add("b_IsCompleted", 0);
        //                         Result.Add("x_Msg", ex.Message);
        //                         return Result;
        //                     }
        //                 }

        //                 // myFunctions.SendApprovalMail(N_NextApproverID, this.N_FormID, N_PkeyID, "PURCHASE", values, dLayer, connection, transaction, User);
        //                 // transaction.Commit();
        //                 // return Ok(_api.Success("Purchase Approved " + "-" + values));
        //                 Result.Add("b_IsCompleted", 1);
        //                 Result.Add("x_Msg", "Purchase Approved " + "-" + values);
        //                 return Result;
        //             }

        //             // if (values == "@Auto")
        //             // {
        //             //     N_SaveDraft = myFunctions.getIntVAL(masterRow["b_IsSaveDraft"].ToString());

        //             //     Params.Add("N_CompanyID", nCompanyID);
        //             //     Params.Add("N_YearID", nFnYearID);
        //             //     Params.Add("N_FormID", this.N_FormID);
        //             //     Params.Add("N_BranchID", masterRow["n_BranchId"].ToString());

        //             //     InvoiceNo = dLayer.GetAutoNumber("Inv_Purchase", "x_InvoiceNo", Params, connection, transaction);
        //             //     if (InvoiceNo == "")
        //             //     {
        //             //         transaction.Rollback();
        //             //         return Ok(_api.Error(User, "Unable to generate Invoice Number"));
        //             //     }
        //             //     MasterTable.Rows[0]["x_InvoiceNo"] = InvoiceNo;
        //             // }
        //             if (N_PurchaseID == 0 && values != "@Auto")
        //             {
        //                 object N_DocNumber = dLayer.ExecuteScalar("Select 1 from Inv_Purchase Where X_InvoiceNo ='" + values + "' and N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection, transaction);
        //                 if (N_DocNumber == null)
        //                 {
        //                     N_DocNumber = 0;
        //                 }
        //                 if (myFunctions.getVAL(N_DocNumber.ToString()) >= 1)
        //                 {
        //                     // transaction.Rollback();
        //                     // return Ok(_api.Error(User, "Invoice number already in use"));
        //                     Result.Add("b_IsCompleted", 0);
        //                     Result.Add("x_Msg", "Invoice number already in use");
        //                     return Result;
        //                 }
        //             }
        //             if (values == "@Auto")
        //             {
        //                 Params.Add("N_CompanyID", MasterTable.Rows[0]["n_CompanyId"].ToString());
        //                 Params.Add("N_YearID", MasterTable.Rows[0]["n_FnYearId"].ToString());
        //                 Params.Add("N_FormID", this.N_FormID);

        //                 while (true)
        //                 {
        //                     InvoiceNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
        //                     object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_Purchase Where X_InvoiceNo ='" + values + "' and N_CompanyID= " + nCompanyID, connection, transaction);
        //                     if (N_Result == null)
        //                         break;
        //                 }
        //                 if (InvoiceNo == "")
        //                 {
        //                     // transaction.Rollback();
        //                     // return Ok(_api.Error(User, "Unable to generate Invoice Number"));
        //                     Result.Add("b_IsCompleted", 0);
        //                     Result.Add("x_Msg", "Unable to generate Invoice Number");
        //                     return Result;
        //                 }
        //                 MasterTable.Rows[0]["x_InvoiceNo"] = InvoiceNo;
        //                // object invoiceCount ;
        //                 if(vendorInvoice!="")
        //                 {
        //                      object invoiceCount = dLayer.ExecuteScalar("select count(N_PurchaseID) as Count from inv_purchase where N_CompanyID= " + nCompanyID + " and X_VendorInvoice= '" +vendorInvoice +"' and N_VendorID = " + N_VendorID, connection, transaction);

        //                 if (myFunctions.getIntVAL(invoiceCount.ToString()) >0)
        //                     {
        //                         // transaction.Rollback();
        //                         // return Ok(_api.Error(User, "vendor invoice number already exist"));
        //                         Result.Add("b_IsCompleted", 0);
        //                         Result.Add("x_Msg", "vendor invoice number already exist");
        //                         return Result;
        //                     }
        //                 }
        //             }

        //             if (N_PurchaseID > 0)
        //             {
        //                 // SortedList DeleteParams = new SortedList(){
        //                 //         {"N_CompanyID",masterRow["n_CompanyId"].ToString()},
        //                 //         {"X_TransType","PURCHASE"},
        //                 //         {"N_VoucherID",N_PurchaseID},
        //                 //                                         {"N_UserID",nUserID},
        //                 //         {"X_SystemName","WebRequest"},
        //                 //         {"B_MRNVisible",n_MRNID>0?"1":"0"}};

        //                 // try
        //                 // {
        //                 //     dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", DeleteParams, connection, transaction);
        //                 // }
        //                 // catch (Exception ex)
        //                 // {
        //                 //     transaction.Rollback();
        //                 //     if (ex.Message.Contains("50"))
        //                 //         return Ok(_api.Error(User, "DayClosed"));
        //                 //     else if (ex.Message.Contains("51"))
        //                 //         return Ok(_api.Error(User, "YearClosed"));
        //                 //     else if (ex.Message.Contains("52"))
        //                 //         return Ok(_api.Error(User, "YearExists"));
        //                 //     else if (ex.Message.Contains("53"))
        //                 //         return Ok(_api.Error(User, "PeriodClosed"));
        //                 //     else if (ex.Message.Contains("54"))
        //                 //         return Ok(_api.Error(User, "TxnDate"));
        //                 //     else if (ex.Message.Contains("55"))
        //                 //         return Ok(_api.Error(User, "TransactionStarted"));
        //                 //     return Ok(_api.Error(User, ex.Message));
        //                 // }


        //                 object OPaymentDone = dLayer.ExecuteScalar("SELECT DISTINCT 1	FROM dbo.Inv_PayReceipt INNER JOIN dbo.Inv_PayReceiptDetails ON dbo.Inv_PayReceipt.N_PayReceiptId = dbo.Inv_PayReceiptDetails.N_PayReceiptId AND dbo.Inv_PayReceipt.N_CompanyID = dbo.Inv_PayReceiptDetails.N_CompanyID " +
        //                                                                              " WHERE dbo.Inv_PayReceipt.X_Type='PP' and dbo.Inv_PayReceiptDetails.X_TransType='PURCHASE' and dbo.Inv_PayReceipt.N_CompanyID =" + nCompanyID + " and dbo.Inv_PayReceipt.N_FnYearID=" + nFnYearID + " and  dbo.Inv_PayReceiptDetails.N_InventoryId=" + N_PurchaseID, connection, transaction);
        //                 if (OPaymentDone != null)
        //                 {
        //                     if (myFunctions.getIntVAL(OPaymentDone.ToString()) == 1)
        //                     {
        //                         // transaction.Rollback();
        //                         // return Ok(_api.Error(User, "Purchase Payment processed against this purchase."));
        //                         Result.Add("b_IsCompleted", 0);
        //                         Result.Add("x_Msg", "Purchase Payment processed against this purchase.");
        //                         return Result;
        //                     }
        //                 }

        //                 object OReturnDone = dLayer.ExecuteScalar("SELECT DISTINCT 1 FROM Inv_Purchase INNER JOIN Inv_PurchaseReturnMaster ON Inv_Purchase.N_CompanyID = Inv_PurchaseReturnMaster.N_CompanyID AND Inv_Purchase.N_FnYearID = Inv_PurchaseReturnMaster.N_FnYearID AND Inv_Purchase.N_PurchaseID = Inv_PurchaseReturnMaster.N_PurchaseId " +
        //                                                                             " where dbo.Inv_PurchaseReturnMaster.N_CompanyID =" + nCompanyID + " and dbo.Inv_PurchaseReturnMaster.N_FnYearID=" + nFnYearID + " and  dbo.Inv_PurchaseReturnMaster.N_PurchaseId=" + N_PurchaseID, connection, transaction);

        //                 if (OReturnDone != null)
        //                 {
        //                     if (myFunctions.getIntVAL(OReturnDone.ToString()) == 1)
        //                     {
        //                         // transaction.Rollback();
        //                         // return Ok(_api.Error(User, "Purchase Return processed against this purchase."));
        //                         Result.Add("b_IsCompleted", 0);
        //                         Result.Add("x_Msg", "Purchase Return processed against this purchase.");
        //                         return Result;
        //                     }
        //                 }

        //                 dLayer.ExecuteNonQuery(" delete from Acc_VoucherDetails Where N_CompanyID=" + nCompanyID + " and X_VoucherNo='" + values + "' and N_FnYearID=" + nFnYearID + " and X_TransType = 'PURCHASE'", connection, transaction);
        //                 dLayer.ExecuteNonQuery("Delete FROM Inv_PurchaseFreights WHERE N_PurchaseID = " + N_PurchaseID + " and N_CompanyID = " + nCompanyID, connection, transaction);
        //                 dLayer.ExecuteNonQuery("Delete from Inv_PurchaseWarranty where N_PurchaseID=" + N_PurchaseID + " and N_CompanyID=" + nCompanyID, connection, transaction);
        //                 dLayer.ExecuteNonQuery("Delete from Inv_PurchaseDetails where N_PurchaseID=" + N_PurchaseID + " and N_CompanyID=" + nCompanyID, connection, transaction);
        //                 dLayer.ExecuteNonQuery(" Delete From Inv_Purchase Where (N_PurchaseID = " + N_PurchaseID + " OR (N_PurchaseType =4 and N_PurchaseRefID =  " + N_PurchaseID + ")) and N_CompanyID = " + nCompanyID, connection, transaction);
        //                 dLayer.ExecuteNonQuery("Delete From Inv_Purchase Where (N_PurchaseID = " + N_PurchaseID + " OR (N_PurchaseType =5 and N_PurchaseRefID =  " + N_PurchaseID + ")) and N_CompanyID = " + nCompanyID, connection, transaction);

        //                 dLayer.ExecuteNonQuery("Delete from Inv_Purchase where N_PurchaseID=" + N_PurchaseID + " and N_CompanyID=" + nCompanyID, connection, transaction);
        //             }
        //             MasterTable.Rows[0]["n_userID"] = myFunctions.GetUserID(User);

        //             MasterTable.AcceptChanges();

        //             MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);

        //             if (MasterTable.Columns.Contains("n_TaxAmtDisp"))
        //                 MasterTable.Columns.Remove("n_TaxAmtDisp");

        //             if (MasterTable.Columns.Contains("N_ActVendorID"))
        //             {
        //                 MasterTable.Rows[0]["N_ActVendorID"] = N_VendorID;
        //             }
        //             else
        //             {
        //                 MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_ActVendorID", typeof(int), N_VendorID);
        //             }

        //             N_PurchaseID = dLayer.SaveData("Inv_Purchase", "N_PurchaseID", MasterTable, connection, transaction);

        //             if (N_PurchaseID <= 0)
        //             {
        //                 // transaction.Rollback();
        //                 // return Ok(_api.Error(User, "Unable to save Purchase Invoice!"));
        //                 Result.Add("b_IsCompleted", 0);
        //                 Result.Add("x_Msg", "Unable to save Purchase Invoice!");
        //                 return Result;
        //             }

        //             if (B_isImport)
        //             {
        //                 foreach (DataColumn col in DetailsToImport.Columns)
        //                 {
        //                     col.ColumnName = col.ColumnName.Replace(" ", "_");
        //                     col.ColumnName = col.ColumnName.Replace("*", "");
        //                     col.ColumnName = col.ColumnName.Replace("/", "_");
        //                 }

        //                 DetailsToImport.Columns.Add("N_MasterID");
        //                 DetailsToImport.Columns.Add("X_Type");
        //                 DetailsToImport.Columns.Add("N_CompanyID");
        //                 DetailsToImport.Columns.Add("PkeyID");
        //                 foreach (DataRow dtRow in DetailsToImport.Rows)
        //                 {
        //                     dtRow["N_MasterID"] = N_PurchaseID;
        //                     dtRow["N_CompanyID"] = nCompanyID;
        //                     dtRow["PkeyID"] = 0;
        //                 }

        //                 dLayer.ExecuteNonQuery("delete from Mig_Purchase ", connection, transaction);
        //                 dLayer.SaveData("Mig_Purchase", "PkeyID", "", "", DetailsToImport, connection, transaction);

        //                 SortedList ProParam = new SortedList();
        //                 ProParam.Add("N_CompanyID", nCompanyID);
        //                 ProParam.Add("N_PKeyID", N_PurchaseID);

        //                 SortedList ValidationParam = new SortedList();
        //                 ValidationParam.Add("N_CompanyID", nCompanyID);
        //                 ValidationParam.Add("N_FnYearID", nFnYearID);
        //                 ValidationParam.Add("X_Type", "purchase invoice");
        //                 try
        //                 {
        //                     dLayer.ExecuteNonQueryPro("SP_SetupData_Validation", ValidationParam, connection, transaction);
        //                 }
        //                 catch (Exception ex)
        //                 {
        //                     // transaction.Rollback();
        //                     // return Ok(_api.Error(User, ex));
        //                     Result.Add("b_IsCompleted", 0);
        //                     Result.Add("x_Msg", ex);
        //                     return Result;
        //                 }

        //                 ProParam.Add("X_Type", "purchase invoice");
        //                 DetailTable = dLayer.ExecuteDataTablePro("SP_ScreenDataImport", ProParam, connection, transaction);
        //             }

        //             N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "PURCHASE", N_PurchaseID, InvoiceNo, 1, objVendorName.ToString(), 0, "", 0, User, dLayer, connection, transaction);
        //             N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_Purchase where N_PurchaseID=" + N_PurchaseID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, connection, transaction).ToString());

        //             for (int j = 0; j < DetailTable.Rows.Count; j++)
        //             {
        //                 int UnitID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_ItemUnitID from inv_itemunit where N_ItemID=" + myFunctions.getIntVAL(DetailTable.Rows[j]["N_ItemID"].ToString()) + " and N_CompanyID=" + myFunctions.getIntVAL(DetailTable.Rows[j]["N_CompanyID"].ToString()) + " and X_ItemUnit='" + DetailTable.Rows[j]["X_ItemUnit"].ToString() + "'", connection, transaction).ToString());
        //                 DetailTable.Rows[j]["N_PurchaseID"] = N_PurchaseID;
        //                 DetailTable.Rows[j]["N_ItemUnitID"] = UnitID;
        //             }
        //             if (DetailTable.Columns.Contains("X_ItemUnit"))
        //                 DetailTable.Columns.Remove("X_ItemUnit");
        //             int N_InvoiceDetailId = 0;
        //             DataTable DetailTableCopy = DetailTable.Copy();
        //             DetailTableCopy.AcceptChanges();
        //             if (DetailTable.Columns.Contains("n_MRNDetailsID"))
        //                 DetailTable.Columns.Remove("n_MRNDetailsID");
        //             for (int j = 0; j < DetailTable.Rows.Count; j++)
        //             {
        //                 N_InvoiceDetailId = dLayer.SaveDataWithIndex("Inv_PurchaseDetails", "n_PurchaseDetailsID", "", "", j, DetailTable, connection, transaction);
        //                 if (N_InvoiceDetailId <= 0)
        //                 {
        //                     // transaction.Rollback();
        //                     // return Ok(_api.Error(User, "Unable to save Purchase Invoice!"));
        //                     Result.Add("b_IsCompleted", 0);
        //                     Result.Add("x_Msg", "Unable to save Purchase Invoice!");
        //                     return Result;
        //                 }

        //                 if (n_MRNID > 0 && B_MRNVisible)
        //                 {
        //                     dLayer.ExecuteScalar("Update Inv_MRNDetails Set N_SPrice=" + myFunctions.getVAL(DetailTableCopy.Rows[j]["N_PPrice"].ToString()) + ",N_PurchaseDetailsID=" + N_InvoiceDetailId + " Where N_ItemID=" + myFunctions.getIntVAL(DetailTableCopy.Rows[j]["N_ItemID"].ToString()) + "  and N_MRNID=" + myFunctions.getVAL(DetailTableCopy.Rows[j]["n_RsID"].ToString()) + " and N_CompanyID=" + nCompanyID + " and N_MRNDetailsID=" + myFunctions.getVAL(DetailTableCopy.Rows[j]["n_MRNDetailsID"].ToString()), connection, transaction);
        //                     dLayer.ExecuteScalar("Update Inv_MRN Set N_Processed = 1 Where  N_MRNID=" + myFunctions.getVAL(DetailTableCopy.Rows[j]["n_RsID"].ToString()) + " and N_CompanyID=" + nCompanyID, connection, transaction);

        //                     SortedList UpdateStockParam = new SortedList();
        //                     UpdateStockParam.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
        //                     UpdateStockParam.Add("N_MRNID", n_MRNID);
        //                     UpdateStockParam.Add("N_ItemID", myFunctions.getIntVAL(DetailTableCopy.Rows[j]["N_ItemID"].ToString()));
        //                     UpdateStockParam.Add("N_SPrice", myFunctions.getVAL(DetailTableCopy.Rows[j]["N_PPrice"].ToString()));

        //                     dLayer.ExecuteNonQueryPro("[SP_UpdateStock_MRN]", UpdateStockParam, connection, transaction);

        //                 }
        //                 if(WarrantyInfo!=null)
        //                 foreach (DataRow dtWarranty in WarrantyInfo.Rows)
        //                 {
        //                     if(myFunctions.getIntVAL(dtWarranty["rowID"].ToString())==j)
        //                     dtWarranty["N_PurchaseID"] = N_PurchaseID;
        //                     dtWarranty["N_PurchaseDetailID"] = N_InvoiceDetailId;
        //                 }


        //             }

        //             if(WarrantyInfo!=null && WarrantyInfo.Rows.Count>0){
        //             if (WarrantyInfo.Columns.Contains("rowID"))
        //                 WarrantyInfo.Columns.Remove("rowID");

        //             dLayer.SaveData("Inv_PurchaseWarranty", "N_WarrantyID", WarrantyInfo, connection, transaction);
        //             }

        //             if (N_PurchaseID > 0)
        //             {
        //                 dLayer.ExecuteScalar("Update Inv_PurchaseOrder Set N_Processed=1 , N_PurchaseID=" + N_PurchaseID + " Where N_POrderID=" + n_POrderID + " and N_CompanyID=" + nCompanyID, connection, transaction);
        //                 dLayer.ExecuteScalar("Update Inv_MRN Set N_Processed=1 Where N_MRNID=" + n_MRNID + " and N_CompanyID=" + nCompanyID, connection, transaction);
        //                 // if (B_ServiceSheet)
        //                 //     dba.ExecuteNonQuery("Update Inv_VendorServiceSheet Set N_Processed=1  Where N_RefID=" + n_POrderID + " and N_FnYearID=" + nFnYearID + " and N_CompanyID=" + nCompanyID,connection,transaction);

        //             }

        //             if (PurchaseFreight.Rows.Count > 0)
        //             {
        //                 if (!PurchaseFreight.Columns.Contains("N_PurchaseID"))
        //                 {
        //                     PurchaseFreight.Columns.Add("N_PurchaseID");
        //                 }
        //                 foreach (DataRow var in PurchaseFreight.Rows)
        //                 {
        //                     var["N_PurchaseID"] = N_PurchaseID;
        //                 }
        //                 dLayer.SaveData("Inv_PurchaseFreights", "N_PurchaseFreightID", PurchaseFreight, connection, transaction);
        //             }



        //             if (b_FreightAmountDirect == 0)
        //             {
        //                 SortedList ProcParams = new SortedList(){
        //                     {"N_FPurchaseID", N_PurchaseID},
        //                     {"N_CompanyID", nCompanyID},
        //                     {"N_FnYearID", nFnYearID},
        //                     {"N_FormID", this.N_FormID},
        //                 };
        //                 dLayer.ExecuteNonQueryPro("SP_FillFreightToPurchase", ProcParams, connection, transaction);
        //             }

        //             if (N_SaveDraft == 0)
        //             {
        //                 try
        //                 {
        //                     // SortedList PostingMRNParam = new SortedList();
        //                     // PostingMRNParam.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
        //                     // PostingMRNParam.Add("N_PurchaseID", N_PurchaseID);
        //                     // PostingMRNParam.Add("N_UserID", nUserID);
        //                     // PostingMRNParam.Add("X_SystemName", "ERP Cloud");
        //                     // PostingMRNParam.Add("X_UseMRN", "");
        //                     // PostingMRNParam.Add("N_SaveDraft", N_SaveDraft);
        //                     // PostingMRNParam.Add("N_MRNID", 0);

        //                     // dLayer.ExecuteNonQueryPro("[SP_Inv_MRNposting]", PostingMRNParam, connection, transaction);

        //                     SortedList PostingMRNParam = new SortedList();
        //                     PostingMRNParam.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
        //                     PostingMRNParam.Add("N_PurchaseID", N_PurchaseID);
        //                     PostingMRNParam.Add("N_UserID", nUserID);
        //                     PostingMRNParam.Add("X_SystemName", "ERP Cloud");
        //                     PostingMRNParam.Add("X_UseMRN", "");
        //                     PostingMRNParam.Add("N_SaveDraft", N_SaveDraft);
        //                     PostingMRNParam.Add("B_DirectPurchase", Dir_Purchase);
        //                     PostingMRNParam.Add("N_MRNID", n_MRNID);

        //                     dLayer.ExecuteNonQueryPro("[SP_Inv_MRNprocessing]", PostingMRNParam, connection, transaction);

        //                     SortedList PostingParam = new SortedList();
        //                     PostingParam.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
        //                     PostingParam.Add("X_InventoryMode", "PURCHASE");
        //                     PostingParam.Add("N_InternalID", N_PurchaseID);
        //                     PostingParam.Add("N_UserID", nUserID);
        //                     PostingParam.Add("X_SystemName", "ERP Cloud");
        //                     PostingParam.Add("MRN_Flag", Dir_Purchase==0 ? "1" : "0");

        //                     dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Purchase_Posting", PostingParam, connection, transaction);
        //                 }
        //                 catch (Exception ex)
        //                 {
        //                     // transaction.Rollback();
        //                     // if (ex.Message.Contains("50"))
        //                     //     return Ok(_api.Error(User, "Day Closed"));
        //                     // else if (ex.Message.Contains("51"))
        //                     //     return Ok(_api.Error(User, "Year Closed"));
        //                     // else if (ex.Message.Contains("52"))
        //                     //     return Ok(_api.Error(User, "Year Exists"));
        //                     // else if (ex.Message.Contains("53"))
        //                     //     return Ok(_api.Error(User, "Period Closed"));
        //                     // else if (ex.Message.Contains("54"))
        //                     //     return Ok(_api.Error(User, "Wrong Txn Date"));
        //                     // else if (ex.Message.Contains("55"))
        //                     //     return Ok(_api.Error(User, "Transaction Started"));
        //                     // return Ok(_api.Error(User, ex.Message));

        //                     Result.Add("b_IsCompleted", 0);
        //                     if (ex.Message.Contains("50"))
        //                         Result.Add("x_Msg", "Day Closed");
        //                     else if (ex.Message.Contains("51"))
        //                         Result.Add("x_Msg", "Year Closed");
        //                     else if (ex.Message.Contains("52"))
        //                         Result.Add("x_Msg", "Year Exists");
        //                     else if (ex.Message.Contains("53"))
        //                         Result.Add("x_Msg", "Period Closed");
        //                     else if (ex.Message.Contains("54"))
        //                         Result.Add("x_Msg", "Wrong Txn Date");
        //                     else if (ex.Message.Contains("55"))
        //                         Result.Add("x_Msg", "Transaction Started");
        //                     else
        //                         Result.Add("x_Msg", ex.Message);
        //                     return Result;
        //                 }

        //                     //StatusUpdate
        //                     int tempPOrderID=0;
        //                     for (int j = 0; j < DetailTable.Rows.Count; j++)
        //                     {
        //                         if (n_POrderID > 0 && tempPOrderID!=n_POrderID)
        //                         {
        //                             if(!myFunctions.UpdateTxnStatus(nCompanyID,n_POrderID,82,false,dLayer,connection,transaction))
        //                             {
        //                                 // xturn Ok(_api.Error(User, "Unable To Update Txn Status"));

        //                                 Result.Add("b_IsCompleted", 0);
        //                                 Result.Add("x_Msg", "Unable To Update Txn Status");
        //                                 return Result;
        //                             }
        //                         }
        //                         tempPOrderID=n_POrderID;
        //                     };
        //             }
        //             SortedList VendorParams = new SortedList();
        //             VendorParams.Add("@nVendorID", N_VendorID);
        //             DataTable VendorInfo = dLayer.ExecuteDataTable("Select X_VendorCode,X_VendorName from Inv_Vendor where N_VendorID=@nVendorID", VendorParams, connection, transaction);
        //             if (VendorInfo.Rows.Count > 0)
        //             {
        //                 try
        //                 {
        //                     myAttachments.SaveAttachment(dLayer, Attachment, InvoiceNo, N_PurchaseID, VendorInfo.Rows[0]["X_VendorName"].ToString().Trim(), VendorInfo.Rows[0]["X_VendorCode"].ToString(), N_VendorID, "Vendor Document", User, connection, transaction);
        //                 }
        //                 catch (Exception ex)
        //                 {
        //                     // transaction.Rollback();
        //                     // return Ok(_api.Error(User, ex));
        //                     Result.Add("b_IsCompleted", 0);
        //                     Result.Add("x_Msg", ex);
        //                     return Result;
        //                 }
        //             }
        //             Result.Add("b_IsCompleted", 1);
        //             Result.Add("x_Msg", "Purchase Invoice Saved");
        //             return Result;
        //             //myFunctions.SendApprovalMail(N_NextApproverID, this.N_FormID, N_PurchaseID, "PURCHASE", InvoiceNo, dLayer, connection, transaction, User);

        //       //      transaction.Commit();
        //     //     }
        //     //     SortedList Result = new SortedList();
        //     //     Result.Add("n_InvoiceID", N_PurchaseID);
        //     //     Result.Add("x_InvoiceNo", InvoiceNo);
        //     //     return Ok(_api.Success(Result, "Purchase Invoice Saved"));


        //     // }



        //     // catch (Exception ex)
        //     // {
        //     //     return Ok(_api.Error(User, ex));
        //     // }
        // }
        private bool CheckProcessed(int nPurchaseID)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            object AdvancePRProcessed = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlCommand = "Select COUNT(N_TransID) From Inv_PaymentRequest Where  N_CompanyID=@p1 and N_TransID=@p2 and N_FormID=65";
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", nPurchaseID);
                AdvancePRProcessed = dLayer.ExecuteScalar(sqlCommand, Params, connection);

            }
            if (AdvancePRProcessed != null)
            {
                if (myFunctions.getIntVAL(AdvancePRProcessed.ToString()) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        //Delete....
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPurchaseID, int nFnYearID, string comments, int nMRNID)
        {
            if (comments == null)
            {
                comments = "";
            }
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            int Results = 0;
            // if (CheckProcessed(nPurchaseID))
            //     return Ok(_api.Error(User, "Transaction Started"));
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nTransID", nPurchaseID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,isNull(N_VendorID,0) as N_VendorID,X_InvoiceNo,N_FormID from Inv_Purchase where N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID and N_PurchaseID=@nTransID";
                    string xButtonAction = "Delete";
                    string X_InvoiceNo = "";

                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    int VendorID = myFunctions.getIntVAL(TransRow["N_VendorID"].ToString());

                    SortedList VendParams = new SortedList();
                    VendParams.Add("@nCompanyID", nCompanyID);
                    VendParams.Add("@N_VendorID", VendorID);
                    VendParams.Add("@nFnYearID", nFnYearID);
                    object objVendorName = dLayer.ExecuteScalar("Select X_VendorName From Inv_Vendor where N_VendorID=@N_VendorID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", VendParams, connection);

                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, this.N_FormID, nPurchaseID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID, 0, 0, User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), comments);
                    SqlTransaction transaction = connection.BeginTransaction();

                    string X_Criteria = "N_PurchaseID=" + nPurchaseID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnYearID;
                    string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());

                    object mrnCount = dLayer.ExecuteScalar("SELECT count(Sec_UserPrevileges.N_MenuID) as Count FROM Sec_UserPrevileges INNER JOIN Sec_UserCategory ON Sec_UserPrevileges.N_UserCategoryID = Sec_UserCategory.N_UserCategoryID and Sec_UserPrevileges.N_MenuID=555 and Sec_UserCategory.N_CompanyID=" + nCompanyID + " and Sec_UserPrevileges.B_Visible=1", connection, transaction);
                    bool B_MRNVisible = myFunctions.getIntVAL(mrnCount.ToString()) > 0 ? true : false;
                    string status = myFunctions.UpdateApprovals(Approvals, nFnYearID, "PURCHASE", nPurchaseID, TransRow["X_InvoiceNo"].ToString(), ProcStatus, "Inv_Purchase", X_Criteria, objVendorName.ToString(), User, dLayer, connection, transaction);
                    bool B_isDirectMRN = false;
                    if (nMRNID > 0)
                        B_isDirectMRN = myFunctions.getBoolVAL(dLayer.ExecuteScalar("SELECT B_isDirectMRN from Inv_MRN where N_CompanyID=" + nCompanyID + " and N_MRNID = " + nMRNID, connection, transaction).ToString());

                   string sqlPaymentRequestID = "select N_PaymentRequestID from Inv_Purchase where N_PurchaseID=@nTransID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID";
                    object reqID = dLayer.ExecuteScalar(sqlPaymentRequestID, ParamList, connection,transaction);
                    int PaymentRequestID = 0;
                    if (reqID != null)
                        PaymentRequestID = myFunctions.getIntVAL(reqID.ToString());
                    

                    if (status != "Error")
                    {
                        if (ButtonTag == "6" || ButtonTag == "0")
                        {

                            SortedList StockUpdateParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"N_TransID",nMRNID},
                                {"X_TransType", "PURCHASE"}};

                            dLayer.ExecuteNonQueryPro("SP_StockDeleteUpdate", StockUpdateParams, connection, transaction);

                            //Activity Log
                            string ipAddress = "";
                            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                                ipAddress = Request.Headers["X-Forwarded-For"];
                            else
                                ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                            myFunctions.LogScreenActivitys(myFunctions.getIntVAL(nFnYearID.ToString()), nPurchaseID, TransRow["X_InvoiceNo"].ToString(), 65, xButtonAction, ipAddress, "", User, dLayer, connection, transaction);

                            SortedList DeleteParams = new SortedList(){
                                    {"N_CompanyID",nCompanyID},
                                    {"X_TransType","PURCHASE"},
                                    {"N_VoucherID",nPurchaseID},
                                    {"N_UserID",nUserID},
                                    {"X_SystemName","WebRequest"},
                                    {"B_MRNVisible",(B_isDirectMRN && B_MRNVisible) ?"1":"0"}};
                            //{"B_MRNVisible",n_MRNID>0?"1":"0"}};
                            DataTable DetailTable = dLayer.ExecuteDataTable("select N_POrderID from Inv_PurchaseDetails where N_CompanyID=@nCompanyID and N_PurchaseID=@nTransID group by N_POrderID order by N_POrderID", ParamList, connection, transaction);
                            DataTable DetailsTable = dLayer.ExecuteDataTable("select N_CompanyID,N_ItemID from Inv_PurchaseDetails where N_CompanyID=@nCompanyID and N_PurchaseID=@nTransID group by N_CompanyID,N_ItemID ", ParamList, connection, transaction);

                            // Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", DeleteParams, connection, transaction);
                            // if (Results <= 0)
                            // {
                            //     transaction.Rollback();
                            //     return Ok(_api.Error(User, "Unable to Delete PurchaseInvoice"));
                            // } 
                            try
                            {
                                Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", DeleteParams, connection, transaction);

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
                                    return Ok(_api.Error(User, "Unable to delete Purchase Invoice"));

                                }

                            }

                            myAttachments.DeleteAttachment(dLayer, 1, nPurchaseID, VendorID, nFnYearID, N_FormID, User, transaction, connection);



                            SortedList StockOutParam = new SortedList();
                            StockOutParam.Add("N_CompanyID", nCompanyID);

                            dLayer.ExecuteNonQueryPro("SP_StockOutUpdate", StockOutParam, connection, transaction);

                            for (int i = 0; i < DetailTable.Rows.Count; i++)
                            {
                                dLayer.ExecuteScalar("UPDATE Inv_ItemMaster SET Inv_ItemMaster.N_PurchaseCost=LastCost.N_LPrice from Inv_ItemMaster INNER JOIN " +
                                               " (select TOP 1 N_CompanyID,N_ItemID,N_LPrice from Inv_StockMaster where X_Type='Purchase' and N_ItemID=" + myFunctions.getVAL(DetailsTable.Rows[i]["N_ItemID"].ToString()) + " " +
                                               " AND N_CompanyID= " + myFunctions.getVAL(DetailsTable.Rows[i]["N_CompanyID"].ToString()) + " order by D_DateIn desc ,N_StockID desc) AS LastCost ON Inv_ItemMaster.N_CompanyID=LastCost.N_CompanyID AND " +
                                               " Inv_ItemMaster.N_ItemID=LastCost.N_ItemID WHERE Inv_ItemMaster.N_CompanyID=" + myFunctions.getVAL(DetailsTable.Rows[i]["N_CompanyID"].ToString()) + " AND Inv_ItemMaster.N_ItemID= " + myFunctions.getVAL(DetailsTable.Rows[i]["N_ItemID"].ToString()), connection, transaction);
                            }
                            //StatusUpdate
                            int tempPOrderID = 0;
                            for (int j = 0; j < DetailTable.Rows.Count; j++)
                            {
                                int n_POrderID = myFunctions.getIntVAL(DetailTable.Rows[j]["N_POrderID"].ToString());
                                if (n_POrderID > 0 && tempPOrderID != n_POrderID)
                                {
                                    // if(!myFunctions.UpdateTxnStatus(nCompanyID,n_POrderID,82,true,dLayer,connection,transaction))
                                    // {
                                    //     transaction.Rollback();
                                    //     return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                    // }

                                    if (myFunctions.getIntVAL(TransRow["n_FormID"].ToString()) == 1605)
                                    {
                                        if (!myFunctions.UpdateTxnStatus(nCompanyID, n_POrderID, 1586, false, dLayer, connection, transaction))
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
                                tempPOrderID = n_POrderID;
                            };
                               if (PaymentRequestID > 0)
                                    {
                                        if (!myFunctions.UpdateTxnStatus(nCompanyID, PaymentRequestID, 1844, false, dLayer, connection, transaction))
                                        {
                                            transaction.Rollback();
                                            return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                        }
                                    }
                        }
                        else if (ButtonTag == "4")
                        {
                            dLayer.ExecuteNonQuery("delete from Acc_VoucherDetails_Segments where N_CompanyID=@nCompanyID AND N_FnYearID=@nFnYearID and X_TransType='PURCHASE' AND N_AccTransID  in (select N_AccTransID from Acc_VoucherDetails where N_CompanyID=@nCompanyID AND N_FnYearID=@nFnYearID and X_TransType='PURCHASE' AND X_VoucherNo='"+TransRow["X_InvoiceNo"].ToString()+"')", ParamList, connection, transaction);
                            dLayer.ExecuteNonQuery("delete from Acc_VoucherDetails where N_CompanyID=@nCompanyID AND N_FnYearID=@nFnYearID and X_TransType='PURCHASE' AND X_VoucherNo='"+TransRow["X_InvoiceNo"].ToString()+"'", ParamList, connection, transaction);
                        }

                        transaction.Commit();
                        if (myFunctions.getIntVAL(TransRow["n_FormID"].ToString()) == 1605)
                        {
                             return Ok(_api.Success("Rental Purchase Invoice " + status + " Successfully"));
                        }
                        else
                        {
                             return Ok(_api.Success("Purchase Invoice " + status + " Successfully"));

                        }
                    }
                   
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete Purchase Invoice"));
                    }

                    // transaction.Commit();
                    // return Ok(_api.Success("Purchase invoice deleted"));

                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }


        }
        [HttpGet("pendingGRN")]
        public ActionResult ProductList(int nFnYearID, int nVendorID, bool bAllbranchData, int nBranchID,int nDivisionID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);

            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nVendorID", nVendorID);
            Params.Add("@nDivisionID", nDivisionID);


            string sqlCommandText = "";
            if (bAllbranchData && nDivisionID > 0)
                sqlCommandText = "Select N_MRNID,X_MRNNo,D_MRNDate,X_VendorName,N_CompanyID,N_VendorID,X_VendorInvoice,N_DivisionID from vw_Inv_PendingPurchases_rpt  Where N_CompanyID=@nCompanyID and N_VendorID=@nVendorID and N_DivisionID=@nDivisionID  GROUP BY N_MRNID,X_MRNNO,D_MRNDate,X_VendorName,N_CompanyID,N_VendorID,X_VendorInvoice,N_DivisionID ";
            else if (bAllbranchData)
                sqlCommandText = "Select N_MRNID,X_MRNNo,D_MRNDate,X_VendorName,N_CompanyID,N_VendorID,X_VendorInvoice from vw_Inv_PendingPurchases_rpt  Where N_CompanyID=@nCompanyID and N_VendorID=@nVendorID  GROUP BY N_MRNID,X_MRNNO,D_MRNDate,X_VendorName,N_CompanyID,N_VendorID,X_VendorInvoice ";
            else
                sqlCommandText = "Select N_MRNID,X_MRNNo,D_MRNDate,X_VendorName,N_CompanyID,N_VendorID,X_VendorInvoice from vw_Inv_PendingPurchases_rpt  Where N_CompanyID=@nCompanyID  and N_VendorID=@nVendorID  GROUP BY N_MRNID,X_MRNNO,D_MRNDate,X_VendorName,N_CompanyID,N_VendorID,X_VendorInvoice ";

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


        [HttpGet("warrantyDashboardList")]
        public ActionResult WarrantyDashboardList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
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
                    string criteria = "";
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nUserID = myFunctions.GetUserID(User);





                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and (x_InvoiceNo like '%" + xSearchkey + "%' or X_VendorName like '%" + xSearchkey + "%' or x_ItemName like '%" + xSearchkey + "%' or x_ItemCode like '%" + xSearchkey + "%' or d_InvoiceDate like '%" + xSearchkey + "%' or x_WarrantyDescription like '%" + xSearchkey + "%' )";


                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_PurchaseID desc";
                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "x_InvoiceNo":
                                xSortBy = "N_PurchaseID " + xSortBy.Split(" ")[1];
                                break;
                            case "d_WarrantyFrom":
                                xSortBy = "Cast([d_WarrantyFrom] as DateTime ) " + xSortBy.Split(" ")[1];
                                break;
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }


                    int Count = (nPage - 1) * nSizeperpage;
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_Inv_PurchaseWarranty where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearId " + criteria + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_Inv_PurchaseWarranty where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearId " + criteria + Searchkey + " and  N_WarrantyId not in (select top(" + Count + ") N_WarrantyId from Vw_Inv_PurchaseWarranty where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearId " + criteria + Searchkey + xSortBy + " ) " + xSortBy;

                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nFnYearId", nFnYearId);
                    SortedList OutPut = new SortedList();


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);



                    sqlCommandCount = "select count(1) as N_Count,0 as TotalAmount from Vw_Inv_PurchaseWarranty where  N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearId " + criteria + " " + Searchkey + "";
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


                    return Ok(_api.Success(OutPut));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }



    }


}