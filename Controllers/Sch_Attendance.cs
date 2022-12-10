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
    [Route("schAttendance")]
    [ApiController]
    public class Sch_Attendance : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly IMyAttachments myAttachments;
        private readonly int N_FormID =1566 ;


        public Sch_Attendance(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf,IMyAttachments myAtt)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

      

        [HttpGet("details")]
        public ActionResult BusRegDetails(string xAssignmentCode,int nFnYearID)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_Sch_Attendance where N_CompanyID=@p1  and X_AttendanceCode=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", xAssignmentCode);
             Params.Add("@nFnYearID", nFnYearID);
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

                    int n_AttendanceID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_AttendanceID"].ToString());
                    Params.Add("@p3", n_AttendanceID);

                    string DetailSql = "select * from vw_Sch_AttendanceDetails where N_CompanyID=@p1 and n_AttendanceID=@p3";

                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);

                     DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(MasterTable.Rows[0]["n_AttendanceID"].ToString()), 0, myFunctions.getIntVAL(MasterTable.Rows[0]["N_FormID"].ToString()), nFnYearID, User, connection);
                     Attachments = api.Format(Attachments, "attachments");
                     dt.Tables.Add(Attachments);

                }
                return Ok(api.Success(dt));               
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
      
        [HttpGet("detailList") ]
        public ActionResult AdmissionList(int nCompanyID,int nSubjectID,int nBatchID, DateTime dDate)
        {    
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="";

          

            param.Add("@N_CompanyID", nCompanyID);             
            param.Add("@N_SubjectID", nSubjectID);             
            param.Add("@N_BatchID", nBatchID);             
            param.Add("@D_Date", dDate);             
                
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTablePro("SP_Attendance",param,connection);
                }
                if(dt.Rows.Count==0)
                {
                   return Ok(api.Success(dt));
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
                int nAttendanceID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_AttendanceID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_AttendanceCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Sch_Attendance", "X_AttendanceCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Assignment Code")); }
                        MasterTable.Rows[0]["X_AttendanceCode"] = Code;
                    }

                    if (nAttendanceID > 0) 
                    {  
                        dLayer.DeleteData("Sch_AttendanceDetails", "n_AttendanceID", nAttendanceID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                        dLayer.DeleteData("Sch_Attendance", "n_AttendanceID", nAttendanceID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    }
                    MasterTable.Columns.Remove("n_FnYearId");
                    nAttendanceID = dLayer.SaveData("Sch_Attendance", "n_AttendanceID", MasterTable, connection, transaction);
                    if (nAttendanceID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_AttendanceID"] = nAttendanceID;
                    }
                    int nAssignStudentID = dLayer.SaveData("Sch_AttendanceDetails", "N_AttDetailsID", DetailTable, connection, transaction);
                    if (nAssignStudentID <= 0)
                    {
                      
                              
                        transaction.Rollback();
                        return Ok("Unable to save ");
                               
                    }
                
                    transaction.Commit();
                    return Ok(api.Success("Attendance Created"));

                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

       
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nAttendanceID)
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
                    Results = dLayer.DeleteData("Sch_Attendance", "n_AttendanceID", nAttendanceID, "N_CompanyID =" + nCompanyID, connection, transaction);                   
                
                    if (Results > 0)
                    {
                        dLayer.DeleteData("Sch_AttendanceDetails", "n_AttendanceID", nAttendanceID, "N_CompanyID =" + nCompanyID, connection, transaction); 

                        transaction.Commit();
                        return Ok(api.Success("Assignment deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User,"Unable to delete Assignment"));
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

