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
    [Route("salesreturn")]
    [ApiController]
    public class Inv_SalesReturn : ControllerBase
    {
        private readonly IDataAccessLayer _dataAccess;
        private readonly IApiFunctions _api;
        private readonly IDLayer dLayer;

        
        public Inv_SalesReturn(IDataAccessLayer dataaccess,IApiFunctions api,IDLayer dl)
        {
            _dataAccess=dataaccess;
            _api=api;
            dLayer=dl;
        }
       

        [HttpGet("list")]
        public ActionResult GetSalesReturn(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table= "vw_InvDebitNo_Search";
            string X_Fields = "*";
            string X_Crieteria = "N_CompanyID=@p1 and N_FnYearID=@p2";
            string X_OrderBy="";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);

            try{
                dt=_dataAccess.Select(X_Table,X_Fields,X_Crieteria,Params,X_OrderBy);
                foreach (DataColumn c in dt.Columns)
                    c.ColumnName = String.Join("", c.ColumnName.Split());
                if (dt.Rows.Count==0)
                    {
                        return StatusCode(200,_api.Response(200 ,"No Results Found" ));
                    }else{
                        return Ok(dt);
                    }   
            }catch(Exception e){
                return StatusCode(403,_api.ErrorResponse(e));
            }
        }
        [HttpGet("details")]
        public ActionResult GetSalesReturnDetails(int? nCompanyId,int nSalesReturnId,int nFnYearId)
        {
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            
            string X_Table="vw_InvSalesQuotationNo_Search";
            string X_Fields = "*";   
            string X_Crieteria = "N_CompanyID=@p1 and N_FnYearID=@p2 and N_QuotationID=@p3";
            string X_OrderBy="";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);
            Params.Add("@p3",nSalesReturnId);

            try{
                DataTable Quotation = new DataTable();
                
                Quotation=_dataAccess.Select(X_Table,X_Fields,X_Crieteria,Params,X_OrderBy);
                foreach(DataColumn c in Quotation.Columns)
                    c.ColumnName = String.Join("", c.ColumnName.Split());
                dt.Tables.Add(Quotation);
                Quotation.TableName="Master";
                
                //Quotation Details

            string  X_Table1="vw_InvQuotationDetails";
            string X_Fields1 = "*";
            string X_Crieteria1 = "N_CompanyID=@p1 and N_FnYearID=@p2 and N_QuotationID=@p3";
            string X_OrderBy1="";
            DataTable QuotationDetails = new DataTable();
            QuotationDetails=_dataAccess.Select(X_Table1,X_Fields1,X_Crieteria1,Params,X_OrderBy1);
            foreach(DataColumn c in QuotationDetails.Columns)
                    c.ColumnName = String.Join("", c.ColumnName.Split());
            dt.Tables.Add(QuotationDetails);
            QuotationDetails.TableName="Details";

            



return Ok(dt);

                // if(dt.Tables["Master"].Rows.Count==0)
                //     {
                //         return StatusCode(200,_api.Response(200 ,"No Results Found" ));
                //     }else{
                //         return Ok(dt.Tables[0]);
                //     }   
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
                    // Auto Gen
                    string InvoiceNo="";
                    DataRow masterRow=MasterTable.Rows[0];
                    var values = masterRow["X_DebitNoteNo"].ToString();
                    
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",masterRow["n_CompanyId"].ToString());
                        Params.Add("N_YearID",masterRow["n_FnYearId"].ToString());
                        Params.Add("N_FormID",80);
                        Params.Add("N_BranchID",masterRow["n_BranchId"].ToString());
                        InvoiceNo =  _dataAccess.GetAutoNumber("Inv_SalesReturnMaster","X_DebitNoteNo", Params);
                        if(InvoiceNo==""){return StatusCode(409,_api.Response(409 ,"Unable to generate sales return" ));}
                        MasterTable.Rows[0]["X_DebitNoteNo"] = InvoiceNo;
                    }

                    dLayer.setTransaction();
                    int N_InvoiceId=dLayer.SaveData("Inv_SalesReturnMaster","N_DebitNoteId",0,MasterTable);                    
                    if(N_InvoiceId<=0){
                        dLayer.rollBack();
                        }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                        {
                            DetailTable.Rows[j]["N_DebitNoteId"]=N_InvoiceId;
                        }
                    int N_InvoiceDetailId=dLayer.SaveData("Inv_SalesReturnDetails","N_DebitnoteDetailsID",0,DetailTable);                    
                    dLayer.commit();
                    return GetSalesReturnDetails(int.Parse(masterRow["n_CompanyId"].ToString()),N_InvoiceId,int.Parse(masterRow["n_FnYearId"].ToString()));
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
                _dataAccess.StartTransaction();
                Results=_dataAccess.DeleteData("Inv_SalesQuotation","n_quotationID",N_QuotationID,"");
                if(Results<=0){
                        _dataAccess.Rollback();
                        return StatusCode(409,_api.Response(409 ,"Unable to delete sales quotation" ));
                        }
                        else{
                _dataAccess.DeleteData("Inv_SalesQuotationDetails","n_quotationID",N_QuotationID,"");
                }
                
                if(Results>0){
                    _dataAccess.Commit();
                    return StatusCode(200,_api.Response(200 ,"Sales quotation deleted" ));
                }else{
                    _dataAccess.Rollback();
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