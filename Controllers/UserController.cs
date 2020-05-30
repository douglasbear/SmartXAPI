using SmartxAPI.Data;
using SmartxAPI.Dtos.Login;
using Microsoft.AspNetCore.Mvc;
using System;
using SmartxAPI.GeneralFunctions;

namespace SmartxAPI.Controllers
{

    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ISec_UserRepo _repository;
        private readonly IApiFunctions _api;

        public UserController(ISec_UserRepo repository,IApiFunctions api)
        {
            _repository = repository;
            _api = api;
        }
        [HttpPost("login")]
        public ActionResult Authenticate([FromBody]Sec_AuthenticateDto model)
        {
            try{
                    var user = _repository.Authenticate(model.CompanyName,model.Username, model.Password);

                    if (user == null){ return StatusCode(403,_api.Response(403,"Username or password is incorrect" )); }

                    return Ok(user);
                }
                catch (Exception ex)
                {
                   return StatusCode(404,_api.Response(404,ex.Message ));
                }  
        }        
    }
}