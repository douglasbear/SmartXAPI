using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("Banks")]
    [ApiController]
    public class Banks : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        
        
        public Banks(IApiFunctions api,IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer=dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetBanksList(int? nCompanyId)
        {
            string sqlCommandText="";
            if(nCompanyId==null){return Ok(_api.Warning("Company ID Required"));}                       
                              
                
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            Params.Add("@p1",nCompanyId);


                sqlCommandText="select code,Description from vw_AccBank_Disp where N_CompanyID=@p1 and B_isCompany =1 group by code,Description";
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                connection.Open();
                dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                dt=_api.Format(dt);
                if(dt.Rows.Count==0)
                    {
                        return Ok(_api.Warning("No Results Found" ));
                    }else{
                        return Ok(_api.Success(dt));
                    }
                
            }
            catch(Exception e){
                return Ok(_api.Error(e));
            }
        }       
    }
}