
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
    [Route("documentCode")]
    [ApiController]
    public class Gen_CodeGenerate : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Gen_CodeGenerate(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            //FormID = 188;
        }


        [HttpGet("code")]
        public ActionResult GetCode(string docNo, int nFnYearID, int formID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    string newCode = "";
                    string masterTable = "";
                    string column = "";
                    if (formID == 188)
                    {
                        masterTable = "Pay_Employee";
                        column = "x_EmpCode";


                    }

                    if (docNo == "@Auto" || docNo == "new")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", formID);
                        newCode = dLayer.GetAutoNumber(masterTable, column, Params, connection, transaction);
                        if (newCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Employee Code")); }
                    }
                   SortedList output=new SortedList();
                   output.Add("newCode",newCode);


                    return Ok(_api.Success(output));

                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
    }
}