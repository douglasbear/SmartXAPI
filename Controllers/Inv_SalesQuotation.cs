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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Data Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return BadRequest(_api.Error(e));
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

                    object objFollowup = dLayer.ExecuteScalar("Select  isnull(max(N_id),0) from vsa_appointment where n_refid = @nQuotationID and N_companyID=@nCompanyID and B_IsComplete=0", Params);
                    if (objFollowup != null)
                    {
                        Params.Add("@nRefTypeID", 8);
                        DataTable dtFollowUp = new DataTable();
                        string qry = "Select * from vw_QuotaionFollowup Where N_CompanyID=@nCompanyID and N_RefTypeID=@nRefTypeID and N_RefID= @nQuotationID";
                        dtFollowUp = dLayer.ExecuteDataTable(qry, Params,connection);
                        dtFollowUp = _api.Format(dtFollowUp, "FollowUp");
                        dsQuotation.Tables.Add(dtFollowUp);
                    }


                    //object objSalesOrder = dLayer.ExecuteScalar("Select N_SalesOrderID from Inv_SalesOrder Where N_CompanyID=@nCompanyID and N_QuotationID =@nQuotationID and B_IsSaveDraft=0", Params);
                    object objSalesOrder = myFunctions.checkProcessed("Inv_SalesOrder", "N_SalesOrderID", "N_QuotationID", "@nQuotationID", "N_CompanyID=@nCompanyID and B_IsSaveDraft=0", Params, dLayer, connection);
                    Master =myFunctions.AddNewColumnToDataTable(Master,"B_SalesOrderProcessed", typeof(Boolean),false);
                    Master =myFunctions.AddNewColumnToDataTable(Master,"B_DeliveryNoteProcessed", typeof(Boolean),false);
                    Master =myFunctions.AddNewColumnToDataTable(Master,"B_SalesProcessed", typeof(Boolean),false);
                    if (objSalesOrder.ToString() != "")
                    {
                        if (myFunctions.getIntVAL(objSalesOrder.ToString()) > 0)
                        {
                            Params.Add("@nSalesOrderID", myFunctions.getIntVAL(objSalesOrder.ToString()));
                            Master.Rows[0]["B_SalesOrderProcessed"] = true;
                            
                            object objDeliveryNote = myFunctions.checkProcessed("Inv_DeliveryNote", "N_DeliveryNoteID", "N_SalesOrderID", "@nSalesOrderID", "N_CompanyID=@nCompanyID and B_IsSaveDraft=0", Params, dLayer, connection);
                            
                            if (objDeliveryNote.ToString() != "")
                            {
                                if (myFunctions.getIntVAL(objDeliveryNote.ToString()) > 0)
                                {
                                    Params.Add("@nDeliveryNoteID", myFunctions.getIntVAL(objDeliveryNote.ToString()));
                                     Master.Rows[0]["B_DeliveryNoteProcessed"] = true;
                                }
                            }


                            object objSales = myFunctions.checkProcessed("Inv_Sales", "N_SalesID", "N_SalesOrderID", "@nSalesOrderID", "N_CompanyID=@nCompanyID and B_IsSaveDraft=0", Params, dLayer, connection);
                            if (objSales.ToString() != "")
                            {
                                if (myFunctions.getIntVAL(objSales.ToString()) > 0)
                                {
                                    Params.Add("@nSalesInvID", myFunctions.getIntVAL(objSales.ToString()));
                                    Master.Rows[0]["B_SalesProcessed"] = true;
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
                            if (BaseUnitQty != null){
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
                                if(BaseUnitQty==null){BaseUnitQty=0;}
                                object LastPurchaseCost = dLayer.ExecuteScalar("Select TOP(1) ISNULL(N_LPrice,0) from Inv_StockMaster Where N_ItemID=@nItemID and N_CompanyID=@nCompanyID and N_LocationID=@nLocationID and (X_Type='Purchase' or X_Type='Opening') Order by N_StockID Desc", Params, connection);
                                if (LastPurchaseCost != null)
                                    var["LastPurchasePrice"] = (myFunctions.getVAL(LastPurchaseCost.ToString()) * myFunctions.getIntVAL(BaseUnitQty.ToString())).ToString(myFunctions.decimalPlaceString(myCompanyID.DecimalPlaces));
                            }


                        }

                    }
                        if(Master.Rows.Count==0 || Details.Rows.Count==0){
                            return Ok(_api.Notice("No data found"));
                        }
                        dsQuotation.Tables.Add(Master);
                        dsQuotation.Tables.Add(Details);

                    return Ok(_api.Success(dsQuotation));
                }
            }
            catch (Exception e)
            {
                return BadRequest(_api.ErrorResponse(e));
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

                        MasterTable.Columns.Remove("n_QuotationId");
                        MasterTable.AcceptChanges();



                    N_QuotationID = dLayer.SaveData("Inv_SalesQuotation", "N_QuotationId", N_QuotationID, MasterTable, connection, transaction);
                    if (N_QuotationID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Quotation"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_QuotationID"] = N_QuotationID;
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


        //  [HttpGet("getItem")]
        // public ActionResult GetItem(int nCompanyID,int locationID,int branchID,string date,string itemCode,int sPriceID, bool selected)
        // {
        //     string ItemClass = "", ItemCondition = "", Sprice = "", Pprice = "";
        //     object subItemPrice, subPprice, subMrp;
        //     int row = flxSales.Row;
        //     double SpriceSum = 0, PpriceSum = 0, Mrpsum = 0;
        //     int ClassRow = 0;
        //     string sql = "";
        //     int SPrice_Id = myFunctions.getIntVAL(flxSales.get_TextMatrix(row, mcUpdatedSPriceID));

        //     if (InputVal == "") { txtGridTextBox.Focus(); return true; }


        //     ItemCondition = "[Item Code] ='" + InputVal + "'";

        //     if (B_BarcodeBilling)
        //         ItemCondition = "([Item Code] ='" + InputVal + "' OR X_Barcode ='" + InputVal + "')";
        //     //else if (selected) ItemCondition = "[Item Code] ='" + InputVal + "'";
        //     //else ItemCondition = "[Item Code] like '%" + InputVal + "%'";


        //     if (dsSalesQuotation.Tables.Contains("Inv_ItemDetails"))
        //         dsSalesQuotation.Tables.Remove("Inv_ItemDetails");
        //     //dbo.SP_GenGetStock(vw_InvSalesOrderDetails.N_ItemID,@N_LocationID,'','branch') As N_Stock
        //     sql="Select * ,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID," + myCompanyID._LocationID + ",'','Location') As N_AvlStock,  dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as T_Stock, dbo.SP_GetQuotationCount(" + myCompanyID._CompanyID + ",vw_InvItem_Search.N_ItemID,'" + myFunctions.getDateVAL(dtpDate.Value.Date) + "') As QuotedQty, dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,'') As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=" + myCompanyID._CompanyID;
           
        //     if (B_SPRiceType)
        //     {
        //         if (SPrice_Id > 0)
        //         {
        //             sql = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID," + myCompanyID._LocationID + ",'','Location') As N_AvlStock , dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as T_Stock ,dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_ItemUnit) As N_LPrice ,dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as T_Stock, dbo.SP_GetQuotationCount(" + myCompanyID._CompanyID + ",vw_InvItem_Search.N_ItemID,'" + myFunctions.getDateVAL(dtpDate.Value.Date) + "') As QuotedQty,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID," + SPrice_Id + "," + myCompanyID._BranchID + ") As N_SPrice  From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=" + myCompanyID._CompanyID;
        //         }
        //         else
        //             sql = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID," + myCompanyID._LocationID + ",'','Location') As N_AvlStock, dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as T_Stock ,dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_ItemUnit) As N_LPrice ,dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as T_Stock, dbo.SP_GetQuotationCount(" + myCompanyID._CompanyID + ",vw_InvItem_Search.N_ItemID,'" + myFunctions.getDateVAL(dtpDate.Value.Date) + "') As QuotedQty,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID," + N_DefSPriceID + "," + myCompanyID._BranchID + ") As N_SPrice  From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=" + myCompanyID._CompanyID;
        //     }
        //     dba.FillDataSet(ref dsSalesQuotation, "Inv_ItemDetails", sql, "TEXT", new DataTable());
        //     if (dsSalesQuotation.Tables["Inv_ItemDetails"].Rows.Count == 1)
        //     {
        //         N_ItemID = myFunctions.getIntVAL(dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_ItemID"].ToString());
        //         //flxSales.set_TextMatrix(row, mcItemID, N_ItemID.ToString());
        //         object Mrp = dba.ExecuteSclar("Select top 1 N_Mrp from Inv_PurchaseDetails where N_ItemId=" + N_ItemID.ToString() + " and N_CompanyID=" + myCompanyID._CompanyID + " Order By N_PurchaseDetailsId desc", "TEXT", new DataTable());
        //         flxSales.set_TextMatrix(row, mcItemcode, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["Item Code"].ToString());
        //         flxSales.set_TextMatrix(row, mcDescription, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["Description"].ToString());
        //         flxSales.set_TextMatrix(row, mcItemClassID, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_ClassID"].ToString());

        //         flxSales.set_TextMatrix(row, mcPartNo, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["Part No"].ToString());

        //         string varUnit = dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["X_SalesUnit"].ToString();
        //         if (varUnit == "")
        //         {
        //             flxSales.set_TextMatrix(row, mcUnit, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["X_ItemUnit"].ToString());

        //         }
        //         else
        //         {
        //             flxSales.set_TextMatrix(row, mcUnit, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["X_SalesUnit"].ToString());
        //         }


        //         if (B_SPRiceType)
        //         {
        //             if (N_SPriceID == 0)
        //             {
        //                 object obj1 = dba.ExecuteSclar("Select isnull(Count(N_PriceID),0) from Inv_ItemPriceMaster where N_ItemID=" + N_ItemID.ToString() + " and N_CompanyID=" + myCompanyID._CompanyID + " and N_BranchID=" + myCompanyID._BranchID + " and N_PriceID=" + N_DefSPriceID.ToString() + " and N_PriceVal>0", "TEXT", new DataTable());
        //                 if (obj1 != null)
        //                 {
        //                     if (myFunctions.getIntVAL(obj1.ToString()) > 0)
        //                     {
        //                         flxSales.set_TextMatrix(row, mcUpdatedSPrice, X_DefSPriceType);
        //                         flxSales.set_TextMatrix(row, mcUpdatedSPriceID, N_DefSPriceID.ToString());
        //                     }
        //                     else
        //                     {
        //                         flxSales.set_TextMatrix(row, mcUpdatedSPrice, "");
        //                         flxSales.set_TextMatrix(row, mcUpdatedSPriceID, "0");
        //                     }
        //                 }
        //                 else
        //                 {
        //                     flxSales.set_TextMatrix(row, mcUpdatedSPrice, "");
        //                     flxSales.set_TextMatrix(row, mcUpdatedSPriceID, "0");
        //                 }
        //             }
        //         }

        //         flxSales.set_TextMatrix(row, mcTempItemID, N_ItemID.ToString());
        //         Pprice = Convert.ToDouble(dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_Lprice"].ToString()).ToString(myFunctions.decimalPlaceString(N_decimalPlace));
        //         flxSales.set_TextMatrix(row, mcBaseUnit, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["X_ItemUnit"].ToString());
        //         flxSales.set_TextMatrix(row, mcUnitSPrice, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_Sprice"].ToString());
        //         flxSales.set_TextMatrix(row, mcUnitCost, myFunctions.getVAL(dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_Lprice"].ToString()).ToString(myFunctions.decimalPlaceString(N_decimalPlace)));
        //         flxSales.set_TextMatrix(row, mcUnitQty, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_Qty"].ToString());
        //         flxSales.set_TextMatrix(row, mcMarginPerc, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_MinimumMargin"].ToString());
               
        //         if (B_ShowPurchaseCost)
        //             flxSales.set_TextMatrix(row, mcPurchaseCost, myFunctions.getVAL(dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_PurchaseCost"].ToString()).ToString(myFunctions.decimalPlaceString(N_decimalPlace)));
        //         else
        //             flxSales.set_TextMatrix(row, mcPurchaseCost, "");
        //         flxSales.set_TextMatrix(row, mcSPrice, myFunctions.getVAL(dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_Sprice"].ToString()).ToString(myFunctions.decimalPlaceString(N_decimalPlace)));
        //         ItemClass = dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["Item Class"].ToString();

        //         if (myFunctions.getVAL(dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_PurchaseCost"].ToString()) > 0 && myFunctions.getVAL(dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_MinimumMargin"].ToString()) > 0)
        //         {
        //             double N_PurchaseCost = myFunctions.getVAL(dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_PurchaseCost"].ToString());
        //             double N_MinimumMargin = myFunctions.getVAL(dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_MinimumMargin"].ToString());
        //             Sprice = (N_PurchaseCost + (N_PurchaseCost * N_MinimumMargin / 100)).ToString(myFunctions.decimalPlaceString(N_decimalPlace));
        //             flxSales.set_TextMatrix(row, mcUnitSPrice, Sprice);
        //         }
        //         else
        //             Sprice = dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_Sprice"].ToString();

        //         flxSales.set_TextMatrix(row, mcDelDays, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_DeliveryDays"].ToString());
        //         double stock = myFunctions.getVAL(dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_AvlStock"].ToString());
        //         flxSales.set_TextMatrix(row, mcStock, stock.ToString());
        //         double T_stock = myFunctions.getVAL(dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["T_Stock"].ToString());
        //         flxSales.set_TextMatrix(row, mcBranchStock, T_stock.ToString());

        //         flxSales.set_TextMatrix(row, mcQtyOnQuotn, myFunctions.getVAL(dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["QuotedQty"].ToString()).ToString());

        //         //TAX          
        //         flxSales.set_TextMatrix(row, mcTaxPerc1, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_TaxAmt"].ToString());
        //         flxSales.set_TextMatrix(row, mcTaxPerc2, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_TaxAmt2"].ToString());

        //         flxSales.set_TextMatrix(row, mcTaxCategoryId1, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_PkeyID"].ToString());
        //         flxSales.set_TextMatrix(row, mcTaxCategoryId2, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_TaxID2"].ToString());

        //         flxSales.set_TextMatrix(row, mcTaxCategoryName1, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["X_DisplayName"].ToString());
        //         flxSales.set_TextMatrix(row, mcTaxCategoryName2, dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["X_DisplayName2"].ToString());



        //         //QuotedQty

        //         if (B_LastSPrice)
        //         {
        //             object res;
        //             res = dba.ExecuteSclar("Select N_Qty from Inv_ItemUnit Where N_CompanyID=" + myCompanyID._CompanyID + " and N_ItemID = " + N_ItemID + " and X_ItemUnit='" + dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["X_SalesUnit"].ToString() + "' ", "TEXT", new DataTable());
        //             if (res != null)
        //                 SetLastSellingPrice(flxSales.Row, myFunctions.getVAL(res.ToString()), N_ItemID);
        //             else
        //                 SetLastSellingPrice(flxSales.Row, myFunctions.getVAL("1"), N_ItemID);
        //         }

        //         if (B_LastPurchaseCost)
        //         {
        //             object LastPurchaseCost = dba.ExecuteSclar("Select TOP(1) ISNULL(N_LPrice,0) from Inv_StockMaster Where N_ItemID=" + N_ItemID.ToString() + " and N_CompanyID=" + myCompanyID._CompanyID + " and N_LocationID=" + myCompanyID._LocationID + " and (X_Type='Purchase' or X_Type='Opening') Order by N_StockID Desc", "TEXT", new DataTable());
        //             if (LastPurchaseCost != null)
        //                 flxSales.set_TextMatrix(row, mcLastPurchasePrice, myFunctions.getVAL(LastPurchaseCost.ToString()).ToString(myFunctions.decimalPlaceString(N_decimalPlace)));
        //             else
        //                 flxSales.set_TextMatrix(row, mcLastPurchasePrice, "0.00");
        //         }


        //         if (ItemClass != "Group Item")
        //         {
        //             flxSales.set_TextMatrix(row, mcItemID, N_ItemID.ToString());
        //         }
        //         if (ItemClass == "Group Item")
        //         {
        //             ClassRow = row;
        //             if (dsSalesQuotation.Tables.Contains("Sub Items"))
        //                 dsSalesQuotation.Tables.Remove("Sub Items");
        //             dba.FillDataSet(ref dsSalesQuotation, "Sub Items", "Select * from vw_invitemdetails where N_MainItemId=" + N_ItemID.ToString() + " and N_CompanyID=" + myCompanyID._CompanyID + " order by X_Itemname", "TEXT", new DataTable());
        //             foreach (DataRow var in dsSalesQuotation.Tables["Sub Items"].Rows)
        //             {
        //                 row += 1;
        //                 if (row + 1 >= flxSales.Rows) flxSales.Rows = flxSales.Rows + 1;
        //                 flxSales.set_TextMatrix(row, mcDescription, var["X_Itemname"].ToString());
        //                 flxSales.set_TextMatrix(row, mcQuantity, var["N_Qty"].ToString());
        //                 flxSales.set_TextMatrix(row, mcUnit, var["X_ItemUnit"].ToString());
        //                 flxSales.set_TextMatrix(row, mcSubItemId, var["N_ItemDetailsID"].ToString());
        //                 flxSales.set_TextMatrix(row, mcItemcode, var["X_ItemCode"].ToString());
        //                 flxSales.set_TextMatrix(row, mcSubQty, var["N_Qty"].ToString());
        //                 flxSales.set_TextMatrix(row, mcDelDays, var["N_DeliveryDays"].ToString());
        //                 if (var["N_ItemDetailsID"].ToString() == "")
        //                 {
        //                     flxSales.set_TextMatrix(row, mcItemID, "");
        //                 }
        //                 else
        //                 {
        //                     flxSales.set_TextMatrix(row, mcItemID, var["N_ItemId"].ToString());
        //                     subItemPrice = dba.ExecuteSclar("Select top 1 N_Sprice from Inv_StockMaster where N_ItemId=" + var["N_ItemId"].ToString() + " and N_CompanyID=" + myCompanyID._CompanyID + " order by n_stockid desc", "TEXT", new DataTable());
        //                     subPprice = dba.ExecuteSclar("Select top 1 N_Sprice from Inv_StockMaster where N_ItemId=" + var["N_ItemId"].ToString() + " and N_CompanyID=" + myCompanyID._CompanyID + " order by n_stockid desc", "TEXT", new DataTable());
        //                     subMrp = dba.ExecuteSclar("Select top 1 N_Mrp from Inv_PurchaseDetails where N_ItemId=" + var["N_ItemId"].ToString() + " and N_CompanyID=" + myCompanyID._CompanyID + " Order By N_PurchaseDetailsId desc", "TEXT", new DataTable());
        //                     if (subItemPrice != null) SpriceSum = myFunctions.getVAL(subItemPrice.ToString()) * myFunctions.getVAL(var["N_Qty"].ToString()) + SpriceSum; else flxSales.set_TextMatrix(row, mcSPrice, "0.00");
        //                     if (subPprice != null) PpriceSum = myFunctions.getVAL(subPprice.ToString()) + PpriceSum; else flxSales.set_TextMatrix(row, mcPPrice, "0.00");
        //                     if (subMrp != null) Mrpsum = myFunctions.getVAL(subMrp.ToString()) + Mrpsum; else flxSales.set_TextMatrix(row, mcMRP, "0.00");
        //                 }
        //             }
        //             flxSales.set_TextMatrix(ClassRow, mcSPrice, SpriceSum.ToString(myFunctions.decimalPlaceString(N_decimalPlace)));
        //             flxSales.set_TextMatrix(ClassRow, mcPPrice, PpriceSum.ToString(myFunctions.decimalPlaceString(N_decimalPlace)));
        //             flxSales.set_TextMatrix(ClassRow, mcMRP, Mrpsum.ToString(myFunctions.decimalPlaceString(N_decimalPlace)));
        //         }
        //         else
        //         {
        //             if (Mrp != null) { flxSales.set_TextMatrix(row, mcMRP, myFunctions.getVAL(Mrp.ToString()).ToString(myFunctions.decimalPlaceString(N_decimalPlace))); }
        //             flxSales.set_TextMatrix(row, mcPPrice, myFunctions.getVAL(Pprice).ToString(myFunctions.decimalPlaceString(N_decimalPlace)));
        //             //flxSales.set_TextMatrix(row, mcSPrice, myFunctions.getVAL(Sprice).ToString(myFunctions.decimalPlaceString(N_decimalPlace)));


        //             flxSales.set_TextMatrix(row, mcSPrice, (myFunctions.getVAL(Sprice) * myFunctions.getVAL(dsSalesQuotation.Tables["Inv_ItemDetails"].Rows[0]["N_SalesUnitQty"].ToString())).ToString(myFunctions.decimalPlaceString(N_decimalPlace)));

        //         }
        //         AllocateStockAndPrice(flxSales.Row);
                
        //         object value = dba.ExecuteSclar("select N_DiscPerc from inv_CustomerDiscount where N_ProductID = '" + N_Value + "' and N_CustomerID = '" + N_CustomerID + "' and N_CompanyID = '" + myCompanyID._CompanyID + "'", "TEXT", new DataTable());
        //     if (value != null)
        //     {
        //         flxSales.set_TextMatrix(Row, mcCustDiscount, value.ToString());
        //     }
        //     else
        //     {
        //         flxSales.set_TextMatrix(Row, mcDiscount, "0.00");
        //         flxSales.set_TextMatrix(Row, mcCustDiscount, "0.00");
        //     }


        //         if (row + 1 >= flxSales.Rows) { flxSales.Rows += 1; }
        //         if (B_BarcodeBilling)
        //         {
        //             flxSales.set_TextMatrix(row, mcQuantity, "1");
        //             CalculateRowTotal(row);
        //             Checkpurchasecost(flxSales.Row);
        //             CalculateGridTotal();
        //             CalculateGrandTotal();
        //         }
        //         return true;
        //     }
        //     else
        //     {
        //         return false;
        //     }
        // }


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
                        return Ok(_api.Error("Unable to delete sales quotation"));
                    }
                    else
                    {
                        dLayer.DeleteData("Inv_SalesQuotationDetails", "n_quotationID", N_QuotationID, "", connection, transaction);
                    }

                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok( _api.Success("Sales quotation deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error( "Unable to delete sales quotation"));
                    }

                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.ErrorResponse(ex));
            }


        }

    }
}