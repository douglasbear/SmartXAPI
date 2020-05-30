using System.Collections.Generic;
using AutoMapper;
using SmartxAPI.Data;
using SmartxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Linq;
using SmartxAPI.GeneralFunctions;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("language")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private readonly ILanguageRepo _repository;
        private readonly IMapper _mapper;
        private readonly IApiFunctions _api;


        public LanguageController(ILanguageRepo repository, IMapper mapper,IApiFunctions api)
        {
            _repository = repository;
            _mapper = mapper;
            _api=api;
        }

        [HttpGet("list")]
        public ActionResult <LanLanguage> GetLanguageList()
        {
            try{
                    var LanguageList = _repository.GetLanguageList();
                    if(!LanguageList.Any())
                    {
                       return StatusCode(200,_api.Response(200,"No Results Found"));
                    }else{
                        return Ok(LanguageList);
                    }
            }catch(Exception e){
                return StatusCode(404,_api.Response(404,e.Message));
            }
        }

        [HttpGet("ml-dataset")]
        public ActionResult GetControllsList()
        {
            try{
                    var LanguageList = _repository.GetControllsListAsync();
                    if(!LanguageList.Any())
                    {
                       return StatusCode(200,_api.Response(200,"No Results Found"));
                    }else{
                        return Ok(LanguageList);
                    }
            }catch(Exception e){
                return StatusCode(404,_api.Response(404,e.Message));
            }
        }


       /*  [HttpPost]
        public ActionResult <VwInvCustomerDisp> CreateCustomer(CustomerCreateDto CustomerCreateDto)
        {
            var CustomerModel = _mapper.Map<InvCustomer>(CustomerCreateDto);
            _repository.CreateCustomer(CustomerModel);
            _repository.SaveChanges();

            var VwInvCustomerDisp = _mapper.Map<VwInvCustomerDisp>(CustomerModel);

            return CreatedAtRoute(nameof(GetCustomerById), new {Id = VwInvCustomerDisp.NCustomerId}, VwInvCustomerDisp);      
        } */

        

        
    }
}