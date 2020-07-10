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
    [Route("vendorpayment")]
    [ApiController]
    public class Inv_VendorPayment : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;

        
        public Inv_VendorPayment(IDataAccessLayer dl,IApiFunctions api)
        {
            dLayer=dl;
            _api=api;
        }
       

        [HttpGet("list")]
        public ActionResult GetVendorPayment(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText= "select * from vw_InvPayment_Search where N_CompanyID=@p1 and N_FnYearID=@p2 order by D_Date DESC,[Memo]";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);

            try{
                dt=dLayer.ExecuteDataTable(sqlCommandText,Params);
                dt=_api.Format(dt);
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
        public ActionResult GetVendorPaymentDetails(int? nCompanyId,int nQuotationId,int nFnYearId)
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

                
    }
}