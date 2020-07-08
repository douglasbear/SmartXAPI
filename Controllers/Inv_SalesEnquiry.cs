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
    [Route("salesenquiry")]
    [ApiController]
    public class Inv_SalesEnquiry : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        
        public Inv_SalesEnquiry(IApiFunctions api,IDataAccessLayer dl)
        {
            _api = api;
            dLayer=dl;
        }

        [HttpGet("list")]
        public ActionResult GetSalesEnqList(int? nCompanyId,int nFnYearId,bool bAllBranchesData,int nBranchId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            Params.Add("@p1",0);
            Params.Add("@p2",nCompanyId);
            Params.Add("@p3",nFnYearId);
            string X_Crieteria;    
            if (bAllBranchesData == true)
                {X_Crieteria = " where B_Processed=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3";}
                else
                {X_Crieteria = " where B_Processed=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3 and N_BranchID=@p4"; 
                Params.Add("@p4",nBranchId);}
        
            string sqlCommandText="select X_CRMCode,[Enquiry Date],X_ClientName,N_CompanyID,N_CRMID,N_SalesmanID,N_FnYearID,D_Date,N_BranchID,N_StatusID,B_Processed from vw_crmmaster "+X_Crieteria+" order by D_Date DESC,X_CRMCode";
            
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