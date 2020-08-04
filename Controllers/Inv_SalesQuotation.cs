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
    [Route("salesquotation")]
    [ApiController]
    public class Inv_SalesQuotation : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        public Inv_SalesQuotation(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
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
        public ActionResult GetSalesQuotationDetails(int? nCompanyId, int nQuotationId, int nFnYearId, bool bAllBranchData, int nBranchID, int nCustomerID, int nFormID)
        {
            DataSet dsQuotation = new DataSet();
            DataTable dtProcess = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "";

            Params.Add("@nCompanyID", nCompanyId);
            Params.Add("@nFnYearID", nFnYearId);
            Params.Add("@nQuotationID", nQuotationId);
            Params.Add("@nCustomerID", nCustomerID);
            Params.Add("@nFormID", nFormID);

            if (bAllBranchData == true)
            {
                sqlCommandText = "select * from vw_InvSalesQuotationNo_Search where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_QuotationID=@nQuotationID";
            }
            else
            {
                sqlCommandText = "select * from vw_InvSalesQuotationNo_Search where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_QuotationID=@nQuotationID and N_BranchID=@nBranchID";
                Params.Add("@nBranchID", nBranchID);
            }


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DataTable dtQuotation = new DataTable();
                    dtQuotation = dLayer.ExecuteDataTable(sqlCommandText, Params);
                    dtQuotation = _api.Format(dtQuotation, "Master");
                    dsQuotation.Tables.Add(dtQuotation);

                    if (dsQuotation.Tables["Master"].Rows.Count == 0)
                        return Ok("There is no data!");

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

                    object objSalesOrder = dLayer.ExecuteScalar("Select N_SalesOrderID from Inv_SalesOrder Where N_CompanyID=@nCompanyID and N_QuotationID =@nQuotationID and B_IsSaveDraft=0", Params);
                    DataColumn col1 = new DataColumn("B_SalesOrderProcessed", typeof(Boolean));
                    col1.DefaultValue = false;
                    dsQuotation.Tables["Master"].Columns.Add(col1);
                    if (objSalesOrder != null)
                    {
                        if (myFunctions.getIntVAL(objSalesOrder.ToString()) > 0)
                        {
                            Params.Add("@nSalesOrderID", myFunctions.getIntVAL(objSalesOrder.ToString()));
                            dsQuotation.Tables["Master"].Rows[0][col1] = true;
                        }

                    }

                    object objDeliveryNote = dLayer.ExecuteScalar("select N_DeliveryNoteID from Inv_DeliveryNote where N_CompanyID=@nCompanyID and N_SalesOrderID=@nSalesOrderID and B_IsSaveDraft=0", Params);
                    DataColumn col2 = new DataColumn("B_DeliveryNoteProcessed", typeof(Boolean));
                    col2.DefaultValue = false;
                    dsQuotation.Tables["Master"].Columns.Add(col2);
                    if (objDeliveryNote != null)
                    {
                        if (myFunctions.getIntVAL(objDeliveryNote.ToString()) > 0)
                        {
                            Params.Add("@nDeliveryNoteID", myFunctions.getIntVAL(objDeliveryNote.ToString()));
                            dsQuotation.Tables["Master"].Rows[0][col2] = true;
                        }
                    }

                    object objSales = dLayer.ExecuteScalar("select N_SalesID from Inv_Sales where N_CompanyID=@nCompanyID and N_SalesOrderID=@nSalesOrderID and B_IsSaveDraft = 0", Params);
                    DataColumn col3 = new DataColumn("B_SalesProcessed", typeof(Boolean));
                    col3.DefaultValue = false;
                    dsQuotation.Tables["Master"].Columns.Add(col3);
                    if (objSales != null)
                    {
                        if (myFunctions.getIntVAL(objSales.ToString()) > 0)
                        {
                            Params.Add("@nSalesID", myFunctions.getIntVAL(objSales.ToString()));
                            dsQuotation.Tables["Master"].Rows[0][col3] = true;
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
                    SortedList ParamsAttachment=new SortedList();
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

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    //dLayer.setTransaction();
                    transaction = connection.BeginTransaction();
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
                        if (QuotationNo == "") { return StatusCode(409, _api.Response(409, "Unable to generate Quotation Number")); }
                        MasterTable.Rows[0]["x_QuotationNo"] = QuotationNo;
                    }


                    int N_QuotationId = dLayer.SaveData("Inv_SalesQuotation", "N_QuotationId", 0, MasterTable, connection, transaction);
                    if (N_QuotationId <= 0)
                    {
                        transaction.Rollback();
                        return StatusCode(409, _api.Response(409, "Unable to save Quotation"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_QuotationID"] = N_QuotationId;
                    }
                    int N_QuotationDetailId = dLayer.SaveData("Inv_SalesQuotationDetails", "n_QuotationDetailsID", 0, DetailTable, connection, transaction);
                    if (N_QuotationDetailId <= 0)
                    {
                        transaction.Rollback();
                        return StatusCode(409, _api.Response(409, "Unable to save Quotation"));
                    }
                    else
                    {
                        transaction.Commit();
                    }
                    return Ok("Sales quotation saved" + ":" + QuotationNo);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.ErrorResponse(ex));
            }
        }
        //Delete....
        [HttpDelete()]
        public ActionResult DeleteData(int N_QuotationID)
        {
            int Results = 0;
            try
            {
                dLayer.setTransaction();
                Results = dLayer.DeleteData("Inv_SalesQuotation", "n_quotationID", N_QuotationID, "");
                if (Results <= 0)
                {
                    dLayer.rollBack();
                    return StatusCode(409, _api.Response(409, "Unable to delete sales quotation"));
                }
                else
                {
                    dLayer.DeleteData("Inv_SalesQuotationDetails", "n_quotationID", N_QuotationID, "");
                }

                if (Results > 0)
                {
                    dLayer.commit();
                    return StatusCode(200, _api.Response(200, "Sales quotation deleted"));
                }
                else
                {
                    dLayer.rollBack();
                    return StatusCode(409, _api.Response(409, "Unable to delete sales quotation"));
                }

            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.ErrorResponse(ex));
            }


        }

    }
}