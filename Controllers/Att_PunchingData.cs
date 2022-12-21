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
using System.Net.Http;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("punchingdata")]
    [ApiController]
    public class Att_PunchingData : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        // private readonly IMyReminders myReminders;

        public Att_PunchingData(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1307;
            // myReminders = myRem;
        }


        [HttpGet("list")]
        public ActionResult PunchingData(DateTime dt)
        {
            var url = "http://webcode.me/ua.php";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# program");

            var res =client.GetStringAsync(url);
            Console.WriteLine(res);
            return Ok();
        }


    }
}