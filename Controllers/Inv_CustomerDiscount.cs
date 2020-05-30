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
    [Route("customerdiscount")]
    [ApiController]
    public class Inv_CustomerDiscount : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer _dataAccess;
        
        public Inv_CustomerDiscount(IApiFunctions api,IDataAccessLayer dataaccess)
        {
            _api = api;
            _dataAccess=dataaccess;
        }

        [HttpGet("list")]
        public ActionResult GetCustDiscountList(int? nCompanyId)
        {
            if(nCompanyId==null){return StatusCode(404,_api.Response(404,"Company ID Required"));}                       
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table="Inv_DiscountSettings";
            string X_Fields = "X_DiscCode,X_DiscDescription,N_CompanyID,N_DiscID,N_FnYearID";
            string X_Crieteria = "N_CompanyID=@p1";
            string X_OrderBy="N_DiscID";
            Params.Add("@p1",nCompanyId);
                
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

        
    }
}