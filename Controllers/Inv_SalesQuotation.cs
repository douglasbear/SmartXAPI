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
    [Route("salesquotation")]
    [ApiController]
    public class Inv_SalesQuotation : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int FormID;

        public Inv_SalesQuotation(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 80;
        }


        [HttpGet("list")]
        public ActionResult GetSalesQuotationList(int? nCompanyId, int nFnYearId, bool bAllBranchData, int nBranchID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
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
                    bool CheckClosedYear = Convert.ToBoolean(dLayer.ExecuteScalar("Select B_YearEndProcess From Acc_FnYear Where N_CompanyID=" + nCompanyId + " and N_FnYearID = " + nFnYearId, Params, connection));
                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and ([Quotation No] like '%" + xSearchkey + "%' or X_CustomerName like '%" + xSearchkey + "%' or X_SalesmanName like '%" + xSearchkey + "%')";

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_QuotationId desc";
                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {

                            case "quotationDate":
                                xSortBy = "Cast([Quotation Date] as DateTime )" + xSortBy.Split(" ")[1];
                                break;
                            case "d_RfqRefDate":
                                xSortBy = "Cast(d_RfqRefDate as DateTime )" + xSortBy.Split(" ")[1];
                                break;
                            case "quotationNo":
                                xSortBy = "N_QuotationId " + xSortBy.Split(" ")[1];
                                break;
                            case "n_AmountF":
                                xSortBy = "Cast(REPLACE(n_AmountF,',','') as Numeric(10,2)) " + xSortBy.Split(" ")[1];
                                break;    
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }
                    if (CheckClosedYear == false)
                    {
                        if (bAllBranchData == true)
                        {
                            Searchkey = Searchkey + " and  N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " ";
                        }
                        else
                        {
                            Searchkey = Searchkey + " and  N_CompanyID=" + nCompanyId + " and N_BranchID=" + nBranchID + " and N_FnYearID=" + nFnYearId + " and B_YearEndProcess =0";
                        }
                    }
                    else
                    {
                        if (bAllBranchData == true)
                        {
                            Searchkey = Searchkey + " and  N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " ";
                        }
                        else
                        {
                            Searchkey = Searchkey + " and  N_CompanyID=" + nCompanyId + " and N_BranchID=" + nBranchID + " and N_FnYearID=" + nFnYearId + " ";
                        }
                    }

                    if (Count == 0)

                        sqlCommandText = "select top(" + nSizeperpage + ") N_QuotationId,[Quotation No],[Quotation Date],N_CompanyId,N_CustomerId,[Customer Code],N_FnYearID,D_QuotationDate,N_BranchId,B_YearEndProcess,X_CustomerName,X_BranchName,X_RfqRefNo,D_RfqRefDate,N_Amount,N_FreightAmt,N_DiscountAmt,N_Processed,N_OthTaxAmt,N_BillAmt,N_ProjectID,X_ProjectName,x_Notes,X_SalesmanName,N_AmountF,N_DiscountAmtF,N_BillAmtF from vw_InvSalesQuotationNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") N_QuotationId,[Quotation No],[Quotation Date],N_CompanyId,N_CustomerId,[Customer Code],N_FnYearID,D_QuotationDate,N_BranchId,B_YearEndProcess,X_CustomerName,X_BranchName,X_RfqRefNo as XRfqRefNo,D_RfqRefDate as DRfqRefDate,N_Amount as NAmount,N_FreightAmt as NFreightAmt,N_DiscountAmt,N_Processed,N_OthTaxAmt,N_BillAmt,N_ProjectID,X_ProjectName,x_Notes,X_SalesmanName,N_AmountF,N_DiscountAmtF,N_BillAmtF from vw_InvSalesQuotationNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and N_QuotationId not in (select top(" + Count + ") N_QuotationId from vw_InvSalesQuotationNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + xSortBy + " ) " + xSortBy;



                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearId);
                    SortedList OutPut = new SortedList();


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count,sum(Cast(REPLACE(n_Amount,',','') as Numeric(10,2)) ) as TotalAmount from vw_InvSalesQuotationNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "";
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
                return Ok(_api.Error(e));
            }
        }



        [HttpGet("details")]
        public ActionResult GetQuotationDetails(int xQuotationNo, int nFnYearId, bool bAllBranchData, int nBranchID, int n_OpportunityID)
        {
            DataSet dsQuotation = new DataSet();
            DataTable dtProcess = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "";
            int nCompanyId = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyId);
            Params.Add("@nFnYearID", nFnYearId);
            Params.Add("@xQuotationNo", xQuotationNo);
            Params.Add("@nBranchID", nBranchID);
            Params.Add("@n_OpportunityID", n_OpportunityID);

            if (bAllBranchData == true)
            {
                sqlCommandText = "SELECT Inv_SalesQuotation.*, Acc_CurrencyMaster.N_CurrencyID, Acc_CurrencyMaster.X_CurrencyName, CRM_Customer.X_Customer as x_CrmCustomer, CRM_Contact.N_ContactID,CRM_Contact.X_Contact FROM  CRM_Contact RIGHT OUTER JOIN Inv_SalesQuotation ON CRM_Contact.N_ContactID = Inv_SalesQuotation.N_ContactID AND CRM_Contact.N_CompanyId = Inv_SalesQuotation.N_CompanyId LEFT OUTER JOIN CRM_Customer ON Inv_SalesQuotation.N_CrmCompanyID = CRM_Customer.N_CustomerID AND Inv_SalesQuotation.N_CompanyId = CRM_Customer.N_CompanyId LEFT OUTER JOIN Acc_CurrencyMaster RIGHT OUTER JOIN Inv_Customer ON Acc_CurrencyMaster.N_CompanyID = Inv_Customer.N_CompanyID AND Acc_CurrencyMaster.N_CurrencyID = Inv_Customer.N_CurrencyID ON Inv_SalesQuotation.N_CompanyId = Inv_Customer.N_CompanyID AND Inv_SalesQuotation.N_CustomerId = Inv_Customer.N_CustomerID Where Inv_SalesQuotation.N_CompanyID=@nCompanyID and Inv_SalesQuotation.N_FnYearID=@nFnYearID and Inv_SalesQuotation.X_QuotationNo=@xQuotationNo";
            }
            else
            {
                sqlCommandText = "SELECT Inv_SalesQuotation.*, Acc_CurrencyMaster.N_CurrencyID, Acc_CurrencyMaster.X_CurrencyName, CRM_Customer.X_Customer as x_CrmCustomer, CRM_Contact.N_ContactID,CRM_Contact.X_Contact FROM  CRM_Contact RIGHT OUTER JOIN Inv_SalesQuotation ON CRM_Contact.N_ContactID = Inv_SalesQuotation.N_ContactID AND CRM_Contact.N_CompanyId = Inv_SalesQuotation.N_CompanyId LEFT OUTER JOIN CRM_Customer ON Inv_SalesQuotation.N_CrmCompanyID = CRM_Customer.N_CustomerID AND Inv_SalesQuotation.N_CompanyId = CRM_Customer.N_CompanyId LEFT OUTER JOIN Acc_CurrencyMaster RIGHT OUTER JOIN Inv_Customer ON Acc_CurrencyMaster.N_CompanyID = Inv_Customer.N_CompanyID AND Acc_CurrencyMaster.N_CurrencyID = Inv_Customer.N_CurrencyID ON Inv_SalesQuotation.N_CompanyId = Inv_Customer.N_CompanyID AND Inv_SalesQuotation.N_CustomerId = Inv_Customer.N_CustomerID Where Inv_SalesQuotation.N_CompanyID=@nCompanyID and Inv_SalesQuotation.N_FnYearID=@nFnYearID and Inv_SalesQuotation.X_QuotationNo=@xQuotationNo and Inv_SalesQuotation.N_BranchID=@nBranchID";
            }
            if (n_OpportunityID > 0)
                sqlCommandText = "select * from vw_OpportunityToQuotation Where N_CompanyID=@nCompanyID and n_OpportunityID=@n_OpportunityID";

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable Master = new DataTable();
                    Master = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    Master = _api.Format(Master, "Master");
                    if (n_OpportunityID > 0 && myFunctions.getIntVAL(Master.Rows[0]["n_CrmCompanyID"].ToString()) > 0)
                    {
                        object CustomerID = dLayer.ExecuteScalar("select N_CustomerId from Inv_Customer where N_CompanyID=@nCompanyID and N_CrmCompanyID=" + Master.Rows[0]["n_CrmCompanyID"].ToString(), Params, connection);
                        object CustomerName = dLayer.ExecuteScalar("select X_CustomerName from Inv_Customer where N_CompanyID=@nCompanyID and N_CrmCompanyID=" + Master.Rows[0]["n_CrmCompanyID"].ToString(), Params, connection);
                        if (CustomerID != null)
                        {
                            Master.Rows[0]["N_CustomerId"] = CustomerID.ToString();
                            Master.Rows[0]["X_CustomerName"] = CustomerName.ToString();
                        }

                    }
                    //dsQuotation.Tables.Add(dtQuotation);

                    if (Master.Rows.Count == 0)
                        return Ok(_api.Notice("There is no data!"));

                    var nQuotationId = Master.Rows[0]["N_QuotationId"];
                    var nFormID = this.FormID;
                    int nCRMID = myFunctions.getIntVAL(Master.Rows[0]["N_CRMID"].ToString());
                    Master.Rows[0]["N_CRMID"] = nCRMID;
                    if (nCRMID > 0)
                    {
                        object crmcode = dLayer.ExecuteScalar("select X_CRMCode from Inv_CRMMaster where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_BranchID=@nBranchID and N_CRMID=@nCRMID", Params, connection);
                        Master = myFunctions.AddNewColumnToDataTable(Master, "X_CRMCode", typeof(string), crmcode.ToString());
                    }
                    else
                    {
                        Master = myFunctions.AddNewColumnToDataTable(Master, "X_CRMCode", typeof(string), "");
                    }

                    var nLocationID = Master.Rows[0]["N_LocationID"];
                    Params.Add("@nLocationID", nLocationID);
                    object X_LocationName = dLayer.ExecuteScalar("select X_LocationName from Inv_Location where N_CompanyID=@nCompanyID and N_LocationID=@nLocationID", Params, connection);
                    Master = myFunctions.AddNewColumnToDataTable(Master, "X_LocationName", typeof(string), X_LocationName.ToString());

                    int nProjectID = myFunctions.getIntVAL(Master.Rows[0]["N_ProjectID"].ToString());
                    if (nProjectID > 0)
                    {
                        Params.Add("@nProjectID", nProjectID);
                        object xProjectName = dLayer.ExecuteScalar("select X_ProjectName from Inv_CustomerProjects where N_CompanyID=@nCompanyID and N_ProjectID=@nProjectID", Params, connection);
                        Master = myFunctions.AddNewColumnToDataTable(Master, "X_ProjectName", typeof(string), xProjectName.ToString());
                    }
                    else
                    {
                        Master = myFunctions.AddNewColumnToDataTable(Master, "X_ProjectName", typeof(string), "");
                    }


                    int nCustomerID = myFunctions.getIntVAL(Master.Rows[0]["N_CustomerId"].ToString());
                    Params.Add("@nCustomerID", nCustomerID);
                    if (nCustomerID > 0)
                    {
                        string sql = "select X_CustomerCode from Inv_Customer where N_CustomerID=@nCustomerID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and (N_BranchID=0 or N_BranchID=@nBranchID) and B_Inactive = 0";
                        if (bAllBranchData)
                            sql = "select X_CustomerCode from Inv_Customer where N_CustomerID=@nCustomerID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and B_Inactive = 0";
                        object xCustomerCode = dLayer.ExecuteScalar(sql, Params, connection);
                        Master = myFunctions.AddNewColumnToDataTable(Master, "X_CustomerCode", typeof(string), xCustomerCode.ToString());
                    }
                    else
                    {
                        Master = myFunctions.AddNewColumnToDataTable(Master, "X_CustomerCode", typeof(string), "");
                    }

                    int nTaxCategoryID = myFunctions.getIntVAL(Master.Rows[0]["N_TaxCategoryID"].ToString());
                    Params.Add("@nTaxCategoryID", nTaxCategoryID);
                    object X_DisplayName = dLayer.ExecuteScalar("Select X_DisplayName from Acc_TaxCategory where N_PkeyID=@nTaxCategoryID", Params, connection);
                    Master = myFunctions.AddNewColumnToDataTable(Master, "X_DisplayName", typeof(string), X_DisplayName);

                    int N_SalesmanID = myFunctions.getIntVAL(Master.Rows[0]["N_SalesmanID"].ToString());
                    object X_SalesmanName = "", X_SalesmanCode = "";
                    if (N_SalesmanID.ToString() != "" && N_SalesmanID != 0)
                    {
                        Params.Add("@nSalesmanID", N_SalesmanID);
                        X_SalesmanName = dLayer.ExecuteScalar("select X_SalesmanName from Inv_Salesman where N_CompanyID=@nCompanyID and N_SalesmanID=@nSalesmanID", Params, connection);
                        X_SalesmanCode = dLayer.ExecuteScalar("select X_SalesmanCode from Inv_Salesman where N_CompanyID=@nCompanyID and N_SalesmanID=@nSalesmanID", Params, connection);
                    }
                    Master = myFunctions.AddNewColumnToDataTable(Master, "X_SalesmanName", typeof(string), X_SalesmanName.ToString());
                    Master = myFunctions.AddNewColumnToDataTable(Master, "X_SalesmanCode", typeof(string), X_SalesmanCode.ToString());

                    Params.Add("@nQuotationID", nQuotationId);
                    Params.Add("@nFormID", nFormID);

                    object objFollowup = dLayer.ExecuteScalar("Select  isnull(max(N_id),0) from vsa_appointment where n_refid = @nQuotationID and N_companyID=@nCompanyID and B_IsComplete=0", Params, connection);
                    if (objFollowup != null)
                    {
                        Params.Add("@nRefTypeID", 8);
                        DataTable dtFollowUp = new DataTable();
                        string qry = "Select * from vw_QuotaionFollowup Where N_CompanyID=@nCompanyID and N_RefTypeID=@nRefTypeID and N_RefID= @nQuotationID";
                        dtFollowUp = dLayer.ExecuteDataTable(qry, Params, connection);
                        dtFollowUp = _api.Format(dtFollowUp, "FollowUp");
                        dsQuotation.Tables.Add(dtFollowUp);
                    }


                    //object objSalesOrder = dLayer.ExecuteScalar("Select N_SalesOrderID from Inv_SalesOrder Where N_CompanyID=@nCompanyID and N_QuotationID =@nQuotationID and B_IsSaveDraft=0", Params);
                    object objSalesOrder = myFunctions.checkProcessed("Inv_SalesOrder", "N_SalesOrderID", "N_QuotationID", "@nQuotationID", "N_CompanyID=@nCompanyID and B_IsSaveDraft=0", Params, dLayer, connection);
                    Master = myFunctions.AddNewColumnToDataTable(Master, "B_SalesOrderProcessed", typeof(int), 0);
                    Master = myFunctions.AddNewColumnToDataTable(Master, "X_SalesOrderNo", typeof(string), "");
                    Master = myFunctions.AddNewColumnToDataTable(Master, "B_DeliveryNoteProcessed", typeof(int), 0);
                    Master = myFunctions.AddNewColumnToDataTable(Master, "X_DeliveryNoteNo", typeof(string), "");
                    Master = myFunctions.AddNewColumnToDataTable(Master, "B_SalesProcessed", typeof(int), 0);
                    Master = myFunctions.AddNewColumnToDataTable(Master, "X_SalesReceiptNo", typeof(string), "");
                    if (myFunctions.getIntVAL(nQuotationId.ToString()) > 0)
                    {
                        if (objSalesOrder.ToString() != "")
                        {
                            if (myFunctions.getIntVAL(objSalesOrder.ToString()) > 0)
                            {
                                Params.Add("@nSalesOrderID", myFunctions.getIntVAL(objSalesOrder.ToString()));
                                Master.Rows[0]["B_SalesOrderProcessed"] = 1;
                                object objxSalesOrderNo = myFunctions.checkProcessed("Inv_SalesOrder", "X_OrderNo", "N_QuotationID", "@nQuotationID", "N_CompanyID=@nCompanyID and B_IsSaveDraft=0", Params, dLayer, connection);
                                Master.Rows[0]["X_SalesOrderNo"] = objxSalesOrderNo;


                                object objDeliveryNote = myFunctions.checkProcessed("Inv_DeliveryNote", "N_DeliveryNoteID", "N_SalesOrderID", "@nSalesOrderID", "N_CompanyID=@nCompanyID and B_IsSaveDraft=0", Params, dLayer, connection);

                                if (objDeliveryNote.ToString() != "")
                                {
                                    if (myFunctions.getIntVAL(objDeliveryNote.ToString()) > 0)
                                    {
                                        Params.Add("@nDeliveryNoteID", myFunctions.getIntVAL(objDeliveryNote.ToString()));
                                        Master.Rows[0]["B_DeliveryNoteProcessed"] = 1;
                                        object objxDeliveryNoteNo = myFunctions.checkProcessed("Inv_DeliveryNote", "X_ReceiptNo", "N_SalesOrderID", "@nSalesOrderID", "N_CompanyID=@nCompanyID and B_IsSaveDraft=0", Params, dLayer, connection);
                                        Master.Rows[0]["X_DeliveryNoteNo"] = objxDeliveryNoteNo;
                                    }
                                }


                                object objSales = myFunctions.checkProcessed("Inv_Sales", "N_SalesID", "N_SalesOrderID", "@nSalesOrderID", "N_CompanyID=@nCompanyID and B_IsSaveDraft=0", Params, dLayer, connection);
                                if (objSales.ToString() != "")
                                {
                                    if (myFunctions.getIntVAL(objSales.ToString()) > 0)
                                    {
                                        Params.Add("@nSalesInvID", myFunctions.getIntVAL(objSales.ToString()));
                                        Master.Rows[0]["B_SalesProcessed"] = 1;
                                        object objxSalesNo = myFunctions.checkProcessed("Inv_Sales", "N_SalesID", "X_ReceiptNo", "@nSalesOrderID", "N_CompanyID=@nCompanyID and B_IsSaveDraft=0", Params, dLayer, connection);
                                        Master.Rows[0]["X_SalesReceiptNo"] = objxSalesNo;

                                    }
                                }

                            }

                        }
                    }

                    var UserCategoryID = User.FindFirst(ClaimTypes.GroupSid)?.Value;
                    DateTime quotationDate = myFunctions.GetFormatedDate(Master.Rows[0]["D_QuotationDate"].ToString());
                    //Check Settings 
                    int N_DefSPriceID = myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "DefSPriceTypeID", "N_Value", "N_UserCategoryID", UserCategoryID, myFunctions.getIntVAL(nCompanyId.ToString()), dLayer, connection));
                    bool B_LastSPrice = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings(this.FormID.ToString(), "LastSPrice_InGrid", "N_Value", "N_UserCategoryID", UserCategoryID, myFunctions.getIntVAL(nCompanyId.ToString()), dLayer, connection)));
                    bool B_LastPurchaseCost = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings(this.FormID.ToString(), "LastPurchaseCost", "N_Value", "N_UserCategoryID", UserCategoryID.ToString(), myFunctions.getIntVAL(nCompanyId.ToString()), dLayer, connection)));

                    Params.Add("@nDefSPriceID", N_DefSPriceID);
                    Params.Add("@dQuotationDate", myFunctions.getDateVAL(quotationDate));
                    string sqlCommandText2 = "Select *,dbo.SP_GenGetStock(vw_InvQuotationDetails.N_ItemID,@nLocationID,'','location') As N_Stock ,dbo.[SP_Stock](vw_InvQuotationDetails.N_ItemID) As N_StockAll ,dbo.SP_Cost(vw_InvQuotationDetails.N_ItemID,vw_InvQuotationDetails.N_CompanyID,'') As N_LPrice,dbo.SP_SellingPrice(vw_InvQuotationDetails.N_ItemID,vw_InvQuotationDetails.N_CompanyID) As N_SPrice,dbo.SP_SellingPrice_Select(vw_InvQuotationDetails.N_ItemID,vw_InvQuotationDetails.N_CompanyID,@nDefSPriceID,@nBranchID) As N_UnitSPrice,dbo.SP_GetQuotationCount(@nCompanyID,vw_InvQuotationDetails.N_ItemID,@dQuotationDate) As QuotedQty  from vw_InvQuotationDetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_QuotationID=@nQuotationID";
                    if (n_OpportunityID > 0)
                        sqlCommandText2 = "select * from vw_OpportunityToQuotationDetails Where N_CompanyID=@nCompanyID and n_OpportunityID=" + n_OpportunityID;
                    DataTable Details = new DataTable();
                    Details = dLayer.ExecuteDataTable(sqlCommandText2, Params, connection);
                    Details = _api.Format(Details, "Details");

                    Details = myFunctions.AddNewColumnToDataTable(Details, "X_UpdatedSPrice", typeof(string), "");
                    Details = myFunctions.AddNewColumnToDataTable(Details, "SubQty", typeof(string), "");
                    Details = myFunctions.AddNewColumnToDataTable(Details, "BaseUnitQty", typeof(string), "0.00");
                    Details = myFunctions.AddNewColumnToDataTable(Details, "LastSellingPrice", typeof(string), "0.00");
                    Details = myFunctions.AddNewColumnToDataTable(Details, "LastPurchasePrice", typeof(string), "0.00");
                    Params.Add("@nSPriceTypeID", "");
                    Params.Add("@xClassItemCode", "");
                    Params.Add("@nItemID", "");
                    Params.Add("@xItemUnit", "");
                    foreach (DataRow var in Details.Rows)
                    {
                        if (var["ItemClass"].ToString() == "Group Item")
                        {
                            Params["@nSPriceTypeID"] = var["N_SPriceTypeID"].ToString();
                            if (var["N_SPriceTypeID"].ToString() != "")
                                var["X_UpdatedSPrice"] = Convert.ToString(dLayer.ExecuteScalar("select X_Name from Gen_LookupTable where N_CompanyID=@nCompanyID and N_ReferId=3 and N_PkeyId=@nSPriceTypeID", Params, connection));

                            Params["@xClassItemCode"] = var["ClassItemCode"].ToString();
                            Params["@nItemID"] = var["N_ItemId"].ToString();
                            object subQty = dLayer.ExecuteScalar("Select N_Qty from Inv_ItemDetails where N_MainItemId=(select N_ItemId from Inv_Itemmaster where X_itemcode=@xClassItemCode) and N_ItemId=@nItemID and N_CompanyID=@nCompanyID", Params, connection);
                            var["X_UpdatedSPrice"] = subQty.ToString();
                        }
                        else
                        {
                            Params["@nSPriceTypeID"] = var["N_SPriceTypeID"].ToString();
                            if (var["N_SPriceTypeID"].ToString() != "")
                                var["X_UpdatedSPrice"] = Convert.ToString(dLayer.ExecuteScalar("select X_Name from Gen_LookupTable where N_CompanyID=@nCompanyID and N_ReferId=3 and N_PkeyId=@nSPriceTypeID", Params, connection));
                            Params["@nItemID"] = var["N_ItemId"].ToString();
                            Params["@xItemUnit"] = var["X_ItemUnit"].ToString();
                            object BaseUnitQty = dLayer.ExecuteScalar("Select N_Qty from Inv_ItemUnit Where N_CompanyID=@nCompanyID and N_ItemID =@nItemID and X_ItemUnit=@xItemUnit", Params, connection);
                            if (BaseUnitQty != null)
                            {
                                var["BaseUnitQty"] = BaseUnitQty.ToString();
                            }

                            if (B_LastSPrice)
                            {
                                if (BaseUnitQty != null)
                                    var["LastSellingPrice"] = GetLastSellingPrice(myFunctions.getVAL(BaseUnitQty.ToString()), Params, connection);
                                else
                                    var["LastSellingPrice"] = GetLastSellingPrice(myFunctions.getVAL("1"), Params, connection);
                            }

                            if (B_LastPurchaseCost)
                            {
                                if (BaseUnitQty == null) { BaseUnitQty = 0; }
                                object LastPurchaseCost = dLayer.ExecuteScalar("Select TOP(1) ISNULL(N_LPrice,0) from Inv_StockMaster Where N_ItemID=@nItemID and N_CompanyID=@nCompanyID and N_LocationID=@nLocationID and (X_Type='Purchase' or X_Type='Opening') Order by N_StockID Desc", Params, connection);
                                if (LastPurchaseCost != null)
                                    var["LastPurchasePrice"] = (myFunctions.getVAL(LastPurchaseCost.ToString()) * myFunctions.getIntVAL(BaseUnitQty.ToString())).ToString(myFunctions.decimalPlaceString(myCompanyID.DecimalPlaces));
                            }


                        }

                    }
                    // if (Master.Rows.Count == 0 || Details.Rows.Count == 0)
                    // {
                    //     return Ok(_api.Notice("No data found"));
                    // }
                    Details = dLayer.ExecuteDataTable(sqlCommandText2, Params, connection);
                    Details = _api.Format(Details, "Details");
                    DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(Master.Rows[0]["N_CustomerID"].ToString()), myFunctions.getIntVAL(Master.Rows[0]["N_QuotationId"].ToString()), this.FormID, myFunctions.getIntVAL(Master.Rows[0]["N_FnYearID"].ToString()), User, connection);
                    Attachments = _api.Format(Attachments, "attachments");

                    dsQuotation.Tables.Add(Attachments);
                    dsQuotation.Tables.Add(Master);
                    dsQuotation.Tables.Add(Details);

                    return Ok(_api.Success(dsQuotation));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }



        private string GetLastSellingPrice(double baseUnitQuantity, SortedList Params, SqlConnection connection)
        {
            int UnitQty = 1;
            double lsprice = 0.0;
            string returnValue = "0.0";
            string sql = "";
            sql = "SELECT top 1  Inv_SalesDetails.N_SPrice,Inv_SalesDetails.N_ItemUnitID FROM  Inv_Sales INNER JOIN Inv_SalesDetails ON Inv_Sales.N_CompanyId = Inv_SalesDetails.N_CompanyID AND Inv_Sales.N_SalesId = Inv_SalesDetails.N_SalesID  where Inv_SalesDetails.N_ItemID =@nItemID and Inv_Sales.N_CompanyId=@nCompanyID and Inv_Sales.N_CustomerId=@nCustomerID order by Inv_Sales.D_SalesDate desc";

            DataTable LastSPrice = dLayer.ExecuteDataTable(sql, Params, connection);


            if (LastSPrice.Rows.Count > 0)
            {
                object res = dLayer.ExecuteScalar("Select N_Qty from Inv_ItemUnit Where N_CompanyID=@nCompanyID and N_ItemID =@nItemID and N_ItemUnitID=" + myFunctions.getIntVAL(LastSPrice.Rows[0][1].ToString()), Params, connection);
                if (res != null)
                    UnitQty = myFunctions.getIntVAL(res.ToString());


                lsprice = myFunctions.getVAL(LastSPrice.Rows[0][0].ToString()) / UnitQty;
            }

            returnValue = (lsprice * baseUnitQuantity).ToString(myFunctions.decimalPlaceString(myCompanyID.DecimalPlaces));

            return returnValue;


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

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    DataRow MasterRow = MasterTable.Rows[0];
                    transaction = connection.BeginTransaction();


                    int N_QuotationID = myFunctions.getIntVAL(MasterRow["n_QuotationID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int N_BranchID = myFunctions.getIntVAL(MasterRow["n_BranchID"].ToString());
                    int N_LocationID = myFunctions.getIntVAL(MasterRow["n_LocationID"].ToString());
                    int N_CustomerID = myFunctions.getIntVAL(MasterRow["n_CustomerID"].ToString());

                    QueryParams.Add("@nCompanyID", N_CompanyID);
                    QueryParams.Add("@nFnYearID", N_FnYearID);
                    QueryParams.Add("@nQuotationID", N_QuotationID);
                    QueryParams.Add("@nBranchID", N_BranchID);
                    QueryParams.Add("@nLocationID", N_LocationID);


                    bool B_SalesEnquiry = myFunctions.CheckPermission(N_CompanyID, 724, "Administrator", "X_UserCategory", dLayer, connection, transaction);


                    // Auto Gen
                    string QuotationNo = MasterTable.Rows[0]["x_QuotationNo"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    if (QuotationNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", Master["n_CompanyId"].ToString());
                        Params.Add("N_YearID", Master["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 80);
                        Params.Add("N_BranchID", Master["n_BranchId"].ToString());
                        QuotationNo = dLayer.GetAutoNumber("Inv_SalesQuotation", "x_QuotationNo", Params, connection, transaction);
                        if (QuotationNo == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Quotation Number")); }
                        MasterTable.Rows[0]["x_QuotationNo"] = QuotationNo;

                    }

                    if (N_QuotationID > 0)
                    {
                        SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",N_CompanyID},
                                {"X_TransType","Sales Quotation"},
                                {"N_VoucherID",N_QuotationID}};
                        dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                    }
                    string DupCriteria = "N_CompanyID=" + N_CompanyID + " and X_QuotationNo='" + QuotationNo + "'";
                    N_QuotationID = dLayer.SaveData("Inv_SalesQuotation", "N_QuotationId", DupCriteria, "", MasterTable, connection, transaction);
                    if (N_QuotationID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Quotation"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_QuotationID"] = N_QuotationID;
                    }

                    int N_QuotationDetailId = dLayer.SaveData("Inv_SalesQuotationDetails", "n_QuotationDetailsID", DetailTable, connection, transaction);
                    if (N_QuotationDetailId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Quotation"));
                    }
                    else
                    {
                        QueryParams.Add("@nItemID", 0);
                        QueryParams.Add("@nCRMID", 0);
                        QueryParams.Add("@nPurchaseCost", 0);
                        for (int k = 0; k < DetailTable.Rows.Count; k++)
                        {
                            QueryParams["@nItemID"] = myFunctions.getIntVAL(DetailTable.Rows[k]["n_ItemID"].ToString());
                            QueryParams["@nCRMID"] = myFunctions.getIntVAL(DetailTable.Rows[k]["n_CRMID"].ToString());
                            QueryParams["@nPurchaseCost"] = myFunctions.getVAL(DetailTable.Rows[k]["n_PurchaseCost"].ToString());

                            if (myFunctions.getVAL(QueryParams["@nPurchaseCost"].ToString()) > 0)
                                dLayer.ExecuteNonQuery("Update Inv_ItemMaster Set N_PurchaseCost=@nPurchaseCost Where N_ItemID=@nItemID and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                            if (myFunctions.getIntVAL(QueryParams["@nCRMID"].ToString()) > 0)
                                dLayer.ExecuteNonQuery("Update Inv_CRMDetails Set B_Processed=1 Where N_CRMID=@nCRMID and N_ItemID=@nItemID and N_CompanyID=@nCompanyID and N_BranchID=@nBranchID", QueryParams, connection, transaction);
                        }

                        SortedList CustomerParams = new SortedList();
                        CustomerParams.Add("@nCustomerID", N_CustomerID);
                        DataTable CustomerInfo = dLayer.ExecuteDataTable("Select X_CustomerCode,X_CustomerName from Inv_Customer where N_CustomerID=@nCustomerID", CustomerParams, connection, transaction);
                        if (CustomerInfo.Rows.Count > 0)
                        {
                            try
                            {
                                myAttachments.SaveAttachment(dLayer, Attachment, QuotationNo, N_QuotationID, CustomerInfo.Rows[0]["X_CustomerName"].ToString().Trim(), CustomerInfo.Rows[0]["X_CustomerCode"].ToString(), N_CustomerID, "Customer Document", User, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(ex));
                            }
                        }
                        transaction.Commit();
                    }
                    SortedList Result = new SortedList();
                    Result.Add("n_QuotationID", N_QuotationID);
                    Result.Add("x_QuotationNo", QuotationNo);
                    return Ok(_api.Success(Result, "Sales quotation saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }


        [HttpGet("getItem")]
        public ActionResult GetItem(int nCompanyID, int nLocationID, int nBranchID, string dDate, string InputVal, int nCustomerID, bool bSelected)
        {
            string ItemCondition = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int N_DefSPriceID = 0;
                var UserCategoryID = User.FindFirst(ClaimTypes.GroupSid)?.Value;
                N_DefSPriceID = myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "DefSPriceTypeID", "N_Value", "N_UserCategoryID", UserCategoryID, myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection));
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


                string sql = "Select * ,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_AvlStock,  dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as T_Stock, dbo.SP_GetQuotationCount(@nCompanyID,vw_InvItem_Search.N_ItemID,@date) As QuotedQty, dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,'') As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nCompanyID";

                if (B_SPRiceType)
                {
                    if (nSPriceID > 0)
                    {
                        sql = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_AvlStock , dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as T_Stock ,dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_ItemUnit) As N_LPrice ,dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as T_Stock, dbo.SP_GetQuotationCount(@nCompanyID,vw_InvItem_Search.N_ItemID,@date) As QuotedQty,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nSPriceID, @nBranchID) As N_SPrice  From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nCompanyID";
                    }
                    else
                        sql = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_AvlStock, dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as T_Stock ,dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_ItemUnit) As N_LPrice ,dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as T_Stock, dbo.SP_GetQuotationCount(@nCompanyID,vw_InvItem_Search.N_ItemID,@date) As QuotedQty,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nDefSPriceID, @nBranchID) As N_SPrice  From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nComapanyID";
                }

                ItemDetails = dLayer.ExecuteDataTable(sql, paramList, connection);
                // ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails,"LastSalesPrice",typeof(string),"0.00");
                // ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails,"X_DefSPriceType",typeof(string),"");
                // ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails,"N_DefSPriceID",typeof(string),"0");
                // ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails,"LastPurchaseCost",typeof(string),"0");
                // if (ItemDetails.Rows.Count == 1)
                // {
                //     DataRow ItemDetailRow = ItemDetails.Rows[0];
                //     int N_ItemID = myFunctions.getIntVAL(ItemDetailRow["N_ItemID"].ToString());
                //     SortedList qParam2 = new SortedList();
                //     qParam2.Add("@nItemID", N_ItemID);
                //     qParam2.Add("@nCompanyID", nCompanyID);
                //     qParam2.Add("@nCustomerID", nCustomerID);


                //     object Mrp = dLayer.ExecuteScalar("Select top 1 N_Mrp from Inv_PurchaseDetails where N_ItemId=@nItemID and N_CompanyID=@nCompanyID Order By N_PurchaseDetailsId desc", qParam2, connection);


                //     if (B_SPRiceType)
                //     {
                //         if (nSPriceID == 0)
                //         {
                //             qParam2.Add("@nDefSPriceID", N_DefSPriceID);
                //             qParam2.Add("@nBranchID", nBranchID);
                //             object obj1 = dLayer.ExecuteScalar("Select isnull(Count(N_PriceID),0) from Inv_ItemPriceMaster where N_ItemID=@nItemID and N_CompanyID=@nCompanyID and N_BranchID=@nBranchID and N_PriceID=@nDefSPriceID  and N_PriceVal>0", qParam2, connection);

                //              if (obj1 != null)
                //                 {
                //                     if (myFunctions.getIntVAL(obj1.ToString()) > 0)
                //                     {
                //                         ItemDetailRow["X_DefSPriceType"]=X_DefSPriceType;
                //                         ItemDetailRow["N_DefSPriceID"]= N_DefSPriceID.ToString();
                //                     }
                //                 }

                //         }
                //     }

                //     bool B_ShowPurchaseCost = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings(this.FormID.ToString(), "Show_Purchase_Cost", "N_Value", "N_UserCategoryID", UserCategoryID, myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                //     bool B_LastSPrice = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings(this.FormID.ToString(), "LastSPrice_InGrid", "N_Value", "N_UserCategoryID", UserCategoryID, myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                //     bool B_LastPurchaseCost = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings(this.FormID.ToString(), "LastPurchaseCost", "N_Value", "N_UserCategoryID", UserCategoryID.ToString(), myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                //     //QuotedQty
                //     qParam2.Add("@xItemUnit", ItemDetailRow["X_SalesUnit"].ToString());
                //     qParam2.Add("@nLocationID", nLocationID);

                //     if (B_LastSPrice)
                //     {
                //         object resBaseUnitQty = dLayer.ExecuteScalar("Select N_Qty from Inv_ItemUnit Where N_CompanyID=@nCompanyID and N_ItemID =@nItemID and X_ItemUnit=@xItemUnit",qParam2,connection);
                //         double baseUnitQuantity = myFunctions.getVAL(resBaseUnitQty.ToString());
                //         int UnitQty = 1;
                //         double lsprice = 0.0;
                //         string lspSql = "SELECT top 1  Inv_SalesDetails.N_SPrice,Inv_SalesDetails.N_ItemUnitID FROM  Inv_Sales INNER JOIN Inv_SalesDetails ON Inv_Sales.N_CompanyId = Inv_SalesDetails.N_CompanyID AND Inv_Sales.N_SalesId = Inv_SalesDetails.N_SalesID  where Inv_SalesDetails.N_ItemID =@nItemID and Inv_Sales.N_CompanyId=@nCompanyID and Inv_Sales.N_CustomerId=@nCustomerID order by Inv_Sales.D_SalesDate desc";

                //          LastSalesPrice = dLayer.ExecuteDataTable(lspSql, qParam2, connection);

                //         if (LastSalesPrice.Rows.Count > 0)
                //         {
                //             qParam2.Add("@nItemUnitID", LastSalesPrice.Rows[0][1].ToString());
                //             object resQty = dLayer.ExecuteScalar("Select N_Qty from Inv_ItemUnit Where N_CompanyID=@nCompanyID and N_ItemID =@nItemID and N_ItemUnitID=@nItemUnitID", qParam2, connection);
                //             if (resQty != null)
                //                 UnitQty = myFunctions.getIntVAL(res.ToString());
                //                 ItemDetailRow["LastSalesPrice"] =  (myFunctions.getVAL(LastSalesPrice.Rows[0][0].ToString()) / UnitQty).ToString();
                //         }


                //     }

                //     if (B_LastPurchaseCost)
                //     {
                //         object LastPurchaseCost = dLayer.ExecuteScalar("Select TOP(1) ISNULL(N_LPrice,0) from Inv_StockMaster Where N_ItemID=@nItemID and N_CompanyID=@nCompanyID and N_LocationID=@nLocationID and (X_Type='Purchase' or X_Type='Opening') Order by N_StockID Desc",qParam2,connection);
                //     ItemDetailRow["LastPurchaseCost"]=LastSalesPrice.ToString();
                //     }


                //     ItemClass = ItemDetailRow["Item Class"].ToString();
                //     if (ItemClass == "Group Item")
                //     {
                //         SortedList qParam3 = new SortedList();
                //         qParam3.Add("@nItemID", 0);
                //         qParam3.Add("@nCompanyID", nCompanyID);
                //         SubItems = dLayer.ExecuteDataTable("Select * from vw_invitemdetails where N_MainItemId=@nItemID and N_CompanyID=@nCompanyID order by X_Itemname", qParam3, connection);
                //         foreach (DataRow var in SubItems.Rows)
                //         {

                //             if (var["N_ItemDetailsID"].ToString() != "")
                //             {
                //                 qParam3["@nItemID"] = var["N_ItemId"].ToString();
                //                 subItemPrice = dLayer.ExecuteScalar("Select top 1 N_Sprice from Inv_StockMaster where N_ItemId=@nItemID and N_CompanyID=@nCompanyID order by n_stockid desc", qParam3, connection);
                //                 subPprice = dLayer.ExecuteScalar("Select top 1 N_Sprice from Inv_StockMaster where N_ItemId=@nItemID and N_CompanyID=@nCompanyID order by n_stockid desc", qParam3, connection);
                //                 subMrp = dLayer.ExecuteScalar("Select top 1 N_Mrp from Inv_PurchaseDetails where N_ItemId=@nItemID and N_CompanyID=@nCompanyID Order By N_PurchaseDetailsId desc", qParam3, connection);
                //                 if (subItemPrice != null) SpriceSum = myFunctions.getVAL(subItemPrice.ToString()) * myFunctions.getVAL(var["N_Qty"].ToString()) + SpriceSum;
                //                 if (subPprice != null) PpriceSum = myFunctions.getVAL(subPprice.ToString()) + PpriceSum;
                //                 if (subMrp != null) Mrpsum = myFunctions.getVAL(subMrp.ToString()) + Mrpsum;
                //             }
                //         }
                //     }


                //     object objSPrice = dLayer.ExecuteScalar("Select Isnull(N_Value,0) from Gen_Settings where N_CompanyID=@nCompanyID and X_Group='Inventory' and X_Description='Selling Price Calculation'", qParam2, connection);

                //     DataTable sellingPrice = dLayer.ExecuteDataTable("Select N_Qty,N_SellingPrice from Inv_ItemUnit Where N_CompanyID=@nCompanyID and N_ItemID =@nItemID and X_ItemUnit=@xItemUnit", qParam2, connection);

                //     object value = dLayer.ExecuteScalar("select N_DiscPerc from inv_CustomerDiscount where N_ProductID =@nItemID and N_CustomerID =@nCustomerID and N_CompanyID =@nCompanyID", qParam2, connection);

                //}
                return Ok(_api.Success(ItemDetails));
            }

        }


        //Delete....
        [HttpDelete()]
        public ActionResult DeleteData(int N_QuotationID, int nBranchID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    var xUserCategory = myFunctions.GetUserCategory(User);// User.FindFirst(ClaimTypes.GroupSid)?.Value;
                    var nUserID = myFunctions.GetUserID(User);// User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    object objSalesProcessed = dLayer.ExecuteScalar("Select Isnull(N_SalesID,0) from Inv_Sales where N_CompanyID=" + nCompanyID + " and N_QuotationID=" + N_QuotationID + " and B_IsSaveDraft = 0", connection, transaction);
                    object objOrderProcessed = dLayer.ExecuteScalar("Select Isnull(N_SalesOrderId,0) from Inv_SalesOrder where N_CompanyID=" + nCompanyID + " and N_QuotationID=" + N_QuotationID + "", connection, transaction);
                    if (objSalesProcessed == null)
                        objSalesProcessed = 0;
                    if (objOrderProcessed == null)
                        objOrderProcessed = 0;
                    if (myFunctions.getIntVAL(objSalesProcessed.ToString()) == 0 && myFunctions.getIntVAL(objOrderProcessed.ToString()) == 0)
                    {
                        SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType","SALES QUOTATION"},
                                {"N_VoucherID",N_QuotationID},
                                {"N_UserID",nUserID},
                                {"X_SystemName","WebRequest"},
                                {"N_BranchID",nBranchID}};
                        Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                        if (Results <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable to delete Sales Quotation"));
                        }
                        else
                        {
                            transaction.Commit();
                            return Ok(_api.Success("Sales Quotation deleted"));

                        }
                    }
                    else
                    {
                        transaction.Rollback();
                        if (myFunctions.getIntVAL(objSalesProcessed.ToString()) > 0)
                            return Ok(_api.Error("Sales invoice processed! Unable to delete"));
                        else if (myFunctions.getIntVAL(objOrderProcessed.ToString()) > 0)
                            return Ok(_api.Error("Sales order processed! Unable to delete"));
                        else
                            return Ok(_api.Error("Unable to delete!"));

                    }


                    // Results=dLayer.DeleteData("Inv_SalesQuotationDetails", "n_quotationID", N_QuotationID, "", connection, transaction);

                    // if (Results <= 0)
                    // {
                    //     transaction.Rollback();
                    //     return Ok(_api.Error("Unable to delete sales quotation"));
                    // }
                    // else
                    // {
                    // Results = dLayer.DeleteData("Inv_SalesQuotation", "n_quotationID", N_QuotationID, "", connection, transaction);

                    // }
                    // if (Results > 0)
                    // {
                    //     transaction.Commit();
                    //     return Ok(_api.Success("Sales quotation deleted"));
                    // }
                    // else
                    // {
                    //     transaction.Rollback();
                    //     return Ok(_api.Error("Unable to delete sales quotation"));
                    // }

                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }


        }


        // [HttpGet("dummy")]
        // public ActionResult GetQtyDummy(int? Id)
        // {
        //     try
        //     {
        //         string sqlCommandText = "select * from Inv_SalesQuotation where N_QuotationID=@p1";
        //         SortedList mParamList = new SortedList() { { "@p1", Id } };
        //         DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList);
        //         masterTable = _api.Format(masterTable, "master");

        //         string sqlCommandText2 = "select * from Inv_SalesQuotationDetails where N_QuotationID=@p1";
        //         SortedList dParamList = new SortedList() { { "@p1", Id } };
        //         DataTable detailTable = dLayer.ExecuteDataTable(sqlCommandText2, dParamList);
        //         detailTable = _api.Format(detailTable, "details");

        //         if (detailTable.Rows.Count == 0) { return Ok(new { }); }
        //         DataSet dataSet = new DataSet();
        //         dataSet.Tables.Add(masterTable);
        //         dataSet.Tables.Add(detailTable);

        //         return Ok(dataSet);

        //     }
        //     catch (Exception e)
        //     {
        //         return StatusCode(403, _api.Error(e));
        //     }
        // }

    }
}