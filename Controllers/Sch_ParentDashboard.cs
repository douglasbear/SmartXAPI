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
           
                     
         
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
            object nBatchID = dLayer.ExecuteScalar("select n_AdmittedDivisionID from vw_schAdmission where N_AdmissionID= "+StudentID+" and  N_CompanyID = " + nCompanyID + " and N_AcYearID="+nAcYearID,Params, connection) ;
            string sqlAssignment = "SELECT COUNT(*) as N_Count FROM vw_Sch_Assignment WHERE MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and  N_CompanyID = " + nCompanyID + " and N_AcYearID="+nAcYearID  ;
            string sqlAssignmentTotal = "SELECT COUNT(*) as N_Count FROM vw_Sch_Assignment WHERE N_CompanyID = " + nCompanyID + " and N_AcYearID="+nAcYearID  ;
            string sqlExam = "SELECT COUNT(*) as N_Count FROM vw_Sch_ExamTimeMaster WHERE N_BatchID= "+nBatchID+" and  N_CompanyID = " + nCompanyID + " and  N_AcYearID="+nAcYearID  + crieteria ;
           //string sqlAbsent = "SELECT COUNT(*) as N_Count FROM vw_SchAdmission WHERE x_Gender='FeMale' and N_CompanyID = " + nCompanyID + " and  N_AcYearID="+nAcYearID  + crieteria ;
           
            SortedList Data = new SortedList();
            DataTable Assignment = new DataTable();
            DataTable AssignmentTotal = new DataTable();
            DataTable Exam = new DataTable();
            //DataTable Absent = new DataTable();
         
                     //bool B_customer = myFunctions.CheckPermission(nCompanyID, 1302, "Administrator", "X_UserCategory", dLayer, connection);
                    Assignment = dLayer.ExecuteDataTable(sqlAssignment, Params, connection);
                    AssignmentTotal = dLayer.ExecuteDataTable(sqlAssignmentTotal, Params, connection);
                    Exam = dLayer.ExecuteDataTable(sqlExam, Params, connection);
                    //Absent = dLayer.ExecuteDataTable(sqlAbsent, Params, connection);


                   Assignment.AcceptChanges();
                AssignmentTotal.AcceptChanges();
                Exam.AcceptChanges();
                //Absent.AcceptChanges();
            
                if (Assignment.Rows.Count > 0) Data.Add("assignment", Assignment);
                if (AssignmentTotal.Rows.Count > 0) Data.Add("assignmentTotal", AssignmentTotal);
                if (Exam.Rows.Count > 0) Data.Add("exam", Exam);
               // if (Absent.Rows.Count > 0) Data.Add("absent", Absent);
           
                return Ok(api.Success(Data));  
                }

                
               

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
      [HttpGet("assignmentList")]
        public ActionResult AssignmentList(int nAcYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData)
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
                sqlCommandText = "select top(10) * from vw_Sch_Assignment where  YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + " and N_AcYearID="+nAcYearID + crieteria + Searchkey + " " + xSortBy ;
            else
                sqlCommandText = "select top(10) * from vw_Sch_Assignment where YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + " and N_AcYearID="+nAcYearID + crieteria + "  " + Searchkey + " and N_AssignmentID not in (select top(" + Count + ") N_AssignmentID from vw_Sch_Assignment where N_CompanyID=@p1 " + crieteria + xSortBy + " )" + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "Select  count(*) from vw_Sch_Assignment Where YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + " and N_AcYearID="+nAcYearID + crieteria ;
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
        public ActionResult ExamList(int nAcYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData)
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
                xSortBy = " order by N_ExamTimeMasterID desc";
            else
                xSortBy = " order by " + xSortBy;
 
            if (Count == 0)
                sqlCommandText = "select top(10) * from vw_Sch_ExamTimeTable where  YEAR(D_ExamDate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + crieteria + Searchkey + " " + xSortBy ;
            else
                sqlCommandText = "select top(10) * from vw_Sch_ExamTimeTable where YEAR(D_ExamDate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + crieteria + "  " + Searchkey + " and N_ExamTimeMasterID not in (select top(" + Count + ") N_ExamTimeMasterID from vw_Sch_ExamTimeTable where N_CompanyID=@p1 " + crieteria + xSortBy + " )" + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "Select  count(*) from vw_Sch_ExamTimeTable Where YEAR(D_ExamDate) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId  + crieteria ;
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

                    sqlCommandCount = "Select  count(*) from vw_Sch_AdmissionFee Where N_RefID="+nAdmissionID+" and B_IsRemoved=0 and YEAR(D_DateFrom) = YEAR(CURRENT_TIMESTAMP) and N_CompanyID = " + nCompanyId + " and N_FnYearID="+nFnYearID + crieteria ;
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