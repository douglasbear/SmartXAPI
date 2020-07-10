using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("salesquotation")]
    [ApiController]
    public class Inv_SalesQuotation : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;

        
        public Inv_SalesQuotation(IDataAccessLayer dl,IApiFunctions api)
        {
            dLayer=dl;
            _api=api;
        }
       

        [HttpGet("list")]
        public ActionResult GetSalesQuotationList(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select N_QuotationId as NQuotationId,[Quotation No] as QuotationNo,[Quotation Date] as QuotationDate,N_CompanyId as NCompanyId,N_CustomerId as NCustomerId,[Customer Code] as CustomerCode,N_FnYearID as NFnYearId,D_QuotationDate as DQuotationDate,N_BranchId as NBranchId,B_YearEndProcess as BYearEndProcess,X_CustomerName as XCustomerName,X_BranchName as XBranchName,X_RfqRefNo as XRfqRefNo,D_RfqRefDate as DRfqRefDate,N_Amount as NAmount,N_FreightAmt as NFreightAmt,N_DiscountAmt as NDiscountAmt,N_Processed as NProcessed,N_OthTaxAmt as NOthTaxAmt,N_BillAmt as NBillAmt,N_ProjectID as NProjectId,X_ProjectName as XProjectName from vw_InvSalesQuotationNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2";

            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);

            try{
                dt=dLayer.ExecuteDataTable(sqlCommandText,Params);
                if(dt.Rows.Count==0)
                    {
                        return StatusCode(200,_api.Response(200 ,"No Results Found" ));
                    }else{
                        return Ok(dt);
                    }   
            }catch(Exception e){
                return StatusCode(403,_api.ErrorResponse(e));
            }
        }
        [HttpGet("listDetails")]
        public ActionResult GetSalesQuotationDetails(int? nCompanyId,int nQuotationId,int nFnYearId)
        {
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from vw_InvSalesQuotationNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_QuotationID=@p3";

            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);
            Params.Add("@p3",nQuotationId);

            try{
                DataTable Quotation = new DataTable();
                
                Quotation=dLayer.ExecuteDataTable(sqlCommandText,Params);
                Quotation=_api.Format(Quotation,"Master");
                dt.Tables.Add(Quotation);
                
                //Quotation Details

            string  sqlCommandText2="select * from vw_InvQuotationDetails where N_CompanyID=@p1 and N_FnYearID=@p2 and N_QuotationID=@p3";

            DataTable QuotationDetails = new DataTable();
            QuotationDetails=dLayer.ExecuteDataTable(sqlCommandText2,Params);
            QuotationDetails=_api.Format(QuotationDetails,"Details");
            dt.Tables.Add(QuotationDetails);
            return Ok(dt);
            }catch(Exception e){
                return StatusCode(403,_api.ErrorResponse(e));
            }
        }

       //Save....
       [HttpPost("Save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            try{
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    dLayer.setTransaction();
                    // Auto Gen
                    string QuotationNo="";
                    var values = MasterTable.Rows[0]["x_QuotationNo"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",Master["n_CompanyId"].ToString());
                        Params.Add("N_YearID",Master["n_FnYearId"].ToString());
                        Params.Add("N_FormID",80);
                        Params.Add("N_BranchID",Master["n_BranchId"].ToString());
                        QuotationNo =  dLayer.GetAutoNumber("Inv_SalesQuotation","x_QuotationNo", Params);
                        if(QuotationNo==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Quotation Number" ));}
                        MasterTable.Rows[0]["x_QuotationNo"] = QuotationNo;
                    }

                    
                    int N_QuotationId=dLayer.SaveData("Inv_SalesQuotation","N_QuotationId",0,MasterTable);                    
                    if(N_QuotationId<=0){
                        dLayer.rollBack();
                        return StatusCode(409,_api.Response(409 ,"Unable to save Quotation" ));
                        }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                        {
                            DetailTable.Rows[j]["n_QuotationID"]=N_QuotationId;
                        }
                    int N_QuotationDetailId=dLayer.SaveData("Inv_SalesQuotationDetails","n_QuotationDetailsID",0,DetailTable);                    
                    if(N_QuotationDetailId<=0){
                        dLayer.rollBack();
                        return StatusCode(409,_api.Response(409 ,"Unable to save Quotation" ));
                    }else{
                        dLayer.commit();
                    }
                    return  GetSalesQuotationDetails(int.Parse(Master["n_CompanyId"].ToString()),N_QuotationId,int.Parse(Master["n_FnYearId"].ToString()));
                }
                catch (Exception ex)
                {
                    dLayer.rollBack();
                    return StatusCode(403,_api.ErrorResponse(ex));
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
                Results=dLayer.DeleteData("Inv_SalesQuotation","n_quotationID",N_QuotationID,"");
                if(Results<=0){
                        dLayer.rollBack();
                        return StatusCode(409,_api.Response(409 ,"Unable to delete sales quotation" ));
                        }
                        else{
                dLayer.DeleteData("Inv_SalesQuotationDetails","n_quotationID",N_QuotationID,"");
                }
                
                if(Results>0){
                    dLayer.commit();
                    return StatusCode(200,_api.Response(200 ,"Sales quotation deleted" ));
                }else{
                    dLayer.rollBack();
                    return StatusCode(409,_api.Response(409 ,"Unable to delete sales quotation" ));
                }
                
                }
            catch (Exception ex)
                {
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
            

        }
        
    }
}