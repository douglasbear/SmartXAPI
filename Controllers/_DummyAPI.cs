using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("apitools")]
    [ApiController]
    public class _DummyAPI : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public _DummyAPI(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString(myCompanyID._ConnectionString);
        }


        [HttpGet("dummy")]
        public ActionResult GetVoucherDummy(string table1,string table2,string xCriteria)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(connectionString))
                {
                    Con.Open();
                    if(xCriteria!=null && xCriteria!=""){
                        xCriteria = " where " + xCriteria; 
                    }
                    string sqlCommandText = "select * from "+table1+xCriteria;
                    DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, Con);
                    masterTable = api.Format(masterTable, "master");
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add(masterTable);

                    if(table2!=null && table2!=""){
                    string sqlCommandText2 = "select * from "+table2+xCriteria;
                    DataTable detailTable = dLayer.ExecuteDataTable(sqlCommandText2, Con);
                    detailTable = api.Format(detailTable, "details");
                    dataSet.Tables.Add(detailTable);
                    }
                    return Ok(dataSet);
                }
            }
            catch (Exception e)
            {
                return StatusCode(403, api.Error(User,e));
            }
        }



    }
}