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
    [Route("EssProfile")]
    [ApiController]
    public class Ess_Profile : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Ess_Profile(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("listDetails")]
        public ActionResult GetProfileDetails(int nEmpID, int nFnyearID)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);

            string sqlEmployeeDetails = "SELECT N_EmpID, X_Position, X_Department, D_DOB, X_Phone1, X_EmailID ,X_Sex,D_HireDate,X_MaritalStatus,X_PassportNo,D_PassportExpiry,X_IqamaNo,D_IqamaExpiry FROM vw_PayEmployee  where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3";
            string sqlSalary = " Select X_Description AS X_SalaryName,CONVERT(varchar, CAST(N_Value AS money), 1) as N_Amount from vw_EmpPayInformation  WHERE N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3 and N_PayMethod = 0 ";


            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnyearID);
            Params.Add("@p3", nEmpID);
            Params.Add("@p4", nUserID);

            DataTable EmployeeDetails = new DataTable();
            DataTable SalaryDetails = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    EmployeeDetails = dLayer.ExecuteDataTable(sqlEmployeeDetails, Params, connection);
                    EmployeeDetails = api.Format(EmployeeDetails, "EmployeeDetails");
                    SalaryDetails = dLayer.ExecuteDataTable(sqlSalary, Params, connection);
                    SalaryDetails = api.Format(SalaryDetails, "SalaryDetails");
                }
                dt.Tables.Add(EmployeeDetails);
                dt.Tables.Add(SalaryDetails);

                return Ok(api.Success(dt));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
 
    }
}