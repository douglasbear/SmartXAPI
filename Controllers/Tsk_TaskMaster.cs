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

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("taskManager")]
    [ApiController]
    public class Tsk_TaskMaster : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;

        public Tsk_TaskMaster(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1056;
        }
        private readonly string connectionString;

        [HttpGet("list")]
        public ActionResult TaskList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy, bool byUser)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";
            string Criteria = "";
            string Criteria2 = "";
            if (byUser == true)
            {
                Criteria = " and N_AssigneeID=@nUserID ";
            }

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_TaskSummery like '%" + xSearchkey + "%' OR X_TaskDescription like '%" + xSearchkey + "%' OR X_Assignee like '%" + xSearchkey + "%' OR X_Submitter like '%" + xSearchkey + "%' OR X_ClosedUser like '%" + xSearchkey + "%'  OR X_ProjectName like '%" + xSearchkey + "%' )";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_TaskID desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Tsk_TaskCurrentStatus where N_CompanyID=@p1 " + Searchkey + Criteria + Criteria2 + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Tsk_TaskCurrentStatus where N_CompanyID=@p1 " + Searchkey + Criteria + Criteria2 + " and N_TaskID not in (select top(" + Count + ") N_TaskID from vw_Tsk_TaskCurrentStatus where N_CompanyID=@p1 " + Criteria + Criteria2 + xSortBy + " ) " + xSortBy;


            Params.Add("@p1", nCompanyId);
            Params.Add("@nUserID", nUserID);
            // Params.Add("@nFnYearId", nFnYearId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from vw_Tsk_TaskCurrentStatus where N_CompanyID=@p1 ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("parentTaskList")]
        public ActionResult ParentTaskList()
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();



            string sqlCommandText = "select  * from vw_Tsk_TaskCurrentStatus where N_CompanyID=@p1 and isnull(N_ParentID,0)=0";

            Params.Add("@p1", nCompanyId);
            Params.Add("@nUserID", nUserID);
            // Params.Add("@nFnYearId", nFnYearId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("details")]
        public ActionResult TaskDetails(string xTaskCode)
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
                    DataTable HistoryTable = new DataTable();
                    DataTable CommentsTable = new DataTable();


                    string Mastersql = "";
                    string DetailSql = "";
                    string HistorySql = "";
                    string CommentsSql = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@xTaskCode", xTaskCode);
                    Mastersql = "select * from vw_Tsk_TaskMaster where N_CompanyId=@nCompanyID and X_TaskCode=@xTaskCode  ";

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int TaskID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TaskID"].ToString());
                    Params.Add("@nTaskID", TaskID);
                    MasterTable = _api.Format(MasterTable, "Master");

                    //Detail
                    DetailSql = "select * from vw_Tsk_TaskCurrentStatus where N_CompanyId=@nCompanyID and N_TaskID=@nTaskID ";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");


                    //History
                    HistorySql = "select * from vw_Tsk_TaskStatus where N_CompanyId=@nCompanyID and N_TaskID=@nTaskID ";
                    HistoryTable = dLayer.ExecuteDataTable(HistorySql, Params, connection);
                    HistoryTable = _api.Format(HistoryTable, "History");


                    //Comments
                    CommentsSql = "select * from vw_Tsk_TaskComments where N_ActionID=@nTaskID ";
                    CommentsTable = dLayer.ExecuteDataTable(CommentsSql, Params, connection);
                    CommentsTable = _api.Format(CommentsTable, "Comments");

                    DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(MasterTable.Rows[0]["N_TaskID"].ToString()), myFunctions.getIntVAL(MasterTable.Rows[0]["N_TaskID"].ToString()), 1324, 0, User, connection);
                    Attachments = _api.Format(Attachments, "attachments");

                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    dt.Tables.Add(HistoryTable);
                    dt.Tables.Add(Attachments);
                    dt.Tables.Add(CommentsTable);

                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
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
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataTable Attachment = ds.Tables["attachments"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    SortedList Paramsforstatus = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nTaskId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TaskID"].ToString());
                    string X_TaskCode = MasterTable.Rows[0]["X_TaskCode"].ToString();
                    string xTaskSummery = MasterTable.Rows[0]["x_TaskSummery"].ToString();



                    //int nUserID = myFunctions.GetUserID(User);

                    if (nTaskId > 0)
                    {
                        dLayer.DeleteData("Tsk_TaskStatus", "N_TaskID", nTaskId, "", connection, transaction);
                        dLayer.DeleteData("Tsk_TaskMaster", "N_TaskID", nTaskId, "", connection, transaction);
                    }
                    DocNo = MasterRow["X_TaskCode"].ToString();
                    if (X_TaskCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", this.FormID);



                        while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Tsk_TaskMaster Where X_TaskCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_TaskCode = DocNo;


                        if (X_TaskCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate")); }
                        MasterTable.Rows[0]["X_TaskCode"] = X_TaskCode;


                    }

                    if (DetailTable.Rows[0]["N_AssigneeID"].ToString() != "0" && DetailTable.Rows[0]["N_AssigneeID"].ToString() != "")
                    {
                        if (DetailTable.Columns.Contains("X_Assignee"))
                            DetailTable.Columns.Remove("X_Assignee");
                    }
                    if (DetailTable.Rows[0]["n_ClosedUserID"].ToString() != "0" && DetailTable.Rows[0]["n_ClosedUserID"].ToString() != "")
                    {
                        if (DetailTable.Columns.Contains("x_ClosedUser"))
                            DetailTable.Columns.Remove("x_ClosedUser");
                    }
                    if (DetailTable.Rows[0]["n_SubmitterID"].ToString() != "0" && DetailTable.Rows[0]["n_SubmitterID"].ToString() != "")
                    {
                        if (DetailTable.Columns.Contains("x_Submitter"))
                            DetailTable.Columns.Remove("x_Submitter");
                    }

                    if (DetailTable.Rows[0]["N_AssigneeID"].ToString() == "0" || DetailTable.Rows[0]["N_AssigneeID"].ToString() == "")
                    {
                        DetailTable.Rows[0]["N_Status"] = 5;

                    }
                    else if (DetailTable.Rows[0]["N_AssigneeID"].ToString() != "0" || DetailTable.Rows[0]["N_AssigneeID"].ToString() != "")
                    {
                        DetailTable.Rows[0]["N_Status"] = 1;
                    }





                    nTaskId = dLayer.SaveData("Tsk_TaskMaster", "N_TaskID", MasterTable, connection, transaction);
                    if (nTaskId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    }

                    for (int i = 0; i < DetailTable.Rows.Count; i++)
                    {
                        DetailTable.Rows[0]["N_TaskID"] = nTaskId;
                    }
                    int nTaskStatusID = dLayer.SaveData("Tsk_TaskStatus", "N_TaskStatusID", DetailTable, connection, transaction);
                    if (nTaskStatusID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    }

                    if (Attachment.Rows.Count > 0)
                    {
                        try
                        {
                            myAttachments.SaveAttachment(dLayer, Attachment, X_TaskCode, nTaskId, xTaskSummery, X_TaskCode, nTaskId, "Task Document", User, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(ex));
                        }
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpPost("UpdateStatus")]
        public ActionResult UpdateStatus([FromBody] DataSet ds)
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
                    SortedList Params = new SortedList();
                    DataRow MasterRow = MasterTable.Rows[0];
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nTaskID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TaskID"].ToString());
                    string nStatus = DetailTable.Rows[0]["N_Status"].ToString();

                    // if(DetailTable.Rows[0]["N_AssigneeID"].ToString() == DetailTable.Rows[0]["N_SubmitterID"].ToString())
                    // {
                    //      DetailTable.Rows[0]["N_AssigneeID"] = DetailTable.Rows[0]["N_ClosedUserID"].ToString();


                    // }


                    if (nStatus == "3" && (DetailTable.Rows[0]["N_AssigneeID"].ToString() != DetailTable.Rows[0]["N_SubmitterID"].ToString()))
                    {
                        DetailTable.Rows[0]["N_AssigneeID"] = DetailTable.Rows[0]["N_SubmitterID"].ToString();

                    }
                    else if (nStatus == "3" && (DetailTable.Rows[0]["N_AssigneeID"].ToString() == DetailTable.Rows[0]["N_SubmitterID"].ToString()))
                    {
                        DetailTable.Rows[0]["N_AssigneeID"] = DetailTable.Rows[0]["N_ClosedUserID"].ToString();
                    }
                    else if (nStatus == "4")
                    {
                        DetailTable.Rows[0]["N_AssigneeID"] = 0;


                    }



                    for (int i = 0; i < DetailTable.Rows.Count; i++)
                    {
                        DetailTable.Rows[0]["N_TaskID"] = nTaskID;
                        DetailTable.Rows[0]["N_TaskStatusID"] = 0;
                    }

                    int nTaskStatusID = dLayer.SaveData("Tsk_TaskStatus", "N_TaskStatusID", DetailTable, connection, transaction);
                    if (nTaskStatusID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    }
                    if (nStatus == "4")
                    {

                        dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET B_Closed=1 where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), connection, transaction);
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpPost("attachmentSave")]
        public ActionResult AttachmentSave([FromBody] DataSet ds)
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
                    DataTable Attachment = ds.Tables["attachments"];
                    SortedList Params = new SortedList();
                    DataRow MasterRow = MasterTable.Rows[0];
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nTaskID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TaskID"].ToString());
                    string XTaskCode = MasterTable.Rows[0]["x_TaskCode"].ToString();
                    string xTaskSummery = MasterTable.Rows[0]["x_TaskSummery"].ToString();


                    if (Attachment.Rows.Count > 0)
                    {
                        try
                        {
                            myAttachments.SaveAttachment(dLayer, Attachment, XTaskCode, nTaskID, xTaskSummery, XTaskCode, nTaskID, "Task Document", User, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(ex));
                        }
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpPost("updateComments")]
        public ActionResult CommentsSave([FromBody] DataSet ds)
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
                    DataTable Comments = ds.Tables["comments"];
                    SortedList Params = new SortedList();
                    DataRow MasterRow = MasterTable.Rows[0];
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nCommentsID = myFunctions.getIntVAL(Comments.Rows[0]["N_CommentsID"].ToString());
                    int nUserID = myFunctions.GetUserID(User);
                    string xComments = (Comments.Rows[0]["x_Comments"].ToString());
                    Comments.Rows[0]["N_ActionID"] = MasterTable.Rows[0]["N_TaskID"].ToString();
                    Comments.Rows[0]["N_Creator"] = nUserID;

                    if (Comments.Rows.Count > 0)
                    {
                        nCommentsID = dLayer.SaveData("Tsk_TaskComments", "N_CommentsID", Comments, connection, transaction);
                        if (nCommentsID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable To Save"));
                        }

                    }
                    if (DetailTable.Rows.Count > 0)
                    {
                        int nTaskStatusID = dLayer.SaveData("Tsk_TaskStatus", "N_TaskStatusID", DetailTable, connection, transaction);
                        if (nTaskStatusID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable To Save"));
                        }




                    }
                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nTaskID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    dLayer.DeleteData("Tsk_TaskStatus", "N_TaskID", nTaskID, "", connection);
                    Results = dLayer.DeleteData("Tsk_TaskMaster", "N_TaskID", nTaskID, "", connection);
                    if (Results > 0)
                    {
                        return Ok(_api.Success("deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
          [HttpGet("calenderData")]
        public ActionResult GetcalenderData(bool byUser)

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            Params.Add("@nCompanyId", nCompanyID);
            Params.Add("@nUserID", nUserID);
            string Criteria = "";
             if (byUser == true)
            {
                Criteria = " and N_AssigneeID=@nUserID ";
            }

            string sqlCommandText = "Select X_TaskSummery as title,'true' as allDay,cast(D_TaskDate as Date) as start, dateadd(dd,1,cast(D_DueDate as date)) as 'end', N_TaskID,X_TaskCode  from vw_Tsk_TaskCurrentStatus Where N_CompanyID= " + nCompanyID +" " +Criteria;


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

                [HttpGet("updateDashboard")]
        public ActionResult UpdateDashboard(int nTaskID, int nStatus)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    SortedList Params = new SortedList();
                    object N_TaskStatusID1;

                    int N_CreatorID = myFunctions.GetUserID(User);
                    int N_ClosedUserID = myFunctions.GetUserID(User);
                    DateTime D_EntryDate = DateTime.Today;
                    DataTable DetailTable;



                    int N_TaskStatusID = 0;
                    Params.Add("N_CompanyID", nCompanyID);
                    N_TaskStatusID1 = dLayer.ExecuteScalar("select max(N_TaskStatusID) from Tsk_TaskStatus where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), Params, connection);

                    if (nStatus == 4)
                    {
                        dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET B_Closed=1 where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), Params, connection);
                    }

                    SqlTransaction transaction = connection.BeginTransaction();
                    string qry = "Select " + nCompanyID + " as N_CompanyID," + N_TaskStatusID + " as N_TaskStatusID," + nTaskID + " as N_TaskID," + 0 + " as N_AssigneeID," + 0 + " as N_SubmitterID ,'" + N_CreatorID + "' as  N_CreaterID,'" + D_EntryDate + "' as D_EntryDate,'" + "" + "' as X_Notes ," + nStatus + " as N_Status ," + 100 + " as N_WorkPercentage";
                    DetailTable = dLayer.ExecuteDataTable(qry, Params, connection, transaction);
                    int nID = dLayer.SaveData("Tsk_TaskStatus", "N_TaskStatusID", DetailTable, connection, transaction);
                    if (nID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    }
                    transaction.Commit();
                    return Ok(_api.Success(""));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

    }
}