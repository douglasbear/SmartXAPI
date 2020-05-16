using System.Collections.Generic;
using AutoMapper;
using SmartxAPI.Data;
using SmartxAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace SmartxAPI.Controllers
{
    [Route("company")]
    [ApiController]
    public class Acc_CompanyController : ControllerBase
    {
        private readonly IAcc_CompanyRepo _repository;

        public Acc_CompanyController(IAcc_CompanyRepo repository, IMapper mapper)
        {
            _repository = repository;
        }
       
        //GET api/Company/list
        [HttpGet("list")]
        public ActionResult <IEnumerable<AccCompany>> GetAllCompanys()
        {
            var Companys = _repository.GetAllCompanys();

            return Ok(Companys);
        }

       
    }
}