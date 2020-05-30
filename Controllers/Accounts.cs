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
        private readonly IDataAccessLayer _dataAccess;
        
        public Accounts(IApiFunctions api,IDataAccessLayer dataaccess)
        {
            _api = api;
            _dataAccess=dataaccess;
        }

        [HttpGet("glaccount/list")]
        public ActionResult GetGLAccountList(int? nCompanyId,int? nFnYearId,int nPaymentMethodId)
        {
            if(nCompanyId==null){return StatusCode(404,_api.Response(404,"Company ID Required"));}                       
            if(nFnYearId==null){return StatusCode(404,_api.Response(404,"FnYear ID Required"));}                       
                
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table="vw_AccMastLedger";
            string X_Fields = "[Account Code],Account,N_CompanyID,N_LedgerID,X_Level,N_FnYearID,N_CashBahavID,X_Type";
            string X_Crieteria ="";
            string X_OrderBy="[Account Code]";
            
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);
            Params.Add("@p3","A");
            Params.Add("@p4","L");
            
            if (nPaymentMethodId == 2){X_Crieteria = "N_CompanyID=@p1 and N_FnYearID=@p2 and (X_Type =@p3 or X_Type =@p4)";}
            else{X_Crieteria = "N_CompanyID=@p1 and N_FnYearID=@p2 and (X_Type =@p3 or X_Type =@p4) and (N_CashBahavID=@p5 or N_CashBahavID=@p6)";
                Params.Add("@p5",4);
                Params.Add("@p6",5);
                }

            try{
                dt=_dataAccess.Select(X_Table,X_Fields,X_Crieteria,Params,X_OrderBy);
                if(dt.Rows.Count==0)
                    {
                        return StatusCode(200,_api.Response(200 ,"No Results Found" ));
                    }else{
                        return Ok(dt);
                    }
                
            }catch(Exception e){
                return StatusCode(404,_api.Response(404,e.Message));
            }
        }       
    }
}