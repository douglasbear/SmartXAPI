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
    [Route("schTeacherDashboard")]
    [ApiController]
    public class SchTeacherDashboard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public SchTeacherDashboard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("details")]
        public ActionResult GetDashboardDetails(int nAcYearID,int nBranchID,bool bAllBranchData,int nTeacherID,DateTime dDate)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            string crieteria = "";
              DayOfWeek dayOfWk = dDate.DayOfWeek;
           
                     
         
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //   Params.Add("@today", dDate);
                      
            object nBatchID = dLayer.ExecuteScalar("select n_AdmittedDivisionID from vw_schAdmission where N_AdmissionID= "+nTeacherID+" and  N_CompanyID = " + nCompanyID + " and N_AcYearID="+nAcYearID,Params, connection) ;
            string sqlAssignment = "SELECT count(1) as N_Count FROM Vw_AssignmentByTeacher WHERE MONTH(D_AssignedDate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_AssignedDate) = YEAR(CURRENT_TIMESTAMP) and isnull(B_IsSaveDraft,0)=0 and  N_CompanyID = " + nCompanyID + " and N_AcYearID="+nAcYearID+" and n_TeacherID="+nTeacherID+" " ;
            string sqlAssignmentTotal = "SELECT count(1) as N_Count FROM Vw_AssignmentByTeacher WHERE N_CompanyID = " + nCompanyID + " and N_AcYearID="+nAcYearID+" and n_TeacherID="+nTeacherID+"" ;
            string sqlExam = "SELECT count(1) as N_Count FROM Vw_ExamByTeacher WHERE N_CompanyID = " + nCompanyID + " and  N_AcYearID="+nAcYearID  +" and n_TeacherID="+nTeacherID+""; 
            string sqlPubResults = "SELECT count(1) as N_Count FROM vw_Sch_Assignment WHERE N_FormID=1547 and isnull(b_PublishMark,0)=1 and  N_CompanyID = " + nCompanyID + " and  N_AcYearID="+nAcYearID  + crieteria ;
            string sqlSheduledExam = "SELECT count(1) as N_Count FROM Vw_ExamByTeacher WHERE MONTH(D_ExamDate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_ExamDate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyID + " and  N_AcYearID="+nAcYearID  +" and n_TeacherID="+nTeacherID; 
             string sqlTimeTableData = "SELECT * FROM vw_TimetableDetails WHERE N_CompanyID = " + nCompanyID + " and  N_FnYearID="+nAcYearID+"  and  x_WeekName='"+dayOfWk+"' and n_TeacherID="+nTeacherID+"" ;
           
            SortedList Data = new SortedList();
            DataTable Assignment = new DataTable();
            DataTable AssignmentTotal = new DataTable();
            DataTable Exam = new DataTable();
            DataTable ExamResult = new DataTable();
            DataTable ScheduleExam = new DataTable();
            DataTable TimeTable = new DataTable();
            
                     //bool B_customer = myFunctions.CheckPermission(nCompanyID, 1302, "Administrator", "X_UserCategory", dLayer, connection);
                    Assignment = dLayer.ExecuteDataTable(sqlAssignment, Params, connection);
                    AssignmentTotal = dLayer.ExecuteDataTable(sqlAssignmentTotal, Params, connection);
                    Exam = dLayer.ExecuteDataTable(sqlExam, Params, connection);
                    ExamResult = dLayer.ExecuteDataTable(sqlPubResults, Params, connection);
                    ScheduleExam = dLayer.ExecuteDataTable(sqlSheduledExam, Params, connection);
                    TimeTable = dLayer.ExecuteDataTable(sqlTimeTableData, Params, connection);


                   Assignment.AcceptChanges();
                AssignmentTotal.AcceptChanges();
                Exam.AcceptChanges();
                ExamResult.AcceptChanges();
                ScheduleExam.AcceptChanges();
                TimeTable.AcceptChanges();
            
                if (Assignment.Rows.Count > 0) Data.Add("assignment", Assignment);
                if (AssignmentTotal.Rows.Count > 0) Data.Add("assignmentTotal", AssignmentTotal);
                if (Exam.Rows.Count > 0) Data.Add("exam", Exam);
                if (ExamResult.Rows.Count > 0) Data.Add("examResult", ExamResult);
                if (ScheduleExam.Rows.Count > 0) Data.Add("ScheduleExam", ScheduleExam);
                if (TimeTable.Rows.Count > 0) Data.Add("TimeTable", TimeTable);
           
                return Ok(api.Success(Data));  
                }

                
               

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("timeTable")]
        public ActionResult GetTimeTableDetails(int nAcYearID,int nBranchID,bool bAllBranchData,int nTeacherID,DateTime dDate)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            string crieteria = "";
            DayOfWeek dayOfWk = dDate.DayOfWeek;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlTimeTableData = "SELECT * FROM vw_TimetableDetails WHERE N_CompanyID = " + nCompanyID + " and  N_FnYearID="+nAcYearID+"  and  x_WeekName='"+dayOfWk+"' and n_TeacherID="+nTeacherID+"" ;
                    SortedList Data = new SortedList();
                    DataTable TimeTable = new DataTable();
                    TimeTable = dLayer.ExecuteDataTable(sqlTimeTableData, Params, connection);
                    TimeTable.AcceptChanges();
                    if (TimeTable.Rows.Count > 0) Data.Add("TimeTable", TimeTable);
                    return Ok(api.Success(Data));  
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
        [HttpGet("assignmentDetails")]
        public ActionResult GetAssignmentDetails(int nAcYearID,int nBranchID,bool bAllBranchData,int nTeacherID, int nPage, int nSizeperpage)

        {
             SortedList Params = new SortedList();
         
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            SortedList Data = new SortedList();
            string crieteria = "";
            DataTable AssignmentDetails = new DataTable();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlAssignment = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (Count == 0)
                    {
                        sqlAssignment = "select top(" + nSizeperpage + ") * from Vw_AssignmentByTeacher WHERE MONTH(D_AssignedDate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_AssignedDate) = YEAR(CURRENT_TIMESTAMP) and isnull(B_IsSaveDraft,0)=0 and  N_CompanyID = " + nCompanyID + " and N_AcYearID="+nAcYearID+" and N_TeacherID="+nTeacherID+"";
                    }
                    else
                    {
                        sqlAssignment = "select top(" + nSizeperpage + ") * from Vw_AssignmentByTeacher WHERE MONTH(D_AssignedDate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_AssignedDate) = YEAR(CURRENT_TIMESTAMP) and isnull(B_IsSaveDraft,0)=0 and  N_CompanyID = " + nCompanyID + " and N_AcYearID="+nAcYearID+" and N_TeacherID="+nTeacherID+"  and N_AssignmentID not in (select top(" + Count + ") N_AssignmentID from vw_Sch_Assignment WHERE MONTH(D_AssignedDate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_AssignedDate) = YEAR(CURRENT_TIMESTAMP) and isnull(B_IsSaveDraft,0)=0 and  N_CompanyID = " + nCompanyID + " and N_AcYearID="+nAcYearID+" and N_TeacherID="+nTeacherID+"";
                    }

                    AssignmentDetails = dLayer.ExecuteDataTable(sqlAssignment, Params, connection);
                    AssignmentDetails = api.Format(AssignmentDetails, "AssignmentDetails");
                    if (AssignmentDetails.Rows.Count > 0) Data.Add("AssignmentDetails", AssignmentDetails);
                    string sqlCommandCount1 = "select count(1) as N_Count from Vw_AssignmentByTeacher WHERE MONTH(D_AssignedDate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_AssignedDate) = YEAR(CURRENT_TIMESTAMP) and isnull(B_IsSaveDraft,0)=0 and  N_CompanyID = " + nCompanyID + " and N_AcYearID="+nAcYearID+" and N_TeacherID="+nTeacherID+" ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount1, Params, connection);
                    Data.Add("TotalCount", TotalCount);
                     return Ok(api.Success(Data));
                    //  if (AssignmentDetails.Rows.Count == 0)
                    // { 
                    //     return Ok(api.Warning("No Results Found"));
                    // }
                    // else
                    // {
                    //     return Ok(api.Success(Data));
                    // }

                   
                    

                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

            [HttpGet("classAndBatch")]
        public ActionResult GetClassAndBatch(int nAcYearID,int nBranchID,bool bAllBranchData,int nTeacherID, int nPage, int nSizeperpage)

        {
             SortedList Params = new SortedList();
         
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            SortedList Data = new SortedList();
            string crieteria = "";
            DataTable BatchwiseDetails = new DataTable();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlAssignment = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (Count == 0)
                    {
                        sqlAssignment = "select top(" + nSizeperpage + ") * from Vw_CLassWiseStudents WHERE   N_CompanyID = " + nCompanyID + " and N_FnYearID="+nAcYearID+" and N_TeacherID="+nTeacherID+"";
                    }
                    else
                    {
                        sqlAssignment ="select top(" + nSizeperpage + ") * from Vw_CLassWiseStudents WHERE   N_CompanyID = " + nCompanyID + " and N_FnYearID="+nAcYearID+" and N_TeacherID="+nTeacherID+"";
                    }

                    BatchwiseDetails = dLayer.ExecuteDataTable(sqlAssignment, Params, connection);
                    BatchwiseDetails = api.Format(BatchwiseDetails, "BatchwiseDetails");
                    if (BatchwiseDetails.Rows.Count > 0) Data.Add("BatchwiseDetails", BatchwiseDetails);
                    string sqlCommandCount1 ="select count(1) as N_Count from Vw_ClassandBatchWiseList WHERE   N_CompanyID = " + nCompanyID + " and N_FnYearID="+nAcYearID+" and N_TeacherID="+nTeacherID+"";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount1, Params, connection);
                    Data.Add("TotalCount", TotalCount);
                     if (BatchwiseDetails.Rows.Count == 0)
                    { 
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(Data));
                    }

                   
                    

                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }







 
   [HttpGet("subjectList")]
        public ActionResult AssignmentList(int nAcYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData,int nTeacherID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
        
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            string crieteria = "";
            if (bAllBranchData == true)
            {
            crieteria="";
           
            }
            else
            {
            crieteria="";
          
            }
            int nCompanyId = myFunctions.GetCompanyID(User);
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and (N_SubMappingID like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_SubMappingID desc";
            else
                xSortBy = " order by " + xSortBy;
 
            if (Count == 0)
                sqlCommandText = "select top(10) * from Vw_SubjectMapping where N_UserID=" + nTeacherID + " and N_CompanyID = " + nCompanyId + " and N_AcYearID="+nAcYearID + crieteria + Searchkey + " " + xSortBy ;
            else
                sqlCommandText = "select top(10) * from Vw_SubjectMapping where  N_UserID=" + nTeacherID + " and  N_CompanyID = " + nCompanyId + " and N_AcYearID="+nAcYearID + crieteria + "  " + Searchkey + " and N_SubMappingID not in (select top(" + Count + ") N_SubMappingID from Vw_SubjectMapping where N_CompanyID=@p1 " + crieteria + xSortBy + " )" + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "Select  count(1) from Vw_SubjectMapping Where N_CompanyID = " + nCompanyId + " and N_AcYearID="+nAcYearID + crieteria ;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Success(OutPut));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
    }
}