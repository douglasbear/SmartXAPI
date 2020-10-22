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
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("purchasereturn")]
    [ApiController]
    public class Inv_PurchaseReturn : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        
        public Inv_PurchaseReturn(IApiFunctions api,IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api=api;
            dLayer=dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");

        }
       

        [HttpGet("list")]
        public ActionResult GetPurchaseReturnList(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText= "select * from vw_InvCreditNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2";

            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);

            try{
                 using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                    connection.Open();
                     dt=dLayer.ExecuteDataTable(sqlCommandText,Params, connection);
                    }
                     dt=_api.Format(dt);
                      if (dt.Rows.Count==0)
                    {
                        return Ok(_api.Notice("No Results Found" ));
                    }else{
                        return Ok(dt);
                    }   
                  }catch(Exception e){
                     return BadRequest(_api.Error(e));
                }
        }
       [HttpGet("listDetails")]
        public ActionResult GetPurchaseReturnDetails(int nCompanyId, string xCreditNoteNo,string xInvoiceNo, int nFnYearId, bool bAllBranchData, int nBranchID)
        {

            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            DataTable DataTable = new DataTable();

            string Mastersql = "";

            if (bAllBranchData == true)
            {
                if(xInvoiceNo=="0")
                    Mastersql = "SP_Inv_PurchaseReturn_Disp @p1, 0, @p3,@p2,'PURCHASE',0";
                else
                {
                    Mastersql = "SP_Inv_PurchaseReturn_Disp @p1, 1, @p4,@p2,'PURCHASE',0";
                    Params.Add("@p4", xInvoiceNo);
                }
            }
            else
            {
              if(xInvoiceNo=="0")
                    Mastersql = "SP_Inv_PurchaseReturn_Disp @p1, 0, @p3,@p2,'PURCHASE',@p5";
                else
                {
                    Mastersql = "SP_Inv_PurchaseReturn_Disp @p1, 1, @p4,@p2,'PURCHASE',@p5";
                    Params.Add("@p5", nBranchID);
                }
            }

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", xCreditNoteNo);

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);

                    MasterTable = _api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                    //PurchaseOrder Details
                    string DetailSql = "";
                        if (bAllBranchData == true)
                        {
                            DetailSql = "Select vw_InvPurchaseReturnEdit.*,dbo.SP_Stock(vw_InvPurchaseReturnEdit.N_ItemID) As N_Stock from vw_InvPurchaseReturnEdit Where N_CompanyID=@p1 and X_CreditNoteNo=@p3 and N_FnYearID =@p2 and N_RetQty<>0";
                        }
                        else
                        {
                             DetailSql = "Select vw_InvPurchaseReturnEdit.*,dbo.SP_Stock(vw_InvPurchaseReturnEdit.N_ItemID) As N_Stock from vw_InvPurchaseReturnEdit Where N_CompanyID=@p1 and X_CreditNoteNo=@p3 and N_FnYearID =@p2 and N_RetQty<>0 and N_BranchId=@p5";
                        }
                        
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);
                }
                return Ok(dt);
            }
            catch (Exception e)
            {
                return StatusCode(403, _api.Error(e));
            }
        }

       //Save....
       [HttpPost("Save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    // Auto Gen
                                try{
                 using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                    connection.Open();
                    SqlTransaction transaction=connection.BeginTransaction();
                    string QuotationNo="";
                    var values = MasterTable.Rows[0]["X_CreditNoteNo"].ToString();
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID",MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID",80);
                        Params.Add("N_BranchID",MasterTable.Rows[0]["n_BranchId"].ToString());
                        QuotationNo =  dLayer.GetAutoNumber("Inv_PurchaseReturnMaster","X_CreditNoteNo", Params,connection,transaction);
                        if(QuotationNo==""){return Ok(_api.Warning("Unable to generate Quotation Number"));}
                        MasterTable.Rows[0]["X_CreditNoteNo"] = QuotationNo;
                    }
                    int N_CreditNoteID=dLayer.SaveData("Inv_PurchaseReturnMaster","N_CreditNoteID",MasterTable,connection,transaction);                    
                    if(N_CreditNoteID<=0){
                        transaction.Rollback();
                        }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                        {
                            DetailTable.Rows[j]["N_CreditNoteID"]=N_CreditNoteID;
                        }
                    int N_QuotationDetailId=dLayer.SaveData("Inv_PurchaseReturnDetails","n_CreditNoteDetailsID",DetailTable,connection,transaction);                    
                    transaction.Commit();
                    return Ok(_api.Success("Purchase Return Saved" ));
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(_api.Error(ex));
                }
        }
       
     

        [HttpDelete()]
        public ActionResult DeleteData(int nCreditNoteId, int nCompanyId, string xType)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList deleteParams = new SortedList()
                            {
                                {"N_CompanyID",nCompanyId},
                                {"X_TransType",xType},
                                {"N_VoucherID",nCreditNoteId}
                            };
                    dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", deleteParams, connection, transaction);
                    transaction.Commit();
                }

                return Ok(_api.Success("Purchase Return Deleted"));

            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(ex));
            }


        }
         [HttpGet("dummy")]
        public ActionResult GetPurchaseReturnDummy(int? nPurchaseReturnId)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(connectionString))
                {
                    Con.Open();
                    string sqlCommandText = "select * from Inv_PurchaseReturnMaster where N_CreditNoteId=@p1";
                    SortedList mParamList = new SortedList() { { "@p1", nPurchaseReturnId } };
                    DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList, Con);
                    masterTable = _api.Format(masterTable, "master");

                    string sqlCommandText2 = "select * from Inv_PurchaseReturnDetails where N_CreditNoteId=@p1";
                    SortedList dParamList = new SortedList() { { "@p1", nPurchaseReturnId } };
                    DataTable detailTable = dLayer.ExecuteDataTable(sqlCommandText2, dParamList, Con);
                    detailTable = _api.Format(detailTable, "details");

                    // string sqlCommandText3 = "select * from Inv_SaleAmountDetails where N_SalesId=@p1";
                    // DataTable dtAmountDetails = dLayer.ExecuteDataTable(sqlCommandText3, dParamList, Con);
                    // dtAmountDetails = _api.Format(dtAmountDetails, "saleamountdetails");

                    if (detailTable.Rows.Count == 0) { return Ok(new { }); }
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add(masterTable);
                    dataSet.Tables.Add(detailTable);
                    //dataSet.Tables.Add(dtAmountDetails);

                    return Ok(dataSet);

                }
            }
            catch (Exception e)
            {
                return StatusCode(403, _api.Error(e));
            }
        }


        
    }
}