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
    [Route("salesreturn")]
    [ApiController]
    public class Inv_SalesReturn : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Inv_SalesReturn(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");

        }


        [HttpGet("list")]
        public ActionResult GetSalesReturn(int? nCompanyId, int nFnYearId,int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
            string sqlCommandCount="";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and X_DebitNoteNo like '%" + xSearchkey + "%' or X_CustomerName like '%"+ xSearchkey + "%'";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_DebitNoteId desc";
            else
                xSortBy = " order by " + xSortBy;

            if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_InvDebitNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_InvDebitNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and N_DebitNoteId not in(select top("+ Count +")  N_DebitNoteId from vw_InvDebitNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + xSortBy + " ) " + xSortBy;
            
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from vw_InvDebitNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details",_api.Format(dt));
                    OutPut.Add("TotalCount",TotalCount);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(OutPut));
                }  
            }
            catch (Exception e)
            {
                return Ok( _api.Error(e));
            }
        }
        [HttpGet("listdetails")]
        public ActionResult GetSalesReturnDetails(int? nCompanyId, string xDebitNoteNo, string xReceiptNo, int nFnYearId, bool bAllBranchData, int nBranchId, bool bDeliveryNote = false)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            string sqlCommandText = "";
            string X_type = "";



            if (bDeliveryNote)
                X_type = "DELIVERY";
            else
                X_type = "SALES";
            if (bAllBranchData == true)
            {
                if (xReceiptNo != "" && xReceiptNo != null)
                {
                    sqlCommandText = "SP_InvSalesReturn_Disp @CompanyID,@RcptNo,0,@Xtype,0,@FnYearID";
                    Params.Add("@RcptNo", xReceiptNo);
                }
                else
                {
                    sqlCommandText = "SP_InvSalesReturn_Disp @CompanyID,@RcptNo,1,@Xtype,0,@FnYearID";
                    Params.Add("@RcptNo", xDebitNoteNo);
                }
            }
            else
            {
                if (xReceiptNo != "" && xReceiptNo != null)
                {
                    sqlCommandText = "SP_InvSalesReturn_Disp @CompanyID,@RcptNo,0,@Xtype,@BranchID,@FnYearID";
                    Params.Add("@RcptNo", xReceiptNo);
                }
                else
                {
                    sqlCommandText = "SP_InvSalesReturn_Disp @CompanyID,@RcptNo,1,@Xtype,@BranchID,@FnYearID";
                    Params.Add("@RcptNo", xDebitNoteNo);
                }
            }

            Params.Add("@CompanyID", nCompanyId);
            Params.Add("@FnYearID", nFnYearId);
            Params.Add("@BranchID", nBranchId);
            Params.Add("@Xtype", X_type);

            try
            {
                DataTable SalesReturn = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //SqlTransaction transaction;

                    object Res = dLayer.ExecuteScalar("Select B_Invoice from Inv_SalesReturnMaster where X_DebitNoteNo=" + xDebitNoteNo + " and N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId, Params, connection);
                    if (myFunctions.getBoolVAL(Res.ToString()) == false)
                    {
                        if (bAllBranchData == true)
                        {
                            sqlCommandText = "Select * from vw_SalesReturnMasterWithoutSale_Disp Where N_CompanyID=@CompanyID and X_DebitNoteNo=@RcptNo and N_FnYearID=@FnYearID and B_Invoice=0";
                        }
                        else
                        {
                            sqlCommandText = "Select * from vw_SalesReturnMasterWithoutSale_Disp Where N_CompanyID=@CompanyID and X_DebitNoteNo=@RcptNo and N_FnYearID=@FnYearID and B_Invoice=0 and N_BranchID=@BranchID";
                        }

                    }


                    SalesReturn = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SalesReturn = _api.Format(SalesReturn, "Master");
                    dt.Tables.Add(SalesReturn);

                    int N_DebitNoteId = myFunctions.getIntVAL(SalesReturn.Rows[0]["N_DebitNoteId"].ToString());
                    Params.Add("@DebitNoteID", N_DebitNoteId);

                    string sqlCommandText2 = "Select * from vw_InvSalesRetunEdit Where N_CompanyID=@CompanyID and N_FnYearID=@FnYearID and N_DebitNoteId=@DebitNoteID and N_RetQty<>0";
                    if (myFunctions.getBoolVAL(Res.ToString()) == false)
                    {
                        sqlCommandText2 = "SELECT   * from vw_SalesReturnWithoutSale_Disp Where N_DebitNoteId=" + N_DebitNoteId + " and N_CompanyID=@CompanyID and N_FnYearID=@FnYearID";
                    }
                    DataTable SalesReturnDetails = new DataTable();
                    SalesReturnDetails = dLayer.ExecuteDataTable(sqlCommandText2, Params, connection);
                    SalesReturnDetails = _api.Format(SalesReturnDetails, "Details");
                    dt.Tables.Add(SalesReturnDetails);

                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok( _api.Error(e));
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
                var values = masterRow["X_DebitNoteNo"].ToString();
                int UserID = myFunctions.GetUserID(User);
                int N_InvoiceId=0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
                        Params.Add("N_YearID", masterRow["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 80);
                        Params.Add("N_BranchID", masterRow["n_BranchId"].ToString());
                        InvoiceNo = dLayer.GetAutoNumber("Inv_SalesReturnMaster", "X_DebitNoteNo", Params, connection, transaction);
                        if (InvoiceNo == "") { return Ok(_api.Error("Unable to generate Return Number")); }
                        MasterTable.Rows[0]["X_DebitNoteNo"] = InvoiceNo;
                    }

                    // dLayer.setTransaction();
                    N_InvoiceId = dLayer.SaveData("Inv_SalesReturnMaster", "N_DebitNoteId", MasterTable, connection, transaction);
                    if (N_InvoiceId <= 0)
                    {
                        transaction.Rollback();
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_DebitNoteId"] = N_InvoiceId;
                    }
                    int N_InvoiceDetailId = dLayer.SaveData("Inv_SalesReturnDetails", "N_DebitnoteDetailsID", DetailTable, connection, transaction);

                    transaction.Commit();
                }
                 SortedList Result = new SortedList();
                Result.Add("n_SalesReturnID",N_InvoiceId);
                Result.Add("x_SalesReturnNo",InvoiceNo);
                return Ok(_api.Success(Result,"Sales Return Saved"));

            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }
        //Delete....
        [HttpDelete()]
        public ActionResult DeleteData(int? nCompanyId, int? nDebitNoteId)
        {
            try
            {
                string sqlCommandText = "";
                SortedList Params = new SortedList();
                sqlCommandText = "SP_Delete_Trans_With_SaleAccounts  @N_CompanyId,'SALES RETURN',@N_DebitNoteId";
                Params.Add("@N_CompanyId", nCompanyId);

                Params.Add("@N_DebitNoteId", nDebitNoteId);
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object objPaymentProcessed = dLayer.ExecuteScalar("Select Isnull(N_PayReceiptId,0) from Inv_PayReceiptDetails where N_InventoryId=" + nDebitNoteId +" and X_TransType='SALES RETURN'" , connection);
                    if (objPaymentProcessed == null)
                        objPaymentProcessed = 0;
                        if(myFunctions.getIntVAL(objPaymentProcessed.ToString()) == 0 )
                            dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                        else
                            return Ok(_api.Error("Payment processed! Unable to delete"));

                    
                }
                return Ok(_api.Success("Sales Return deleted"));
                //return StatusCode(200, _api.Response(200, "Sales Return deleted"));




            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }

        }


        [HttpGet("salesreturnpendinglist")]
        public ActionResult GetReturnPendingList(int? nCompanyId, int nCustomerId, bool bAllBranchData, int nBranchId, int nLocationId)
        {
            SortedList Params = new SortedList();

            string crieteria = "";


            if (bAllBranchData == true)
            {
                if (nCustomerId > 0)
                    crieteria = " where X_TransType='SALES' and N_SalesType = 0 and N_CustomerID=@nCustomerId and N_CompanyID=@nCompanyId and B_IsSaveDraft=0 and N_balanceQty>0";
                else
                    crieteria = " where X_TransType='SALES' and N_SalesType = 0 and N_CompanyID=@nCompanyId and B_IsSaveDraft=0 and N_balanceQty>0";
            }
            else
            {
                if (nCustomerId > 0)
                    crieteria = " where X_TransType='SALES' and N_SalesType = 0 and N_CustomerID=@nCustomerId and N_CompanyID=@nCompanyId and N_BranchID=@nBranchId and N_LocationID=@nLocationId and B_IsSaveDraft=0 and N_balanceQty>0 ";
                else
                    crieteria = " where X_TransType='SALES' and N_SalesType = 0 and N_CompanyID=@nCompanyId and N_BranchID=@nBranchId and N_LocationID=@nLocationId and B_IsSaveDraft=0 and N_balanceQty>0 ";
            }

            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nCustomerId", nCustomerId);
            Params.Add("@bAllBranchData", bAllBranchData);
            Params.Add("@nBranchId", nBranchId);
            Params.Add("@nLocationId", nLocationId);
            string sqlCommandText = "select [Invoice No],[Invoice Date],[Customer] as X_CustomerName,X_CustPONo,X_BranchName,N_CompanyID,N_CustomerID,N_SalesID,N_SalesType,X_TransType,N_FnYearID,N_BranchID,X_LocationName,N_LocationID,B_IsSaveDraft,N_balanceQty from vw_InvSalesReturnPending_Search " + crieteria + " order by N_SalesID DESC,[Invoice No]";
            try
            {
                DataTable SalesRetunPList = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SalesRetunPList = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SalesRetunPList = _api.Format(SalesRetunPList);
                    if (SalesRetunPList.Rows.Count == 0) { return Ok(_api.Notice("No Sales Return Pending List Found")); }
                }
                return Ok(_api.Success(SalesRetunPList));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }


    }
}