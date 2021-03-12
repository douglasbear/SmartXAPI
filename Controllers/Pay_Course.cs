using AutoMapper;
using SmartxAPI.Data;
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
namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
     [Route("Course")]
     [ApiController]
    public class PayCourse : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
         private readonly IMyFunctions myFunctions;
        //  private readonly int N_FormID = 1083;
           public PayCourse(IDataAccessLayer dl,IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions=myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
         [HttpGet("CourseList") ]
        public ActionResult GetCourseList ()
        {    int nCompanyID=myFunctions.GetCompanyID(User);
  
            SortedList param = new SortedList(){{"@nCompanyID",nCompanyID}};
            
            DataTable dt=new DataTable();
            
            string sqlCommandText="select N_CompanyID,N_CourseID,N_FnYearID,D_StartDate,D_EndDate,X_CourseCode,X_CourseName from Pay_Course where N_CompanyID=@nCompanyID";
                
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
                return Ok(api.Error(e));
            }   
        }
    }
}
    