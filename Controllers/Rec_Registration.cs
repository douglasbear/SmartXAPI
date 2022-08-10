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
    [Route("recRegistration")]
    [ApiController]
    public class Rec_Registration : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID = 913;


        public Rec_Registration(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString =
            conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("details")]
        public ActionResult RegistrationDetails(string x_RecruitmentCode)
        {
            DataSet dt = new DataSet();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_RecRegistrartion where N_CompanyID=@p1  and x_RecruitmentCode=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", x_RecruitmentCode);
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
                DataTable MasterTable, dtRec_CandidateEducation, dtRec_CandidateHistory;
                MasterTable = ds.Tables["master"];
                dtRec_CandidateEducation = ds.Tables["Rec_CandidateEducation"];
                dtRec_CandidateHistory = ds.Tables["Rec_CandidateHistory"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nRecruitmentID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_RecruitmentID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_RecruitmentCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Rec_Registration", "X_RecruitmentCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback(); return Ok(api.Error(User, "Unable to generate Registration Code")); }
                        MasterTable.Rows[0]["X_RecruitmentCode"] = Code;
                    }

                    if (nRecruitmentID > 0)
                    {
                        dLayer.DeleteData("Rec_Registration", "N_RecruitmentID", nRecruitmentID, "N_CompanyID =" + nCompanyID, connection, transaction);
                    }

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and X_RecruitmentCode='" + Code + "'";
                    string X_Criteria = "N_CompanyID=" + nCompanyID + "";

                    nRecruitmentID = dLayer.SaveData("Rec_Registration", "N_RecruitmentID", DupCriteria, X_Criteria, MasterTable, connection, transaction);
                    int Rec_CandidateEducationRes = 0;
                    if (dtRec_CandidateEducation.Rows.Count > 0)
                        foreach (DataRow dRow in dtRec_CandidateEducation.Rows)
                        {
                            dRow["N_RecruitmentID"] = nRecruitmentID;
                        }
                    dtRec_CandidateEducation.AcceptChanges();
                    Rec_CandidateEducationRes = dLayer.SaveData("Rec_CandidateEducation", "N_EduID", dtRec_CandidateEducation, connection, transaction);

                    int Rec_CandidateHistoryRes = 0;
                    if (dtRec_CandidateHistory.Rows.Count > 0)
                        foreach (DataRow dRow in dtRec_CandidateHistory.Rows)
                        {
                            dRow["N_RecruitmentID"] = nRecruitmentID;
                        }
                    dtRec_CandidateHistory.AcceptChanges();
                    Rec_CandidateHistoryRes = dLayer.SaveData("Rec_EmploymentHistory", "N_JobID", dtRec_CandidateHistory, connection, transaction);

                    if (nRecruitmentID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Candidate Registered"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }

        [HttpGet("list")]
        public ActionResult RegistrartionList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int nCompanyId = myFunctions.GetCompanyID(User);
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_RecruitmentCode like'%" + xSearchkey + "%'or X_Name like'%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_RecruitmentID desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_RecRegistrartion where N_CompanyID=@p1 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_RecRegistrartion where N_CompanyID=@p1 " + Searchkey + " and N_RecruitmentID not in (select top(" + Count + ") N_RecruitmentID from vw_RecRegistrartion where N_CompanyID=@p1 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_RecRegistrartion where N_CompanyID=@p1";
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
        public ActionResult DeleteData(int nRecruitmentID)
        {

            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Rec_Registration", "N_RecruitmentID", nRecruitmentID, "N_CompanyID =" + nCompanyID, connection, transaction);

                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("Registration deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User, "Unable to delete Registration"));
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

