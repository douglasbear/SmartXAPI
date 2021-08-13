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
    [Route("activity")]
    [ApiController]
    public class CRM_Activity : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        private readonly IMyReminders myReminders;

        public CRM_Activity(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyReminders myRem)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1307;
            myReminders = myRem;
        }


        [HttpGet("list")]
        public ActionResult ActivityList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy, bool bySalesMan)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Criteria = "";
            if (bySalesMan == true)
            {
                Criteria = " and N_UserID=@nUserID and isnull(B_Closed,0)<>1";
            }
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (x_subject like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by d_scheduleDate Desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRM_Activity where N_CompanyID=@p1 " + Searchkey + Criteria + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRM_Activity where N_CompanyID=@p1 " + Searchkey + Criteria + " and N_ActivityID not in (select top(" + Count + ") N_ActivityID from vw_CRM_Activity where N_CompanyID=@p1 " + Criteria + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);
            Params.Add("@nUserID", nUserID);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_CRM_Activity where N_CompanyID=@p1 " + Searchkey + Criteria;
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
                return Ok(api.Error(e));
            }
        }

        [HttpGet("details")]
        public ActionResult ActivityListDetails(string xActivityCode,int nopportunityID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_CRM_Activity where N_CompanyID=@p1 and X_ActivityCode=@p3";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p3", xActivityCode);
            object Company, Oppportunity, Contact, CustomerID;


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection,transaction);
                    if (nopportunityID > 0)
                    {
                        Oppportunity = dLayer.ExecuteScalar("select x_Opportunity from vw_CRMOpportunity where N_CompanyID =" + nCompanyId + " and N_OpportunityID=" + nopportunityID, Params, connection, transaction);
                        Contact = dLayer.ExecuteScalar("Select x_Contact from vw_CRMOpportunity where N_CompanyID=" + nCompanyId + " and N_OpportunityID=" + nopportunityID, Params, connection, transaction);
                        Company = dLayer.ExecuteScalar("select x_customer from vw_CRMOpportunity where N_CompanyID =" + nCompanyId + " and N_OpportunityID=" + nopportunityID, Params, connection, transaction);
                        CustomerID = dLayer.ExecuteScalar("select N_CustomerID from vw_CRMOpportunity where N_CompanyID =" + nCompanyId + " and N_OpportunityID=" + nopportunityID, Params, connection, transaction);


                        dt.Rows[0]["x_Body"] = dt.Rows[0]["x_Body"].ToString().Replace("@CompanyName", Company.ToString());
                        dt.Rows[0]["x_Body"] = dt.Rows[0]["x_Body"].ToString().Replace("@ContactName", Contact.ToString());
                        dt.Rows[0]["x_Body"] = dt.Rows[0]["x_Body"].ToString().Replace("@LeadName", Oppportunity.ToString());

                        dt.Rows[0]["x_TempSubject"] = dt.Rows[0]["x_TempSubject"].ToString().Replace("@CompanyName", Company.ToString());
                        dt.Rows[0]["x_TempSubject"] = dt.Rows[0]["x_TempSubject"].ToString().Replace("@ContactName", Contact.ToString());
                        dt.Rows[0]["x_TempSubject"] = dt.Rows[0]["x_TempSubject"].ToString().Replace("@LeadName", Oppportunity.ToString());
                        dt.AcceptChanges();

                    }
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
                return Ok(api.Error(e));
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
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nActivityID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ActivityID"].ToString());
                string bClosed = MasterTable.Rows[0]["b_Closed"].ToString();
                string xStatus = MasterTable.Rows[0]["X_Status"].ToString();
                int nUserID = myFunctions.GetUserID(User);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string ActivityCode = "";
                    var values = MasterTable.Rows[0]["x_ActivityCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.FormID);
                        ActivityCode = dLayer.GetAutoNumber("CRM_Activity", "x_ActivityCode", Params, connection, transaction);
                        if (ActivityCode == "") { transaction.Rollback(); return Ok(api.Error("Unable to generate Activity Code")); }
                        MasterTable.Rows[0]["x_ActivityCode"] = ActivityCode;
                    }
                    if (MasterTable.Rows[0]["N_RelatedTo"].ToString() == "294")
                    {
                        if (nActivityID == 0)
                        {
                            object Count = dLayer.ExecuteScalar("select MAX(isnull(N_Order,0)) from crm_activity where N_ReffID=" + MasterTable.Rows[0]["N_ReffID"].ToString(), Params, connection, transaction);
                            if (Count != null)
                            {

                                int NOrder = myFunctions.getIntVAL(Count.ToString()) + 1;
                                dLayer.ExecuteNonQuery("update crm_activity set N_Order" + NOrder + " where N_Order" + Count, Params, connection, transaction);
                                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_Order", typeof(int), 0);
                                MasterTable.Rows[0]["N_Order"] = Count.ToString();
                            }
                        }

                    }

                    nActivityID = dLayer.SaveData("CRM_Activity", "n_ActivityID", MasterTable, connection, transaction);
                    if (nActivityID > 0)
                    {
                        try
                        {
                            myReminders.ReminderDelete(dLayer, nActivityID, this.FormID, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(api.Error("Unable to save"));
                        }
                    }


                    if (nActivityID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        DataTable dtSave = new DataTable();
                        dtSave.Clear();
                        dtSave.Columns.Add("N_CompanyID");
                        dtSave.Columns.Add("N_FormID");
                        dtSave.Columns.Add("N_PartyID");
                        dtSave.Columns.Add("X_Subject");
                        dtSave.Columns.Add("X_Title");
                        dtSave.Columns.Add("D_ExpiryDate");
                        dtSave.Columns.Add("N_RemCategoryID");
                        dtSave.Columns.Add("B_IsAttachment");
                        dtSave.Columns.Add("N_SettingsID");
                        dtSave.Columns.Add("N_UserID");
                        dtSave.Columns.Add("N_ReminderId");

                        DataRow row = dtSave.NewRow();
                        row["N_ReminderId"] = 0;
                        row["N_CompanyID"] = myFunctions.GetCompanyID(User);
                        row["N_FormID"] = this.FormID;
                        row["N_PartyID"] = nActivityID;
                        row["X_Subject"] = "Activity";
                        row["X_Title"] = "Activity";
                        row["D_ExpiryDate"] = MasterTable.Rows[0]["D_ScheduleDate"].ToString();
                        row["B_IsAttachment"] = 0;
                        row["N_SettingsID"] = 0;
                        row["N_UserID"] = nUserID;
                        if (MasterTable.Columns.Contains("B_IsReminder"))
                        {
                            if (MasterTable.Rows[0]["B_IsReminder"].ToString() == "True")
                            {
                                row["N_RemCategoryID"] = MasterTable.Rows[0]["N_ReminderCategoryID"].ToString();
                            }
                            dtSave.Rows.Add(row);
                            myReminders.ReminderSave(dLayer, dtSave, connection, transaction);
                        }
                        if (MasterTable.Columns.Contains("b_IsAutoMail"))
                        {
                            DataRow row1 = dtSave.NewRow();
                            row1["N_ReminderId"] = 0;
                            row1["N_CompanyID"] = myFunctions.GetCompanyID(User);
                            row1["N_FormID"] = this.FormID;
                            row1["N_PartyID"] = nActivityID;
                            row1["X_Subject"] = "Activity";
                            row1["X_Title"] = "Activity";
                            row1["D_ExpiryDate"] = MasterTable.Rows[0]["D_ScheduleDate"].ToString();
                            row1["B_IsAttachment"] = 0;
                            row1["N_SettingsID"] = 0;
                            row1["N_UserID"] = nUserID;
                            if (MasterTable.Rows[0]["b_IsAutoMail"].ToString() == "True")
                            {
                                row1["N_RemCategoryID"] = MasterTable.Rows[0]["N_ScheduleCategoryID"].ToString();
                            }
                            dtSave.Rows.Add(row1);
                            myReminders.ReminderSave(dLayer, dtSave, connection, transaction);
                        }






                        // if (MasterTable.Rows[0]["B_IsReminder"].ToString() == "True")
                        //     myReminders.ReminderSet(dLayer, 24, nActivityID, MasterTable.Rows[0]["D_ScheduleDate"].ToString(), this.FormID, nUserID, User, connection, transaction);
                        // if (MasterTable.Rows[0]["B_IsAutoMail"].ToString() == "True")
                        //     myReminders.ReminderSet(dLayer, 25, nActivityID, MasterTable.Rows[0]["D_ScheduleDate"].ToString(), this.FormID, nUserID, User, connection, transaction);

                        transaction.Commit();
                        if (bClosed == "1" && xStatus == "Closed")
                        {
                            return Ok(api.Success("Activity Closed"));
                        }
                        else
                        {
                            return Ok(api.Success("Activity Created"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nActivityID)
        {

            int Results = 0;
            try
            {


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("CRM_Activity", "N_ActivityID", nActivityID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("N_ActivityID", nActivityID.ToString());
                    return Ok(api.Success(res, "Activity deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete Activity"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }

        }
          [HttpGet("calenderData")]
        public ActionResult GetcalenderData(bool bySalesMan)

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            Params.Add("@nCompanyId", nCompanyID);
            Params.Add("@nUserID", nUserID);
            string Criteria = "";
            if (bySalesMan == true)
            {
                Criteria = " and N_UserID=@nUserID and isnull(B_Closed,0)<>1";
            }
            string sqlCommandText = "Select X_Subject as title,'true' as allDay,cast(D_ScheduleDate as Date) as start,cast(D_ScheduleDate as Date) as 'end' from vw_CRM_Activity Where N_CompanyID= " + nCompanyID +" " +Criteria;


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
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
    }
}