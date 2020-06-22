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
using SmartxAPI.Dtos.Language;

namespace SmartxAPI.Controllers
{
    //[Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("language")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private readonly ILanguageRepo _repository;
        private readonly IMapper _mapper;
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;


        public LanguageController(ILanguageRepo repository, IMapper mapper,IApiFunctions api, IDataAccessLayer dl)
        {
            _repository = repository;
            _mapper = mapper;
            _api=api;
            dLayer = dl;
        }

        [HttpGet("list")]
        public ActionResult <LanLanguage> GetLanguageList()
        {
            try{
                    var LanguageList = _repository.GetLanguageList();
                    if(!LanguageList.Any())
                    {
                       return StatusCode(200,_api.Response(200,"No Results Found"));
                    }else{
                        return Ok(LanguageList);
                    }
            }catch(Exception e){
                return StatusCode(403,_api.ErrorResponse(e));
            }
        }

        [HttpGet("ml-datasetold")]
        public ActionResult GetControllsList()
        {
            try{
                    var LanguageList = _repository.GetControllsListAsync();
                    if(!LanguageList.Any())
                    {
                       return StatusCode(200,_api.Response(200,"No Results Found"));
                    }else{
                        return Ok(LanguageList);
                    }
            }catch(Exception e){
                return StatusCode(403,_api.ErrorResponse(e));
            }
        }
        

        [HttpGet("ml-dataset")]
        public ActionResult GetControllsListnew()
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from vw_WebLanMultilingual";

            try{
                dt=dLayer.ExecuteDataTable(sqlCommandText,Params);
                
                Dictionary<string,Dictionary<string,Dictionary<string,string>>> MlData = new Dictionary<string,Dictionary<string,Dictionary<string,string>>>();
                foreach(string ScreenName in dt.AsEnumerable().Select(row => row.Field<string>("X_WFormName")).Distinct()){
                    
                    Dictionary<string,Dictionary<string,string>> Controlls = new Dictionary<string,Dictionary<string,string>>();
                    foreach(string CntrlName in dt.AsEnumerable().Where(row => row.Field<string>("X_WFormName")==ScreenName).Select(row => row.Field<string>("X_WControlName")).Distinct()){
                        
                        Dictionary<string,string> Lang = new Dictionary<string,string>();
                        foreach(var ControllStringItem in dt.AsEnumerable().Where(row => row.Field<string>("X_WFormName")==ScreenName && row.Field<string>("X_WControlName")==CntrlName)){
                            Lang.Add(ControllStringItem["LanguageId"].ToString(),ControllStringItem["Text"].ToString());
                        }
                        Controlls.Add(CntrlName,Lang);
                    }
                    MlData.Add(ScreenName,Controlls);
                }
            return Ok(MlData);

                
            }catch(Exception e){
                return StatusCode(403,_api.ErrorResponse(e));
            }
        }
    }
}