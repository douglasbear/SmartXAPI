using System.Collections.Generic;
using AutoMapper;
using SmartxAPI.Data;
using SmartxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using SmartxAPI.Dtos;

namespace SmartxAPI.Controllers
{
    [Route("company")]
    [ApiController]
    public class Acc_CompanyController : ControllerBase
    {
        private readonly IAcc_CompanyRepo _repository;

        private readonly IMapper _mapper;

        public Acc_CompanyController(IAcc_CompanyRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper=mapper;
        }
       
        //GET api/Company/list
        [HttpGet("list")]
        public ActionResult <IEnumerable<Acc_CompanyReadDto>> GetAllCompanys()
        {
            var Companys = _repository.GetAllCompanys();
            return Ok(_mapper.Map<IEnumerable<Acc_CompanyReadDto>>(Companys));
          //  return Ok(Companys);
        }

       
    }
}