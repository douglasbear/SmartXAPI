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
    [Route("schDashboard")]
    [ApiController]
    public class SchDashboard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public SchDashboard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("details")]
        public ActionResult GetDashboardDetails(int nFnYearID,int nBranchID,bool bAllBranchData)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            string crieteria = "";
            

            if (bAllBranchData == true)
            {
            crieteria="";
           
            }
            else
            {
            crieteria=" and N_BranchID="+nBranchID;
           
            }
                     
            string sqlStudent = "SELECT COUNT(*) as N_Count FROM vw_SchAdmission WHERE N_CompanyID = " + nCompanyID + " and N_AcYearID="+nFnYearID  + crieteria ;
            string sqlMaleStudent = "SELECT COUNT(*) as N_Count FROM vw_SchAdmission WHERE x_Gender='Male' and  N_CompanyID = " + nCompanyID + " and  N_AcYearID="+nFnYearID  + crieteria ;
            string sqlFemaleStudent = "SELECT COUNT(*) as N_Count FROM vw_SchAdmission WHERE x_Gender='FeMale' and N_CompanyID = " + nCompanyID + " and  N_AcYearID="+nFnYearID  + crieteria ;
            string sqlParents = "SELECT COUNT(*) as N_Count FROM vw_Sch_ParentDetails_Disp WHERE N_CompanyID = " + nCompanyID  + crieteria ;
            string sqlTeachers = "SELECT COUNT(*) as N_Count FROM vw_SchTeacher WHERE N_CompanyID = " + nCompanyID + " and N_FnyearID="+nFnYearID  + crieteria ;
            string sqlOtherStaffs = "SELECT COUNT(*) as N_Count FROM Pay_Employee WHERE B_EnableTeacher = 0 and N_CompanyID = " + nCompanyID + " and N_FnyearID="+nFnYearID  + crieteria ;
            string sqlCourseBasedStudents = "select X_Class,COUNT(1) as N_Count from  vw_SchAdmission WHERE  N_CompanyID = " + nCompanyID + " and N_AcYearID="+nFnYearID  + crieteria + "  group by X_Class";
            string sqlCountryBasedStudents = "SELECT X_Nationality,COUNT(1) as N_Count from  vw_SchAdmission WHERE  N_CompanyID = " + nCompanyID + " and N_AcYearID="+nFnYearID  + crieteria + "  group by X_Nationality";
            // string sqlCourseBasedStudents = "select X_Class,COUNT(1) as N_Count from  vw_SchAdmission group by X_Class";
            // string sqlCountryBasedStudents = "SELECT X_Nationality,COUNT(1) as N_Count from  vw_SchAdmission group by X_Nationality";

            SortedList Data = new SortedList();
            DataTable Student = new DataTable();
            DataTable MaleStudent = new DataTable();
            DataTable FeMaleStudent = new DataTable();
            DataTable Parents = new DataTable();
            DataTable Teachers = new DataTable();
            DataTable OtherStaffs = new DataTable();
            DataTable CourseBasedStudents = new DataTable();
            DataTable CountryBasedStudents = new DataTable();
       
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                     //bool B_customer = myFunctions.CheckPermission(nCompanyID, 1302, "Administrator", "X_UserCategory", dLayer, connection);
                    Student = dLayer.ExecuteDataTable(sqlStudent, Params, connection);
                    MaleStudent = dLayer.ExecuteDataTable(sqlMaleStudent, Params, connection);
                    FeMaleStudent = dLayer.ExecuteDataTable(sqlFemaleStudent, Params, connection);
                    Parents = dLayer.ExecuteDataTable(sqlParents, Params, connection);
                    Teachers = dLayer.ExecuteDataTable(sqlTeachers, Params, connection);
                    OtherStaffs = dLayer.ExecuteDataTable(sqlOtherStaffs, Params, connection);
                    CourseBasedStudents = dLayer.ExecuteDataTable(sqlCourseBasedStudents, Params, connection);
                    CountryBasedStudents = dLayer.ExecuteDataTable(sqlCountryBasedStudents, Params, connection);
                    // MonthlySales = dLayer.ExecuteDataTable(sqlMonthlySales, Params, connection);
                   

                }

                
                Student.AcceptChanges();
                MaleStudent.AcceptChanges();
                FeMaleStudent.AcceptChanges();
                Parents.AcceptChanges();
                Teachers.AcceptChanges();
                OtherStaffs.AcceptChanges();
                CourseBasedStudents.AcceptChanges();
                CountryBasedStudents.AcceptChanges();
                // MonthlySales.AcceptChanges();
                if (Student.Rows.Count > 0) Data.Add("student", Student);
                if (MaleStudent.Rows.Count > 0) Data.Add("malestudent", MaleStudent);
                if (FeMaleStudent.Rows.Count > 0) Data.Add("feMaleStudent", FeMaleStudent);
                if (Parents.Rows.Count > 0) Data.Add("parents", Parents);
                if (Teachers.Rows.Count > 0) Data.Add("teachers", Teachers);
                if (OtherStaffs.Rows.Count > 0) Data.Add("otherStaffs", OtherStaffs);
                if (CourseBasedStudents.Rows.Count > 0) Data.Add("courseBasedStudents", CourseBasedStudents);
                if (CountryBasedStudents.Rows.Count > 0) Data.Add("countryBasedStudents", CountryBasedStudents);
                // if (MonthlySales.Rows.Count > 0) Data.Add("monthlySales", MonthlySales);
                return Ok(api.Success(Data));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
    


    }
}