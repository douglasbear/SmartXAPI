
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
        public ActionResult GetCode(string docNo, int nFnYearID, int formID, string xDescription)
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
                    if (formID == 64)
                    {
                        masterTable = "Inv_Sales";
                        column = "X_ReceiptNo";


                    }



                    if (docNo == "@Auto" || docNo == "new")
                    {
                        if (formID == 188)
                        {
                            if (xDescription == null || xDescription == "")
                            {
                                Params.Add("N_CompanyID", nCompanyID);
                                Params.Add("N_YearID", nFnYearID);
                                Params.Add("N_FormID", 188);
                                Params.Add("X_Type", "");
                            }
                            else
                            {
                                Params.Add("N_CompanyID", nCompanyID);
                                Params.Add("N_YearID", nFnYearID);
                                Params.Add("N_FormID", 188);
                                Params.Add("X_Type", xDescription);
                            }

                            while (true)
                            {

                                newCode = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate_New", Params, connection, transaction).ToString();
                                object N_Result = dLayer.ExecuteScalar("Select 1 from Pay_Employee Where X_EmpCode ='" + newCode + "' and N_CompanyID= " + nCompanyID, Params, connection, transaction);
                                if (N_Result == null)
                                    break;
                            }
                        }
                        else if (formID == 64)
                        {
                            Params.Add("N_CompanyID", nCompanyID);
                            Params.Add("N_YearID", nFnYearID);
                            Params.Add("N_FormID", 64);
            
                            newCode = dLayer.GetAutoNumber("Inv_Sales", "X_ReceiptNo", Params, connection, transaction);
                            if (newCode == "") { transaction.Rollback(); return Ok(_api.Warning("Unable to generate ")); }
                            
                        }
                    }



                    SortedList output = new SortedList();
                    output.Add("newCode", newCode);


                    return Ok(_api.Success(output));

                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
    }
}