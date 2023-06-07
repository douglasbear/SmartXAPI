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
        public ActionResult TaskList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy, bool byUser, bool b_assign, bool b_reAssign, bool b_Closed)
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
                Searchkey = "and (X_TaskCode like '%" + xSearchkey + "%' OR X_TaskSummery like '%" + xSearchkey + "%' OR X_TaskDescription like '%" + xSearchkey + "%' OR X_Assignee like '%" + xSearchkey + "%' OR X_Submitter like '%" + xSearchkey + "%' OR X_ClosedUser like '%" + xSearchkey + "%'  OR X_ProjectName like '%" + xSearchkey + "%' )";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_TaskID desc";
            else
                xSortBy = " order by " + xSortBy;
            if (byUser == true)
            {
                if (Count == 0)
                    sqlCommandText = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus] where N_CompanyID=@p1  " + Searchkey + Criteria + Criteria2 + xSortBy;
                else
                    sqlCommandText = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus] where N_CompanyID=@p1" + Searchkey + Criteria + Criteria2 + " and N_TaskID not in (select top(" + Count + ") N_TaskID from [vw_TaskCurrentStatus] where N_CompanyID=@p1 " + Criteria + Criteria2 + xSortBy + " ) " + xSortBy;
            }
            else
            {
                if (b_assign)
                {
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus] where N_CompanyID=@p1 and N_CreaterID=@nUserID  " + Searchkey + Criteria + Criteria2 + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus] where N_CompanyID=@p1 and N_CreaterID=@nUserID  " + Searchkey + Criteria + Criteria2 + " and N_TaskID not in (select top(" + Count + ") N_TaskID from [vw_TaskCurrentStatus] where N_CompanyID=@p1 and and N_CreaterID=@nUserID " + Criteria + Criteria2 + xSortBy + " ) " + xSortBy;

                }
                if (b_reAssign)
                {
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus] where N_CompanyID=@p1 and N_CreaterID=@nUserID and  X_ActionName='Re Assign'" + Searchkey + Criteria + Criteria2 + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus] where N_CompanyID=@p1 and N_CreaterID=@nUserID and  X_ActionName='Re Assign'" + Searchkey + Criteria + Criteria2 + " and N_TaskID not in (select top(" + Count + ") N_TaskID from [vw_TaskCurrentStatus] where N_CompanyID=@p1 and and N_CreaterID=@nUserID and X_ActionName='Re Assign' " + Criteria + Criteria2 + xSortBy + " ) " + xSortBy;

                }
                if (b_Closed)
                {
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus] where N_CompanyID=@p1 and N_CreaterID=@nUserID and B_Closed =1 " + Searchkey + Criteria + Criteria2 + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from [vw_TaskCurrentStatus] where N_CompanyID=@p1 and N_CreaterID=@nUserID and B_Closed =1" + Searchkey + Criteria + Criteria2 + " and N_TaskID not in (select top(" + Count + ") N_TaskID from [vw_TaskCurrentStatus] where N_CompanyID=@p1 and and N_CreaterID=@nUserID and B_Closed =1 " + Criteria + Criteria2 + xSortBy + " ) " + xSortBy;

                }
            }



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
                    sqlCommandCount = "select count(1) as N_Count  from [vw_TaskCurrentStatus] where N_CompanyID=@p1 ";
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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("parentTaskList")]
        public ActionResult ParentTaskList( int nProjectID)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText="";

            if(nProjectID >0){
                 sqlCommandText = "select  * from [vw_TaskCurrentStatus] where N_CompanyID=@p1 and N_ProjectID=@p2";
            }
            else{
               sqlCommandText = "select  * from [vw_TaskCurrentStatus] where N_CompanyID=@p1";
            }
            Params.Add("@p1", nCompanyId);
            Params.Add("@nUserID", nUserID);
            Params.Add("@p2", nProjectID);
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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("details")]
        public ActionResult TaskDetails(string xTaskCode, int nTaskID)
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
                    DataTable TimeTable = new DataTable();
                    DataTable options = new DataTable();
                    DataTable TasksList = new DataTable();
                    DataTable Materials = new DataTable();
                    int loginUserID = myFunctions.GetUserID(User);


                    string Mastersql = "";
                    string DetailSql = "";
                    string HistorySql = "";
                    string CommentsSql = "";
                    string ActionSql = "";
                    string nextAction = "";
                    string timeSql = "";


                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    if (nTaskID > 0)
                    {
                        Params.Add("@xTaskCode", nTaskID);
                        Mastersql = "select * from vw_Tsk_TaskMaster where N_CompanyId=@nCompanyID and N_TaskID=@xTaskCode  ";
                    }
                    else
                    {
                        Params.Add("@xTaskCode", xTaskCode);
                        Mastersql = "select * from vw_Tsk_TaskMaster where N_CompanyId=@nCompanyID and X_TaskCode=@xTaskCode  ";
                    }



                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_Comments", typeof(string), "");


                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int TaskID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TaskID"].ToString());
                    object comments = dLayer.ExecuteScalar("select  isnull(MAX(N_CommentsID),0) from Tsk_TaskComments where N_ActionID=" + myFunctions.getIntVAL(MasterTable.Rows[0]["N_TaskID"].ToString()), Params, connection);
                    if (comments == null) comments = 0;
                    if (myFunctions.getIntVAL(comments.ToString()) > 0)
                    {
                        object x_Comments = dLayer.ExecuteScalar("select  X_Comments from Tsk_TaskComments where N_CommentsID=" + myFunctions.getIntVAL(comments.ToString()), Params, connection);
                        MasterTable.Rows[0]["x_Comments"] = x_Comments.ToString();
                    }
                    MasterTable.AcceptChanges();
                    Params.Add("@nTaskID", TaskID);
                    MasterTable = _api.Format(MasterTable, "Master");
                    string subTasksList = "select * from vw_TaskCurrentStatus where N_CompanyID=@nCompanyID  and  N_ParentID=@nTaskID order by N_SortID";

                    //

                    // Materials
                    string ReqMaterialsSql = "select * from vw_Inv_ServiceMaterials where N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_TaskID=" + TaskID;
                    Materials = dLayer.ExecuteDataTable(ReqMaterialsSql, Params, connection);
                    Materials = _api.Format(Materials, "Materials");
                    // TimeTable
                    timeSql = "select * from vw_Tsk_TaskStatus where N_CompanyId=@nCompanyID and N_TaskID=" + TaskID + " ";
                    TimeTable = dLayer.ExecuteDataTable(timeSql, Params, connection);
                    double seconds = 0;
                    double Individualseconds = 0;

                    DateTime entryDateHold = new DateTime();
                    DateTime entryDateStart = new DateTime();
                    DateTime entryDateComplete = new DateTime();
                    int timeflag=0;
                    foreach (DataRow row in TimeTable.Rows)
                    {

                
                        if (row["N_Status"].ToString() == "7" && row["N_CreaterID"].ToString() == loginUserID.ToString())
                        { 
                            timeflag=1;
                            entryDateStart = Convert.ToDateTime(row["d_EntryDate"].ToString());

                        }
                        if (row["N_Status"].ToString() == "6" && row["N_CreaterID"].ToString() == loginUserID.ToString())
                        {
                            entryDateHold = Convert.ToDateTime(row["d_EntryDate"].ToString());
                        }
                        if (row["N_Status"].ToString() == "4" && row["N_CreaterID"].ToString() == loginUserID.ToString())
                        {
                            entryDateComplete = Convert.ToDateTime(row["d_EntryDate"].ToString());
                        }
                        if (row["N_Status"].ToString() == "6" && row["N_CreaterID"].ToString() == loginUserID.ToString())
                        {
                            Individualseconds = Individualseconds + (entryDateHold - entryDateStart).TotalSeconds;
                        }
                        else if (row["N_Status"].ToString() != "7" && row["N_Status"].ToString() != "6" && row["N_Status"].ToString() == "4" && row["N_CreaterID"].ToString() == loginUserID.ToString())
                        {
                            Individualseconds = Individualseconds + (entryDateComplete - entryDateStart).TotalSeconds;
                        }

                    }
                    foreach (DataRow row in TimeTable.Rows)
                    {


                        if (row["N_Status"].ToString() == "7")
                        {
                            entryDateStart = Convert.ToDateTime(row["d_EntryDate"].ToString());

                        }
                        if (row["N_Status"].ToString() == "6")
                        {
                            entryDateHold = Convert.ToDateTime(row["d_EntryDate"].ToString());
                        }
                        if (row["N_Status"].ToString() == "4")
                        {
                            entryDateComplete = Convert.ToDateTime(row["d_EntryDate"].ToString());
                        }
                        if (row["N_Status"].ToString() == "6")
                        {
                            seconds = seconds + (entryDateHold - entryDateStart).TotalSeconds;
                        }
                        else if (row["N_Status"].ToString() != "7" && row["N_Status"].ToString() != "6" && row["N_Status"].ToString() == "4")
                        {
                            seconds = seconds + (entryDateComplete - entryDateStart).TotalSeconds;
                        }

                    }

                    if (timeflag==0){
                        Individualseconds=0;
                        seconds=0;
                    }

                    //    TimeSpan t = TimeSpan.FromSeconds(seconds);
                    //  string answer = string.Format("{0:D2}h:{1:D2}m",
                    //                  t.Hours,
                    //                  t.Minutes);



                    //Detail
                    DetailSql = "select * from [vw_TaskCurrentStatus] where N_CompanyId=@nCompanyID and N_TaskID=@nTaskID ";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    if (DetailTable.Rows.Count > 0)
                    {
                        nextAction = DetailTable.Rows[0]["X_NextActions"].ToString();
                        DetailTable = myFunctions.AddNewColumnToDataTable(DetailTable, "seconds", typeof(double), seconds);
                        DetailTable = myFunctions.AddNewColumnToDataTable(DetailTable, "Individualseconds", typeof(double), Individualseconds);
                    }


                    DetailTable = _api.Format(DetailTable, "Details");






                    //History
                    HistorySql = "select * from (select N_TaskID,N_CreaterID, D_EntryDate,X_HistoryText,X_Assignee,D_DueDate,D_TaskDate,X_Creator,N_Status from vw_Tsk_TaskStatus  where N_CompanyId=@nCompanyID and N_TaskID=" + TaskID + " " +
                     "union all " +
                     "select N_ActionID as N_TaskID ,N_Creator as N_CreaterID,D_EntryDate,'Commented by #CREATOR on #TIME - ' + X_Comments as X_HistoryText,'' as x_Assignee,GETDATE() as D_DueDate,GETDATE() as D_TaskDate,X_UserName as X_Creator,'' as N_Status  from vw_Tsk_TaskComments  where N_ActionID=" + TaskID + "  ) as temptable order by D_EntryDate";
                    HistoryTable = dLayer.ExecuteDataTable(HistorySql, Params, connection);



                    foreach (DataRow row in HistoryTable.Rows)
                    {
                        object creator = row["x_Creator"];

                        string duettime = row["d_DueDate"].ToString();
                        DateTime _date;
                        string day = "";
                        if (duettime != null && duettime != "")
                        {
                            _date = DateTime.Parse(duettime);
                            day = _date.ToString("dd-MMM-yyyy HH:mm tt");
                        }
                        string time = row["d_EntryDate"].ToString();
                        DateTime _date1;
                        string day1 = "";
                        if (time != null && time != "")
                        {
                            _date1 = DateTime.Parse(time.ToString());
                            day1 = _date1.ToString("dd-MMM-yyyy  HH:mm tt");
                        }


                        object assignee = row["x_Assignee"].ToString();

                        if (row["x_HistoryText"].ToString().Contains("#CREATOR"))
                        {
                            row["x_HistoryText"] = row["x_HistoryText"].ToString().Replace("#CREATOR", creator.ToString());
                        }
                        if (row["x_HistoryText"].ToString().Contains("#TIME"))
                        {
                            row["x_HistoryText"] = row["x_HistoryText"].ToString().Replace("#TIME", day1);
                        }
                        if (row["x_HistoryText"].ToString().Contains("#DUEDATE"))
                        {
                            row["x_HistoryText"] = row["x_HistoryText"].ToString().Replace("#DUEDATE", day);
                        }
                        if (row["x_HistoryText"].ToString().Contains("#ASSIGNEE"))
                        {
                            row["x_HistoryText"] = row["x_HistoryText"].ToString().Replace("#ASSIGNEE", assignee.ToString());
                        }
                        //  if (row["x_HistoryText"].ToString().Contains("#STOPTIME"))
                        // {
                        //     row["x_HistoryText"] = row["x_HistoryText"].ToString().Replace("#STOPTIME", answer );
                        // }


                    }
                    HistoryTable.AcceptChanges();
                    HistoryTable = _api.Format(HistoryTable, "History");



                    //Comments
                    CommentsSql = "select * from vw_Tsk_TaskComments where N_ActionID=@nTaskID ";
                    CommentsTable = dLayer.ExecuteDataTable(CommentsSql, Params, connection);
                    CommentsTable = _api.Format(CommentsTable, "Comments");

                    //ActionTable
                    if (nextAction == "")
                    {
                        nextAction = "0";
                    }
                    ActionSql = "select N_ActionID,X_ActionName as title ,B_SysDefault from Tsk_TaskAction Where N_ActionID in (" + nextAction + ") ";
                    options = dLayer.ExecuteDataTable(ActionSql, Params, connection);
                    options = _api.Format(options, "options");

                    TasksList = dLayer.ExecuteDataTable(subTasksList, Params, connection);
                    TasksList = _api.Format(TasksList, "tasksList");




                    DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(MasterTable.Rows[0]["N_TaskID"].ToString()), myFunctions.getIntVAL(MasterTable.Rows[0]["N_TaskID"].ToString()), 1324, 0, User, connection);
                    Attachments = _api.Format(Attachments, "attachments");

                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    dt.Tables.Add(HistoryTable);
                    dt.Tables.Add(Attachments);
                    dt.Tables.Add(CommentsTable);
                    dt.Tables.Add(options);
                    dt.Tables.Add(TasksList);
                    dt.Tables.Add(Materials);

                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
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
                    DataTable MaterialTable;
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    MaterialTable = ds.Tables["materials"];
                    DataTable Attachment = ds.Tables["attachments"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    SortedList Paramsforstatus = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nTaskId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TaskID"].ToString());
                    string X_TaskCode = MasterTable.Rows[0]["X_TaskCode"].ToString();
                    string xTaskSummery = MasterTable.Rows[0]["x_TaskSummery"].ToString();
                    int nProjectID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ProjectID"].ToString());
                    int nParentyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ParentID"].ToString());
                    DataTable SavedData;


                    if (DetailTable.Columns.Contains("x_WorkPercentage"))
                    {
                        DetailTable.Columns.Remove("x_WorkPercentage");
                    }


                    //int nUserID = myFunctions.GetUserID(User);

                    if (nTaskId > 0)
                    {
                        dLayer.DeleteData("Tsk_TaskStatus", "N_TaskID", nTaskId, "", connection, transaction);
                        dLayer.DeleteData("Tsk_TaskMaster", "N_TaskID", nTaskId, "", connection, transaction);
                    }

                    //Percentage Calculation



                    //     if(nParentyID >0)
                    //     {
                    //         double totalweightage= myFunctions.getVAL(dLayer.ExecuteScalar("select sum(N_WeightPercentage) from Tsk_TaskMaster  where N_ParentID="+nParentyID+" and N_CompanyID="+nCompanyID, Params, connection,transaction).ToString());
                    //         ParentTaskPercentage= myFunctions.getVAL(dLayer.ExecuteScalar("select sum(N_CompletedPercentage) from Tsk_TaskMaster  where N_TaskID="+nParentyID+" and N_CompanyID="+nCompanyID, Params, connection,transaction).ToString());
                    //         ParentTaskPercentage=(((totalweightage+n_WeightPercentage) *100 )/ParentTaskPercentage );
                    //         ParentTaskPercentage = 0 ;
                    //           dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET N_CompletedPercentage= "+ParentTaskPercentage+"  where N_CompanyID=" + nCompanyID + " and N_TaskID=" + nParentyID, Params, connection,transaction);


                    //    }


                    DocNo = MasterRow["X_TaskCode"].ToString();
                    if (X_TaskCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", this.FormID);



                        while (true)
                        {

                            object N_Result = dLayer.ExecuteScalar("Select 1 from Tsk_TaskMaster Where X_TaskCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null) DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            break;
                        }
                        X_TaskCode = DocNo;


                        if (X_TaskCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate")); }
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
                        DetailTable.Rows[0]["N_Status"] = 1;
                        MasterTable.Rows[0]["N_StatusID"] = 2;

                    }
                    else if (DetailTable.Rows[0]["N_AssigneeID"].ToString() != "0" || DetailTable.Rows[0]["N_AssigneeID"].ToString() != "")
                    {
                        DetailTable.Rows[0]["N_Status"] = 2;
                        MasterTable.Rows[0]["N_StatusID"] = 2;




                    }

                    MasterTable.Rows[0]["N_StatusID"] = DetailTable.Rows[0]["N_Status"].ToString();



                    if (nProjectID > 0)
                    {
                        if (nTaskId == 0)
                        {
                            object Count = dLayer.ExecuteScalar("select isnull(MAX(N_Order),0) from tsk_taskmaster where N_CompanyID=" + nCompanyID + " and N_ProjectID=" + MasterTable.Rows[0]["N_ProjectID"].ToString(), Params, connection, transaction);
                            if (Count != null)
                            {

                                int NOrder = myFunctions.getIntVAL(Count.ToString()) + 1;
                                dLayer.ExecuteNonQuery("update tsk_taskmaster set N_Order=" + NOrder + " where N_CompanyID=" + nCompanyID + " and N_Order=" + Count + " and N_ProjectID=" + MasterTable.Rows[0]["N_ProjectID"].ToString(), Params, connection, transaction);
                                if (!MasterTable.Columns.Contains("N_Order"))
                                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_Order", typeof(int), 0);
                                MasterTable.Rows[0]["N_Order"] = Count.ToString();
                            }
                        }

                    }

                    nTaskId = dLayer.SaveData("Tsk_TaskMaster", "N_TaskID", MasterTable, connection, transaction);
                    if (nTaskId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }
                    if (MaterialTable.Rows.Count > 0)
                    {
                        for (int i = 0; i < MaterialTable.Rows.Count; i++)
                        {
                            MaterialTable.Rows[i]["n_TaskID"] = nTaskId;
                        }

                        dLayer.SaveData("Inv_ServiceMaterials", "n_MaterialID", MaterialTable, connection, transaction);
                    }

                    else if (DetailTable.Rows[0]["N_AssigneeID"].ToString() != "0" && DetailTable.Rows[0]["N_AssigneeID"].ToString() != "")
                    {
                        DataRow row = DetailTable.NewRow();
                        row["N_TaskID"] = nTaskId;
                        row["n_TaskStatusID"] = 0;
                        row["n_CompanyId"] = nCompanyID;
                        row["n_AssigneeID"] = 0;
                        row["n_CreaterID"] = myFunctions.GetUserID(User);
                        row["n_SubmitterID"] = 0;
                        row["n_ClosedUserID"] = 0;
                        row["n_Status"] = 1;
                        row["d_EntryDate"] = DetailTable.Rows[0]["d_EntryDate"];
                        DetailTable.Rows.InsertAt(row, 0);
                        //  dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET N_StatusID= 1  where N_CompanyID=" + nCompanyID + " and N_TaskID=" + nTaskId, Params, connection,transaction);
                    }



                    for (int i = 0; i < DetailTable.Rows.Count; i++)
                    {
                        DetailTable.Rows[i]["N_TaskID"] = nTaskId;
                    }
                    int nTaskStatusID = dLayer.SaveData("Tsk_TaskStatus", "N_TaskStatusID", DetailTable, connection, transaction);
                    if (nTaskStatusID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
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
                            return Ok(_api.Error(User, ex));
                        }
                    }

                    if (nParentyID > 0)
                    {


                        string sql = "select * from Vw_TaskCurrentStatus where N_CompanyID=" + nCompanyID + " and N_ParentID =" + nParentyID + "";
                        SavedData = dLayer.ExecuteDataTable(sql, Params, connection, transaction);
                        // object count = dLayer.ExecuteScalar("select  N_TaskID from Count(Tsk_TaskMaster where N_ParentID="+nParentyID+" and N_CompanyID="+nCompanyID+"",Params,connection,transaction);

                        double ParentTaskPercentage = 0.00;

                        foreach (DataRow item in SavedData.Rows)

                        {
                            double n_WeightPercentage = 0.00;
                            double TaskPercentage = 0.00;

                            double workPercentage = 0.00;

                            n_WeightPercentage = myFunctions.getVAL(item["N_WeightPercentage"].ToString());
                            workPercentage = myFunctions.getVAL(item["N_CompletedPercentage"].ToString());

                            TaskPercentage = (n_WeightPercentage * workPercentage) / 100;
                            //  dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET N_CompletedPercentage= "+workPercentage+"  where N_CompanyID=" + nCompanyID + " and N_TaskID=" + nTaskID, Params, connection,transaction);

                            double totalweightage = myFunctions.getVAL(dLayer.ExecuteScalar("select sum(N_WeightPercentage) from Tsk_TaskMaster  where N_ParentID=" + nParentyID + " and N_CompanyID=" + nCompanyID, Params, connection, transaction).ToString());
                            if (totalweightage == 0) { totalweightage = 1; }
                            //ParentTaskPercentage= myFunctions.getVAL(dLayer.ExecuteScalar("select sum(N_CompletedPercentage) from Tsk_TaskMaster  where N_TaskID="+nParentyID+" and N_CompanyID="+nCompanyID, Params, connection,transaction).ToString());
                            ParentTaskPercentage = ParentTaskPercentage + ((TaskPercentage * 100) / totalweightage);




                        }
                        dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET N_CompletedPercentage= " + ParentTaskPercentage + "  where N_CompanyID=" + nCompanyID + " and N_TaskID=" + nParentyID, Params, connection, transaction);




                    }

                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
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
                    //int nparentID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ParentID"].ToString());
                    string nStatus = DetailTable.Rows[0]["N_Status"].ToString();
                    int masterStatus = myFunctions.getIntVAL(DetailTable.Rows[0]["N_Status"].ToString());
                    int nParentyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ParentID"].ToString());
                    double n_UsedWorkHours = myFunctions.getVAL(DetailTable.Rows[0]["n_UsedWorkHours"].ToString());
                    double n_WeightPercentage = myFunctions.getVAL(MasterTable.Rows[0]["n_WeightPercentage"].ToString());
                    double workPercentage = myFunctions.getIntVAL(DetailTable.Rows[0]["x_WorkPercentage"].ToString());
                    double TaskPercentage = 0.00;
                    double ParentTaskPercentage = 0.00;
                    // DateTime dStartDate=DetailTable.Rows[0]["d_TaskStartDate"].ToString();
                    // DateTime dEndDate=DetailTable.Rows[0]["d_TaskEndDate"].ToString();
                   
                    if (DetailTable.Columns.Contains("x_WorkPercentage"))
                    {
                        DetailTable.Columns.Remove("x_WorkPercentage");
                    }
                      if (DetailTable.Columns.Contains("n_UsedWorkHours"))
                    {
                        DetailTable.Columns.Remove("n_UsedWorkHours");
                    }
                  
                    //Percentage Calculation



                    if (myFunctions.getIntVAL(nStatus) == 6 || myFunctions.getIntVAL(nStatus) == 4)
                    {

                        TaskPercentage = (n_WeightPercentage * workPercentage) / 100;
                        dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET N_CompletedPercentage= " + workPercentage + "  where N_CompanyID=" + nCompanyID + " and N_TaskID=" + nTaskID, Params, connection, transaction);
                        if (nParentyID > 0)
                        {
                            double totalweightage = myFunctions.getVAL(dLayer.ExecuteScalar("select sum(N_WeightPercentage) from Tsk_TaskMaster  where N_ParentID=" + nParentyID + " and N_CompanyID=" + nCompanyID, Params, connection, transaction).ToString());
                            ParentTaskPercentage = myFunctions.getVAL(dLayer.ExecuteScalar("select sum(N_CompletedPercentage) from Tsk_TaskMaster  where N_TaskID=" + nParentyID + " and N_CompanyID=" + nCompanyID, Params, connection, transaction).ToString());
                            if (totalweightage == 0) { totalweightage = 1; }
                            ParentTaskPercentage = ParentTaskPercentage + ((TaskPercentage * 100) / totalweightage);
                            dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET N_CompletedPercentage= " + ParentTaskPercentage + "  where N_CompanyID=" + nCompanyID + " and N_TaskID=" + nParentyID, Params, connection, transaction);


                        }



                    }

                    /// all Updates
                    if (nParentyID > 0)
                    {
                        if (myFunctions.getIntVAL(nStatus) == 6 || myFunctions.getIntVAL(nStatus) == 4)
                        {
                            {
                                int ParentID = 0;
                                ParentID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_ParentID from Tsk_TaskMaster  where N_TaskID=" + nParentyID + " and N_CompanyID=" + nCompanyID, Params, connection, transaction).ToString());
                                bool ok;
                                int taskIDNew = nTaskID;
                                if (ParentID > 0) { ok = true; } else { ok = false; }
                                while (ok)
                                {
                                    double totalweightage = myFunctions.getVAL(dLayer.ExecuteScalar("select sum(N_WeightPercentage) from Tsk_TaskMaster  where N_ParentID=" + ParentID + " and N_CompanyID=" + nCompanyID, Params, connection, transaction).ToString());
                                    dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET N_CompletedPercentage= " + totalweightage + "  where N_CompanyID=" + nCompanyID + " and N_TaskID=" + ParentID, Params, connection, transaction);
                                    ParentID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_ParentID from Tsk_TaskMaster  where N_TaskID=" + ParentID + " and N_CompanyID=" + nCompanyID, Params, connection, transaction).ToString());
                                    if (ParentID > 0) { ok = true; } else { ok = false; }
                                }
                            }
                        }
                    }



                    if (myFunctions.getIntVAL(nStatus.ToString()) == 4)
                    {
                        object N_ClosedTaskStatus = dLayer.ExecuteScalar("select count(1) from Tsk_TaskMaster where N_ParentID=" + nTaskID + " and ISNULL(B_Closed,0)=0 and N_CompanyID=" + nCompanyID.ToString(), Params, connection, transaction);
                        int N_Count = myFunctions.getIntVAL(N_ClosedTaskStatus.ToString());
                        if (N_Count > 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Please complete the subtasks........."));
                        }
                    }

                    if (nStatus == "4" && (DetailTable.Rows[0]["N_AssigneeID"].ToString() == DetailTable.Rows[0]["N_SubmitterID"].ToString()) && (DetailTable.Rows[0]["N_AssigneeID"].ToString() == DetailTable.Rows[0]["N_ClosedUserID"].ToString()))
                    {
                        DataRow row = DetailTable.NewRow();
                        row["N_TaskID"] = nTaskID;
                        row["n_TaskStatusID"] = 0;
                        row["n_CompanyId"] = nCompanyID;
                        row["n_AssigneeID"] = DetailTable.Rows[0]["N_AssigneeID"].ToString();
                        row["n_CreaterID"] = myFunctions.GetUserID(User);
                        row["n_SubmitterID"] = DetailTable.Rows[0]["N_AssigneeID"].ToString();
                        row["n_ClosedUserID"] = DetailTable.Rows[0]["N_AssigneeID"].ToString();
                        row["n_Status"] = 5;
                        row["d_EntryDate"] = DetailTable.Rows[0]["d_EntryDate"];
                        DetailTable.Rows.InsertAt(row, 1);
                        masterStatus = 5;
                        //  dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET N_StatusID= 5  where N_CompanyID=" + nCompanyID + " and N_TaskID=" + nTaskID, Params, connection,transaction);
                        dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET B_Closed= 1  where N_CompanyID=" + nCompanyID + " and N_TaskID=" + nTaskID, Params, connection, transaction);
                    }
                    if (nStatus == "4" && (DetailTable.Rows[0]["N_AssigneeID"].ToString() == DetailTable.Rows[0]["N_SubmitterID"].ToString()) && (DetailTable.Rows[0]["N_AssigneeID"].ToString() != DetailTable.Rows[0]["N_ClosedUserID"].ToString()))
                    {
                        DetailTable.Rows[0]["N_AssigneeID"] = DetailTable.Rows[0]["N_ClosedUserID"].ToString();
                        masterStatus = 9;
                        //  dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET N_StatusID= 9  where N_CompanyID=" + nCompanyID + " and N_TaskID=" + nTaskID, Params, connection,transaction);

                    }
                    else if (nStatus == "4" && (DetailTable.Rows[0]["N_AssigneeID"].ToString() != DetailTable.Rows[0]["N_SubmitterID"].ToString()))
                    {
                        if (n_UsedWorkHours>0)
                        {
                          DetailTable.Rows[0]["N_UsedTime"] = n_UsedWorkHours;
                        }
                      
                        DetailTable.Rows[0]["N_AssigneeID"] = DetailTable.Rows[0]["N_SubmitterID"].ToString();

                    }
                    if (nStatus == "4" && (DetailTable.Rows[0]["N_SubmitterID"].ToString() == DetailTable.Rows[0]["N_ClosedUserID"].ToString()))
                    {
                        if (n_UsedWorkHours>0)
                        {
                          DetailTable.Rows[0]["N_UsedTime"] = n_UsedWorkHours;
                        }
                        masterStatus = 9;
                    }
                   

                    if (nStatus == "9" && (DetailTable.Rows[0]["N_AssigneeID"].ToString() == DetailTable.Rows[0]["N_ClosedUserID"].ToString()))
                    {
                        DetailTable.Rows[0]["N_AssigneeID"] = DetailTable.Rows[0]["N_ClosedUserID"].ToString();
                        DetailTable.Rows[0]["n_Status"] = 5;
                        masterStatus = 5;


                        // dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET N_StatusID= 5  where N_CompanyID=" + nCompanyID + " and N_TaskID=" + nTaskID, Params, connection,transaction);
                        dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET B_Closed= 1  where N_CompanyID=" + nCompanyID + " and N_TaskID=" + nTaskID, Params, connection, transaction);
                           
                    }
                    else if (nStatus == "9" && (DetailTable.Rows[0]["N_AssigneeID"].ToString() == DetailTable.Rows[0]["N_SubmitterID"].ToString()))
                    {
                        DetailTable.Rows[0]["N_AssigneeID"] = DetailTable.Rows[0]["N_ClosedUserID"].ToString();
                    }
                    dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET N_StatusID= " + masterStatus + "  where N_CompanyID=" + nCompanyID + " and N_TaskID=" + nTaskID, Params, connection, transaction);

                    if (DetailTable.Columns.Contains("X_Assignee"))
                        DetailTable.Columns.Remove("X_Assignee");
                    if (DetailTable.Columns.Contains("x_Submitter"))
                        DetailTable.Columns.Remove("x_Submitter");
                    if (DetailTable.Columns.Contains("x_ClosedUser"))
                        DetailTable.Columns.Remove("x_ClosedUser");



                    for (int i = 0; i < DetailTable.Rows.Count; i++)
                    {
                        DetailTable.Rows[0]["N_TaskID"] = nTaskID;
                        DetailTable.Rows[0]["N_TaskStatusID"] = 0;
                    }


                    int nTaskStatusID = dLayer.SaveData("Tsk_TaskStatus", "N_TaskStatusID", DetailTable, connection, transaction);
                    if (nTaskStatusID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }
                    if (nStatus == "5")
                    {

                        dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET B_Closed=1 where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), connection, transaction);
                        if (myFunctions.getVAL(MasterTable.Rows[0]["n_StageID"].ToString()) > 0)
                            dLayer.ExecuteNonQuery("Update inv_customerprojects SET N_StageID=" + MasterTable.Rows[0]["n_StageID"].ToString()  + " where N_ProjectID=" + MasterTable.Rows[0]["n_ProjectID"].ToString()  + " and N_CompanyID=" + nCompanyID.ToString(), connection, transaction);

                    }
                    if (masterStatus == 8)
                    {

                        dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET B_Closed=0 where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), connection, transaction);
                    }

                   if (MasterTable.Columns.Contains("d_TaskStartDate"))
                     {

                //    if (MasterTable.Columns.Contains("d_TaskStartDate"))
                //     {
                //         var dStartDate = MasterTable.Rows[0]["d_TaskStartDate"].ToString();
                    
                //       if (MasterTable.Columns.Contains("d_TaskEndDate"))
                //     {
                //         var dEndDate=MasterTable.Rows[0]["d_TaskEndDate"].ToString();
                      
                //     }
                  
                    if (MasterTable.Columns.Contains("d_TaskStartDate"))
                    {
                       if(nStatus == "4" && MasterTable.Columns.Contains("d_TaskEndDate"))
                       {
                        dLayer.ExecuteNonQuery("Update Tsk_TaskStatus SET D_EntryDate= '" + MasterTable.Rows[0]["d_TaskEndDate"].ToString() + "'  where N_CompanyID=" + nCompanyID + " and N_TaskStatusID ="+nTaskStatusID+" and N_TaskID=" + nTaskID, Params, connection, transaction);
                        dLayer.ExecuteNonQuery("Update Tsk_TaskStatus SET D_EntryDate= '" + MasterTable.Rows[0]["d_TaskStartDate"].ToString() + "'  where N_CompanyID=" + nCompanyID + " and N_Status =7 and N_TaskID=" + nTaskID, Params, connection, transaction);
 
                       }
                       else if(nStatus == "7")
                       {
                        dLayer.ExecuteNonQuery("Update Tsk_TaskStatus SET D_EntryDate= '" + MasterTable.Rows[0]["d_TaskStartDate"].ToString() + "'  where N_CompanyID=" + nCompanyID + " and N_Status =7 and N_TaskStatusID ="+nTaskStatusID+" and N_TaskID=" + nTaskID, Params, connection, transaction);
 
                       }
                       
                    }
                     }


                    //   if (masterStatus == 11)
                    //     {

                    //        dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET B_Closed=0 where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), connection, transaction);
                    //     }

                    SortedList Result = new SortedList();
                    Result.Add("n_AssigneeID", DetailTable.Rows[0]["N_AssigneeID"]);
                    dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET d_DueDate='" + MasterTable.Rows[0]["d_DueDate"] + "' where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), connection, transaction);
                    dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET N_CurrentAssigneeID='" + DetailTable.Rows[0]["N_AssigneeID"] + "' where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), connection, transaction);
                    dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET N_CurrentAssignerID='" + DetailTable.Rows[0]["n_CreaterID"] + "' where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), connection, transaction);
                    dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET n_SubmitterID='" + DetailTable.Rows[0]["n_SubmitterID"] + "' where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), connection, transaction);
                    dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET n_ClosedUserID='" + DetailTable.Rows[0]["n_ClosedUserID"] + "' where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), connection, transaction);
                    dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET D_EntryDate='" + DetailTable.Rows[0]["D_EntryDate"] + "' where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), connection, transaction);
                    dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET x_SolutionNotes='" + MasterTable.Rows[0]["x_SolutionNotes"] + "' where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), connection, transaction);
                    if (MasterTable.Columns.Contains("N_WorkHours"))
                    {
                        if (myFunctions.getVAL(MasterTable.Rows[0]["N_WorkHours"].ToString()) > 0)
                        {
                            dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET N_WorkHours=" + MasterTable.Rows[0]["N_WorkHours"] + " where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), connection, transaction);

                        }
                    }

                    transaction.Commit();
                    return Ok(_api.Success(Result, "Task Updated Successfully"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
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
                            return Ok(_api.Error(User, ex));
                        }
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
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
                            return Ok(_api.Error(User, "Unable To Save"));
                        }

                    }
                    if (DetailTable.Rows.Count > 0)
                    {
                        int nTaskStatusID = dLayer.SaveData("Tsk_TaskStatus", "N_TaskStatusID", DetailTable, connection, transaction);
                        if (nTaskStatusID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable To Save"));
                        }




                    }
                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nTaskID, int nFnyearID)
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
                        SqlTransaction transaction = connection.BeginTransaction();

                        myAttachments.DeleteAttachment(dLayer, 1, nTaskID, nTaskID, nFnyearID, 1324, User, transaction, connection);
                        transaction.Commit();
                        return Ok(_api.Success("deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
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

            string sqlCommandText = "Select case when x_ProjectName is null then X_TaskSummery else X_TaskSummery + ' - ' + x_ProjectName end  as title,'true' as allDay,cast(D_TaskDate as Date) as start, dateadd(dd,1,cast(D_DueDate as date)) as 'end', N_TaskID,X_TaskCode  from [vw_TaskCurrentStatus] Where isnull(B_Closed,0) =0 and N_CompanyID= " + nCompanyID + " " + Criteria;


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
                    //return Ok(_api.Notice("No Results Found"));
                    return Ok(_api.Success(dt));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("updateDashboard")]
        public ActionResult UpdateDashboard(int nTaskID, int nStatus, int nProjectID, int nStageID, bool b_Closed)
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

                    // if (nStatus.ToString() == "4")
                    // {
                    //     nStatus = 5;
                    // }



                    int N_TaskStatusID = 0;
                    Params.Add("N_CompanyID", nCompanyID);
                    N_TaskStatusID1 = dLayer.ExecuteScalar("select max(N_TaskStatusID) from Tsk_TaskStatus where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), Params, connection);
                    if (b_Closed)
                    {
                        if (nStatus == 4)
                        {
                            dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET B_Closed=1 where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), Params, connection);
                            //Layer.ExecuteNonQuery("Update inv_customerprojects SET N_StageID=" + nStageID + " where N_ProjectID=" + nProjectID + " and N_CompanyID=" + nCompanyID.ToString(), Params, connection);
                        }
                        // dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET B_Closed=1 where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), Params, connection);
                        if (nStageID > 0)
                            dLayer.ExecuteNonQuery("Update inv_customerprojects SET N_StageID=" + nStageID + " where N_ProjectID=" + nProjectID + " and N_CompanyID=" + nCompanyID.ToString(), Params, connection);


                        SqlTransaction transaction = connection.BeginTransaction();
                        string qry = "Select " + nCompanyID + " as N_CompanyID," + N_TaskStatusID + " as N_TaskStatusID," + nTaskID + " as N_TaskID," + 0 + " as N_AssigneeID," + 0 + " as N_SubmitterID ,'" + N_CreatorID + "' as  N_CreaterID,'" + D_EntryDate + "' as D_EntryDate,'" + "" + "' as X_Notes ," + nStatus + " as N_Status ," + 100 + " as N_WorkPercentage";
                        DetailTable = dLayer.ExecuteDataTable(qry, Params, connection, transaction);
                        int nID = dLayer.SaveData("Tsk_TaskStatus", "N_TaskStatusID", DetailTable, connection, transaction);
                        if (nID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable To Save"));
                        }
                        transaction.Commit();
                        return Ok(_api.Success(""));
                    }
                    else
                    {
                        dLayer.ExecuteNonQuery("Update Tsk_TaskMaster SET B_Closed=0 where N_TaskID=" + nTaskID + " and N_CompanyID=" + nCompanyID.ToString(), Params, connection);
                        dLayer.DeleteData("Tsk_TaskStatus", "N_TaskStatusID", myFunctions.getIntVAL(N_TaskStatusID1.ToString()), "", connection);

                    }
                    return Ok(_api.Success(""));



                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("maildetails")]
        public ActionResult MailDetails()
        {
            DataSet dt = new DataSet();
            DataTable MailData = new DataTable();
            DataTable MasterTable = new DataTable();
            MasterTable = _api.Format(MasterTable, "Master");
            SortedList Params = new SortedList();
            int N_UserID = myFunctions.GetUserID(User);
            int nCompanyId = myFunctions.GetCompanyID(User);
            DateTime datetime = DateTime.Now;
            string X_Body="";
            int N_StatusID=0;
            string sqlCommandText = "select * from vw_TaskDetailsRPT where N_CompanyID=" + nCompanyId + " and N_AssigneeID=" + N_UserID + " and Cast(D_EntryDate as DATE) =  Cast('" + datetime + "' as DATE) and N_CompletedPercentage>0";
            string sqlmailData = "select * from Gen_MailTemplates where N_CompanyID=" + nCompanyId + " and x_templatename='Daily Task'";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(sqlmailData, Params, connection);
                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        double TotalWorkHrs=0;
                        MailData = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                        foreach(DataRow dr in MailData.Rows)
                        {
                           
                                X_Body=X_Body+"*"+dr["X_TaskSummery"] + "- "+dr["N_CompletedPercentage"]+"%<br>";
                                X_Body=X_Body + dr["N_WorkedTime"] + " Hrs ("+dr["N_WorkHours"]+" Hrs)<br>";
                                TotalWorkHrs=TotalWorkHrs+ myFunctions.getVAL(dr["N_WorkedTime"].ToString());
                        }
                        X_Body=X_Body + "<br>Total hours Worked : " + TotalWorkHrs + " Hrs";
                        string x_body=(MasterTable.Rows[0]["x_body"]).ToString();
                        x_body= x_body.Replace("@Body",X_Body);                      
                        x_body= x_body.Replace("@Date",datetime.ToString("dd-MM-yyyy"));                      
                        MasterTable.Rows[0]["x_body"]=x_body;
                        string x_Subject=(MasterTable.Rows[0]["x_Subject"]).ToString();
                        x_Subject=x_Subject.Replace("@Month",datetime.ToString("MMM").ToUpper());
                        MasterTable.Rows[0]["x_Subject"]=x_Subject;


                    }
                    MasterTable.AcceptChanges();


                    MasterTable = _api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                }

                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
    }
    }
    

