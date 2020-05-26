using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using SmartxAPI.Controllers;
using System.Collections.Generic;
using SmartxAPI.Models;
using SmartxAPI.Dtos.General;

namespace SmartxAPI.Profiles
{
    public class APIResponse{
        
      
          public Gen_APIResponse ResponseAPI(string ErrorCode,string Message)
        {
                var res=new Gen_APIResponse();
                res.ErrorCode=ErrorCode;
                res.Message=Message;
                return res;
        }
}

        
    
}