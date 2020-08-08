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
        private readonly string connectionString;
        private readonly int FormID;

        public Inv_SalesQuotation(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 80;
        }


        [HttpGet("list")]
        public ActionResult GetSalesQuotationList(int? nCompanyId, int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select N_QuotationId as NQuotationId,[Quotation No] as QuotationNo,[Quotation Date] as QuotationDate,N_CompanyId as NCompanyId,N_CustomerId as NCustomerId,[Customer Code] as CustomerCode,N_FnYearID as NFnYearId,D_QuotationDate as DQuotationDate,N_BranchId as NBranchId,B_YearEndProcess as BYearEndProcess,X_CustomerName as XCustomerName,X_BranchName as XBranchName,X_RfqRefNo as XRfqRefNo,D_RfqRefDate as DRfqRefDate,N_Amount as NAmount,N_FreightAmt as NFreightAmt,N_DiscountAmt as NDiscountAmt,N_Processed as NProcessed,N_OthTaxAmt as NOthTaxAmt,N_BillAmt as NBillAmt,N_ProjectID as NProjectId,X_ProjectName as XProjectName from vw_InvSalesQuotationNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2";

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

            try
            {
                dt = dLayer.ExecuteDataTable(sqlCommandText, Params);
                if (dt.Rows.Count == 0)
                {
                    return Ok(new { });
                }
                else
                {
                    return Ok(dt);
                }
            }
            catch (Exception e)
            {
                return StatusCode(403, _api.ErrorResponse(e));
            }
        }
        [HttpGet("listDetails")]
        public ActionResult GetSalesQuotationDetails(int? nCompanyId, int xQuotationNo, int nFnYearId, bool bAllBranchData, int nBranchID)
        {
            DataSet dsQuotation = new DataSet();
            DataTable dtProcess = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "";

            Params.Add("@nCompanyID", nCompanyId);
            Params.Add("@nFnYearID", nFnYearId);
            Params.Add("@xQuotationNo", xQuotationNo);

            if (bAllBranchData == true)
            {
                sqlCommandText = "select * from vw_InvSalesQuotationNo_Search where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and [Quotation No]=@xQuotationNo";
            }
            else
            {
                sqlCommandText = "select * from vw_InvSalesQuotationNo_Search where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and [Quotation No]=@xQuotationNo and N_BranchID=@nBranchID";
                Params.Add("@nBranchID", nBranchID);
            }


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dtQuotation = new DataTable();
                    dtQuotation = dLayer.ExecuteDataTable(sqlCommandText, Params);
                    dtQuotation = _api.Format(dtQuotation, "Master");
                    dsQuotation.Tables.Add(dtQuotation);

                    if (dsQuotation.Tables["Master"].Rows.Count == 0)
                        return Ok(_api.Notice("There is no data!"));

                    var nQuotationId = dtQuotation.Rows[0]["N_QuotationId"];
                    var nFormID = 80;
                    var nCustomerID = dtQuotation.Rows[0]["N_CustomerId"];
                    var nSalesOrderID = dtQuotation.Rows[0]["N_CustomerId"];
                    Params.Add("@nQuotationID", nQuotationId);
                    Params.Add("@nFormID", nFormID);

                    object objFollowup = dLayer.ExecuteScalar("Select  isnull(max(N_id),0) from vsa_appointment where n_refid = @nQuotationID and N_companyID=@nCompanyID and B_IsComplete=0", Params);
                    if (objFollowup != null)
                    {
                        Params.Add("@nRefTypeID", 8);
                        DataTable dtFollowUp = new DataTable();
                        string qry = "Select * from vw_QuotaionFollowup Where N_CompanyID=@nCompanyID and N_RefTypeID=@nRefTypeID and N_RefID= @nQuotationID";
                        dtFollowUp = dLayer.ExecuteDataTable(qry, Params);
                        dtFollowUp = _api.Format(dtFollowUp, "FollowUp");
                        dsQuotation.Tables.Add(dtFollowUp);
                    }

                    //object objSalesOrder = dLayer.ExecuteScalar("Select N_SalesOrderID from Inv_SalesOrder Where N_CompanyID=@nCompanyID and N_QuotationID =@nQuotationID and B_IsSaveDraft=0", Params);
                    object objSalesOrder = myFunctions.checkProcessed("Inv_SalesOrder", "N_SalesOrderID", "N_QuotationID", "@nQuotationID", "N_CompanyID=@nCompanyID and B_IsSaveDraft=0", Params, dLayer, connection);
                    DataColumn col1 = new DataColumn("B_SalesOrderProcessed", typeof(Boolean));
                    col1.DefaultValue = false;
                    dsQuotation.Tables["Master"].Columns.Add(col1);
                    if (objSalesOrder.ToString() != "")
                    {
                        if (myFunctions.getIntVAL(objSalesOrder.ToString()) > 0)
                        {
                            Params.Add("@nSalesOrderID", myFunctions.getIntVAL(objSalesOrder.ToString()));
                            dsQuotation.Tables["Master"].Rows[0][col1] = true;



                            //object objDeliveryNote = dLayer.ExecuteScalar("select N_DeliveryNoteID from Inv_DeliveryNote where N_CompanyID=@nCompanyID and N_SalesOrderID=@nSalesOrderID and B_IsSaveDraft=0", Params);
                            object objDeliveryNote = myFunctions.checkProcessed("Inv_DeliveryNote", "N_DeliveryNoteID", "N_SalesOrderID", "@nSalesOrderID", "N_CompanyID=@nCompanyID and B_IsSaveDraft=0", Params, dLayer, connection);
                            DataColumn col2 = new DataColumn("B_DeliveryNoteProcessed", typeof(Boolean));
                            col2.DefaultValue = false;
                            dsQuotation.Tables["Master"].Columns.Add(col2);
                            if (objDeliveryNote.ToString() != "")
                            {
                                if (myFunctions.getIntVAL(objDeliveryNote.ToString()) > 0)
                                {
                                    Params.Add("@nDeliveryNoteID", myFunctions.getIntVAL(objDeliveryNote.ToString()));
                                    dsQuotation.Tables["Master"].Rows[0][col2] = true;
                                }
                            }


                            //object objSales = dLayer.ExecuteScalar("select N_SalesID from Inv_Sales where N_CompanyID=@nCompanyID and N_SalesOrderID=@nSalesOrderID and B_IsSaveDraft = 0", Params);
                            object objSales = myFunctions.checkProcessed("Inv_Sales", "N_SalesID", "N_SalesOrderID", "@nSalesOrderID", "N_CompanyID=@nCompanyID and B_IsSaveDraft=0", Params, dLayer, connection);
                            DataColumn col3 = new DataColumn("B_SalesProcessed", typeof(Boolean));
                            col3.DefaultValue = false;
                            dsQuotation.Tables["Master"].Columns.Add(col3);
                            if (objSales.ToString() != "")
                            {
                                if (myFunctions.getIntVAL(objSales.ToString()) > 0)
                                {
                                    Params.Add("@nSalesInvID", myFunctions.getIntVAL(objSales.ToString()));
                                    dsQuotation.Tables["Master"].Rows[0][col3] = true;
                                }
                            }

                        }

                    }



                    //Quotation Details

                    string sqlCommandText2 = "select * from vw_InvQuotationDetails where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_QuotationID=@nQuotationID";
                    DataTable QuotationDetails = new DataTable();
                    QuotationDetails = dLayer.ExecuteDataTable(sqlCommandText2, Params);
                    QuotationDetails = _api.Format(QuotationDetails, "Details");
                    dsQuotation.Tables.Add(QuotationDetails);


                    //ATTACHMENTS
                    DataTable dtAttachments = new DataTable();
                    SortedList ParamsAttachment = new SortedList();
                    ParamsAttachment.Add("CompanyID", nCompanyId);
                    ParamsAttachment.Add("FnyearID", nFnYearId);
                    ParamsAttachment.Add("PayID", nQuotationId);
                    ParamsAttachment.Add("PartyID", nCustomerID);
                    ParamsAttachment.Add("FormID", nFormID);
                    dtAttachments = dLayer.ExecuteDataTablePro("SP_VendorAttachments", ParamsAttachment);
                    dtAttachments = _api.Format(dtAttachments, "Attachments");
                    dsQuotation.Tables.Add(dtAttachments);
                }
                return Ok(dsQuotation);
            }
            catch (Exception e)
            {
                return StatusCode(403, _api.ErrorResponse(e));
            }
        }


        [HttpGet("details")]
        public ActionResult GetQuotationDetails(int? nCompanyId, int xQuotationNo, int nFnYearId, bool bAllBranchData, int nBranchID)
        {
            DataSet dsQuotation = new DataSet();
            DataTable dtProcess = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "";

            Params.Add("@nCompanyID", nCompanyId);
            Params.Add("@nFnYearID", nFnYearId);
            Params.Add("@xQuotationNo", xQuotationNo);
            Params.Add("@nBranchID", nBranchID);

            if (bAllBranchData == true)
            {
                sqlCommandText = "Select * from Inv_SalesQuotation Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and X_QuotationNo=@xQuotationNo";
            }
            else
            {
                sqlCommandText = "Select * from Inv_SalesQuotation Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and X_QuotationNo=@xQuotationNo and N_BranchID=@nBranchID";
            }


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable Master = new DataTable();
                    Master = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    Master = _api.Format(Master, "Master");
                    //dsQuotation.Tables.Add(dtQuotation);

                    if (Master.Rows.Count == 0)
                        return Ok(_api.Notice("There is no data!"));

                    var nQuotationId = Master.Rows[0]["N_QuotationId"];
                    var nFormID = 80;
                    var nCRMID = Master.Rows[0]["N_CRMID"];

                    if (myFunctions.getIntVAL(nCRMID.ToString()) > 0)
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
                        object xCustomerCode = dLayer.ExecuteScalar(" select X_CustomerCode from Inv_Customer where N_CustomerID=@nCustomerID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and (N_BranchID=0 or N_BranchID=@nBranchID) and B_Inactive = 0", Params, connection);
                        Master = myFunctions.AddNewColumnToDataTable(Master, "X_CustomerCode", typeof(string), xCustomerCode.ToString());
                    }
                    else
                    {
                        Master = myFunctions.AddNewColumnToDataTable(Master, "X_CustomerCode", typeof(string), "");
                    }

                    int nOthTaxCategoryID = myFunctions.getIntVAL(Master.Rows[0]["N_OthTaxCategoryID"].ToString());
                    Params.Add("@nOthTaxCategoryID", nOthTaxCategoryID);
                    object X_DisplayName = dLayer.ExecuteScalar("Select X_DisplayName from Acc_TaxCategory where N_PkeyID=@nOthTaxCategoryID", Params, connection);
                    Master = myFunctions.AddNewColumnToDataTable(Master, "X_DisplayName", typeof(string), X_DisplayName);

                    int N_SalesmanID = myFunctions.getIntVAL(Master.Rows[0]["N_SalesmanID"].ToString());
                    object X_SalesmanName = "", X_SalesmanCode = "";
                    if (N_SalesmanID.ToString() != "")
                    {
                        Params.Add("@nSalesmanID", N_SalesmanID);
                        X_SalesmanName = dLayer.ExecuteScalar("select X_SalesmanName from Inv_Salesman where N_CompanyID=@nCompanyID and N_SalesmanID=@nSalesmanID", Params, connection);
                        X_SalesmanCode = dLayer.ExecuteScalar("select X_SalesmanCode from Inv_Salesman where N_CompanyID=@nCompanyID and N_SalesmanID=@nSalesmanID", Params, connection);
                    }
                    Master = myFunctions.AddNewColumnToDataTable(Master, "X_SalesmanName", typeof(string), X_SalesmanName.ToString());
                    Master = myFunctions.AddNewColumnToDataTable(Master, "X_SalesmanCode", typeof(string), X_SalesmanCode.ToString());

                    Params.Add("@nQuotationID", nQuotationId);
                    Params.Add("@nFormID", nFormID);

                    object objFollowup = dLayer.ExecuteScalar("Select  isnull(max(N_id),0) from vsa_appointment where n_refid = @nQuotationID and N_companyID=@nCompanyID and B_IsComplete=0", Params);
                    if (objFollowup != null)
                    {
                        Params.Add("@nRefTypeID", 8);
                        DataTable dtFollowUp = new DataTable();
                        string qry = "Select * from vw_QuotaionFollowup Where N_CompanyID=@nCompanyID and N_RefTypeID=@nRefTypeID and N_RefID= @nQuotationID";
                        dtFollowUp = dLayer.ExecuteDataTable(qry, Params);
                        dtFollowUp = _api.Format(dtFollowUp, "FollowUp");
                        dsQuotation.Tables.Add(dtFollowUp);
                    }


                    //object objSalesOrder = dLayer.ExecuteScalar("Select N_SalesOrderID from Inv_SalesOrder Where N_CompanyID=@nCompanyID and N_QuotationID =@nQuotationID and B_IsSaveDraft=0", Params);
                    object objSalesOrder = myFunctions.checkProcessed("Inv_SalesOrder", "N_SalesOrderID", "N_QuotationID", "@nQuotationID", "N_CompanyID=@nCompanyID and B_IsSaveDraft=0", Params, dLayer, connection);
                    DataColumn col1 = new DataColumn("B_SalesOrderProcessed", typeof(Boolean));
                    col1.DefaultValue = false;
                    Master.Columns.Add(col1);
                    if (objSalesOrder.ToString() != "")
                    {
                        if (myFunctions.getIntVAL(objSalesOrder.ToString()) > 0)
                        {
                            Params.Add("@nSalesOrderID", myFunctions.getIntVAL(objSalesOrder.ToString()));
                            Master.Rows[0][col1] = true;



                            //object objDeliveryNote = dLayer.ExecuteScalar("select N_DeliveryNoteID from Inv_DeliveryNote where N_CompanyID=@nCompanyID and N_SalesOrderID=@nSalesOrderID and B_IsSaveDraft=0", Params);
                            object objDeliveryNote = myFunctions.checkProcessed("Inv_DeliveryNote", "N_DeliveryNoteID", "N_SalesOrderID", "@nSalesOrderID", "N_CompanyID=@nCompanyID and B_IsSaveDraft=0", Params, dLayer, connection);
                            DataColumn col2 = new DataColumn("B_DeliveryNoteProcessed", typeof(Boolean));
                            col2.DefaultValue = false;
                            Master.Columns.Add(col2);
                            if (objDeliveryNote.ToString() != "")
                            {
                                if (myFunctions.getIntVAL(objDeliveryNote.ToString()) > 0)
                                {
                                    Params.Add("@nDeliveryNoteID", myFunctions.getIntVAL(objDeliveryNote.ToString()));
                                    Master.Rows[0][col2] = true;
                                }
                            }


                            //object objSales = dLayer.ExecuteScalar("select N_SalesID from Inv_Sales where N_CompanyID=@nCompanyID and N_SalesOrderID=@nSalesOrderID and B_IsSaveDraft = 0", Params);
                            object objSales = myFunctions.checkProcessed("Inv_Sales", "N_SalesID", "N_SalesOrderID", "@nSalesOrderID", "N_CompanyID=@nCompanyID and B_IsSaveDraft=0", Params, dLayer, connection);
                            DataColumn col3 = new DataColumn("B_SalesProcessed", typeof(Boolean));
                            col3.DefaultValue = false;
                            Master.Columns.Add(col3);
                            if (objSales.ToString() != "")
                            {
                                if (myFunctions.getIntVAL(objSales.ToString()) > 0)
                                {
                                    Params.Add("@nSalesInvID", myFunctions.getIntVAL(objSales.ToString()));
                                    Master.Rows[0][col3] = true;
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
                                var["X_UpdatedSPrice"] = Convert.ToString(dLayer.ExecuteScalar("select X_Name from Gen_LookupTable where N_CompanyID=@nCompany and N_ReferId=3 and N_PkeyId=@nSPriceTypeID", Params, connection));

                            Params["@xClassItemCode"] = var["ClassItemCode"].ToString();
                            Params["@nItemID"] = var["N_ItemId"].ToString();
                            object subQty = dLayer.ExecuteScalar("Select N_Qty from Inv_ItemDetails where N_MainItemId=(select N_ItemId from Inv_Itemmaster where X_itemcode=@xClassItemCode) and N_ItemId=@nItemID and N_CompanyID=@nCompanyID", Params, connection);
                            var["X_UpdatedSPrice"] = subQty.ToString();
                        }
                        else
                        {
                            Params["@nSPriceTypeID"] = var["N_SPriceTypeID"].ToString();
                            if (var["N_SPriceTypeID"].ToString() != "")
                                var["X_UpdatedSPrice"] = Convert.ToString(dLayer.ExecuteScalar("select X_Name from Gen_LookupTable where N_CompanyID=@nCompany and N_ReferId=3 and N_PkeyId=@nSPriceTypeID", Params, connection));
                            Params["@nItemID"] = var["N_ItemId"].ToString();
                            Params["@xItemUnit"] = var["X_ItemUnit"].ToString();
                            object BaseUnitQty = dLayer.ExecuteScalar("Select N_Qty from Inv_ItemUnit Where N_CompanyID=@nCompanyID and N_ItemID =@nItemID and X_ItemUnit=@xItemUnit", Params, connection);
                            if (BaseUnitQty != null)
                                var["BaseUnitQty"] = BaseUnitQty.ToString();

                            if (B_LastSPrice)
                            {
                                if (BaseUnitQty != null)
                                    var["LastSellingPrice"] = GetLastSellingPrice(myFunctions.getVAL(BaseUnitQty.ToString()), Params, connection);
                                else
                                    var["LastSellingPrice"] = GetLastSellingPrice(myFunctions.getVAL("1"), Params, connection);
                            }

                            if (B_LastPurchaseCost)
                            {
                                object LastPurchaseCost = dLayer.ExecuteScalar("Select TOP(1) ISNULL(N_LPrice,0) from Inv_StockMaster Where N_ItemID=@nItemID and N_CompanyID=@nCompanyID and N_LocationID=@nLocationID and (X_Type='Purchase' or X_Type='Opening') Order by N_StockID Desc",Params,connection);
                                if (LastPurchaseCost != null)
                                    var["LastPurchasePrice"] = (myFunctions.getVAL(LastPurchaseCost.ToString()) * myFunctions.getIntVAL(BaseUnitQty.ToString())).ToString(myFunctions.decimalPlaceString(myCompanyID.DecimalPlaces));
                            }


                        }

                        dsQuotation.Tables.Add(Master);
                        dsQuotation.Tables.Add(Details);


                        //ATTACHMENTS
                        // DataTable dtAttachments = new DataTable();
                        // SortedList ParamsAttachment = new SortedList();
                        // ParamsAttachment.Add("CompanyID", nCompanyId);
                        // ParamsAttachment.Add("FnyearID", nFnYearId);
                        // ParamsAttachment.Add("PayID", nQuotationId);
                        // ParamsAttachment.Add("PartyID", nCustomerID);
                        // ParamsAttachment.Add("FormID", nFormID);
                        // dtAttachments = dLayer.ExecuteDataTablePro("SP_VendorAttachments", ParamsAttachment);
                        // dtAttachments = _api.Format(dtAttachments, "Attachments");
                        // dsQuotation.Tables.Add(dtAttachments);




                    }
                    return Ok(dsQuotation);
                }
            }
            catch (Exception e)
            {
                return StatusCode(403, _api.ErrorResponse(e));
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

                    QueryParams.Add("@nCompanyID", N_CompanyID);
                    QueryParams.Add("@nFnYearID", N_FnYearID);
                    QueryParams.Add("@nQuotationID", N_QuotationID);
                    QueryParams.Add("@nBranchID", N_BranchID);
                    QueryParams.Add("@nLocationID", N_LocationID);


                    bool B_SalesEnquiry = myFunctions.CheckPermission(N_CompanyID, 724, "Administrator", dLayer, connection, transaction);


                    // Auto Gen
                    string QuotationNo = "";
                    var values = MasterTable.Rows[0]["x_QuotationNo"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", Master["n_CompanyId"].ToString());
                        Params.Add("N_YearID", Master["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 80);
                        Params.Add("N_BranchID", Master["n_BranchId"].ToString());
                        QuotationNo = dLayer.GetAutoNumber("Inv_SalesQuotation", "x_QuotationNo", Params, connection, transaction);
                        if (QuotationNo == "") { return Ok(_api.Error("Unable to generate Quotation Number")); }
                        MasterTable.Rows[0]["x_QuotationNo"] = QuotationNo;


                        MasterTable.Columns.Remove("n_QuotationID");
                        DetailTable.Columns.Remove("n_QuotationDetailsID");
                        DetailTable.AcceptChanges();
                    }
                    else
                    {
                        if (N_QuotationID > 0)
                        {
                            SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",N_CompanyID},
                                {"X_TransType","Sales Quotation"},
                                {"N_VoucherID",N_QuotationID}};
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                        }
                    }
                    int MasterID = dLayer.SaveData("Inv_SalesQuotation", "N_QuotationId", N_QuotationID, MasterTable, connection, transaction);
                    if (MasterID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Quotation"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_QuotationID"] = MasterID;
                    }

                    int N_QuotationDetailId = dLayer.SaveData("Inv_SalesQuotationDetails", "n_QuotationDetailsID", 0, DetailTable, connection, transaction);
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
                        transaction.Commit();
                    }
                    return Ok(_api.Success("Sales quotation saved" + ":" + QuotationNo));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(ex));
            }
        }
        //Delete....
        [HttpDelete()]
        public ActionResult DeleteData(int N_QuotationID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Inv_SalesQuotation", "n_quotationID", N_QuotationID, "", connection, transaction);
                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return StatusCode(409, _api.Response(409, "Unable to delete sales quotation"));
                    }
                    else
                    {
                        dLayer.DeleteData("Inv_SalesQuotationDetails", "n_quotationID", N_QuotationID, "", connection, transaction);
                    }

                    if (Results > 0)
                    {
                        transaction.Commit();
                        return StatusCode(200, _api.Response(200, "Sales quotation deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return StatusCode(409, _api.Response(409, "Unable to delete sales quotation"));
                    }

                }
            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.ErrorResponse(ex));
            }


        }

    }
}