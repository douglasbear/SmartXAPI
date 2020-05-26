using System.Collections.Generic;
using AutoMapper;
using SmartxAPI.Data;
using SmartxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Linq;



namespace SmartxAPI.Controllers
{
   // [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("products")]
    [ApiController]
    public class InvProductsListController : ControllerBase
    {
        private readonly IInvProductsListRepo _repository;

        public InvProductsListController(IInvProductsListRepo repository, IMapper mapper)
        {
            _repository = repository;
        }
       
        //GET api/Projects/list
        [HttpGet("list") ]
        public ActionResult <IEnumerable<VwInvItemSearch>> GetAllItems (int? nCompanyID)
        {
            try
             {

             var ProductsList = _repository.GetAllItems(nCompanyID);

            //return Ok(CustomerProjectsList);
             if(!ProductsList.Any())
                     {
                        return NotFound("No Results Found");
                     }else{
                         return Ok(ProductsList);
                     }
             }
             catch(Exception e){
                return BadRequest(e);
            }
            
        }

       
    }
}