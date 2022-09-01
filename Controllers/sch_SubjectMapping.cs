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
    [Route("schSubjectMapping")]
    [ApiController]
    public class sch_subjectMapping : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =1556 ;


        public sch_subjectMapping(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = 
            conf.GetConnectionString("SmartxConnection");
        }



           [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nSubMappingID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_SubMappingID"].ToString());
                int nCourseID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CourseID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_SubMappingCode"].ToString();

                      if (nSubMappingID > 0)
                    {

                        dLayer.ExecuteNonQuery("delete from sch_subjectMapping Where N_CompanyID = "+nCompanyID+" and N_CourseID = "+nCourseID+"", connection, transaction);
                        dLayer.ExecuteNonQuery("delete from Sch_SubjectMappingDetails Where N_CompanyID = "+nCompanyID+"  and N_CourseID ="+nCourseID+"", connection, transaction);

                    }

                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("sch_subjectMapping", "X_SubMappingCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Route Code")); }
                        MasterTable.Rows[0]["X_SubMappingCode"] = Code;
                    }
                    MasterTable.Columns.Remove("n_FnYearID");

                    // if (nSubMappingID > 0) 
                    // {  
                    //     dLayer.DeleteData("sch_subjectMapping", "N_SubMappingID", nSubMappingID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    //     dLayer.DeleteData("Sch_SubjectMappingDetails", "N_SubMappingID", nSubMappingID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    // }

                    nSubMappingID = dLayer.SaveData("sch_subjectMapping", "N_SubMappingID", MasterTable, connection, transaction);
                    if (nSubMappingID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_SubMappingID"] = nSubMappingID;
                        DetailTable.Rows[j]["X_SubMappingCode"] = Code;
                       
                       
                    }
                    int nRouteDetailID = dLayer.SaveData("Sch_SubjectMappingDetails", "N_SubMappingDetailsID", DetailTable, connection, transaction);
                    if (nRouteDetailID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save ");
                    }
                    transaction.Commit();
                    return Ok(api.Success("Subject Created"));

                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }




          [HttpGet("dashboardList")]
        public ActionResult GetAssignmentList(int? nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
            
                Searchkey = "and (N_SubMappingID like '%" + xSearchkey + "%' OR X_SubMappingCode like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_SubMappingID desc";
                
       
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_sch_SubjectMapping where N_CompanyID=@nCompanyID " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_sch_SubjectMapping where N_CompanyID=@nCompanyID " + Searchkey + " " + xSortBy;

            Params.Add("@nCompanyID", nCompanyID);
           

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(*) as N_Count  from vw_sch_SubjectMapping where N_CompanyID=@nCompanyID " + Searchkey + " ";
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
        public ActionResult BusRegDetails(string nCourseID)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_sch_SubjectMapping where N_CompanyID=@p1 and N_CourseID=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", nCourseID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    if (MasterTable.Rows.Count == 0)
                    {
                       // return Ok(api.Success(OutPut));
                    }
                
                    MasterTable = api.Format(MasterTable, "Master");
                    string DetailSql = "select * from Sch_SubjectMappingDetails where N_CompanyID=@p1 and N_CourseID=@p2";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = api.Format(DetailTable, "Details");

                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(api.Success(dt));
                }           
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
        
    }
}