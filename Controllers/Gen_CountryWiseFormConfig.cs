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
    [Route("genFormConfig")]
    [ApiController]
    public class Gen_CountryWiseFormConfig : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Gen_CountryWiseFormConfig(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 0;
        }



        [HttpGet("list")]
        public ActionResult FormContrils(int nCountryID,int nFormID)
        {
            SortedList Params = new SortedList();

            string  sqlCommandText = "select * from Gen_CountryWiseFormConfig where N_CountryID=@nCountryID and N_FormID=@nFormID ";

            Params.Add("@nCountryID", nCountryID);
            Params.Add("@nFormID", nFormID);
            DataTable dt=new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                SortedList res = new SortedList();
                res.Add("Details", dt);
                return Ok(api.Success(res));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
 
    }
}