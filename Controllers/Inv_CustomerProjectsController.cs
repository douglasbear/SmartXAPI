using System.Collections.Generic;
using AutoMapper;
using SmartxAPI.Data;
using SmartxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace SmartxAPI.Controllers
{
    //[Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
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
        [HttpGet("list")]
        public ActionResult <IEnumerable<VwInvCustomerProjects>> GetAllProjects ()
        {
            var Menu = _repository.GetAllProjects();

            return Ok(Menu);
        }

       
    }
}