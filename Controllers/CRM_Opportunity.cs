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
    [Route("opportunity")]
    [ApiController]
    public class CRM_Opportunity : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        public CRM_Opportunity(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("list")]
        public ActionResult OpportunityList(int nFnYearId,int nPage, int nSizeperpage, string xSearchkey, string xSortBy, string screen, string winoe, int nCustomerID)
        {
            DataTable dt = new DataTable();
            DataTable dtRevenue = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "",sqlCommandTotRevenue="";
            string criteria = "";

            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            string UserPattern = myFunctions.GetUserPattern(User);
            string Pattern = "";

            if (screen=="Opportunity")
                criteria = "and (N_ClosingStatusID=0 or N_ClosingStatusID is null)";
             if (screen=="Win")
            {
            criteria = "and N_StatusTypeID=308 and MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP)";
            }
             if (screen=="Lose")
            {
            criteria = "and N_StatusTypeID=309 and MONTH(D_Entrydate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_Entrydate) = YEAR(CURRENT_TIMESTAMP)";
            }   

            if (UserPattern != "")
            {
                Pattern = " and (Left(X_Pattern,Len(@p2))=@p2 or N_LoginUserID="+nUserID+")";
                Params.Add("@p2", UserPattern);
            }
            // else
            // {
            //     Pattern = " and (N_CreatedUser=" + nUserID + " or N_LoginUserID="+nUserID+")";

            // }
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and (X_Opportunity like'%" + xSearchkey + "%'or X_OpportunityCode like'%" + xSearchkey + "%'or N_ExpRevenue like'%" + xSearchkey + "%'or X_Stage like'%" + xSearchkey + "%'or D_ClosingDate like'%" + xSearchkey + "%'or X_nextStep like'%" + xSearchkey + "%'or X_Contact like'%" + xSearchkey + "%'or X_Mobile like'%" + xSearchkey + "%'or X_SalesmanName like'%" + xSearchkey + "%'or X_StatusType like'%" + xSearchkey + "%' or X_Probability like'%" + xSearchkey + "%' or x_NextActivity like'%" + xSearchkey + "%' or X_WorkType like'%" + xSearchkey + "%' or X_Contact like'%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by D_EntryDate desc";
            else
                xSortBy = " order by " + xSortBy;

            if(screen =="Lose" ||  screen=="Win")
            {
                if (nCustomerID > 0) {
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMOpportunity where N_CompanyID=@p1 and N_CustID="+ nCustomerID + criteria + Pattern + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMOpportunity where N_CompanyID=@p1 and N_CustID="+ nCustomerID + criteria + Pattern + Searchkey + " and N_OpportunityID not in (select top(" + Count + ") N_OpportunityID from vw_CRMOpportunity where N_CompanyID=@p1 and N_CustID="+ nCustomerID + criteria + xSortBy + " ) " + xSortBy;
                } else {
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMOpportunity where N_CompanyID=@p1  " + criteria + Pattern + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMOpportunity where N_CompanyID=@p1  " + criteria + Pattern + Searchkey + " and N_OpportunityID not in (select top(" + Count + ") N_OpportunityID from vw_CRMOpportunity where N_CompanyID=@p1 " + criteria + xSortBy + " ) " + xSortBy;
                }
             }
             else
             {
                if (nCustomerID > 0) {
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMOpportunity where N_CompanyID=@p1  and isnull(N_ClosingStatusID,0) = 0 and N_CustID="+ nCustomerID + criteria + Pattern + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMOpportunity where N_CompanyID=@p1  and isnull(N_ClosingStatusID,0) = 0 and N_CustID="+ nCustomerID + criteria + Pattern + Searchkey + " and N_OpportunityID not in (select top(" + Count + ") N_OpportunityID from vw_CRMOpportunity where N_CompanyID=@p1 and N_CustID="+ nCustomerID + criteria + xSortBy + " ) " + xSortBy;
                } else {
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMOpportunity where N_CompanyID=@p1  and isnull(N_ClosingStatusID,0) = 0 " + criteria + Pattern + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMOpportunity where N_CompanyID=@p1  and isnull(N_ClosingStatusID,0) = 0 " + criteria + Pattern + Searchkey + " and N_OpportunityID not in (select top(" + Count + ") N_OpportunityID from vw_CRMOpportunity where N_CompanyID=@p1 " + criteria + xSortBy + " ) " + xSortBy;
                }
             }
            Params.Add("@p1", nCompanyId);
            Params.Add("@p3", nFnYearId);
            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    if (nCustomerID > 0) {
                        sqlCommandCount = "select count(1) as N_Count  from vw_CRMOpportunity where N_CompanyID=@p1  and  isnull(N_ClosingStatusID,0) = 0 and N_CustID="+ nCustomerID + criteria + Pattern;
                    } else {
                        sqlCommandCount = "select count(1) as N_Count  from vw_CRMOpportunity where N_CompanyID=@p1  and  isnull(N_ClosingStatusID,0) = 0 " + criteria + Pattern;
                    }
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);

                    //sqlCommandTotRevenue = "select N_StageID,X_Stage,SUM(ISNULL(N_ExpRevenue,0)) AS N_TotExpRevenue from vw_CRMOpportunity where N_CompanyID=@p1  and isnull(N_ClosingStatusID,0) = 0 " + Pattern +" group by N_StageID,X_Stage";
                    sqlCommandTotRevenue = "SELECT Gen_LookupTable.N_PkeyId AS N_StageID, Gen_LookupTable.X_Name AS X_Stage, ISNULL(CRM_Opp.N_TotExpRevenue ,0) AS N_TotExpRevenue from Gen_LookupTable LEFT OUTER JOIN "+
                                            " (select N_StageID,X_Stage,SUM(ISNULL(N_ExpRevenue,0)) AS N_TotExpRevenue,N_CompanyID from vw_CRMOpportunity where N_CompanyID=@p1  and isnull(N_ClosingStatusID,0) = 0 " + Pattern +"  "+
                                            " group by N_StageID,X_Stage,N_CompanyID) AS CRM_Opp ON Gen_LookupTable.N_CompanyID=CRM_Opp.N_CompanyID AND Gen_LookupTable.N_PkeyId=CRM_Opp.N_StageID "+
                                            " WHERE Gen_LookupTable.N_ReferId=1310 AND Gen_LookupTable.N_CompanyID=@p1";
                    dtRevenue = dLayer.ExecuteDataTable(sqlCommandTotRevenue, Params, connection);

                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    OutPut.Add("TotRevenue",  api.Format(dtRevenue));
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
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("details")]
        public ActionResult OpportunityListDetails(string xOpportunityCode)
        {
            DataTable dt = new DataTable();
            DataTable dtItem = new DataTable();
            DataTable dtMaterial = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from vw_CRMOpportunity where N_CompanyID=@p1 and X_OpportunityCode=@p2";
            string sqlCommandTextItem = "select * from vw_CRMProducts where N_CompanyID=@p1 and X_OpportunityCode=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", xOpportunityCode);
            SortedList Result = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dtItem = dLayer.ExecuteDataTable(sqlCommandTextItem, Params, connection);

                    int N_OpportunityID = myFunctions.getIntVAL(dt.Rows[0]["N_OpportunityID"].ToString());
                    Params.Add("@p3", N_OpportunityID);

                    string MaterialSql = "select * from vw_Crm_Materials where N_CompanyID=@p1 and N_OpportunityID=@p3";

                    dtMaterial = dLayer.ExecuteDataTable(MaterialSql, Params, connection);
                }
                dt = api.Format(dt);
                Result.Add("Details", dt);
                Result.Add("Items", dtItem);
                Result.Add("Materials", dtMaterial);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(Result));
                }
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
                DataTable MasterTable, Items, TaskMaster,TaskStatus,Activity, Participants,Materials;
                MasterTable = ds.Tables["master"];
                Items = ds.Tables["Items"];
                Materials = ds.Tables["Materials"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nOPPOId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_OpportunityID"].ToString());
                int nOpportunityID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_OpportunityID"].ToString());
                int nBranchId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchId"].ToString());
                int nWActivityID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_WActivityID"].ToString());
                int nTaskOwner = myFunctions.getIntVAL(MasterTable.Rows[0]["N_SalesmanID"].ToString());
                string X_ContactEmail = MasterTable.Rows[0]["X_Email"].ToString();
                string X_ContactNumber = MasterTable.Rows[0]["X_Mobile"].ToString();
                int nContactID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ContactID"].ToString());
                object N_WorkFlowID = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string OpportunityCode = "";
                    var values = MasterTable.Rows[0]["X_OpportunityCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", 1302);
                        Params.Add("N_BranchID", nBranchId);
                        OpportunityCode = dLayer.GetAutoNumber("CRM_Opportunity", "x_OpportunityCode", Params, connection, transaction);
                        if (OpportunityCode == "") { transaction.Rollback(); return Ok(api.Error(User, "Unable to generate Opportunity Code")); }
                        MasterTable.Rows[0]["x_OpportunityCode"] = OpportunityCode;
                    }
                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_OpportunityID='" + nOpportunityID + "'" ;
                    if (nOpportunityID > 0)
                        N_WorkFlowID = dLayer.ExecuteScalar("select N_WactivityID from CRM_Opportunity where N_CompanyID=" + nCompanyID + " and N_OpportunityID=" + nOpportunityID, Params, connection, transaction);
                    nOpportunityID = dLayer.SaveData("CRM_Opportunity", "n_OpportunityID", DupCriteria, "", MasterTable, connection, transaction);
                    if (Items.Rows.Count > 0)
                    {
                        foreach (DataRow dRow in Items.Rows)
                        {
                            dRow["N_OpportunityID"] = nOpportunityID;
                            dRow["n_CompanyID"] = nCompanyID;
                        }
                    }
                    if (nOpportunityID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                    else
                    {

                if (N_WorkFlowID != null)
                        {
                            if (nWActivityID != myFunctions.getIntVAL(N_WorkFlowID.ToString()))
                            {
                                if (nWActivityID > 0)
                                {
                                    //dLayer.DeleteData("Tsk_TaskMaster", "N_ProjectID", nProjectID, "", connection, transaction);

                                //    dLayer.DeleteData("Tsk_TaskStatus", "N_ProjectID", nProjectID, "", connection, transaction);

                                    TaskMaster = dLayer.ExecuteDataTable("select N_CompanyID,2 as N_StatusID,N_StageID,x_tasksummery,x_taskdescription,'"+DateTime.Today+"'  as D_TaskDate,'"+DateTime.Today+"' as D_DueDate, "+myFunctions.GetUserID(User)+" as N_CreatorID, "+myFunctions.GetUserID(User)+" as N_CurrentAssigneeID, "+myFunctions.GetUserID(User)+"  as n_ClosedUserID ,"+myFunctions.GetUserID(User)+"  as n_SubmitterID,"+myFunctions.GetUserID(User)+" as N_CurrentAssignerID,'" + DateTime.Today + "' as D_EntryDate, "+myFunctions.GetUserID(User)+" as N_AssigneeID, N_StartDateBefore,N_StartDateUnitID,N_EndDateBefore,N_EndUnitID,N_WTaskDetailID,N_Order,N_TemplateID,N_PriorityID,N_CategoryID from vw_Prj_WorkflowDetails where N_CompanyID=" + nCompanyID + " and N_WTaskID=" + nWActivityID + " order by N_Order", Params, connection, transaction);
                                    if (TaskMaster.Rows.Count > 0)
                                    {
                                        SortedList AParams = new SortedList();
                                        AParams.Add("N_CompanyID", nCompanyID);
                                        AParams.Add("N_YearID", nFnYearId);
                                        AParams.Add("N_FormID", 1056);
                                        string TaskCode = "";
                                        double Minuts = 0;
                                        int N_TaskID = 0;
                                        TaskMaster = myFunctions.AddNewColumnToDataTable(TaskMaster, "n_TaskID", typeof(int), 0);
                                        TaskMaster = myFunctions.AddNewColumnToDataTable(TaskMaster, "x_TaskCode", typeof(string), "");
                                        TaskMaster = myFunctions.AddNewColumnToDataTable(TaskMaster, "n_OpportunityID", typeof(int), nOpportunityID);
                                        TaskMaster = myFunctions.AddNewColumnToDataTable(TaskMaster, "B_Closed", typeof(int), 0);
                                        // if(!TaskMaster.columns.Contains("D_TaskDate"))
                                        //     TaskMaster = myFunctions.AddNewColumnToDataTable(TaskMaster, "D_TaskDate", typeof(int), 0);
                                     
                                        TaskMaster.Rows[0]["B_Closed"] = 0;
                                        foreach (DataRow var in TaskMaster.Rows)
                                        {
                                            TaskCode = dLayer.GetAutoNumber("Tsk_TaskMaster", "X_TaskCode", AParams, connection, transaction);
                                            if (TaskCode == "") { transaction.Rollback(); return Ok(api.Error(User, "Unable to generate Task Code")); }
                                            var["x_TaskCode"] = TaskCode;

                                            if (var["N_StartDateUnitID"].ToString() == "248")
                                                Minuts = 1;
                                            else if (var["N_StartDateUnitID"].ToString() == "247")
                                                Minuts = 60;
                                            else if (var["N_StartDateUnitID"].ToString() == "246")
                                                Minuts = 1440;
                                            else
                                                Minuts = 10080;

                                            if (var["N_StartDateBefore"].ToString() == "0")
                                                Minuts = 0;
                                            else
                                                Minuts = myFunctions.getVAL(var["N_StartDateBefore"].ToString()) * Minuts;

                                            if(Minuts==0)
                                                var["D_TaskDate"] = DateTime.Now;
                                            else
                                                var["D_TaskDate"] = DateTime.Now.AddMinutes(Minuts);

                                            if (var["N_EndUnitID"].ToString() == "248")
                                                Minuts = 1;
                                            else if (var["N_EndUnitID"].ToString() == "247")
                                                Minuts = 60;
                                            else if (var["N_EndUnitID"].ToString() == "246")
                                                Minuts = 1440;
                                            else
                                                Minuts = 10080;

                                            if (var["N_EndDateBefore"].ToString() == "0")
                                                Minuts = 0;
                                            else
                                                Minuts = myFunctions.getVAL(var["N_EndDateBefore"].ToString()) * Minuts;

                                            if(Minuts==0)
                                                var["D_DueDate"]=DateTime.Now;
                                            else
                                                var["D_DueDate"] = DateTime.Now;

                                        }
                                        TaskMaster.Columns.Remove("N_StartDateBefore");
                                        TaskMaster.Columns.Remove("N_StartDateUnitID");
                                        TaskMaster.Columns.Remove("N_EndDateBefore");
                                        TaskMaster.Columns.Remove("N_EndUnitID");
                                        int N_CreatorID = myFunctions.GetUserID(User);
                                        for (int j = 0; j < TaskMaster.Rows.Count; j++)
                                        {

                                            N_TaskID = dLayer.SaveDataWithIndex("Tsk_TaskMaster", "N_TaskID", "", "", j, TaskMaster, connection, transaction);
                                             TaskStatus = dLayer.ExecuteDataTable("select N_CompanyID,N_AssigneeID,N_SubmitterID,N_CreaterID,N_ClosedUserID,'" + DateTime.Today + "' as D_EntryDate,1 as N_Status from Prj_WorkflowTasks where N_CompanyID=" + nCompanyID + " and N_WTaskDetailID=" + TaskMaster.Rows[j]["N_WTaskDetailID"] + " order by N_Order", Params, connection, transaction);
                                            if (TaskStatus.Rows.Count > 0)
                                            {
                                                TaskStatus = myFunctions.AddNewColumnToDataTable(TaskStatus, "n_TaskID", typeof(int), N_TaskID);
                                                TaskStatus = myFunctions.AddNewColumnToDataTable(TaskStatus, "n_TaskStatusID", typeof(int), 0);
                                                dLayer.SaveData("Tsk_TaskStatus", "n_TaskStatusID", TaskStatus, connection, transaction);
                                                TaskStatus.Clear();

                                            }
                                            if (j == 0)
                                            {
                                                string qry = "Select " + nCompanyID + " as N_CompanyID," + 0 + " as N_TaskStatusID," + N_TaskID + " as N_TaskID,"+myFunctions.GetUserID(User)+" as N_AssigneeID,"+myFunctions.GetUserID(User)+" as N_SubmitterID ,"+myFunctions.GetUserID(User)+" as  N_CreaterID,'" + DateTime.Today + "' as D_EntryDate,'" + "" + "' as X_Notes ," + 4 + " as N_Status ," + 100 + " as N_WorkPercentage";
                                                DataTable DetailTable = dLayer.ExecuteDataTable(qry, Params, connection, transaction);
                                                int nID = dLayer.SaveData("Tsk_TaskStatus", "N_TaskStatusID", DetailTable, connection, transaction);
                                            }
                                           
                                        }
                                    }
                                }
                            }
                        }

                        
                        // if (N_WorkFlowID != null)
                        // {
                        //     if (nWActivityID != myFunctions.getIntVAL(N_WorkFlowID.ToString()))
                        //     {
                        //         if (nWActivityID > 0)
                        //         {
                        //             dLayer.DeleteData("CRM_Activity", "N_ReffID", nOpportunityID, "", connection, transaction);
                        //             Activity = dLayer.ExecuteDataTable("select * from CRM_WorkflowActivities where N_CompanyID=" + nCompanyID + " and N_WActivityID=" + nWActivityID + " order by N_Order", Params, connection, transaction);
                        //             if (Activity.Rows.Count > 0)
                        //             {
                        //                 SortedList AParams = new SortedList();
                        //                 Activity = myFunctions.AddNewColumnToDataTable(Activity, "X_ContactEmail", typeof(string), "");
                        //                 Activity = myFunctions.AddNewColumnToDataTable(Activity, "X_ContactNumber", typeof(string), "");
                        //                 Activity = myFunctions.AddNewColumnToDataTable(Activity, "x_ActivityCode", typeof(string), "");
                        //                 Activity = myFunctions.AddNewColumnToDataTable(Activity, "N_TaskOwner", typeof(int), 0);
                        //                 Activity = myFunctions.AddNewColumnToDataTable(Activity, "n_ActivityID", typeof(int), 0);
                        //                 Activity = myFunctions.AddNewColumnToDataTable(Activity, "X_Contact", typeof(string), "");
                        //                 string ActivityCode = "";
                        //                 AParams.Add("N_CompanyID", nCompanyID);
                        //                 AParams.Add("N_YearID", nFnYearId);
                        //                 AParams.Add("N_FormID", 1307);
                        //                 Activity.Columns.Remove("N_WActivityID");
                        //                 Activity.Columns.Remove("N_WActivityDetailID");
                        //                 int Order = 1;
                        //                 double Minuts = 0;
                        //                 object Contact = dLayer.ExecuteScalar("select X_Contact from crm_Contact where N_ContactID=" + nContactID, Params, connection, transaction);
                        //                 foreach (DataRow var in Activity.Rows)
                        //                 {
                        //                     ActivityCode = dLayer.GetAutoNumber("CRM_Activity", "x_ActivityCode", AParams, connection, transaction);
                        //                     if (ActivityCode == "") { transaction.Rollback(); return Ok(api.Error(User, "Unable to generate Activity Code")); }
                        //                     var["x_ActivityCode"] = ActivityCode;
                        //                     var["N_RelatedTo"] = 294;
                        //                     var["N_ReffID"] = nOpportunityID;
                        //                     var["X_ContactEmail"] = X_ContactEmail;
                        //                     var["X_ContactNumber"] = X_ContactNumber;
                        //                     var["N_TaskOwner"] = nTaskOwner;
                        //                     var["n_ActivityID"] = 0;
                        //                     var["N_Order"] = Order;
                        //                     var["X_Contact"] = Contact;
                        //                     var["N_FnYearId"] = nFnYearId;
                        //                     if (Order == 1)
                        //                     {
                        //                         var["b_closed"] = 1;
                        //                         var["x_status"] = "Closed";
                        //                     }
                        //                     if (var["N_ReminderUnitID"].ToString() == "248")
                        //                         Minuts = 1;
                        //                     else if (var["N_ReminderUnitID"].ToString() == "247")
                        //                         Minuts = 60;
                        //                     else if (var["N_ReminderUnitID"].ToString() == "246")
                        //                         Minuts = 1440;
                        //                     else
                        //                         Minuts = 10080;

                        //                     if (var["N_ReminderBrfore"].ToString() == "0")
                        //                         Minuts = 0;
                        //                     else
                        //                         Minuts = myFunctions.getVAL(var["N_ReminderBrfore"].ToString()) * Minuts;

                        //                     var["D_ScheduleDate"] = DateTime.Now.AddMinutes(Minuts);
                        //                     Order++;
                        //                 }
                        //                 dLayer.SaveData("CRM_Activity", "n_ActivityID", Activity, connection, transaction);
                        //             }
                        //         }

                        //     }

                        // }                   

                        dLayer.SaveData("Crm_Products", "N_CrmItemID", Items, connection, transaction);

                        for (int j = 0; j < Materials.Rows.Count; j++)
                        {
                            Materials.Rows[j]["N_OpportunityID"] = nOpportunityID;
                        }
                        int N_CrmMaterialID = dLayer.SaveData("Crm_Materials", "N_CrmMaterialID", Materials, connection, transaction);

                        transaction.Commit();
                        return Ok(api.Success("Oppurtunity Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }
        [HttpGet("stageupdate")]
        public ActionResult StageUpdate(string xstage, int nOpportunityID)
        {

            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            Params.Add("@p1", nCompanyId);
            xstage = xstage.ToString().Trim();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object N_StageID = dLayer.ExecuteScalar("select N_PkeyID from gen_lookuptable where n_companyID=" + nCompanyId + " and N_ReferID=1310 and X_Name='" + xstage + "'", Params, connection);
                    if (N_StageID != null)
                        dLayer.ExecuteNonQuery("update crm_Opportunity set N_StageID=" + N_StageID.ToString() + " where N_CompanyID=@p1 and N_OpportunityID=" + nOpportunityID, Params, connection);

                }
                return Ok(api.Success("Order Updated"));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nOpportunityID)
        {

            int Results = 0;
            try
            {
                SortedList Params = new SortedList();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    Results = dLayer.DeleteData("CRM_Opportunity", "N_OpportunityID", nOpportunityID, "", connection, transaction);
                    dLayer.DeleteData("Crm_Materials", "N_OpportunityID", nOpportunityID, "", connection, transaction);
                    dLayer.DeleteData("Crm_Products", "N_OpportunityID", nOpportunityID, "", connection, transaction);
                    dLayer.DeleteData("CRM_Activity", "N_ReffID", nOpportunityID, "", connection, transaction);
                    dLayer.DeleteData("TSK_TaskMaster", "N_OpportunityID", nOpportunityID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("N_OpportunityID", nOpportunityID.ToString());
                    return Ok(api.Success(res, "Opportunity deleted"));
                }
                else
                {
                    return Ok(api.Error(User, "Unable to delete Opportunity"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }



        }
        [HttpPost("stageSorting")]
        public ActionResult ChangeData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable StageTable;
                    StageTable = ds.Tables["stageorder"];
                    SortedList Params = new SortedList();
                    int i = 0;

                    int nCompanyId = myFunctions.GetCompanyID(User);
                    Params.Add("@p1", nCompanyId);

                    foreach (DataRow dtRow in StageTable.Rows)
                    {
                        i = i + 1;
                        dLayer.ExecuteNonQuery("update Gen_LookUpTable set N_Sort=" + i + " where N_CompanyID=@p1 and X_Name='" + dtRow["X_Name"].ToString() + "'", Params, connection);

                    }
                    return Ok(api.Success("Stage Updated"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }
      
    }
}