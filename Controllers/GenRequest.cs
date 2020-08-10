using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("general")]
    [ApiController]
    
    
    
    public class GenDefults : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        

        public GenDefults(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

       
        //GET api/Projects/list
        [HttpGet("defults/{type}") ]
        public ActionResult GetDefults (string type)
        {
            int id=0;
            switch(type){
                case "LocationType": id=1;
                break;
                case "PartnerType": id=25;
                break;
                case "ProductType": id=36;
                break;
                default: return Ok("Invalid Type");
            }
            string X_Criteria="N_DefaultId=@p1";
            SortedList param = new SortedList(){{"@p1",id}};
            
            DataTable dt=new DataTable();
            
            string sqlCommandText="select * from Gen_Defaults where "+X_Criteria;
                
            try{
                                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
                }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }else{
                            return Ok(api.Success(dt));
                        }
                
            }catch(Exception e){
                return BadRequest(api.Error(e));
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
                                                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
                }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }else{
                            return Ok(api.Success(dt));
                        }
                
            }catch(Exception e){
                return BadRequest(api.Error(e));
            }   
        }

       
    }
}