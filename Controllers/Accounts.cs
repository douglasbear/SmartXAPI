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
    [Route("accounts")]
    [ApiController]
    public class Accounts : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        
        
        public Accounts(IApiFunctions api,IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer=dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("glaccount/list")]
        public ActionResult GetGLAccountList(int? nCompanyId,int? nFnYearId,string xType)
        {
            string sqlCommandText="";
            if(nCompanyId==null){return StatusCode(404,_api.Response(404,"Company ID Required"));}                       
            if(nFnYearId==null){return StatusCode(404,_api.Response(404,"FnYear ID Required"));}                       
                
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);
            Params.Add("@p3",xType);

            if(xType!="All")
                sqlCommandText="select [Account Code] as accountCode,Account,N_CompanyID,N_LedgerID,X_Level,N_FnYearID,N_CashBahavID,X_Type from vw_AccMastLedger where N_CompanyID=@p1 and N_FnYearID=@p2 and X_Type =@p3 and B_Inactive = 0  order by [Account Code]";
            else
                sqlCommandText="select [Account Code] as accountCode,Account,N_CompanyID,N_LedgerID,X_Level,N_FnYearID,N_CashBahavID,X_Type from vw_AccMastLedger where N_CompanyID=@p1 and N_FnYearID=@p2 and B_Inactive = 0  order by [Account Code]";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                dt=_api.Format(dt);
                if(dt.Rows.Count==0)
                    {
                        return StatusCode(200,_api.Response(200 ,"No Results Found" ));
                    }else{
                        return Ok(dt);
                    }
                
            }
            catch(Exception e){
                return StatusCode(403,_api.ErrorResponse(e));
            }
        }       
    }
}