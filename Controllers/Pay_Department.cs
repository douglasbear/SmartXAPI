using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("department")]
    [ApiController]
    public class Pay_Department : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Pay_Department(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 0;
        }

        [HttpGet("list")]
        public ActionResult GetJobTitle(int nDivisionID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);

            string sqlCommandText="Select N_CompanyID,N_DepartmentID,N_DivisionID,Code,Description from vw_PayDepartment_Disp Where N_CompanyID=@nCompanyID order by Code";
            if(nDivisionID>0){
            Params.Add("@nDivisionID",nDivisionID);
             sqlCommandText="Select N_CompanyID,N_DepartmentID,N_DivisionID,Code,Description from vw_PayDepartment_Disp Where N_CompanyID=@nCompanyID and N_DivisionID=@nDivisionID order by Code";
             }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        }
    }
