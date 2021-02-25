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
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("invminstockalert")]
    [ApiController]
    public class InvMinStockAlert : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;

        public InvMinStockAlert(IDataAccessLayer dl, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

       // GET api/invminstockalert/list    
        [HttpGet("list")]
        public ActionResult GetAllStocks(int? nCompanyID,string xcriteria)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string criteria="";

            if(xcriteria=="All")
                criteria="N_CompanyID=@p1";
            else if(xcriteria=="No Stock")
                criteria="N_CompanyID=@p1 and N_CurrStock=0" ;
            else if(xcriteria=="N_MinQty")
                criteria="N_CompanyID=@p1 and N_CurrStock < = "+xcriteria ;


            string sqlCommandText = "select * from vw_stockstatusbylocation where "+criteria;
            Params.Add("@p1", nCompanyID);
        

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                   return Ok(api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }
        }
    }
}