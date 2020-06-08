using System.Collections.Generic;
using AutoMapper;
using SmartxAPI.Data;
using SmartxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using SmartxAPI.Dtos;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using System;

namespace SmartxAPI.Controllers
{
    [Route("company")]
    [ApiController]
    public class Acc_CompanyController : ControllerBase
    {
        private readonly IDataAccessLayer _dataAccess; 
        private readonly IApiFunctions _api;

        public Acc_CompanyController(IDataAccessLayer data,IApiFunctions api)
        {
            _dataAccess=data;
            _api=api;
        }
       
        //GET api/Company/list
        [HttpGet("list")]
        public ActionResult GetAllCompanys()
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table="Acc_Company";
            string X_Fields = "N_CompanyId as nCompanyId,X_CompanyName as xCompanyName,X_CompanyCode as xCompanyCode";
            string X_Crieteria = "B_Inactive =@p1";
            string X_OrderBy="X_CompanyName";
            Params.Add("@p1",0);
            try{
                    dt=_dataAccess.Select(X_Table,X_Fields,X_Crieteria,Params,X_OrderBy);
                    if(dt.Rows.Count==0)
                        {
                            return StatusCode(200,_api.Response(200 ,"No Results Found" ));
                        }else{
                            return Ok(dt);
                        }
                
            }catch(Exception e){
                return StatusCode(404,_api.Response(404,"Error"));
            }
          
        }

       
    }
}