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
    [Route("schCourse")]
    [ApiController]
    public class Sch_Class : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =158 ;


        public Sch_Class(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = 
            conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("details")]
        public ActionResult ClassDetails(int n_ClassID)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_Sch_Class where N_CompanyID=@p1  and n_ClassID=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", n_ClassID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                
                    MasterTable = api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);
                }
                return Ok(api.Success(dt));               
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }



        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nClassID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ClassID"].ToString());
                string xclass = MasterTable.Rows[0]["X_Class"].ToString();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_ClassCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Sch_Class", "X_ClassCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Course Code")); }
                        MasterTable.Rows[0]["X_ClassCode"] = Code;
                         object courseCount = dLayer.ExecuteScalar("select count(1) from Sch_Class  where N_CompanyID="+ nCompanyID +" and  x_class='"+xclass+"'", Params, connection, transaction);
                         
                           if (myFunctions.getIntVAL(courseCount.ToString()) > 0){
                             return Ok(api.Error(User, "Course Already exist"));
                           }
                            
                    }
                    MasterTable.Columns.Remove("n_FnYearId");

                    // if (nClassID > 0) 
                    // {  
                    //     dLayer.DeleteData("Sch_Class", "N_ClassID", nClassID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    // }

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and X_ClassCode='" + Code + "'";
                    string X_Criteria = "N_CompanyID=" + nCompanyID + "";

                    nClassID = dLayer.SaveData("Sch_Class", "N_ClassID",DupCriteria,X_Criteria, MasterTable, connection, transaction);

                    if (nClassID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Course Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        [HttpGet("list") ]
        public ActionResult ClassList(int nCompanyID,int nClassTypeID)
        {    
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="";

            if(nClassTypeID>0)
                sqlCommandText="select * from vw_Sch_Class where N_CompanyID=@p1 and N_ClassTypeID=@p2";
            else    
                sqlCommandText="select * from vw_Sch_Class where N_CompanyID=@p1";

            param.Add("@p1", nCompanyID);  
            param.Add("@p2", nClassTypeID);                
                
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
                }
                // if(dt.Rows.Count==0)
                // {
                //     return Ok(api.Notice("No Results Found"));
                // }
                // else
                // {
                    return Ok(api.Success(dt));
                //}
                
            }
            catch(Exception e)
            {
                return Ok(api.Error(User,e));
            }   
        }   
      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nClassID)
        {

            int Results = 0;
            int nCompanyID=myFunctions.GetCompanyID(User);
            try
            {                        
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Sch_Class ", "N_ClassID", nClassID, "N_CompanyID =" + nCompanyID, connection, transaction);
                
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("Course deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User,"Unable to delete Course"));
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("REFERENCE constraint"))
                    return Ok(api.Error(User, "Unable to delete course! It has been used."));
                else
                    return Ok(api.Error(User, ex));
            }



        }

        [HttpGet("multilist") ]
        public ActionResult multiCourselist(int nCompanyID,int nAdmissionID)
        {    
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="";

            if(nAdmissionID>0)
                sqlCommandText="select * from Vw_SchCourseDetails where N_CompanyID=@p1 and N_StudentID=@p2";
            else    
                sqlCommandText="select * from Vw_SchCourseDetails where N_CompanyID=@p1";

            param.Add("@p1", nCompanyID);  
            param.Add("@p2", nAdmissionID);                
                
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
                }
              
                    return Ok(api.Success(dt));
               
                
            }
            catch(Exception e)
            {
                return Ok(api.Error(User,e));
            }   
        }   
      
    }
}

