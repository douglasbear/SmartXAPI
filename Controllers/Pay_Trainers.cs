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
     [Route("Trainers")]
     [ApiController]
    public class PayTrainers : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
         private readonly IMyFunctions myFunctions;
        //  private readonly int N_FormID = ;
           public PayTrainers(IDataAccessLayer dl,IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions=myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
         [HttpGet("TrainersList") ]
        public ActionResult GetTrainersList ()
        {    int nCompanyID=myFunctions.GetCompanyID(User);
  
            SortedList param = new SortedList(){{"@nCompanyID",nCompanyID}};
            
            DataTable dt=new DataTable();
            
            string sqlCommandText="select X_TraineeCode,X_TraineeName,X_TraineeAddress,N_TraineeID,N_CompanyID from Pay_Trainers where N_companyID=@nCompanyID";
                
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
    