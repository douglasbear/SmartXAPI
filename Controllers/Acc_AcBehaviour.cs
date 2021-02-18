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
    [Route("accountbehaviour")]
    [ApiController]
    public class Acc_AccountBehaviour : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1113;

        public Acc_AccountBehaviour(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }




        [HttpGet("Account")]
        public ActionResult GetContractTypeList()
        {
            int nCompanyID = myFunctions.GetCompanyID(User);

            SortedList param = new SortedList() { { "@p1", nCompanyID } };

            DataTable dt = new DataTable();

            string sqlCommandText = "select * from Acc_LedgerBehaviour where N_CompanyID=@p1";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt = dLayer.ExecuteDataTable(sqlCommandText, param, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpGet("Payment")]
        public ActionResult GetCustomerProjectList()
        {
            int nCompanyID = myFunctions.GetCompanyID(User);

            SortedList param = new SortedList() { { "@p1", nCompanyID } };

            DataTable dt = new DataTable();

            string sqlCommandText = "select * from vw_accmastledger where N_CompanyID=@p1 and N_Type=2";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt = dLayer.ExecuteDataTable(sqlCommandText, param, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }


    }
}