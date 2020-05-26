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
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("projects")]
    [ApiController]
    public class InvCustomerProjectsController : ControllerBase
    {
        private readonly IInvCustomerProjectsRepo _repository;

        public InvCustomerProjectsController(IInvCustomerProjectsRepo repository, IMapper mapper)
        {
            _repository = repository;
        }
       
        //GET api/Projects/list
        [HttpGet("list") ]
        public ActionResult <IEnumerable<VwInvCustomerProjects>> GetAllProjects (int? nCompanyID,int? nFnYearID)
        {
            try
             {

             var CustomerProjectsList = _repository.GetAllProjects(nCompanyID,nFnYearID);

            //return Ok(CustomerProjectsList);
             if(!CustomerProjectsList.Any())
                     {
                        return NotFound("No Results Found");
                     }else{
                         return Ok(CustomerProjectsList);
                     }
             }
             catch(Exception e){
                return BadRequest(e);
            }
            
        }

       
    }
}