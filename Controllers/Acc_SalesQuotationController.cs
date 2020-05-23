using System.Collections.Generic;
using AutoMapper;
using SmartxAPI.Data;
using SmartxAPI.Dtos;
using SmartxAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.Profiles;
using System;
using System.Linq;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("salesquotation")]
    [ApiController]
    public class Acc_SalesQuotationController : ControllerBase
    {
        private readonly IAcc_SalesQuotationRepo _repository;
        private readonly IMapper _mapper;

        private readonly JWTManager _jwtManager;

        public Acc_SalesQuotationController(IAcc_SalesQuotationRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
       
        


        [HttpGet("list")]
        public ActionResult <VwInvSalesQuotationNoSearch> GetSalesQuotationList(int? nCompanyId,int nFnYearId)
        {

            //var identity = HttpContext.User.Identity as ClaimsIdentity;
            //string CompanyID = _jwtManager.GetClaim(identity.ToString(), "j");
            try{
                    var QuotationList = _repository.GetSalesQuotationList(nCompanyId,nFnYearId);
                    if(!QuotationList.Any())
                    {
                       return NotFound("No Results Found");
                    }else{
                        return Ok(QuotationList);
                    }
            }catch(Exception e){
                return BadRequest(e);
            }
        }

        

        
    }
}