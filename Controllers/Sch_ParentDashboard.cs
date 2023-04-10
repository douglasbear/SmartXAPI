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
    [Route("schParentDashboard")]
    [ApiController]
    public class SchParentDashboard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public SchParentDashboard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("details")]
        public ActionResult GetDashboardDetails(int nAcYearID,int nBranchID,bool bAllBranchData,int StudentID)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            string crieteria = "";
            // if(StudentID>0) 
            //         {
            //   crieteria = " and N_StudentID="+nStudentID+" "
            //         }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

            object nBatchID = dLayer.ExecuteScalar("select n_AdmittedDivisionID from vw_schAdmission where N_AdmissionID= "+StudentID+" and  N_CompanyID = " + nCompanyID + " and N_AcYearID="+nAcYearID,Params, connection) ;
            string sqlStudentInfo = "select X_Name,x_Class,x_ClassDivision,x_EmpName,x_Phone1 from vw_schAdmission where N_AdmissionID= "+StudentID+" and  N_CompanyID = " + nCompanyID + " and N_AcYearID="+nAcYearID ;
            string sqlAssignment = "SELECT count(1) as N_Count FROM vw_Sch_AssignmentDetails WHERE N_StudentID= "+StudentID+" and MONTH(D_AssignedDate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_AssignedDate) = YEAR(CURRENT_TIMESTAMP) and isnull(B_IsSaveDraft,0)=0 and N_FormID=1485 and  N_CompanyID = " + nCompanyID + " and N_AcYearID="+nAcYearID  ;
            string sqlAssignmentTotal = "SELECT count(1) as N_Count FROM vw_Sch_AssignmentDetails WHERE N_StudentID= "+StudentID+" and N_FormID=1485 and N_CompanyID = " + nCompanyID + " and N_AcYearID="+nAcYearID  ;
            string sqlExam = "SELECT count(1) as N_Count FROM vw_Sch_AssignmentDetails WHERE N_StudentID= "+StudentID+" and N_FormID=1547 and isnull(B_IsSaveDraft,0)=0 and  N_CompanyID = " + nCompanyID + " and  N_AcYearID="+nAcYearID  + crieteria ;
            string sqlPubResults = "SELECT count(1) as N_Count FROM vw_Sch_AssignmentDetails WHERE N_StudentID= "+StudentID+" and N_FormID=1547 and isnull(b_PublishMark,0)=1 and  N_CompanyID = " + nCompanyID + " and  N_AcYearID="+nAcYearID  + crieteria ;
           
           //string sqlAbsent = "SELECT count(1) as N_Count FROM vw_SchAdmission WHERE x_Gender='FeMale' and N_CompanyID = " + nCompanyID + " and  N_AcYearID="+nAcYearID  + crieteria ;
           
            SortedList Data = new SortedList();
            DataTable Assignment = new DataTable();
            DataTable AssignmentTotal = new DataTable();
            DataTable Exam = new DataTable();
            DataTable ExamResult = new DataTable();
            DataTable StudentInfo = new DataTable();
            
                     //bool B_customer = myFunctions.CheckPermission(nCompanyID, 1302, "Administrator", "X_UserCategory", dLayer, connection);
                    Assignment = dLayer.ExecuteDataTable(sqlAssignment, Params, connection);
                    AssignmentTotal = dLayer.ExecuteDataTable(sqlAssignmentTotal, Params, connection);
                    Exam = dLayer.ExecuteDataTable(sqlExam, Params, connection);
                    ExamResult = dLayer.ExecuteDataTable(sqlPubResults, Params, connection);
                    StudentInfo = dLayer.ExecuteDataTable(sqlStudentInfo, Params, connection);

                   Assignment.AcceptChanges();
                AssignmentTotal.AcceptChanges();
                Exam.AcceptChanges();
                ExamResult.AcceptChanges();
                StudentInfo.AcceptChanges();
                if (Assignment.Rows.Count > 0) Data.Add("assignment", Assignment);
                if (AssignmentTotal.Rows.Count > 0) Data.Add("assignmentTotal", AssignmentTotal);
                if (Exam.Rows.Count > 0) Data.Add("exam", Exam);
                if (ExamResult.Rows.Count > 0) Data.Add("examResult", ExamResult);
                if (StudentInfo.Rows.Count > 0) Data.Add("studentInfo", StudentInfo);
                return Ok(api.Success(Data));  
                }

                
               

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
      [HttpGet("assignmentList")]
        public ActionResult AssignmentList(int nAcYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData,int nStudentID)
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
                Searchkey = " and (X_OrderNo like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_AssignmentID desc";
            else
                xSortBy = " order by " + xSortBy;
 
            if (Count == 0)
                sqlCommandText = "select top(10) * from vw_Sch_AssignmentStudents where  N_FormID=1485 and isnull(B_IsSaveDraft,0)=0 and N_StudentID=" + nStudentID + " and N_CompanyID = " + nCompanyId + " and N_AcYearID="+nAcYearID + crieteria + Searchkey + " " + xSortBy ;
            else
                sqlCommandText = "select top(10) * from vw_Sch_AssignmentStudents where N_FormID=1485 and  isnull(B_IsSaveDraft,0)=0 and  N_StudentID=" + nStudentID + " and N_CompanyID = " + nCompanyId + " and N_AcYearID="+nAcYearID + crieteria + "  " + Searchkey + " and N_AssignmentID not in (select top(" + Count + ") N_AssignmentID from vw_Sch_AssignmentDetails where N_CompanyID=@p1 " + crieteria + xSortBy + " )" + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "Select  count(1) from vw_Sch_AssignmentStudents Where N_FormID=1485 and isnull(B_IsSaveDraft,0)=0 and N_CompanyID = " + nCompanyId + " and N_AcYearID="+nAcYearID + crieteria ;
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
  [HttpGet("examList")]
        public ActionResult ExamList(int nAcYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData,int nStudentID)
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
            crieteria=" and N_BranchID="+nBranchID;
          
            }
            int nCompanyId = myFunctions.GetCompanyID(User);
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and (X_OrderNo like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_AssignmentID desc";
            else
                xSortBy = " order by " + xSortBy;
 
            if (Count == 0)
                sqlCommandText = "select top(10) * from vw_Sch_AssignmentStudents where  YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_FormID=1547 and isnull(B_IsSaveDraft,0)=0 and N_StudentID=" + nStudentID + " and N_CompanyID = " + nCompanyId + crieteria + Searchkey + " " + xSortBy ;
            else
                sqlCommandText = "select top(10) * from vw_Sch_AssignmentStudents where YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_FormID=1547 and isnull(B_IsSaveDraft,0)=0 and N_StudentID=" + nStudentID + " and N_CompanyID = " + nCompanyId + crieteria + "  " + Searchkey + " and N_ExamTimeMasterID not in (select top(" + Count + ") N_ExamTimeMasterID from vw_Sch_AssignmentStudents where N_CompanyID=@p1 " + crieteria + xSortBy + " )" + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "Select  count(1) from vw_Sch_AssignmentStudents Where YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_StudentID=" + nStudentID + " and N_CompanyID = " + nCompanyId  + crieteria ;
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
  [HttpGet("feeList")]
        public ActionResult ExamList(int nFnYearID,int nAdmissionID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData)
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
            crieteria=" and N_BranchID="+nBranchID;
          
            }
            int nCompanyId = myFunctions.GetCompanyID(User);
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and (X_OrderNo like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_RefID desc";
            else
                xSortBy = " order by " + xSortBy;
 
            if (Count == 0)
             sqlCommandText = "select top(10) * from vw_Sch_AdmissionFee where  YEAR(D_DateFrom) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + " and N_FnYearID="+nFnYearID + crieteria + Searchkey + " " + xSortBy ;
          
                //sqlCommandText = "select top(10) * from vw_Sch_AdmissionFee where N_RefID="+nAdmissionID+" and B_IsRemoved=0  and YEAR(D_DateFrom) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + " and N_FnYearID="+nFnYearID + crieteria + Searchkey + " " + xSortBy ;
            else
                sqlCommandText = "select top(10) * from vw_Sch_AdmissionFee where N_RefID="+nAdmissionID+" and B_IsRemoved=0  and YEAR(D_DateFrom) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + " and N_FnYearID="+nFnYearID + crieteria + "  " + Searchkey + " and N_RefID not in (select top(" + Count + ") N_RefID from vw_Sch_AdmissionFee where N_CompanyID=@p1 " + crieteria + xSortBy + " )" + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "Select  count(1) from vw_Sch_AdmissionFee Where N_RefID="+nAdmissionID+" and B_IsRemoved=0 and YEAR(D_DateFrom) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + " and N_FnYearID="+nFnYearID + crieteria ;
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

        //  [HttpGet("timeTableList")]
        // public ActionResult TimetableList(int nAcYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData,int nStudentID)
        // {
        //     DataTable dt = new DataTable();
        //     SortedList Params = new SortedList();
        
        //     string sqlCommandCount = "";
        //     int Count = (nPage - 1) * nSizeperpage;
        //     string sqlCommandText = "";
        //     string Searchkey = "";
        //     string crieteria = "";
        //     if (bAllBranchData == true)
        //     {
        //     crieteria="";
           
        //     }
        //     else
        //     {
        //     crieteria=" and N_BranchID="+nBranchID;
          
        //     }
        //     int nCompanyId = myFunctions.GetCompanyID(User);
        //     if (xSearchkey != null && xSearchkey.Trim() != "")
        //         Searchkey = " and (X_OrderNo like '%" + xSearchkey + "%')";

        //     if (xSortBy == null || xSortBy.Trim() == "")
        //         xSortBy = " order by N_AssignmentID desc";
        //     else
        //         xSortBy = " order by " + xSortBy;
           
        //     if (Count == 0)
        //         sqlCommandText = "select top(10) * from vw_TimetableDisplay where  N_ClassID=" + N_ClassID + " and N_CompanyID = " + nCompanyId + crieteria + Searchkey + " " + xSortBy ;
        //     else
        //         sqlCommandText = "select top(10) * from vw_TimetableDisplay where N_ClassID=" + N_ClassID + " and N_CompanyID = " + nCompanyId + crieteria + "  " + Searchkey + " and N_ExamTimeMasterID not in (select top(" + Count + ") N_ExamTimeMasterID from vw_Sch_AssignmentStudents where N_CompanyID=@p1 " + crieteria + xSortBy + " )" + xSortBy;
        //     Params.Add("@p1", nCompanyId);

        //     SortedList OutPut = new SortedList();


        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

        //             sqlCommandCount = "Select  count(1) from vw_TimetableDisplay Where YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_StudentID=" + nStudentID + " and N_CompanyID = " + nCompanyId  + crieteria ;
        //             object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
        //             OutPut.Add("Details", api.Format(dt));
        //             OutPut.Add("TotalCount", TotalCount);
        //             if (dt.Rows.Count == 0)
        //             {
        //                 return Ok(api.Success(OutPut));
        //             }
        //             else
        //             {
        //                 return Ok(api.Success(OutPut));
        //             }

        //         }

        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(api.Error(User,e));
        //     }
        // }

    }
}