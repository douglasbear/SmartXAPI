using System.Collections.Generic;
using AutoMapper;
using SmartxAPI.Data;
using SmartxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Linq;
using SmartxAPI.Profiles;




namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("taxcategory")]
    [ApiController]
    
    
    
    public class AccTaxCategoryController : ControllerBase
    {
        private readonly IAccTaxCategoryRepo _repository;
        APIResponse Resp =new APIResponse();
        

        public AccTaxCategoryController(IAccTaxCategoryRepo repository, IMapper mapper)
        {
            _repository = repository;
        }
       
        //GET api/Projects/list
        [HttpGet("list") ]
        public ActionResult <IEnumerable<AccTaxCategory>> GetAllTaxTypes (int? nCompanyID)
        {
            try
             {

             var TaxType = _repository.GetAllTaxTypes(nCompanyID);

            //return Ok(CustomerProjectsList);
             if(!TaxType.Any())
                     {
                         return StatusCode (404,Resp.ResponseAPI("404","Not Valid"));
                         //return Ok(TaxType);
                     }else{
                         return Ok(TaxType);
                     }
             }
             catch(Exception)
             {
               return StatusCode (404,Resp.ResponseAPI("404","Not Valid"));
                
            }
            
        }

       
    }
}