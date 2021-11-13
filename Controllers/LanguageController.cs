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
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

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
        private readonly string connectionString;

        public LanguageController(ILanguageRepo repository, IMapper mapper, IApiFunctions api, IDataAccessLayer dl, IConfiguration conf)
        {
            _repository = repository;
            _mapper = mapper;
            _api = api;
            dLayer = dl;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult<LanLanguage> GetLanguageList()
        {
            try
            {
                var LanguageList = _repository.GetLanguageList();
                if (!LanguageList.Any())
                {
                    return StatusCode(200, _api.Response(200, "No Results Found"));
                }
                else
                {
                    return Ok(LanguageList);
                }
            }
            catch (Exception e)
            {
                return StatusCode(403, _api.Error(User,e));
            }
        }


        [HttpGet("ml-dataset")]
        public ActionResult GetControllsListnew(int nLangId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_WebLanMultilingual where LanguageId=@p1 order by X_WFormName,X_WControlName,Text";
            Params.Add("@p1", nLangId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                }
                Dictionary<string, Dictionary<string, string>> MlData = new Dictionary<string, Dictionary<string, string>>();
                foreach (string ScreenName in dt.AsEnumerable().Select(row => row.Field<string>("X_WFormName")).Distinct())
                {

                    Dictionary<string, string> Controll = new Dictionary<string, string>();
                    foreach (var ControllsArray in dt.AsEnumerable().Where(row => row.Field<string>("X_WFormName") == ScreenName))
                    {
                        Controll.Add(ControllsArray["X_WControlName"].ToString(), ControllsArray["Text"].ToString());
                    }
                    MlData.Add(ScreenName, Controll);
                }
                return Ok(MlData);


            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e.Message));
            }
        }
    }
}