using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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


[HttpGet("file")]
  public async Task<IActionResult> Download(string filename)  
  {  
      if (filename == null)  
          return Content("filename not present");  

          var path ="";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                          SortedList param = new SortedList();
                param.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                DataTable tblDetails = dLayer.ExecuteDataTable("select ISNULL(X_Value,'') AS X_Value from Gen_Settings where X_Description ='EmpDocumentLocation' and N_CompanyID =@nCompanyID", param, connection);
                path = tblDetails.Rows[0]["X_Value"].ToString();
                  }


            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }
       path = path + filename;
  
      var memory = new MemoryStream();  
      using (var stream = new FileStream(path, FileMode.Open))  
      {  
          await stream.CopyToAsync(memory);  
      }  
      memory.Position = 0;  
      return File(memory, GetContentType(path), Path.GetFileName(path));  
  } 

     private string GetContentType(string path)  
        {  
            var types = GetMimeTypes();  
            var ext = Path.GetExtension(path).ToLowerInvariant();  
            return types[ext];  
        }  

                private Dictionary<string, string> GetMimeTypes()  
        {  
            return new Dictionary<string, string>  
            {  
                {".txt", "text/plain"},  
                {".pdf", "application/pdf"},  
                {".doc", "application/vnd.ms-word"},  
                {".docx", "application/vnd.ms-word"},  
                {".xls", "application/vnd.ms-excel"},  
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"}, 
                {".png", "image/png"},  
                {".jpg", "image/jpeg"},  
                {".jpeg", "image/jpeg"},  
                {".gif", "image/gif"},  
                {".csv", "text/csv"}  
            };  
        } 

       
    }
}