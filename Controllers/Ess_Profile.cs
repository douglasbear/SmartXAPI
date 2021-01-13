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
        [HttpGet("details")]
        public ActionResult GetProfileDetails(int nEmpID, int nFnyearID)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);

            string sqlEmployeeDetails = "select CONVERT(VARCHAR,vw_PayEmployee.d_DOB, 106) as d_DOB1,CONVERT(VARCHAR,vw_PayEmployee.d_HireDate, 106) as d_HireDate1,* from vw_PayEmployee where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3";
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

                    EmployeeDetails = myFunctions.AddNewColumnToDataTable(EmployeeDetails, "EmployeeImage", typeof(string), null);
                    if (EmployeeDetails.Rows[0]["i_Employe_Image"] != null)
                    {
                        DataRow dataRow = EmployeeDetails.Rows[0];
                        string ImageData = dataRow["i_Employe_Image"].ToString();
                        if (ImageData != "")
                        {
                            byte[] Image = (byte[])dataRow["i_Employe_Image"];
                            EmployeeDetails.Rows[0]["EmployeeImage"] = "data:image/png;base64," + Convert.ToBase64String(Image, 0, Image.Length);
                            EmployeeDetails.Columns.Remove("i_Employe_Image");
                        }
                        EmployeeDetails.AcceptChanges();
                    }
                    
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