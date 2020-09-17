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
using System.Security.Claims;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("approval")]
    [ApiController]
    public class Gen_Approvals : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        public Gen_Approvals(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("dashboard")]
        public ActionResult GetApprovalDetails(int nCompanyID, int nnextApproverID, bool bShowAll, bool bShowAllBranch, int N_Branchid, int nApprovalType)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "";

            if (nApprovalType == 1)
            {
                if (bShowAllBranch)
                {
                    if (bShowAll)
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1";
                    else
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1 and N_NextApproverID=@p2";
                }
                else
                {
                    if (bShowAll)
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1 and N_Branchid = @p3";
                    else
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1 and N_NextApproverID=@p2 and N_Branchid = @p3";

                    Params.Add("@p3", N_Branchid);
                }

            }
            else
            {
                if (bShowAllBranch)
                    sqlCommandText = "select * from vw_ApprovalSummary where N_CompanyID=@p1 and N_ActionUserID=@p2 and N_ProcStatusID<>6";
                else
                {
                    sqlCommandText = "select * from vw_ApprovalSummary where N_CompanyID=@p1 and N_ActionUserID=@p2 and N_ProcStatusID<>6 and N_Branchid = @p3";
                    Params.Add("@p3", N_Branchid);
                }

            }

            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nnextApproverID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }
        }

        [HttpGet("GetApprovalSettings")]
        public ActionResult GetApprovalSettings(int nIsApprovalSystem, int nFormID, int nTransID, int nTransUserID, int nTransStatus, int nTransApprovalLevel, int nNextApprovalLevel, int nApprovalID, int nGroupID, int nFnYearID, int nEmpID, int nActionID)
        {
            DataTable SecUserLevel = new DataTable();
            DataTable GenStatus = new DataTable();
            int nMinLevel = 1, nMaxLevel = 0, nActionLevelID = 0, nSubmitter = 0;
            int nNextApprovalID = nTransApprovalLevel + 1;
            string xLastUserName = "", xEntryTime = "";
            int nTempStatusID = 0;
            bool bIsEditable = false;
            int loggedInUserID = api.GetUserID(User);


            /* Approval Response Set */
            SortedList Response = new SortedList();
            Response.Add("btnSaveText", "Save");
            Response.Add("btnDeleteText", "Delete");
            Response.Add("saveEnabled", true);
            Response.Add("deleteEnabled", true);
            Response.Add("saveVisible", true);
            Response.Add("deleteVisible", true);
            Response.Add("saveTag", 0);
            Response.Add("deleteTag", 0);
            Response.Add("isApprovalSystem", nIsApprovalSystem);
            Response.Add("isEditable", true);
            Response.Add("nextApprovalLevel", nNextApprovalLevel);
            Response.Add("lblVisible", false);
            Response.Add("lblText", "");

            /* Approval Param Set */
            SortedList ApprovalParams = new SortedList();
            int nCompanyID = api.GetCompanyID(User);
            ApprovalParams.Add("@nCompanyID", nCompanyID);
            ApprovalParams.Add("@nFormID", nFormID);
            ApprovalParams.Add("@nTransID", nTransID);
            ApprovalParams.Add("@nApprovalID", nApprovalID);
            ApprovalParams.Add("@nNextApprovalID", nNextApprovalID);
            ApprovalParams.Add("@nTransUserID", nTransUserID);
            ApprovalParams.Add("@nTransApprovalLevel", nTransApprovalLevel);
            ApprovalParams.Add("@nTransStatus", nTransStatus);
            ApprovalParams.Add("@nGroupID", nGroupID);
            ApprovalParams.Add("@loggedInUserID", loggedInUserID);




            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (nApprovalID == 0)
                    {
                        if (nEmpID.ToString() != null && nActionID.ToString() != null && nEmpID.ToString() != "" && nActionID.ToString() != "")
                        {
                            SortedList EmpParams = new SortedList();
                            EmpParams.Add("@nCompanyID", nCompanyID);
                            EmpParams.Add("@nFnYearID", nFnYearID);
                            EmpParams.Add("@nEmpID", nEmpID);
                            EmpParams.Add("@nActionID", nActionID);
                            object objApproval = dLayer.ExecuteScalar("Select isnull(N_ApprovalID,0) as N_ApprovalID from vw_EmpApprovalSettings where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_EmpID=@nEmpID and N_ActionID=@nActionID", EmpParams, connection);
                            if (objApproval != null)
                            {
                                nApprovalID = myFunctions.getIntVAL(objApproval.ToString());
                                ApprovalParams["@nApprovalID"] = nApprovalID;
                            }

                        }
                        else
                        {
                            object ApprovalCode = dLayer.ExecuteScalar("Select N_ApprovalID from Sec_ApprovalSettings_General where N_FormID=@nFormID and N_CompanyID=@nCompanyID", ApprovalParams, connection);
                            if (ApprovalCode != null)
                            {
                                nApprovalID = myFunctions.getIntVAL(ApprovalCode.ToString());
                                ApprovalParams["@nApprovalID"] = nApprovalID;
                            }
                        }
                    }

                    if (nApprovalID > 0)
                    {
                        nIsApprovalSystem = 1;
                        Response["nIsApprovalSystem"] = nIsApprovalSystem;
                    }

                    if (nIsApprovalSystem == -1)
                    {

                        object res = dLayer.ExecuteScalar("Select COUNT(N_FormID) from Sec_ApprovalSettings_General Where N_FormID=@nFormID and N_CompanyID=@nCompanyID", ApprovalParams, connection);

                        if (myFunctions.getIntVAL(res.ToString()) > 0)
                        {
                            nIsApprovalSystem = 1;
                            Response["nIsApprovalSystem"] = nIsApprovalSystem;
                        }
                        else
                        {
                            Response["btnSaveText"] = "Save";
                            Response["btnDeleteText"] = "Delete";
                            Response["saveEnabled"] = true;
                            Response["deleteEnabled"] = true;
                            Response["saveTag"] = 0;
                            Response["deleteTag"] = 0;
                            Response["isApprovalSystem"] = 0;
                            Response["isEditable"] = true;
                            return Ok(api.Success(Response));
                        }
                    }


                    if (nIsApprovalSystem == 1)
                    {
                        nTempStatusID = nTransStatus;


                        object MaxLevel = null;
                        object ActionLevel = null;
                        object OB_Submitter = null;
                        if (nTransID == 0)
                        {
                            MaxLevel = dLayer.ExecuteScalar("Select Isnull (MAX(N_level),0) from Gen_ApprovalCodesDetails where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID", ApprovalParams, connection);

                            if ((nTransApprovalLevel > nNextApprovalLevel) && nTransStatus != 4 && nTransStatus != 3)
                            {
                                ActionLevel = dLayer.ExecuteScalar("Select Isnull (N_ActionTypeId,0) from Gen_ApprovalCodesDetails where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_UserID=@loggedInUserID  and N_Level=( @nNextApprovalID + 1 )", ApprovalParams, connection);
                            }
                            else
                                ActionLevel = dLayer.ExecuteScalar("Select Isnull (N_ActionTypeId,0) from Gen_ApprovalCodesDetails where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_UserID=@loggedInUserID  and N_Level=@nNextApprovalID", ApprovalParams, connection);

                            OB_Submitter = dLayer.ExecuteScalar("Select isnull(N_level,0) as N_level from Gen_ApprovalCodesDetails where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_ActionTypeId=111", ApprovalParams, connection);
                        }
                        else
                        {
                            MaxLevel = dLayer.ExecuteScalar("Select Isnull (MAX(N_LevelID),0) from Gen_ApprovalCodesTrans where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_FormID=@nFormID and N_TransID=@nTransID", ApprovalParams, connection);

                            if ((nTransApprovalLevel > nNextApprovalLevel) && nTransStatus != 4 && nTransStatus != 3)
                            {
                                ActionLevel = dLayer.ExecuteScalar("Select Isnull (N_ActionTypeID,0) from Gen_ApprovalCodesTrans where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_UserID=@loggedInUserID  and N_LevelID=( @nNextApprovalID + 1 ) and N_FormID=@nFormID and N_TransID=@nTransID", ApprovalParams, connection);
                            }
                            else
                                ActionLevel = dLayer.ExecuteScalar("Select Isnull (N_ActionTypeID,0) from Gen_ApprovalCodesTrans where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_UserID=@loggedInUserID  and N_LevelID=@nNextApprovalID and N_FormID=@nFormID and N_TransID=@nTransID", ApprovalParams, connection);

                            OB_Submitter = dLayer.ExecuteScalar("Select isnull(N_LevelID,0) as N_level from Gen_ApprovalCodesTrans where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_ActionTypeId=111 and N_FormID=@nFormID and N_TransID=@nTransID", ApprovalParams, connection);
                        }



                        if (MaxLevel != null)
                            nMaxLevel = myFunctions.getIntVAL(MaxLevel.ToString());

                        if (ActionLevel != null)
                            nActionLevelID = myFunctions.getIntVAL(ActionLevel.ToString());


                        if (OB_Submitter != null)
                            nSubmitter = myFunctions.getIntVAL(OB_Submitter.ToString());
                        else
                            nSubmitter = nMaxLevel;


                        if (nTransID > 0)
                        {
                            if (nTransUserID == loggedInUserID)
                            {
                                if (nTransStatus != 3 && nTransStatus != 4)
                                {
                                    if (nMinLevel == nTransApprovalLevel)
                                    {
                                        nTransStatus = 911;
                                        ApprovalParams["@nTransStatus"] = nTransStatus;
                                    }
                                    else
                                    {
                                        nTransStatus = 912;
                                        ApprovalParams["@nTransStatus"] = nTransStatus;
                                    }
                                }
                                else if (nTransStatus == 3)
                                {
                                    nTransStatus = 918;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                                else if (nTransStatus == 4)
                                {
                                    nTransStatus = 919;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                                nNextApprovalLevel = nTransApprovalLevel;
                                Response["nextApprovalLevel"] = nNextApprovalLevel;
                            }
                            else
                            {

                                SecUserLevel = dLayer.ExecuteDataTable("SELECT TOP (1) N_FormID, N_TransID, Approved_User, N_ApprovalLevelId,D_ApprovedDate, N_BranchID, N_CompanyID, N_FnYearID, N_ProcStatus, N_UserID, N_IssaveDraft, N_NextApprovalLevelId, X_Status, N_CurrentApprover, N_NextApproverID FROM vw_ApprovalDashBoard WHERE (N_FormID = @nFormID ) AND (N_CompanyID = @nCompanyID) AND (N_TransID = @nTransID ) AND (N_NextApproverID = @loggedInUserID )", ApprovalParams, connection);
                                if (SecUserLevel.Rows.Count > 0)
                                {
                                    DataRow Drow = SecUserLevel.Rows[0];
                                    xLastUserName = Drow["Approved_User"].ToString();
                                    xEntryTime = Drow["D_ApprovedDate"].ToString();
                                    nTransStatus = myFunctions.getIntVAL(Drow["N_ProcStatus"].ToString());
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                    //nTransStatus = 912;`
                                    nNextApprovalLevel = myFunctions.getIntVAL(Drow["N_NextApprovalLevelId"].ToString());
                                    Response["nextApprovalLevel"] = nNextApprovalLevel;
                                    if (nActionLevelID == 110)
                                    {
                                        if (((nMaxLevel == nTransApprovalLevel || nSubmitter == nTransApprovalLevel)) && nTransStatus != 3)
                                        {
                                            nTransStatus = 921;
                                            ApprovalParams["@nTransStatus"] = nTransStatus;
                                        }
                                        //else
                                        //    nTransStatus = 7;
                                    }
                                    if (nTransStatus == 3)
                                    {
                                        if (nMinLevel == nNextApprovalLevel)
                                        {
                                            nTransStatus = 923;
                                            ApprovalParams["@nTransStatus"] = nTransStatus;
                                        }
                                        else
                                        {
                                            nTransStatus = 917;
                                            ApprovalParams["@nTransStatus"] = nTransStatus;
                                        }
                                    }
                                    if (nTransStatus == 4)
                                    {
                                        nTransStatus = 920;
                                        ApprovalParams["@nTransStatus"] = nTransStatus;
                                        nNextApprovalLevel = nNextApprovalLevel - 1;
                                        Response["nextApprovalLevel"] = nNextApprovalLevel;
                                    }
                                }
                                else
                                {
                                    if (nTransStatus != 3 && nTransStatus != 4)
                                    {
                                        nTransStatus = 913;
                                        ApprovalParams["@nTransStatus"] = nTransStatus;
                                    }
                                    else if (nTransStatus == 3)
                                    {
                                        nTransStatus = 918;
                                        ApprovalParams["@nTransStatus"] = nTransStatus;
                                    }
                                    else if (nTransStatus == 4)
                                    {
                                        nTransStatus = 920;
                                        ApprovalParams["@nTransStatus"] = nTransStatus;
                                    }
                                }
                            }
                        }
                        else if (nTransID == 0)
                        {
                            nActionLevelID = 0;
                            SecUserLevel = dLayer.ExecuteDataTable("Select N_UserID from Gen_ApprovalCodesDetails Where N_CompanyID=@nCompanyID and N_ApprovalID=@nApprovalID and N_level=1 and (N_UserID in (-11,@loggedInUserID ))", ApprovalParams, connection);
                            if (SecUserLevel.Rows.Count > 0)
                            {
                                nNextApprovalLevel = 1;
                                Response["nextApprovalLevel"] = nNextApprovalLevel;
                                nTransStatus = 901;
                                ApprovalParams["@nTransStatus"] = nTransStatus;
                            }
                            else
                            {
                                nTransStatus = 900;
                                ApprovalParams["@nTransStatus"] = nTransStatus;
                            }
                        }

                        object NextApprovalUser = null;

                        if (xLastUserName.Trim() == "" && nTransID > 0)
                        {
                            if ((nMaxLevel == nTransApprovalLevel || nSubmitter == nTransApprovalLevel) && nTransUserID == loggedInUserID)
                            {
                                if (nMaxLevel == nMinLevel || nSubmitter == nMinLevel)
                                {
                                    nTransStatus = 922;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                                else if (nTransStatus != 3 && nTransStatus != 918 && nTransStatus != 4 && nTransStatus != 919)
                                {
                                    nTransStatus = 914;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                                NextApprovalUser = dLayer.ExecuteScalar("SELECT X_UserName FROM Sec_User Where N_UserID=@nTransUserID  and N_CompanyID=@nCompanyID", ApprovalParams, connection);
                                if (NextApprovalUser != null)
                                    xLastUserName = NextApprovalUser.ToString();
                            }
                            else if ((nMaxLevel == nTransApprovalLevel || nSubmitter == nTransApprovalLevel) && nTransUserID != loggedInUserID)
                            {
                                if (nTransStatus != 918 && nTransStatus != 919 && nTransStatus != 920)
                                {
                                    if (nTransStatus == 913 || nTransStatus == 7)
                                    { nTransStatus = 916; }
                                    else
                                    { nTransStatus = 915; }
                                }
                                NextApprovalUser = dLayer.ExecuteScalar("SELECT X_UserName FROM Sec_User Where N_UserID=@nTransUserID  and N_CompanyID=@nCompanyID", ApprovalParams, connection);
                                if (NextApprovalUser != null)
                                    xLastUserName = NextApprovalUser.ToString();
                            }
                            else
                            {
                                if (nTransStatus == 3 || nTransStatus == 918 || nTransStatus == 917 || nTransStatus == 4 || nTransStatus == 919)
                                {
                                    NextApprovalUser = dLayer.ExecuteScalar("SELECT X_UserName FROM Sec_User Where N_UserID=@nTransUserID  and N_CompanyID=@nCompanyID", ApprovalParams, connection);
                                    if (NextApprovalUser != null)
                                        xLastUserName = NextApprovalUser.ToString();
                                }
                                else
                                {
                                    NextApprovalUser = dLayer.ExecuteScalar("SELECT Sec_User.X_UserName FROM vw_ApprovalDashBoard INNER JOIN Sec_User ON vw_ApprovalDashBoard.N_NextApproverId = Sec_User.N_UserID AND vw_ApprovalDashBoard.N_CompanyID = Sec_User.N_CompanyID WHERE vw_ApprovalDashBoard.N_CompanyID =@nCompanyID AND vw_ApprovalDashBoard.N_TransID = @nTransID AND vw_ApprovalDashBoard.N_ApprovalLevelId =@nTransApprovalLevel AND vw_ApprovalDashBoard.N_FormID=@nFormID", ApprovalParams, connection);
                                    if (NextApprovalUser != null)
                                        xLastUserName = NextApprovalUser.ToString();
                                }

                            }
                        }
                        //}
                        GenStatus = dLayer.ExecuteDataTable("SELECT * FROM vw_GenApproval_Status WHERE N_CompanyId=@nCompanyID and N_StatusId=@nTransStatus and N_GroupID=@nGroupID", ApprovalParams, connection);
                        if (GenStatus.Rows.Count == 1)
                        {
                            DataRow Status = GenStatus.Rows[0];
                            if (Status["X_NoStatusCaption"].ToString().Trim() == "")
                            {
                                Response["btnDeleteText"] = "Delete";
                                Response["deleteVisible"] = true;
                                Response["deleteEnabled"] = false;

                            }
                            else
                            {
                                Response["btnDeleteText"] = Status["X_NoStatusCaption"].ToString();
                                Response["deleteVisible"] = true;
                                Response["deleteEnabled"] = true;
                                Response["deleteTag"] = Status["N_NoAction"].ToString();

                            }
                            if (Status["X_YesStatusCaption"].ToString().Trim() == "")
                            {
                                Response["btnSaveText"] = "Save";
                                Response["saveVisible"] = true;
                                Response["saveEnabled"] = false;
                            }
                            else
                            {
                                Response["btnSaveText"] = Status["X_YesStatusCaption"].ToString();
                                Response["saveEnabled"] = true;
                                Response["saveTag"] = Status["N_YesAction"].ToString();

                            }
                            if (nActionLevelID == 110)
                            {
                                Response["saveEnabled"] = false;
                                Response["deleteEnabled"] = true;
                                Response["btnDeleteText"] = "OK";
                                Response["deleteTag"] = 7;
                            }

                            if (Status != null)
                            {
                                Response["lblVisible"] = true;
                                Response["lblText"] = Status["X_MsgStatus"].ToString();
                                Response["lblText"] = Response["lblText"].ToString().Replace("#NAME", xLastUserName);
                                if (xEntryTime.Trim() != "")
                                    xEntryTime = Convert.ToDateTime(xEntryTime).ToString("dd/MM/yyyy HH:mm:ss");
                                Response["lblText"] = Response["lblText"].ToString().Replace("#DATE", xEntryTime);
                            }

                        }

                        //Blocking edit control of Approvers
                        if (!bIsEditable)
                        {
                            if (nNextApprovalLevel != 1)
                                Response["isEditable"] = false;
                        }
                    }
                }
                return Ok(api.Success(Response));
            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }

        }

        // public SortedList SetStatus(SortedList Response, DataRow rw, string struserName, string strEntryTime)
        // {
        //     if (rw != null)
        //     {
        //         Response["lblVisible"] = true;
        //         Response["lblText"] = rw["X_MsgStatus"].ToString();
        //         Response["lblText"] = Response["lblText"].ToString().Replace("#NAME", struserName);
        //         if (strEntryTime.Trim() != "")
        //             strEntryTime = Convert.ToDateTime(strEntryTime).ToString("dd/MM/yyyy HH:mm:ss");
        //         Response["lblText"] = Response["lblText"].ToString().Replace("#DATE", strEntryTime);
        //     }
        //     return Response;
        // }

        // public static void saveApprovals(DataTable MasterTable, int FormID, int ApprovalID,DataAccessLayer dLayer)
        // {
        //     int N_MaxLevelID = 0, N_Submitter = 0;
        //     int N_ApprovalID = 0;
        //     N_ApprovalID = ApprovalID;
        //     SortedList Params = new SortedList();

        //     if (N_ApprovalID == 0)
        //     {
        //         object ApprovalCode = dLayer.ExecuteScalar("Select N_ApprovalID from Sec_ApprovalSettings_General where N_FormID=" + FormID + " and N_CompanyID=" + myCompanyID._CompanyID);
        //         if (ApprovalCode != null)
        //             N_ApprovalID = myFunctions.getIntVAL(ApprovalCode.ToString());
        //     }

        //     if (N_IsApprovalSystem == 1)
        //     {
        //         N_MaxLevelID = myFunctions.getIntVAL(dba.ExecuteSclar("Select Isnull (max(N_level),0) from Gen_ApprovalCodesDetails where N_ApprovalID=" + N_ApprovalID + " and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable()).ToString());

        //         object OB_Submitter = dba.ExecuteSclar("Select isnull(N_level,0) as N_level from Gen_ApprovalCodesDetails where N_ApprovalID=" + N_ApprovalID + " and N_CompanyID=" + myCompanyID._CompanyID + " and N_ActionTypeId=111", "TEXT", new DataTable());
        //         if (OB_Submitter != null)
        //             N_Submitter = myFunctions.getIntVAL(OB_Submitter.ToString());
        //         else
        //             N_Submitter = N_MaxLevelID;

        //         if (N_userLevel == N_MaxLevelID || N_userLevel == N_Submitter)
        //             N_SaveDraft = 0;
        //         else
        //             N_SaveDraft = 1;
        //         FieldList += ",N_ApprovalLevelId,N_ProcStatus,B_IssaveDraft";
        //         FieldValues += "|" + N_userLevel + "|'" + N_ProcStatus + "'|" + N_SaveDraft;
        //     }
        //     else if (N_IsApprovalSystem == 0)
        //     {
        //         FieldList += ",B_IssaveDraft";
        //         FieldValues += "|" + 0;
        //     }
        // }



    }
}