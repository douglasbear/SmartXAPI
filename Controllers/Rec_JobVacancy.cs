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
    [Route("jobvacancy")]
    [ApiController]
    public class Rec_JobVacancy : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID = 962;


        public Rec_JobVacancy(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString =
            conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("details")]
        public ActionResult VacancyDetails(string x_VacancyCode)
        {
            DataSet dt = new DataSet();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_JobVacancy where N_CompanyID=@p1  and x_VacancyCode=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", x_VacancyCode);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

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
                return Ok(api.Error(User, e));
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

                DataTable Approvals;
                Approvals = ds.Tables["approval"];
                DataRow ApprovalRow = Approvals.Rows[0];

                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nVacancyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_VacancyID"].ToString());

                int N_SaveDraft = myFunctions.getIntVAL(MasterTable.Rows[0]["b_IsSaveDraft"].ToString());
                int nUserID = myFunctions.GetUserID(User);
                int N_NextApproverID=0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen 
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_VacancyCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Rec_JobVacancy", "X_VacancyCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate JobVacancy Code")); }
                        MasterTable.Rows[0]["X_VacancyCode"] = Code;
                    }

                    if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()) && nVacancyID > 0)
                    {
                        int N_PkeyID = nVacancyID;
                        string Criteria = "N_VacancyID=" + nVacancyID + " and N_CompanyID=" + nCompanyID;
                        myFunctions.UpdateApproverEntry(Approvals, "Rec_JobVacancy", Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        N_NextApproverID = myFunctions.LogApprovals(Approvals,myFunctions.getIntVAL(nFnYearId.ToString()), "Job Vacancy", N_PkeyID, MasterTable.Rows[0]["X_VacancyCode"].ToString(), 1, "", 0, "",0, User, dLayer, connection, transaction);

                        N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Rec_JobVacancy where N_VacancyID=" + N_PkeyID + " and N_CompanyID=" + nCompanyID , connection, transaction).ToString());

                        transaction.Commit();
                        return Ok(api.Success("Job vacancy Approved " + "-" + MasterTable.Rows[0]["X_VacancyCode"].ToString()));
                    }     

                    if (nVacancyID > 0)
                    {
                        dLayer.DeleteData("Rec_JobVacancy", "N_VacancyID", nVacancyID, "N_CompanyID =" + nCompanyID, connection, transaction);
                    }

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and X_VacancyCode='" + Code + "'";
                    string X_Criteria = "N_CompanyID=" + nCompanyID + "";

                    MasterTable.Rows[0]["n_UserID"] = nUserID;
                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);

                    nVacancyID = dLayer.SaveData("Rec_JobVacancy", "N_VacancyID", DupCriteria, X_Criteria, MasterTable, connection, transaction);

                    if (nVacancyID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                    else
                    {
                        N_NextApproverID = myFunctions.LogApprovals(Approvals,myFunctions.getIntVAL(nFnYearId.ToString()), "Job Vacancy", nVacancyID, MasterTable.Rows[0]["X_VacancyCode"].ToString(), 1,"",0, "",0, User, dLayer, connection, transaction);
                        N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Rec_JobVacancy where N_VacancyID=" + nVacancyID + " and N_CompanyID=" + nCompanyID , connection, transaction).ToString());

                        transaction.Commit();
                        return Ok(api.Success("Vacancy Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }

        [HttpGet("list")]
        public ActionResult VacancyList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int nCompanyId = myFunctions.GetCompanyID(User);
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_VacancyCode like'%" + xSearchkey + "%'or X_PostingTitle like'%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_VacancyID desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_JobVacancy where N_CompanyID=@p1 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_JobVacancy where N_CompanyID=@p1 " + Searchkey + " and N_VacancyID not in (select top(" + Count + ") N_VacancyID from vw_JobVacancy where N_CompanyID=@p1 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(1) as N_Count  from vw_JobVacancy where N_CompanyID=@p1";
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

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nVacancyID,int nFnyearID,string comments)
        {

            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList(); 
                    ParamList.Add("@nTransID", nVacancyID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    string xButtonAction = "Delete";
                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,X_VacancyCode,N_VacancyID from Rec_JobVacancy where N_CompanyId=@nCompanyID and N_VacancyID=@nTransID";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, this.N_FormID, nVacancyID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnyearID, 0, 0, User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), comments);
                    SqlTransaction transaction = connection.BeginTransaction();

                    string X_Criteria = "N_VacancyID=" + nVacancyID + " and N_CompanyID=" + nCompanyID;
                    string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());        

                    string status = myFunctions.UpdateApprovals(Approvals, nFnyearID, "Job Vacancy", nVacancyID, TransRow["X_VacancyCode"].ToString(), ProcStatus, "Rec_JobVacancy", X_Criteria, "", User, dLayer, connection, transaction);
                    if (status != "Error")
                    {

                    // Results = dLayer.DeleteData("Rec_JobVacancy", "N_VacancyID", nVacancyID, "N_CompanyID =" + nCompanyID, connection, transaction);

                    // if (Results > 0)
                    // {
                        transaction.Commit();
                        return Ok(api.Success("Job Vacancy " + status + " Successfully"));
                    }
                    else
                    {
                        return Ok(api.Error(User, "Unable to delete Job Vacancy"));
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }



        }
    }
}

