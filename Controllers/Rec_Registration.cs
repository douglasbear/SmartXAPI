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
        private readonly IMyAttachments myAttachments;

        private readonly int N_FormID = 913;


        public Rec_Registration(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("details")]
        public ActionResult RegistrationDetails(string x_RecruitmentCode, int n_MainActionID)
        {
            DataSet dt = new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable CandidateEducation = new DataTable();
            DataTable EmploymentHistory = new DataTable();
            DataTable Actions = new DataTable();
            SortedList Params = new SortedList();
            DataTable Attachments = new DataTable();
            DataTable Master = new DataTable();
            DataTable MailData = new DataTable();

            MasterTable = api.Format(MasterTable, "Master");
            int N_RecruitmentID = 0;
            int N_ActionID = 0;
            int N_UserID=myFunctions.GetUserID(User);

            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_RecRegistrartion where N_CompanyID=@p1  and x_RecruitmentCode=@p2";
            string sqlCommandEducation = "select * from Rec_CandidateEducation where N_CompanyID=@p1  and N_RecruitmentID=@p3";
            string sqlCommandPaymentHistory = "select * from Rec_EmploymentHistory where N_CompanyID=@p1  and N_RecruitmentID=@p3";
            string sqlCommandOptions = "select * from vw_GenStatusDetails where N_CompanyID=@p1  and N_CurrentActionID=@p4 and N_userID in (0,"+N_UserID+")";
            string sqlmailData = "select * from vw_Genmail where N_CompanyID=@p1  and N_ActionID=" + n_MainActionID;
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
                    N_RecruitmentID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_RecruitmentID"].ToString());
                    N_ActionID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ActionID"].ToString());
                    Params.Add("@p3", N_RecruitmentID);
                    Params.Add("@p4", N_ActionID);
                    CandidateEducation = dLayer.ExecuteDataTable(sqlCommandEducation, Params, connection);
                    EmploymentHistory = dLayer.ExecuteDataTable(sqlCommandPaymentHistory, Params, connection);
                    Actions = dLayer.ExecuteDataTable(sqlCommandOptions, Params, connection);

                    if (n_MainActionID > 0)
                    {
                        MailData = dLayer.ExecuteDataTable(sqlmailData, Params, connection);
                        MasterTable.Rows[0]["x_Subject"] = MailData.Rows[0]["x_Subject"];
                        MasterTable.Rows[0]["x_Body"] = MailData.Rows[0]["x_Body"];
                    }


                    MasterTable = api.Format(MasterTable, "Master");
                    CandidateEducation = api.Format(CandidateEducation, "Rec_CandidateEducation");
                    EmploymentHistory = api.Format(EmploymentHistory, "Rec_EmploymentHistory");
                    Actions = api.Format(Actions, "Actions");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(CandidateEducation);
                    dt.Tables.Add(EmploymentHistory);
                    dt.Tables.Add(Actions);
                    if (dt.Tables.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(MasterTable.Rows[0]["N_RecruitmentID"].ToString()), myFunctions.getIntVAL(MasterTable.Rows[0]["N_RecruitmentID"].ToString()), this.N_FormID, myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString()), User, connection);
                        Attachments = api.Format(Attachments, "attachments");
                        dt.Tables.Add(Attachments);
                    }
                }

                return Ok(api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
        [HttpGet("listvacancy")]
        public ActionResult VacancyList()
        {
            DataTable dt = new DataTable();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();


            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_JobVacancy where N_CompanyID=@p1 and b_issavedraft=0";
            Params.Add("@p1", nCompanyId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
        [HttpGet("listaction")]
        public ActionResult GetActionDetails(int nActionID, int nFormID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nActionID", nActionID);
            Params.Add("@nFormID", nFormID);
            string sqlCommandText = "";
            if (nActionID > 0)
                sqlCommandText = "select * from vw_GenStatusDetails where N_CompanyID=@nCompanyID and N_CurrentActionID=@nActionID and N_FormID=@nFormID";
            else
                sqlCommandText = "select * from vw_GenStatusDetails where N_CompanyID=@nCompanyID  and N_FormID=@nFormID and B_IsDefault=1";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                    return Ok(api.Notice("No Results Found"));
                else
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
                DataTable Attachment = ds.Tables["attachments"];
                DataRow MasterRow = MasterTable.Rows[0];
                dtRec_CandidateEducation = ds.Tables["Rec_CandidateEducation"];
                dtRec_CandidateHistory = ds.Tables["rec_EmploymentHistory"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nRecruitmentID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_RecruitmentID"].ToString());
                int N_RecruitmentID = myFunctions.getIntVAL(MasterRow["N_RecruitmentID"].ToString());
                string X_RecruitmentCode = "";
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
                    string image = myFunctions.ContainColumn("i_Photo", MasterTable) ? MasterTable.Rows[0]["i_Photo"].ToString() : "";
                    Byte[] photoBitmap = new Byte[image.Length];
                    photoBitmap = Convert.FromBase64String(image);
                    if (myFunctions.ContainColumn("i_Photo", MasterTable))
                        MasterTable.Columns.Remove("i_Photo");
                    MasterTable.AcceptChanges();

                    if (nRecruitmentID > 0)
                    {
                        dLayer.DeleteData("Rec_Registration", "N_RecruitmentID", nRecruitmentID, "N_CompanyID =" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Rec_CandidateEducation", "N_RecruitmentID", nRecruitmentID, "N_CompanyID =" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Rec_EmploymentHistory", "N_RecruitmentID", nRecruitmentID, "N_CompanyID =" + nCompanyID, connection, transaction);
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
                    SortedList RecruitmentParams = new SortedList();
                    RecruitmentParams.Add("@nRecruitmentID", N_RecruitmentID);
                    DataTable CustomerInfo = dLayer.ExecuteDataTable("Select X_RecruitmentCode,X_Name from Rec_Registration where N_RecruitmentID=@nRecruitmentID", RecruitmentParams, connection, transaction);
                    if (CustomerInfo.Rows.Count > 0)
                    {
                        try
                        {

                            myAttachments.SaveAttachment(dLayer, Attachment, X_RecruitmentCode, nRecruitmentID, CustomerInfo.Rows[0]["X_Name"].ToString().Trim(), CustomerInfo.Rows[0]["X_RecruitmentCode"].ToString(), myFunctions.getIntVAL(MasterTable.Rows[0]["N_RecruitmentID"].ToString()), "Recruitment", User, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User, ex));
                        }
                    }


                    if (nRecruitmentID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                    else

                    {
                        if (image.Length > 0)
                        {
                            dLayer.SaveImage("Rec_Registration", "I_Photo", photoBitmap, "N_RecruitmentID", nRecruitmentID, connection, transaction);
                        }

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

        //     [HttpPost("actionupdate")]
        //     public ActionResult UpdateData([FromBody] DataSet ds)
        //     {
        //         try
        //         {
        //             DataTable MasterTable, dtRec_CandidateEducation, dtRec_CandidateHistory;
        //             MasterTable = ds.Tables["master"];

        //             DataRow MasterRow = MasterTable.Rows[0];

        //             int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
        //             int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
        //             int nRecruitmentID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_RecruitmentID"].ToString());
        //             int N_RecruitmentID = myFunctions.getIntVAL(MasterRow["N_RecruitmentID"].ToString());
        //             string X_RecruitmentCode = "";
        //             using (SqlConnection connection = new SqlConnection(connectionString))
        //             {
        //                 connection.Open();
        //                 SqlTransaction transaction = connection.BeginTransaction();
        //                 SortedList Params = new SortedList();

        //                 nRecruitmentID = dLayer.SaveData("Rec_Registration", "N_RecruitmentID", DupCriteria, X_Criteria, MasterTable, connection, transaction);
        //                 int Rec_CandidateEducationRes = 0;
        //                 if (dtRec_CandidateEducation.Rows.Count > 0)
        //                     foreach (DataRow dRow in dtRec_CandidateEducation.Rows)
        //                     {
        //                         dRow["N_RecruitmentID"] = nRecruitmentID;
        //                     }
        //                 dtRec_CandidateEducation.AcceptChanges();
        //                 Rec_CandidateEducationRes = dLayer.SaveData("Rec_CandidateEducation", "N_EduID", dtRec_CandidateEducation, connection, transaction);

        //                 int Rec_CandidateHistoryRes = 0;
        //                 if (dtRec_CandidateHistory.Rows.Count > 0)
        //                     foreach (DataRow dRow in dtRec_CandidateHistory.Rows)
        //                     {
        //                         dRow["N_RecruitmentID"] = nRecruitmentID;
        //                     }
        //                 dtRec_CandidateHistory.AcceptChanges();
        //                 Rec_CandidateHistoryRes = dLayer.SaveData("Rec_EmploymentHistory", "N_JobID", dtRec_CandidateHistory, connection, transaction);
        //                 SortedList RecruitmentParams = new SortedList();
        //                 RecruitmentParams.Add("@nRecruitmentID", N_RecruitmentID);
        //                 DataTable CustomerInfo = dLayer.ExecuteDataTable("Select X_RecruitmentCode,X_Name from Rec_Registration where N_RecruitmentID=@nRecruitmentID", RecruitmentParams, connection, transaction);
        //                 if (CustomerInfo.Rows.Count > 0)
        //                 {
        //                     try
        //                     {

        //                         myAttachments.SaveAttachment(dLayer, Attachment, X_RecruitmentCode, nRecruitmentID, CustomerInfo.Rows[0]["X_Name"].ToString().Trim(), CustomerInfo.Rows[0]["X_RecruitmentCode"].ToString(), myFunctions.getIntVAL(MasterTable.Rows[0]["N_RecruitmentID"].ToString()), "Recruitment", User, connection, transaction);
        //                     }
        //                     catch (Exception ex)
        //                     {
        //                         transaction.Rollback();
        //                         return Ok(api.Error(User, ex));
        //                     }
        //                 }


        //                 if (nRecruitmentID <= 0)
        //                 {
        //                     transaction.Rollback();
        //                     return Ok(api.Error(User, "Unable to save"));
        //                 }
        //                 else

        //                 {
        //                     if (image.Length > 0)
        //                     {
        //                         dLayer.SaveImage("Rec_Registration", "I_Photo", photoBitmap, "N_RecruitmentID", nRecruitmentID, connection, transaction);
        //                     }

        //                     transaction.Commit();
        //                     return Ok(api.Success("Candidate Registered"));
        //                 }
        //             }
        //         }
        //         catch (Exception ex)
        //         {
        //             return Ok(api.Error(User, ex));
        //         }
        //     }
    }
}

