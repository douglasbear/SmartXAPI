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
    [Route("salesinvoice")]
    [ApiController]
    public class Inv_SalesInvoice : ControllerBase
    {
        private readonly IDataAccessLayer _dataAccess;
        private readonly IApiFunctions _api;

        
        public Inv_SalesInvoice(IDataAccessLayer dataaccess,IApiFunctions api)
        {
            _dataAccess=dataaccess;
            _api=api;
        }
       

        [HttpGet("list")]
        public ActionResult GetSalesQuotationList(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table= "vw_InvSalesInvoiceNo_Search";
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
                return StatusCode(404,_api.Response(404,e.Message));
            }
        }
        [HttpGet("listDetails")]
        public ActionResult GetSalesQuotationList(int? nCompanyId,int nQuotationId,int nFnYearId)
        {
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            
            string X_Table="vw_InvSalesQuotationNo_Search";
            string X_Fields = "*";   
            string X_Crieteria = "N_CompanyID=@p1 and N_FnYearID=@p2 and N_QuotationID=@p3";
            string X_OrderBy="";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);
            Params.Add("@p3",nQuotationId);

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
                return StatusCode(404,_api.Response(404,e.Message));
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
                    string QuotationNo="";
                    var values = MasterTable.Rows[0]["x_QuotationNo"].ToString();
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID",MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID",80);
                        Params.Add("N_BranchID",MasterTable.Rows[0]["n_BranchId"].ToString());
                        QuotationNo =  _dataAccess.GetAutoNumber("Inv_SalesQuotation","x_QuotationNo", Params);
                        if(QuotationNo==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Quotation Number" ));}
                        MasterTable.Rows[0]["x_QuotationNo"] = QuotationNo;
                    }

                    _dataAccess.StartTransaction();
                    int N_QuotationId=_dataAccess.SaveData("Inv_SalesQuotation","N_QuotationId",0,MasterTable);                    
                    if(N_QuotationId<=0){
                        _dataAccess.Rollback();
                        }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                        {
                            DetailTable.Rows[j]["n_QuotationID"]=N_QuotationId;
                        }
                    int N_QuotationDetailId=_dataAccess.SaveData("Inv_SalesQuotationDetails","n_QuotationDetailsID",0,DetailTable);                    
                    _dataAccess.Commit();
                    return StatusCode(200,_api.Response(200 ,"Sales Quotation Saved" ));
                }
                catch (Exception ex)
                {
                    _dataAccess.Rollback();
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
                    return StatusCode(404,_api.Response(404,ex.Message));
                }
            

        }
        
    }
}