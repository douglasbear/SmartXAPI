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
    [Route("schSubject")]
    [ApiController]
    public class Sch_Subject : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =1481 ;


        public Sch_Subject(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = 
            conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("details")]
        public ActionResult SubjectDetails(int n_SubjectID)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from Sch_Subject where N_CompanyID=@p1  and n_SubjectID=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", n_SubjectID);
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
                int nSubjectID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_SubjectID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["x_SubjectCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Sch_Subject", "x_SubjectCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Course Code")); }
                        MasterTable.Rows[0]["x_SubjectCode"] = Code;
                    }
                    MasterTable.Columns.Remove("n_FnYearId");

                    if (nSubjectID > 0) 
                    {  
                        dLayer.DeleteData("Sch_Subject", "n_SubjectID", nSubjectID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    }

                    nSubjectID = dLayer.SaveData("Sch_Subject", "n_SubjectID", MasterTable, connection, transaction);
                    if (nSubjectID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Subject Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        [HttpGet("list") ]
        public ActionResult SubjectList(int nCompanyID)
        {    
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="";

            sqlCommandText="select * from Sch_Subject where N_CompanyID=@p1";

            param.Add("@p1", nCompanyID);                 
                
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
                }
                if(dt.Rows.Count==0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
                
            }
            catch(Exception e)
            {
                return Ok(api.Error(User,e));
            }   
        }   
      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nSubjectID)
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
                    Results = dLayer.DeleteData("Sch_Subject ", "N_SubjectID", nSubjectID, "N_CompanyID =" + nCompanyID, connection, transaction);
                
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("Subject deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User,"Unable to delete Subject"));
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }



        }
    }
}

