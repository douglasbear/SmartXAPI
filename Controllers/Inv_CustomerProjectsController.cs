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
    [Route("projects")]
    [ApiController]
    public class InvCustomerProjectsController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;

        public InvCustomerProjectsController(IDataAccessLayer dl, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        //GET api/Projects/list
        [HttpGet("list")]
        public ActionResult GetAllProjects(int? nCompanyID, int? nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from Vw_InvCustomerProjects where N_CompanyID=@p1 and N_FnYearID=@p2 rder by X_ProjectCode";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearID);

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
                    return Ok(dt);
                }

            }
            catch (Exception e)
            {
                return StatusCode(403, api.Error(e));
            }


        }


    }
}