using SmartxAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Linq;
using System.Security.Claims;
using SmartxAPI.GeneralFunctions;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("services")]
    [ApiController]
    public class CommenServiceController : ControllerBase
    {
        private readonly ICommenServiceRepo _repository;
        private readonly IApiFunctions _api;


        public CommenServiceController(ICommenServiceRepo repository,IApiFunctions api)
        {
            _repository = repository;
            _api = api;
        }
       

        [HttpGet("auth-user/{reqType}")]
        public ActionResult AuthenticateUser(string reqType)
        {
            try{
                int userid = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                int companyid = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid).Value);
                string companyname = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.StreetAddress).Value;
                string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
                
                var user = _repository.Authenticate(companyid,companyname,username,userid,reqType);

                if (user == null){ return StatusCode(403,_api.Response(403,"Unauthorized Access" )); }

                    return Ok(user);
                }
                catch (Exception ex)
                {
                   return StatusCode(403,_api.ErrorResponse(ex));
                }  
        }

       
    }
}