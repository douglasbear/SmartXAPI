using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("general")]
    [ApiController]
    
    
    
    public class GenDefults : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        

        public GenDefults(IDataAccessLayer dl,IApiFunctions api)
        {
            dLayer = dl;
            _api=api;
        }

       
        //GET api/Projects/list
        [HttpGet("defults/{type}") ]
        public ActionResult GetDefults (string type)
        {
            int id=0;
            switch(type){
                case "PartnerType": id=25;
                break;
                default: return Ok("Invalid Type");
            }
            string X_Criteria="N_DefaultId=@p1";
            SortedList param = new SortedList(){{"@p1",id}};
            
            DataTable dt=new DataTable();
            
            string sqlCommandText="select * from Gen_Defaults where "+X_Criteria;
                
            try{
                    dt=dLayer.ExecuteDataTable(sqlCommandText,param);
                    if(dt.Rows.Count==0)
                        {
                            return Ok(new {});
                        }else{
                            return Ok(dt);
                        }
                
            }catch(Exception e){
                return StatusCode(403,_api.ErrorResponse(e));
            }   
        }

        [HttpGet("lookup/{type}") ]
        public ActionResult GetLookup (string type)
        {
            int id=0;
            switch(type){
                case "VendorType": id=52;
                break;
                default: return Ok("Invalid Type");
            }
            string X_Criteria="N_ReferId=@p1";
            SortedList param = new SortedList(){{"@p1",id}};
            
            DataTable dt=new DataTable();
            
            string sqlCommandText="select * from Gen_LookupTable where "+X_Criteria;
                
            try{
                    dt=dLayer.ExecuteDataTable(sqlCommandText,param);
                    if(dt.Rows.Count==0)
                        {
                            return Ok(new {});
                        }else{
                            return Ok(dt);
                        }
                
            }catch(Exception e){
                return StatusCode(403,_api.ErrorResponse(e));
            }   
        }

       
    }
}