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
    [Route("schAssignment")]
    [ApiController]
    public class Sch_Assignment : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =1485 ;


        public Sch_Assignment(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("dashboardList")]
        public ActionResult GetAssignmentList(int? nCompanyId, int nAcYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_AssignmentCode like '%" + xSearchkey + "%' or X_Title like '%" + xSearchkey + "%' or X_Subject like '%" + xSearchkey + "%' or X_ClassDivision like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_AssignmentCode desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_AssignmentCode":
                        xSortBy = "X_AssignmentCode " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Sch_Assignment where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID  " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Sch_Assignment where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + Searchkey + " and N_AssignmentID not in (select top(" + Count + ") N_AssignmentID from vw_Sch_Assignment where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + xSortBy + " ) " + " " + xSortBy;

            Params.Add("@nCompanyId", nCompanyID);
            Params.Add("@nAcYearID", nAcYearID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(*) as N_Count  from vw_Sch_Assignment where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + Searchkey + "";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("details")]
        public ActionResult BusRegDetails(string xAssignmentCode)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_Sch_Assignment where N_CompanyID=@p1  and x_AssignmentCode=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", xAssignmentCode);
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

                    int N_AssignmentID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AssignmentID"].ToString());
                    Params.Add("@p3", N_AssignmentID);

                    string DetailSql = "select * from vw_Sch_AssignmentStudents where N_CompanyID=@p1 and N_AssignmentID=@p3";

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
                int nAssignmentID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AssignmentID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_AssignmentCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Sch_Assignment", "X_AssignmentCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Assignment Code")); }
                        MasterTable.Rows[0]["X_AssignmentCode"] = Code;
                    }

                    if (nAssignmentID > 0) 
                    {  
                        dLayer.DeleteData("Sch_AssignmentStudents", "n_AssignmentID", nAssignmentID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                        dLayer.DeleteData("Sch_Assignment", "n_AssignmentID", nAssignmentID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    }
                    MasterTable.Columns.Remove("n_FnYearId");
                    nAssignmentID = dLayer.SaveData("Sch_Assignment", "n_AssignmentID", MasterTable, connection, transaction);
                    if (nAssignmentID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_AssignmentID"] = nAssignmentID;
                    }
                    int nAssignStudentID = dLayer.SaveData("Sch_AssignmentStudents", "N_AssignStudentID", DetailTable, connection, transaction);
                    if (nAssignStudentID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save ");
                    }
                    transaction.Commit();
                    return Ok(api.Success("Assignment Created"));

                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        [HttpGet("list") ]
        public ActionResult AssignmentList(int nCompanyID,int nSubjectID,int nBatchID)
        {    
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="";

            if(nSubjectID!=0) 
            { 
                if(nBatchID!=0)
                    sqlCommandText="select * from vw_Sch_Assignment where N_CompanyID=@p1 and n_SubjectID=@p2 and n_BatchID=@p3";
                else
                    sqlCommandText="select * from vw_Sch_Assignment where N_CompanyID=@p1 and n_SubjectID=@p2";
            }
            else
            {
                if(nBatchID!=0)
                    sqlCommandText="select * from vw_Sch_Assignment where N_CompanyID=@p1 and n_BatchID=@p3";
                else
                    sqlCommandText="select * from vw_Sch_Assignment where N_CompanyID=@p1";
            }

            param.Add("@p1", nCompanyID);             
            param.Add("@p2", nSubjectID);             
            param.Add("@p3", nBatchID);             
                
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
        public ActionResult DeleteData(int nAssignmentID)
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
                    Results = dLayer.DeleteData("Sch_Assignment", "n_AssignmentID", nAssignmentID, "N_CompanyID =" + nCompanyID, connection, transaction);                   
                
                    if (Results > 0)
                    {
                        dLayer.DeleteData("Sch_AssignmentStudents", "n_AssignmentID", nAssignmentID, "N_CompanyID =" + nCompanyID, connection, transaction); 

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

