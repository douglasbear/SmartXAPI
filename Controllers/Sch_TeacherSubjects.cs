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
    [Route("schTeacherSubjects")]
    [ApiController]
    public class Sch_TeacherSubjects : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =1501 ;


        public Sch_TeacherSubjects(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("details")]
        public ActionResult TeacherSubjectDetails(int xTeacherSubCode)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_Sch_TeacherSubjectMaster where N_CompanyID=@p1  and x_TeacherSubCode=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", xTeacherSubCode);
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

                    int N_TeacherSubMasterID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TeacherSubMasterID"].ToString());
                    Params.Add("@p3", N_TeacherSubMasterID);

                    string DetailSql = "select * from vw_Sch_TeacherSubjects where N_CompanyID=@p1 and N_TeacherSubMasterID=@p3";

                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);
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
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nTeacherSubMasterID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TeacherSubMasterID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["x_TeacherSubCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Sch_TeacherSubjectMaster", "x_TeacherSubCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Teacher Subject Code")); }
                        MasterTable.Rows[0]["x_TeacherSubCode"] = Code;
                    }
                    MasterTable.Columns.Remove("n_FnYearId");

                    if (nTeacherSubMasterID > 0) 
                    {  
                        dLayer.DeleteData("Sch_TeacherSubjectMaster", "n_TeacherSubMasterID", nTeacherSubMasterID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    }

                    nTeacherSubMasterID = dLayer.SaveData("Sch_TeacherSubjectMaster", "n_TeacherSubMasterID", MasterTable, connection, transaction);
                    if (nTeacherSubMasterID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_TeacherSubMasterID"] = nTeacherSubMasterID;
                    }
                    int nTeacherSubID = dLayer.SaveData("Sch_TeacherSubjects", "N_TeacherSubID", DetailTable, connection, transaction);
                    if (nTeacherSubID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save ");
                    }
                    transaction.Commit();
                    return Ok(api.Success("Teacher Subject Created"));
                   
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        // [HttpGet("list") ]
        // public ActionResult SubjectList(int nCompanyID)
        // {    
        //     SortedList param = new SortedList();           
        //     DataTable dt=new DataTable();
            
        //     string sqlCommandText="";

        //     sqlCommandText="select * from Sch_Subject where N_CompanyID=@p1";

        //     param.Add("@p1", nCompanyID);                 
                
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();

        //             dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
        //         }
        //         if(dt.Rows.Count==0)
        //         {
        //             return Ok(api.Notice("No Results Found"));
        //         }
        //         else
        //         {
        //             return Ok(api.Success(dt));
        //         }
                
        //     }
        //     catch(Exception e)
        //     {
        //         return Ok(api.Error(User,e));
        //     }   
        // }   
      
        // [HttpDelete("delete")]
        // public ActionResult DeleteData(int nSubjectID)
        // {

        //     int Results = 0;
        //     int nCompanyID=myFunctions.GetCompanyID(User);
        //     try
        //     {                        
        //         SortedList Params = new SortedList();
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();
        //             Results = dLayer.DeleteData("Sch_Subject ", "N_SubjectID", nSubjectID, "N_CompanyID =" + nCompanyID, connection, transaction);
                
        //             if (Results > 0)
        //             {
        //                 transaction.Commit();
        //                 return Ok(api.Success("Subject deleted"));
        //             }
        //             else
        //             {
        //                 return Ok(api.Error(User,"Unable to delete Subject"));
        //             }
        //         }

        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(api.Error(User,ex));
        //     }



        // }
    }
}

