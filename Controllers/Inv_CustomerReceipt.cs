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
    [Route("salesreceipt")]
    [ApiController]
    public class Inv_CustomerReceipt : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;

        
        public Inv_CustomerReceipt(IDataAccessLayer dl,IApiFunctions api)
        {
            dLayer=dl;
            _api=api;
        }
       

        [HttpGet("list")]
        public ActionResult GetSalesReceipt(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText= "select * from vw_InvReceipt_Search where N_CompanyID=@p1 and N_FnYearID=@p2";
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
       
    }
}