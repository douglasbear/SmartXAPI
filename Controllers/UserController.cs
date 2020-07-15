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

        private readonly IMyFunctions myFunctions;

        public UserController(ISec_UserRepo repository,IApiFunctions api,IMyFunctions myFun)
        {
            _repository = repository;
            _api = api;
            myFunctions=myFun;
        }
        [HttpPost("login")]
        public ActionResult Authenticate([FromBody]Sec_AuthenticateDto model)
        {
            try{
                string ipAddress="";
                    if (Request.Headers.ContainsKey("X-Forwarded-For"))
                        ipAddress= Request.Headers["X-Forwarded-For"];
                    else
                        ipAddress= HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                        //var password = myFunctions.EncryptString(model.Password);
                        var password = model.Password;
                    var user = _repository.Authenticate(model.CompanyName,model.Username, password ,ipAddress);

                    if (user == null){ return StatusCode(403,_api.Response(403,"Username or password is incorrect" )); }

                    return Ok(user);
                }
                catch (Exception ex)
                {
                   return StatusCode(403,_api.ErrorResponse(ex));
                }  
        }       
    }
}