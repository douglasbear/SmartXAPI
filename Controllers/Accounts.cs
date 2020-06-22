using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("accounts")]
    [ApiController]
    public class Accounts : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        
        public Accounts(IApiFunctions api,IDataAccessLayer dl)
        {
            _api = api;
            dLayer=dl;
        }

        [HttpGet("glaccount/list")]
        public ActionResult GetGLAccountList(int? nCompanyId,int? nFnYearId,int nPaymentMethodId)
        {
            if(nCompanyId==null){return StatusCode(404,_api.Response(404,"Company ID Required"));}                       
            if(nFnYearId==null){return StatusCode(404,_api.Response(404,"FnYear ID Required"));}                       
                
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);
            Params.Add("@p3","A");
            Params.Add("@p4","L");
            string X_Crieteria="";
             if (nPaymentMethodId == 2){X_Crieteria = "where N_CompanyID=@p1 and N_FnYearID=@p2 and (X_Type =@p3 or X_Type =@p4)";}
            else{X_Crieteria = "where N_CompanyID=@p1 and N_FnYearID=@p2 and (X_Type =@p3 or X_Type =@p4) and (N_CashBahavID=@p5 or N_CashBahavID=@p6)";
                Params.Add("@p5",4);
                Params.Add("@p6",5);
                }
            string sqlCommandText="select [Account Code] as accountCode,Account,N_CompanyID,N_LedgerID,X_Level,N_FnYearID,N_CashBahavID,X_Type from vw_AccMastLedger "+X_Crieteria+" order by [Account Code]";

            try{
                dt=dLayer.ExecuteDataTable(sqlCommandText,Params);
                dt=_api.Format(dt);
                if(dt.Rows.Count==0)
                    {
                        return StatusCode(200,_api.Response(200 ,"No Results Found" ));
                    }else{
                        return Ok(dt);
                    }
                
            }catch(Exception e){
                return StatusCode(403,_api.ErrorResponse(e));
            }
        }       
    }
}