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
                        return StatusCode(200,_api.Response(200 ,"No Results Found" ));
                    }else{
                        return Ok(dt);
                    }   
                  }catch(Exception e){
                     return StatusCode(404,_api.Response(404,e.Message));
                }
        }
        [HttpGet("listdetails")]
        public ActionResult GetPurchaseReturnList(int? nCompanyId,int nQuotationId,int nFnYearId)
        {
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from vw_InvCreditNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_QuotationID=@p3";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);
            Params.Add("@p3",nQuotationId);

            try{
                 DataTable MasterTable = new DataTable();
                 DataTable DetailTable = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                connection.Open();
                MasterTable=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                
                MasterTable= _api.Format(MasterTable,"Master");
                dt.Tables.Add(MasterTable);
                
                //Quotation Details

            string  sqlCommandText2="select * from vw_InvQuotationDetails where N_CompanyID=@p1 and N_FnYearID=@p2 and N_QuotationID=@p3";
           // DataTable QuotationDetails = new DataTable();
            DetailTable=dLayer.ExecuteDataTable(sqlCommandText2,Params,connection);
            DetailTable=_api.Format(DetailTable,"Details");
            dt.Tables.Add(DetailTable);
            }
            return Ok(dt);
            }catch(Exception e){
                return StatusCode(404,_api.Response(404,e.Message));
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
                    string QuotationNo="";
                    var values = MasterTable.Rows[0]["X_CreditNoteNo"].ToString();
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID",MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID",80);
                        Params.Add("N_BranchID",MasterTable.Rows[0]["n_BranchId"].ToString());
                        QuotationNo =  dLayer.GetAutoNumber("Inv_PurchaseReturnMaster","X_CreditNoteNo", Params);
                        if(QuotationNo==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Quotation Number" ));}
                        MasterTable.Rows[0]["X_CreditNoteNo"] = QuotationNo;
                    }
                try{
                    dLayer.setTransaction();
                    int N_CreditNoteID=dLayer.SaveData("Inv_PurchaseReturnMaster","N_CreditNoteID",0,MasterTable);                    
                    if(N_CreditNoteID<=0){
                        dLayer.rollBack();
                        }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                        {
                            DetailTable.Rows[j]["N_CreditNoteID"]=N_CreditNoteID;
                        }
                    int N_QuotationDetailId=dLayer.SaveData("Inv_PurchaseReturnDetails","n_CreditNoteDetailsID",0,DetailTable);                    
                    dLayer.commit();
                    return StatusCode(200,_api.Response(200 ,"Sales Quotation Saved" ));
                }
                catch (Exception ex)
                {
                    dLayer.rollBack();
                    return StatusCode(403,ex);
                }
        }
        //Delete....
         [HttpDelete()]
        public ActionResult DeleteData(int N_QuotationID)
        {
             int Results=0;
            try
            {
                dLayer.setTransaction();
                Results=dLayer.DeleteData("Inv_PurchaseReturnMaster","N_CreditNoteID",N_QuotationID,"");
                if(Results<=0){
                        dLayer.rollBack();
                        return StatusCode(409,_api.Response(409 ,"Unable to delete sales quotation" ));
                        }
                        else{
                dLayer.DeleteData("Inv_PurchaseReturnDetails","N_CreditNoteDetailsID",N_QuotationID,"");
                }
                
                if(Results>0){
                    dLayer.commit();
                    return StatusCode(200,_api.Response(200 ,"Purchase Return deleted" ));
                }else{
                    dLayer.rollBack();
                    return StatusCode(409,_api.Response(409 ,"Unable to delete Purchase Return" ));
                }
                
                }
            catch (Exception ex)
                {
                    return StatusCode(404,_api.Response(404,ex.Message));
                }
            

        }
        [HttpGet("dummy")]
        public ActionResult GetPurchaseInvoiceDummy(int? Id)
        {
            try{
            string  sqlCommandText="select * from Inv_PurchaseReturnMaster where N_CreditNoteID=@p1";
            SortedList mParamList = new SortedList() { {"@p1",Id} };
            DataTable masterTable =dLayer.ExecuteDataTable(sqlCommandText,mParamList);
            masterTable=_api.Format(masterTable,"master");

            string  sqlCommandText2="select * from Inv_PurchaseReturnDetails where N_CreditNoteID=@p1";
            SortedList dParamList = new SortedList() { {"@p1",Id} };
            DataTable detailTable =dLayer.ExecuteDataTable(sqlCommandText2,dParamList);
            detailTable=_api.Format(detailTable,"details");

            if(detailTable.Rows.Count==0){return Ok(new {});}
            DataSet dataSet=new DataSet();
            dataSet.Tables.Add(masterTable);
            dataSet.Tables.Add(detailTable);
            
            return Ok(dataSet);
            
            }catch(Exception e){
                return StatusCode(403,_api.ErrorResponse(e));
            }
        }
        
    }
}