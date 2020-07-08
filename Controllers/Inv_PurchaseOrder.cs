using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("purchaseorder")]
    [ApiController]
    public class Inv_PurchaseOrderController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;

        
        public Inv_PurchaseOrderController(IDataAccessLayer dl,IApiFunctions api)
        {
            dLayer=dl;
            _api=api;
        }
       

        [HttpGet("list")]
        public ActionResult GetPurchaseOrderList(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from vw_InvPurchaseOrderNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 order by D_POrderDate DESC,[Order No]";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);

            try{
                dt=dLayer.ExecuteDataTable(sqlCommandText,Params);
                dt=_api.Format(dt);
                if(dt.Rows.Count==0)
                    {
                        return StatusCode(200,_api.Response(200 ,"No Results Found" ));
                    }else{
                        return Ok(dt);
                    }   
            }catch(SqlException e){
                return StatusCode(404,_api.Response(404,e.StackTrace));
            }
        }
        [HttpGet("listDetails")]
        public ActionResult GetPurchaseOrderDetails(int? nCompanyId,int nPOrderId,int nFnYearId)
        {
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from vw_InvPurchaseOrderNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_QuotationID=@p3";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);
            Params.Add("@p3",nPOrderId);

            try{
                DataTable Quotation = new DataTable();
                
                Quotation=dLayer.ExecuteDataTable(sqlCommandText,Params);
                Quotation = _api.Format(Quotation,"Master");
                dt.Tables.Add(Quotation);
                
                //Quotation Details

            string  sqlCommandText2="select * from vw_InvQuotationDetails where N_CompanyID=@p1 and N_FnYearID=@p2 and N_QuotationID=@p3";
            DataTable QuotationDetails = new DataTable();
            QuotationDetails=dLayer.ExecuteDataTable(sqlCommandText2);
            QuotationDetails=_api.Format(QuotationDetails,"Details");
            dt.Tables.Add(QuotationDetails);

            



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

                    // Auto Gen
                    var values = MasterTable.Rows[0]["x_QuotationNo"].ToString();


                    dLayer.setTransaction();
                    int N_QuotationId=dLayer.SaveData("Inv_PurchaseOrder","N_QuotationId",0,MasterTable);                    
                    if(N_QuotationId<=0){
                        dLayer.rollBack();
                        }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                        {
                            DetailTable.Rows[j]["n_QuotationID"]=N_QuotationId;
                        }
                    int N_QuotationDetailId=dLayer.SaveData("Inv_PurchaseOrderDetails","n_QuotationDetailsID",0,DetailTable);                    
                    dLayer.commit();
                    return Ok("DataSaved");
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
                Results=dLayer.DeleteData("Inv_PurchaseOrder","n_quotationID",N_QuotationID,"");
                if(Results<=0){
                        dLayer.rollBack();
                        return StatusCode(409,_api.Response(409 ,"Unable to delete sales quotation" ));
                        }
                        else{
                Results=dLayer.DeleteData("Inv_PurchaseOrderDetails","n_quotationID",N_QuotationID,"");
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