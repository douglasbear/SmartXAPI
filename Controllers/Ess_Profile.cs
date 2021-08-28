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

            // string sqlEmployeeDetails = "select CONVERT(VARCHAR,vw_PayEmployee.d_DOB, 106) as d_DOB1,CONVERT(VARCHAR,vw_PayEmployee.d_HireDate, 106) as d_HireDate1,* from vw_PayEmployee where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3";

            string sqlEmployeeDetails = "SELECT CONVERT(VARCHAR, vw_PayEmployee.D_DOB, 106) AS d_DOB1, CONVERT(VARCHAR, vw_PayEmployee.D_HireDate, 106) AS d_HireDate1,  vw_PayEmployee.*, Pay_Employee.X_EmpName AS X_ReportTo FROM vw_PayEmployee LEFT OUTER JOIN Pay_Employee ON vw_PayEmployee.N_CompanyID = Pay_Employee.N_CompanyID AND vw_PayEmployee.N_EmpID = Pay_Employee.N_EmpID AND vw_PayEmployee.N_FnYearID = Pay_Employee.N_FnYearID LEFT OUTER JOIN Pay_Supervisor ON Pay_Employee.N_EmpID = Pay_Supervisor.N_EmpID AND Pay_Employee.N_CompanyID = Pay_Supervisor.N_CompanyID AND vw_PayEmployee.N_ReportToID = Pay_Supervisor.N_SupervisorID AND vw_PayEmployee.N_CompanyID = Pay_Supervisor.N_CompanyID where vw_PayEmployee.N_CompanyID=@p1 and vw_PayEmployee.N_FnYearID=@p2 and vw_PayEmployee.N_EmpID=@p3";
            // string sqlSalary = " Select X_Description AS X_SalaryName,CONVERT(varchar, CAST(N_Value AS money), 1) as N_Amount from vw_EmpPayInformation  WHERE N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3 and N_PayMethod in (0,3) ";
            
            string sqlSalary ="SELECT ROW_NUMBER() OVER (ORDER BY vw_PayEmployeePayHistory.N_PayID) As Srl,vw_PayEmployeePayHistory.* FROM         dbo.vw_PayEmployeePayHistory WHERE  (dbo.vw_PayEmployeePayHistory.N_CompanyID =@p1) AND (dbo.vw_PayEmployeePayHistory.N_EmpID =@p3) AND (dbo.vw_PayEmployeePayHistory.N_FnYearID =@p2) Order by D_EffectiveDate Desc,vw_PayEmployeePayHistory.N_PayTypeID";
            string sqlEducation = "Select * from Pay_EmployeeEducation where N_CompanyID=@p1 and N_EmpID=@p3";
            string sqlExperience = "Select N_CompanyID,N_JobID,N_EmpID,X_Position,X_Company,X_Year,X_Country,X_Industry,CONVERT(VARCHAR,D_From,106) as D_From,CONVERT(VARCHAR,D_To,106) as D_To,X_Department from Pay_EmploymentHistory where N_CompanyID=@p1 and N_EmpID=@p3";
            string sqlAsset = "Select X_ItemCode,X_ItemName,X_Category from vw_AssetMaster where N_CompanyID=@p1 and N_EmpID=@p3 and N_Status<2";
            string sqlFamily = "select N_DependenceID,X_DName,X_DLName,X_FGender,X_Relation,CONVERT(VARCHAR,Pay_EmployeeDependence.D_DDOB, 106) as D_DDOB1 from Pay_EmployeeDependence Inner Join Pay_Relation on Pay_EmployeeDependence.N_RelationID = Pay_Relation.N_RelationID and Pay_EmployeeDependence.N_CompanyID = Pay_Relation.N_CompanyID Where Pay_EmployeeDependence.N_CompanyID=@p1 and N_EmpID=@p3";


            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnyearID);
            Params.Add("@p3", nEmpID);
            Params.Add("@p4", nUserID);

            DataTable EmployeeDetails = new DataTable();
            DataTable SalaryDetails = new DataTable();
            DataTable EducationDetails = new DataTable();
            DataTable ExperienceDetails = new DataTable();
            DataTable AssetDetails = new DataTable();
            DataTable FamilyDetails = new DataTable();


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

                    EducationDetails = dLayer.ExecuteDataTable(sqlEducation, Params, connection);
                    EducationDetails = api.Format(EducationDetails, "EducationDetails");

                    ExperienceDetails = dLayer.ExecuteDataTable(sqlExperience, Params, connection);
                    ExperienceDetails = api.Format(ExperienceDetails, "ExperienceDetails");

                    AssetDetails = dLayer.ExecuteDataTable(sqlAsset, Params, connection);
                    AssetDetails = api.Format(AssetDetails, "AssetDetails");

                    FamilyDetails = dLayer.ExecuteDataTable(sqlFamily, Params, connection);
                    FamilyDetails = api.Format(FamilyDetails, "FamilyDetails");
                }
                dt.Tables.Add(EmployeeDetails);
                dt.Tables.Add(SalaryDetails);
                dt.Tables.Add(EducationDetails);
                dt.Tables.Add(ExperienceDetails);
                dt.Tables.Add(AssetDetails);
                dt.Tables.Add(FamilyDetails);

                return Ok(api.Success(dt));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
 
    }
}