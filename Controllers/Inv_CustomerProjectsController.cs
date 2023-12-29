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
    [Route("projects")]
    [ApiController]
    public class InvCustomerProjectsController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly IMyFunctions myFunctions;
        private readonly int N_FormID = 74;

        public InvCustomerProjectsController(IDataAccessLayer dl, IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf,IMyAttachments myAtt)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        //GET api/Projects/list
        [HttpGet("list")]
        public ActionResult GetAllProjects(int? nCompanyId, int? nFnYearID, int nEmpID,int nDivisionID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText="";

            if(nEmpID>0)
            {
                //  sqlCommandText = "select * from Vw_InvCustomerProjects where N_CompanyID=@p1 and N_FnYearID=@p2 and X_ProjectCode is not null and x_EmpsID like '%"+nEmpID+"%' or n_ProjectCoordinator ="+nEmpID+" or n_ProjectManager="+nEmpID+" order by N_ProjectID desc";
                // Criterea=" and x_EmpsID like '%"+nEmpID+"%' or n_ProjectCoordinator ="+nEmpID+" or n_ProjectManager="+nEmpID+"";

                sqlCommandText="select N_ProjectID,X_ProjectCode,N_companyID,X_ProjectName,x_EmpsID,N_ProjectCoordinator,N_ProjectManager from Vw_InvCustomerProjects where N_CompanyID=1 and N_FnYearID=16 and X_ProjectCode is not null and x_EmpsID like '%"+nEmpID+"%' or n_ProjectCoordinator ="+nEmpID+" or n_ProjectManager="+nEmpID+""+
                " union all SELECT Tsk_ProjectSettingsDetails.N_ProjectID,Inv_CustomerProjects.X_ProjectCode, Inv_CustomerProjects.N_companyID, Inv_CustomerProjects.X_ProjectName,'' as x_EmpsID,Inv_CustomerProjects.N_ProjectCoordinator,Inv_CustomerProjects.N_ProjectManager FROM  Tsk_ProjectSettingsDetails LEFT OUTER JOIN "+
                " Sec_User ON Tsk_ProjectSettingsDetails.N_CompanyID = Sec_User.N_CompanyID AND Tsk_ProjectSettingsDetails.N_UserID = Sec_User.N_UserID LEFT OUTER JOIN Inv_CustomerProjects ON Tsk_ProjectSettingsDetails.N_ProjectID = Inv_CustomerProjects.N_ProjectID where Sec_User.N_EmpID="+nEmpID+" and Tsk_ProjectSettingsDetails.B_View=1";
                      
            }
            if(nDivisionID>0){
                     sqlCommandText = "select * from Vw_InvCustomerProjects where N_CompanyID=@p1 and N_FnYearID=@p2 and N_DivisionID=@p3 and X_ProjectCode is not null  order by N_ProjectID desc";
            }
            else{
                     sqlCommandText = "select * from Vw_InvCustomerProjects where N_CompanyID=@p1 and N_FnYearID=@p2  and X_ProjectCode is not null  order by N_ProjectID desc";
            }
      
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearID);
            Params.Add("@p3", nDivisionID);
           // Params.Add("@p3", nCustomerID);
            

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
                    return Ok(api.Success(dt));
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

        [HttpGet("projectlist")]
        public ActionResult GetAllProjectlist(int? nCompanyId,int? nFnYearID ,int nCustomerID,bool bAllBranchData,int nBranchID, int nDivisionID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string Criteria = "";
            string divCriteria = "";
            
           if (bAllBranchData == true)
            {
                if (nCustomerID > 0)
                   Criteria =  " and ISNULL(B_IsSaveDraft,0)=0 and ISNULL(B_InActive,0)=0 and   (N_CustomerID =@p3 or  N_CustomerID=0 ) and N_StatusID=1 ";
                else
                    Criteria=  " and ISNULL(B_IsSaveDraft,0)=0 and ISNULL(B_InActive,0)=0 and N_StatusID=1 ";
            }
            else
            {
                if (nCustomerID > 0)
                    Criteria = " and  ISNULL(B_IsSaveDraft,0)=0 and ISNULL(B_InActive,0)=0  and  (N_CustomerID =@p3 or  N_CustomerID=0 ) and  N_StatusID=1";
                else
                    Criteria = " and  ISNULL(B_IsSaveDraft,0)=0 and ISNULL(B_InActive,0)=0 and N_StatusID=1 ";

            }

            if(nDivisionID>0){
               divCriteria = " and  N_DivisionID=@p6";

            }

            //  if (bAllBranchData == true)
            //        Criteria = "AND ISNULL(B_IsSaveDraft,0)=0 and ISNULL(B_InActive,0)=0 ";
            //  else
            //         Criteria = "and  N_BranchID=" + N_BranchID + " AND ISNULL(B_IsSaveDraft,0)=0 and ISNULL(B_InActive,0)=0 ";

            string sqlCommandText = "select * from vw_InvCustomerProjects where N_CompanyID=@p1 and N_FnYearID=@p5" + Criteria + divCriteria +" and X_ProjectCode is not null order by N_ProjectID desc";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p3", nCustomerID);
            Params.Add("@p4", nBranchID);
            Params.Add("@p5", nFnYearID);
            Params.Add("@p6", nDivisionID);
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
                       return Ok(api.Success(dt));
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


        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable, TaskMaster, TaskStatus,JobTable;
                MasterTable = ds.Tables["master"];
                JobTable = ds.Tables["jobMaster"];
                DataTable Attachment = ds.Tables["attachments"];
                DataTable OtherCost = ds.Tables["otherCostDetails"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nProjectID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ProjectID"].ToString());
                int nWTaskID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_WTaskID"].ToString());
                string X_ProjectCode = MasterTable.Rows[0]["X_ProjectCode"].ToString();
                string X_ProjectName = MasterTable.Rows[0]["X_ProjectName"].ToString();
                string xAction = "";
                object N_WorkFlowID = "";
                if (MasterTable.Columns.Contains("x_Action")){
                     xAction = MasterTable.Rows[0]["x_Action"].ToString();
                }
               
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string ProjectCode = "";
                    var values = MasterTable.Rows[0]["X_ProjectCode"].ToString();
                   
                    if (values == "@Auto")

                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);

                        while (true)
                        {
                            values = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from inv_CustomerProjects Where X_ProjectCode ='" + values + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_ProjectCode = values;


                        if (X_ProjectCode == "") { transaction.Rollback(); return Ok(api.Error(User, "Unable to generate")); }
                        MasterTable.Rows[0]["X_ProjectCode"] = X_ProjectCode;

                    }
                    // {
                    //     Params.Add("N_CompanyID", nCompanyID);
                    //     Params.Add("N_YearID", nFnYearId);
                    //     Params.Add("N_FormID", this.N_FormID);
                    //     ProjectCode = dLayer.GetAutoNumber("inv_CustomerProjects", "X_ProjectCode", Params, connection, transaction);
                    //     if (ProjectCode == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Project Information")); }
                    //     MasterTable.Rows[0]["X_ProjectCode"] = ProjectCode;
                    // }
                    MasterTable.Columns.Remove("n_FnYearId");
                    // MasterTable.Columns.Remove("n_LocationID");

                    //Check for Existing Workflow
                    if (nProjectID > 0)
                    {
                        N_WorkFlowID = dLayer.ExecuteScalar("select N_WTaskID from inv_customerprojects where N_CompanyID=" + nCompanyID + " and N_ProjectID=" + nProjectID, Params, connection, transaction);
                        dLayer.ExecuteNonQuery("delete from Inv_OtherCost where  N_CompanyID=" + nCompanyID + "and N_TransID=" + nProjectID + "and X_TransType='Job File' and N_FormID=1579", Params, connection, transaction);// add menuid
                    }
                    if (MasterTable.Columns.Contains("x_Action")){
                        MasterTable.Columns.Remove("x_Action");
                    }
                    
                    nProjectID = dLayer.SaveData("inv_CustomerProjects", "N_ProjectID", MasterTable, connection, transaction);
                    if (nProjectID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                    else
                    {
                        if(JobTable.Rows.Count>0)
                        {
                          JobTable.Rows[0]["N_ProjectID"] =nProjectID;
                          int jobID = dLayer.SaveData("Inv_CustomerJobDetails", "N_JobID", JobTable, connection, transaction);


                        }
                        if (OtherCost.Rows.Count > 0)
                        {
                            foreach (DataRow var in OtherCost.Rows)
                            {
                                var["N_TransID"] = nProjectID;
                                var["X_TransType"] = xAction;
                              
                            }
                        int nOtherCostID = myFunctions.getIntVAL(OtherCost.Rows[0]["n_OtherCostID"].ToString());
                        dLayer.SaveData("Inv_OtherCost", "N_OtherCostID", OtherCost, connection, transaction);
                        }
                        if (N_WorkFlowID != null)
                        {
                            if (nWTaskID != myFunctions.getIntVAL(N_WorkFlowID.ToString()))
                            {
                                if (nWTaskID > 0)
                                {
                                    dLayer.ExecuteScalar("delete from Tsk_TaskStatus where N_TaskID in (select N_TaskID from tsk_TaskMaster where N_ProjectID="+nProjectID+" and N_CompanyID=" + nCompanyID + ")", connection,transaction);
                                    dLayer.ExecuteScalar("delete from Tsk_TaskComments where  N_ActionID in (select N_TaskID from Tsk_TaskMaster where N_ProjectID="+nProjectID+"and N_CompanyID=" + nCompanyID + ")", connection, transaction);
                                    dLayer.DeleteData("TSK_TaskMaster", "N_ProjectID", nProjectID, "", connection, transaction);
                       
                                    TaskMaster = dLayer.ExecuteDataTable("select N_CompanyID,2 as N_StatusID,N_StageID,x_tasksummery,x_taskdescription,'' as D_TaskDate,'' as D_DueDate, "+myFunctions.GetUserID(User)+" as N_CreatorID, "+myFunctions.GetUserID(User)+" as N_CurrentAssigneeID, "+myFunctions.GetUserID(User)+"  as n_ClosedUserID ,"+myFunctions.GetUserID(User)+"  as n_SubmitterID,"+myFunctions.GetUserID(User)+" as N_CurrentAssignerID,'" + DateTime.Today + "' as D_EntryDate, "+myFunctions.GetUserID(User)+" as N_AssigneeID, N_StartDateBefore,N_StartDateUnitID,N_EndDateBefore,N_EndUnitID,N_WTaskDetailID,N_Order,N_TemplateID,N_PriorityID,N_CategoryID from vw_Prj_WorkflowDetails where N_CompanyID=" + nCompanyID + " and N_WTaskID=" + nWTaskID + " order by N_Order", Params, connection, transaction);
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
                                        TaskMaster = myFunctions.AddNewColumnToDataTable(TaskMaster, "n_ProjectID", typeof(int), nProjectID);
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
                                                var["D_DueDate"]=null;
                                            else
                                                var["D_DueDate"] = DateTime.Parse(var["D_TaskDate"].ToString()).AddMinutes(Minuts);

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
                           if (Attachment.Rows.Count > 0)
                    {
                         try
                        {
                           //myAttachments.SaveAttachment(dLayer, Attachment, X_ProjectCode, nProjectID, objCustName.ToString().Trim(), objCustCode.ToString(), N_CustomerID, "Customer Document", User, connection, transaction);
                           myAttachments.SaveAttachment(dLayer, Attachment, X_ProjectCode, nProjectID, X_ProjectName, X_ProjectCode, nProjectID, "Project Document", User, connection, transaction);
                              }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User, ex));
                        }
                    }
                        transaction.Commit();
                        return Ok(api.Success("Project Information Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nProjectID,int nFnyearID)
        {

            int Results = 0;
            try
            {
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                     int nCompanyID = myFunctions.GetCompanyID(User);
                     object objNProjectIDCount = dLayer.ExecuteScalar("Select count(N_ProjectID) from inv_Sales where N_CompanyID=" + nCompanyID + " and N_ProjectID=" + nProjectID + " and N_FnYearID="+nFnyearID, connection, transaction);
                     object objPurchaseCount = dLayer.ExecuteScalar("Select count(N_ProjectID) from inv_Purchase where N_CompanyID=" + nCompanyID + " and N_ProjectID=" + nProjectID + " and N_FnYearID="+nFnyearID, connection, transaction);
                     object objVoucherCount = dLayer.ExecuteScalar("Select count(N_ProjectID) from Acc_VoucherMaster where N_CompanyID=" + nCompanyID + " and N_ProjectID=" + nProjectID + " and N_FnYearID="+nFnyearID, connection, transaction);
                     if (objNProjectIDCount == null)
                        objNProjectIDCount = 0;
                    if (objPurchaseCount == null)
                        objPurchaseCount = 0;
                    if (objVoucherCount == null)
                        objVoucherCount = 0;
                    if (myFunctions.getIntVAL(objNProjectIDCount.ToString()) > 0 || myFunctions.getIntVAL(objPurchaseCount.ToString()) > 0 || myFunctions.getIntVAL(objVoucherCount.ToString()) > 0)
                    {
                       return Ok(api.Error(User, "Unable to delete! transaction processed"));     
                    }
                    dLayer.ExecuteNonQuery("delete from Inv_OtherCost where  N_CompanyID=" +nCompanyID+ "and N_TransID=" + nProjectID +" and X_TransType= 'Job File' and N_FormID=1579", connection, transaction);//add type and menuid
                    dLayer.ExecuteScalar("delete from Tsk_TaskStatus where N_TaskID in (select N_TaskID from tsk_TaskMaster where N_ProjectID="+nProjectID+" and N_CompanyID=" + nCompanyID + ")", connection,transaction);
                    dLayer.ExecuteScalar("delete from Tsk_TaskComments where  N_ActionID in (select N_TaskID from Tsk_TaskMaster where  N_ProjectID="+nProjectID+" and N_CompanyID=" + nCompanyID + ")", connection, transaction);
                    dLayer.DeleteData("Tsk_TaskMaster", "N_ProjectID", nProjectID, "", connection, transaction);                    
                    Results = dLayer.DeleteData("inv_CustomerProjects", "N_ProjectID", nProjectID, "", connection, transaction);
                     myAttachments.DeleteAttachment(dLayer, 1, nProjectID, nProjectID,nFnyearID,74, User, transaction, connection);
                       
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("N_ProjectID", nProjectID.ToString());
                    return Ok(api.Success(res, "Project Information deleted"));
                }
                else
                {
                    return Ok(api.Error(User, "Unable to delete Project Information"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }

        }



  [HttpGet("details")]
        public ActionResult GetCustomerProjectDetails(string xProjectCode, int nFnYearId, int nOpportunityID)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataTable jobMaster = new DataTable();
            DataTable otherCostDetails = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            string sqlJob = "";
             try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
              connection.Open();
             if (nOpportunityID > 0)
            {
                sqlCommandText = "select TOP 1 0 as N_ProjectID,'@Auto' as X_ProjectCode,vw_CRMOpportunity.N_CompanyId,vw_CRMOpportunity.N_FnYearID,vw_CRMOpportunity.N_OpportunityID, ISNULL(Inv_Customer.N_CustomerID,0) AS N_CustomerID, Inv_Customer.X_CustomerCode, Inv_Customer.X_CustomerName, " +
                "vw_CRMOpportunity.N_ContactID,vw_CRMOpportunity.X_Contact,vw_CRMOpportunity.X_ProjectName,vw_CRMOpportunity.N_CustomerID AS N_CrmCustomerID,vw_CRMOpportunity.N_WorkType,vw_CRMOpportunity.X_WorkType,vw_CRMOpportunity.N_WTaskID,vw_CRMOpportunity.X_WTask " +
                "FROM vw_CRMOpportunity LEFT OUTER JOIN Inv_Customer ON vw_CRMOpportunity.N_FnYearId = Inv_Customer.N_FnYearID AND vw_CRMOpportunity.N_CompanyId = Inv_Customer.N_CompanyID AND vw_CRMOpportunity.N_CustomerID = Inv_Customer.N_CrmCompanyID " +
                "where vw_CRMOpportunity.N_OpportunityID=@nOpportunityID and vw_CRMOpportunity.N_CompanyId=@nCompanyID and vw_CRMOpportunity.N_FnYearID=@YearID";
            }
           else{
                Params.Add("@xProjectCode", xProjectCode);
                Params.Add("@nCompanyID", nCompanyID);
                Params.Add("@YearID", nFnYearId);
                Params.Add("@nOpportunityID", nOpportunityID);
            sqlCommandText = "select * from Vw_InvCustomerProjects  where N_CompanyID=@nCompanyID and N_FnYearID=@YearID  and X_ProjectCode=@xProjectCode";
            sqlJob = "select * from Vw_JobDetails  where N_CompanyID=@nCompanyID and X_ProjectCode=@xProjectCode";
             jobMaster = dLayer.ExecuteDataTable(sqlJob, Params, connection);
                }
           
           
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    
                   
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                    int nProjectID = myFunctions.getIntVAL(dt.Rows[0]["N_ProjectID"].ToString());
                    string sqlOtherCost = "select * from vw_Inv_OtherCost where N_CompanyID=" + nCompanyID + " and N_TransID="+ nProjectID +"and X_TransType='Job File'";
                    otherCostDetails = dLayer.ExecuteDataTable(sqlOtherCost, Params, connection);
                    DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(dt.Rows[0]["N_ProjectID"].ToString()), myFunctions.getIntVAL(dt.Rows[0]["N_ProjectID"].ToString()), 74, 0, User, connection);
                    Attachments = api.Format(Attachments, "attachments");
                   
                        dt = api.Format(dt, "master");
                        otherCostDetails = api.Format(otherCostDetails, "otherCostDetails");
                        
                        jobMaster = api.Format(jobMaster, "jobMaster");
                        ds.Tables.Add(dt);
                        ds.Tables.Add(otherCostDetails);
                        ds.Tables.Add(jobMaster);
                        ds.Tables.Add(Attachments);

                        return Ok(api.Success(ds));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }

        }





        // [HttpGet("Details")]
        // public ActionResult GetCustomerProjectDetails(string xProjectCode, int nFnYearId, int nOpportunityID)

        // {
        //     DataTable dt = new DataTable();
        //     SortedList Params = new SortedList();
        //     int nCompanyID = myFunctions.GetCompanyID(User);
        //     if (xProjectCode == null) xProjectCode = "";
        //     string sqlCommandText = "";
        //     //sqlCommandText = "select 0 as N_ProjectID,'@Auto' as X_ProjectCode,* from vw_CRMOpportunity where N_OpportunityID=@nOpportunityID and N_CompanyID=@nCompanyID and N_FnYearID=@YearID";
        //     if (nOpportunityID > 0)
        //     {
        //         sqlCommandText = "select TOP 1 0 as N_ProjectID,'@Auto' as X_ProjectCode,vw_CRMOpportunity.N_CompanyId,vw_CRMOpportunity.N_FnYearID,vw_CRMOpportunity.N_OpportunityID, ISNULL(Inv_Customer.N_CustomerID,0) AS N_CustomerID, Inv_Customer.X_CustomerCode, Inv_Customer.X_CustomerName, " +
        //         "vw_CRMOpportunity.N_ContactID,vw_CRMOpportunity.X_Contact,vw_CRMOpportunity.X_ProjectName,vw_CRMOpportunity.N_CustomerID AS N_CrmCustomerID,vw_CRMOpportunity.N_WorkType,vw_CRMOpportunity.X_WorkType,vw_CRMOpportunity.N_WTaskID,vw_CRMOpportunity.X_WTask " +
        //         "FROM vw_CRMOpportunity LEFT OUTER JOIN Inv_Customer ON vw_CRMOpportunity.N_FnYearId = Inv_Customer.N_FnYearID AND vw_CRMOpportunity.N_CompanyId = Inv_Customer.N_CompanyID AND vw_CRMOpportunity.N_CustomerID = Inv_Customer.N_CrmCompanyID " +
        //         "where vw_CRMOpportunity.N_OpportunityID=@nOpportunityID and vw_CRMOpportunity.N_CompanyId=@nCompanyID and vw_CRMOpportunity.N_FnYearID=@YearID";
        //     }
          
        //     else
        //   sqlCommandText = "select * from Vw_InvCustomerProjects  where N_CompanyID=@nCompanyID and N_FnYearID=@YearID  and X_ProjectCode=@xProjectCode";
        //            DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(MasterTable.Rows[0]["N_ProjectID"].ToString()), myFunctions.getIntVAL(MasterTable.Rows[0]["N_ProjectID"].ToString()), 74, 0, User, connection);
        //             Attachments = _api.Format(Attachments, "attachments");
        //     Params.Add("@nCompanyID", nCompanyID);
        //     Params.Add("@YearID", nFnYearId);
        //     Params.Add("@xProjectCode", xProjectCode);
        //     Params.Add("@nOpportunityID", nOpportunityID);
            
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();

        //             dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
        //         }
        //         if (dt.Rows.Count == 0)
        //         {
        //             return Ok(api.Notice("No Results Found"));
        //         }
        //         else
        //         {
        //             return Ok(api.Success(dt));
        //         }

        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(api.Error(User, e));
        //     }
        // }
        [HttpGet("AccountList")]
        public ActionResult GetAccountList()
        {
            int nCompanyID = myFunctions.GetCompanyID(User);

            SortedList param = new SortedList() { { "@p1", nCompanyID } };

            DataTable dt = new DataTable();

            string sqlCommandText = "select * from prj_AccountSettings where N_CompanyID=@p1";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt = dLayer.ExecuteDataTable(sqlCommandText, param, connection);
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
        [HttpGet("stageupdate")]
        public ActionResult StageUpdate(string xstage, int nProjectID)
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
                    object N_StageID = dLayer.ExecuteScalar("select N_PkeyID from gen_lookuptable where n_companyID=" + nCompanyId + " and N_ReferID=1388 and X_Name='" + xstage + "'", Params, connection);
                    if (N_StageID != null)
                        dLayer.ExecuteNonQuery("update Inv_CustomerProjects set N_StageID=" + N_StageID.ToString() + " where N_CompanyID=@p1 and N_ProjectID=" + nProjectID, Params, connection);

                }
                return Ok(api.Success("Order Updated"));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }


        [HttpGet("dashboardlist")]
        public ActionResult ProjectList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nEmpID,int nUserID)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nEmpID", nEmpID);
            string UserPattern = myFunctions.GetUserPattern(User);
            string Pattern="";
            string Criterea="";
            string prjctCriterea="";
            // if (UserPattern != "")
            // {
            //     Pattern = " and Left(X_Pattern,Len(@p3))=@p3";
            //     Params.Add("@p3", UserPattern);
            // }
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";

            if(nUserID>0){
                 prjctCriterea=" or n_ProjectID in (select n_ProjectID from Tsk_ProjectSettingsDetails where n_UserID="+nUserID+" and b_view=1)";
            }

            if(nEmpID>0)
            {
                Criterea=" and x_EmpsID like '%"+nEmpID+"%' or n_ProjectCoordinator ="+nEmpID+" or n_ProjectManager="+nEmpID+"";
               

            }
          
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (x_projectcode like '%" + xSearchkey + "%'or x_projectname like'%" + xSearchkey + "%' or x_CustomerName like '%" + xSearchkey + "%' or x_District like '%" + xSearchkey + "%' or d_StartDate like '%" + xSearchkey + "%' or n_ContractAmt like '%" + xSearchkey + "%')";

            Searchkey = Searchkey + " and X_ProjectCode is not null ";
            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ProjectID desc";
            else
                xSortBy = " order by " + xSortBy;

                if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") X_ProjectCode,X_ProjectName,X_CustomerName,X_District,X_Name,N_StatusID,D_StartDate,N_ContractAmt,N_EstimateCost,AwardedBudget,ActualBudget,CommittedBudget,RemainingBudget,X_PO,N_Progress,N_CompanyID,N_Branchid,N_CustomerID,B_IsSaveDraft,B_Inactive,N_ProjectID,N_StageID,X_Stage,CONVERT(VARCHAR(10), D_EndDate,111) as D_EndDate,N_LastActionID,X_ClosingRemarks,x_TaskSummery,CONVERT(VARCHAR(10), D_DueDate,111) as D_DueDate,X_ProjectManager,N_ProjectCoordinator,N_ProjectManager,x_EmpsID from vw_InvProjectDashBoard where N_CompanyID=@p1 "+Criterea + prjctCriterea + Pattern  + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") X_ProjectCode,X_ProjectName,X_CustomerName,X_District,X_Name,N_StatusID,D_StartDate,N_ContractAmt,N_EstimateCost,AwardedBudget,ActualBudget,CommittedBudget,RemainingBudget,X_PO,N_Progress,N_CompanyID,N_Branchid,N_CustomerID,B_IsSaveDraft,B_Inactive,N_ProjectID,N_StageID,X_Stage,CONVERT(VARCHAR(10), D_EndDate,111) as D_EndDate,N_LastActionID,X_ClosingRemarks,x_TaskSummery,CONVERT(VARCHAR(10), D_DueDate,111) as D_DueDate,X_ProjectManager,N_ProjectCoordinator,N_ProjectManager,x_EmpsID from vw_InvProjectDashBoard where N_CompanyID=@p1 " +Criterea + prjctCriterea + Pattern + Searchkey + " and N_ProjectID not in (select top(" + Count + ") N_ProjectID from vw_InvProjectDashBoard where N_CompanyID=@p1 " + Pattern + Searchkey + xSortBy + " ) " + xSortBy;
               Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    string sqlCommandCount = "select count(1) as N_Count  from vw_InvProjectDashBoard where N_CompanyID=@p1 "+Criterea+" ";
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
    }
}



