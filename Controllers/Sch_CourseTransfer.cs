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
    [Route("schCourseTransfer")]
    [ApiController]
    public class Sch_CourseTransfer : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Sch_CourseTransfer(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("dashboardList")]
        public ActionResult GetDashboardList(int nAcYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, int nFormID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_TransferCode like '%" + xSearchkey + "%' or X_CourseFrom like '%" + xSearchkey + "%' or X_CourseTo like '%" + xSearchkey + "%' or cast(D_Docdate as VarChar) like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_TransferCode desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_TransferCode":
                        xSortBy = "X_TransferCode " + xSortBy.Split(" ")[1];
                        break;
                    case "d_Docdate":
                        xSortBy = "Cast(D_Docdate as DateTime ) " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_Sch_CourseTransfer where N_CompanyID=@nCompanyID and N_AcYearID=@nAcYearID and N_FormID=@nFormID  " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_Sch_CourseTransfer where N_CompanyID=@nCompanyID and N_AcYearID=@nAcYearID and N_FormID=@nFormID " + Searchkey + " and N_TransferID not in (select top(" + Count + ") N_TransferID from Vw_Sch_CourseTransfer where N_CompanyID=@nCompanyID and N_AcYearID=@nAcYearID and N_FormID=@nFormID " + xSortBy + " ) " + " " + xSortBy;

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nAcYearID", nAcYearID);
            Params.Add("@nFormID", nFormID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(1) as N_Count  from Vw_Sch_CourseTransfer where N_CompanyID=@nCompanyID and N_AcYearID=@nAcYearID and N_FormID=@nFormID " + Searchkey + "";
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

        [HttpGet("studentList") ]
        public ActionResult GetStudentList(int nFnYearID, int nClassID)
        {    
            int nCompanyID = myFunctions.GetCompanyID(User);
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="select * from vw_SchFeeReceived where N_CompanyId=@p1 and N_FnYearId=@p2 and N_ClassID=@p3 and isNull(N_Inactive,0)=0";

            param.Add("@p1", nCompanyID);        
            param.Add("@p2", nFnYearID);      
            param.Add("@p3", nClassID);   
                
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

        [HttpGet("studentCount") ]
        public ActionResult GetStudentCount(int nClassID)
        {    
            int nCompanyID = myFunctions.GetCompanyID(User);
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="select * from vw_BatchStudentCount where N_CompanyID=@p1 and N_ClassID=@p2";

            param.Add("@p1", nCompanyID);        
            param.Add("@p2", nClassID);   
                
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

        [HttpGet("details")]
        public ActionResult CourseTransferDetails(string xTransferCode)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable DataTable = new DataTable();

                    string Mastersql = "";
                    string DetailSql = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@xTransferCode", xTransferCode);
                    Mastersql = "select * from Vw_Sch_CourseTransfer where N_CompanyID=@nCompanyID and X_TransferCode=@xTransferCode  ";
                   
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(api.Warning("No data found")); }
                    int nTransferID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_TransferID"].ToString());
                    Params.Add("@nTransferID", nTransferID);

                    MasterTable = api.Format(MasterTable, "Master");
                    DetailSql = "select * from vw_Sch_CourseTransferStudents where N_CompanyID=@nCompanyID and N_TransferID=@nTransferID ";
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

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();

                    int nTransferID = myFunctions.getIntVAL(MasterRow["n_TransferID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int nAcYearID = myFunctions.getIntVAL(MasterRow["n_AcYearID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int nFormID = myFunctions.getIntVAL(MasterRow["n_FormID"].ToString());
                    int nCourseTo = myFunctions.getIntVAL(MasterRow["n_CourseTo"].ToString());
                    int n_CourseFrom = myFunctions.getIntVAL(MasterRow["n_CourseFrom"].ToString());
                    string xTransferCode = MasterRow["x_TransferCode"].ToString();
                    int nInactive = 0;
                      int N_FormID = 0;
                   if (MasterTable.Columns.Contains("N_FormID"))
                    {
                        N_FormID = myFunctions.getIntVAL(MasterRow["N_FormID"].ToString());
                    }

                    if (nCourseTo == n_CourseFrom) nInactive = 1;

                    if (xTransferCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", nFormID);
                        xTransferCode = dLayer.GetAutoNumber("Sch_CourseTransfer", "x_TransferCode", Params, connection, transaction);
                        if (xTransferCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Employee Evaluation");
                        }
                        MasterTable.Rows[0]["x_TransferCode"] = xTransferCode;
                    }
                    MasterTable.Columns.Remove("n_FnYearID");

                    nTransferID = dLayer.SaveData("Sch_CourseTransfer", "N_TransferID", "", "", MasterTable, connection, transaction);
                    if (nTransferID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Course Transfer");
                    }
                    dLayer.DeleteData("Sch_CourseTransferStudents", "N_TransferID", nTransferID, "", connection, transaction);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_TransferID"] = nTransferID;

                        int nBatchTo = myFunctions.getIntVAL(DetailTable.Rows[j]["n_BatchTo"].ToString());
                        int nStudentID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_StudentID"].ToString());
                        dLayer.ExecuteNonQuery("update Sch_Admission Set N_DivisionID="+ nBatchTo +", N_ClassID="+ nCourseTo +", N_Inactive="+ nInactive +" where N_AdmissionID="+ nStudentID +" and N_CompanyID="+ nCompanyID + "and N_AcYearID=" + nAcYearID , Params, connection, transaction);
                    }
                    int nTransferStudentID = dLayer.SaveData("Sch_CourseTransferStudents", "N_TransferStudentID", DetailTable, connection, transaction);

                    if (nTransferStudentID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Course Transfer");
                    }
                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_TransferID", nTransferID);
                    Result.Add("x_TransferCode", xTransferCode);
                    Result.Add("n_TransferStudentID", nTransferStudentID);
                     if (N_FormID == 1520)
                            {
                    return Ok(api.Success(Result, "Course Transfer Saved"));
                            }

                     else if (N_FormID == 1648) 
                        {
                     return Ok(api.Success(Result,"Transfer Certificate Saved Successfully"));
                        }
                    else {
                         return Ok(api.Success(Result, "Course Transfer saved")); }
                }

                    
                
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }
      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID, int nAcYearID, int nTransferID,int nFormID)
        {
            int Results = 0;
             
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nAcYearID", nAcYearID);
                QueryParams.Add("@nTransferID", nTransferID);

                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable StudentTable = new DataTable();
                    DataTable MasterTable = new DataTable();
 
                    StudentTable = dLayer.ExecuteDataTable("SELECT Sch_CourseTransferStudents.N_CompanyID, Sch_CourseTransferStudents.N_AcYearID, Sch_CourseTransferStudents.N_TransferID, Sch_CourseTransferStudents.N_TransferStudentID, " +
                                                            "Sch_CourseTransferStudents.N_StudentID, Sch_CourseTransferStudents.N_BatchFrom, Sch_CourseTransferStudents.N_BatchTo, Sch_Admission.N_ClassID " +
                                                            "FROM Sch_CourseTransferStudents LEFT OUTER JOIN " +
                                                            "Sch_Admission ON Sch_CourseTransferStudents.N_CompanyID = Sch_Admission.N_CompanyID AND Sch_CourseTransferStudents.N_AcYearID = Sch_Admission.N_AcYearID AND " +
                                                            "Sch_CourseTransferStudents.N_StudentID = Sch_Admission.N_AdmissionID where Sch_CourseTransferStudents.N_CompanyID="+ nCompanyID +" and Sch_CourseTransferStudents.N_AcYearID="+ nAcYearID +" and Sch_CourseTransferStudents.N_TransferID="+ nTransferID, QueryParams, connection, transaction);
                    MasterTable = dLayer.ExecuteDataTable("select * from Sch_CourseTransfer where N_CompanyID="+ nCompanyID +" and N_AcYearID="+ nAcYearID +" and N_TransferID="+ nTransferID, QueryParams, connection, transaction);
                    
                    int nClassID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CourseFrom"].ToString());

                    for (int j = 0; j < StudentTable.Rows.Count; j++)
                    {
                        int nStudentID = myFunctions.getIntVAL(StudentTable.Rows[j]["N_StudentID"].ToString());
                        int nDivisionID = myFunctions.getIntVAL(StudentTable.Rows[j]["N_BatchFrom"].ToString());
                        if (StudentTable.Rows[j]["N_ClassID"].ToString() == MasterTable.Rows[0]["N_CourseTo"].ToString())
                        {
                            dLayer.ExecuteNonQuery("update Sch_Admission Set N_DivisionID="+ nDivisionID +", N_ClassID="+ nClassID +", N_Inactive=0 where N_AdmissionID="+ nStudentID +" and N_CompanyID="+ nCompanyID + "and N_AcYearID=" + nAcYearID , QueryParams, connection, transaction);
                        }
                    };
                    transaction.Commit();

                    Results = dLayer.DeleteData("Sch_CourseTransfer", "N_TransferID", nTransferID, "", connection);

                    if (Results > 0)
                    {
                        dLayer.DeleteData("Sch_CourseTransferStudents", "N_TransferID", nTransferID, "", connection);
                          if (nFormID == 1520)
                            {
                          return Ok(api.Success( "Course Transfer deleted"));
                            }

                       else if (nFormID == 1648) 
                        {
                            return Ok(api.Success("Transfer Certificate deleted Successfully"));
                        }
                        return Ok(api.Success("Course Transfer deleted"));
                    }
                    
                    else
                    {
                        return Ok(api.Error(User,"Unable to delete"));
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

