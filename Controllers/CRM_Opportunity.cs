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
        public ActionResult OpportunityList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int nCompanyId = myFunctions.GetCompanyID(User);
            string UserPattern = myFunctions.GetUserPattern(User);
            string Pattern = "";
            if (UserPattern != "")
            {
                Pattern = " and Left(X_Pattern,Len(@p2))=@p2";
                Params.Add("@p2", UserPattern);
            }
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and (X_Opportunity like'%" + xSearchkey + "%'or X_OpportunityCode like'%" + xSearchkey + "%'or N_ExpRevenue like'%" + xSearchkey + "%'or X_Stage like'%" + xSearchkey + "%'or d_ClosingDate like'%" + xSearchkey + "%'or x_nextStep like'%" + xSearchkey + "%'or x_Contact like'%" + xSearchkey + "%'or x_Mobile like'%" + xSearchkey + "%'or x_SalesmanName like'%" + xSearchkey + "%'or x_StatusType like'%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_OpportunityID desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMOpportunity where N_CompanyID=@p1 and isnull(N_ClosingStatusID,0) = 0 " + Pattern + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMOpportunity where N_CompanyID=@p1 and isnull(N_ClosingStatusID,0) = 0 " + Pattern + Searchkey + " and N_OpportunityID not in (select top(" + Count + ") N_OpportunityID from vw_CRMOpportunity where N_CompanyID=@p1 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_CRMOpportunity where N_CompanyID=@p1 and  isnull(N_ClosingStatusID,0) = 0 " + Pattern;
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
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("details")]
        public ActionResult OpportunityListDetails(string xOpportunityCode)
        {
            DataTable dt = new DataTable();
            DataTable dtItem = new DataTable();
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
                }
                dt = api.Format(dt);
                Result.Add("Details", dt);
                Result.Add("Items", dtItem);
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
                DataTable MasterTable, Items, Activity, Participants;
                MasterTable = ds.Tables["master"];
                Items = ds.Tables["Items"];
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
                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_OpportunityID='" + nOpportunityID + "' and N_FnyearID=" + nFnYearId;
                    if (nOpportunityID > 0)
                        N_WorkFlowID = dLayer.ExecuteScalar("select N_WactivityID from CRM_Opportunity where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and N_OpportunityID=" + nOpportunityID, Params, connection, transaction);
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
                                    dLayer.DeleteData("CRM_Activity", "N_ReffID", nOpportunityID, "", connection, transaction);
                                    Activity = dLayer.ExecuteDataTable("select * from CRM_WorkflowActivities where N_CompanyID=" + nCompanyID + " and N_WActivityID=" + nWActivityID + " order by N_Order", Params, connection, transaction);
                                    if (Activity.Rows.Count > 0)
                                    {
                                        SortedList AParams = new SortedList();
                                        Activity = myFunctions.AddNewColumnToDataTable(Activity, "X_ContactEmail", typeof(string), "");
                                        Activity = myFunctions.AddNewColumnToDataTable(Activity, "X_ContactNumber", typeof(string), "");
                                        Activity = myFunctions.AddNewColumnToDataTable(Activity, "x_ActivityCode", typeof(string), "");
                                        Activity = myFunctions.AddNewColumnToDataTable(Activity, "N_TaskOwner", typeof(int), 0);
                                        Activity = myFunctions.AddNewColumnToDataTable(Activity, "n_ActivityID", typeof(int), 0);
                                        Activity = myFunctions.AddNewColumnToDataTable(Activity, "X_Contact", typeof(string), "");
                                        string ActivityCode = "";
                                        AParams.Add("N_CompanyID", nCompanyID);
                                        AParams.Add("N_YearID", nFnYearId);
                                        AParams.Add("N_FormID", 1307);
                                        Activity.Columns.Remove("N_WActivityID");
                                        Activity.Columns.Remove("N_WActivityDetailID");
                                        int Order = 1;
                                        double Minuts = 0;
                                        object Contact = dLayer.ExecuteScalar("select X_Contact from crm_Contact where N_ContactID=" + nContactID, Params, connection, transaction);
                                        foreach (DataRow var in Activity.Rows)
                                        {
                                            ActivityCode = dLayer.GetAutoNumber("CRM_Activity", "x_ActivityCode", AParams, connection, transaction);
                                            if (ActivityCode == "") { transaction.Rollback(); return Ok(api.Error(User, "Unable to generate Activity Code")); }
                                            var["x_ActivityCode"] = ActivityCode;
                                            var["N_RelatedTo"] = 294;
                                            var["N_ReffID"] = nOpportunityID;
                                            var["X_ContactEmail"] = X_ContactEmail;
                                            var["X_ContactNumber"] = X_ContactNumber;
                                            var["N_TaskOwner"] = nTaskOwner;
                                            var["n_ActivityID"] = 0;
                                            var["N_Order"] = Order;
                                            var["X_Contact"] = Contact;
                                            if (Order == 1)
                                            {
                                                var["b_closed"] = 1;
                                                var["x_status"] = "Closed";
                                            }
                                            if (var["N_ReminderUnitID"].ToString() == "248")
                                                Minuts = 1;
                                            else if (var["N_ReminderUnitID"].ToString() == "247")
                                                Minuts = 60;
                                            else if (var["N_ReminderUnitID"].ToString() == "246")
                                                Minuts = 1440;
                                            else
                                                Minuts = 10080;

                                            if (var["N_ReminderBrfore"].ToString() == "0")
                                                Minuts = 0;
                                            else
                                                Minuts = myFunctions.getVAL(var["N_ReminderBrfore"].ToString()) * Minuts;

                                            var["D_ScheduleDate"] = DateTime.Now.AddMinutes(Minuts);
                                            Order++;
                                        }
                                        dLayer.SaveData("CRM_Activity", "n_ActivityID", Activity, connection, transaction);
                                    }
                                }

                            }

                        }
                   


                        dLayer.SaveData("Crm_Products", "N_CrmItemID", Items, connection, transaction);

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
                    Results = dLayer.DeleteData("Crm_Products", "N_OpportunityID", nOpportunityID, "", connection, transaction);
                    Results = dLayer.DeleteData("CRM_Activity", "N_ReffID", nOpportunityID, "", connection, transaction);
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