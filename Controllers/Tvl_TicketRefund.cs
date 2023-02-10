using AutoMapper;
using SmartxAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("ticketRefund")]
    [ApiController]
    public class Tvl_TicketRefund : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID =1638 ;
        private readonly ITxnHandler txnHandler;

        public Tvl_TicketRefund(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf,ITxnHandler txn)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            txnHandler=txn;
        }        
       
        [HttpGet("details")]
        public ActionResult RefundDetails(string X_RefundNo)
        {
            DataSet dt = new DataSet();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_tvlTicketRefundDetails where N_CompanyID=@p1  and X_RefundNo=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", X_RefundNo);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }

                    MasterTable = api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);
                }
                return Ok(api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nTicketRefundID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TicketRefundID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    SortedList PResult = new SortedList();
                    SortedList SResult = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_RefundNo"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Tvl_TicketRefund", "X_RefundNo", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Refund Code")); }
                        MasterTable.Rows[0]["X_RefundNo"] = Code;
                    }
                    
                    if (nTicketRefundID > 0) 
                    {  
                        dLayer.DeleteData("Tvl_TicketRefund", "N_TicketRefundID", nTicketRefundID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    }

                    nTicketRefundID = dLayer.SaveData("Tvl_TicketRefund", "N_TicketRefundID", MasterTable, connection, transaction);
                    if (nTicketRefundID <= 0)
                    {                       
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        //-------------------------------Sales Return Save------------------------------------//
                        DataSet Sds= new DataSet();
                        DataTable SMasterDt = new DataTable();
                        DataTable SDetailsDt = new DataTable();
                        DataTable SAttachmentDt = new DataTable();
                        
                        int n_SIsCompleted=0;
                        string x_SMessage="";

                        string sqlSalesMaster= "SELECT     Tvl_TicketRefund.N_CompanyID, Tvl_TicketRefund.N_FnyearID, ISNULL(Inv_SalesReturnMaster.N_DebitNoteId, 0) AS N_DebitNoteId, ISNULL(Inv_SalesReturnMaster.X_DebitNoteNo, '@Auto') "+
                                                " AS X_DebitNoteNo, Inv_Sales.N_SalesId, Tvl_TicketRefund.D_RefundDate AS D_ReturnDate, Tvl_TicketRefund.N_userID AS N_UserId, 0 AS N_TotalPaidAmount, "+
                                                " Tvl_TicketRefund.N_CustRefund AS N_TotalReturnAmount, Tvl_TicketRefund.N_CustomerID, GETDATE() AS D_Entrydate, Inv_Location.N_LocationID, Acc_BranchMaster.N_BranchID, "+
                                                " 0 AS N_DeliveryNoteId, 1 AS B_Invoice, Tvl_TicketRefund.X_Notes, 0 AS N_Discountreturn, 0 AS N_TaxAmt, Inv_Sales.N_PaymentMethodId, Tvl_TicketRefund.N_SalesmanID, "+
                                                " 0 AS B_IsSaveDraft,Acc_Company.N_CurrencyID,1 AS N_ExchangeRate, 0 AS N_TotalPaidAmountF,Tvl_TicketRefund.N_CustRefund AS N_TotalReturnAmountF, "+
                                                " Tvl_TicketRefund.N_CreatedUser,Tvl_TicketRefund.D_CreatedDate , 1638 AS N_FormID,0 as N_ProjectID"+
                                                " FROM         Inv_Sales INNER JOIN "+
                                                " Tvl_Ticketing ON Inv_Sales.N_CompanyId = Tvl_Ticketing.N_CompanyID AND Inv_Sales.N_SalesRefID = Tvl_Ticketing.N_TicketID AND Inv_Sales.N_SalesType=7 INNER JOIN "+
                                                " Tvl_TicketRefund ON Tvl_Ticketing.N_CompanyID = Tvl_TicketRefund.N_CompanyID AND Tvl_Ticketing.N_TicketID = Tvl_TicketRefund.N_TicketID INNER JOIN "+
                                                " Inv_Location ON Tvl_TicketRefund.N_CompanyID = Inv_Location.N_CompanyID AND Inv_Location.B_IsDefault = 1 INNER JOIN "+
                                                " Acc_BranchMaster ON Tvl_TicketRefund.N_CompanyID = Acc_BranchMaster.N_CompanyID AND Acc_BranchMaster.B_DefaultBranch = 1 INNER JOIN "+
                                                " Acc_Company ON Tvl_TicketRefund.N_CompanyID = Acc_Company.N_CompanyID LEFT OUTER JOIN "+
                                                " Inv_SalesReturnMaster ON Inv_Sales.N_CompanyId = Inv_SalesReturnMaster.N_CompanyID AND Inv_Sales.N_SalesId = Inv_SalesReturnMaster.N_SalesId "+
                                                " WHERE           Tvl_TicketRefund.N_CompanyID= "+nCompanyID+" and Tvl_TicketRefund.N_TicketRefundID= "+nTicketRefundID;

                        SMasterDt = dLayer.ExecuteDataTable(sqlSalesMaster, Params, connection,transaction);
                        SMasterDt = api.Format(SMasterDt, "master");
                        Sds.Tables.Add(SMasterDt); 

                        //   DateTime dStartDate = Convert.ToDateTime(SMasterDt.Rows[0]["D_ReturnDate"]);

                        // //  string NewDate =dStartDate.ToString("yyyy-MM-dd HH:mm:ss:fff");
                        // DateTime Start = new DateTime(Convert.ToDateTime(dStartDate.ToString()).Year, Convert.ToDateTime(dStartDate.ToString()).Month, 1);

                        // SMasterDt.Rows[0]["D_ReturnDate"]= Start;
                        // SMasterDt.AcceptChanges();



                        string sqlSalesDetails =" SELECT     Tvl_TicketRefund.N_CompanyID, ISNULL(Inv_SalesReturnDetails.N_DebitNoteDetailsID, 0) AS N_DebitNoteDetailsID, ISNULL(Inv_SalesReturnDetails.N_DebitNoteId, 0) AS N_DebitNoteId, "+
                                                " 1 AS N_RetQty, Tvl_TicketRefund.N_CustRefund As N_RetAmount, Inv_SalesDetails.N_ItemID, GETDATE() AS D_Entrydate,Inv_Location.N_LocationID,Acc_BranchMaster.N_BranchID, "+
                                                " Inv_SalesDetails.N_Cost AS N_ReturnCost,Inv_SalesDetails.N_ItemUnitID AS N_UnitID,Inv_SalesDetails.N_Sprice AS N_SPrice,1 AS N_BaseUnitQty,Tvl_TicketRefund.N_CustRefund AS N_RetAmountF, "+
                                                " Inv_SalesDetails.N_Cost AS N_ReturnCostF,Inv_SalesDetails.N_Sprice AS N_SPriceF "+
                                                " FROM         Inv_SalesDetails INNER JOIN "+
                                                " Tvl_TicketRefund INNER JOIN "+
                                                " Tvl_Ticketing ON Tvl_TicketRefund.N_CompanyID = Tvl_Ticketing.N_CompanyID AND Tvl_TicketRefund.N_TicketID = Tvl_Ticketing.N_TicketID INNER JOIN "+
                                                " Inv_Sales ON Tvl_Ticketing.N_CompanyID = Inv_Sales.N_CompanyId AND Tvl_Ticketing.N_TicketID = Inv_Sales.N_SalesRefID AND Inv_Sales.N_SalesType = 7 ON "+
                                                " Inv_SalesDetails.N_CompanyID = Inv_Sales.N_CompanyId AND Inv_SalesDetails.N_SalesID = Inv_Sales.N_SalesId INNER JOIN "+
                                                " Acc_BranchMaster ON Tvl_TicketRefund.N_CompanyID = Acc_BranchMaster.N_CompanyID AND Acc_BranchMaster.B_DefaultBranch = 1 INNER JOIN "+
                                                " Inv_Location ON Tvl_TicketRefund.N_CompanyID = Inv_Location.N_CompanyID AND Inv_Location.B_IsDefault = 1 LEFT OUTER JOIN "+
                                                " Inv_SalesReturnDetails ON Inv_SalesDetails.N_SalesDetailsID = Inv_SalesReturnDetails.N_SalesDetailsId AND Inv_SalesDetails.N_CompanyID = Inv_SalesReturnDetails.N_CompanyID "+
                                                " WHERE      Tvl_TicketRefund.N_CompanyID= "+nCompanyID+" AND  Tvl_TicketRefund.N_TicketRefundID= "+nTicketRefundID;     

                        SDetailsDt = dLayer.ExecuteDataTable(sqlSalesDetails, Params, connection,transaction);
                        SDetailsDt = api.Format(SDetailsDt, "details");
                        Sds.Tables.Add(SDetailsDt);  

                        SAttachmentDt = api.Format(SAttachmentDt, "attachments");
                        Sds.Tables.Add(SAttachmentDt);                                              

                        SResult=txnHandler.SalesReturnSaveData( Sds ,User , dLayer, connection, transaction);
                        
                        n_SIsCompleted=myFunctions.getIntVAL(SResult["b_IsCompleted"].ToString());
                        x_SMessage=SResult["x_Msg"].ToString();

                        if(n_SIsCompleted==0)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User, x_SMessage));
                        }
                        //-------------------------------^^^^^^^^^^^^^^------------------------------------//

                        //-------------------------------Purchase Return Save------------------------------------//
                        DataSet Pds= new DataSet();
                        DataTable PMasterDt = new DataTable();
                        DataTable PDetailsDt = new DataTable();
                        
                        int n_PIsCompleted=0;
                        string x_PMessage="";

                        string sqlPurchaseMaster ="SELECT     Tvl_TicketRefund.N_CompanyID, Tvl_TicketRefund.N_FnyearID,ISNULL(Inv_PurchaseReturnMaster.N_CreditNoteId,0) AS N_CreditNoteId,ISNULL(Inv_PurchaseReturnMaster.X_CreditNoteNo,'@Auto') AS X_CreditNoteNo, "+
			                                        " Inv_Purchase.N_PurchaseId AS N_PurchaseId,Tvl_TicketRefund.D_RefundDate AS D_RetDate,Tvl_TicketRefund.N_userID AS N_UserId,0 AS N_TotalReceived,Tvl_TicketRefund.N_SuppRefund AS N_TotalReturnAmount, "+
			                                        " Tvl_TicketRefund.N_VendorID,GETDATE() AS D_Entrydate,Acc_BranchMaster.N_BranchID,Inv_Location.N_LocationID,0 AS N_TotalReceivedF,Tvl_TicketRefund.N_SuppRefund AS N_TotalReturnAmountF,Acc_Company.N_CurrencyID, "+
			                                        " 1 AS N_ExchangeRate,Inv_Purchase.N_PaymentMethod,Tvl_TicketRefund.N_CreatedUser,Tvl_TicketRefund.D_CreatedDate, 1638 AS N_FormID "+
                                                    " FROM         Tvl_TicketRefund INNER JOIN "+
                                                    " Tvl_Ticketing ON Tvl_TicketRefund.N_CompanyID = Tvl_Ticketing.N_CompanyID AND Tvl_TicketRefund.N_TicketID = Tvl_Ticketing.N_TicketID INNER JOIN "+
                                                    " Inv_Purchase ON Tvl_Ticketing.N_CompanyID = Inv_Purchase.N_CompanyID AND Tvl_Ticketing.N_TicketID = Inv_Purchase.N_PurchaseRefID AND Inv_Purchase.N_PurchaseType=7 INNER JOIN "+
                                                    " Acc_Company ON Tvl_TicketRefund.N_CompanyID = Acc_Company.N_CompanyID INNER JOIN "+
                                                    " Inv_Location ON Tvl_TicketRefund.N_CompanyID = Inv_Location.N_CompanyID AND Inv_Location.B_IsDefault = 1 INNER JOIN "+
                                                    " Acc_BranchMaster ON Tvl_TicketRefund.N_CompanyID = Acc_BranchMaster.N_CompanyID AND Acc_BranchMaster.B_DefaultBranch = 1 LEFT OUTER JOIN "+
                                                    " Inv_PurchaseReturnMaster ON Inv_Purchase.N_CompanyID = Inv_PurchaseReturnMaster.N_CompanyID AND Inv_Purchase.N_PurchaseID = Inv_PurchaseReturnMaster.N_PurchaseId  "+            
                                                    " WHERE       Tvl_TicketRefund.N_CompanyID= "+nCompanyID+" AND Tvl_TicketRefund.N_TicketRefundID= "+ nTicketRefundID;

                        PMasterDt = dLayer.ExecuteDataTable(sqlPurchaseMaster, Params, connection,transaction);
                        PMasterDt = api.Format(PMasterDt, "master");
                        Pds.Tables.Add(PMasterDt);

                        string sqlPurchaseDetails =" SELECT     Tvl_TicketRefund.N_CompanyID,ISNULL(Inv_PurchaseReturnDetails.N_CreditNoteDetailsID,0) AS N_CreditNoteDetailsID,ISNULL(Inv_PurchaseReturnDetails.N_CreditNoteId,0) AS N_CreditNoteId, "+
			                                        " Inv_PurchaseDetails.N_PurchaseDetailsId AS N_PurchaseDetailsId,1 AS N_RetQty,Tvl_TicketRefund.N_SuppRefund AS N_RetAmount,Inv_PurchaseDetails.N_ItemID,GETDATE() AS D_Entrydate, "+
			                                        " Acc_BranchMaster.N_BranchId AS N_BranchId,Inv_Location.N_LocationID,Tvl_TicketRefund.N_SuppRefund AS N_RetAmountF,Tvl_TicketRefund.N_SuppRefund AS N_RPrice,1 AS N_QtyDisplay, "+
			                                        " Inv_PurchaseDetails.N_Cost,Inv_PurchaseDetails.N_ItemUnitID "+
                                                    " FROM         Tvl_TicketRefund INNER JOIN "+
                                                    " Tvl_Ticketing ON Tvl_TicketRefund.N_CompanyID = Tvl_Ticketing.N_CompanyID AND Tvl_TicketRefund.N_TicketID = Tvl_Ticketing.N_TicketID INNER JOIN "+
                                                    " Inv_Purchase ON Tvl_Ticketing.N_CompanyID = Inv_Purchase.N_CompanyID AND Tvl_Ticketing.N_TicketID = Inv_Purchase.N_PurchaseRefID INNER JOIN "+
                                                    " Acc_Company ON Tvl_TicketRefund.N_CompanyID = Acc_Company.N_CompanyID INNER JOIN "+
                                                    " Acc_BranchMaster ON Tvl_TicketRefund.N_CompanyID = Acc_BranchMaster.N_CompanyID INNER JOIN "+
                                                    " Inv_Location ON Tvl_TicketRefund.N_CompanyID = Inv_Location.N_CompanyID INNER JOIN "+
                                                    " Inv_PurchaseDetails ON Inv_Purchase.N_CompanyID = Inv_PurchaseDetails.N_CompanyID AND Inv_Purchase.N_PurchaseID = Inv_PurchaseDetails.N_PurchaseID LEFT OUTER JOIN "+
                                                    " Inv_PurchaseReturnDetails ON Inv_PurchaseDetails.N_CompanyID = Inv_PurchaseReturnDetails.N_CompanyID AND "+
                                                    " Inv_PurchaseDetails.N_PurchaseDetailsID = Inv_PurchaseReturnDetails.N_PurchaseDetailsId "+
                                                    " WHERE    Tvl_TicketRefund.N_CompanyID= "+nCompanyID+" AND  Tvl_TicketRefund.N_TicketRefundID= "+nTicketRefundID;

                        PDetailsDt = dLayer.ExecuteDataTable(sqlPurchaseDetails, Params, connection,transaction);
                        PDetailsDt = api.Format(PDetailsDt, "details");
                        Pds.Tables.Add(PDetailsDt);                                                

                        PResult=txnHandler.PurchaseReturnSaveData( Pds ,User , dLayer, connection, transaction);

                        n_PIsCompleted=myFunctions.getIntVAL(PResult["b_IsCompleted"].ToString());
                        x_PMessage=PResult["x_Msg"].ToString();

                        if(n_PIsCompleted==0)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User, x_PMessage));
                        }
                        //-------------------------------^^^^^^^^^^^^^^------------------------------------//


                        transaction.Commit();
                        return Ok(api.Success("Ticket Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

         [HttpGet("list")]
        public ActionResult GetRefundList(int? nCompanyId, int nFnYearId ,int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_RefundNo like '%" + xSearchkey + "%' or X_TicketNo like '%" + xSearchkey + "%' or X_InvoiceNo like '%" + xSearchkey + "%' or X_Passenger like '%" + xSearchkey + "%' or X_Route like '%" + xSearchkey +"%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_TicketRefundID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_RefundNo":
                        xSortBy = "X_RefundNo " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }


            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_TvlTicketRefund where N_CompanyID=@nCompanyId and N_FnyearID=@nFnYearId  " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_TvlTicketRefund where N_CompanyID=@nCompanyId and N_FnyearID=@nFnYearId " + Searchkey + " and N_TicketRefundID not in (select top(" + Count + ") N_TicketRefundID from vw_TvlTicketRefund where N_CompanyID=@nCompanyId and N_FnyearID=@nFnYearId " + xSortBy + " ) " + " " + xSortBy;

            Params.Add("@nCompanyId", nCompanyID);
            Params.Add("@nFnYearId", nFnYearId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(*) as N_Count  from vw_TvlTicketRefund  where N_CompanyID=@nCompanyId and N_FnyearID=@nFnYearId " + Searchkey + "";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

   
      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nTicketRefundID)
        {

            int Results = 0;
            int nCompanyID=myFunctions.GetCompanyID(User);
            try
            {                        
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    int nCreditNoteId=0,nDebitNoteId=0;

                    //-----------------------------------Purchase Return Delete---------------------------------------//
                    object CreditNoteID = dLayer.ExecuteScalar("SELECT     Inv_PurchaseReturnMaster.N_CreditNoteId FROM         Tvl_TicketRefund INNER JOIN "+
                                                                " Tvl_Ticketing ON Tvl_TicketRefund.N_CompanyID = Tvl_Ticketing.N_CompanyID AND Tvl_TicketRefund.N_TicketID = Tvl_Ticketing.N_TicketID INNER JOIN "+
                                                                " Inv_PurchaseReturnMaster INNER JOIN "+
                                                                " Inv_Purchase ON Inv_PurchaseReturnMaster.N_PurchaseId = Inv_Purchase.N_PurchaseID AND Inv_PurchaseReturnMaster.N_CompanyID = Inv_Purchase.N_CompanyID ON "+
                                                                " Tvl_Ticketing.N_CompanyID = Inv_Purchase.N_CompanyID AND Tvl_Ticketing.N_TicketID = Inv_Purchase.N_PurchaseRefID AND Inv_Purchase.N_PurchaseType=7 "+
                                                                " WHERE Tvl_TicketRefund.N_CompanyID= "+nCompanyID+ " and Tvl_TicketRefund.N_TicketRefundID= "+ nTicketRefundID, Params, connection,transaction);
                    if(CreditNoteID!=null)
                        nCreditNoteId=myFunctions.getIntVAL(CreditNoteID.ToString());

                    if(nCreditNoteId>0)
                    {
                        object objPaymentProcessed = dLayer.ExecuteScalar("Select Isnull(N_PayReceiptId,0) from Inv_PayReceiptDetails where N_InventoryId=" + nCreditNoteId + " and X_TransType='PURCHASE RETURN'", connection, transaction);
                        if (objPaymentProcessed == null)
                            objPaymentProcessed = 0;

                        SortedList deleteParamsPR = new SortedList()
                                {
                                    {"N_CompanyID",nCompanyID},
                                    {"X_TransType","PURCHASE RETURN"},
                                    {"N_VoucherID",nCreditNoteId}
                                };
                        if (myFunctions.getIntVAL(objPaymentProcessed.ToString()) == 0)
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", deleteParamsPR, connection, transaction);
                        else
                            return Ok(api.Error(User, "Payment processed! Unable to delete"));

                    }
                    //-----------------------------------------^^^^^^^^^^^^^^------------------------------------------//
                    //--------------------------------------Sales Return Master---------------------------------------//
                    object DebitNoteId = dLayer.ExecuteScalar("SELECT     Inv_SalesReturnMaster.N_DebitNoteID FROM         Tvl_TicketRefund INNER JOIN "+
                                                                "Tvl_Ticketing ON Tvl_TicketRefund.N_CompanyID = Tvl_Ticketing.N_CompanyID AND Tvl_TicketRefund.N_TicketID = Tvl_Ticketing.N_TicketID INNER JOIN "+
                                                                " Inv_SalesReturnMaster INNER JOIN "+
                                                                " Inv_Sales ON Inv_SalesReturnMaster.N_SalesId = Inv_Sales.N_SalesId AND Inv_SalesReturnMaster.N_CompanyID = Inv_Sales.N_CompanyId ON "+
                                                                " Tvl_Ticketing.N_CompanyID = Inv_Sales.N_CompanyId AND Tvl_Ticketing.N_TicketID = Inv_Sales.N_SalesRefID AND Inv_Sales.N_SalesType=7 "+
                                                                " WHERE Tvl_TicketRefund.N_CompanyID= "+nCompanyID+ " and Tvl_TicketRefund.N_TicketRefundID= "+ nTicketRefundID, Params, connection,transaction);

                    if(DebitNoteId!=null)
                        nDebitNoteId=myFunctions.getIntVAL(DebitNoteId.ToString());

                    if(nDebitNoteId>0)
                    {
                        object objPaymentProcessed = dLayer.ExecuteScalar("Select Isnull(N_PayReceiptId,0) from Inv_PayReceiptDetails where N_InventoryId=" + nDebitNoteId + " and X_TransType='SALES RETURN'", connection,transaction);
                        if (objPaymentProcessed == null)
                            objPaymentProcessed = 0;

                        SortedList deleteParamsSR = new SortedList()
                                {
                                    {"N_CompanyID",nCompanyID},
                                    {"X_TransType","SALES RETURN"},
                                    {"N_VoucherID",nDebitNoteId}
                                };
                        if (myFunctions.getIntVAL(objPaymentProcessed.ToString()) == 0)
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", deleteParamsSR, connection, transaction);
                        else
                            return Ok(api.Error(User, "Payment processed! Unable to delete"));
                    }                        

                    //--------------------------------------^^^^^^^^^^^^^^^^^^^^--------------------------------------//                       

                    Results = dLayer.DeleteData("Tvl_TicketRefund ", "N_TicketRefundID", nTicketRefundID, "N_CompanyID =" + nCompanyID, connection, transaction);
                
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("Ticket refund deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User,"Unable to delete "));
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }



        }



               [HttpGet("tktList")]
        public ActionResult RefundDetails()
        {
            DataSet dt = new DataSet();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from Tvl_Ticketing where N_CompanyID=@p1 and ISNULL(B_NonRefundable,0)=0 and N_TicketID not in(select N_TicketID from Tvl_TicketRefund)";
            Params.Add("@p1", nCompanyId);  
            // Params.Add("@p2", X_RefundNo);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }

                    MasterTable = api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);
                }
                return Ok(api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
    }
}

