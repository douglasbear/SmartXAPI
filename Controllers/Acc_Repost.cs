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
    [Route("repost")]
    [ApiController]
    public class Acc_Repost : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Acc_Repost(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("fnYear")]
        public ActionResult FnYearList(int nCompanyId)
        {
            DataTable dt = new DataTable(); 
            SortedList Params = new SortedList();
            string sqlCommandText ="";

            sqlCommandText = "select * from Acc_FnYear where N_CompanyID=53 and ISNULL(B_YearEndProcess,0)=0 order by N_FnYearID ASC";
          
            Params.Add("@p1", nCompanyId);

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
                return Ok( _api.Error(User,e));
            }
        }

        [HttpGet("transactionType")]
        public ActionResult TransactionTypeList(int nCompanyId,int nFnYearID,int N_PartyType)
        {
            DataTable dt = new DataTable(); 
            SortedList Params = new SortedList();
            string sqlCommandText ="";

            //N_PartyType=1 -> Vendor
            //N_PartyType=2 -> Customer
            if(N_PartyType!=0)
            {
                sqlCommandText = "select X_TransType from Acc_VoucherDetails where N_CompanyID=@p1 and N_FnYearID=@p2 and X_TransType in (select X_TransType from Gen_PartyTransaction where N_PartyType=@p3) GROUP BY X_TransType ORDER BY X_TransType ASC";
                Params.Add("@p3", N_PartyType);
            }
            else
                sqlCommandText = "select X_TransType from Acc_VoucherDetails where N_CompanyID=@p1 and N_FnYearID=@p2 GROUP BY X_TransType ORDER BY X_TransType ASC";
          
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearID);

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
                return Ok( _api.Error(User,e));
            }
        }

        [HttpGet("trans")]
        public ActionResult TransList(int nCompanyId,int nFnYearID,string xTransType)
        {
            DataTable dt = new DataTable(); 
            SortedList Params = new SortedList();
            string sqlCommandText ="";

            if(xTransType=="GRN")
                sqlCommandText = "select X_MRNNo,N_MRNID from Inv_MRN where N_CompanyID=@p1 and N_FnYearID=@p2 ORDER BY X_MRNNo ASC";
            else if(xTransType=="PURCHASE")
                sqlCommandText = "select X_InvoiceNo,N_PurchaseID from Inv_Purchase where  N_CompanyID=@p1 and N_FnYearID=@p2 and X_TransType ='PURCHASE' ORDER BY X_InvoiceNo ASC";
            else if(xTransType=="DELIVERY")   
                sqlCommandText = "select X_ReceiptNo,N_DeliveryNoteID from Inv_DeliveryNote where N_CompanyID=@p1 and N_FnYearID=@p2 ORDER BY X_ReceiptNo ASC";
            else if(xTransType=="SALES")
                sqlCommandText = "select X_ReceiptNo,N_SalesID from Inv_Sales where N_CompanyID=@p1 and N_FnYearID=@p2 ORDER BY X_ReceiptNo ASC";
            else if(xTransType=="IA")
                sqlCommandText = "select X_RefNo,N_AdjustmentId from Inv_StockAdjustment where N_CompanyID=@p1 and N_FnYearID=@p2 ORDER BY X_RefNo ASC";
            else if(xTransType=="PURCHASE RETURN")    
                sqlCommandText = "select X_CreditNoteNo,N_CreditNoteId from Inv_PurchaseReturnMaster where N_CompanyID=@p1 and N_FnYearID=@p2 ORDER BY X_CreditNoteNo ASC";
            else if(xTransType=="STOCK TRANSFER") 
                sqlCommandText = "select X_ReferenceNo,N_TransferId from Inv_TransferStock where N_CompanyID=@p1 and N_FnYearID=@p2 ORDER BY X_ReferenceNo ASC";
            else if(xTransType=="STOCK RECEIVE") 
                sqlCommandText = "select X_ReferenceNo,N_ReceivableId from Inv_ReceivableStock where N_CompanyID=@p1 and N_FnYearID=@p2 ORDER BY X_ReferenceNo ASC";
            else if(xTransType=="SALES RETURN")
                sqlCommandText = "select X_DebitNoteNo,N_DebitNoteId from Inv_SalesReturnMaster where N_CompanyID=@p1 and N_FnYearID=@p2 ORDER BY X_DebitNoteNo ASC";
            else if(xTransType=="FTPURCHASE")
                sqlCommandText = "select X_InvoiceNo,N_PurchaseID from Inv_Purchase where N_CompanyID=@p1 and N_FnYearID=@p2 and X_TransType ='FTPURCHASE' ORDER BY X_InvoiceNo ASC";
          
          
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearID);

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
                return Ok( _api.Error(User,e));
            }
        }       

        [HttpPost("reposting")]
        public ActionResult Reposting([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                int N_RepostType = myFunctions.getIntVAL(MasterTable.Rows[0]["N_RepostType"].ToString());         

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    Params.Add("N_CompanyID", nCompanyID);                    

                    if(N_RepostType==1)
                    {                
                        Params.Add("N_FnYearID", myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString()));

                        dLayer.ExecuteNonQueryPro("[UTL_RepostByYear]", Params, connection, transaction);
                    }
                    else if(N_RepostType==2)
                    {
                        Params.Add("N_FnYearID", myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString()));
                        Params.Add("X_TransType", MasterTable.Rows[0]["X_TransType"].ToString());

                        dLayer.ExecuteNonQueryPro("[UTL_Repost]", Params, connection, transaction);
                    }
                    else if(N_RepostType==3)
                    {
                        Params.Add("N_TransID", myFunctions.getIntVAL(MasterTable.Rows[0]["N_TransID"].ToString()));
                        Params.Add("X_TransType", MasterTable.Rows[0]["X_TransType"].ToString());

                        dLayer.ExecuteNonQueryPro("[UTL_RepostByTransaction]", Params, connection, transaction);
                    }
                    else if(N_RepostType==4||N_RepostType==6)
                    {
                        Params.Add("N_FnyearID", myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString()));
                        Params.Add("N_PartyType", myFunctions.getIntVAL(MasterTable.Rows[0]["N_PartyType"].ToString()));
                        Params.Add("N_PartyID", myFunctions.getIntVAL(MasterTable.Rows[0]["N_PartyID"].ToString()));

                        dLayer.ExecuteNonQueryPro("[UTL_RepostByPartyWise]", Params, connection, transaction);
                    }
                    else if(N_RepostType==5||N_RepostType==7)
                    {
                        Params.Add("N_FnyearID", myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString()));
                        Params.Add("X_TransType", MasterTable.Rows[0]["X_TransType"].ToString());
                        Params.Add("N_PartyType", myFunctions.getIntVAL(MasterTable.Rows[0]["N_PartyType"].ToString()));
                        Params.Add("N_PartyID", myFunctions.getIntVAL(MasterTable.Rows[0]["N_PartyID"].ToString()));

                        dLayer.ExecuteNonQueryPro("[UTL_RepostByPartyTypeWise]", Params, connection, transaction);
                    }

                    transaction.Commit();
                    return Ok(_api.Success("Reposted"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(User,ex));
            }
        }          

    }
}