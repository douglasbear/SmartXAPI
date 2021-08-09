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
     [Route("Course")]
     [ApiController]
    public class PayCourse : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
         private readonly IMyFunctions myFunctions;
          private readonly int N_FormID = 1083;
           public PayCourse(IDataAccessLayer dl,IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions=myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
         [HttpGet("CourseList") ]
        public ActionResult GetCourseList ()
        {    int nCompanyID=myFunctions.GetCompanyID(User);
  
            SortedList param = new SortedList(){{"@nCompanyID",nCompanyID}};
            
            DataTable dt=new DataTable();
            
            string sqlCommandText="select N_CompanyID,N_CourseID,N_FnYearID,D_StartDate,D_EndDate,X_CourseCode,X_CourseName from Pay_Course where N_CompanyID=@nCompanyID";
                
            try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
                }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }else{
                            return Ok(api.Success(dt));
                        }
                
            }catch(Exception e){
                return Ok(api.Error(e));
            }   
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nCourseID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CourseID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string CourseCode = " ";
                    var values = MasterTable.Rows[0]["X_CourseCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                         Params.Add("N_FormID", this.N_FormID);
                        CourseCode = dLayer.GetAutoNumber("Pay_Course", "X_CourseCode", Params, connection, transaction);
                        if (CourseCode == " ") { transaction.Rollback();return Ok(api.Error("Unable to generate Course Details")); }
                        MasterTable.Rows[0]["X_CourseCode"] = CourseCode;
                    }
                    


                    nCourseID = dLayer.SaveData("Pay_Course", "N_CourseID", MasterTable, connection, transaction);
                    if (nCourseID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Course Details Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }

         [HttpGet("Details") ]
        public ActionResult GetCourseDetails (int nCourseID,int nCompanyID,int nFnYearId)
          
        {   DataTable dt=new DataTable();
            SortedList Params = new SortedList();
           //  int nCompanyID=myFunctions.GetCompanyID(User);
              string sqlCommandText="select * from vw_PayCourse where N_CompanyID=@nCompanyID and N_FnYearID=@YearID and N_CourseID=@nCourseID";
               Params.Add("@nCompanyID",nCompanyID);
                Params.Add("@YearID", nFnYearId);
             Params.Add("@nCourseID",nCourseID);
            
            try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }else{
                            return Ok(api.Success(dt));
                        }
                
            }catch(Exception e){
                return Ok(api.Error(e));
            }   
        }

         [HttpDelete("delete")]
        public ActionResult DeleteData(int nCourseID)
        {

             int Results = 0;
            try
            {                        
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Pay_Course", "N_CourseID", nCourseID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_CourseID",nCourseID.ToString());
                    return Ok(api.Success(res,"Course Details deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete Course Details "));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }

        }



    }
}
    