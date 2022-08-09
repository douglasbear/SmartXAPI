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
    [Route("schStudentCategory")]
    [ApiController]
    public class Sch_StudentCategory : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =1490 ;


        public Sch_StudentCategory(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("details")]
        public ActionResult CategoryDetails(string n_StudentCatID)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_Sch_StudentCategory where N_CompanyID=@p1  and n_StudentCatID=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", n_StudentCatID);
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
                int nStudentCatID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_StudentCatID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_StudentCatCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Sch_StudentCategory", "X_StudentCatCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Course Code")); }
                        MasterTable.Rows[0]["X_StudentCatCode"] = Code;
                    }
                    MasterTable.Columns.Remove("n_FnYearId");

                    if (nStudentCatID > 0) 
                    {  
                        dLayer.DeleteData("Sch_StudentCategory", "N_StudentCatID", nStudentCatID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    }

                    nStudentCatID = dLayer.SaveData("Sch_StudentCategory", "N_StudentCatID", MasterTable, connection, transaction);
                    if (nStudentCatID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Student Category Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        [HttpGet("list") ]
        public ActionResult CategoryList(int nCompanyID)
        {    
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="";

            sqlCommandText="select * from vw_Sch_StudentCategory where N_CompanyID=@p1";

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
        public ActionResult DeleteData(int nStudentCatID,int nAcYearID)
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
                   if (nStudentCatID > 0)
                    {
                        object stuCatCount = dLayer.ExecuteScalar("select COUNT(*) From Sch_Admission where N_StudentCatID =" + nStudentCatID + " and N_CompanyID =" + nCompanyID + " and N_AcYearID=" + nAcYearID , connection, transaction);
                        stuCatCount = stuCatCount == null ? 0 : stuCatCount;
                        if (myFunctions.getIntVAL(stuCatCount.ToString()) > 0)
                            return Ok(api.Error(User, "Student Category Already In Use !!"));
                    }

                    Results = dLayer.DeleteData("Sch_StudentCategory", "N_StudentCatID", nStudentCatID, "N_CompanyID =" + nCompanyID, connection, transaction);
                    transaction.Commit();
                
                    if (Results > 0)
                    {
                        return Ok(api.Success("Student Category deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User,"Unable to delete Student Category"));
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

