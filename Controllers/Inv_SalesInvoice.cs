using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

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
        private readonly string connectionString;
        public Inv_SalesInvoice(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("list")]
        public ActionResult GetSalesInvoiceList(int? nCompanyId, int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_InvSalesInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params);
                    dt = _api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        return Ok(dt);
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        [HttpGet("details")]
        public ActionResult GetSalesInvoiceDetails(int nCompanyId, int nFnYearId, int nBranchId, string xInvoiceNo)
        {

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
                    QueryParamsList.Add("@xInvoiceNo", xInvoiceNo);
                    QueryParamsList.Add("@xTransType", "SALES");

                    SortedList mParamsList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"X_ReceiptNo",xInvoiceNo},
                        {"X_TransType","SALES"},
                        {"N_FnYearID",nFnYearId},
                        {"N_BranchId",nBranchId}
                    };
                    DataTable masterTable = dLayer.ExecuteDataTablePro("SP_InvSales_Disp", mParamsList);
                    masterTable = _api.Format(masterTable, "Master");
                    if (masterTable.Rows.Count == 0) { return Ok(new { }); }
                    DataRow MasterRow = masterTable.Rows[0];

                    QueryParamsList.Add("@nSalesID", myFunctions.getIntVAL(MasterRow["N_TruckID"].ToString()));
                    int N_TruckID = myFunctions.getIntVAL(MasterRow["N_TruckID"].ToString());
                    object objPlateNo = null;
                    if (N_TruckID > 0)
                    {
                        DataColumn ColPlateNo = new DataColumn("X_PlateNo", typeof(string));
                        ColPlateNo.DefaultValue = "";
                        masterTable.Columns.Add(ColPlateNo);
                        QueryParamsList.Add("@nTruckID", myFunctions.getIntVAL(masterTable.Rows[0]["N_TermsID"].ToString()));
                        objPlateNo = dLayer.ExecuteScalar("Select X_PlateNumber from Inv_TruckMaster where N_TruckID=@nTruckID and N_companyID=@nCompanyID", mParamsList, Con);
                        if (objPlateNo != null)
                            masterTable.Rows[0][ColPlateNo] = objPlateNo.ToString();

                    }

                    if (masterTable.Rows[0]["X_TandC"].ToString() == "")
                        masterTable.Rows[0]["X_TandC"] = myFunctions.ReturnSettings("64", "TermsandConditions", "X_Value", "N_UserCategoryID", "0", mParamsList, dLayer, Con);

                    int N_TermsID = myFunctions.getIntVAL(masterTable.Rows[0]["N_TermsID"].ToString());
                    if (N_TermsID > 0)
                    {
                        QueryParamsList.Add("@nTermsID", myFunctions.getIntVAL(masterTable.Rows[0]["N_TermsID"].ToString()));
                        DataColumn ColTerms = new DataColumn("X_Terms", typeof(string));
                        ColTerms.DefaultValue = "";
                        masterTable.Columns.Add(ColTerms);
                        masterTable.Rows[0][ColTerms] = myFunctions.ReturnValue("Inv_Terms", "X_Terms", "N_TermsID =@nTermsID and N_CompanyID =@nCompanyID", mParamsList, dLayer, Con);
                    }

                    if (myFunctions.getIntVAL(masterTable.Rows[0]["N_DeliveryNoteId"].ToString()) > 0)
                    {
                        QueryParamsList.Add("@nDeliveryNoteId", myFunctions.getIntVAL(masterTable.Rows[0]["N_DeliveryNoteId"].ToString()));
                        DataColumn ColFileNo = new DataColumn("X_FileNo", typeof(string));
                        ColFileNo.DefaultValue = "";
                        masterTable.Columns.Add(ColFileNo);

                        SortedList ProParamList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"N_FnYearID",nFnYearId},
                        {"N_PkID",myFunctions.getIntVAL(masterTable.Rows[0]["N_DeliveryNoteId"].ToString())},
                        {"Type","DN"}
                    };
                        object objFileNo = dLayer.ExecuteScalarPro("SP_GetSalesOrder", ProParamList, Con);
                        if (objFileNo != null)
                            masterTable.Rows[0][ColFileNo] = objFileNo.ToString();
                    }




                    object objPayment = dLayer.ExecuteScalar("SELECT dbo.Inv_PayReceipt.X_Type, dbo.Inv_PayReceiptDetails.N_InventoryId,Inv_PayReceiptDetails.N_Amount FROM  dbo.Inv_PayReceipt INNER JOIN dbo.Inv_PayReceiptDetails ON dbo.Inv_PayReceipt.N_PayReceiptId = dbo.Inv_PayReceiptDetails.N_PayReceiptId Where dbo.Inv_PayReceipt.X_Type='SR' and dbo.Inv_PayReceiptDetails.N_InventoryId=@nSalesID", QueryParamsList);
                    if (objPayment != null)
                        myFunctions.AddNewColumnToDataTable(masterTable, "B_PaymentProcessed", typeof(Boolean), myFunctions.getBoolVAL(objPayment.ToString()));

                    //sales return count(draft and non draft)
                    object objSalesReturn = dLayer.ExecuteScalar("select Isnull(Count(N_DebitNoteId),0) from Inv_SalesReturnMaster where N_SalesId =@nSalesID and B_IsSaveDraft=0 and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", QueryParamsList);
                    if (objSalesReturn != null)
                        myFunctions.AddNewColumnToDataTable(masterTable, "N_SalesReturn", typeof(int), myFunctions.getIntVAL(objSalesReturn.ToString()));

                    object objSalesReturnDraft = dLayer.ExecuteScalar("select Isnull(Count(N_DebitNoteId),0) from Inv_SalesReturnMaster where N_SalesId =@nSalesID and B_IsSaveDraft=1 and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", QueryParamsList);
                    if (objSalesReturnDraft != null)
                        myFunctions.AddNewColumnToDataTable(masterTable, "N_SalesReturnDraft", typeof(int), myFunctions.getIntVAL(objSalesReturnDraft.ToString()));

                    object obPaymentMenthodid = dLayer.ExecuteScalar("Select N_TypeID From vw_InvCustomer Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID and (N_BranchID=0 or N_BranchID=@nBranchID) and B_Inactive = 0", QueryParamsList);
                    if (obPaymentMenthodid != null)
                    {
                        QueryParamsList.Add("@nPaymentMethodID", myFunctions.getIntVAL(obPaymentMenthodid.ToString()));
                        myFunctions.AddNewColumnToDataTable(masterTable, "N_PaymentMethodID", typeof(int), myFunctions.getIntVAL(obPaymentMenthodid.ToString()));
                        myFunctions.AddNewColumnToDataTable(masterTable, "X_PaymentMethod", typeof(string), myFunctions.ReturnValue("Inv_CustomerType", "X_TypeName", "N_TypeID =@nPaymentMethodID", mParamsList, dLayer, Con));
                    }

                    string qry = "";
                    bool B_DeliveryDispatch = myFunctions.CheckPermission(nCompanyId, 948, "Administrator", dLayer);
                    if (B_DeliveryDispatch)
                    {
                        DataTable dtDispatch = new DataTable();
                        qry = "Select * From Inv_DeliveryDispatch Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_InvoiceID=@nSalesID";
                        dtDispatch = dLayer.ExecuteDataTable(qry, QueryParamsList);
                        dtDispatch = _api.Format(dtDispatch, "Delivery Dispatch");
                        dsSalesInvoice.Tables.Add(dtDispatch);
                    }

                    DataTable dtPayment = new DataTable();
                    string qry1 = "SELECT  dbo.Inv_PayReceipt.X_VoucherNo FROM  dbo.Inv_PayReceipt INNER JOIN dbo.Inv_PayReceiptDetails ON dbo.Inv_PayReceipt.N_PayReceiptId = dbo.Inv_PayReceiptDetails.N_PayReceiptId Where dbo.Inv_PayReceipt.X_Type='SR' and dbo.Inv_PayReceiptDetails.N_InventoryId =@nSalesID";
                    dtPayment = dLayer.ExecuteDataTable(qry1, QueryParamsList);
                    string InvoiceNos = "";
                    foreach (DataRow var in dtPayment.Rows)
                        InvoiceNos += var["X_VoucherNo"].ToString() + " , ";




                    //invoice status
                    DataTable dtStatus = new DataTable();
                    object objInvoiceRecievable = null, objBal = null;
                    double N_InvoiceRecievable = 0, N_BalanceAmt = 0;

                    objInvoiceRecievable = dLayer.ExecuteScalar("SELECT isnull((Inv_Sales.N_BillAmt-Inv_Sales.N_DiscountAmt + Inv_Sales.N_FreightAmt +isnull(Inv_Sales.N_OthTaxAmt,0)+ Inv_Sales.N_TaxAmt),0) as N_InvoiceAmount FROM Inv_Sales where Inv_Sales.N_SalesId=@nSalesID and Inv_Sales.N_CompanyID=@nCompanyID", QueryParamsList);
                    objBal = dLayer.ExecuteScalar("SELECT SUM(N_BalanceAmount) from  vw_InvReceivables where N_SalesId=@nSalesID and X_Type= @xTransType and N_CompanyID=@nCompanyID", QueryParamsList);
                    if (objInvoiceRecievable != null)
                        myFunctions.AddNewColumnToDataTable(masterTable, "N_InvoiceRecievable", typeof(double), N_InvoiceRecievable);
                    if (objBal != null)
                        myFunctions.AddNewColumnToDataTable(masterTable, "N_BalanceAmt", typeof(double), N_BalanceAmt);

                    dsSalesInvoice.Tables.Add(dtStatus);




                    //Details
                    SortedList dParamList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"N_SalesID",masterTable.Rows[0]["n_SalesId"].ToString()}
                    };
                    DataTable detailTable = dLayer.ExecuteDataTablePro("SP_InvSalesDtls_Disp", dParamList);
                    detailTable = _api.Format(detailTable, "Details");
                    if (detailTable.Rows.Count == 0) { return Ok(new { }); }
                    dsSalesInvoice.Tables.Add(masterTable);
                    dsSalesInvoice.Tables.Add(detailTable);

                    return Ok(dsSalesInvoice);

                }
            }
            catch (Exception e)
            {
                return StatusCode(403, _api.ErrorResponse(e));
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
                // Auto Gen
                string InvoiceNo = "";
                DataRow masterRow = MasterTable.Rows[0];
                var values = masterRow["x_ReceiptNo"].ToString();

                if (values == "@Auto")
                {
                    Params.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
                    Params.Add("N_YearID", masterRow["n_FnYearId"].ToString());
                    Params.Add("N_FormID", 80);
                    Params.Add("N_BranchID", masterRow["n_BranchId"].ToString());
                    InvoiceNo = dLayer.GetAutoNumber("Inv_Sales", "x_ReceiptNo", Params);
                    if (InvoiceNo == "") { return StatusCode(409, _api.Response(409, "Unable to generate Invoice Number")); }
                    MasterTable.Rows[0]["x_ReceiptNo"] = InvoiceNo;
                }

                dLayer.setTransaction();
                int N_InvoiceId = dLayer.SaveData("Inv_Sales", "N_SalesId", 0, MasterTable);
                if (N_InvoiceId <= 0)
                {
                    dLayer.rollBack();
                }
                for (int j = 0; j < DetailTable.Rows.Count; j++)
                {
                    DetailTable.Rows[j]["N_SalesId"] = N_InvoiceId;
                }
                int N_InvoiceDetailId = dLayer.SaveData("Inv_SalesDetails", "n_SalesDetailsID", 0, DetailTable);
                dLayer.commit();
                return GetSalesInvoiceDetails(int.Parse(masterRow["n_CompanyId"].ToString()), int.Parse(masterRow["n_FnYearId"].ToString()), int.Parse(masterRow["n_BranchId"].ToString()), InvoiceNo);
            }
            catch (Exception ex)
            {
                dLayer.rollBack();
                return StatusCode(403, ex);
            }
        }
        //Delete....
        [HttpDelete()]
        public ActionResult DeleteData(int N_InvoiceID)
        {
            int Results = 0;
            try
            {
                dLayer.setTransaction();
                Results = dLayer.DeleteData("Inv_SalesInvoice", "n_InvoiceID", N_InvoiceID, "");
                if (Results <= 0)
                {
                    dLayer.rollBack();
                    return StatusCode(409, _api.Response(409, "Unable to delete sales Invoice"));
                }
                else
                {
                    dLayer.DeleteData("Inv_SalesInvoiceDetails", "n_InvoiceID", N_InvoiceID, "");
                }

                if (Results > 0)
                {
                    dLayer.commit();
                    return StatusCode(200, _api.Response(200, "Sales Invoice deleted"));
                }
                else
                {
                    dLayer.rollBack();
                    return StatusCode(409, _api.Response(409, "Unable to delete sales Invoice"));
                }

            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.ErrorResponse(ex));
            }


        }


        [HttpGet("dummy")]
        public ActionResult GetSalesInvoiceDummy(int? nSalesId)
        {
            try
            {
                string sqlCommandText = "select * from Inv_Sales where N_SalesId=@p1";
                SortedList mParamList = new SortedList() { { "@p1", nSalesId } };
                DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList);
                masterTable = _api.Format(masterTable, "master");

                string sqlCommandText2 = "select * from Inv_SalesDetails where N_SalesId=@p1";
                SortedList dParamList = new SortedList() { { "@p1", nSalesId } };
                DataTable detailTable = dLayer.ExecuteDataTable(sqlCommandText2, dParamList);
                detailTable = _api.Format(detailTable, "details");

                if (detailTable.Rows.Count == 0) { return Ok(new { }); }
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(masterTable);
                dataSet.Tables.Add(detailTable);

                return Ok(dataSet);

            }
            catch (Exception e)
            {
                return StatusCode(403, _api.ErrorResponse(e));
            }
        }





        [HttpGet("deliveryNoteSearch")]
        public ActionResult GetInvoiceList(int? nCompanyId,int nCustomerId,bool bAllBranchData,int nBranchId,int nLocationId)
        {
            SortedList Params=new SortedList();
            
            string crieteria="";


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
            
            Params.Add("@nCompanyId",nCompanyId);
            Params.Add("@nCustomerId",nCustomerId);
            Params.Add("@bAllBranchData",bAllBranchData);
            Params.Add("@nBranchId",nBranchId);
            Params.Add("@nLocationId",nLocationId);
            string sqlCommandText="select [Invoice No],[Invoice Date],[Customer] as X_CustomerName,N_CompanyID,N_CustomerID,N_DeliveryNoteId,N_DeliveryType,X_TransType,N_FnYearID,N_BranchID,X_LocationName,N_LocationID,B_IsSaveDraft from vw_InvDeliveryNote_Search "+ crieteria + " order by N_DeliveryNoteId DESC,[Invoice No]";
            try{
                DataTable SalesInvoiceList = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                connection.Open();
                SalesInvoiceList=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                SalesInvoiceList=_api.Format(SalesInvoiceList);
                if(SalesInvoiceList.Rows.Count==0){return Ok(_api.Notice("No Sales Invoices Found"));}
                }
                return Ok(_api.Success(SalesInvoiceList));
                }catch(Exception e){
                return BadRequest(_api.Error(e));
                }
        }


       

    }
}