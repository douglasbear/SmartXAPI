using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("taxcategory")]
    [ApiController]
    
    
    
    public class AccTaxCategoryController : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        

        public AccTaxCategoryController(IDataAccessLayer dl,IApiFunctions api)
        {
            dLayer = dl;
            _api=api;
        }

       

        //GET api/Projects/list
        [HttpGet("list") ]
        public ActionResult GetAllTaxTypes (int? nCompanyID)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from vw_TaxCategory_Disp where N_CompanyID=@p1";
            Params.Add("@p1",nCompanyID);
                
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