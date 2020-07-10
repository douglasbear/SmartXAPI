using System.Collections.Generic;
using AutoMapper;
using SmartxAPI.Data;
using SmartxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Linq;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("projects")]
    [ApiController]
    public class InvCustomerProjectsController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;

        public InvCustomerProjectsController(IDataAccessLayer dl,IApiFunctions api)
        {
            dLayer = dl;
            _api=api;
        }
       
        //GET api/Projects/list
        [HttpGet("list") ]
        public ActionResult GetAllProjects (int? nCompanyID,int? nFnYearID)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from Vw_InvCustomerProjects where N_CompanyID=@p1 and N_FnYearID=@p2 rder by X_ProjectCode";
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nFnYearID);

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