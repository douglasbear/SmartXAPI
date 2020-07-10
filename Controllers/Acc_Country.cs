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
    [Route("country")]
    [ApiController]
    public class Acc_Country : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        
        public Acc_Country(IApiFunctions api,IDataAccessLayer dl)
        {
            _api = api;
            dLayer=dl;
        }

        [HttpGet("list")]
        public ActionResult GetCountryList(int? nCompanyId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select X_CountryCode,X_CountryName,N_CompanyID,N_CountryID,B_TaxImplement from Acc_Country where N_CompanyID=@p1 order by N_CountryID";
            Params.Add("@p1",nCompanyId);
                
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

        
    }
}