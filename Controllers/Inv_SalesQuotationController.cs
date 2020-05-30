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
    public class Inv_SalesQuotationController : ControllerBase
    {
        private readonly IDataAccessLayer _dataAccess;
        private readonly IApiFunctions _api;

        
        public Inv_SalesQuotationController(IDataAccessLayer dataaccess,IApiFunctions api)
        {
            _dataAccess=dataaccess;
            _api=api;
        }
       

        [HttpGet("list")]
        public ActionResult GetSalesQuotationList(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table="vw_InvSalesQuotationNo_Search";
            string X_Fields = "*";
            string X_Crieteria = "N_CompanyID=@p1 and N_FnYearID=@p2";
            string X_OrderBy="";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);

            try{
                dt=_dataAccess.Select(X_Table,X_Fields,X_Crieteria,Params,X_OrderBy);
                if(dt.Rows.Count==0)
                    {
                        return StatusCode(200,_api.Response(200 ,"No Results Found" ));
                    }else{
                        return Ok(dt);
                    }   
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

                    // Auto Gen
                    var values = MasterTable.Rows[0]["x_QuotationNo"].ToString();


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
                    return Ok("DataSaved");
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
            {Results=_dataAccess.DeleteData("Inv_SalesQuotation","n_quotationID",N_QuotationID,"");
            return Ok();}
            catch (Exception ex)
            {return Ok();}
            

        }
        
    }
}